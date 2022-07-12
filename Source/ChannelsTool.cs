using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace Bio
{
    public partial class ChannelsTool : Form
    {
        private HistogramControl hist;
        public List<Channel> Channels
        {
            get
            {
                return ImageView.viewer.image.Channels;
            }
        }
        public ChannelsTool()
        {
            InitializeComponent();
            foreach (Channel item in Channels)
            {
                channelsBox.Items.Add(item);
            }
            channelsBox.SelectedIndex = 0;
            minBox.Value = Channels[0].Min;
            maxBox.Value = Channels[0].Max;
            hist = new HistogramControl(Channels[channelsBox.SelectedIndex].statistics);
            MouseWheel += new System.Windows.Forms.MouseEventHandler(ChannelsTool_MouseWheel);
            statsPanel.Controls.Add(hist);
        }

        private void minBox_ValueChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            Channels[channelsBox.SelectedIndex].Min = (int)minBox.Value;
            if (hist != null)
            {
                hist.Min = (int)minBox.Value;
                hist.Invalidate();
            }
            ImageView.viewer.UpdateView();
        }

        private void maxBox_ValueChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            Channel c = (Channel)channelsBox.SelectedItem;
            Channels[channelsBox.SelectedIndex].Max = (int)maxBox.Value;
            if (hist != null)
            {
                hist.Max = (int)maxBox.Value;
                hist.Invalidate();
            }
            ImageView.viewer.UpdateView();
        }
        private void channelsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            minBox.Value = Channels[channelsBox.SelectedIndex].Min;
            maxBox.Value = Channels[channelsBox.SelectedIndex].Max;
            if (hist != null)
            {
                hist.Statistics = Channels[channelsBox.SelectedIndex].statistics;
                hist.Invalidate();
            }
            ImageView.viewer.UpdateView();
        }

        private void maxUintBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = int.Parse((string)maxUintBox.SelectedItem,System.Globalization.CultureInfo.InvariantCulture);
            if(i<=maxBox.Maximum)
            maxBox.Value = i;

        }

        private void ChannelsTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void setMaxAllBut_Click(object sender, EventArgs e)
        {
            foreach (Channel c in Channels)
            {
                c.Max = (int)maxBox.Value;
            }
            ImageView.viewer.UpdateView();
        }

        private void setMinAllBut_Click(object sender, EventArgs e)
        {
            foreach (Channel c in Channels)
            {
                c.Min = (int)minBox.Value;
            }
            ImageView.viewer.UpdateView();
        }

        private void ChannelsTool_Activated(object sender, EventArgs e)
        {
            if (channelsBox.SelectedItem == null)
                return;
            Channel c = (Channel)channelsBox.SelectedItem;
            ZCT coord = ImageView.viewer.GetCoordinate();
            ImageView.viewer.image.GetBitmap(coord);
            Bitmap b = ImageView.viewer.image.GetBitmap(coord);
            if (hist.Statistics.BitsPerPixel > 8)
            {
                maxBox.Maximum = ushort.MaxValue;
                maxGraphBox.Maximum = ushort.MaxValue;
            }
            else
            {
                maxBox.Maximum = 255;
                maxGraphBox.Maximum = 255;
            }
        }

        private void ChannelsTool_ResizeEnd(object sender, EventArgs e)
        {
            hist.Invalidate();
        }

        private void maxUintBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = int.Parse((string)maxUintBox2.SelectedItem, System.Globalization.CultureInfo.InvariantCulture);
            if(i <= maxGraphBox.Maximum)
            maxGraphBox.Value = i;
        }

        private void minGraphBox_ValueChanged(object sender, EventArgs e)
        {
            if (hist != null)
            {
                hist.GraphMin = (int)minGraphBox.Value;
                hist.Invalidate();
            }
        }

        private void maxGraphBox_ValueChanged(object sender, EventArgs e)
        {
            if (hist != null)
            {
                hist.GraphMax = (int)maxGraphBox.Value;
                hist.Invalidate();
            }
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void binBox_ValueChanged(object sender, EventArgs e)
        {
            if (hist != null)
            {
                hist.Bin = (int)binBox.Value;
                hist.Invalidate();
            }
        }

        private void ChannelsTool_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void ChannelsTool_MouseHover(object sender, EventArgs e)
        {

        }

        private void ChannelsTool_MouseDown(object sender, MouseEventArgs e)
        {
            
        }
        private bool pressedX1 = false;
        private bool pressedX2 = false;
        private void ChannelsTool_MouseMove(object sender, MouseEventArgs e)
        {
            //The event is fired twice per XButton click so we need to only process one of them.
            if(e.Button == MouseButtons.XButton1)
            {
                if (channelsBox.SelectedIndex < channelsBox.Items.Count-1)
                {
                    if (pressedX1 == false)
                    {
                        channelsBox.SelectedIndex++;
                        pressedX1 = true;
                        return;
                    }
                }
            }
            pressedX1 = false;
            if (e.Button == MouseButtons.XButton2)
            {
                if (channelsBox.SelectedIndex > 0)
                {
                    if (pressedX2 == false)
                    {
                        channelsBox.SelectedIndex--;
                        pressedX2 = true;
                        return;
                    }
                }
            }
            pressedX2 = false;
        }

        private void ChannelsTool_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta == 0)
                return;
            int i = 10;
            if (e.Delta > 0)
            if (maxGraphBox.Value + i < maxGraphBox.Maximum)
            {
                maxGraphBox.Value += i;
                hist.Invalidate();
                return;
            }
            if (e.Delta < 0)
            if (maxGraphBox.Value - i > maxGraphBox.Minimum)
            {
                maxGraphBox.Value -= i;
                hist.Invalidate();
                return;
            }
        }

        private void stackHistoBox_CheckedChanged(object sender, EventArgs e)
        {
            hist.StackHistogram = stackHistoBox.Checked;
            hist.Invalidate();
        }
    }
}
