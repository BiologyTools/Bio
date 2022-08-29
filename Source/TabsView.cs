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
        public static bool init = false;
        public Filter filters = null;
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
                return App.tabsView.Image;
            }
        }
        public static ImageView SelectedViewer
        {
            get
            {
                return App.tabsView.ImageView;
            }
        }
        public TabsView(BioImage arg)
        {
            InitializeComponent();
            LoadProperties();
            ResizeView();
            UpdateTabs();
            Init();
        }
        public TabsView()
        {
            InitializeComponent();
            LoadProperties();
            ResizeView();
            UpdateTabs();
            Init();
        }
        public TabsView(string arg)
        {
            InitializeComponent();
            LoadProperties();
            if (arg.Length == 0)
                return;
            else
            {
                BioImage b = new BioImage(arg);
                AddTab(b);
            }
            ResizeView();
            UpdateTabs();
            Init();
        }
        public TabsView(string[] arg)
        {
            InitializeComponent();
            LoadProperties();
            Init();
            if (arg.Length == 0)
                return;
            else
            {
                if (arg[0].EndsWith(".cs"))
                {
                    App.runner.RunScriptFile(arg[0]);
                }
                else
                {
                    for (int i = 0; i < arg.Length; i++)
                    {
                        if(arg[i].EndsWith(".ijm"))
                        {
                            ImageJ.RunMacro(arg[i], "");
                        }
                        else
                            BioImage.Open(arg[i]);
                    }
                    
                }
            }
            UpdateTabs();
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
                    ResizeView();
                }
            }
        }
        public void AddTab(BioImage b)
        {
            TabPage t = new TabPage(Path.GetFileName(b.ID));
            ImageView v = new ImageView(b);
            v.Dock = DockStyle.Fill;
            t.Controls.Add(v);
            //TO DO Sizeing
            if(Width < b.SizeX || Height < b.SizeY)
            {
                Width = b.SizeX;
                Height = b.SizeY + 190;
            }
            tabControl.TabPages.Add(t);
        }
        private void Init()
        {
            filters = new Filter();
            init = true;
        }

        public void ResizeView()
        {
            if (Image == null)
                return;
            System.Drawing.Size s = new System.Drawing.Size(Viewer.image.SizeX + 20, Viewer.image.SizeY + 180);
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
            foreach (string item in openFilesDialog.FileNames)
            {
                BioImage.Open(item);
            }
            App.recent.AddRange(openFilesDialog.FileNames);
            foreach (string item in App.recent)
            {
                openRecentToolStripMenuItem.DropDownItems.Add(item, null, ItemClicked);
            }
            UpdateTabs();
        }

        private void SaveProperties()
        {
            int re = (int)Properties.Settings.Default["RecentFilesCount"];
            //string s = (string)Properties.Settings.Default["Recent"]; 
            string s = "";
            for (int i = 0; i < App.recent.Count; i++)
            {
                 s += "$"+App.recent[i];
            }
            Properties.Settings.Default["Recent"] = s;
            Properties.Settings.Default.Save();
        }

        private void LoadProperties()
        {
            App.recent.Clear();
            string s = (string)Properties.Settings.Default["Recent"];
            string[] sts = s.Split('$');
            foreach (string item in sts)
            {
                if(item!="")
                    App.recent.Add(item);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.Viewer != null)
            {
                Table.RemoveImage(SelectedImage);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Image == null)
                return;
            saveTiffFileDialog.FileName = Image.Filename;
            if (saveTiffFileDialog.ShowDialog() != DialogResult.OK)
                return;
            foreach (string file in saveTiffFileDialog.FileNames)
            {
                BioImage.Save(file,Image.ID);
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
            App.tools.Show();
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

        private void rOIManagerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.manager.Show();
        }

        private void ImageViewer_Activated(object sender, EventArgs e)
        {
            App.tabsView = this;
            //App.Image = SelectedImage;
            UpdateTabs();
        }

        private void channelsToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Viewer.image == null)
                return;
            if (App.channelsTool == null)
                App.channelsTool = new ChannelsTool(App.Channels);
            App.channelsTool.Show();
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
            App.runner.WindowState = FormWindowState.Normal;
            App.runner.Show();
        }

        private void stackToolsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.stackTools.Show();
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
                BioImage.SaveAsync(file,Image.ID);
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
            Viewer.UpdateImage();
        }

        private void bit16ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To16Bit();
            Viewer.UpdateImage();
        }

        private void to24BitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To24Bit();
            Viewer.UpdateImage();
        }

        private void to48BitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image.To48Bit();
            Viewer.UpdateImage();
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

            ImageView.SelectedImage = Image;
            //We update the active Viewer.
            App.viewer = Viewer;
        }

        private void closeToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (tabControl.SelectedIndex == -1)
                return;
            ImageView v = (ImageView)tabControl.SelectedTab.Controls[0];
            Table.RemoveImage(v.image);
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
                BioImage.Save(Image.ID, Image.ID);
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            About about = new About();
            about.Show();
        }

        private void scriptRunnerToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            App.runner.Show();
        }

        private void scriptRecorderToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            App.recorder.Show();
        }

        private void openOMEToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (openFilesDialog.ShowDialog() != DialogResult.OK)
                return;
            BioImage.OpenOME(openFilesDialog.FileName);
            UpdateTabs();
        }

        private void newTabViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Application.ExecutablePath);
        }

        private void nodeViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.nodeView.Show();
            App.nodeView.ShowInTaskbar = true;
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
        private void openRecentToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            openRecentToolStripMenuItem.DropDownItems.Clear();
            foreach (string item in App.recent)
            {
                openRecentToolStripMenuItem.DropDownItems.Add(item, null, ItemClicked);
            }
        }
        private void ItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem ts = (ToolStripMenuItem)sender;
            BioImage.Open(ts.Text);
        }
        private void TabsView_KeyDown(object sender, KeyEventArgs e)
        {
            
            int moveAmount = 5;
            if (e.KeyCode == Keys.C && e.Control)
            {
                Viewer.CopySelection();
                return;
            }
            if (e.KeyCode == Keys.V && e.Control)
            {
                Viewer.PasteSelection();
                return;
            }
            if (e.KeyCode == Keys.Subtract || e.KeyCode == Keys.NumPad7)
            {
                Viewer.scale.Width -= 0.1f;
                Viewer.scale.Height -= 0.1f;
                Viewer.UpdateOverlay();
            }
            if (e.KeyCode == Keys.Add || e.KeyCode == Keys.NumPad9)
            {
                Viewer.scale.Width += 0.1f;
                Viewer.scale.Height += 0.1f;
                Viewer.UpdateOverlay();
            }
            if (e.KeyCode == Keys.W || e.KeyCode == Keys.NumPad8)
            {
                Viewer.Origin = new System.Drawing.PointF(Viewer.Origin.X, Viewer.Origin.Y - moveAmount);
                return;
            }
            if (e.KeyCode == Keys.S || e.KeyCode == Keys.NumPad2)
            {
                Viewer.Origin = new System.Drawing.PointF(Viewer.Origin.X, Viewer.Origin.Y + moveAmount);
                return;
            }
            if (e.KeyCode == Keys.A || e.KeyCode == Keys.NumPad4)
            {
                Viewer.Origin = new System.Drawing.PointF(Viewer.Origin.X - moveAmount, Viewer.Origin.Y);
                return;
            }
            if (e.KeyCode == Keys.D || e.KeyCode == Keys.NumPad6)
            {
                Viewer.Origin = new System.Drawing.PointF(Viewer.Origin.X + moveAmount, Viewer.Origin.Y);
                return;
            }
        }

        private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BioImage im = Image.Copy();
            AddTab(im);
        }

        private void saveSeriesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.seriesTool.Show();
        }

        private void TabsView_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveProperties();
        }

        private void automationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            App.elements.Show();
        }
    }
}
