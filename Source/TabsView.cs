using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Threading;

namespace Bio
{
    public partial class TabsView : Form
    {
        public static Tools tools;
        public static ROIManager manager = null;
        public static bool init = false;
        public static TabsView tabview = null;
        public static ImageView viewer = null;
        public static NodeView nodeView = null;
        public Filter filters = null;
        public StackTools stackTools = null;
        public static Graphics graphics = null;
        public ImageView Viewer
        {
            get
            {
                if (tabControl.TabPages.Count == 0)
                    return null;
                if (tabControl.SelectedIndex == -1)
                    return null;
                if (tabControl.SelectedTab.Controls.Count == 0)
                    return null;
                return (ImageView)tabControl.SelectedTab.Controls[0];
            }
        }
        public static BioImage SelectedImage
        {
            get
            {
                return NodeView.viewer.Image;
            }
        }
        
        public TabsView(BioImage arg)
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            tools = new Tools();
            stackTools = new StackTools();
            filters = new Filter();
            tabview = this;
            SetImage(arg);
            UpdateTabs();
            Init();
        }
        public TabsView()
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            tools = new Tools();
            stackTools = new StackTools();
            filters = new Filter();
            tabview = this;
            UpdateTabs();
            Init();
        }
        public TabsView(string arg)
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            tools = new Tools();
            manager = new ROIManager();
            filters = new Filter();
            tabview = this;
            stackTools = new StackTools();
            if (arg.Length == 0)
                return;
            else
            {
                BioImage b = new BioImage(arg);
                AddTab(b);
            }
            UpdateTabs();
            Init();
        }
        public TabsView(string[] arg)
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
            Init();
            tools = new Tools();
            manager = new ROIManager();
            tabview = this;
            stackTools = new StackTools();
            filters = new Filter();
            if (arg.Length == 0)
                return;
            else
            {
                if (arg[0].EndsWith(".cs"))
                {
                    NodeView.runner.RunScriptFile(arg[0]);
                }
                else
                {
                    for (int i = 0; i < arg.Length; i++)
                    {
                        if(arg[i].EndsWith("ome.tif"))
                        { 
                            BioImage b = BioImage.OpenOME(arg[i]);
                            AddTab(b);
                        }
                        else
                        if(arg[i].EndsWith(".tif") || arg[i].EndsWith(".tiff"))
                        {
                            BioImage b = BioImage.Open(arg[i]);
                            AddTab(b);
                        }
                        else
                        {
                            BioImage b = BioImage.OpenOME(arg[i]);
                            AddTab(b);
                        }
                    }
                    
                }
            }
            UpdateTabs();
        }
        public static TabsView GetByID(string id)
        {
            if (Table.viewers.ContainsKey(id))
                return new TabsView(Table.GetImage(id));
            else
            {
                MessageBox.Show("No viewer by " + id + " found.");
                return null;
            }
        }
  
        public void UpdateTabs()
        {
            if (Table.images.Count != tabControl.TabPages.Count)
            {
                tabControl.TabPages.Clear();
                for (int i = 0; i < Table.images.Count; i++)
                {
                    BioImage b = Table.images[i];
                    TabPage t = new TabPage(Path.GetFileName(b.ID));
                    t.Controls.Add(new ImageView(b));
                    tabControl.TabPages.Add(t);
                }
            }
        }
        public void AddTab(BioImage b)
        {
            TabPage t = new TabPage(Path.GetFileName(b.ID));
            ImageView v = new ImageView(b);
            v.Dock = DockStyle.Fill;
            t.Controls.Add(v);
            Table.AddViewer(v);
            if(Width < b.SizeX || Height < b.SizeY)
            {
                Width = b.SizeX;
                Height = b.SizeY + 190;
            }
            tabControl.TabPages.Add(t);
        }
        public void AddTab(ImageView v)
        {
            TabPage t = new TabPage(Path.GetFileName(v.image.ID));
            v.Dock = DockStyle.Fill;
            t.Controls.Add(v);
            tabControl.TabPages.Add(t);
            Table.AddViewer(v);
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
            ImageView v = new ImageView(file, seri);
            v.serie = seri;
            v.filepath = file;
            v.Dock = DockStyle.Fill;
            AddTab(v);
            string name = Path.GetFileName(file);
            v.UpdateStatus();
            System.Drawing.Size s = new System.Drawing.Size(Viewer.image.SizeX + 20, Viewer.image.SizeY + 165);
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
            //AddTab(b);
            Viewer.serie = b.series;
            Viewer.filepath = b.ID;
            Viewer.Dock = DockStyle.Fill;
            Viewer.image = b;
            this.Text = b.Filename;
            Viewer.UpdateStatus();
            System.Drawing.Size s = new System.Drawing.Size(Viewer.image.SizeX + 20, Viewer.image.SizeY + 165);
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
                if (Viewer == null)
                    return null;
                if (Viewer.image == null)
                    return null;
                return Viewer.image; 
            }
            set
            {
                Viewer.image = value;
            }
        }

        public ImageView ImageView
        {
            get
            {
                return Viewer;
            }
        }

        public int TabCount
        {
            get
            {
                return tabControl.TabPages.Count;
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
            BioImage.OpenThread(openFilesDialog.FileNames);
            UpdateTabs();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Viewer != null)
            {
                Table.RemoveViewer(this.Viewer);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Image == null)
                return;
            if (saveTiffFileDialog.ShowDialog() != DialogResult.OK)
                return;
            foreach (string file in saveTiffFileDialog.FileNames)
            {
                BioImage.SaveThread(file,Image.ID);
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
            BioImage.ExportROIsCSV(saveCSVFileDialog.FileName, Viewer.image.Annotations);
        }

        private void importCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openCSVFileDialog.ShowDialog() != DialogResult.OK)
                return;
            Viewer.image.Annotations.AddRange(BioImage.ImportROIsCSV(openCSVFileDialog.FileName));
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
                ImageView.selectedImage = Viewer.image;
        }

        private void panel_Click(object sender, EventArgs e)
        {
            if(Image != null)
            ImageView.selectedImage = Viewer.image;
        }

        private void rOIManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            manager.Show();
            
        }

        private void ImageViewer_Activated(object sender, EventArgs e)
        {
            NodeView.viewer = this;
            tabview = this;
            ImageView.app = this;
            if (this.Viewer == null)
                this.Height = 400;
            ImageView.viewer = this.Viewer;
            UpdateTabs();
        }

        private void channelsToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Viewer.image == null)
                return;
            ChannelsTool ch = new ChannelsTool();
            ch.Show();
        }

        private void rGBToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Viewer == null)
                return;
            Viewer.Mode = ImageView.ViewMode.RGBImage;
            filteredToolStripMenuItem.Checked = false;
            rawToolStripMenuItem.Checked = false;
            Viewer.UpdateStatus();
        }

        private void filteredToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Viewer == null)
                return;
            Viewer.Mode = ImageView.ViewMode.Filtered;
            rGBToolStripMenuItem.Checked = false;
            rawToolStripMenuItem.Checked = false;
            Viewer.UpdateStatus();
        }

        private void rawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Viewer == null)
                return;
            Viewer.Mode = ImageView.ViewMode.Raw;
            rGBToolStripMenuItem.Checked = false;
            filteredToolStripMenuItem.Checked = false;
            Viewer.UpdateStatus();
        }

        private void autoThresholdToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Viewer == null)
                return;
            BioImage.AutoThresholdThread(Viewer.image);
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
            NodeView.runner.WindowState = FormWindowState.Normal;
            NodeView.runner.Show();
        }

        private void ImageViewer_FormClosing(object sender, FormClosingEventArgs e)
        {
            //We close the mainform nodeview to exit application.
            nodeView.Exit();
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
            foreach (string file in saveOMEFileDialog.FileNames)
            {
                BioImage.SaveOMEThread(file,Image.ID);
            }
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
            //Here we update the tabs based on images in the table.

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

        private void toWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1)
                return;
            BioImage im = ((ImageView)tabControl.SelectedTab.Controls[0]).image;
            ImageWindow vi = new ImageWindow(im);
            vi.Show();
        }

        private void tabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1)
                return;
            //We update the view status based on tab.
            if (Viewer.Mode == ImageView.ViewMode.Raw)
                rawToolStripMenuItem.Checked = true;
            else
                rawToolStripMenuItem.Checked = false;
            if (Viewer.Mode == ImageView.ViewMode.Filtered)
                filteredToolStripMenuItem.Checked = true;
            else
                filteredToolStripMenuItem.Checked = false;
            if (Viewer.Mode == ImageView.ViewMode.RGBImage)
                rGBToolStripMenuItem.Checked = true;
            else
                rGBToolStripMenuItem.Checked = false;
            //We update the active viewer.
            ImageView.viewer = Viewer;
            TabsView.viewer = Viewer;
        }

        private void closeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1)
                return;
            ImageView v = (ImageView)tabControl.SelectedTab.Controls[0];
            Table.RemoveImage(v.image);
            Table.RemoveViewer(v);
            tabControl.TabPages.RemoveAt(tabControl.SelectedIndex);
            v.Dispose();

        }

        private void openOMEToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void saveAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> sts = new List<string>();
            foreach (BioImage item in Table.images)
            {
                BioImage.SaveThread(Image.ID, Image.ID);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void scriptRunnerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Scripting runner = new Scripting();
            runner.Show();
        }

        private void scriptRecorderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Recorder rec = new Recorder();
            rec.Show();
        }

        private void openOMEToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (openFilesDialog.ShowDialog() != DialogResult.OK)
                return;
            BioImage.OpenOMEThread(openFilesDialog.FileNames);
            UpdateTabs();
        }

        private void newTabViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.ExecutablePath);
        }

        private void nodeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NodeView.nodeView.Show();
            NodeView.nodeView.ShowInTaskbar = true;
        }

        private void tabControl_Click(object sender, EventArgs e)
        {
            ImageView.viewer = Viewer;
            TabsView.viewer = Viewer;
        }

        private void saveToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (saveTiffFileDialog.ShowDialog() == DialogResult.OK)
                Bio.BioImage.Save(Viewer.image.ID, saveTiffFileDialog.FileName);
        }

        private void saveOMEToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (saveOMEFileDialog.ShowDialog() == DialogResult.OK)
                Bio.BioImage.Save(Viewer.image.ID, saveOMEFileDialog.FileName);
        }
    }
}
