using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using CSScripting;
using csscript;
using CSScriptLib;

namespace BioImage
{
    public partial class ScriptRunner : Form
    {
        public static ScriptRunner runner;

        public static Dictionary<string, Script> Scripts = new Dictionary<string, Script>();
        public class Script
        {
            public string name;
            public string file;
            public string scriptString;
            public dynamic script;
            public object obj;
            public string output = "";
            public bool done = false;
            public Exception ex = null;
            public Thread thread;
            public Script(string file, string scriptStr)
            {
                name = Path.GetFileName(file);
                scriptString = scriptStr;
            }
            public static void Run(Script rn)
            {
                scriptName = rn.name;
                Thread t = new Thread(new ThreadStart(RunScript));
                t.Start();
            }

            private static string scriptName = "";
            private static void RunScript()
            {
                Script rn = Scripts[scriptName];
                rn.ex = null;
                try
                {
                    rn.done = false;
                    rn.script = CSScript.Evaluator.LoadCode(rn.scriptString);
                    rn.obj = rn.script.Load();
                    rn.output = rn.obj.ToString();
                    rn.done = true;
                }
                catch (Exception e)
                {
                    rn.ex = e;
                }
            }
            public void Run()
            {
                scriptName = this.name;
                thread = new Thread(new ThreadStart(RunScript));
                thread.Start();
            }
            public override string ToString()
            {
                if (thread != null)
                {
                    if (thread.ThreadState == ThreadState.Running)
                        return name.ToString() + ", Running";
                    else
                    if (thread.ThreadState == ThreadState.Stopped && ex == null && output != "")
                        return name.ToString() + ", Output: " + output;
                    else
                    if (thread.ThreadState == ThreadState.Stopped && ex != null)
                        return name.ToString() + ", Exception: " + ex.ToString();
                    else
                    if (thread.ThreadState == ThreadState.Stopped && ex == null)
                        return name.ToString() + ", Exception: " + ex.ToString();
                }
                return name.ToString();
            }
        }

        public void RefreshItems()
        {
            string dir = Application.StartupPath + "//" + "Scripts";
            foreach (string file in Directory.GetFiles(dir))
            {
                if (file.EndsWith(".cs"))
                {
                    if (!Scripts.ContainsKey(Path.GetFileName(file)))
                    {
                        //This is a script file.
                        Script sc = new Script(file, File.ReadAllText(file));
                        ListViewItem lv = new ListViewItem();
                        lv.Tag = sc;
                        lv.Text = sc.ToString();
                        scriptView.Items.Add(lv);
                        Scripts.Add(lv.Text, sc);
                    }
                }
            }
        }
        public void RefreshStatus()
        {
            foreach (ListViewItem item in scriptView.Items)
            {
                item.Text = ((Script)item.Tag).ToString();
            }
        }
        public ScriptRunner()
        {
            InitializeComponent();
            scriptView.MultiSelect = true;
            runner = this;
            RefreshItems();
            timer.Start();
        }

        public static void RunScriptByName(string name)
        {
            Scripts[name].Run();
        }

        public void Run()
        {
            if (scriptView.SelectedItems.Count == 0)
                    return;
            foreach (ListViewItem item in scriptView.SelectedItems)
            {
                //We run this script
                Script sc = (Script)item.Tag;
                sc.Run();
            }
        }

        public static void RunByName(string name)
        {
            Scripts[name].Run();
        }

        private void scriptView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Run();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run();
        }

        private void openScriptFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Application.StartupPath + "\\Scripts";
            System.Diagnostics.Process.Start("explorer.exe", path);
        }

        private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RefreshItems();
            RefreshStatus();
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if(this.WindowState != FormWindowState.Minimized)
            RefreshStatus();
        }

        private void ScriptRunner_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
