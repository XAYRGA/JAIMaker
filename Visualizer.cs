
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SdlDotNet.Core;
using SdlDotNet.Graphics;
using System.Drawing;
using System.Runtime;
using System.Runtime.InteropServices;
using SdlDotNet.Input;
using System.Diagnostics;

namespace JaiMaker
{
    public static class Visualizer
    {

        static Thread RenderThread; // 
        static Surface IVideoSurface;
        static SdlDotNet.Graphics.Font myfont;
        static Process me;

        public static void Init()
        {

            Events.TargetFps = 60; // 60 fps
            Events.Quit += (QuitEventHandler); // pizzaz, to make it so when we press the exit button it does.
            Events.Tick += (draw); // Drawing handler
            Events.KeyboardDown += KeyDown; // a
            Events.KeyboardUp += KeyUp;
            myfont = new SdlDotNet.Graphics.Font("tahoma.ttf", 12); // Load the font
            RenderThread = new Thread(new ThreadStart(startDrawWindowThread)); // Create render thread
            me = Process.GetCurrentProcess();
            RenderThread.Start(); // Start it
        }

        private static void startDrawWindowThread()
        {
            IVideoSurface = Video.SetVideoMode(800, 800, 16, false, false, false, true); // Initialize video surface, 800x800, 16 bit
            Events.Run(); // Start the drawing / event loop
        }
        private static void KeyDown(object sender, KeyboardEventArgs kbe)
        {

            var channel = (byte)kbe.Key; 
            Console.WriteLine("key {0}", channel);
            Keyboard.startSound((byte)(channel));
    
        }

        private static void KeyUp(object sender, KeyboardEventArgs kbe)
        {

            var channel = (byte)kbe.Key;
            Keyboard.stopSound((byte)(channel));
           

        }

        private static void draw(object sender, TickEventArgs args)
        {
            IVideoSurface.Fill(Color.Black); // flush background black

            Point HeaderPos = new Point(5, 5); // Point for header drawing
            var HeaderText = myfont.Render("JAIMaker by XayrGA ", Color.White); // yay me.
            Video.Screen.Blit(HeaderText, HeaderPos);

            Point RAMPos = new Point(150, 5); // Point for header drawing

            var ramstring = string.Format("MEM: {0}MB ", me.PrivateMemorySize / (1024 * 1024));
            if (Root.currentProg != null && Root.currentBank != null)
            {
                ramstring = string.Format("MEM: {0}MB BNK: {1} PRG: {2}", me.PrivateMemorySize / (1024 * 1024), Root.currentBank.id, Root.ProgNumber);
            }
           
            var RAMText = myfont.Render(ramstring, Color.White); // yay me.
            Video.Screen.Blit(RAMText, RAMPos);
            
            RAMText.Dispose();
            HeaderText.Dispose();

            
            IVideoSurface.Update(); // Update the video surface. 
        }
        private static void QuitEventHandler(object sender, QuitEventArgs args)
        {
            Environment.Exit(0x00);
            Events.QuitApplication();
        }
    }
}

