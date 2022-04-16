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
    public partial class MainForm : Form
    {
        public MainForm(string[] args)
        {
            InitializeComponent();
            UpdateNodes();
            if(args.Length > 0)
                viewer = new ImageViewer(args[0]);
        }

        public void UpdateNodes()
        {
            treeView.Nodes.Clear();
            TreeNode images = new TreeNode();
            images.Text = "BioImages";
            images.ForeColor = Color.White;
            foreach (BioImage item in Table.bioimages.Values)
            {
                TreeNode node = new TreeNode();
                node.Text = item.Filename;
                node.ForeColor = Color.White;
                node.Tag = item;

                TreeNode implanes = new TreeNode();
                implanes.Text = "Planes";
                implanes.ForeColor = Color.White;

                foreach (BioImage.Buf buf in item.Buffers)
                {
                    TreeNode plane = new TreeNode();
                    plane.Text = buf.info.stringId;
                    plane.ForeColor = Color.White;
                    plane.Tag = buf;
                    implanes.Nodes.Add(plane);
                }
                node.Nodes.Add(implanes);


                TreeNode rois = new TreeNode();
                rois.Text = "ROI";
                rois.ForeColor = Color.White;

                foreach (BioImage.Annotation an in item.Annotations)
                {
                    TreeNode roi = new TreeNode();
                    roi.Text = an.ToString();
                    roi.ForeColor = Color.White;
                    roi.Tag = an;
                    rois.Nodes.Add(roi);
                }
                node.Nodes.Add(rois);

                images.Nodes.Add(node);
            }
            treeView.Nodes.Add(images);
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            UpdateNodes();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFilesDialog.ShowDialog() != DialogResult.OK)
                return;
            ImageViewer iv = new ImageViewer(openFilesDialog.FileNames);
            iv.Show();
        }
        private void refreshToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            UpdateNodes();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageViewer iv = new ImageViewer("");
            iv.Show();
        }
    }
}
