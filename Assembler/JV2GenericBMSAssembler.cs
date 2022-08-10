using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace JaiMaker.Assembler
{
    class JV2GenericBMSAssembler : ISequenceAssembler
    {
        public JV2GenericBMSAssembler()
        {
            game = "GENERIC_V2";
        }

        public override void writeBankChange(byte bankID)
        {
            output.Write((byte)0xE2);
            output.Write(bankID);
        }

        public override void writeExtended(int data, params object[] arguments)
        {
            // do nothing. 
            Console.WriteLine($"0x{output.BaseStream.Position:X6} EXT");
        }

        public override void writeFinish()
        {
            Console.WriteLine("EOT?");
            output.Write((byte)0xFF);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} EOT FF");
        }

        public override void writeJump(int address)
        {
            output.Write((byte)0xC7);
            util.writeInt24BE(output,address);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} JMP E7 0x{address:X}");
        }

        public override void writeNop()
        {
            output.Write((byte)0xFE);
        }

        public override void writeNote(bool onOrOff, int note, int vel, byte voice)
        {
            if (onOrOff)
                writeNoteOn(note, vel, voice);
            else
                writeNoteOff(voice);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} NOT {note:X} {vel:X} {voice:X}");
        }

        public override void writeNoteOff(byte voice)
        {
            output.Write((byte)(0x81 + voice));
            Console.WriteLine($"0x{output.BaseStream.Position:X6} NFI {voice:X} LOWER {(0x81 + voice):X}");
        }

        public override void writeNoteOn(int note, int vel, byte voice)
        {
            output.Write((byte)note);
            output.Write((byte)(voice + 1));
            output.Write((byte)vel);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} NNI {note:X} {vel:X} {voice:X}");
        }

        public override void writeOpenTrack(byte trkId, int address)
        {
            output.Write((byte)0xC1);
            output.Write(trkId);
            util.writeInt24BE(output, address);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} OTK C1 {trkId:X} 0x{address:X}");
        }

        public override void writePort(byte port, byte value)
        {
            output.Write((byte)0xD1);
            output.Write(port);
            output.Write(value);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} PUT D1 {port:X} {value:X4}");
        }

        public override void readPort(byte port, byte register)
        {
            output.Write((byte)0xD0);
            output.Write(port);
            output.Write(register);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} LDI D0 {port:X} {register:X}");
        }

        public override void writePrint(string data)
        {
            output.Write((byte)0xFD);
            output.Write(Encoding.ASCII.GetBytes(data));
            output.Write((byte)0x00); // null terminator. 
            Console.WriteLine($"0x{output.BaseStream.Position:X6} PRN FD ds {data}");
        }

        public override void writeProgramChange(byte programID)
        {
            output.Write((byte)0xE3);
            output.Write(programID);
        }

        public override void writeRegister(byte register, short value)
        {
            if (value < 0xFF)
            {
                output.Write((byte)0xB8);
                output.Write(register);
                output.Write((byte)value);
                Console.WriteLine($"0x{output.BaseStream.Position:X6} REW B8 {register:X} {value:X4}");
            } else
            {
                output.Write((byte)0xB9);
                output.Write(register);
                output.Write(value);
                Console.WriteLine($"0x{output.BaseStream.Position:X6} REW B9 {register:X} {value:X4}");
            }
        }

        public override void writeTempoChange(short tempo)
        {
            writeTempoChange(0x62, tempo);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} TCH 62 {tempo:X4}");
        }

        public void writeTempoChange(byte type, short tempo)
        {
            output.Write((byte)0xD8);
            output.Write((byte)type);
            output.Write(tempo);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} TCA D8 {type:X} {tempo:X4}");
        }

        public override void writeTimebaseChange(short timebase)
        {
            output.Write((byte)0xE0);
            output.Write(timebase);
            Console.WriteLine($"0x{output.BaseStream.Position:X6} TBS 62 {timebase:X4}");
        }

        public override void writeTimedEvent(byte param, short time, short value)
        {
            //throw new NotImplementedException();
        }

        public override void writeWait(int delay)
        {
            output.Write((byte)0xF0);
            var vlqBytes = util.getVLQBytes(delay);
            if (vlqBytes.Length > 2)
            {
                Console.WriteLine("Uh oh. Stinky.");
                Console.ReadLine();
            }
            output.BaseStream.Write(vlqBytes,0,vlqBytes.Length);
           // Console.WriteLine($"0x{output.BaseStream.Position:X6} WAI F0 {delay:X5}");
        }

        public override void writeParentPort(byte port, byte value)
        {
            throw new NotImplementedException();
        }

        public override void readParentPort(byte port, byte dest_reg)
        {
            throw new NotImplementedException();
        }

        public override void writePitchBend(short bend)
        {
            throw new NotImplementedException();
        }

        public override void writeVolume(byte volume)
        {
            throw new NotImplementedException();
        }

        public override void writePanning(byte volume)
        {
            throw new NotImplementedException();
        }
    }
}
