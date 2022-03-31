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
        private string name = "";
        public string ScriptName
        {
            get { return name; }
            set 
            { 
                name = value; 
                
            }
        }
        private static Random rand = new Random();
        public Script(string file)
        {
            InitializeComponent();
            timer.Start();

            name = Path.GetFileName(file);
            textBox.Text = File.ReadAllText(file);
        }
        public Script()
        {
            InitializeComponent();
            timer.Start();

            name = "Script" + rand.Next(0,1000).ToString();
            scriptLabel.Text = name;
        }
        private void runButton_Click(object sender, EventArgs e)
        {
            errorBox.Text = "";
            outputBox.Text = "";
            Run(textBox.Text);
        }
        public void Run(string code)
        {
            errorBox.Text = "";
            outputBox.Text = "";
            scriptString = textBox.Text;
            Thread t = new Thread(new ThreadStart(RunScript));
            t.Start();
        }
        public void Run()
        {
            scriptString = textBox.Text;
            Thread t = new Thread(new ThreadStart(RunScript));
            t.Start();
        }
        private static void RunCode()
        {
            Thread t = new Thread(new ThreadStart(RunScript));
            t.Start();
        }
        static string scriptString;
        static dynamic script;
        static object obj;
        static string output;
        static bool done = false;
        static Exception ex = null;

        public static void RunScript(string sc)
        {
            scriptString = sc;
            RunCode();
        }
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
            openFileDialog.InitialDirectory = Application.StartupPath + "\\Scripts";
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            textBox.Text = File.ReadAllText(openFileDialog.FileName);
            scriptLabel.Text = Path.GetFileName(openFileDialog.FileName);
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (ex != null)
                errorBox.Text = ex.Message;
            outputBox.Text = output;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            scriptLabel.Text = saveFileDialog.FileName;
            File.WriteAllText(saveFileDialog.FileName, textBox.Text);
        }

        public override string ToString()
        {
            return name.ToString();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
