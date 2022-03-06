using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

namespace BioImage
{
    public partial class ImageViewer : Form
    {
        ImageView viewer;
        public ImageViewer(string[] arg)
        {
            InitializeComponent();
            //InitViewer("E:/TESTIMAGES/RGB3Test.czi");
            if (arg.Length == 0)
                return;
            string path = arg[0];
            InitViewer(path);
        }

        public void InitViewer(string file)
        {
            viewer = new ImageView(file);
            viewer.Dock = DockStyle.Fill;
            panel.Controls.Add(viewer);
        }

        public BioImage Image
        {
            get 
            {
                if (viewer == null)
                    return null;
                if (viewer.image == null)
                    return null;
                return viewer.image; 
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            InitViewer(openFileDialog.FileName);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.viewer != null)
            {
                viewer.image.Dispose();
                this.viewer.Dispose();
            }
            this.viewer = null;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Image == null)
                return;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            //viewer.image.Save(saveFileDialog.FileName);
            viewer.image.Save(saveFileDialog.FileName,0,viewer.image.imageCount);
        }

        private void ImageViewer_SizeChanged(object sender, EventArgs e)
        {
            if (this.Width <= MinimumSize.Width)
            {
                this.Width = MinimumSize.Width;
            }
            if (this.Height <= MinimumSize.Height)
            {
                this.Height = MinimumSize.Height;
            }
        }
        private void ImageViewer_Resize(object sender, EventArgs e)
        {
            if (this.Width <= MinimumSize.Width)
            {
                this.Width = MinimumSize.Width;
            }
            if (this.Height <= MinimumSize.Height)
            {
                this.Height = MinimumSize.Height;
            }
        }

        private void ImageViewer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyCode == Keys.S && e.Control)
            {
                saveToolStripMenuItem.PerformClick();
            }
            if (e.KeyCode == Keys.O && e.Control)
            {
                openToolStripMenuItem.PerformClick();
            }
        }
    }
}
