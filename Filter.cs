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
    public partial class Filter : Form
    {

        public Filter()
        {
            InitializeComponent();
            Init();
        }
        private void UpdateView()
        {
            ImageView.viewer.UpdateView();
        }
        public class Node
        {
            public TreeNode node;
            public Filt filt;
            public Node(TreeNode nod, Filt f)
            {
                node = nod;
                filt = f;
            }
            public override string ToString()
            {
                return filt.name.ToString();
            }
        }
        public void Init()
        {
            foreach (Filt.Type t in (Filt.Type[])Enum.GetValues(typeof(Filt.Type)))
            {
                TreeNode gr = new TreeNode();
                gr.Text = t.ToString();
                filterView.Nodes.Add(gr);
            }
            foreach (Filt f in Filters.filters.Values)
            {
                TreeNode nod = new TreeNode();
                nod.Text = f.name;
                Node node = new Node(nod, f);
                nod.Tag = node;
                filterView.Nodes[(int)f.type].Nodes.Add(nod);
            }
        }

        private void applyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filterView.SelectedNode==null)
                return;
            Node n = (Node)filterView.SelectedNode.Tag;

            Filters.Apply(ImageView.viewer.image,n.filt.name);
            Recorder.AddLine("Filters.Apply(" + '"' + ImageView.viewer.image.ID + '"' + "," + '"' + n.filt.name + '"' + ", false, ImageView.viewer.Index");
            UpdateView();
        }
        private void applyStackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (filterView.SelectedNode == null)
                return;
            Node n = (Node)filterView.SelectedNode.Tag;
            Filters.Apply(ImageView.viewer.image, n.filt.name);
            Recorder.AddLine("Filters.Apply(" + '"' + ImageView.viewer.image.ID + '"' + "," + '"' + n.filt.name + '"' + ", false, ImageView.viewer.Index");
            UpdateView();
        }
    }
}
