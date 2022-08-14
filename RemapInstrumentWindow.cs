using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace JaiMaker
{
    public partial class RemapInstrumentWindow : Form
    {
        public Dictionary<int, JAIMakerSoundInfo> RemapData;
        public JAIMakerSoundInfo CurrentRemap;
        private int[] ListBoxMap = new int[127];
        public RemapInstrumentWindow()
        {
            InitializeComponent();
        }

        private string getRemapName(JAIMakerSoundInfo SoundInfo, int MidiProgram = 0)
        {
            if (SoundInfo.name == null || SoundInfo.name.Length < 1)
                return $"{MidiProgram} -> B:{SoundInfo.bank} P:{SoundInfo.prog}";
            return SoundInfo.name;
        }

        private void refreshRemapList()
        {
            remapList.Items.Clear();
            ListBoxMap = new int[127];
            foreach (KeyValuePair<int, JAIMakerSoundInfo> si in RemapData)
                ListBoxMap[remapList.Items.Add($"{getRemapName(si.Value, si.Key)}")] = si.Key;
        }

        private void updateSelectedControls(int midiIndex)
        {
            nsBank.Value = CurrentRemap.bank;
            nsProg.Value = CurrentRemap.prog;
            lblMidiProg.Text = $"MIDI Program: {midiIndex}";
            tbName.Text = CurrentRemap.name;
        }

        private void btnCreate_Click(object sender, EventArgs e)
        {
            var dlg = new dlgEnterMidiProgram();
            if (dlg.ShowDialog()==DialogResult.OK)
            {
                var midiValue = dlg.MIDIProgram;
                if (RemapData.ContainsKey(midiValue))
                    if (MessageBox.Show($"You already have a MIDI program remap with the value {midiValue}.\r\nContinuing will overwrite. Are you OK with that?", "JAIMaker - Warning", MessageBoxButtons.OKCancel) != DialogResult.OK)
                        return;

                RemapData[midiValue] = new JAIMakerSoundInfo();
                refreshRemapList();
            }
        }

        private void remapList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (remapList.SelectedIndex < ListBoxMap.Length &  remapList.SelectedIndex >= 0)
            {
               
                var DictionaryLookup = ListBoxMap[remapList.SelectedIndex];
                if (RemapData.ContainsKey(DictionaryLookup))
                {
                    CurrentRemap = RemapData[DictionaryLookup];
                    updateSelectedControls(DictionaryLookup);
                }
            }
        }

        private void tbName_TextChanged(object sender, EventArgs e)
        {
            if (CurrentRemap == null)
                return;
            CurrentRemap.name = tbName.Text;
        }

        private void nsBank_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentRemap == null)
                return;
            CurrentRemap.bank = (int)nsBank.Value;
        }

        private void nsProg_ValueChanged(object sender, EventArgs e)
        {
            if (CurrentRemap == null)
                return;
            CurrentRemap.prog = (int)nsProg.Value;
        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show($"You're about to delete this remap.\r\nThis cannot be undone, proceed?", "JAIMaker - Warning", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;
            if (remapList.SelectedIndex < 0)
                return;

            var DictionaryLookup = ListBoxMap[remapList.SelectedIndex];
            if (RemapData.ContainsKey(DictionaryLookup))
            {
                CurrentRemap = null;
                RemapData.Remove(DictionaryLookup);
                ListBoxMap[remapList.SelectedIndex] = 0;
                refreshRemapList();
            }
        }

        private void RemapInstrumentWindow_Load(object sender, EventArgs e)
        {
            refreshRemapList();
        }

        private void btnSetSelected_Click(object sender, EventArgs e)
        {
            if (CurrentRemap == null)
                return;
            CurrentRemap.bank = Root.BankNumber; // I apologize for my crimes. 
            CurrentRemap.prog = Root.ProgNumber;
            nsBank.Value = CurrentRemap.bank;
            nsProg.Value = CurrentRemap.prog;

        }
    }
}
