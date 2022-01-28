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
    public partial class RangeTool : Form
    {
        public RangeTool(bool timeEnabled,int zmin,int zmax,int timeMin, int timeMax)
        {
            InitializeComponent();
            zMinBox.Value = zmin;
            zMaxBox.Value = zmax;
            if (!timeEnabled)
            {
                timeMinBox.Enabled = false;
                timeMaxBox.Enabled = false;
                return;
            }
            timeMinBox.Value = timeMin;
            timeMaxBox.Value = timeMax;
        }

        public int ZMin
        {
            get
            {
                return (int)zMinBox.Value;
            }
        }

        public int ZMax
        {
            get
            {
                return (int)zMaxBox.Value;
            }
        }

        public int TimeMin
        {
            get
            {
                return (int)timeMinBox.Value;
            }
        }

        public int TimeMax
        {
            get
            {
                return (int)timeMaxBox.Value;
            }
        }

        private void RangeTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void timeMinBox_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
