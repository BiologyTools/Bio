using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using CSScripting;
using csscript;
using CSScriptLib;
using System.Reflection;
using System.IO;
using System.Threading;

namespace BioImage
{
    public partial class Script : Form
    {
        public Script()
        {
            InitializeComponent();
            timer.Start();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            Run(textBox1.Text);
        }
        private void Run(string code)
        {
            scriptString = textBox1.Text;
            Thread t = new Thread(new ThreadStart(RunScript));
            t.Start();
        }
        static string scriptString;
        static dynamic script;
        static object obj;
        static string output;
        static bool done = false;
        static Exception ex = null;
        public static void RunScript()
        {
            ex = null;
            try
            {
                done = false;
                script = CSScript.Evaluator.LoadCode(scriptString);
                obj = script.Load();
                output = obj.ToString();
                done = true;
            }
            catch (Exception e)
            {
                ex = e;
            }
        }

        private void scriptBut_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            textBox1.Text = File.ReadAllText(openFileDialog.FileName);
            Run(textBox1.Text);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (done == true)
            {
                if (ex != null)
                    errorBox.Text = ex.Message;
                outputBox.Text = output;
            }
        }
    }
}
