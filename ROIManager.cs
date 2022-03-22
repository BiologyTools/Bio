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
        public void updateROI(int index, BioImage.Annotation an)
        {
            if (ImageView.selectedImage == null)
                return;
            ImageView.selectedImage.Annotations[index] = an;
        }
        private void xBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.X = (double)xBox.Value;
        }

        private void yBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.Y = (double)yBox.Value;
        }

        private void wBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            if(anno.type == BioImage.Annotation.Type.Rectangle || anno.type == BioImage.Annotation.Type.Ellipse)
                anno.W = (double)wBox.Value;
        }

        private void hBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            if (anno.type == BioImage.Annotation.Type.Rectangle || anno.type == BioImage.Annotation.Type.Ellipse)
                anno.H = (double)hBox.Value;
        }
        private void sBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
        }
        private void zBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.Z = (int)zBox.Value;
        }

        private void cBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.C = (int)cBox.Value;
        }

        private void tBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.T = (int)cBox.Value;
        }

        private void rBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb((byte)rBox.Value, (byte)gBox.Value, (byte)bBox.Value);
        }

        private void gBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb((byte)rBox.Value, (byte)gBox.Value, (byte)bBox.Value);
        }

        private void bBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb((byte)rBox.Value, (byte)gBox.Value, (byte)bBox.Value);
        }

        private void typeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.type = (BioImage.Annotation.Type)typeBox.SelectedItem;
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.text = textBox.Text;
        }

        private void idBox_TextChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.id = idBox.Text;
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
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (ImageView.selectedImage == null)
                return;
            ImageView.selectedImage.Annotations.Add(anno);
        }
    }
}
