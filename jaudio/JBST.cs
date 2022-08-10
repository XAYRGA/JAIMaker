using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Be.IO;

namespace JaiMaker
{
    public static class JBSTParseUtil {
        public static string readNameAnchored(BeBinaryReader reader, int name_address)
        {
            var anchor = reader.BaseStream.Position;
            reader.BaseStream.Position = name_address;
            var ret = readTerminated(reader, 0x00);
            reader.BaseStream.Position = anchor;
            return ret;
        }

        public static void moveBack(BeBinaryReader stream, int amt)
        {
            stream.BaseStream.Position -= amt;
        }
        public static int[] readInt32Array(BeBinaryReader binStream, int count)
        {
            var b = new int[count];
            for (int i = 0; i < count; i++)
                b[i] = binStream.ReadInt32();

            return b;
        }

        public static string readTerminated(BeBinaryReader rd, byte term)
        {
            int count = 0;
            int nextbyte = 0xFFFFFFF;
            byte[] name = new byte[0xFF];
            while ((nextbyte = rd.ReadByte()) != term)
            {
                // Console.WriteLine(nextbyte);
                name[count] = (byte)nextbyte;
                count++;
            }
            return Encoding.ASCII.GetString(name, 0, count);
        }

        public static uint readUInt24BE(BinaryReader reader)
        {
            try
            {
                var b1 = reader.ReadByte();
                var b2 = reader.ReadByte();
                var b3 = reader.ReadByte();
                return
                    (((uint)b1) << 16) |
                    (((uint)b2) << 8) |
                    ((uint)b3);
            }
            catch
            {
                return 0u;
            }
        }

        public static void mergeJBST_JBSTN(JBST bst, JBST bstn)
        {
            bst.name = bstn.name;
            for (int catIndex = 0; catIndex < bst.categories.Length; catIndex++)
            {
                var bstCat = bst.categories[catIndex];
                var nameCat = bstn.categories[catIndex];
                bstCat.name = nameCat.name;
                for (int libIndex = 0; libIndex < bstCat.libraries.Length; libIndex++)
                {
                    var catLib = bstCat.libraries[libIndex];
                    var nameLib = nameCat.libraries[libIndex];
                    catLib.name = nameLib.name;
                    for (int soundIndex = 0; soundIndex < catLib.sounds.Length; soundIndex++)
                    {

                        var libSound = catLib.sounds[soundIndex];
                        if (libSound != null)
                        {
                            var nameSound = nameLib.sounds[soundIndex];
                            libSound.name = nameSound.name;
                        }
                    }
                }
            }
        }
    }
    public class JBST
    {
        private const int HEAD_BSTN = 0x4253544E;
        private const int HEAD_BST = 0x42535420;

        public string name;
        public bool bstn = false;
        public int version;
        public JBSTCategory[] categories;

        
        public int mOffset;
        
        public int NameOffset;


        public static JBST LoadFromStream(BeBinaryReader stream)
        {
            var nBST = new JBST();
            var header = stream.ReadInt32();
            var isBSTN = false;
            switch (header)
            {
                case HEAD_BST:
                    isBSTN = false;
                    break;
                case HEAD_BSTN:
                    isBSTN = true;
                    break;
                default:
                    throw new FormatException($"Unrecognized format {header:X6}");
            }
            nBST.bstn = isBSTN;
            stream.ReadInt32();
            var version = stream.ReadInt32();
            if (version != 0x01000000)
                throw new InvalidDataException($"Version is not 0x01000000! ({version})");
            var categoryTableOffset = stream.ReadInt32();
            stream.BaseStream.Position = categoryTableOffset;
            var categoryCount = stream.ReadInt32();
            var sectionPointers = JBSTParseUtil.readInt32Array(stream, categoryCount);
            nBST.categories = new JBSTCategory[categoryCount];
            for (int i = 0; i < categoryCount; i++)
            {
                stream.BaseStream.Position = sectionPointers[i];
                nBST.categories[i] = JBSTCategory.fromStream(stream, isBSTN);
            }
            return nBST;
        }
    }

    public class JBSTCategory
    {
        public string name;
        public JBSTLibrary[] libraries;

        
        public int mOffset;

        
        public int NameOffset;

        public static JBSTCategory fromStream(BeBinaryReader stream, bool is_bstn = false)
        {
            JBSTCategory cat = new JBSTCategory();

            var count = stream.ReadInt32();

            var nameAddress = stream.ReadInt32();
            if (is_bstn)
                cat.name = JBSTParseUtil.readNameAnchored(stream, nameAddress);
            else
                JBSTParseUtil.moveBack(stream, 0x04);

            cat.libraries = new JBSTLibrary[count];
            var libraryPointers = JBSTParseUtil.readInt32Array(stream, count);
            for (int i = 0; i < count; i++)
            {
                stream.BaseStream.Position = libraryPointers[i];
                cat.libraries[i] = JBSTLibrary.fromStream(stream, is_bstn);
            }
            return cat;
        }
    }

    public class JBSTLibrary
    {
        public string name;
        public JBSTSound[] sounds;

        
        public int mOffset;

        
        public int NameOffset;

        public static JBSTLibrary fromStream(BeBinaryReader stream, bool is_bstn)
        {
            JBSTLibrary lib = new JBSTLibrary();
            var soundCount = stream.ReadInt32();
            var nameAddress = stream.ReadInt32();
            var soundPointers = JBSTParseUtil.readInt32Array(stream, soundCount);

            if (is_bstn)
                lib.name = JBSTParseUtil.readNameAnchored(stream, nameAddress);

            lib.sounds = new JBSTSound[soundCount];

            if (is_bstn)
                for (int i = 0; i < soundCount; i++)
                    lib.sounds[i] = new JBSTNSound() { name = JBSTParseUtil.readNameAnchored(stream, soundPointers[i]) };
            else
            {
                for (int i = 0; i < soundCount; i++)
                {

                    var formatData = soundPointers[i];
                    var formatType = (byte)(formatData >> 24);
                    var formatOffset = formatData & 0x00FFFFFF;
                    stream.BaseStream.Position = formatOffset;
                    var mo = (int)stream.BaseStream.Position;
                    //Console.WriteLine($"{formatType:X} at 0x{formatOffset:X}");
                    var upperFormatType = formatType;

                    switch (upperFormatType)
                    {
                        case 0x00:
                            lib.sounds[i] = null;
                            break;

                        case 0x10:
                            lib.sounds[i] = JBSTSeSoundOld.deserialize(stream);
                            break;
                        case 0x20:
                            //lib.sounds[i] = JBSTSequenceEntryOld.deserialize(stream);
                            break;
                        case 0x30:
                            lib.sounds[i] = JBSTStreamEntryOld.deserialize(stream);
                            break;
                        case 0x50:
                            lib.sounds[i] = JBSTSeSound.deserialize(stream);
                            break;
                        case 0x51:
                            lib.sounds[i] = JBSTExtendedSeSound.deserialize(stream);
                            break;
                        case 0x60:
                            lib.sounds[i] = JBSTSequenceEntry.deserialize(stream);
                            break;
                        case 0x70:
                        case 0x71:
                            lib.sounds[i] = JBSTStreamEntry.CreateFromStream(stream);
                            break;
                        default:
                            throw new Exception($"Unknown type 0x{formatType:X} at 0x{(stream.BaseStream.Position - 2):X} format data 0x{formatData:X} ");
                    }
                    if (lib.sounds[i] != null)
                    {
                        lib.sounds[i].mOffset = mo;
                        lib.sounds[i].type = formatType;
                    }
                }
            }
            return lib;
        }
    }

    public abstract class JBSTSound : JAudioSerializable
    {
        public short type;
#if RELEASE
        
#endif
        public int mOffset;
#if RELEASE
        
#endif
        public int NameOffset;

        public string name;

    }

    public class JBSTNSound : JBSTSound // empty sound. Container format only since primary class cannot be instanced
    {
        public void deserialize()
        {

        }

        public override void WriteToStream(BeBinaryWriter stream)
        {

        }
    }
    /*
      40 00 00 00 00 00 00 00 64 00 00 00 3F 80 00 00
      b  b  b  b  b  b  b  b  b  b  b  b  f
    */

    public class JBSTStreamEntryOld : JBSTSound
    {
        public byte unk1; // ? probably class identifier? 
        public byte unk2;
        public byte unk3;
        public byte unk4;
        public byte unk5;
        public byte unk6;
        public byte unk7;
        public byte unk8;
        public byte unk9; // Probably volume ?
        public byte unk10;
        public byte unk11;
        public byte unk12;
        public float unk13; // Probably frequency multiplier ? 
        public static JBSTStreamEntryOld deserialize(BeBinaryReader stream)
        {
            var obj = new JBSTStreamEntryOld()
            {
                unk1 = stream.ReadByte(),
                unk2 = stream.ReadByte(),
                unk3 = stream.ReadByte(),
                unk4 = stream.ReadByte(),
                unk5 = stream.ReadByte(),
                unk6 = stream.ReadByte(),
                unk7 = stream.ReadByte(),
                unk8 = stream.ReadByte(),
                unk9 = stream.ReadByte(),
                unk10 = stream.ReadByte(),
                unk11 = stream.ReadByte(),
                unk12 = stream.ReadByte(),
                unk13 = stream.ReadSingle(),
            };
            return obj;
        }
        public override void WriteToStream(BeBinaryWriter stream)
        {
            stream.Write(unk1);
            stream.Write(unk2);
            stream.Write(unk3);
            stream.Write(unk4);
            stream.Write(unk5);
            stream.Write(unk6);
            stream.Write(unk7);
            stream.Write(unk8);
            stream.Write(unk9);
            stream.Write(unk10);
            stream.Write(unk11);
            stream.Write(unk12);
            stream.Write(unk13);
        }
    }

    public class JBSTStreamEntry : JBSTSound
    {
        public byte unk1;
        public byte unk2;
        public short streamType; // 0x0E for regular, 0xEE for SMG multiBGM

        public int pathOffset;
        public string streamPath;

        public void loadFromStream(BeBinaryReader stream)
        {
            unk1 = stream.ReadByte();
            unk2 = stream.ReadByte();
            streamType = stream.ReadInt16();
            pathOffset = stream.ReadInt32();
            streamPath = JBSTParseUtil.readNameAnchored(stream, pathOffset);
        }

        public override void WriteToStream(BeBinaryWriter stream)
        {
            stream.Write(unk1);
            stream.Write(unk2);
            stream.Write(streamType);
            stream.Write(0);
        }

        public static JBSTStreamEntry CreateFromStream(BeBinaryReader stream)
        {
            var obj = new JBSTStreamEntry();
            obj.loadFromStream(stream);
            return obj;
        }
    }

    public class JBSTSequenceEntryOld : JBSTSound
    {
        public byte unk1;
        public byte unk2;
        public byte unk3;
        public byte unk4;
        public byte unk5;
        public byte unk6;
        public byte unk7;
        public byte unk8;
        public byte unk9;
        public byte unk10;
        public short unk11;

        public void loadFromStream(BeBinaryReader stream)
        {
            unk1 = stream.ReadByte();
            unk2 = stream.ReadByte();
            unk3 = stream.ReadByte();
            unk4 = stream.ReadByte();
            unk5 = stream.ReadByte();
            unk6 = stream.ReadByte();
            unk7 = stream.ReadByte();
            unk8 = stream.ReadByte();
            unk9 = stream.ReadByte();
            unk10 = stream.ReadByte();
            unk11 = stream.ReadInt16();
        }

        public override void WriteToStream(BeBinaryWriter stream)
        {
            stream.Write(unk1);
            stream.Write(unk2);
            stream.Write(unk3);
            stream.Write(unk4);
            stream.Write(unk5);
            stream.Write(unk6);
            stream.Write(unk7);
            stream.Write(unk8);
            stream.Write(unk9);
            stream.Write(unk10);
            stream.Write(unk11);
        }

        public static JBSTSequenceEntryOld CreateFromSTream(BeBinaryReader stream)
        {
            var obj = new JBSTSequenceEntryOld();
            obj.loadFromStream(stream);
            return obj;
        }

    }



    public class JBSTSequenceEntry : JBSTSound
    {
        public byte unk1;
        public byte unk2;
        public short unk3;
        public short unk4;
        public short unk5;

        public static JBSTSequenceEntry deserialize(BeBinaryReader stream)
        {
            var obj = new JBSTSequenceEntry()
            {
                unk1 = stream.ReadByte(),
                unk2 = stream.ReadByte(),
                unk3 = stream.ReadInt16(),
                unk4 = stream.ReadInt16(),
                unk5 = stream.ReadInt16()
            };
            return obj;
        }
        public override void WriteToStream(BeBinaryWriter stream)
        {
            stream.Write(unk1);
            stream.Write(unk2);
            stream.Write(unk3);
            stream.Write(unk4);
            stream.Write(unk5);
        }

    }

    public class JBSTSeSound : JBSTSound
    {
        public byte unk1;
        public byte unk2;
        public byte unk3;
        public byte unk4;
        public byte unk5;
        public byte unk6;

        public static JBSTSeSound deserialize(BeBinaryReader stream)
        {
            var obj = new JBSTSeSound()
            {
                unk1 = stream.ReadByte(),
                unk2 = stream.ReadByte(),
                unk3 = stream.ReadByte(),
                unk4 = stream.ReadByte(),
                unk5 = stream.ReadByte(),
                unk6 = stream.ReadByte(),
            };
            return obj;
        }

        public override void WriteToStream(BeBinaryWriter stream)
        {
            stream.Write(unk1);
            stream.Write(unk2);
            stream.Write(unk3);
            stream.Write(unk4);
            stream.Write(unk5);
            stream.Write(unk6);
        }
    }


    public class JBSTSeSoundOld : JBSTSound
    {
        public short unk1;
        public short unk2;
        public short unk3;
        public short unk4;
        public byte unk5;
        public byte unk6;
        public short unk7;
        public float unk8;
        public byte unk9;
        public int unk10;
      

        public static JBSTSeSoundOld deserialize(BeBinaryReader stream)
        {
            var obj = new JBSTSeSoundOld()
            {
                unk1 = stream.ReadInt16(),
                unk2 = stream.ReadInt16(),
                unk3 = stream.ReadInt16(),
                unk4 = stream.ReadInt16(),
                unk5 = stream.ReadByte(),
                unk6 = stream.ReadByte(), 
                unk7 = stream.ReadInt16(),
                unk8 = stream.ReadSingle(), 
                unk9 = stream.ReadByte(), 
                unk10 = stream.ReadInt32()
            };
            return obj;
        }

        public override void WriteToStream(BeBinaryWriter stream)
        {
            stream.Write(unk1);
            stream.Write(unk2);
            stream.Write(unk3);
            stream.Write(unk4);
            stream.Write(unk5);
            stream.Write(unk6);
            stream.Write(unk7);
            stream.Write(unk8);
            stream.Write(unk9);
            stream.Write(unk10);
        }
    }

    public class JBSTExtendedSeSound : JBSTSound
    {
        public byte unk1;
        public byte unk2;
        public byte unk3;
        public byte unk4;
        public byte unk5;
        public byte unk6;
        public short unk7;
        public float unk8;

        public static JBSTExtendedSeSound deserialize(BeBinaryReader stream)
        {
            var obj = new JBSTExtendedSeSound()
            {
                unk1 = stream.ReadByte(),
                unk2 = stream.ReadByte(),
                unk3 = stream.ReadByte(),
                unk4 = stream.ReadByte(),
                unk5 = stream.ReadByte(),
                unk6 = stream.ReadByte(),
                unk7 = stream.ReadInt16(),
                unk8 = stream.ReadSingle(),
            };
            return obj;
        }

        public override void WriteToStream(BeBinaryWriter stream)
        {
            stream.Write(unk1);
            stream.Write(unk2);
            stream.Write(unk3);
            stream.Write(unk4);
            stream.Write(unk5);
            stream.Write(unk6);
            stream.Write(unk7);
            stream.Write(unk8);
        }
    }
}
