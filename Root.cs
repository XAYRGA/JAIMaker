using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
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
        public static int currentVel = 125;
        public static int keyOffset = 34;
        public static int[] instrumentBanks = new int[1024];
        public static int[] programs = new int[1024];
        public static int Tempo = 120;


        
        [STAThread]
        static void Main()
        {
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
