using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.IO;
using System.IO;

namespace JaiMaker
{

    /* 
     
        byte sflags
	        0x80 = Start Paused  
        byte pflags
        byte uflags1
        byte uflags2 
        byte type 
        byte loadMode 
        bool(short) is_dummy // tells if the sound is empty
        float pitch  // Doesnt seem to affect sequences
        short volume // Doesn't seem to affect sequences 
        short padding // Padding 
    */


    public class JASESound
    {
        public int entryNumCategory;
        public int entryNum = 0;
        public int entryNumGlobal = 0;
        public string name;  
        public byte sflags;
        public byte pflags;
        public byte uflags1;
        public byte uflags2;
        public byte type;
        public byte loadMode;
        public ushort unk3;

        public float pitch;  // Doesnt seem to affect sequences
        public ushort volume; // Doesn't seem to affect sequences 


        private string readName(BeBinaryReader aafRead)
        {
            var ofs = aafRead.BaseStream.Position; // Store where we started 
            byte nextbyte; // Blank byte
            byte[] name = new byte[0x70]; // Array for the name

            int count = 0; // How many we've done
            while ((nextbyte = aafRead.ReadByte()) != 0xFF & nextbyte != 0x00) // Read until we've read 0 or FF
            {
                name[count] = nextbyte; // Store into byte array
                count++; // Count  how many valid bytes  we've read.
            }
            aafRead.BaseStream.Seek(ofs + 0x1C, SeekOrigin.Begin); // Seek 0x1C bytes, because thats the statically allocated space for the wavegroup path. 
            return Encoding.ASCII.GetString(name, 0, count); // Return a string with the name, but only of the valid bytes we've read. 
        }


        public void readInfo(BeBinaryReader reader, bool nametable = false)
        {
            if (nametable)
            {
                name = readName(reader);
                reader.ReadUInt16();
                reader.ReadUInt16();
            } else
            {
                sflags = reader.ReadByte();
                pflags = reader.ReadByte();
                uflags1 = reader.ReadByte();
                uflags2 = reader.ReadByte();
                type = reader.ReadByte();
                loadMode = reader.ReadByte();
                unk3 = reader.ReadUInt16();     
                pitch = reader.ReadSingle();
                volume = reader.ReadUInt16();
                reader.ReadUInt16(); // padding. 
            }
        }
        // Padding short
    }
    public class JASECategory
    {
        public string name = "";
        public ushort count;
        public ushort startID;
        public JASESound[] sounds;
        public byte index;

        public void readInfo(BeBinaryReader reader, bool nametable = false)
        {
            if (nametable)
                name = readName(reader);
       
            count = reader.ReadUInt16();
            startID = reader.ReadUInt16();

            sounds = new JASESound[count];
            Console.WriteLine($"{startID:X}");
        }

        private string readName(BeBinaryReader aafRead)
        {
            var ofs = aafRead.BaseStream.Position; // Store where we started 
            byte nextbyte; // Blank byte
            byte[] name = new byte[0x70]; // Array for the name

            int count = 0; // How many we've done
            while ((nextbyte = aafRead.ReadByte()) != 0xFF & nextbyte != 0x00) // Read until we've read 0 or FF
            {
                name[count] = nextbyte; // Store into byte array
                count++; // Count  how many valid bytes  we've read.
            }
            aafRead.BaseStream.Seek(ofs + 0x1C, SeekOrigin.Begin); // Seek 0x1C bytes, because thats the statically allocated space for the wavegroup path. 
            return Encoding.ASCII.GetString(name, 0, count); // Return a string with the name, but only of the valid bytes we've read. 
        }

        public void loadWaves(BeBinaryReader reader, bool nametable = false)
        {
            for (int i=0; i < count; i++)
            {
                sounds[i] = new JASESound();
                sounds[i].readInfo(reader, nametable);
            }
        }
    }

    public class JASE
    {
        public JASECategory[] Categories = new JASECategory[0x12];
        public int u1;
        public int u2;
        public int SoundCount = 0;

        public void readInfo(BeBinaryReader reader, bool nametable = false)
        {
            u1 = reader.ReadUInt16();
            u2 = reader.ReadUInt16();
            SoundCount = reader.ReadUInt16();            
        }


        public void loadCategories(BeBinaryReader reader, bool nametable = false)
        {
            for (byte i=0; i < 0x12; i++)
            {
                Categories[i] = new JASECategory();
                Categories[i].readInfo(reader, nametable);
                Categories[i].index = i;
            }
            reader.ReadUInt16(); // padding.
        }
    }
}
