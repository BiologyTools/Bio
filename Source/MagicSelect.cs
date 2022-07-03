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
    public partial class MagicSelect : Form
    {
        public MagicSelect(int index)
        {
            InitializeComponent();
            thBox.SelectedIndex = index;
        }
        public bool Numeric = false;

        public int Threshold
        {
            get
            {
                return (int)numBox.Value;
            }
        }
        public int Index
        {
            get
            {
                return thBox.SelectedIndex;
            }
        }
        private void numericBox_CheckedChanged(object sender, EventArgs e)
        {
            Numeric = numericBox.Checked;
        }

        private void okBut_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
    }
}
