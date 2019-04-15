using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MidiSharp;
using System.IO;
using Be.IO;

namespace JaiMaker
{
    static class MidiToBMS
    {
        static MemoryStream[] JaiTracks;
        static int[] DeltaEnds;
        static int[] TrackAddresses;
        public static void doToBMS(MidiSequence wtf, string filename)
        {

            var highest_delta = 0;

            
            JaiTracks = new MemoryStream[wtf.Tracks.Count];
            DeltaEnds = new int[wtf.Tracks.Count];
            TrackAddresses = new int[wtf.Tracks.Count];
            Console.WriteLine("Sequence has {0} tracks.", JaiTracks.Length);

            for (int trk = 0; trk < JaiTracks.Length; trk++)
            {

                int total_delta = 0;
                Stack<byte> voiceStack = new Stack<byte>(8);
                Queue<byte> notehistory = new Queue<byte>(8);
                for (byte v = 7; v > 0; v--)
                {
                    Console.WriteLine("PushVoice {0}", v);
                    voiceStack.Push(v);
                }
                
                byte[] voiceMap = new byte[1024];

                var midTrack = wtf.Tracks[trk];
                JaiTracks[trk] = new MemoryStream();
                var currentTrack = JaiTracks[trk];
               
                var trkWriter = new BeBinaryWriter(currentTrack);
                var lastDelta = 0;

                //A4 20 01(bank 1)
                //A4 21 66(program 66)

                trkWriter.Write((byte)0xA4);
                trkWriter.Write((byte)0x20);
                trkWriter.Write((byte)Root.instrumentBanks[trk]);

                trkWriter.Write((byte)0xA4);
                trkWriter.Write((byte)0x21);
                trkWriter.Write((byte)Root.programs[trk]);

                for (int evt = 0; evt < midTrack.Events.Count; evt++)
                {
                    var cevent = midTrack.Events[evt];
                    
                    total_delta += (int)cevent.DeltaTime;
                    if (total_delta > highest_delta)
                    {
                        highest_delta = total_delta;
                    }
                    if (cevent.DeltaTime > 0)
                    {
                        if (cevent.DeltaTime < 0xFF)
                        {
                            trkWriter.Write((byte)0x80);
                            trkWriter.Write((byte)cevent.DeltaTime);
                        }  else if (cevent.DeltaTime < 0xFFFF)
                        {
                            trkWriter.Write((byte)0x88);
                            trkWriter.Write((ushort)cevent.DeltaTime);
                        } else // dont feel like writing VLQ timing, so i'll just spam u16 waits :V

                        {
                            var total = cevent.DeltaTime;

                            while (total > 0xFFFA)
                            {
                                total -= 0xFFFA;
                                trkWriter.Write((byte)0x88);
                                trkWriter.Write((ushort)0xFFFA);
                            }

                            if (total > 0)
                            {
                                trkWriter.Write((byte)0x88);
                                trkWriter.Write((short)total);
                            }
                        }
                    }
                    if (cevent is MidiSharp.Events.Voice.Note.OnNoteVoiceMidiEvent)
                    {
                        var mevent = (MidiSharp.Events.Voice.Note.OnNoteVoiceMidiEvent)cevent;
                        if (voiceMap[mevent.Note]!=0)
                        {
                          
                            var stopVoice = voiceMap[mevent.Note];
                            if (stopVoice == 0)
                            {
                                Console.WriteLine("VOICE LEAK {0}", trk);

                            }
                            else
                            {
                                voiceStack.Push(stopVoice);
                                trkWriter.Write((byte)(0x80 + stopVoice));
                            }

                        }

                        if (voiceStack.Count < 1)
                        {
                            for (int i=0; i < voiceMap.Length;i++ )
                            {
                                if (voiceMap[i] > 0) {
                                    voiceStack.Push(voiceMap[i]);
                                    voiceMap[i] = 0;
                                        
                                }
                            }
                        }
                        if (voiceStack.Count > 0)
                        {
                            trkWriter.Write(mevent.Note);
                            var useVoice = voiceStack.Pop();
                            voiceMap[mevent.Note] = useVoice;
                            trkWriter.Write(useVoice);
                            trkWriter.Write(mevent.Velocity);
                        } else
                        {
                            Console.WriteLine("Too many voices {0}", trk);
                        }

                    }
                    else if (cevent is MidiSharp.Events.Voice.Note.OffNoteVoiceMidiEvent)
                    {
                        var mevent = (MidiSharp.Events.Voice.Note.OffNoteVoiceMidiEvent)cevent;
                        var stopVoice = voiceMap[mevent.Note];
                        if (stopVoice == 0)
                        {
                            Console.WriteLine("VOICE LEAK {0}", trk);

                        }
                        else
                        {
                            voiceMap[mevent.Note] = 0;
                            voiceStack.Push(stopVoice);
                            trkWriter.Write((byte)(0x80 + stopVoice));
                        }


                    }

                   
                }

                DeltaEnds[trk] = total_delta;
                for (int i=0; i < voiceMap.Length; i++)
                {
                    var stopVoice = voiceMap[i];
                    if (stopVoice > 0)
                    {
                        voiceStack.Push(stopVoice);
                        trkWriter.Write((byte)(0x80 + stopVoice));

                    }
                }

            }

            for (int trk = 0; trk < JaiTracks.Length; trk++)
            {
                var lastD = DeltaEnds[trk];
                var final_wait = highest_delta - lastD;
                var currentTrack = JaiTracks[trk];

                var trkWriter = new BeBinaryWriter(currentTrack);
                trkWriter.Seek(0, SeekOrigin.End);


                var total = final_wait;

                while (total > 0xFFFA)
                {
                    total -= 0xFFFA;
                    trkWriter.Write((byte)0x88);
                    trkWriter.Write((ushort)0xFFFA);
                }

                if (total > 0)
                {
                    trkWriter.Write((byte)0x88);
                    trkWriter.Write((short)total);
                }

            }

            

            var b = new MemoryStream();
            var bw = new BeBinaryWriter(b);
            for ( int trk = 0; trk < JaiTracks.Length; trk++)
            {
                var ctrk = JaiTracks[trk];
                bw.Write((byte)0xC1);
                bw.Write((byte)trk);
                bw.Write((byte)0x00);
                bw.Write((byte)0x00);
                bw.Write((byte)0x00);
                
            }

            bw.Write((byte)0xFD);
            bw.Write((ushort)wtf.TicksPerBeatOrFrame);
            bw.Write((byte)0xFE);
            bw.Write((ushort)Root.Tempo);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);
            bw.Write((byte)0x88);
            bw.Write((ushort)0xFFFA);


            bw.Write((byte)0xFF); // FIN command for now.

            for (int trk = 0; trk < JaiTracks.Length; trk++)
            {
                TrackAddresses[trk] = (int)b.Position;
                var ctrk = JaiTracks[trk];
                var tdata = ReadToEnd(ctrk);
                b.Write(tdata, 0, tdata.Length);
                bw.Write((byte)0xFF); // FIN command for now.
            }


            b.Position = 0;
            for (int trk = 0; trk < JaiTracks.Length; trk++)
            {
                
                var ctrk = JaiTracks[trk];
               
                bw.Write((byte)0xC1);
                bw.Write((byte)trk);
                var ta = TrackAddresses[trk];
                var b1 = (ta) & 0xFF;
                var b2 = (ta >> 8) & 0xFF;
                var b3 = (ta >> 16) & 0xFF; 
                bw.Write((byte)b3);
                bw.Write((byte)b2);
                bw.Write((byte)b1);

            }


            File.WriteAllBytes(filename,ReadToEnd(b));



         
        }


        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = 0;

            if (stream.CanSeek)
            {
                originalPosition = stream.Position;
                stream.Position = 0;
            }

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                if (stream.CanSeek)
                {
                    stream.Position = originalPosition;
                }
            }
        }
    }



}
    