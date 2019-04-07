using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using JaiSeqX.JAI.Seq;
using JaiSeqX.JAI.Types;
using JaiSeqX.JAI;
using MidiSharp;
using System.IO;
using System.Diagnostics;

namespace JaiMaker
{
    public partial class RootWindow : Form
    {
        public int[] bankMap = new int[1024];
        public int[] progMap = new int[1024];
        InstrumentBank currentIBNK;
        Instrument currentInst;
        MidiSequence currentSequence;
        JaiSeqX.JAIVersion type;
        KeysConverter kk; 
        public bool[] keysPressed = new bool[1024];
        string JaiFile = "";
        
        public RootWindow()
        {
            InitializeComponent();
        }

        private void contextMenuStrip1_Opening(object sender, CancelEventArgs e)
        {

        }

        private void RootWindow_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
            this.KeyDown += OnKey;
            this.KeyUp += OnKeyUp;
            kk = new KeysConverter();
        }

        private void OnKey(object sender, KeyEventArgs kbe)
        {
            kbe.SuppressKeyPress = kbmode.Checked;
            if (!keysPressed[kbe.KeyValue])
            {
                var channel = (byte)(char)kbe.KeyValue + 32;
                Console.WriteLine("key {0}", channel);
                Keyboard.startSound((byte)(channel));
                keysPressed[kbe.KeyValue] = true;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs kbe)
        {
            Console.WriteLine("key up?");

            var channel = (byte)(char)kbe.KeyValue + 32;
            Keyboard.stopSound((byte)(channel));
            keysPressed[kbe.KeyValue] = false;
        }

        private void EnableFunctions()
        {
            mainControls.Enabled = true;
            importMIDIToolStripMenuItem.Enabled = true;

            UpdateBanks();

        }

        private void UpdateBanks()
        {

            banksList.Items.Clear();
            var bankidx = 0; 
            var IBNK = Root.g_AAF.IBNK;
            for (int i = 0; i < IBNK.Length;i++)
            {
                if (IBNK[i]!=null)
                {
                    banksList.Items.Add("Bank " + i);
                    
                    bankMap[bankidx] = i;
                    bankidx++;
                }
            }
        }
        private void openAAFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Opening AAF.");
            currentStatus.Text = "Opening AAF";
            var dlgr = fileSelector.ShowDialog();
           
            try
            {
            
                var wtf = new AAFFile();
                wtf.LoadAAFile(fileSelector.FileName,JaiSeqX.JAIVersion.ONE);
                JaiFile = fileSelector.FileName;
                Root.g_AAF = wtf;
                Root.allWSYS = wtf.WSYS;
                currentStatus.Text = "AAF Loaded successfully.";
                EnableFunctions();
                type = JaiSeqX.JAIVersion.ONE;
                

            } catch (Exception E)
            {
                MessageBox.Show("Failed opening AAF\n" + E.ToString(), "ugh");
            }
        }


        private void mainControls_Paint(object sender, PaintEventArgs e)
        {

        }

        private void mainControls_Paint_1(object sender, PaintEventArgs e)
        {

        }

        private void banksList_SelectedIndexChanged(object sender, EventArgs e)
        {

            try
            {
                progList.Items.Clear();
                var progidx = 0;
                var IBNK = Root.g_AAF.IBNK;
                if (banksList.SelectedIndex > IBNK.Length || banksList.SelectedIndex > bankMap.Length)
                {
                    return;
                }
                var CurrentIBNK = IBNK[bankMap[banksList.SelectedIndex]];
                currentIBNK = CurrentIBNK;
                for (int i = 0; i < CurrentIBNK.Instruments.Length; i++)
                {
                    if (CurrentIBNK.Instruments[i] != null)
                    {
                     
                        if (CurrentIBNK.Instruments[i].IsPercussion)
                        {
                            progList.Items.Add("(PRC)Program " + (i));
                        } else
                        {
                            progList.Items.Add("Program " + (i));
                        }


                        progMap[progidx] = i;
                        progidx++;

                    }
                }
                Root.currentBank = currentIBNK;
            }
            catch { } // Fuck this too. 
        }

        private void updateUIGlobal()
        {
            if (currentSequence!=null)
            {
                midiChannelData.Enabled = true; 

                for (int i=0; i < 16; i++)
                {
                    var rer = i < currentSequence.Tracks.Count;
           
                        midiChannelData.GetControlFromPosition(2, i).Enabled = rer; ;
                        midiChannelData.GetControlFromPosition(1, i).Enabled = rer;  ;
                        midiChannelData.GetControlFromPosition(0, i).ForeColor = rer ? Color.Green : Color.Red;
             
                }
                exportBMS.Enabled = true;
                if (File.Exists("JaiSeqX.exe"))
                {
                    launchJSEQ.Enabled = true;
                }
            } else
            {
                launchJSEQ.Enabled = false;
                exportBMS.Enabled = false; 
            }
         
        }

        private void updateChannelData()
        {
            for (int i=0; i < midiChannelData.RowCount; i++)
            {
                
                var text = (Label)midiChannelData.GetControlFromPosition(0, i);
                var bank = (NumericUpDown)midiChannelData.GetControlFromPosition(1, i);
                var program = (NumericUpDown)midiChannelData.GetControlFromPosition(2, i);
                var volume = (TrackBar)midiChannelData.GetControlFromPosition(3, i);

                Root.programs[i] = (int)program.Value;
                Root.instrumentBanks[i] = (int)bank.Value; 
                
            }
        }

        private void progList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (currentIBNK == null) { return; }
                if (progList.SelectedIndex > currentIBNK.Instruments.Length || progList.SelectedIndex > progMap.Length)
                {
                    return;
                }

                currentInst = currentIBNK.Instruments[progMap[progList.SelectedIndex]];
                Root.currentProg = currentInst;
                Root.ProgNumber = progMap[progList.SelectedIndex];
                updateChannelData();
            }
            catch { } // fuck this. I added checks. I cant figure it out

        }

        private void velocityBar_Scroll(object sender, EventArgs e)
        {
            velLabel.Text = "Velocity: " + velocityBar.Value;
            Root.currentVel = velocityBar.Value;
        }

        private void keyOffsetBar_Scroll(object sender, EventArgs e)
        {
            keyOffsetLabel.Text = "Key Offset: " + keyOffsetBar.Value;
            Root.keyOffset = keyOffsetBar.Value;
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e)
        {

        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void updateTimer_Tick(object sender, EventArgs e)
        {
            updateChannelData();
            updateUIGlobal();
        }

        private void launchJSEQ_Click(object sender, EventArgs e)
        {
            MidiToBMS.doToBMS(currentSequence, "test.bms");

            var args = string.Format("visu \"{0}\" {1} test.bms",JaiFile,(int)type);
            var b = new ProcessStartInfo("JaiSeqX.exe", args);
            var bw = Process.Start(b);
           
            bw.WaitForExit();

        }

        private void exportBMS_Click(object sender, EventArgs e)
        {
            saveSelector.ShowDialog();
            var name = saveSelector.FileName;
            MidiToBMS.doToBMS(currentSequence, name);
           
        }

        private void importMIDIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                fileSelector.Title = "Open MIDI file";
                fileSelector.ShowDialog();
                var myfile = fileSelector.FileName;
                var b = File.OpenRead(myfile);
                currentSequence = MidiSequence.Open(b);
            } catch 
            {
                MessageBox.Show("Not a valid midi file.");
            }
            
        }

        private void type1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Console.WriteLine("Opening BAA.");
            currentStatus.Text = "Opening BAA";
            var dlgr = fileSelector.ShowDialog();

            try
            {

                var wtf = new BAAFile();
                wtf.LoadBAAFile(fileSelector.FileName, JaiSeqX.JAIVersion.TWO);
                JaiFile = fileSelector.FileName;
                Root.g_AAF = wtf;
                Root.allWSYS = wtf.WSYS;
                currentStatus.Text = "BAA Loaded successfully.";
                EnableFunctions();
                type = JaiSeqX.JAIVersion.TWO;

            }
            catch (Exception E)
            {
                MessageBox.Show("Failed opening BAA\n" + E.ToString(), "ugh");
            }
        }
    }
}
