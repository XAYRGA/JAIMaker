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




    public class JAIMakerProjectFileV2 : JAIMakerProjectFile
    {
        public int[] banks;
        public int[] programs;
        public int[] volumes;
        public int tempo;
        public Dictionary<int, JAIMakerSoundInfo> Remap;
        public JAIMakerProjectFileV2()
        {
            Version = 2;
            volumes = new int[16];
        }

        public override void load(BinaryReader reader)
        {
            if (reader.ReadInt32() != JAIM)
                throw new InvalidDataException("Not a valid JAIM file");
            var version = reader.ReadInt32();
            switch (version) {
                case 1:
                    {
                        Console.WriteLine("Upgrading jaimaker v1 file...");
                        reader.BaseStream.Position = 0;
                        var ojm = new JAIMakerProjectFileV1();
                        ojm.load(reader);
                        banks = ojm.banks;
                        programs = ojm.programs;
                        Remap = ojm.Remap;
                        return;
                    }
                case 2:
                    break;// Current version of file.                
                default:
                    throw
                        new InvalidDataException("JAIM file is too new for this version of JAIMaker");
            }
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

            for (int i = 0; i < remapCount; i++)
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

            for (int i=0; i < 16; i++)
                volumes[i] = reader.ReadInt32();

            tempo = reader.ReadInt32();
         

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
            for (int i = 0; i < volumes.Length; i++)
                writer.Write(volumes[i]);

            writer.Write(tempo);
        }
    }


    public class JAIMakerProjectFileV3 : JAIMakerProjectFile
    {
        public int[] banks;
        public int[] programs;
        public int[] volumes;
        public int[] offsets; 
        public int tempo;
        public Dictionary<int, JAIMakerSoundInfo> Remap;
        public JAIMakerProjectFileV3()
        {
            Version = 3;
            volumes = new int[16];
            offsets = new int[16];
        }

        public override void load(BinaryReader reader)
        {
            if (reader.ReadInt32() != JAIM)
                throw new InvalidDataException("Not a valid JAIM file");
            var version = reader.ReadInt32();
            switch (version)
            {
                case 1:
                    {
                        Console.WriteLine("Upgrading jaimaker v1 file...");
                        reader.BaseStream.Position = 0;
                        var ojm = new JAIMakerProjectFileV1();
                        ojm.load(reader);
                        banks = ojm.banks;
                        programs = ojm.programs;
                        Remap = ojm.Remap;
                        return;
                    }
                case 2:
                    {
                        Console.WriteLine("Upgrading jaimaker v2 file...");
                        reader.BaseStream.Position = 0;
                        var ojm = new JAIMakerProjectFileV2();
                        ojm.load(reader);
                        banks = ojm.banks;
                        programs = ojm.programs;
                        Remap = ojm.Remap;
                        volumes = ojm.volumes;
                        return;
                    }
                case 3:
                    break;
                default:
                    throw
                        new InvalidDataException("JAIM file is too new for this version of JAIMaker");
            }
            var bankCount = reader.ReadInt32();
            var progCount = reader.ReadInt32();
            var remapCount = reader.ReadInt32();
            var volumeCount = reader.ReadInt32();
            var offsetCount = reader.ReadInt32();
            banks = new int[bankCount];
            programs = new int[progCount];
            Remap = new Dictionary<int, JAIMakerSoundInfo>();
            for (int i = 0; i < bankCount; i++)
                banks[i] = reader.ReadInt32();

            for (int i = 0; i < progCount; i++)
                programs[i] = reader.ReadInt32();

            for (int i = 0; i < remapCount; i++)
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

            for (int i = 0; i < volumeCount; i++)
                volumes[i] = reader.ReadInt32();


            for (int i = 0; i < offsetCount; i++)
                offsets[i] = reader.ReadInt32();

            tempo = reader.ReadInt32();
        }
        public override void save(BinaryWriter writer)
        {
            writer.Write(JAIM);
            writer.Write(Version);
            writer.Write(banks.Length);
            writer.Write(programs.Length);
            writer.Write(Remap.Count);
            writer.Write(volumes.Length);
            writer.Write(offsets.Length);

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
            for (int i = 0; i < volumes.Length; i++)
                writer.Write(volumes[i]);

            for (int i = 0; i < offsets.Length; i++)
                writer.Write(offsets[i]);

            writer.Write(tempo);
        }
    }


    public class JAIMakerProjectFileV4 : JAIMakerProjectFile
    {
        public int[] banks;
        public int[] programs;
        public int[] volumes;
        public int[] offsets;
        public bool[] dynamics;
        public int tempo;
        public Dictionary<int, JAIMakerSoundInfo> Remap;
        public JAIMakerProjectFileV4()
        {
            Version = 4;
            volumes = new int[16];
            offsets = new int[16];
            dynamics = new bool[16];
        }

        public override void load(BinaryReader reader)
        {
            if (reader.ReadInt32() != JAIM)
                throw new InvalidDataException("Not a valid JAIM file");
            var version = reader.ReadInt32();
            switch (version)
            {
                case 1:
                    {
                        Console.WriteLine("Upgrading jaimaker v1 file...");
                        reader.BaseStream.Position = 0;
                        var ojm = new JAIMakerProjectFileV1();
                        ojm.load(reader);
                        banks = ojm.banks;
                        programs = ojm.programs;
                        Remap = ojm.Remap;
                        return;
                    }
                case 2:
                    {
                        Console.WriteLine("Upgrading jaimaker v2 file...");
                        reader.BaseStream.Position = 0;
                        var ojm = new JAIMakerProjectFileV2();
                        ojm.load(reader);
                        banks = ojm.banks;
                        programs = ojm.programs;
                        Remap = ojm.Remap;
                        volumes = ojm.volumes;
                        return;
                    }
                case 3:
                    {
                        Console.WriteLine("Upgrading jaimaker v3 file...");
                        reader.BaseStream.Position = 0;
                        var ojm = new JAIMakerProjectFileV3();
                        ojm.load(reader);
                        banks = ojm.banks;
                        programs = ojm.programs;
                        Remap = ojm.Remap;
                        volumes = ojm.volumes;
                        offsets = ojm.offsets;
                        return;
                    }
                case 4:
                    break;
                default:
                    throw
                        new InvalidDataException("JAIM file is too new for this version of JAIMaker");
            }
            var bankCount = reader.ReadInt32();
            var progCount = reader.ReadInt32();
            var remapCount = reader.ReadInt32();
            var volumeCount = reader.ReadInt32();
            var offsetCount = reader.ReadInt32();
            var dynamicCount = reader.ReadInt32();

            banks = new int[bankCount];
            programs = new int[progCount];
            Remap = new Dictionary<int, JAIMakerSoundInfo>();
            for (int i = 0; i < bankCount; i++)
                banks[i] = reader.ReadInt32();

            for (int i = 0; i < progCount; i++)
                programs[i] = reader.ReadInt32();

            for (int i = 0; i < remapCount; i++)
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

            for (int i = 0; i < volumeCount; i++)
                volumes[i] = reader.ReadInt32();


            for (int i = 0; i < offsetCount; i++)
                offsets[i] = reader.ReadInt32();

            for (int i = 0; i < dynamicCount; i++)
                dynamics[i] = reader.ReadBoolean();

            tempo = reader.ReadInt32();
        }
        public override void save(BinaryWriter writer)
        {
            writer.Write(JAIM);
            writer.Write(Version);
            writer.Write(banks.Length);
            writer.Write(programs.Length);
            writer.Write(Remap.Count);
            writer.Write(volumes.Length);
            writer.Write(offsets.Length);
            writer.Write(dynamics.Length);

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
            for (int i = 0; i < volumes.Length; i++)
                writer.Write(volumes[i]);

            for (int i = 0; i < offsets.Length; i++)
                writer.Write(offsets[i]);

            for (int i=0; i < dynamics.Length; i++)            
                writer.Write(dynamics[i]);            

            writer.Write(tempo);
        }
    }

}
