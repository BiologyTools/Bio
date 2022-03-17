using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace BioImage
{
    public partial class ImageViewer : Form
    {
        Tools tools;
        private ImageView viewer = null;
        bool useFolderBrowser = false;

        public ImageViewer(string id)
        {
            InitializeComponent();
            string[] sp = id.Split('/');


            tools = new Tools();
            //viewer = new ImageView(sp[0],sp[1], sp[2] )
            string s = "E:/TESTIMAGES/TIF16B-T3C4Z6.tif";
            SetFolder(s, false, 0);
            viewer.image.SaveSeries(s, 0);
        }

        public ImageViewer(string[] arg)
        {
            InitializeComponent();
            tools = new Tools();


            string s = "E:/TESTIMAGES/TIF16B-T3C4Z6.tif";
            SetFolder(s, false, 0);
            viewer.image.SaveSeries(s, 0);


            if (arg.Length == 0)
                return;
            else
            if (arg.Length == 1)
            {

                SetFile(arg[0], 0, false);
            }
        }

        public void SetFile(string file, int seri, bool folder)
        {
            if (viewer == null)
            {
                viewer = new ImageView(file, seri, folder);
            }
            viewer.serie = seri;
            viewer.filepath = file;
            
            viewer.Dock = DockStyle.Fill;
            
            panel.Controls.Add(viewer);
            string name = Path.GetFileName(file);
            this.Text = name;
        }

        public void SetFolder(string file, bool folder, int seri)
        {
            viewer = new ImageView(file, seri, folder);
            viewer.Dock = DockStyle.Fill;
            panel.Controls.Add(viewer);
            viewer.serie = seri;
            string name = Path.GetFileName(file);
            this.Text = name;
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
            SetFolder(openFileDialog.FileName, false, 0);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (useFolderBrowser)
            {
                if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                    return;
                SetFile(openFileDialog.FileName, Image.serie, true);
            }
            else
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                SetFile(openFileDialog.FileName, Image.serie, true);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.viewer != null)
            {
                //viewer.image.Dispose();
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
            viewer.image.SaveSeries(saveFileDialog.FileName,0);
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

        private void toolboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tools.Show();
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            Process p = new Process();
            p.StartInfo.FileName = Application.ExecutablePath;
            p.StartInfo.Arguments = openFileDialog.FileName;
            p.Start();
        }

        
    }
}
