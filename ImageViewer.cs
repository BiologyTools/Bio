using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace BioImage
{
    public partial class ImageViewer : Form
    {
        public static Tools tools;
        public static ROIManager manager = null;
        public ImageView viewer = null;
        public ScriptRunner runner = null;

        public static ImageViewer app = null;
        public ImageViewer(BioImage arg)
        {
            InitializeComponent();
            tools = new Tools();
            manager = new ROIManager();
            runner = new ScriptRunner();
            app = this;
            SetImage(arg);
        }
        public ImageViewer(string arg)
        {
            InitializeComponent();
            tools = new Tools();
            manager = new ROIManager();
            runner = new ScriptRunner();
            app = this;
            if (arg.Length == 0)
                return;
            else
            {
                SetFile(arg, 0);
            }
        }
        public ImageViewer(string[] arg)
        {
            InitializeComponent();
            tools = new Tools();
            manager = new ROIManager();
            app = this;
            runner = new ScriptRunner();
            //scriptToolStripMenuItem.PerformClick();
            string file = "E://TESTIMAGES//text.ome.tif";
            SetFile(file, 0);
            //ushort[] GetBlock(int ix, int iy, int iw, int ih)
            ushort[,] val = Image.GetBlock(0, 0, 0, 0, 0, 0, 150, 150);
            Image.SetBlock(0, 0, 0, 0, 150, 150, 150, 150, val);
            viewer.UpdateView();
            //BioImage im = new BioImage(file, 0);
            //BioImage b = new BioImage(im, "subStack.ome.tif", 0, 0, 3, 0, 3, 0, 2);
            //im.Dispose();
            //SetImage(b);
            if (arg.Length == 0)
                return;
            else
            if (arg.Length == 1)
            {
                SetFile(arg[0], 0);
            }
        }

        public void SetFile(string file, int seri)
        {
            if (viewer == null)
            {
                viewer = new ImageView(file, seri);
            }
            viewer.serie = seri;
            viewer.filepath = file;
            viewer.Dock = DockStyle.Fill;
            panel.Controls.Add(viewer);
            string name = Path.GetFileName(file);
            this.Text = name;
        }
        public void SetImage(BioImage b)
        {
            if (viewer == null)
            {
                viewer = new ImageView(b);
            }
            viewer.serie = b.serie;
            viewer.filepath = b.filename;
            viewer.Dock = DockStyle.Fill;
            panel.Controls.Add(viewer);
            this.Text = b.idString;
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
            set
            {
                viewer.image = value;
            }
        }

        public void Open(string[] files)
        {
            foreach (string file in files)
            {
                if(viewer != null)
                {
                    string[] sts = new string[1];
                    sts[0] = file;
                    ImageViewer viewer = new ImageViewer(sts);
                    viewer.Show();
                }
                else
                    SetFile(openFilesDialog.FileName,0);
            }
        }

        public void OpenInNewProcess(string file)
        {
            Process p = new Process();
            p.StartInfo.FileName = Application.ExecutablePath;
            p.StartInfo.Arguments = file;
            p.Start();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFilesDialog.ShowDialog() != DialogResult.OK)
                return;
            Open(openFilesDialog.FileNames);
            viewer.Size = new System.Drawing.Size(viewer.image.SizeX + 100, viewer.image.SizeY + 100);
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
            else
            if (e.KeyCode == Keys.O && e.Control)
            {
                openToolStripMenuItem.PerformClick();
            }
           
        }

        private void toolboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tools.Show();
        }

        private void exportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveCSVFileDialog.ShowDialog() != DialogResult.OK)
                return;
            BioImage.ExportROIsCSV(saveCSVFileDialog.FileName, viewer.image.Annotations);
        }

        private void importCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openCSVFileDialog.ShowDialog() != DialogResult.OK)
                return;
            viewer.image.Annotations.AddRange(BioImage.ImportROIsCSV(openCSVFileDialog.FileName));
        }

        private void exportROIsOfFolderOfImagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            saveFileDialog.InitialDirectory = folderBrowserDialog.SelectedPath;
            if (saveCSVFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string f = Path.GetFileName(saveCSVFileDialog.FileName);

            BioImage.ExportROIFolder(folderBrowserDialog.SelectedPath, f);
        }

        private void ImageViewer_Click(object sender, EventArgs e)
        {
            if (Image != null)
                ImageView.selectedImage = viewer.image;
        }

        private void panel_Click(object sender, EventArgs e)
        {
            if(Image != null)
            ImageView.selectedImage = viewer.image;
        }

        private void rOIManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.Show();
            
        }

        private void ImageViewer_Deactivate(object sender, EventArgs e)
        {
        }

        private void ImageViewer_Activated(object sender, EventArgs e)
        {
            app = this;
            ImageView.app = this;
            if (this.viewer != null)
                ImageView.viewer = this.viewer;
        }

        private void channelsToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer.image == null)
                return;
            ChannelsTool ch = new ChannelsTool(viewer.image.Channels);
            ch.Show();
        }

        private void rGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer == null)
                return;
            viewer.Mode = ImageView.ViewMode.RGBImage;
            filteredToolStripMenuItem.Checked = false;
            rawToolStripMenuItem.Checked = false;
            viewer.UpdateView();
        }

        private void filteredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer == null)
                return;
            viewer.Mode = ImageView.ViewMode.Filtered;
            rGBToolStripMenuItem.Checked = false;
            rawToolStripMenuItem.Checked = false;
            viewer.UpdateView();
        }

        private void rawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer == null)
                return;
            viewer.Mode = ImageView.ViewMode.Raw;
            rGBToolStripMenuItem.Checked = false;
            filteredToolStripMenuItem.Checked = false;
            viewer.UpdateView();
        }

        private void autoThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (viewer == null)
                return;
            viewer.image.AutoThreshold();
        }

        public void UpdateViewMode(ImageView.ViewMode v)
        {
            if (v == ImageView.ViewMode.RGBImage)
                rGBToolStripMenuItem.Checked = true;
            if (v == ImageView.ViewMode.Filtered)
                filteredToolStripMenuItem.Checked = true;
            if(v == ImageView.ViewMode.Raw)
                rawToolStripMenuItem.Checked = true;
        }

        private void panel_Paint(object sender, PaintEventArgs e)
        {

        }
        private void scriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ScriptEditor sc = new ScriptEditor();
            sc.Show();
        }
        private void scriptRunnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            runner.Show();
        }
        private void ImageViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }
    }
}
