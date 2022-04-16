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
        public class Node
        {
            public TreeNode node;
            public enum DataType
            {
                image,
                buf,
                roi,
                text
            }
            private DataType type;
            public DataType Type
            {
                get { return type; }
                set { type = value; }
            }
            private object obj;
            public object Object
            {
                get { return obj; }
                set { obj = value; }
            }
            public Node(object data,DataType typ)
            {
                type = typ;
                obj = data;
                node = new TreeNode();
                node.Tag = this;
                node.Text = obj.ToString();
                node.ForeColor = Color.White;
            }
            public string Text
            {
                get { return node.Text; }
                set { node.Text = value; }
            }
        }
        public MainForm(string[] args)
        {
            InitializeComponent();
            UpdateNodes();
            if (args.Length > 0)
            {
                ImageViewer viewer = new ImageViewer(args[0]);
                viewer.Show();
            }
        }

        public void UpdateNodes()
        {
            
            treeView.Nodes.Clear();
            TreeNode images = new TreeNode();
            images.Text = "BioImages";
            images.ForeColor = Color.White;
            foreach (BioImage item in Table.bioimages.Values)
            {
                //TreeNode node = new TreeNode();
                Node tree = new Node(item, Node.DataType.image);


                Node implanes = new Node(item, Node.DataType.text);
                implanes.Text = "Planes";

                foreach (BioImage.Buf buf in item.Buffers)
                {
                    Node plane = new Node(item, Node.DataType.buf);
                    plane.Text = buf.info.stringId;
                    implanes.node.Nodes.Add(plane.node);
                }
                tree.node.Nodes.Add(implanes.node);

                Node rois = new Node(item, Node.DataType.text);
                rois.Text = "ROI";

                foreach (BioImage.Annotation an in item.Annotations)
                {
                    Node roi = new Node(an, Node.DataType.text);
                    rois.node.Nodes.Add(roi.node);
                }
                tree.node.Nodes.Add(rois.node);
                images.Nodes.Add(tree.node);
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
