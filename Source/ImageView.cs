using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Bio
{
    public partial class ImageView : UserControl
    {
        public ImageView(string file, int ser)
        {
            file = file.Replace("\\", "/");
            InitializeComponent();
            serie = ser;
            Dock = DockStyle.Fill;
            tools = new Tools();
            if (file == "" || file == null)
                return;
            SetCoordinate(0, 0, 0);

            //image = new BioImage(file, ser);
            InitGUI(file);
            
            MouseWheel += new System.Windows.Forms.MouseEventHandler(ImageView_MouseWheel);
            zBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(ZTrackBar_MouseWheel);
            cBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(CTrackBar_MouseWheel);
            timeBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(TimeTrackBar_MouseWheel);
            //We set the trackbar event to handled so that it only scrolls one tick not the default multiple.
            zBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            timeBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            cBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            TimeFps = 60;
            ZFps = 60;
            CFps = 1;
            UpdateView();
            viewer = this;
            // Change parent for overlay PictureBox.
            overlayPictureBox.Parent = pictureBox;
            overlayPictureBox.Location = new Point(0, 0);
        }
        public ImageView(BioImage im)
        {
            string file = im.ID.Replace("\\", "/");
            InitializeComponent();
            serie = im.series;
            image = im;
            tools = new Tools();
            Dock = DockStyle.Fill;
            if (file == "" || file == null)
                return;
            SetCoordinate(0, 0, 0);
            InitGUI(file);
            //Buf = image.GetBufByCoord(GetCoordinate());

            MouseWheel += new System.Windows.Forms.MouseEventHandler(ImageView_MouseWheel);
            zBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(ZTrackBar_MouseWheel);
            cBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(CTrackBar_MouseWheel);
            timeBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(TimeTrackBar_MouseWheel);
            //We set the trackbar event to handled so that it only scrolls one tick not the default multiple.
            zBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            timeBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            cBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            TimeFps = 60;
            ZFps = 60;
            CFps = 1;
            UpdateView();
            viewer = this;
            // Change parent for overlay PictureBox.
            overlayPictureBox.Parent = pictureBox;
            overlayPictureBox.Location = new Point(0, 0);
        }
        ~ImageView()
        {

        }

        public static bool showBounds = true;
        public static bool showText = true;
        public Image Buf = null;
        public BioImage image;
        public string filepath = "";
        public int serie = 0;
        public static ZCT coordinate = new ZCT(0, 0, 0);
        public void SetCoordinate(int z, int c, int t)
        {
            ImageView.coordinate = new ZCT(z, c, t);
            zBar.Value = z;
            cBar.Value = c;
            timeBar.Value = t;
            //UpdateImage();
        }
        public ZCT GetCoordinate()
        {
            return coordinate;
        }
        public bool ShowControls
        {
            get { return trackBarPanel.Visible; }
            set 
            {
                if (!value)
                {
                    statusPanel.Hide();
                    trackBarPanel.Hide();
                    panel.Dock = DockStyle.Fill;
                    //overlayPictureBox.Anchor = AnchorStyles.None;
                    //pictureBox.Anchor = AnchorStyles.None;
                    //trackBarPanel.Hide();
                    //statusPanel.Hide();
                    //statusPanel.SendToBack();
                    overlayPictureBox.Location = new Point(0, 0);
                    overlayPictureBox.Height = image.SizeY;
                    pictureBox.Location = new Point(0, 0);
                    pictureBox.Height = image.SizeY;
                    showControlToolStripMenuItem.Text = "Show Controls";
                    //pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                    
                }
                else
                {
                    statusPanel.Show();
                    trackBarPanel.Show();
                    panel.Top = 25;
                    panel.Height -= 75;
                    panel.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;

                    showControlToolStripMenuItem.Text = "Hide Controls";
                    /*
                    overlayPictureBox.Anchor = AnchorStyles.None;
                    pictureBox.Anchor = AnchorStyles.None;
                    overlayPictureBox.Location = new Point(0, 25);
                    pictureBox.Location = new Point(0, 25);
                    overlayPictureBox.Height = image.SizeY;
                    pictureBox.Height = image.SizeY;
                    
                    pictureBox.Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom | AnchorStyles.Left;
                    trackBarPanel.Show();
                    trackBarPanel.BringToFront();
                    statusPanel.Show();
                    statusPanel.BringToFront();
                    showControlToolStripMenuItem.Visible = false;
                    */
                }
            }
        }
        
        public bool ShowStatus
        {
            get { return statusPanel.Visible; }
            set 
            {
                if (!value)
                {
                    statusPanel.Hide();
                    statusPanel.Visible = value;
                    statusPanel.Height = 0;
                    pictureBox.Location = new Point(0, 0);
                    showStatusToolStripMenuItem.Visible = true;
                }
                else
                {
                    statusPanel.Show();
                    statusPanel.Visible = value;
                    pictureBox.Top = 0;
                    showStatusToolStripMenuItem.Visible = false;
                    statusPanel.Height = 25;
                }
            }
        }

        public int Index
        {
            get
            {
                return image.Coords[coordinate.Z, coordinate.C, coordinate.T];
            }
        }

        public int minSizeX = 50;
        public int minSizeY = 20;
        public bool loopZ = true;
        public bool loopT = true;
        public bool loopC = true;
        public enum ViewMode
        {
            Raw,
            Filtered,
            RGBImage
        }
        private ViewMode viewMode = ViewMode.Filtered;
        public ViewMode Mode
        {
            get
            {
                return viewMode;
            }
            set
            {
                viewMode = value;
                //If view mode is changed we update.
                TabsView.tabview.UpdateViewMode(viewMode);
                UpdateView();
                UpdateOverlay();
                UpdateImage();
                if (viewMode == ViewMode.RGBImage)
                {
                    cBar.Value = 0;
                    rgbBoxsPanel.BringToFront();
                    cBar.SendToBack();
                    cLabel.SendToBack();
                }
                else
                if (viewMode == ViewMode.Filtered)
                {
                    rgbBoxsPanel.SendToBack();
                    cBar.BringToFront();
                    cLabel.BringToFront();
                }
                else
                if (viewMode == ViewMode.Raw)
                {
                    rgbBoxsPanel.SendToBack();
                    cBar.BringToFront();
                    cLabel.BringToFront();
                }
            }
        }
        public string Path
        {
            get
            {
                return filepath;
            }
        }
        //public Buf Buf;
        public Channel RChannel
        {
            get
            {
                return image.Channels[image.rgbChannels[0]];
            }
        }
        public Channel GChannel
        {
            get
            {
                return image.Channels[image.rgbChannels[1]];
            }
        }
        public Channel BChannel
        {
            get
            {
                return image.Channels[image.rgbChannels[2]];
            }
        }

        public Tools tools;

        public void UpdateRGBChannels()
        {
            //Buf = image.GetBufByCoord(GetCoordinate());
            if (channelBoxR.SelectedIndex == -1)
                image.rgbChannels[0] = 0;
            else
                image.rgbChannels[1] = channelBoxR.SelectedIndex;
            if (channelBoxG.SelectedIndex == -1)
                image.rgbChannels[1] = 0;
            else
                image.rgbChannels[1] = channelBoxG.SelectedIndex;
            if (channelBoxB.SelectedIndex == -1)
                image.rgbChannels[2] = 0;
            else
                image.rgbChannels[2] = channelBoxB.SelectedIndex;
            
        }

        private bool timeEnabled = false;

        private int zfps;
        public int ZFps
        {
            get
            {
                return zfps;
            }
            set
            {
                zfps = value;
                float f = value;
                zTimer.Interval = (int)Math.Floor(1000 / f);
            }
        }

        private int timefps;
        public int TimeFps
        {
            get
            {
                return timefps;
            }
            set
            {
                timefps = value;
                float f = value;
                timelineTimer.Interval = (int)Math.Floor(1000 / f);
            }
        }

        private int cfps;
        public int CFps
        {
            get
            {
                return cfps;
            }
            set
            {
                cfps = value;
                float f = value;
                cTimer.Interval = (int)Math.Floor(1000 / f);
            }
        }

        public void InitGUI(string path)
        {
            filepath = path;
            //image = new BioImage(filePath);
            zBar.Maximum = image.SizeZ - 1;
            cBar.Maximum = image.SizeC - 1;
            if (image.SizeT > 1)
            {
                timeBar.Maximum = image.SizeT - 1;
                timeEnabled = true;
            }
            else
            {
                timeBar.Enabled = false;
                timeEnabled = false;
                timeBar.Maximum = image.SizeT - 1;
            }
            //rgbPictureBox.Image = image.plane.GetBitmap();
            //we clear the channel comboboxes incase we have channels from previous loaded image.
            channelBoxR.Items.Clear();
            channelBoxG.Items.Clear();
            channelBoxB.Items.Clear();

            foreach (Channel ch in image.Channels)
            {
                channelBoxR.Items.Add(ch);
                channelBoxG.Items.Add(ch);
                channelBoxB.Items.Add(ch);
            }
            if (image.Channels.Count > 2)
            {
                channelBoxR.SelectedIndex = 0;
                channelBoxG.SelectedIndex = 1;
                channelBoxB.SelectedIndex = 2;
            }
            else
            if (image.Channels.Count == 2)
            {
                channelBoxR.SelectedIndex = 0;
                channelBoxG.SelectedIndex = 1;
            }
            else
            {
                channelBoxR.SelectedIndex = 0;
            }
                UpdateRGBChannels();
            //We threshold the image so that the max threshold value is the max pixel value in image. 
        }

        public void UpdateSelectBoxSize(float size)
        {
            foreach (Annotation item in image.Annotations)
            {
                item.selectBoxSize = size;
            }
        }

        public void UpdateOverlay()
        {
            if(image.Buffers[0].PixelFormat == PixelFormat.Format24bppRgb)
            UpdateView();
            else
            overlayPictureBox.Invalidate();
        }

        public void UpdateStatus()
        {
            if (Mode == ViewMode.RGBImage)
            {
                //Since combining 3 planes to one image takes time we show the image load time.
                float ms = image.loadTimeMS;
                ticksLabel.Text = " Ticks: " + image.loadTimeTicks;

                if (timeEnabled)
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (timeBar.Value + 1) + "/" + (timeBar.Maximum + 1) + ", " + mousePoint + mouseColor + ", " + image.Buffers[0].PixelFormat.ToString();
                }
                else
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + mousePoint + mouseColor;
                }
                
            }
            else
            {
                if (timeEnabled)
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (cBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + (timeBar.Value + 1) + "/" + (timeBar.Maximum + 1) + ", " + mousePoint + mouseColor + ", " + image.Buffers[0].PixelFormat.ToString();
                }
                else
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (cBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + mousePoint + mouseColor + ", " + image.Buffers[0].PixelFormat.ToString();
                }
            }
        }

        public void UpdateView()
        {
            UpdateImage();
            UpdateStatus();
        }

        public void UpdateImage()
        {
            ZCT coords = new ZCT(zBar.Value, cBar.Value, timeBar.Value);
            if (image == null)
                return;
            int index = image.Coords[zBar.Value, cBar.Value, timeBar.Value];
            if (Mode == ViewMode.Filtered)
            {
                bm = image.GetFiltered(coords, RChannel.range, GChannel.range, BChannel.range);
            }
            else if (Mode == ViewMode.RGBImage)
            {
                PixelFormat px = image.Buffers[index].PixelFormat;
                if (px == PixelFormat.Format8bppIndexed || px == PixelFormat.Format16bppGrayScale)
                {
                    bm = image.GetRGBBitmap(coords, RChannel.range, BChannel.range, GChannel.range);
                }
                else
                {
                    bm = (Bitmap)image.Buffers[index].Image;
                }
            }
            else
            {
                bm = (Bitmap)image.Buffers[index].Image;
            }
            if (bm.PixelFormat == PixelFormat.Format16bppGrayScale || bm.PixelFormat == PixelFormat.Format48bppRgb)
                bm = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)bm);

            pictureBox.Invalidate();
        }

        private void channelBoxR_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxR.SelectedIndex == -1)
                return;
            image.rgbChannels[0] = channelBoxR.SelectedIndex;
            UpdateView();
            UpdateImage();
        }

        private void channelBoxG_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxG.SelectedIndex == -1)
                return;
            image.rgbChannels[1] = channelBoxG.SelectedIndex;
            UpdateView();
            UpdateImage();
        }

        private void channelBoxB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxB.SelectedIndex == -1)
                return;
            image.rgbChannels[2] = channelBoxB.SelectedIndex;
            UpdateView();
            UpdateImage();
        }

        private void showControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (trackBarPanel.Visible)
            {
                trackBarPanel.Hide();
                trackBarPanel.Height = 0;
                Application.DoEvents();
                pictureBox.Height += 75;
                overlayPictureBox.Height += 75;
                showControlsToolStripMenuItem.Text = "Show Controls";
                pictureBox.Dock = DockStyle.Fill;
            }
            else
            {
                trackBarPanel.Show();
                pictureBox.Height -= 75;
                Application.DoEvents();
                trackBarPanel.Height = 75;
                overlayPictureBox.Height += trackBarPanel.Height;
                showControlsToolStripMenuItem.Text = "Hide Controls";
                pictureBox.Dock = DockStyle.Fill;
            }

        }

        private void playZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (playZToolStripMenuItem.Checked)
            {
                //We stop
                playZToolStripMenuItem.Checked = false;
                stopZToolStripMenuItem.Checked = true;
                zTimer.Stop();
            }
            else
            {
                //We start
                playZToolStripMenuItem.Checked = true;
                stopZToolStripMenuItem.Checked = false;
                zTimer.Start();
            }
        }

        private void stopZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stopZToolStripMenuItem.Checked)
            {
                //We start
                playZToolStripMenuItem.Checked = true;
                stopZToolStripMenuItem.Checked = false;
                zTimer.Start();

            }
            else
            {
                //We stop
                playZToolStripMenuItem.Checked = false;
                stopZToolStripMenuItem.Checked = true;
                zTimer.Stop();
            }
        }

        private void playTimeToolStripMenu_Click(object sender, EventArgs e)
        {
            if (playTimeToolStripMenu.Checked)
            {
                //We stop
                playTimeToolStripMenu.Checked = false;
                stopTimeToolStripMenu.Checked = true;
                timelineTimer.Stop();
            }
            else
            {
                //We start
                playTimeToolStripMenu.Checked = true;
                stopTimeToolStripMenu.Checked = false;
                timelineTimer.Start();
            }
        }

        private void stopTimeToolStripMenu_Click(object sender, EventArgs e)
        {
            playTimeToolStripMenu.Checked = false;
            stopTimeToolStripMenu.Checked = true;
            timelineTimer.Stop();
        }

        private void playCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (playCToolStripMenuItem.Checked)
            {
                //We stop
                playCToolStripMenuItem.Checked = false;
                stopCToolStripMenuItem.Checked = true;
                cTimer.Stop();
            }
            else
            {
                //We start
                playCToolStripMenuItem.Checked = true;
                stopCToolStripMenuItem.Checked = false;
                cTimer.Start();
            }
        }

        private void stopCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopCToolStripMenuItem.Checked = false;
            playCToolStripMenuItem.Checked = true;
            cTimer.Stop();
        }

        private string mousePoint = "";
        private string mouseColor = "";
        private void playSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if (Mode == ViewMode.RGBImage)
                sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            else
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            zTimer.Interval = sp.TimePlayspeed;
            cTimer.Interval = sp.CPlayspeed;
            timelineTimer.Interval = sp.TimePlayspeed;
        }
        private void playSpeedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if (Mode == ViewMode.RGBImage)
                sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            else
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            zTimer.Interval = sp.TimePlayspeed;
            cTimer.Interval = sp.CPlayspeed;
            timelineTimer.Interval = sp.TimePlayspeed;
        }
        private void CPlaySpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if (Mode == ViewMode.RGBImage)
                sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            else
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            zTimer.Interval = sp.TimePlayspeed;
            cTimer.Interval = sp.CPlayspeed;
            timelineTimer.Interval = sp.TimePlayspeed;
        }
        private void ImageView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete)
            {
                deleteROIToolStripMenuItem.PerformClick();
            }
            
        }

        public void GetRange()
        {
            RangeTool t;
            if (Mode == ViewMode.Filtered)
                t = new RangeTool(timeEnabled, true, zBar.Minimum, zBar.Maximum, timeBar.Minimum, timeBar.Maximum, cBar.Minimum, cBar.Maximum);
            else
                t = new RangeTool(timeEnabled, false, zBar.Minimum, zBar.Maximum, timeBar.Minimum, timeBar.Maximum, cBar.Minimum, cBar.Maximum);
            if (t.ShowDialog() != DialogResult.OK)
                return;
            zBar.Minimum = t.ZMin;
            zBar.Maximum = t.ZMax;
            timeBar.Minimum = t.TimeMin;
            timeBar.Maximum = t.TimeMax;
            cBar.Minimum = t.CMin;
            cBar.Maximum = t.CMax;
        }

        private void setValueRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRange();
        }

        private void setValueRangeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            GetRange();
        }

        private void setCValueRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetRange();
        }

        public SizeF scale = new SizeF(1, 1);
        private void ImageView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Ctrl)
            {
                if (e.Delta > 0)
                {
                    scale.Width += 0.05f;
                    scale.Height += 0.05f;
                }
                else
                {
                    scale.Width -= 0.05f;
                    scale.Height -= 0.05f;
                }
                pictureBox.Invalidate();
                overlayPictureBox.Invalidate();
            }
            else
            if (e.Delta > 0)
            {
                if (zBar.Value + 1 <= zBar.Maximum)
                    zBar.Value += 1;
            }
            else
            {
                if (zBar.Value - 1 >= zBar.Minimum)
                    zBar.Value -= 1;
            }
        }
        private void ZTrackBar_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                if (zBar.Value + 1 <= zBar.Maximum)
                    zBar.Value += 1;
            }
            else
            {
                if (zBar.Value - 1 >= zBar.Minimum)
                    zBar.Value -= 1;
            }

        }
        private void TimeTrackBar_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (!timeEnabled)
                return;
            if (e.Delta > 0)
            {
                if (timeBar.Value + 1 <= timeBar.Maximum)
                    timeBar.Value += 1;
            }
            else
            {
                if (timeBar.Value - 1 >= timeBar.Minimum)
                    timeBar.Value -= 1;
            }
        }
        private void CTrackBar_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {

        }

        private void cTimer_Tick(object sender, EventArgs e)
        {
            if (playCToolStripMenuItem.Checked)
            {
                if (cBar.Maximum >= cBar.Value + 1)
                    cBar.Value++;
                else
                {
                    if (loopC)
                        cBar.Value = cBar.Minimum;
                }
            }
        }
        private void zTimer_Tick(object sender, EventArgs e)
        {
            if (playZToolStripMenuItem.Checked)
            {
                if (zBar.Maximum >= zBar.Value + 1)
                    zBar.Value++;
                else
                {
                    if (loopZ)
                        zBar.Value = zBar.Minimum;
                }
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {
            if (playTimeToolStripMenu.Checked)
            {
                if (timeBar.Maximum >= timeBar.Value + 1)
                    timeBar.Value++;
                else
                {
                    if (loopT)
                        timeBar.Value = timeBar.Minimum;
                }
            }
        }
        private void zBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateView();
            UpdateImage();
        }
        private void timeBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateView();
            UpdateImage();
        }
        private void cBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateView();
            UpdateImage();
        }

        private void loopTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loopT = loopTimeToolStripMenuItem.Checked;
        }
        private void loopZToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loopZ = loopZToolStripMenuItem.Checked;
        }
        private void loopCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            loopC = loopCToolStripMenuItem.Checked;
        }
        private Point GetImagePoint()
        {
            return new Point(0, 0);
        }
        private Point GetImageSize()
        {
            if(image==null)
                return new Point(pictureBox.Width, pictureBox.Height);
            return new Point(image.SizeX, image.SizeY);
        }

        public bool showRROIs = true;
        public bool showGROIs = true;
        public bool showBROIs = true;

        private List<Annotation> annotationsR = new List<Annotation>();
        public List<Annotation> AnnotationsR
        {
            get
            {
                return image.GetAnnotations(coordinate.Z, image.RChannel.Index, coordinate.T);
            }
        }
        private List<Annotation> annotationsG = new List<Annotation>();
        public List<Annotation> AnnotationsG
        {
            get
            {
                return image.GetAnnotations(coordinate.Z, image.GChannel.Index, coordinate.T);
            }
        }
        private List<Annotation> annotationsB = new List<Annotation>();
        public List<Annotation> AnnotationsB
        {
            get
            {
                return image.GetAnnotations(coordinate.Z, image.BChannel.Index, coordinate.T);
            }
        }

        public List<Annotation> AnnotationsRGB
        {
            get
            {
                List<Annotation> ans = new List<Annotation>();
                if (Mode == ViewMode.RGBImage)
                {
                    if (showRROIs)
                        ans.AddRange(AnnotationsR);
                    if (showGROIs)
                        ans.AddRange(AnnotationsG);
                    if (showBROIs)
                        ans.AddRange(AnnotationsB);
                }
                else
                    ans.AddRange(image.GetAnnotations(coordinate));
                return ans;
            }
        }
        private void DrawOverlay(Graphics g)
        {
            Pen pen = null;
            Brush b = null;
            bool bounds = showBounds;
            bool labels = showText;
            if (Mode == ViewMode.RGBImage)
            {
                foreach (Annotation an in AnnotationsRGB)
                {
                    if (an.coord != GetCoordinate())
                        return;
                    pen = new Pen(an.strokeColor, (float)an.strokeWidth);
                    if (an.selected)
                    {
                        b = new SolidBrush(Color.Magenta);
                    }
                    else
                        b = new SolidBrush(an.strokeColor);
                    PointF pc = new PointF((float)(an.BoundingBox.X + (an.BoundingBox.W / 2)), (float)(an.BoundingBox.Y + (an.BoundingBox.H / 2)));
                    if (an.type == Annotation.Type.Point)
                    {
                        g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Line)
                    {
                        g.DrawLine(pen, an.GetPoint(0).ToPointF(), an.GetPoint(1).ToPointF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Rectangle)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.Rect.ToRectangleF();
                        g.DrawRectangles(pen, rects);
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Ellipse)
                    {
                        g.DrawEllipse(pen, an.Rect.ToRectangleF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Polygon && an.closed)
                    {
                        g.DrawPolygon(pen, an.GetPointsF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Polygon && !an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        }
                        else
                            g.DrawLines(pen, points);
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Polyline)
                    {
                        g.DrawLines(pen, an.GetPointsF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    
                    else
                    if (an.type == Annotation.Type.Freeform && an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length > 1)
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        }
                        else
                        g.DrawPolygon(pen, an.GetPointsF());
                    }
                    else
                    if (an.type == Annotation.Type.Freeform && !an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if(points.Length > 1)
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        }
                        else
                            g.DrawLines(pen, points);
                    }
                    if (an.type == Annotation.Type.Label)
                    {
                        g.DrawString(an.Text, an.font, b, an.Point.ToPointF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    if (labels)
                    {
                        //Lets draw the text of this ROI in the middle of the ROI
                        float fw = ((float)an.Rect.X + ((float)an.Rect.W / 2)) - ((float)an.TextSize.Width / 2);
                        float fh = ((float)an.Rect.Y + ((float)an.Rect.H / 2)) - ((float)an.TextSize.Height / 2);
                        g.DrawString(an.Text, an.font, b, new PointF(fw, fh));
                    }
                    if (bounds)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.BoundingBox.ToRectangleF();
                        g.DrawRectangles(Pens.Green, rects);
                    }
                    if (an.selected)
                    {
                        //Lets draw the bounding box.
                        RectangleF[] bo = new RectangleF[1];
                        bo[0] = an.BoundingBox.ToRectangleF();
                        g.DrawRectangles(Pens.Magenta, bo);
                        //Lets draw the selectBoxes.
                        List<RectangleF> rects = new List<RectangleF>();
                        for (int i = 0; i < an.selectedPoints.Count; i++)
                        {
                            if (an.selectedPoints[i] < an.selectBoxs.Count)
                            {
                                rects.Add(an.selectBoxs[an.selectedPoints[i]]);
                            }
                        }
                        if(rects.Count > 0)
                        g.DrawRectangles(Pens.Blue, rects.ToArray());
                        rects.Clear();
                        //Lets draw the text of this ROI in the middle of the ROI
                        float fw = ((float)an.Rect.X + ((float)an.Rect.W / 2)) - ((float)an.TextSize.Width / 2);
                        float fh = ((float)an.Rect.Y + ((float)an.Rect.H / 2)) - ((float)an.TextSize.Height / 2);
                        g.DrawString(an.Text, an.font, b, new PointF(fw, fh));
                    }
                    pen.Dispose();
                }
            }
            else
            {
                foreach (Annotation an in image.GetAnnotations(GetCoordinate()))
                {
                    pen = new Pen(an.strokeColor, (float)an.strokeWidth);
                    if (an.selected)
                    {
                        b = new SolidBrush(Color.Magenta);
                    }
                    else
                        b = new SolidBrush(an.strokeColor);
                    PointF pc = new PointF((float)(an.BoundingBox.X + (an.BoundingBox.W / 2)), (float)(an.BoundingBox.Y + (an.BoundingBox.H / 2)));
                    if (an.type == Annotation.Type.Point)
                    {
                        g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Line)
                    {
                        g.DrawLine(pen, an.GetPoint(0).ToPointF(), an.GetPoint(1).ToPointF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Rectangle)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.Rect.ToRectangleF();
                        g.DrawRectangles(pen, rects);
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Ellipse)
                    {
                        g.DrawEllipse(pen, an.Rect.ToRectangleF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Polygon && an.closed)
                    {
                        g.DrawPolygon(pen, an.GetPointsF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Polygon && !an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        }
                        else
                            g.DrawLines(pen, points);
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == Annotation.Type.Polyline)
                    {
                        g.DrawLines(pen, an.GetPointsF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }

                    else
                    if (an.type == Annotation.Type.Freeform && an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length > 1)
                            if (points.Length == 1)
                            {
                                g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                            }
                            else
                                g.DrawPolygon(pen, an.GetPointsF());
                    }
                    else
                    if (an.type == Annotation.Type.Freeform && !an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length > 1)
                            if (points.Length == 1)
                            {
                                g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                            }
                            else
                                g.DrawLines(pen, points);
                    }
                    if (an.type == Annotation.Type.Label)
                    {
                        g.DrawString(an.Text, an.font, b, an.Point.ToPointF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    if (labels)
                    {
                        //Lets draw the text of this ROI in the middle of the ROI
                        float fw = ((float)an.Rect.X + ((float)an.Rect.W / 2)) - ((float)an.TextSize.Width / 2);
                        float fh = ((float)an.Rect.Y + ((float)an.Rect.H / 2)) - ((float)an.TextSize.Height / 2);
                        g.DrawString(an.Text, an.font, b, new PointF(fw, fh));
                    }
                    if (bounds)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.BoundingBox.ToRectangleF();
                        g.DrawRectangles(Pens.Green, rects);
                    }
                    if (an.selected)
                    {
                        //Lets draw the bounding box.
                        RectangleF[] bo = new RectangleF[1];
                        bo[0] = an.BoundingBox.ToRectangleF();
                        g.DrawRectangles(Pens.Magenta, bo);
                        //Lets draw the selectBoxes.
                        List<RectangleF> rects = new List<RectangleF>();
                        for (int i = 0; i < an.selectedPoints.Count; i++)
                        {
                            if (an.selectedPoints[i] < an.selectBoxs.Count)
                            {
                                rects.Add(an.selectBoxs[an.selectedPoints[i]]);
                            }
                        }
                        if (rects.Count > 0)
                            g.DrawRectangles(Pens.Blue, rects.ToArray());
                        rects.Clear();
                        //Lets draw the text of this ROI in the middle of the ROI
                        float fw = ((float)an.Rect.X + ((float)an.Rect.W / 2)) - ((float)an.TextSize.Width / 2);
                        float fh = ((float)an.Rect.Y + ((float)an.Rect.H / 2)) - ((float)an.TextSize.Height / 2);
                        g.DrawString(an.Text, an.font, b, new PointF(fw, fh));
                    }
                    pen.Dispose();
                }
            }
        }
        private PointF origin = new PointF(0,0);
        public PointF Origin
        {
            get { return origin; }
            set 
            { 
                origin = value;
                viewer.UpdateOverlay(); 
            }
        }

        private void overlayPictureBox_Paint(object sender, PaintEventArgs e)
        {
            PointF pp = GetImagePoint();
            PointF s = GetImageSize();
            Graphics g = e.Graphics;
            g.TranslateTransform(origin.X, origin.Y);
            g.ScaleTransform(scale.Width, scale.Height);
            DrawOverlay(g);
            TabsView.graphics = g;
            if ((Tools.currentTool.type == Tools.Tool.Type.rectSel && down) || (Tools.currentTool.type == Tools.Tool.Type.magic && down))
            {
                RectangleF[] fs = new RectangleF[1];
                fs[0] = Tools.GetTool(Tools.Tool.Type.rectSel).RectangleF;
                g.DrawRectangles(Pens.Magenta, fs);
            }
            else
                Tools.GetTool(Tools.Tool.Type.rectSel).Rectangle = new RectangleD(0, 0, 0, 0);
        }
        Bitmap bm = null;
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (bm == null)
                UpdateImage();
            PointF pp = GetImagePoint();
            Graphics g = e.Graphics;
            g.TranslateTransform(origin.X, origin.Y);
            g.ScaleTransform(scale.Width, scale.Height);
            g.DrawImage(bm, 0,0, bm.Size.Width, bm.Size.Height);
        }

        public static List<Annotation> selectedAnnotations = new List<Annotation>();
        public static BioImage selectedImage = null;

        public static ImageView viewer = null;
        public static TabsView app = null;

        public static PointF mouseDown;
        public static bool down;
        public static PointF mouseUp;
        public static bool up;

        public static bool Ctrl
        {
            get
            {
                return Win32.GetKeyState(Keys.LControlKey);
            }
        }
        private bool x1State = false;
        private bool x2State = false;
        public static MouseButtons mouseUpButtons;
        public static MouseButtons mouseDownButtons;
        private PointF pd;
        private void rgbPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            selectedImage = image;
            PointF p = new PointF(e.Location.X, e.Location.Y);
            p.X -= origin.X;
            p.Y -= origin.Y;
            p.X /= scale.Width;
            p.Y /= scale.Height;
            mousePoint = "(" + p.X + ", " + p.Y + ")";
            if (Mode != ViewMode.RGBImage)
            {
                if (e.Button == MouseButtons.XButton1 && !x1State)
                {
                    if (cBar.Value < cBar.Maximum)
                        cBar.Value++;
                    x1State = true;
                }
                if (e.Button != MouseButtons.XButton1)
                    x1State = false;

                if (e.Button == MouseButtons.XButton2 && !x2State)
                {
                    if (cBar.Value > cBar.Minimum)
                        cBar.Value--;
                    x2State = true;
                }
                if (e.Button != MouseButtons.XButton2)
                    x2State = false;
            }

            if (Tools.currentTool.type == Tools.Tool.Type.move && e.Button == MouseButtons.Left)
            {
                foreach (Annotation an in selectedAnnotations)
                {
                    if (an.selectedPoints.Count > 0 && an.selectedPoints.Count < an.GetPointCount())
                    {
                        if (an.type == Annotation.Type.Rectangle || an.type == Annotation.Type.Ellipse)
                        {
                            RectangleD d = an.Rect;
                            if (an.selectedPoints[0] == 0)
                            {
                                double dw = d.X - p.X;
                                double dh = d.Y - p.Y;
                                d.X = p.X;
                                d.Y = p.Y;
                                d.W += dw;
                                d.H += dh;
                            }
                            else
                            if (an.selectedPoints[0] == 1)
                            {
                                double dw = p.X - (d.W + d.X);
                                double dh = d.Y - p.Y;
                                d.W += dw;
                                d.H += dh;
                                d.Y -= dh;
                            }
                            else
                            if (an.selectedPoints[0] == 2)
                            {
                                double dw = d.X - p.X;
                                double dh = p.Y - (d.Y + d.H);
                                d.W += dw;
                                d.H += dh;
                                d.X -= dw;
                            }
                            else
                            if (an.selectedPoints[0] == 3)
                            {
                                double dw = d.X - p.X;
                                double dh = d.Y - p.Y;
                                d.W = p.X - an.X;
                                d.H = p.Y - an.Y;
                            }
                            an.Rect = d;
                        }
                        else
                        {
                            PointD pod = new PointD(p.X - pd.X, p.Y - pd.Y);
                            for (int i = 0; i < an.selectedPoints.Count; i++)
                            {
                                PointD poid = an.GetPoint(an.selectedPoints[i]);
                                an.UpdatePoint(new PointD(poid.X + pod.X, poid.Y + pod.Y), an.selectedPoints[i]);
                            }
                        }
                    }
                    else
                    {
                        PointD pod = new PointD(p.X - pd.X, p.Y - pd.Y);
                        for (int i = 0; i < an.GetPointCount(); i++)
                        {
                            PointD poid = an.PointsD[i];
                            an.UpdatePoint(new PointD(poid.X + pod.X, poid.Y + pod.Y), i);
                        }
                    }
                }
                UpdateOverlay();
            }

            //Pencil tool
            if (Tools.currentTool != null)
            if(Tools.currentTool.type == Tools.Tool.Type.pencil && e.Button == MouseButtons.Left)
            if (Mode == ViewMode.RGBImage)
            {
                BioImage b = ImageView.viewer.image;
                ZCT co = ImageView.coordinate;
                if (b.RGBChannelCount > 1)
                {
                    ZCTXY cor = new ZCTXY(co.Z,co.C,co.T,(int)p.X,(int)p.Y);
                    Tools.Tool tool = Tools.currentTool;
                    if (Tools.rEnabled)
                    b.SetValueRGB(cor, 0, tool.Color.R);
                    if (Tools.gEnabled)
                    b.SetValueRGB(cor, 0, tool.Color.G);
                    if (Tools.bEnabled)
                    b.SetValueRGB(cor, 0, tool.Color.B);
                }
                else
                if (Mode == ViewMode.RGBImage)
                {
                    if (Tools.rEnabled)
                        image.SetValue((int)p.X, (int)p.Y, RChannel.Index, Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                    if (Tools.gEnabled)
                        image.SetValue((int)p.X, (int)p.Y, GChannel.Index, Tools.GetTool(Tools.Tool.Type.pencil).Color.G);
                    if (Tools.bEnabled)
                        image.SetValue((int)p.X, (int)p.Y, BChannel.Index, Tools.GetTool(Tools.Tool.Type.pencil).Color.B);
                }
                else
                {
                    image.SetValue((int)p.X, (int)p.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                }
                UpdateView();
            }
            else
            if (Mode == ViewMode.Filtered)
            {
                if (image.RGBChannelCount > 1)
                {
                    image.SetValue((int)p.X, (int)p.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                }
                else
                {
                    image.SetValue((int)p.X, (int)p.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                }
                UpdateView();
            }
            else
            if (Mode == ViewMode.Raw)
            {
                if (image.RGBChannelCount > 1)
                {
                image.SetValue((int)p.X, (int)p.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                }
                else
                {
                image.SetValue((int)p.X, (int)p.Y, GetCoordinate(), Tools.GetTool(Tools.Tool.Type.pencil).Color.R);
                }
                UpdateView();
            }

            UpdateStatus();
            tools.ToolMove(p, mouseDownButtons);
            pd = p;
        }
        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            viewer = this;
            selectedImage = image;
            PointF p = new PointF(e.Location.X, e.Location.Y);
            p.X -= origin.X;
            p.Y -= origin.Y;
            p.X /= scale.Width;
            p.Y /= scale.Height;
            mouseUpButtons = e.Button;
            mouseDownButtons = MouseButtons.None;
            mouseUp = p;
            down = false;
            up = true;
            tools.ToolUp(p, e.Button);
        }
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            tools.BringToFront();
            if (!Ctrl)
            {
                foreach (Annotation item in selectedAnnotations)
                {
                    if(item.selected)
                    item.selectedPoints.Clear();
                }
                selectedAnnotations.Clear();
            }
            viewer = this;
            selectedImage = image;
            mouseDownButtons = e.Button;
            mouseUpButtons = MouseButtons.None;
            PointF p = new PointF(e.Location.X, e.Location.Y);
            p.X -= origin.X;
            p.Y -= origin.Y;
            p.X /= scale.Width;
            p.Y /= scale.Height;
            mouseDown = p;
            pd = new PointF(p.X, p.Y);
            down = true;
            up = false;
            if (Tools.currentTool.type == Tools.Tool.Type.move)
            {
                foreach (Annotation an in AnnotationsRGB)
                {
                    if (an.GetSelectBound().IntersectsWith(p.X, p.Y))
                    {
                        selectedAnnotations.Add(an);
                        an.selected = true;

                        RectangleF r = new RectangleF(p.X, p.Y, 1, 1);
                        for (int i = 0; i < an.selectBoxs.Count; i++)
                        {
                            if (an.selectBoxs[i].IntersectsWith(r))
                            {
                                an.selectedPoints.Add(i);
                            }
                        }
                    }
                    else
                        if (!Ctrl)
                        an.selected = false;
                }
                UpdateOverlay();
            }

            if (e.Button == MouseButtons.Left)
            {
                Point s = GetImageSize();
                if ((p.X < s.X && p.Y < s.Y) || (p.X >= 0 && p.Y >= 0))
                {
                    int zc = coordinate.Z;
                    int cc = coordinate.C;
                    int tc = coordinate.T;
                    if (Mode == ViewMode.RGBImage)
                    {
                        if (image.RGBChannelCount == 1)
                        {
                            int r = image.GetValue(zc, RChannel.Index, tc, (int)p.X, (int)p.Y);
                            int g = image.GetValue(zc, GChannel.Index, tc, (int)p.X, (int)p.Y);
                            int b = image.GetValue(zc, BChannel.Index, tc, (int)p.X, (int)p.Y);
                            mouseColor = ", " + r + "," + g + "," + b;
                        }
                        else
                        {
                            int r = image.GetValueRGB(zc, RChannel.Index, tc, (int)p.X, (int)p.Y, 0);
                            int g = image.GetValueRGB(zc, GChannel.Index, tc, (int)p.X, (int)p.Y, 1);
                            int b = image.GetValueRGB(zc, BChannel.Index, tc, (int)p.X, (int)p.Y, 2);
                            mouseColor = ", " + r + "," + g + "," + b;
                        }
                    }
                    else
                    {
                        int r = image.GetValueRGB(zc, 0, tc, (int)p.X, (int)p.Y, 0);
                        int g = image.GetValueRGB(zc, 1, tc, (int)p.X, (int)p.Y, 1);
                        int b = image.GetValueRGB(zc, 2, tc, (int)p.X, (int)p.Y, 2);
                        mouseColor = ", " + r + "," + g + "," + b;
                    }
                }
            }

            UpdateStatus();
            tools.ToolDown(p,e.Button);
        }
        private void pictureBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            viewer = this;
            selectedImage = image;
            PointF p = new PointF(e.Location.X, e.Location.Y);
            p.X -= origin.X;
            p.Y -= origin.Y;
            p.X /= scale.Width;
            p.Y /= scale.Height;
            tools.ToolDown(p, e.Button);
        }

        private void pictureBox_Click(object sender, EventArgs e)
        {
            tools.BringToFront();
            tools.TopMost = true;
            
        }
        private void deleteROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Annotation item in AnnotationsRGB)
            {
                if (item.selected && (item.selectedPoints.Count == 0 || item.selectedPoints.Count == item.GetPointCount()))
                {
                    image.Annotations.Remove(item);
                }
                else
                {
                    if ((item.type == Annotation.Type.Polygon || item.type == Annotation.Type.Freeform || item.type == Annotation.Type.Polyline) && item.selectedPoints.Count > 0)
                    {
                        item.RemovePoints(item.selectedPoints.ToArray());
                        
                    }
                    item.selectedPoints.Clear();
                }
            }
            UpdateOverlay();
        }
        private void setTextSelectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Annotation item in AnnotationsRGB)
            {
                if(item.selected)
                {
                    TextInput input = new TextInput(item.Text);
                    if (input.ShowDialog() != DialogResult.OK)
                        return;
                    item.Text = input.textInput;
                    item.font = input.font;
                    item.strokeColor = input.color;
                    UpdateOverlay();
                }
            }
        }

        private void copyViewToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Bitmap bm = new Bitmap(pictureBox.Width, pictureBox.Height);
            using (Graphics g = Graphics.FromImage(bm))
            {
                g.CopyFromScreen(PointToScreen(new Point(pictureBox.Left, pictureBox.Top + 25)), Point.Empty, bm.Size);
            }
            Clipboard.SetImage(bm);
        }
        List<Annotation> copys = new List<Annotation>();

        public void CopySelection()
        {
            copys.Clear();
            string s = "";
            foreach (Annotation item in AnnotationsRGB)
            {
                if (item.selected)
                {
                    copys.Add(item);
                    s += BioImage.ROItoString(item);
                }
            }
            Clipboard.SetText(s);
        }
        public void PasteSelection()
        {
            string[] sts = Clipboard.GetText().Split(BioImage.NewLine);
            foreach (string line in sts)
            {
                if (line.Length > 8)
                {
                    Annotation an = BioImage.StringToROI(line);
                    //We set the coordinates of the ROI's we are pasting
                    an.coord = GetCoordinate();
                    image.Annotations.Add(an);
                }
            }
            UpdateOverlay();
        }
        private void copyROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopySelection();
        }

        private void pasteROIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSelection();
        }

        private enum KeyMessages
        {
            WM_KEYFIRST = 0x100,
            WM_KEYDOWN = 0x100,
            WM_KEYUP = 0x101,
            WM_CHAR = 0x102,
            WM_SYSKEYDOWN = 0x0104,
            WM_SYSKEYUP = 0x0105,
            WM_SYSCHAR = 0x0106,
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys key)
        {
            int moveAmount = 5;
            if (viewer != null && msg.Msg == (int)KeyMessages.WM_KEYDOWN)
            {
                if(key == Keys.Subtract || key == Keys.NumPad7)
                {
                    scale.Width -= 0.1f;
                    scale.Height -= 0.1f;
                    UpdateOverlay();
                }
                if (key == Keys.Add || key == Keys.NumPad9)
                {
                    scale.Width += 0.1f;
                    scale.Height += 0.1f;
                    UpdateOverlay();
                }
                if (key == Keys.W || key == Keys.NumPad8)
                {
                    viewer.Origin = new System.Drawing.PointF(viewer.Origin.X, viewer.Origin.Y + moveAmount);
                    return true;
                }
                if (key == Keys.S || key == Keys.NumPad2)
                {
                    viewer.Origin = new System.Drawing.PointF(viewer.Origin.X, viewer.Origin.Y - moveAmount);
                    return true;
                }
                if (key == Keys.A || key == Keys.NumPad4)
                {
                    viewer.Origin = new System.Drawing.PointF(viewer.Origin.X + moveAmount, viewer.Origin.Y);
                    return true;
                }
                if (key == Keys.D || key == Keys.NumPad6)
                {
                    viewer.Origin = new System.Drawing.PointF(viewer.Origin.X - moveAmount, viewer.Origin.Y);
                    return true;
                }
                if (key == Keys.Delete)
                {
                    Tools.currentTool = Tools.GetTool(Tools.Tool.Type.delete);
                    foreach (Annotation an in ImageView.selectedAnnotations)
                    {
                        if (an != null)
                        {
                            if (an.selectedPoints.Count == 0)
                            {
                                ImageView.selectedImage.Annotations.Remove(an);
                            }
                            else
                            if(!(an.type == Annotation.Type.Polygon || an.type == Annotation.Type.Polyline || an.type == Annotation.Type.Freeform))
                            {
                                ImageView.selectedImage.Annotations.Remove(an);
                            }
                            else
                            {
                                if (an.type == Annotation.Type.Polygon ||
                                    an.type == Annotation.Type.Polyline ||
                                    an.type == Annotation.Type.Freeform)
                                {
                                    an.closed = false;
                                    an.RemovePoints(an.selectedPoints.ToArray());
                                }
                            }
                        }
                    }
                    UpdateOverlay();
                    return true;
                }
                if (key.HasFlag(Keys.C) && Ctrl && Tools.currentTool.type == Tools.Tool.Type.move)
                {
                    viewer.CopySelection();
                    return true;
                }
                if (key.HasFlag(Keys.V) && Ctrl && Tools.currentTool.type == Tools.Tool.Type.move)
                {
                    viewer.PasteSelection();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, key);
        }

        private void hideControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowControls = false;
        }

        private void showControlToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowControls)
                ShowControls = false;
            else
                ShowControls = true;
        }

        private void HideStatusMenuItem_Click(object sender, EventArgs e)
        {
            ShowStatus = false;
        }

        private void showStatusToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ShowStatus)
                ShowStatus = false;
            else
                ShowStatus = true;
        }

        private void zBar_Scroll(object sender, EventArgs e)
        {
            ImageView.coordinate.Z = zBar.Value;
            overlayPictureBox.Invalidate();
        }

        private void timeBar_Scroll(object sender, EventArgs e)
        {
            ImageView.coordinate.T = timeBar.Value;
            overlayPictureBox.Invalidate();
        }

        private void cBar_Scroll(object sender, EventArgs e)
        {
            ImageView.coordinate.C = timeBar.Value;
            overlayPictureBox.Invalidate();
        }
    }
}
