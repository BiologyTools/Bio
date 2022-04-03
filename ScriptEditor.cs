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
    public partial class ScriptEditor : Form
    {
        private ScriptRunner.Script script = null;
        private static Random rand = new Random();
        public ScriptEditor(string file)
        {
            InitializeComponent();
            timer.Start();
            script = new ScriptRunner.Script(Path.GetFileName(file), File.ReadAllText(file));
        }
        public ScriptEditor()
        {
            InitializeComponent();
            timer.Start();
            script = new ScriptRunner.Script("Script" + rand.Next(0, 1000).ToString(), "");
            scriptLabel.Text = script.name;
        }
        private void runButton_Click(object sender, EventArgs e)
        {
            errorBox.Text = "";
            outputBox.Text = "";
            script.scriptString = textBox.Text;
            script.Run();
        }

        public static string scriptName = "";
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
            if (script.ex != null)
                errorBox.Text = script.ex.Message;
            outputBox.Text = script.output;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            scriptLabel.Text = saveFileDialog.FileName;
            File.WriteAllText(saveFileDialog.FileName, textBox.Text);
            script.name = scriptLabel.Text;
        }
        public override string ToString()
        {
            return script.ToString();
        }
    }
}
