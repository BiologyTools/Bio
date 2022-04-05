using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BioImage
{
    public partial class Recorder : Form
    {
        public static Recorder recorder = null;
        public Recorder()
        {
            InitializeComponent();
        }
        public static void AddLine(string line)
        {
            if(recorder.Visible)
            if (recorder.textBox.Lines.Length > 0)
            {
                recorder.textBox.Text += Environment.NewLine;
                recorder.textBox.Text += line;
            }
            else
                recorder.textBox.Text += line;
        }

        private void clearBut_Click(object sender, EventArgs e)
        {
            textBox.Clear();
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
    }
}
