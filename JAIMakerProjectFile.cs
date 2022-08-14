using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace JaiMaker
{
    public abstract class JAIMakerProjectFile
    {
        public const int JAIM = 0x4D49414A;
        public int Version;

        public abstract void load(BinaryReader reader);
        public abstract void save(BinaryWriter writer);
    }


    public class JAIMakerProjectFileV1 : JAIMakerProjectFile
    {
        public int[] banks;
        public int[] programs;
        public Dictionary<int, JAIMakerSoundInfo> Remap;
        public JAIMakerProjectFileV1()
        {
            Version = 1;
        }

        public override void load(BinaryReader reader)
        {
            if (reader.ReadInt32() != JAIM)
                throw new InvalidDataException("Not a valid JAIM file");
            if (reader.ReadInt32() > 1)
                throw new InvalidDataException("JAIM file is too new for this version of JAIMaker");
            var bankCount = reader.ReadInt32();
            var progCount = reader.ReadInt32();
            var remapCount = reader.ReadInt32();
            banks = new int[bankCount];
            programs = new int[progCount];
            Remap = new Dictionary<int, JAIMakerSoundInfo>();
            for (int i = 0; i < bankCount; i++)
                banks[i] = reader.ReadInt32();

            for (int i = 0; i < progCount; i++)
                programs[i] = reader.ReadInt32();

            for (int i=0; i < remapCount; i++)
            {
                var midiProg = reader.ReadInt32();
                var name = reader.ReadString();
                var bank = reader.ReadInt32();
                var prog = reader.ReadInt32();
                Remap[midiProg] = new JAIMakerSoundInfo()
                {
                    prog = prog,
                    bank = bank,
                    name = name,
                };
            }

        }
        public override void save(BinaryWriter writer)
        {
            writer.Write(JAIM);
            writer.Write(Version);
            writer.Write(banks.Length);
            writer.Write(programs.Length);
            writer.Write(Remap.Count);

            for (int i = 0; i < banks.Length; i++)
                writer.Write(banks[i]);

            for (int i = 0; i < programs.Length; i++)
                writer.Write(programs[i]);

            foreach (KeyValuePair<int, JAIMakerSoundInfo> kvp in Remap)
            {
                writer.Write(kvp.Key);
                writer.Write(kvp.Value.name);
                writer.Write(kvp.Value.bank);
                writer.Write(kvp.Value.prog);
            }          
        }
    }
}
