using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace BioImage
{
    public partial class MainForm : Form
    {
        public static Scripting runner = null;
        public static Recorder recorder = null;
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
            Init();
            InitNodes();
            if (args.Length > 0)
            {
                ImageViewer viewer = new ImageViewer(args[0]);
                viewer.Show();
            }
        }

        private static void Init()
        {
            runner = new Scripting();
            recorder = new Recorder();
        }

        public void InitNodes()
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

                foreach (Buf buf in item.Buffers)
                {
                    Node plane = new Node(buf, Node.DataType.buf);
                    plane.Text = buf.info.stringId;
                   
                    implanes.node.Nodes.Add(plane.node);
                }
                tree.node.Nodes.Add(implanes.node);

                Node rois = new Node(item, Node.DataType.text);
                rois.Text = "ROI";

                foreach (Annotation an in item.Annotations)
                {
                    Node roi = new Node(an, Node.DataType.roi);
                    rois.node.Nodes.Add(roi.node);
                }
                tree.node.Nodes.Add(rois.node);
                images.Nodes.Add(tree.node);
            }
            treeView.Nodes.Add(images);
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            InitNodes();
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
            InitNodes();
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImageViewer iv = new ImageViewer("");
            iv.Show();
        }

        private void treeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (treeView.SelectedNode == null)
                return;

            Node node = (Node)treeView.SelectedNode.Tag;
            if(node!=null)
            if(node.Type == Node.DataType.buf)
            {
                Buf buf = (Buf)node.Object;
                int ind = int.Parse(Path.GetFileName(buf.ToString()));
                string name = buf.ToString();
                int inds = name.IndexOf("/s");
                string filename = name.Substring(0, inds);
                ImageViewer v = Table.GetViewer(Path.GetFileName(filename));
                if(v!=null)
                    v.viewer.SetCoordinate(buf.info.Coordinate.S, buf.info.Coordinate.Z, buf.info.Coordinate.C, buf.info.Coordinate.T);
            }
            else
            if(node.Type == Node.DataType.roi)
            {
                Annotation an = (Annotation)node.Object;
                string name = node.node.Parent.Parent.Text;
                ImageViewer v = Table.GetViewer(name);
                if (v != null)
                    v.viewer.SetCoordinate(an.coord.S, an.coord.Z, an.coord.C, an.coord.T);
            }
        }

        private void scriptRunnerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            runner.WindowState = FormWindowState.Normal;
            runner.Show();
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Node node = (Node)treeView.SelectedNode.Tag;
            if(node.Type == Node.DataType.roi)
            {
                Annotation an = (Annotation)node.Object;
                Node nod = (Node)treeView.SelectedNode.Parent.Tag;
                BioImage im = (BioImage)nod.Object;
                im.Annotations.Remove(an);
            }
        }

        private void scriptRecorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            recorder.WindowState = FormWindowState.Maximized;
            recorder.Show();
        }
    }
}
