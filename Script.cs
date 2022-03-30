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

namespace BioImage
{
    public partial class Script : Form
    {
        public Script()
        {
            InitializeComponent();
        }
        private void runButton_Click(object sender, EventArgs e)
        {
            string script = textBox1.Text;
            dynamic image = CSScript.Evaluator.LoadCode(script);
            object sc = image.Load();
            outputBox.Text = sc.ToString();
        }
    }
}
