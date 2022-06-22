using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;

namespace BioImage
{
    public partial class ChannelsTool : Form
    {
        public List<Channel> Channels;
        private HistogramControl hist;
        public void UpdateChannels()
        {
            if (ImageView.viewer != null)
            {
                ImageView.viewer.image.Channels = Channels;
                ImageView.viewer.UpdateView();
            }
        }
        public ChannelsTool()
        {

        }
        public ChannelsTool(List<Channel> channels)
        {
            InitializeComponent();
            Channels = channels;
            foreach (Channel item in Channels)
            {
                channelsBox.Items.Add(item);
            }
            channelsBox.SelectedIndex = 0;
            minBox.Value = Channels[0].Min;
            maxBox.Value = Channels[0].Max;
            hist = new HistogramControl(ImageView.viewer.image.Statistics);
            statsPanel.Controls.Add(hist);
        }

        private void minBox_ValueChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            Channel c = (Channel)channelsBox.SelectedItem;
            Channels[channelsBox.SelectedIndex].Min = (int)minBox.Value;
            UpdateChannels();
            if (hist != null)
            {
                hist.Min = (int)minBox.Value;
                hist.Invalidate();
            }
        }

        private void maxBox_ValueChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            Channel c = (Channel)channelsBox.SelectedItem;
            Channels[channelsBox.SelectedIndex].Max = (int)maxBox.Value;
            UpdateChannels();
            if (hist != null)
            {
                hist.Max = (int)maxBox.Value;
                hist.Invalidate();
            }
        }
        private void channelsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            minBox.Value = Channels[channelsBox.SelectedIndex].Min;
            maxBox.Value = Channels[channelsBox.SelectedIndex].Max;
            if (hist != null)
            {
                hist.Statistics = Channels[channelsBox.SelectedIndex].stats;
                hist.Invalidate();
            }
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
            UpdateChannels();
        }

        private void setMinAllBut_Click(object sender, EventArgs e)
        {
            foreach (Channel c in Channels)
            {
                c.Min = (int)minBox.Value;
            }
            UpdateChannels();
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
    }
}
