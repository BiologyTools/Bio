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
    public partial class ROIManager : Form
    {
        public ROIManager()
        {
            InitializeComponent();
            foreach (BioImage.Annotation.Type item in Enum.GetValues(typeof(BioImage.Annotation.Type)))
            {
                typeBox.Items.Add(item);
            }
        }

        public void GetROIsFromImage()
        {
            if (ImageView.selectedImage == null)
                return;
            roiView.Items.Clear();
            foreach (BioImage.Annotation an in ImageView.selectedImage.Annotations)
            {
                ListViewItem it = new ListViewItem();
                it.Tag = an;
                it.Text = an.ToString();
                roiView.Items.Add(it);
            }
        }

        public void UpdateView()
        {
            if(ImageView.viewer != null)
                ImageView.viewer.UpdateView();
        }
        public void updateROI(int index, BioImage.Annotation an)
        {
            if (ImageView.selectedImage == null)
                return;
            ImageView.selectedImage.Annotations[index] = an;
            UpdateView();
        }
        private void xBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.X = (double)xBox.Value;
            UpdateView();
        }

        private void yBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.Y = (double)yBox.Value;
            UpdateView();
        }

        private void wBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            if(anno.type == BioImage.Annotation.Type.Rectangle || anno.type == BioImage.Annotation.Type.Ellipse)
                anno.W = (double)wBox.Value;
            UpdateView();
        }

        private void hBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            if (anno.type == BioImage.Annotation.Type.Rectangle || anno.type == BioImage.Annotation.Type.Ellipse)
                anno.H = (double)hBox.Value;
            UpdateView();
        }
        private void sBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            UpdateView();
        }
        private void zBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.Z = (int)zBox.Value;
            UpdateView();
        }

        private void cBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.C = (int)cBox.Value;
            UpdateView();
        }
        private void tBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.T = (int)cBox.Value;
            UpdateView();
        }

        private void rBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb((byte)rBox.Value, anno.strokeColor.G, anno.strokeColor.B);
            UpdateView();
        }

        private void gBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb(anno.strokeColor.R, (byte)gBox.Value, anno.strokeColor.B);
            UpdateView();
        }

        private void bBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb(anno.strokeColor.R, anno.strokeColor.G, (byte)bBox.Value);
            UpdateView();
        }

        private void typeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.type = (BioImage.Annotation.Type)typeBox.SelectedItem;
            UpdateView();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.text = textBox.Text;
            UpdateView();
        }

        private void idBox_TextChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.id = idBox.Text;
            UpdateView();
        }

        private void ROIManager_Activated(object sender, EventArgs e)
        {
            if (ImageView.selectedImage == null)
                return;
            string n = System.IO.Path.GetFileName(ImageView.selectedImage.filename);
            if (imageNameLabel.Text != n)
                imageNameLabel.Text = n;
            GetROIsFromImage();
        }
        public BioImage.Annotation anno = new BioImage.Annotation();
        private void roiView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            ListViewItem it = roiView.SelectedItems[0];
            anno = (BioImage.Annotation)it.Tag;
            if(ImageView.viewer!=null)
            ImageView.viewer.SetCoordinate(anno.coord.S, anno.coord.Z, anno.coord.C, anno.coord.T);
            if(anno.type == BioImage.Annotation.Type.Line || anno.type == BioImage.Annotation.Type.Polygon ||
               anno.type == BioImage.Annotation.Type.Polyline)
            {
                xBox.Enabled = false;
                yBox.Enabled = false;
            }
            else
            {
                xBox.Enabled = true;
                yBox.Enabled = true;
            }
            if(anno.type == BioImage.Annotation.Type.Rectangle || anno.type == BioImage.Annotation.Type.Ellipse)
            {
                pointIndexBox.Enabled = false;
                pointXBox.Enabled = false;
                pointYBox.Enabled = false;
            }
            else
            {
                pointIndexBox.Enabled = true;
                pointXBox.Enabled = true;
                pointYBox.Enabled = true;
            }
            xBox.Value = (decimal)anno.X;
            yBox.Value = (decimal)anno.Y;
            wBox.Value = (decimal)anno.W;
            hBox.Value = (decimal)anno.H;
            sBox.Value = anno.coord.S;
            zBox.Value = anno.coord.Z;
            cBox.Value = anno.coord.C;
            tBox.Value = anno.coord.T;
            rBox.Value = anno.strokeColor.R;
            gBox.Value = anno.strokeColor.G;
            bBox.Value = anno.strokeColor.B;
            idBox.Text = anno.id;
            textBox.Text = anno.text;
            typeBox.SelectedIndex = (int)anno.type;
        }

        private void updateBut_Click(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            if (ImageView.selectedImage == null)
                return;
            ImageView.selectedImage.Annotations[roiView.SelectedIndices[0]] = anno;
            UpdateView();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (ImageView.selectedImage == null)
                return;
            ImageView.selectedImage.Annotations.Add(anno);
            UpdateView();
        }

        private void showBoundsBox_CheckedChanged(object sender, EventArgs e)
        {
            ImageView.showBounds = showBoundsBox.Checked;
            UpdateView();
        }

        private void showTextBox_CheckedChanged(object sender, EventArgs e)
        {
            ImageView.showText = showTextBox.Checked;
            UpdateView();
        }

        private void pointXBox_ValueChanged(object sender, EventArgs e)
        {
            if (anno == null)
                return;
            if (anno.type == BioImage.Annotation.Type.Rectangle || anno.type == BioImage.Annotation.Type.Ellipse)
                return;
            anno.UpdatePoint(new BioImage.PointD((double)pointXBox.Value, (double)pointYBox.Value),(int)pointIndexBox.Value);
            UpdateView();
        }

        private void pointYBox_ValueChanged(object sender, EventArgs e)
        {
            if (anno == null)
                return;
            if (anno.type == BioImage.Annotation.Type.Rectangle || anno.type == BioImage.Annotation.Type.Ellipse)
                return;
            anno.UpdatePoint(new BioImage.PointD((double)pointXBox.Value, (double)pointYBox.Value), (int)pointIndexBox.Value);
            UpdateView();
        }

        public bool autoUpdate = true;
        private void autoUpdateBut_CheckedChanged(object sender, EventArgs e)
        {
            autoUpdate = autoUpdateBut.Checked;
        }
    }
}
