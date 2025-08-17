using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.IO;
using System.IO;

namespace JaiSeqX.JAI.Seq
{

    public enum JaiSeqEvent
    {

        /* wait with u8 arg */
        WAIT_8 = 0x80,
        /* wait with u16 arg */
        WAIT_16 = 0x88,
        /* wait with variable-length arg */
        WAIT_VAR = 0xF0,

        /* perf / lerp */
        PERF_U8_NODUR = 0x94,
        PERF_U8_DUR_U8 = 0x96,
        PERF_U8_DUR_U16 = 0x97,
        PERF_S8_NODUR = 0x98,
        PERF_S8_DUR_U8 = 0x9A,
        PERF_S8_DUR_U16 = 0x9B,
        PERF_S16_NODUR = 0x9C,
        PERF_S16_DUR_U8 = 0x9E,
        PERF_S16_DUR_U16 = 0x9F,

        PARAM_SET = 0xA0,
        ADDR = 0xA1,
        MULR = 0xA2,
        CMPR = 0xA3,
        PARAM_SET_8 = 0xA4, 
        ADD8 = 0xA5,
        MUL8 = 0xA6,
        CMP8 = 0xA7,
        LOADTBL = 0xAA,
        SUB = 0xAB,
        PARAM_SET_16 = 0xAC,
        ADD16 = 0xAD,
        MUL16 = 0xAE,
        CMP16 = 0xAF,
        LOAD_TABLE = 0xAA,
        SUBTRACT = 0xAB,
        BITWISE = 0xA9,
   


        OPEN_TRACK = 0xC1,
        OPEN_TRACK_BROS = 0xC2,
        CALL = 0xC3,
        CALL_COND = 0xC4,
        RET = 0xC5,
        RET_COND = 0xC6,
        JUMP = 0xC7,
        JUMP_COND = 0xC8,
        LOOP_COUNT = 0xC9,
        PORTREAD = 0xCB,
        PORTWRITE = 0xCC,
        SPECIALWAIT = 0xCF,
       

        NAMEBUS = 0xD0,
        ADSR = 0xD8,
        BUSCONNECT = 0xDD,
        INTERRUPT = 0xDF,
        INTERRUPT_TIMER = 0xE4,
        SYNC_CPU = 0xE7,
        PANSWSET = 0xEF,
        OSCILLATORFULL = 0xF2,  
        PRINTF = 0xFB,
        TIME_BASE = 0xFD,
        TEMPO = 0xFE,
        FIN = 0xFF,


        /* "Improved" JaiSeq from TP / SMG / SMG2 seems to use this instead */
        J2_SET_PERF_8 = 0xB8,
        J2_SET_PERF_16 = 0xB9,
        /* Set "articulation"? Used for setting timebase. */
        J2_SET_ARTIC = 0xD8,
        J2_TEMPO = 0xE0,
        J2_SET_BANK = 0xE2,
        J2_SET_PROG = 0xE3,
    }





    
}
