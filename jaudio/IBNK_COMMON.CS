using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.IO;

namespace JaiMaker
{
    public class JVelocityRegion
    {
        public int mBaseAddress = 0;
        public byte Velocity;
        public ushort WSYSID;
        public ushort WaveID;
        public float Volume;
        public float Pitch;
    }

    public class JKeyRegion
    {
        public int mBaseAddress = 0;
        public byte BaseKey;
    }


    public class JInstrument
    {

        public int mBaseAddress = 0;

        public float Pitch = 1;
        public float Volume = 1;

        internal const int INST = 0x494E5354;
        internal const int PER2 = 0x50455232;
        public bool Percussion = false;

        public static JInstrument CreateFromStream(BeBinaryReader reader, int seekbase)
        {
            var magic = reader.ReadUInt32();
            if (magic == INST)
                return JStandardInstrumentv1.CreateFromStream(reader, seekbase);
            else if (magic == PER2)
                return JPercussion.CreateFromStream(reader, seekbase);
            return null;
        }
    }

}
