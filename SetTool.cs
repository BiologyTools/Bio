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
    public partial class SetTool : Form
    {

        public SetTool()
        {
            InitializeComponent();
            if(Tools.tools.Count == 0)
            foreach (Tools.Tool tool in Tools.tools)
            {
                toolsBox.Items.Add(tool);
            }
        }
        public Tools.Tool Tool
        {
            get { return (Tools.Tool)Tools.tools[toolsBox.SelectedIndex]; }
        }
        private void toolsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            
        }
    }
}
