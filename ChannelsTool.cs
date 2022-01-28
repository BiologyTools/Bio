using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BioImage
{
    public partial class ChannelsTool : Form
    {
        public List<BioImage.Channel> Channels;
        public ChannelsTool(List<BioImage.Channel> channels)
        {
            InitializeComponent();
            Channels = channels;
            foreach (BioImage.Channel item in Channels)
            {
                channelsBox.Items.Add(item);
            }
            channelsBox.SelectedIndex = 0;
        }

        private void minBox_ValueChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            BioImage.Channel c = (BioImage.Channel)channelsBox.SelectedItem;
            Channels[channelsBox.SelectedIndex].min = (int)minBox.Value;
        }

        private void maxBox_ValueChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            BioImage.Channel c = (BioImage.Channel)channelsBox.SelectedItem;
            Channels[channelsBox.SelectedIndex].max = (int)maxBox.Value;

        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void channelsBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelsBox.SelectedIndex == -1)
                return;
            minBox.Value = Channels[channelsBox.SelectedIndex].min;
            maxBox.Value = Channels[channelsBox.SelectedIndex].max;
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
                c.max = (int)maxBox.Value;
            }
        }

        private void setMinAllBut_Click(object sender, EventArgs e)
        {
            foreach (BioImage.Channel c in Channels)
            {
                c.min = (int)minBox.Value;
            }
        }
    }
}
