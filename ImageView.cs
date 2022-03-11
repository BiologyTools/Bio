using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;

namespace BioImage
{
    public partial class ImageView : UserControl
    {
        public ImageView(string path)
        {
            InitializeComponent();
            tools = new Toolbox();

            if (path == "" || path == null)
                return;
            image = new BioImage(true);
            Coordinate = new BioImage.ZCT(0, 0, 0);
            image.Open(path);
            rgbPictureBox.Image = image.GetBitmap(Coordinate);
            bool isRGB;
            if (image.RGBCount > 1)
                isRGB = true;
            else
                isRGB = false;
            if (image.SizeC == 1 && !isRGB)
            {
                Mode = ViewMode.Filtered;
            }
            else
                Mode = ViewMode.RGBImage;
            
            Plane = image.GetPlane(RCoordinate);

            if (image.SizeC == 0)
            {   
                RCoordinate = new BioImage.ZCT(0, 0, 0);
                RBuf = image.GetPlane(RCoordinate);
            }
            else
            if (image.SizeC >= 1)
            {   
                GCoordinate = new BioImage.ZCT(0, 1, 0);
                GBuf = image.GetPlane(GCoordinate);
            }
            else
            if (image.SizeC > 2)
            {
                BCoordinate = new BioImage.ZCT(0, 2, 0);
                BBuf = image.GetPlane(BCoordinate);
            }


            LoadImage(path);
            //image.AutoThresholdImage();
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
            
        }
        ~ImageView()
        {
            
        }

        public BioImage image;
        private string filePath = "";
        public BioImage.ZCT Coordinate;
        public BioImage.ZCT RCoordinate;
        public BioImage.ZCT GCoordinate;
        public BioImage.ZCT BCoordinate;
        public int minSizeX = 60;
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
                ViewModeChanged();
            }
        }
        public string Path
        {
            get
            {
                return filePath;
            }
        }
        public BioImage.Buf Plane;
        public BioImage.Buf RBuf;
        public BioImage.Buf GBuf;
        public BioImage.Buf BBuf;
        public BioImage.Channel RChannel = null;
        public BioImage.Channel GChannel = null;
        public BioImage.Channel BChannel = null;

        public Toolbox tools;

        public void UpdateRGBChannels()
        {
            RChannel = image.Channels[image.rgbChannels[0]];
            GChannel = image.Channels[image.rgbChannels[1]];
            BChannel = image.Channels[image.rgbChannels[2]];
            RBuf = image.GetPlane(RCoordinate);
            GBuf = image.GetPlane(GCoordinate);
            BBuf = image.GetPlane(BCoordinate);
        }

        public void ViewModeChanged()
        {
            if (Mode == ViewMode.Raw)
            {
                rgbBoxsPanel.SendToBack();
                cPanel.BringToFront();
                cPanel.Show();
                cPanel.Visible = true;
                cBar.Visible = true;
                rgbBoxsPanel.Hide();
                if (!timeEnabled)
                {
                    cPanel.Location = new Point(0, timeBar.Location.Y);
                }
                foreach (BioImage.Channel c in image.Channels)
                {
                    c.Min = 0;
                    c.Max = ushort.MaxValue;
                }
            }
            if (Mode == ViewMode.Filtered)
            {
                rgbBoxsPanel.SendToBack();
                cPanel.BringToFront();
                cPanel.Show();
                cPanel.Visible = true;
                cBar.Visible = true;
                rgbBoxsPanel.Hide();
                if (!timeEnabled)
                {
                    cPanel.Location = new Point(0, timeBar.Location.Y);
                }
            }
            if (Mode == ViewMode.RGBImage)
            {
                cPanel.Hide();
                cBar.Enabled = true;
                cBar.Visible = false;
                cPanel.SendToBack();
                cPanel.Visible = false;
                rgbBoxsPanel.BringToFront();
                rgbBoxsPanel.Show();
                rgbBoxsPanel.Visible = true;
                labelRGB.Visible = true;
                if (image.SizeT == 0)
                {
                    rgbBoxsPanel.Location = new Point(0, timeBar.Location.Y);
                }
            }
        }

        private bool timeEnabled = false;
        public bool TimeEnabled
        {
            get
            {
                return timeEnabled;
            }
            set
            {
                timeEnabled = value;
                if (timeEnabled)
                {
                    timeBar.Enabled = true;
                    timeBar.Show();
                    timeBar.Visible = true;
                    timeLabel.Visible = true;
                    splitContainer.SplitterDistance = splitContainer.Height - 90;
                }
                else
                {
                    timeBar.Hide();
                    timeBar.Visible = false;
                    timeBar.Enabled = false;
                    timeLabel.Visible = false;
                    splitContainer.SplitterDistance = splitContainer.Height - 70;
                }
            }
        }

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

        public int SplitterDistance
        {
            get
            {
                return splitContainer.SplitterDistance;
            }
            set
            {
                splitContainer.SplitterDistance = value;
            }
        }

        public bool LoadImage(string path)
        {
            filePath = path;
            //image = new BioImage(filePath);
            zBar.Maximum = image.SizeZ-1;
            cBar.Maximum = image.SizeC-1;
            if (image.SizeT > 1)
            {
                timeBar.Maximum = image.imageReader.getSizeT()-1;
                TimeEnabled = true;
            }
            else
            {
                TimeEnabled = false;
            }
            //rgbPictureBox.Image = image.plane.GetBitmap();
            //we clear the channel comboboxes incase we have channels from previous loaded image.
            channelBoxR.Items.Clear();
            channelBoxG.Items.Clear();
            channelBoxB.Items.Clear();
            foreach (BioImage.Channel ch in image.Channels)
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
            if (image.Channels.Count == 2)
            {
                channelBoxR.SelectedIndex = 0;
                channelBoxG.SelectedIndex = 1;
            }
            if (image.Channels.Count == 1)
            {
                channelBoxR.SelectedIndex = 0;
            }
            //We threshold the image so that the max threshold value is the max pixel value in image. 
            image.AutoThreshold();

            return true;
        }

        public void UpdateView()
        {
            if (Mode == ViewMode.Raw)
            {
                Coordinate = new BioImage.ZCT(zBar.Value, cBar.Value, timeBar.Value);
                rgbPictureBox.Image = image.GetBitmap(Coordinate); 
            }
            else
            if (Mode == ViewMode.Filtered)
            {
                Coordinate = new BioImage.ZCT(zBar.Value, cBar.Value, timeBar.Value);
                rgbPictureBox.Image = image.GetFiltered(Coordinate, image.Channels[0].range);
            }
            else
            if (Mode == ViewMode.RGBImage)
            {
                Coordinate = new BioImage.ZCT(zBar.Value, cBar.Value, timeBar.Value);
                RCoordinate = new BioImage.ZCT(zBar.Value, 0, timeBar.Value);
                GCoordinate = new BioImage.ZCT(zBar.Value, 1, timeBar.Value);
                BCoordinate = new BioImage.ZCT(zBar.Value, 2, timeBar.Value);
                RBuf = image.GetBuffer(RCoordinate);
                GBuf = image.GetBuffer(GCoordinate);
                BBuf = image.GetBuffer(BCoordinate);
                AForge.IntRange rr = image.Channels[image.rgbChannels[0]].range;
                AForge.IntRange gr = image.Channels[image.rgbChannels[1]].range;
                AForge.IntRange br = image.Channels[image.rgbChannels[2]].range;
                rgbPictureBox.Image = image.GetRGBBitmap(Coordinate, rr, gr, br);
            }
            UpdateStatus();
        }

        public void UpdateStatus()
        {
            if (timeEnabled)
            {
                statusLabel.Text = zBar.Value + "/" + zBar.Maximum + ", " + timeBar.Value + "/" + timeBar.Maximum + ", " + mouseColor ;
            }
            else
            {
                statusLabel.Text = zBar.Value + "/" + zBar.Maximum + ", " + mouseColor;
            }

            float ms = image.loadTimeMS;
            statusLabel.Text += " FrameTime:" + ms + " ms" + " Ticks:" + image.loadTimeTicks;

        }

        private void channelBoxR_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxR.SelectedIndex == -1)
                return;
            BioImage.Channel ch = (BioImage.Channel)channelBoxR.SelectedItem;
            RCoordinate.C = ch.Index;
            image.Channels[image.rgbChannels[0]] = ch;
            image.rgbChannels[0] = ch.Index;
            //image.SetRGBChannelIndex(BioImage.RGB.Red, ch.Index);
            //image.rgbimage.RChannel = ch;
            UpdateRGBChannels();
            UpdateView();
        }

        private void channelBoxG_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxG.SelectedIndex == -1)
                return;
            BioImage.Channel ch = (BioImage.Channel)channelBoxG.SelectedItem;
            GCoordinate.C = ch.Index;
            image.Channels[image.rgbChannels[1]] = ch;
            image.rgbChannels[0] = ch.Index;
            //image.SetRGBChannelIndex(BioImage.RGB.Green, ch.Index);
            //image.rgbimage.GChannel = ch;
            UpdateRGBChannels();
            UpdateView();
        }

        private void channelBoxB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxB.SelectedIndex == -1)
                return;
            BioImage.Channel ch = (BioImage.Channel)channelBoxB.SelectedItem;
            BCoordinate.C = ch.Index;
            image.Channels[image.rgbChannels[2]] = ch;
            image.rgbChannels[0] = ch.Index;
            //image.SetRGBChannelIndex(BioImage.RGB.Blue, ch.Index);
            //image.rgbimage.BChannel = ch;
            UpdateRGBChannels();
            UpdateView();
        }

        private void normalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            autoSizeToolStripMenuItem.Checked = false;
            centerToolStripMenuItem.Checked = false;
            strechToolStripMenuItem.Checked = false;
            normalToolStripMenuItem.Checked = true;
            zoomToolStripMenuItem.Checked = false;
            rgbPictureBox.SizeMode = PictureBoxSizeMode.Normal;
        }

        private void strechToolStripMenuItem_Click(object sender, EventArgs e)
        {
            normalToolStripMenuItem.Checked = false;
            autoSizeToolStripMenuItem.Checked = false;
            centerToolStripMenuItem.Checked = false;
            strechToolStripMenuItem.Checked = true;
            zoomToolStripMenuItem.Checked = false;
            rgbPictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
        }

        private void autoSizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            normalToolStripMenuItem.Checked = false;
            autoSizeToolStripMenuItem.Checked = true;
            centerToolStripMenuItem.Checked = false;
            strechToolStripMenuItem.Checked = false;
            zoomToolStripMenuItem.Checked = false;
            rgbPictureBox.SizeMode = PictureBoxSizeMode.AutoSize;
        }

        private void centerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            normalToolStripMenuItem.Checked = false;
            autoSizeToolStripMenuItem.Checked = false;
            centerToolStripMenuItem.Checked = true;
            strechToolStripMenuItem.Checked = false;
            zoomToolStripMenuItem.Checked = false;
            rgbPictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
        }

        private void zoomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            normalToolStripMenuItem.Checked = false;
            autoSizeToolStripMenuItem.Checked = false;
            centerToolStripMenuItem.Checked = false;
            strechToolStripMenuItem.Checked = false;
            zoomToolStripMenuItem.Checked = true;
            rgbPictureBox.SizeMode = PictureBoxSizeMode.Zoom;
        }

        private void showControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(splitContainer.Panel2Collapsed)
            {
                splitContainer.Panel2Collapsed = false;
                showControlsToolStripMenuItem.Text = "Hide Controls";
            }
            else
            {
                splitContainer.Panel2Collapsed = true;
                showControlsToolStripMenuItem.Text = "Show Controls";
            }

        }

        private void ImageView_SizeChanged(object sender, EventArgs e)
        {
            if (splitContainer.Height <= minSizeY)
                return;
            if (splitContainer.Height <= 0)
                return;
            if (timeEnabled)
                splitContainer.SplitterDistance = splitContainer.Height - 90;
            else
                splitContainer.SplitterDistance = splitContainer.Height - 70;
        }
        
        private void ImageView_Resize(object sender, EventArgs e)
        {
            if (splitContainer.Height <= minSizeY)
                return;
            if (splitContainer.Height <= 0)
                return;

            if (timeEnabled)
                splitContainer.SplitterDistance = splitContainer.Height - 90;
            else
                splitContainer.SplitterDistance = splitContainer.Height - 70;
        }

        private void hideTimeTrackbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timeEnabled = false;
            showTimeTrackbarToolStripMenuItem.Visible = true;
        }

        private void showTimeTrackbarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (image.SizeT > 1)
            {
                timeEnabled = true;
                showTimeTrackbarToolStripMenuItem.Visible = false;
            }
        }

        private void openChannelsToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ChannelsTool channelsTool = new ChannelsTool(image.Channels);
            if (channelsTool.ShowDialog() != DialogResult.OK)
                return;
            image.Channels = channelsTool.Channels;
            UpdateView();
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
            stopCToolStripMenuItem.Checked = true;
            playCToolStripMenuItem.Checked = false;
            cTimer.Stop();
        }

        private void stopCToolStripMenuItem_Click(object sender, EventArgs e)
        {
            stopCToolStripMenuItem.Checked = false;
            playCToolStripMenuItem.Checked = true;
            cTimer.Stop();
        }

        private Point toImagePoint(int px,int py)
        {
            int x, y;
            Point p = new Point(px, py);
            if (rgbPictureBox.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                x = (int)((((float)p.X) / (float)rgbPictureBox.Width) * (float)image.SizeX);
                y = (int)((((float)p.Y) / (float)rgbPictureBox.Height) * (float)image.SizeY);
                return new Point(x, y);
            }
            if (rgbPictureBox.SizeMode == PictureBoxSizeMode.Zoom)
            {
                //First we calculate the scaling factor for image width based on picturebox width
                float fw = (float)rgbPictureBox.Width / (float)image.SizeX;
                //Next we calculate the scaling factor for image height based on picturebox height
                float fh = (float)rgbPictureBox.Height / (float)image.SizeY;
                //Next we calculate the (0,0) origin point of the image in the picturebox.
                float x0 = (rgbPictureBox.Width - (fh * image.SizeX)) / 2;
                if (x0 < 0)
                    x0 = 0;
                float y0 = (rgbPictureBox.Height - (fw * image.SizeY)) / 2;
                if (y0 < 0)
                    y0 = 0;
                float sw = (float)image.SizeX / (float)(rgbPictureBox.Width - (x0*2));
                float sh = (float)image.SizeY / (float)(rgbPictureBox.Height - (y0*2));
                float xp = (px - x0) * sw;
                float yp = (py - y0) * sh;
                return new Point((int)xp,(int)yp);
            }
            if (rgbPictureBox.SizeMode == PictureBoxSizeMode.Normal || rgbPictureBox.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                x = p.X;
                y = p.Y;
                if (x > image.SizeX)
                    x = image.SizeX;
                if (y > image.SizeY)
                    y = image.SizeY;
                if (x < 0)
                    x = 0;
                if (y < 0)
                    y = 0;
                return new Point(x, y);
            }
            if (rgbPictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                //We calculate the center position
                float cpx = (float)(rgbPictureBox.Width / 2);
                float cpy = (float)(rgbPictureBox.Height / 2);
                //we calculate position of image X:0 Y:0.
                float x0 = cpx - ((float)image.SizeX / 2);
                float y0 = cpy - ((float)image.SizeY / 2);
                x = p.X - (int)x0;
                y = p.Y - (int)y0;
                if (x < 0)
                    x = 0;
                if (y < 0)
                    y = 0;
                if (x > image.SizeX)
                    x = image.SizeX;
                if (y > image.SizeY)
                    y = image.SizeY;
                return new Point(x, y);
            }
            return new Point(-1,-1);
        }

        public string mouseColor = "";
        private void rgbPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = toImagePoint(e.Location.X, e.Location.Y);
            if (Mode == ViewMode.RGBImage)
            {
                if (image.RGBCount == 1)
                {
                    int r = RBuf.GetValue(p.X, p.Y);
                    int g = GBuf.GetValue(p.X, p.Y);
                    int b = BBuf.GetValue(p.X, p.Y);
                    mouseColor = "(" + p.X + "," + p.Y + "), " + r + "," + g + "," + b;
                }
                else
                {
                    int r = Plane.GetValue(p.X, p.Y, 0);
                    int g = Plane.GetValue(p.X, p.Y, 1);
                    int b = Plane.GetValue(p.X, p.Y, 2);
                    mouseColor = "(" + p.X + "," + p.Y + "), " + r + "," + g + "," + b;
                }
                if (e.Button == MouseButtons.Left)
                {
                    if (Plane.RGBChannelsCount > 1)
                    {
                        Plane.SetValueRGB(p.X, p.Y, cBar.Value, ushort.MaxValue);
                    }
                    else
                    {
                        image.SetValue(p.X, p.Y, Coordinate, ushort.MaxValue);
                    }
                    UpdateView();
                }
            }
            else
            if(Mode == ViewMode.Filtered)
            {
                int r = Plane.GetValue(p.X, p.Y);
                mouseColor = "(" + p.X + "," + p.Y + "), " + r;
                if (e.Button == MouseButtons.Left)
                {
                    if (image.RGBCount > 1)
                    {
                        Plane.SetValueRGB(p.X, p.Y, cBar.Value, ushort.MaxValue); 
                    }
                    else
                    {
                        image.SetValue(p.X, p.Y, Coordinate, ushort.MaxValue);
                    }
                    UpdateView();
                }
            }
            else
            if (Mode == ViewMode.Raw)
            {
                int r = Plane.GetValue(p.X, p.Y);
                mouseColor = "(" + p.X + "," + p.Y + "), " + r;
                if (e.Button == MouseButtons.Left)
                {
                    if (image.RGBCount > 1)
                    {
                        Plane.SetValueRGB(p.X, p.Y, cBar.Value, ushort.MaxValue);
                    }
                    else
                    {
                        image.SetValue(p.X, p.Y, Coordinate, ushort.MaxValue);
                    }
                    UpdateView();
                }
            }
            UpdateStatus();
        }

        private void autoContrastChannelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.AutoThreshold();
            UpdateView();
        }

        private void playSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if(Mode == ViewMode.RGBImage)
             sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            if (Mode == ViewMode.Filtered)
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (Mode == ViewMode.Raw)
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            ZFps = sp.ZFps;
            TimeFps = sp.TimeFps;
            CFps = sp.CFps;
        }
        private void playSpeedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if (Mode == ViewMode.RGBImage)
                sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            if (Mode == ViewMode.Filtered)
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            ZFps = sp.ZFps;
            TimeFps = sp.TimeFps;
            CFps = sp.CFps;
        }
        private void CPlaySpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if (Mode == ViewMode.RGBImage)
                sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            if (Mode == ViewMode.Filtered)
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            ZFps = sp.ZFps;
            TimeFps = sp.TimeFps;
            CFps = sp.CFps;
        }
        private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /*
            if (Mode == ViewMode.RGBImage)
                Clipboard.SetImage(image.rgbBitmap);
            if (Mode != ViewMode.Plane)
                Clipboard.SetImage(image.planeBitmap);
            */
        }

        private void ImageView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && e.Control)
            {
                /*
                if(Mode == ViewMode.RGBImage)   
                    Clipboard.SetImage(image.rgbBitmap);
                if(Mode != ViewMode.Plane)
                    Clipboard.SetImage(image.planeBitmap);
                */
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

        
        private void ImageView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
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
            if (Mode != ViewMode.Filtered)
                return;
            if (e.Delta > 0)
            {
                if (cBar.Value + 1 <= cBar.Maximum)
                    cBar.Value += 1;
            }
            else
            {
                if (cBar.Value - 1 >= cBar.Minimum)
                    cBar.Value -= 1;
            }
        }

        private void cTimer_Tick(object sender, EventArgs e)
        {
            if (playCToolStripMenuItem.Checked)
            {
                if (cBar.Maximum >= cBar.Value + 1)
                    cBar.Value++;
                else
                {
                    if(loopC)
                    cBar.Value = cBar.Minimum;
                }
            }
        }
        private void zTimer_Tick(object sender, EventArgs e)
        {
            if(playZToolStripMenuItem.Checked)
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
                    if(loopT)
                    timeBar.Value = timeBar.Minimum;
                }
            }
        }

        private void zBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateView();
        }
        private void timeBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateView();
        }
        private void cBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateView();
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

        private void rawModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = ViewMode.Raw;
            rGBImageModeToolStripMenuItem.Checked = false;
            rawModeToolStripMenuItem.Checked = false;
        }
        private void planeModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(planeModeToolStripMenuItem.Checked)
            {
                Mode = ViewMode.Filtered;
            }
            else
            {
                Mode = ViewMode.RGBImage;
            }
            UpdateView();
        }
        private void rGBImageModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Mode = ViewMode.RGBImage;
            rawModeToolStripMenuItem.Checked = false;
            planeModeToolStripMenuItem.Checked = false;
        }
    }
}
