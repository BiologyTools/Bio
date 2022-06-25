using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;

namespace BioImage
{
    public partial class ImageViewer : Form
    {
        public static Tools tools;
        public static ROIManager manager = null;
        public static bool init = false;
        public static ImageViewer app = null;
        public Filter filters = null;
        public ImageView viewer = null;
        public StackTools stackTools = null;
        public static Graphics graphics = null;
        
        public ImageViewer(BioImage arg)
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            Init();
            tools = new Tools();
            stackTools = new StackTools();
            filters = new Filter();
            app = this;
            SetImage(arg);
            Table.AddViewer(this);
            
        }
        public ImageViewer(string arg)
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            Init();
            tools = new Tools();
            manager = new ROIManager();
            filters = new Filter();
            app = this;
            stackTools = new StackTools();
            if (arg.Length == 0)
                return;
            else
            {
                SetFile(arg, 0);
            }
            Table.AddViewer(this);
        }
        public static ImageViewer FromID(string id)
        {
            if(Table.images.ContainsKey(id))
            return new ImageViewer(Table.GetImage(id));
            else
            {
                MessageBox.Show("No image by " + id + " found for viewer.");
                return null;
            }
        }
        public static ImageViewer GetByID(string id)
        {
            if (Table.viewers.ContainsKey(id))
                return new ImageViewer(Table.GetImage(id));
            else
            {
                MessageBox.Show("No viewer by " + id + " found.");
                return null;
            }
        }
        public ImageViewer(string[] arg)
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            Init();
            tools = new Tools();
            manager = new ROIManager();
            app = this;
            stackTools = new StackTools();
            filters = new Filter();
            if (arg.Length == 0)
                return;
            else
            if (arg.Length == 1)
            {
                if (arg[0].EndsWith(".cs"))
                {
                    MainForm.runner.RunScriptFile(arg[0]);
                }
                else
                {
                    SetFile(arg[0], 0);
                }
            }
            Table.AddViewer(this);
        }

        private static void Init()
        {
            if (Recorder.recorder == null)
                Recorder.recorder = new Recorder();
            if(manager == null)
                manager = new ROIManager();
            init = true;
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
            viewer.UpdateView();
            System.Drawing.Size s = new System.Drawing.Size(viewer.image.SizeX + 20, viewer.image.SizeY + 165);
            if (s.Width > Screen.PrimaryScreen.Bounds.Width || s.Height > Screen.PrimaryScreen.Bounds.Height)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                Size = s;
            }
        }
        public void SetImage(BioImage b)
        {
            if (viewer == null)
            {
                viewer = new ImageView(b);
            }
            viewer.serie = b.series;
            viewer.filepath = b.ID;
            viewer.Dock = DockStyle.Fill;
            panel.Controls.Add(viewer);
            this.Text = b.Filename;

            viewer.UpdateView();
            System.Drawing.Size s = new System.Drawing.Size(viewer.image.SizeX + 20, viewer.image.SizeY + 165);
            if (s.Width > Screen.PrimaryScreen.Bounds.Width || s.Height > Screen.PrimaryScreen.Bounds.Height)
            {
                this.WindowState = FormWindowState.Maximized;
            }
            else
            {
                Size = s;
            }
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
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.viewer != null)
            {
                //viewer.image.Dispose();
                this.viewer.Dispose();
                this.viewer = null;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Image == null)
                return;
            if (saveTiffFileDialog.ShowDialog() != DialogResult.OK)
                return;
            //We save the tiff fast Libtiff otherwise we have to use BioFormats.
            //We export the ROI's to CSV to preserve ROI information without Bioformats.
            Image.Save(saveTiffFileDialog.FileName);
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
            saveOMEFileDialog.InitialDirectory = folderBrowserDialog.SelectedPath;
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
            {
                ImageView.viewer = this.viewer;
                Recorder.AddLine("BioImage.ImageView iv = Table.GetViewer(" + '"' + this.Text + '"' + ");");
            }
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
        private void scriptRunnerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MainForm.runner.WindowState = FormWindowState.Normal;
            MainForm.runner.Show();
        }
        private void ImageViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (this.Image == null)
                return;
            Recorder.AddLine("Table.RemoveImage(" + '"' + this.Text + '"' + ");");
            Table.RemoveViewer(this);
            this.Image.Dispose();
            Recorder.AddLine("Table.RemoveViewer(" + '"' + this.Text + '"' + ");");
        }

        private void stackToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stackTools.WindowState = FormWindowState.Normal;
            stackTools.Show();
        }
        private void scriptRecorderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Recorder.recorder.WindowState = FormWindowState.Normal;
            Recorder.recorder.Show();
        }

        private void saveOMEToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Image == null)
                return;
            if (saveOMEFileDialog.ShowDialog() != DialogResult.OK)
                return;
            Image.SaveOME(saveOMEFileDialog.FileName, Image.series);
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void setToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetTool tool = new SetTool();
            tool.Show();
        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }
        private void filtersToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
        }

        private void to8BitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To8Bit();
        }

        private void to16BitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To16Bit();
        }

        private void filtersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            filters = new Filter();
            filters.Show();
        }

        private void bit8ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To8Bit();
        }

        private void bit16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To16Bit();
        }

        private void to24BitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To24Bit();
        }

        private void to48BitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To48Bit();
        }

        private void to32BitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To32Bit();
        }
    }
}
