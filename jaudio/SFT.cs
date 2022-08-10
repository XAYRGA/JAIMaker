using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.IO;
using System.Diagnostics;

namespace JaiMaker
{
    public class BinaryStreamMapEntry 
    {
        public string name;
        public int size;
        public int sampleCount;
        public short sampleRate; 
        public short format;
        // 0 
        public short frameRate;
        public bool loop;
        public int loopStart;
        public void loadFromStream(BeBinaryReader read, bool noName = false)
        {
            if (!noName)
                name = Encoding.ASCII.GetString(read.ReadBytes(16)).Replace("\x00",string.Empty);
            size = read.ReadInt32();
            sampleCount = read.ReadInt32();
            sampleRate = read.ReadInt16();
            format = read.ReadInt16();
            read.ReadUInt16(); // unused short. 
            frameRate = read.ReadInt16();
            loop = read.ReadInt32() == 1 ? true : false;
            loopStart = read.ReadInt32();
            read.ReadInt64();
        }

        public void WriteToStream(BeBinaryWriter writer)
        {
            var w = Encoding.ASCII.GetBytes(name);
            for (int i = 0; i < 16; i++)
                if (i < w.Length)
                    writer.Write(w[i]);
                else
                    writer.Write((byte)0);
            writer.Write(size);
            writer.Write(sampleCount);
            writer.Write(sampleRate);
            writer.Write(format);
            writer.Write((short)0);
            writer.Write(frameRate);
            writer.Write(loop ? 1 : 0);
            writer.Write(loopStart);
            writer.Write(0l);
        }

        public static BinaryStreamMapEntry CreateFromStream(BeBinaryReader rd)
        {
            var b = new BinaryStreamMapEntry();
            b.loadFromStream(rd);
            return b;
        }
    }

    public class BinaryStreamMap 
    {
        public int count;
        public BinaryStreamMapEntry[] entries;

        public void loadFromStream(BeBinaryReader read)
        {
            // I don't love you any more. 
            count = (int)(read.BaseStream.Length - 0x10) / 0x30; // read.ReadInt32(); well, technically this is count, but nintendo doesn't seem to use it. Thanks nintendo. 
            read.ReadUInt32(); // skip bytes. 
            read.ReadUInt64();
            read.ReadUInt32(); // skip bytes. 
            Debug.WriteLine($"Start read at 0x{read.BaseStream.Position:X}");
            entries = new BinaryStreamMapEntry[count];
            for (int i = 0; i < count; i++)
                (entries[i] = new BinaryStreamMapEntry()).loadFromStream(read);
            
        }

        public void WriteToStream(BeBinaryWriter writer)
        {
            writer.Write(entries.Length);
            writer.Write(0l);
            writer.Write(0);
            for (int i = 0; i < entries.Length; i++)
                entries[i].WriteToStream(writer);
            writer.Write(0l);
            writer.Write(0l);
        }

        public static BinaryStreamMap CreateFromStream(BeBinaryReader rd)
        {
            var b = new BinaryStreamMap();
            b.loadFromStream(rd);
            return b;
        }

    }

}
