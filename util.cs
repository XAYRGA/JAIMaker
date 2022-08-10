using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Be.IO;
using System.Windows.Forms;

namespace JaiMaker
{
    public static class util
    {
        public static bool consoleProgress_quiet = false;
        public static void consoleProgress(string txt, int progress, int max, bool show_progress = false)
        {
            if (consoleProgress_quiet)
                return;
            var flTotal = (float)progress / max;
            Console.CursorLeft = 0;
            Console.Write($"{txt} [");
            for (float i = 0; i < 32; i++)
                if (flTotal > (i / 32f))
                    Console.Write("#");
                else
                    Console.Write(" ");
            Console.Write("]");
            if (show_progress)
                Console.Write($" ({progress}/{max})");           
        }
        public static int padTo(BeBinaryWriter bw, int padding)
        {
            int del = 0; 
            while (bw.BaseStream.Position % padding != 0)
            {
                bw.BaseStream.WriteByte(0x00);
                bw.BaseStream.Flush();
                del++;
            }
            return del;
        }

        public static int[] readInt32Array(BeBinaryReader binStream, int count)
        {
            var b = new int[count];
            for (int i = 0; i < count; i++)
                b[i] = binStream.ReadInt32();
            return b;
        }

        public static byte[] getVLQBytes(int number)
        {


            byte[] bytes = new byte[4];
            int index = 0;
            int buffer = number & 0x7F;

            while ((number >>= 7) > 0)
            {
                buffer <<= 8;
                buffer |= 0x80;
                buffer += (number & 0x7F);
            }
            while (true)
            {
                bytes[index] = (byte)buffer;
                index++;
                if ((buffer & 0x80) > 0)
                    buffer >>= 8;
                else
                    break;
            }

            var Length = index;
            var Bytes = new byte[index];
            Array.Copy(bytes, 0, Bytes, 0, Length);
            return Bytes;
        }


        public static void writeVLQ(BeBinaryWriter bw, int value)
        {
            do
            {
                byte lower7bits = (byte)(value & 0x7f);
                value >>= 7;
                if (value > 0)
                    lower7bits |= 128;
                bw.Write(lower7bits);
            } while (value > 0);
        }

        public static void writeInt24BE(BeBinaryWriter bw, int ta)
        {
            var b1 = (ta) & 0xFF;
            var b2 = (ta >> 8) & 0xFF;
            var b3 = (ta >> 16) & 0xFF;
            bw.Write((byte)b3);
            bw.Write((byte)b2);
            bw.Write((byte)b1);
        }


        public static int padToInt(int Addr, int padding)
        {
            var delta = (int)(Addr % padding);
            return (padding - delta);        
        }

        public static uint ReadUInt24BE(BinaryReader reader)
        {
            return
                (((uint)reader.ReadByte()) << 16) |
                (((uint)reader.ReadByte()) << 8) |
                ((uint)reader.ReadByte());
        }

        public static int ReadVLQ(BinaryReader reader)
        {
            int vlq = 0;
            int temp = 0;
            do
            {
                temp = reader.ReadByte();
                vlq = (vlq << 7) | (temp & 0x7F);
            } while ((temp & 0x80) > 0);
            return vlq;
        }

        public static string getOpenFile(string title = "Open File", string filter = "All files (*.*)|*.*")
        {
            var OFD = new OpenFileDialog();
            OFD.Title = title;
            OFD.RestoreDirectory = true;
            var diaRes = OFD.ShowDialog();
            if (diaRes == DialogResult.OK)
                return OFD.FileName;
            else
                return null;          
        }

        public static Stream getOpenFileStream(string title = "Open File", string filter = "All files (*.*)|*.*")
        {
            var pth = getOpenFile(title, filter);
            if (pth == null)
                return null;
            var fHnd = File.OpenRead(pth);
            return fHnd;     
        }

    }
}
