using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BioImage
{
    public partial class ChannelsTool : Form
    {
        public List<BioImage.Channel> Channels;

        public void UpdateChannels()
        {
            if (ImageView.viewer != null)
            {
                ImageView.viewer.image.Channels = Channels;
                ImageView.viewer.UpdateView();
            }
        }
        public ChannelsTool(List<BioImage.Channel> channels)
        {
            InitializeComponent();
            Channels = channels;
            foreach (BioImage.Channel item in Channels)
            {
                channelsBox.Items.Add(item);
            }
            channelsBox.SelectedIndex = 0;
            minBox.Value = Channels[0].Min;
            maxBox.Value = Channels[0].Max;
        }

        private void minBox_ValueChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            BioImage.Channel c = (BioImage.Channel)channelsBox.SelectedItem;
            Channels[channelsBox.SelectedIndex].Min = (int)minBox.Value;
            UpdateChannels();
        }

        private void maxBox_ValueChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            BioImage.Channel c = (BioImage.Channel)channelsBox.SelectedItem;
            Channels[channelsBox.SelectedIndex].Max = (int)maxBox.Value;
            UpdateChannels();
        }
        private void channelsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            minBox.Value = Channels[channelsBox.SelectedIndex].Min;
            maxBox.Value = Channels[channelsBox.SelectedIndex].Max;
        }

        private void maxUintBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int i = int.Parse((string)maxUintBox.SelectedItem,System.Globalization.CultureInfo.InvariantCulture);
            maxBox.Value = i;

        }

        private void ChannelsTool_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void setMaxAllBut_Click(object sender, EventArgs e)
        {
            foreach (BioImage.Channel c in Channels)
            {
                c.Max = (int)maxBox.Value;
            }
            UpdateChannels();
        }

        private void setMinAllBut_Click(object sender, EventArgs e)
        {
            foreach (BioImage.Channel c in Channels)
            {
                c.Min = (int)minBox.Value;
            }
            UpdateChannels();
        }
    }
}
