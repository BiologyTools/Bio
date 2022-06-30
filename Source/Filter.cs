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
            ImageView.viewer.UpdateStatus();
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

        private void ApplyFilter(bool inPlace)
        {
            if (filterView.SelectedNode==null)
                return;
            Node n = (Node)filterView.SelectedNode.Tag;
            if (n.filt.type == Filt.Type.Base)
            {
                Filters.BaseFilter(ImageView.viewer.image.ID, n.filt.name, false);
                Recorder.AddLine("Filters.BaseFilter(" + '"' + ImageView.viewer.image.ID +
                    '"' + "," + '"' + n.filt.name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            if (n.filt.type == Filt.Type.Base2)
            {
                ApplyFilter two = new ApplyFilter(true);
                if (two.ShowDialog() != DialogResult.OK)
                    return;
                Filters.BaseFilter2(two.ImageA.ID, two.ImageB.ID, n.filt.name, false);
                //Filters.BaseInPlaceFilter2(two.ImageA.ID, two.ImageB.ID, n.filt.name, false);
                Recorder.AddLine("Filters.BaseFilter2(" + '"' + two.ImageA.ID + '"' + "," +
                   '"' + two.ImageB.ID + '"' + "," + '"' + n.filt.name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            else
            if (n.filt.type == Filt.Type.InPlace)
            {
                Filters.BaseInPlaceFilter(ImageView.viewer.image.ID, n.filt.name, false);
                Recorder.AddLine("Filters.BaseInPlaceFilter(" + '"' + ImageView.viewer.image.ID +
                    '"' + "," + '"' + n.filt.name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            else
            if (n.filt.type == Filt.Type.InPlace2)
            {
                ApplyFilter two = new ApplyFilter(true);
                if (two.ShowDialog() != DialogResult.OK)
                    return;
                Filters.BaseInPlaceFilter2(two.ImageA.ID, two.ImageB.ID, n.filt.name, false);
                Recorder.AddLine("Filters.BaseInPlaceFilter2(" + '"' + two.ImageA.ID + '"' + "," +
                   '"' + two.ImageB.ID + '"' + "," + '"' + n.filt.name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            else
            if (n.filt.type == Filt.Type.InPlacePartial)
            {
                Filters.BaseInPlacePartialFilter(ImageView.viewer.image.ID, n.filt.name, false);
                Recorder.AddLine("Filters.BaseInPlacePartialFilter(" + '"' + ImageView.viewer.image.ID +
                    '"' + "," + '"' + n.filt.name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            else
            if (n.filt.type == Filt.Type.Resize)
            {
                ApplyFilter two = new ApplyFilter(false);
                Filters.BaseResizeFilter(ImageView.viewer.image.ID, n.filt.name, false, two.W,two.H);
                Recorder.AddLine("Filters.BaseResizeFilter(" + '"' + ImageView.viewer.image.ID +
                    '"' + "," + '"' + n.filt.name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            else
            if (n.filt.type == Filt.Type.Rotate)
            {
                ApplyFilter two = new ApplyFilter(false);
                Filters.BaseRotateFilter(ImageView.viewer.image.ID, n.filt.name, false, two.Angle, two.Color);
                Recorder.AddLine("Filters.BaseRotateFilter(" + '"' + ImageView.viewer.image.ID +
                    '"' + "," + '"' + n.filt.name + '"' + "," + inPlace.ToString().ToLower() + ");");
            }
            else
            if (n.filt.type == Filt.Type.Transformation)
            {
                ApplyFilter two = new ApplyFilter(false);
                if (two.ShowDialog() != DialogResult.OK)
                    return;
                if (n.filt.name == "Crop")
                {
                    Filters.Crop(two.ImageA.ID, two.Rectangle);
                }
                else
                {
                    Filters.BaseTransformationFilter(ImageView.viewer.image.ID, n.filt.name, false, two.Angle);
                    Recorder.AddLine("Filters.BaseTransformationFilter(" + '"' + ImageView.viewer.image.ID +
                        '"' + "," + '"' + n.filt.name + '"' + "," + inPlace.ToString().ToLower() + ");");
                }
            }
            UpdateView();
        }

        private void applyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(false);
        }

        private void applyRGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ApplyFilter(true);
        }
    }
}
