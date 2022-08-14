using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime;
using System.Reflection;


namespace JaiMaker
{
    partial class AboutCreatorWindow
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
            this.Creator = new System.Windows.Forms.Label();
            this.github = new System.Windows.Forms.LinkLabel();
            this.website = new System.Windows.Forms.LinkLabel();
            this.linkLabel2 = new System.Windows.Forms.LinkLabel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // Creator
            // 
            this.Creator.Font = new System.Drawing.Font("Segoe UI", 27.75F);
            this.Creator.Location = new System.Drawing.Point(119, 9);
            this.Creator.Name = "Creator";
            this.Creator.Size = new System.Drawing.Size(233, 54);
            this.Creator.TabIndex = 1;
            this.Creator.Text = "Created by";
            // 
            // github
            // 
            this.github.AutoSize = true;
            this.github.Location = new System.Drawing.Point(10, 359);
            this.github.Name = "github";
            this.github.Size = new System.Drawing.Size(202, 13);
            this.github.TabIndex = 2;
            this.github.TabStop = true;
            this.github.Text = "https://www.github.com/xayrga/jaimaker";
            this.github.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.github_LinkClicked);
            // 
            // website
            // 
            this.website.AutoSize = true;
            this.website.Location = new System.Drawing.Point(10, 380);
            this.website.Name = "website";
            this.website.Size = new System.Drawing.Size(109, 13);
            this.website.TabIndex = 3;
            this.website.TabStop = true;
            this.website.Text = "https://www.xayr.ga/";
            this.website.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.website_LinkClicked);
            // 
            // linkLabel2
            // 
            this.linkLabel2.AutoSize = true;
            this.linkLabel2.Location = new System.Drawing.Point(10, 401);
            this.linkLabel2.Name = "linkLabel2";
            this.linkLabel2.Size = new System.Drawing.Size(122, 13);
            this.linkLabel2.TabIndex = 4;
            this.linkLabel2.TabStop = true;
            this.linkLabel2.Text = "https://ko-fi.com/xayrga";
            this.linkLabel2.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel2_LinkClicked);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::JaiMaker.Properties.Resources.xbadgetrns_sml;
            this.pictureBox1.Location = new System.Drawing.Point(99, 66);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(270, 258);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.Click += new System.EventHandler(this.pictureBox1_Click);
            // 
            // AboutCreatorWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(465, 443);
            this.Controls.Add(this.linkLabel2);
            this.Controls.Add(this.website);
            this.Controls.Add(this.github);
            this.Controls.Add(this.Creator);
            this.Controls.Add(this.pictureBox1);
            this.Name = "AboutCreatorWindow";
            this.Text = "AboutCreatorWindow";
            this.Load += new System.EventHandler(this.AboutCreatorWindow_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private Label Creator;
        private LinkLabel github;
        private LinkLabel website;
        private LinkLabel linkLabel2;
        private PictureBox pictureBox1;
    }
}