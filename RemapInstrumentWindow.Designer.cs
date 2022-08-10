namespace JaiMaker
{
    partial class RemapInstrumentWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnDelete = new System.Windows.Forms.Button();
            this.remapList = new System.Windows.Forms.ListBox();
            this.btnCreate = new System.Windows.Forms.Button();
            this.nsProg = new System.Windows.Forms.NumericUpDown();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.nsBank = new System.Windows.Forms.NumericUpDown();
            this.btnSetSelected = new System.Windows.Forms.Button();
            this.tbName = new System.Windows.Forms.TextBox();
            this.lbName = new System.Windows.Forms.Label();
            this.lblMidiProg = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.nsProg)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.nsBank)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.Location = new System.Drawing.Point(12, 256);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(160, 23);
            this.btnDelete.TabIndex = 0;
            this.btnDelete.Text = "Delete Selected";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // remapList
            // 
            this.remapList.FormattingEnabled = true;
            this.remapList.Location = new System.Drawing.Point(12, 12);
            this.remapList.Name = "remapList";
            this.remapList.Size = new System.Drawing.Size(331, 160);
            this.remapList.TabIndex = 1;
            this.remapList.SelectedIndexChanged += new System.EventHandler(this.remapList_SelectedIndexChanged);
            // 
            // btnCreate
            // 
            this.btnCreate.Location = new System.Drawing.Point(183, 256);
            this.btnCreate.Name = "btnCreate";
            this.btnCreate.Size = new System.Drawing.Size(160, 23);
            this.btnCreate.TabIndex = 2;
            this.btnCreate.Text = "Create New";
            this.btnCreate.UseVisualStyleBackColor = true;
            this.btnCreate.Click += new System.EventHandler(this.btnCreate_Click);
            // 
            // nsProg
            // 
            this.nsProg.Location = new System.Drawing.Point(96, 230);
            this.nsProg.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.nsProg.Name = "nsProg";
            this.nsProg.Size = new System.Drawing.Size(76, 20);
            this.nsProg.TabIndex = 3;
            this.nsProg.ValueChanged += new System.EventHandler(this.nsProg_ValueChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 213);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(32, 13);
            this.label1.TabIndex = 4;
            this.label1.Text = "Bank";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 213);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Program";
            // 
            // nsBank
            // 
            this.nsBank.Location = new System.Drawing.Point(12, 230);
            this.nsBank.Maximum = new decimal(new int[] {
            127,
            0,
            0,
            0});
            this.nsBank.Name = "nsBank";
            this.nsBank.Size = new System.Drawing.Size(72, 20);
            this.nsBank.TabIndex = 5;
            this.nsBank.ValueChanged += new System.EventHandler(this.nsBank_ValueChanged);
            // 
            // btnSetSelected
            // 
            this.btnSetSelected.Location = new System.Drawing.Point(183, 230);
            this.btnSetSelected.Name = "btnSetSelected";
            this.btnSetSelected.Size = new System.Drawing.Size(160, 23);
            this.btnSetSelected.TabIndex = 7;
            this.btnSetSelected.Text = "Set Selected";
            this.btnSetSelected.UseVisualStyleBackColor = true;
            this.btnSetSelected.Click += new System.EventHandler(this.btnSetSelected_Click);
            // 
            // tbName
            // 
            this.tbName.Location = new System.Drawing.Point(12, 191);
            this.tbName.Name = "tbName";
            this.tbName.Size = new System.Drawing.Size(160, 20);
            this.tbName.TabIndex = 8;
            this.tbName.TextChanged += new System.EventHandler(this.tbName_TextChanged);
            // 
            // lbName
            // 
            this.lbName.AutoSize = true;
            this.lbName.Location = new System.Drawing.Point(12, 175);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(35, 13);
            this.lbName.TabIndex = 9;
            this.lbName.Text = "Name";
            // 
            // lblMidiProg
            // 
            this.lblMidiProg.AutoSize = true;
            this.lblMidiProg.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMidiProg.Location = new System.Drawing.Point(180, 193);
            this.lblMidiProg.Name = "lblMidiProg";
            this.lblMidiProg.Size = new System.Drawing.Size(149, 18);
            this.lblMidiProg.TabIndex = 10;
            this.lblMidiProg.Text = "MIDI Program: N/A";
            // 
            // RemapInstrumentWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(355, 293);
            this.Controls.Add(this.lblMidiProg);
            this.Controls.Add(this.lbName);
            this.Controls.Add(this.tbName);
            this.Controls.Add(this.btnSetSelected);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.nsBank);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nsProg);
            this.Controls.Add(this.btnCreate);
            this.Controls.Add(this.remapList);
            this.Controls.Add(this.btnDelete);
            this.Name = "RemapInstrumentWindow";
            this.Text = "Remap MIDI Instruments";
            this.Load += new System.EventHandler(this.RemapInstrumentWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nsProg)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.nsBank)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox remapList;
        private System.Windows.Forms.Button btnCreate;
        private System.Windows.Forms.NumericUpDown nsProg;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown nsBank;
        private System.Windows.Forms.Button btnSetSelected;
        private System.Windows.Forms.TextBox tbName;
        private System.Windows.Forms.Label lbName;
        private System.Windows.Forms.Label lblMidiProg;
    }
}