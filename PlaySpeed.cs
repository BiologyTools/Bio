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
        public PlaySpeed(bool timeEnabled, bool cEnabled, int zFps, int timeFps, int cFps)
        {
            InitializeComponent();
            if (!timeEnabled)
            {
                timePlayspeed.Enabled = false;
                timeFpsBox.Enabled = false;
            }
            if (!cEnabled)
            {
                cPlayspeed.Enabled = false;
                cFpsBox.Enabled = false;
            }

            float zfps = zFps;
            float timefps = timeFps;
            zPlayspeed.Value = (int)Math.Floor((double)1000 / zfps);
            timePlayspeed.Value = (int)Math.Floor((double)1000 / timefps);
            cPlayspeed.Value = (int)Math.Floor((double)1000 / cFps);
            zFpsBox.Value = zFps;
            timeFpsBox.Value = timeFps;

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

        public int CPlayspeed
        {
            get
            {
                return (int)cPlayspeed.Value;
            }
        }

        public int ZFps
        {
            get
            {
                return (int)zFpsBox.Value;
            }
        }

        public int TimeFps
        {
            get
            {
                return (int)timeFpsBox.Value;
            }
        }

        public int CFps
        {
            get
            {
                return (int)cFpsBox.Value;
            }
        }

        private void PlaySpeed_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void zFpsBox_ValueChanged(object sender, EventArgs e)
        {
            float fps = (float)zFpsBox.Value;
            float ms = 1000 / fps;
            zPlayspeed.Value = (int)Math.Floor(ms);
        }

        private void timeFpsBox_ValueChanged(object sender, EventArgs e)
        {
            float fps = (float)timeFpsBox.Value;
            float ms = 1000 / fps;
            timePlayspeed.Value = (int)Math.Floor(ms);
        }

        private void cFpsBox_ValueChanged(object sender, EventArgs e)
        {
            float fps = (float)cFpsBox.Value;
            float ms = 1000 / fps;
            cPlayspeed.Value = (int)Math.Floor(ms);
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
