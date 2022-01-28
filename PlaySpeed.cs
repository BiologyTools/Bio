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
    public partial class PlaySpeed : Form
    {
        public PlaySpeed(bool timeEnabled,int zSpeed,int timeSpeed)
        {
            InitializeComponent();
            if (!timeEnabled)
                timePlayspeed.Enabled = false;
            else
                timePlayspeed.Enabled = true;
            timePlayspeed.Value = timeSpeed;
            zPlayspeed.Value = zSpeed;
        }

        public int TimePlayspeed
        {
            get
            {
                return (int)timePlayspeed.Value;
            }
        }

        public int ZPlayspeed
        {
            get
            {
                return (int)zPlayspeed.Value;
            }
        }

        private void PlaySpeed_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void timePlayspeed_ValueChanged(object sender, EventArgs e)
        {

        }
    }
}
