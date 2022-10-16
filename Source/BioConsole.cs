﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bio
{
    public partial class BioConsole : Form
    {
        public BioConsole()
        {
            InitializeComponent();
        }

        private void runBut_Click(object sender, EventArgs e)
        {
            object o = Scripting.Script.RunString(textBox.Text);
            consoleBox.Text += o.ToString() + Environment.NewLine;
            textBox.Text = "";
        }

        private void imagejBut_Click(object sender, EventArgs e)
        {
            ImageJ.RunOnImage(textBox.Text, ImageView.SelectedImage.ID, headlessBox.Checked);
            consoleBox.Text += textBox.Text + Environment.NewLine;
            textBox.Text = "";
            string file = System.IO.Path.GetDirectoryName(ImageView.SelectedImage.ID) + "/" + System.IO.Path.GetFileNameWithoutExtension(ImageView.SelectedImage.ID) + ".ome.tif";
            if (ImageView.SelectedImage.ID.EndsWith(".ome.tif"))
                ImageView.SelectedImage.Update();
            else
                App.tabsView.AddTab(BioImage.OpenOME(file));
        }

        private void topMostBox_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = topMostBox.Checked;
        }

        private void BioConsole_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.Hide();
        }
    }
}