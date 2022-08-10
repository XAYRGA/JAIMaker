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
    public partial class dlgEnterMidiProgram : Form
    {
        public int MIDIProgram = 0;
        public dlgEnterMidiProgram()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            this.Close();
        }

        private void nsMidiProg_ValueChanged(object sender, EventArgs e)
        {
           MIDIProgram = (int)nsMidiProg.Value; 
        }
    }
}
