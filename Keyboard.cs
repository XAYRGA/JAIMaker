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
using JaiSeqX.Player;

namespace JaiMaker
{
    public static class Keyboard
    {
        public static BMSChannelManager channelManager = new BMSChannelManager();
        static string keyOrderString = @"1234567890-=qwertyuiop[]\asdfghjkl;'zxcvbnm,./";
        static int[] pitches;
        public static void init()
        {
            var lastPitch = 0;
            pitches = new int[1024];
            for (int i=0; i < keyOrderString.Length;i++)
            {
                var str = keyOrderString[i];
                pitches[str] = lastPitch++; 
            }
        }


        public static void stopSound(byte inkey)
        {
            channelManager.stopVoice(0, inkey);
        }
        public static void startSound(byte inkey)
        {
            
            var prog = Root.currentProg;
            if (prog!=null)
            {
                var note = pitches[inkey] + Root.keyOffset;
                var vel = Root.currentVel;

                if (prog.Keys[note]!=null)
                {
                    var notedata = prog.Keys[note];
                    var key = notedata.keys[vel];

                    if (key!=null)
                    {
                        try
                        {
                            var wsysid = key.wsysid;
                            var waveid = key.wave;
                            var wsys = Root.allWSYS[wsysid];
                            if (wsys != null)
                            {
                                
                                var wave = wsys.waves[waveid];
                                var sound = channelManager.loadSound(wave.pcmpath, wave.loop, wave.loop_start, wave.loop_end).CreateInstance();
                                var pmul = prog.Pitch * key.Pitch;
                                var vmul = prog.Volume * key.Volume;
                                var real_pitch = Math.Pow(2, ((note - wave.key) * pmul) / 12);
                                var true_volume = (Math.Pow(((float)vel + Root.keyOffset) / 127, 2) * vmul) * 0.5;
                                sound.Volume = (float)(true_volume * 0.6);
                                sound.ShouldFade = true;
                                sound.FadeOutMS = 30;
                                if (prog.IsPercussion)
                                {
                                    real_pitch = (float)(key.Pitch * prog.Pitch);

                                    sound.ShouldFade = true;
                                    sound.FadeOutMS = 200; // no instant stops
                                }
                                sound.Pitch = (float)(real_pitch);

                                channelManager.startVoice(sound, 0, inkey);
                             
                                    sound.Play();
                              

                            }
                            else
                            {
                                Console.WriteLine("Null WSYS??");
                            }
                        }
                        catch (Exception E)
                        {
                            var b = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("fuuuuuck");
                            Console.WriteLine(E.ToString());
                            Console.ForegroundColor = b;
                        }
                    } else
                    {
                        Console.WriteLine("Null key :(");
                    }
                    

                } else
                {
                    Console.WriteLine("ugh.");
                }
            }
           
        }
    }
}
