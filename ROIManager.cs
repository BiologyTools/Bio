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
            foreach (Annotation.Type item in Enum.GetValues(typeof(Annotation.Type)))
            {
                typeBox.Items.Add(item);
            }
        }

        public void GetROIsFromImage()
        {
            if (ImageView.selectedImage == null)
                return;
            roiView.Items.Clear();
            foreach (Annotation an in ImageView.selectedImage.Annotations)
            {
                ListViewItem it = new ListViewItem();
                it.Tag = an;
                it.Text = an.ToString();
                roiView.Items.Add(it);
            }
        }

        public void UpdateOverlay()
        {
            if(ImageView.viewer != null)
                ImageView.viewer.UpdateOverlay();
        }
        public void updateROI(int index, Annotation an)
        {
            if (ImageView.selectedImage == null)
                return;
            ImageView.selectedImage.Annotations[index] = an;
            UpdateOverlay();
        }
        private void xBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.X = (double)xBox.Value;
            UpdateOverlay();
        }

        private void yBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.Y = (double)yBox.Value;
            UpdateOverlay();
        }

        private void wBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            if(anno.type == Annotation.Type.Rectangle || anno.type == Annotation.Type.Ellipse)
                anno.W = (double)wBox.Value;
            UpdateOverlay();
        }

        private void hBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            if (anno.type == Annotation.Type.Rectangle || anno.type == Annotation.Type.Ellipse)
                anno.H = (double)hBox.Value;
            UpdateOverlay();
        }
        private void sBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            UpdateOverlay();
        }
        private void zBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.Z = (int)zBox.Value;
            UpdateOverlay();
        }

        private void cBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.C = (int)cBox.Value;
            UpdateOverlay();
        }
        private void tBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.coord.T = (int)cBox.Value;
            UpdateOverlay();
        }

        private void rBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb((byte)rBox.Value, anno.strokeColor.G, anno.strokeColor.B);
            UpdateOverlay();
        }

        private void gBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb(anno.strokeColor.R, (byte)gBox.Value, anno.strokeColor.B);
            UpdateOverlay();
        }

        private void bBox_ValueChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.strokeColor = Color.FromArgb(anno.strokeColor.R, anno.strokeColor.G, (byte)bBox.Value);
            UpdateOverlay();
        }

        private void typeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.type = (Annotation.Type)typeBox.SelectedItem;
            UpdateOverlay();
        }

        private void textBox_TextChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.Text = textBox.Text;
            UpdateOverlay();
        }

        private void idBox_TextChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            anno.id = idBox.Text;
            UpdateOverlay();
        }

        private void ROIManager_Activated(object sender, EventArgs e)
        {
            if (ImageView.selectedImage == null)
                return;
            string n = System.IO.Path.GetFileName(ImageView.selectedImage.Filename);
            if (imageNameLabel.Text != n)
                imageNameLabel.Text = n;
            GetROIsFromImage();
        }
        public Annotation anno = new Annotation();
        private void roiView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            ListViewItem it = roiView.SelectedItems[0];
            anno = (Annotation)it.Tag;
            if(ImageView.viewer!=null)
            ImageView.viewer.SetCoordinate(anno.coord.S, anno.coord.Z, anno.coord.C, anno.coord.T);
            if(anno.type == Annotation.Type.Line || anno.type == Annotation.Type.Polygon ||
               anno.type == Annotation.Type.Polyline)
            {
                xBox.Enabled = false;
                yBox.Enabled = false;
                wBox.Enabled = false;
                hBox.Enabled = false;
            }
            else
            {
                xBox.Enabled = true;
                yBox.Enabled = true;
                wBox.Enabled = true;
                hBox.Enabled = true;
            }
            if(anno.type == Annotation.Type.Rectangle || anno.type == Annotation.Type.Ellipse)
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
            strokeWBox.Value = (decimal)anno.strokeWidth;
            idBox.Text = anno.id;
            textBox.Text = anno.Text;
            typeBox.SelectedIndex = (int)anno.type;
            UpdatePointBox();
        }

        private void updateBut_Click(object sender, EventArgs e)
        {
            if (roiView.SelectedItems.Count == 0)
                return;
            if (ImageView.selectedImage == null)
                return;
            ImageView.selectedImage.Annotations[roiView.SelectedIndices[0]] = anno;
            UpdateOverlay();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            if (ImageView.selectedImage == null)
                return;
            if (anno == null)
                return;
            ImageView.selectedImage.Annotations.Add(anno);
            UpdateOverlay();
        }

        private void showBoundsBox_CheckedChanged(object sender, EventArgs e)
        {
            ImageView.showBounds = showBoundsBox.Checked;
            UpdateOverlay();
        }

        private void showTextBox_CheckedChanged(object sender, EventArgs e)
        {
            ImageView.showText = showTextBox.Checked;
            UpdateOverlay();
        }

        private void pointXBox_ValueChanged(object sender, EventArgs e)
        {
            if (anno == null)
                return;
            if (anno.type == Annotation.Type.Rectangle || anno.type == Annotation.Type.Ellipse)
                return;
            anno.UpdatePoint(new PointD((double)pointXBox.Value, (double)pointYBox.Value),(int)pointIndexBox.Value);
            UpdateOverlay();
        }

        private void pointYBox_ValueChanged(object sender, EventArgs e)
        {
            if (anno == null)
                return;
            if (anno.type == Annotation.Type.Rectangle || anno.type == Annotation.Type.Ellipse)
                return;
            anno.UpdatePoint(new PointD((double)pointXBox.Value, (double)pointYBox.Value), (int)pointIndexBox.Value);
            UpdateOverlay();
        }

        public bool autoUpdate = true;

        public void UpdatePointBox()
        {
            if (anno == null)
                return;
            PointD d = anno.GetPoint((int)pointIndexBox.Value);
            pointXBox.Value = (int)d.X;
            pointYBox.Value = (int)d.Y;
        }
        private void pointIndexBox_ValueChanged(object sender, EventArgs e)
        {
            UpdatePointBox();
        }

        private void fontBut_Click(object sender, EventArgs e)
        {
            if (anno == null)
                return;
            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;
            anno.font = fontDialog.Font;
        }

        private void strokeWBox_ValueChanged(object sender, EventArgs e)
        {
            if (anno == null)
                return;
            anno.strokeWidth = (int)strokeWBox.Value;
            UpdateOverlay();
        }

        private void selectBoxSize_ValueChanged(object sender, EventArgs e)
        {
            ImageView.viewer.UpdateSelectBoxSize((float)selectBoxSize.Value);
            UpdateOverlay();
        }

        private void rChBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ImageView.viewer == null)
                return;
            ImageView.viewer.showRROIs = rChBox.Checked;
            UpdateOverlay();
        }

        private void gChBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ImageView.viewer == null)
                return;
            ImageView.viewer.showGROIs = gChBox.Checked;
            UpdateOverlay();
        }

        private void bChBox_CheckedChanged(object sender, EventArgs e)
        {
            if (ImageView.viewer == null)
                return;
            ImageView.viewer.showBROIs = bChBox.Checked;
            UpdateOverlay();
        }

        private void ROIManager_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.WindowState = FormWindowState.Minimized;
        }
    }
}
