using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using JaiSeqX.JAI;
using JaiSeqX.JAI.Types;
using JaiSeqX.JAI.Types.WSYS;

using JaiSeqX.Player;
using JaiSeqX.Player.BassBuff;

namespace JaiMaker
{
    static class Root
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        ///
        public static AABase g_AAF;
        public static WaveSystem[] allWSYS;
        public static InstrumentBank currentBank;
        public static Instrument currentProg;
        public static int ProgNumber;
        public static int BankNumber;
        public static int currentVel = 125;
        public static int keyOffset = 34;
        public static int[] instrumentBanks = new int[64];
        public static int[] programs = new int[64];
        public static int[] volumes = new int[16];
        public static int[] offsets = new int[16];
        public static bool[] dynamics = new bool[16];
        public static int Tempo = 120;


        
        [STAThread]
        static void Main()
        {
            Console.WriteLine(@"       _         _____ __  __       _               __ ");
            Console.WriteLine(@"      | |  /\   |_   _|  \/  |     | |             /_ |");
            Console.WriteLine(@"      | | /  \    | | | \  / | __ _| | _____ _ __   | |");
            Console.WriteLine(@"  _   | |/ /\ \   | | | |\/| |/ _` | |/ / _ \ '__|  | |");
            Console.WriteLine(@" | |__| / ____ \ _| |_| |  | | (_| |   <  __/ |     | |");
            Console.WriteLine(@"  \____/_/    \_\_____|_|  |_|\__,_|_|\_\___|_|     |_|");
            Console.WriteLine(@"  Created by XAYRGA! -- http://www.xayr.gay/           ");
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();
            Console.WriteLine();

            //MidiToBMS.doToBMS();

            Engine.Init(); // Start audio engine.
            Keyboard.init();
            // Visualizer.Init();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new RootWindow());
            //*/
        }
    }
}
