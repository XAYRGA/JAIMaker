using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.IO;
using System.IO;

namespace JaiMaker
{
    internal class AudioArchive : AudioSystem
    {
        //public JASE SoundTable;
        public List<JInstrumentBankv1> Instruments = new List<JInstrumentBankv1>(); 
        public List<WaveSystem> WaveSystems = new List<WaveSystem>();
        public List<AudioArchiveSectionInfo> Sections = new List<AudioArchiveSectionInfo>();


        public static AudioArchive CreateFromStream(BeBinaryReader rd)
        {
            var a = new AudioArchive();
            a.loadFromStream(rd);
            return a;
        }

        public void loadFromStream(BeBinaryReader rd)
        {
            var go = true;
            while (go)
            {
                var ChunkType = rd.ReadInt32();
                var offset = 0;
                var size = 0;
                var flags = 0;
                switch (ChunkType)
                {
                    case 1:
                    case 5:
                    case 4:
                    case 6:
                    case 7:
                        {
                            offset = rd.ReadInt32();
                            size = rd.ReadInt32();
                            flags = rd.ReadInt32();
                            Sections.Add(new AudioArchiveSectionInfo(ChunkType, offset, size, flags));
                            break;
                        }
                    case 2:
                    case 3:
                        {
                            while (true)
                            {

                                offset = rd.ReadInt32();
                                if (offset == 0)
                                    break;
                                size = rd.ReadInt32();
                                flags = rd.ReadInt32();

                                Sections.Add(new AudioArchiveSectionInfo(ChunkType,offset,size,flags));
                            }
                            break;
                       }
                    case 0:
                        go = false;
                        break;
                }
            }
     
            for (int i=0; i < Sections.Count; i++)
            {
                var sect = Sections[i];
                rd.BaseStream.Position = sect.offset;
                sect.stream = new MemoryStream(rd.ReadBytes(sect.size));
                sect.reader = new BeBinaryReader(sect.stream);
                sect.writer = new BeBinaryWriter(sect.stream);

                switch (sect.type)
                {
                    case 3:
                        WaveSystems.Add(WaveSystem.CreateFromStream(sect.reader));
                        break;
                    case 2:
                        Instruments.Add(JInstrumentBankv1.CreateFromStream(sect.reader));
                        break;
                }
            }
        }
    }

    internal class AudioArchiveSectionInfo
    {
        public int type;
        public int offset;
        public int size;
        public int flags;
        public Stream stream;
        public BeBinaryReader reader;
        public BeBinaryWriter writer;
        public object obj;

        public AudioArchiveSectionInfo(int type, int offset, int size, int flags)
        {
            this.type = type;
            this.offset = offset;
            this.size = size;
            this.flags = flags;
        }
    }
}
