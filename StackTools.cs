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
    public partial class StackTools : Form
    {
        public StackTools()
        {
            InitializeComponent();
            UpdateStacks();
        }

        public void UpdateStacks()
        {
            stackABox.Items.Clear();
            stackBBox.Items.Clear();    
            foreach (BioImage b in Table.bioimages.Values)
            {
                stackABox.Items.Add(b);
                stackBBox.Items.Add(b);
            }
        }
        public BioImage ImageA
        {
            get { return (BioImage)stackABox.SelectedItem; }
        }
        public BioImage ImageB
        {
            get { return (BioImage)stackBBox.SelectedItem; }
        }
        private void substackBut_Click(object sender, EventArgs e)
        {
            if (stackABox.SelectedIndex == -1)
                return;
            BioImage b = new BioImage(ImageA, ImageA.Filename, 0, (int)zStartBox.Value, (int)zEndBox.Value, (int)cStartBox.Value, (int)cEndBox.Value, (int)tStartBox.Value, (int)tEndBox.Value);
            ImageViewer iv = new ImageViewer(b);
            iv.Show();
            UpdateStacks();
        }

        private void stackABox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stackABox.SelectedIndex == -1)
                return;
            if (stackBBox.SelectedIndex == -1)
                return;
            if(stackABox.SelectedItem == stackBBox.SelectedItem)
            {
                //Same image selected for A & B
                MessageBox.Show("Same image selected for A & B. Change either A stack or B stack.");
                stackABox.SelectedIndex = -1;
            }
            else
            {
                zStartBox.Maximum = ImageA.SizeZ;
                cStartBox.Maximum = ImageA.SizeC;
                tStartBox.Maximum = ImageA.SizeT;

            }
        }

        private void stackBBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (stackABox.SelectedIndex == -1)
                return;
            if (stackBBox.SelectedIndex == -1)
                return;
            if (stackABox.SelectedItem == stackBBox.SelectedItem)
            {
                //Same image selected for A & B
                MessageBox.Show("Same image selected for A & B. Change either A stack or B stack.");
                stackBBox.SelectedIndex = -1;
            }
        }

        private void mergeBut_Click(object sender, EventArgs e)
        {
            if (stackABox.SelectedIndex == -1)
                return;
            if (stackBBox.SelectedIndex == -1)
                return;
            BioImage b = BioImage.MergeChannels(ImageA, ImageB);
            ImageViewer iv = new ImageViewer(b);
            iv.Show();
            UpdateStacks();
        }

        private void splitChannelsBut_Click(object sender, EventArgs e)
        {
            if (stackABox.SelectedIndex == -1)
                return;
            foreach (BioImage im in BioImage.SplitChannnelsToList(ImageA))
            {
                ImageViewer iv = new ImageViewer(im);
                iv.Show();
            }
            UpdateStacks();
        }

        private void StackTools_Activated(object sender, EventArgs e)
        {
            UpdateStacks();
        }

        private void setMaxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            zEndBox.Value = ImageA.SizeZ;
        }

        private void setMaxCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            cEndBox.Value = ImageA.SizeC;
        }

        private void setMaxTToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tEndBox.Value = ImageA.SizeT;
        }
    }
}
