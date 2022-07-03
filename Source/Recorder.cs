using System;
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
    public partial class Recorder : Form
    {
        public static string log;
        private static bool update = false;
        public static void AddLine(string s)
        {
            log += s + Environment.NewLine;
            update = true;
        }
        public static Recorder recorder = null;
        public Recorder()
        {
            InitializeComponent();
            timer.Start();
        }

        private void clearBut_Click(object sender, EventArgs e)
        {
            textBox.Clear();
            log = "";
        }

        private void delLineBut_Click(object sender, EventArgs e)
        {
            string[] sts = textBox.Lines;
            string[] st = new string[sts.Length - 1];
            for (int i = 0; i < st.Length; i++)
            {
                st[i] = sts[i];
            }
            textBox.Lines = st;
        }

        private void Recorder_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }

        private void timer_Tick_1(object sender, EventArgs e)
        {
            if (update)
            {
                textBox.Text = log;
                update = false;
            }
            
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            log = textBox.Text;
        }

        private void topMostBox_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = topMostBox.Checked;
        }
    }
}
