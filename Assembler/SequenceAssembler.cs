using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.IO;


namespace JaiMaker.Assembler
{
    public abstract class ISequenceAssembler
    {

        public string game = "GENERIC";
        public BeBinaryWriter output;

        public void setOutput(BeBinaryWriter wrt)
        {
            output = wrt;
        }

        public abstract void writeNoteOn(int note, int vel, byte voice);
        public abstract void writeNoteOff(byte voice);
        public abstract void writeWait(int delay);
        public abstract void writeNote(bool onOrOff, int note, int vel, byte voice);
        public abstract void writeTimedEvent(byte param, short time, short value);
        public abstract void writeRegister(byte register, short value);
        public abstract void writeTempoChange(short tempo);
        public abstract void writeTimebaseChange(short timebase);
        public abstract void writeOpenTrack(byte trkId, int address);
        public abstract void writeBankChange(byte bankID);
        public abstract void writeProgramChange(byte programID);
        public abstract void writeFinish();
        public abstract void writePort(byte port, byte value);
        public abstract void writeJump(int address);
        public abstract void writeNop();
        public abstract void writeExtended(int data, params object[] arguments);
        public abstract void writePrint(string data);
        public abstract void readPort(byte port, byte register);
        public abstract void writeParentPort(byte port, byte value);
        public abstract void readParentPort(byte port, byte dest_reg);
        public abstract void writePitchBend(short bend);
        public abstract void writeVolume(byte volume);
        public abstract void writePanning(byte volume);
        public abstract void writePitchSensitivity(byte sensitivity);
    }
}
