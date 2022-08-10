﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JaiMaker.Assembler;
using MidiSharp;

using Be.IO;

namespace JaiMaker
{
    class MidiToBMSAssembler
    {
        public ISequenceAssembler Assembler;
        public MidiSequence MidiSeq;
        public Dictionary<int, JAIMakerSoundInfo> MIDIInstrumentRemap;

        public BeBinaryWriter output;

        public MidiToBMSAssembler(MidiSequence MIDI, ISequenceAssembler ASS)
        {
            MidiSeq = MIDI;
            Assembler = ASS;
        }

        #region Address Storage / Call+Jump management
        private Dictionary<string, int> addressLookup = new Dictionary<string, int>();
        private void saveAddress(string name)
        {
            addressLookup[name] = (int)Assembler.output.BaseStream.Position;
        }
        private int getAddress(string name)
        {
            int outaddr;
            if (!addressLookup.TryGetValue(name, out outaddr))
                return -1;
            return outaddr;
        }

        private void goAddress(string name, short offset = 0)
        {
            var addr = getAddress(name);
            if (addr != -1)
                Assembler.output.BaseStream.Position = addr + offset;
            else
                throw new Exception($"Return position miss {name}");
        }

        private void clearAddress(string name)
        {
            addressLookup.Remove(name);
        }
        #endregion

        #region  Remapping Functions
  
        private int calculateEndingDelta(MidiSequence midSeq)
        {
            var largest_delta = 0;
            for (int tridx = 0; tridx < midSeq.Tracks.Count; tridx++)
            {
                var total_trk_delta = 0; // Total delta for current track 
                var CTrk = midSeq.Tracks[tridx]; // Current track object
                for (int evntid = 0; evntid < CTrk.Events.Count; evntid++) // Iterate through events
                {
                    var CEvent = CTrk.Events[evntid]; // Current event object
                    total_trk_delta += (int)CEvent.DeltaTime; // Add the event to the total delta for our track 
                }
                if (total_trk_delta > largest_delta) // We want to know what our highest delta is so we can make all the tracks end at the same time. 
                    largest_delta = total_trk_delta;  // So we should store it if it's greater than the last
            }
            return largest_delta;
        }


        private int[] voiceLookup;
        private int allocateVoice(int note)
        {
            for (int i = 0; i < 7; i++)
                if (voiceLookup[i] == 0)
                {
                    voiceLookup[i] = note;
                    return i;
                }
            return -1;
        }

        private int isVoiceAllocated(int note)
        {
            for (int i = 0; i < 7; i++)
                if (voiceLookup[i] == note)                
                    return i;

            return -1;
        }

        private int freeVoice(int note)
        {
            for (int i = 0; i < 7; i++)
                if (voiceLookup[i] == note)
                {
                    voiceLookup[i] = 0;
                    return i;
                }
            return -1;
        }

        public void processSequence()
        {
            var endDelta = calculateEndingDelta(MidiSeq);


            //Assembler.writePrint("Sequence generated by JaiMaker-2!");
            Assembler.writeTempoChange((short)MidiSeq.TicksPerBeatOrFrame);
            Console.WriteLine($"Assembler ticks per frame {(short)MidiSeq.TicksPerBeatOrFrame}");
            Assembler.writeTimebaseChange((short)120);

            for (int trk = 0; trk < MidiSeq.Tracks.Count; trk++)
            {
                if (trk == 0)
                    continue;
                saveAddress($"trk{trk}Open");
                Assembler.writeOpenTrack((byte)trk, 0);
            }

            for (int trk = 0; trk < MidiSeq.Tracks.Count; trk++) // 
            {
                voiceLookup = new int[7];
                if (trk != 0) // root track doesn't need opening.
                {
                    saveAddress("last"); // Save previous address (where we're opening from)
                    goAddress($"trk{trk}Open"); // Go back to header and write the address where that track starts
                    Assembler.writeOpenTrack((byte)(trk ), getAddress($"last"));
                    clearAddress($"trk{trk}Open");
                    goAddress("last"); // Reuturn
                }
                writeTrack(MidiSeq.Tracks[trk], (byte)trk, endDelta);
                Console.WriteLine($"Finished assembling TRK{trk:X2}");
            }

            Assembler.writeFinish();
        }

        public void writeTrack(MidiTrack mTrack, byte trackID, int lastDelta)
        {

            int currentInst = 0;
            int currentBnk = 0;
            long totalDelta = 0;

            int rpn = 4; // Pitchwheel range. 

            Assembler.writeBankChange((byte)Root.instrumentBanks[trackID]); // 0xa4 0x20 0xYY
            Assembler.writeProgramChange((byte)Root.programs[trackID]); // 0xA4 0x21 0xYY ??

            for (int i = 0; i < mTrack.Events.Count; i++)
            {
                var currentEvent = mTrack.Events[i];
                totalDelta += currentEvent.DeltaTime;

                if (currentEvent.DeltaTime > 0)
                    Assembler.writeWait((int)currentEvent.DeltaTime);

                if (currentEvent is MidiSharp.Events.Voice.Note.OnNoteVoiceMidiEvent)
                {
                    var ev = (MidiSharp.Events.Voice.Note.OnNoteVoiceMidiEvent)currentEvent;

                    var alloc = isVoiceAllocated(ev.Note);
                    if (alloc > -1)
                    {
                        Assembler.writeNoteOff((byte)alloc);
                        freeVoice(ev.Note);
                    }

                    if (ev.Velocity > 0)
                    {
                        var voice = allocateVoice(ev.Note);
                        if (voice > -1)
                            Assembler.writeNoteOn(ev.Note, ev.Velocity, (byte)voice);
                        else
                            Console.WriteLine($"! Voice overflow on track {trackID}");
                    }
                }
                else if (currentEvent is MidiSharp.Events.Voice.Note.OffNoteVoiceMidiEvent)
                {
                    var ev = (MidiSharp.Events.Voice.Note.OffNoteVoiceMidiEvent)currentEvent;
                    //ev.Note = (byte)getNoteRemap(currentBnk, currentInst, ev.Note);
                    var voiceFree = freeVoice(ev.Note);
                    if (voiceFree > -1)
                        Assembler.writeNoteOff((byte)voiceFree);
                    else
                        Console.WriteLine($"! Cannot stop voice with ID {voiceFree} because it isn't playing...");
                }
                else if (currentEvent is MidiSharp.Events.Meta.TempoMetaMidiEvent)
                {
                    var ev = (MidiSharp.Events.Meta.TempoMetaMidiEvent)currentEvent;
                    Assembler.writeTimebaseChange((short)(60000000 / ev.Value));
                }
                else if (currentEvent is MidiSharp.Events.Voice.ProgramChangeVoiceMidiEvent)
                {
                    var ev = (MidiSharp.Events.Voice.ProgramChangeVoiceMidiEvent)currentEvent;
                    /*
                 
                    if (Project.UseMidiOverride || !Project.UseMidiRemap)
                        continue;
                    var ovr = getMidiProgRemap(ev.Number);
                    if (!ovr.enable)
                        continue;
                    Assembler.writeBankChange((byte)ovr.bank); // 0xa4 0x20 0xYY
                    Assembler.writeProgramChange((byte)ovr.program); // 0xA4 0x21 0xYY ??
                    currentInst = ovr.program;
                    currentBnk = ovr.bank;
                    */

                    if (MIDIInstrumentRemap != null && MIDIInstrumentRemap.ContainsKey(ev.Number))
                    {
                        var remap = MIDIInstrumentRemap[ev.Number];
                        Assembler.writeBankChange((byte)remap.bank); // 0xa4 0x20 0xYY
                        Assembler.writeProgramChange((byte)remap.prog); // 0xA4 0x21 0xYY ??
                    }
                }
                else if (currentEvent is MidiSharp.Events.Voice.PitchWheelVoiceMidiEvent)
                {
                    var ev = (MidiSharp.Events.Voice.PitchWheelVoiceMidiEvent)currentEvent;
                    var RealValue = 128 * ev.LowerBits + ev.UpperBits; // MidiSharp calculates this wrong, in MIDI it's big endian.
                    Assembler.writePitchBend((short)((RealValue - 8192))); 
                }
                else if (currentEvent is MidiSharp.Events.Voice.ControllerVoiceMidiEvent)
                {

                    var ev = (MidiSharp.Events.Voice.ControllerVoiceMidiEvent)currentEvent;
                    if (ev.Number == (byte)Controller.VolumeCourse)
                        Assembler.writeVolume(ev.Value);
                    else if (ev.Number == (byte)Controller.VolumeFine)
                        Assembler.writeVolume(ev.Value);
                    else if (ev.Number == (byte)Controller.PanPositionCourse)
                        Assembler.writePanning(ev.Value);
                    else if (ev.Number == (byte)Controller.PanPositionFine)
                        Assembler.writePanning(ev.Value);
                } else if (currentEvent is MidiSharp.Events.Meta.Text.CuePointTextMetaMidiEvent)
                {
                    var ev = (MidiSharp.Events.Meta.Text.CuePointTextMetaMidiEvent)currentEvent;
                    if (ev.Text == "JLOOP" || ev.Text == "LOOP")
                        saveAddress("LOOP");
                }
            }
            Assembler.writeWait((int)(lastDelta - totalDelta)); // Synchronize the ending of all tracks
            Assembler.writeFinish(); // close track.
        }

        #endregion
    }
}















