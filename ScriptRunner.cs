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

namespace BioImage
{
    public partial class ScriptRunner : Form
    {
        public ScriptRunner()
        {
            InitializeComponent();
            string dir = Application.StartupPath + "//" + "Scripts";
            foreach (string file in Directory.GetFiles(dir))
            {
                if(file.EndsWith(".cs"))
                {
                    //This is a script file.
                    Script sc = new Script(file);
                    ListViewItem lv = new ListViewItem();
                    lv.Tag = sc;
                    lv.Text = Path.GetFileName(file);
                    scriptView.Items.Add(lv);
                }
            }
        }

        public void Run()
        {
            if (scriptView.SelectedItems.Count == 0)
                return;
            //We run this script
            Script sc = (Script)scriptView.SelectedItems[0].Tag;
            sc.Run();
        }

        private void scriptView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            Run();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Run();
        }
    }
}
