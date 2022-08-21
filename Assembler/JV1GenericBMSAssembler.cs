using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JaiMaker.Assembler
{
    class JV1GenericBMSAssembler : ISequenceAssembler
    {
        public JV1GenericBMSAssembler()
        {
            game = "GENERIC_V1";
        }
        public override void readParentPort(byte port, byte dest_reg)
        {
            throw new NotImplementedException();
        }

        public override void readPort(byte port, byte register)
        {
            throw new NotImplementedException();
        }

        public override void writeBankChange(byte bankID)
        {
            output.Write((byte)0xA4);
            output.Write((byte)0x20);
            output.Write(bankID);
        }

        public override void writeExtended(int data, params object[] arguments)
        {
            throw new NotImplementedException();
        }

        public override void writeFinish()
        {
            output.Write((byte)0xFF);
        }

        public override void writeJump(int address)
        {
            output.Write((byte)0xC8);
            output.Write(address);
            //throw new NotImplementedException();
        }

        public override void writeNop()
        {
            output.Write((byte)0xFC);
        }

        public override void writeNote(bool onOrOff, int note, int vel, byte voice)
        {
            throw new NotImplementedException();
        }

        public override void writeNoteOff(byte voice)
        {
            output.Write((byte)(0x81 + voice));
            //throw new NotImplementedException();
        }

        public override void writeNoteOn(int note, int vel, byte voice)
        {

            output.Write((byte)note);
            output.Write((byte)(voice + 1));
            output.Write((byte)vel);
            //throw new NotImplementedException();
        }

        public override void writeOpenTrack(byte trkId, int address)
        {
            output.Write((byte)0xC1);
            output.Write(trkId);
            util.writeInt24BE(output, address);
        }

        public override void writePanning(byte volume)
        {
            //throw new NotImplementedException();
        }

        public override void writeParentPort(byte port, byte value)
        {
            throw new NotImplementedException();
        }

        public override void writePitchBend(short bend)
        {
            output.Write((byte)0x9C);
            output.Write((byte)1);
            output.Write(bend);
        }

        public override void writePitchSensitivity(byte sensitivity)
        {
            output.Write((byte)0xA4);
            output.Write((byte)7);
            output.Write(sensitivity);
        }

        public override void writePort(byte port, byte value)
        {
            //throw new NotImplementedException();
        }

        public override void writePrint(string data)
        {
            output.Write((byte)0xFB);
            output.Write(Encoding.ASCII.GetBytes(data));
            output.Write((byte)0x00); // null terminator. 
        }

        public override void writeProgramChange(byte programID)
        {
            output.Write((byte)0xA4);
            output.Write((byte)0x21);
            output.Write((byte)programID);
        }

        public override void writeRegister(byte register, short value)
        {
            throw new NotImplementedException();
        }

        public override void writeTempoChange(short tempo)
        {
            output.Write((byte)0xFD);
            output.Write(tempo);
        }

        public override void writeTimebaseChange(short timebase)
        {
            output.Write((byte)0xFE);
            output.Write(timebase);
        }

        public override void writeTimedEvent(byte param, short time, short value)
        {
            //throw new NotImplementedException();
        }

        public override void writeVolume(byte volume)
        {
            //output.Write((byte)0x94); // U8 NODUR
            //output.Write((byte)0); // 0 is volume
            //output.Write((byte)(volume * 2)); // volume value

            output.Write((byte)0x9C);
            output.Write((byte)0);
            output.Write((ushort)(((float)volume / (float)0x7F) * 16383f));

        }

        public override void writeWait(int delay)
        {
            var delta = delay;
            var JaiWriter = output;
            if (delta < 0xFF) // 8-bit wait
            {
                JaiWriter.Write((byte)0x80); // 8 bit wait command
                JaiWriter.Write((byte)delta);  // write delta
            }
            else if (delta < 0xFFFF) // 16 bit wait
            {
                JaiWriter.Write((byte)0x88);
                JaiWriter.Write((ushort)delta);
            }
            else // dont feel like writing VLQ timing, so i'll just spam u16 waits :V
            { // VLQ wait.
                var total = delta;
                while (total > 0xFFFA)
                {
                    total -= 0xFFFA;
                    JaiWriter.Write((byte)0x88);
                    JaiWriter.Write((ushort)0xFFFA);
                }
                if (total > 0)
                {
                    JaiWriter.Write((byte)0x88);
                    JaiWriter.Write((ushort)total);
                }
            }
        }

    }
}
