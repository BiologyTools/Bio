using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BioImage
{
    public partial class ImageView : UserControl
    {
        public ImageView(string file, int ser)
        {
            file = file.Replace("\\", "/");
            InitializeComponent();
            serie = ser;

            tools = new Tools();
            if (file == "" || file == null)
                return;
            SetCoordinate(ser, 0, 0, 0);

            image = new BioImage(file, ser);
            InitGUI(file);

            Buf = image.GetBufByCoord(GetCoordinate());
            
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
            string file = im.Filename.Replace("\\", "/");
            InitializeComponent();
            serie = im.serie;
            image = im;
            tools = new Tools();
            if (file == "" || file == null)
                return;
            SetCoordinate(serie, 0, 0, 0);
            InitGUI(file);

            Buf = image.GetBufByCoord(GetCoordinate());

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
        public static bool showText = false;
        public BioImage image;
        public string filepath = "";
        public int serie = 0;
        private BioImage.SZCT coordinate = new BioImage.SZCT(0, 0, 0, 0);
        public static BioImage.SZCT Coordinate = new BioImage.SZCT(0, 0, 0, 0);
        public void SetCoordinate(int s, int z, int c, int t)
        {
            coordinate = new BioImage.SZCT(s, z, c, t);
            Coordinate = coordinate;
        }
        public BioImage.SZCT GetCoordinate()
        {
            return coordinate;
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
                ImageViewer.app.UpdateViewMode(viewMode);
                UpdateView();
                UpdateOverlay();
                if (viewMode == ViewMode.RGBImage)
                {
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
        public BioImage.Buf Buf;
        public BioImage.Channel RChannel
        {
            get
            {
                return image.Channels[image.rgbChannels[0]];
            }
        }
        public BioImage.Channel GChannel
        {
            get
            {
                return image.Channels[image.rgbChannels[1]];
            }
        }
        public BioImage.Channel BChannel
        {
            get
            {
                return image.Channels[image.rgbChannels[2]];
            }
        }

        public Tools tools;

        public void UpdateRGBChannels()
        {
            Buf = image.GetBufByCoord(GetCoordinate());
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
            foreach (BioImage.Channel ch in image.Channels)
            {
                channelBoxR.Items.Add(ch);
                channelBoxG.Items.Add(ch);
                channelBoxB.Items.Add(ch);
            }
            if(image.RGBChannelCount == 1)
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
            image.AutoThreshold();
            
        }

        public void UpdateSelectBoxSize(float size)
        {
            foreach (BioImage.Annotation item in image.Annotations)
            {
                item.selectBoxSize = size;
            }
        }

        public void UpdateOverlay()
        {
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
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (timeBar.Value + 1) + "/" + (timeBar.Maximum + 1) + ", " + mousePoint + mouseColor;
                }
                else
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + mousePoint;
                }
                
            }
            else
            {
                if (timeEnabled)
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (cBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + (timeBar.Value + 1) + "/" + (timeBar.Maximum + 1) + ", " + mousePoint + mouseColor;
                }
                else
                {
                    statusLabel.Text = (zBar.Value + 1) + "/" + (zBar.Maximum + 1) + ", " + (cBar.Value + 1) + "/" + (cBar.Maximum + 1) + ", " + mousePoint + mouseColor;
                }
            }
        }

        Bitmap bitmap;
        public void UpdateView()
        {
            if(bitmap!=null)
            bitmap.Dispose();
            if (Mode == ViewMode.Raw)
            {
                SetCoordinate(image.serie, zBar.Value, cBar.Value, timeBar.Value);
                bitmap = image.GetImageRaw(GetCoordinate());
            }
            else
            if (Mode == ViewMode.Filtered)
            {
                SetCoordinate(image.serie, zBar.Value, cBar.Value, timeBar.Value);
                bitmap = image.GetImageFiltered(GetCoordinate(), image.Channels[cBar.Value].range);
            }
            else
            if (Mode == ViewMode.RGBImage)
            {
                SetCoordinate(image.serie, zBar.Value, image.Channels[image.rgbChannels[0]].Index, timeBar.Value);
                UpdateRGBChannels();
                if (image.RGBChannelCount == 1)
                {
                    if (image.bitsPerPixel > 8)
                    {
                        if (image.SizeC == 1)
                        {
                            bitmap = image.GetImageRGB(GetCoordinate(), RChannel.range, RChannel.range, RChannel.range);
                        }
                        else
                        {
                            bitmap = image.GetImageRGB(GetCoordinate(), RChannel.range, GChannel.range, BChannel.range);
                        }
                    }
                    else
                    {
                        bitmap = image.GetImageRGB(GetCoordinate());
                    }
                }
                else
                {
                    bitmap = image.GetImageRGB(GetCoordinate());
                }
            }
            pictureBox.Invalidate();
            UpdateStatus();
        }

        private void channelBoxR_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxR.SelectedIndex == -1)
                return;
            image.rgbChannels[0] = channelBoxR.SelectedIndex;
            UpdateView();
        }

        private void channelBoxG_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxG.SelectedIndex == -1)
                return;
            image.rgbChannels[1] = channelBoxG.SelectedIndex;
            UpdateView();
        }

        private void channelBoxB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxB.SelectedIndex == -1)
                return;
            image.rgbChannels[2] = channelBoxB.SelectedIndex;
            UpdateView();
        }

        private void showControlsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (trackBarPanel.Visible)
            {
                trackBarPanel.Hide();
                pictureBox.Height += trackBarPanel.Height;
                showControlsToolStripMenuItem.Text = "Show Controls";
            }
            else
            {
                trackBarPanel.Show();
                pictureBox.Height -= trackBarPanel.Height;
                showControlsToolStripMenuItem.Text = "Hide Controls";
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
       
        private void autoContrastChannelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.AutoThreshold();
            UpdateView();
        }

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

        public SizeF scale = new SizeF(1, 1);
        private void ImageView_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            if (Ctrl)
            {
                if (e.Delta > 0)
                {
                    scale.Width += 0.1f;
                    scale.Height += 0.1f;
                }
                else
                {
                    scale.Width -= 0.1f;
                    scale.Height -= 0.1f;
                }
                UpdateView();
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

        private int zBarCoord = 0;
        private int tBarCoord = 0;
        private int cBarCoord = 0;
        private void zBar_ValueChanged(object sender, EventArgs e)
        {
            if (zBarCoord != zBar.Value)
            {
                UpdateView();
                zBarCoord = zBar.Value;
            }
        }
        private void timeBar_ValueChanged(object sender, EventArgs e)
        {
            if (tBarCoord != timeBar.Value)
            {
                UpdateView();
                tBarCoord = timeBar.Value;
            }
        }
        private void cBar_ValueChanged(object sender, EventArgs e)
        {
            if (cBarCoord != cBar.Value)
            {
                UpdateView();
                cBarCoord = cBar.Value;
            }
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
        private Point toImagePoint(int px, int py)
        {
            int x, y;
            Point p = new Point(px, py);
            if (pictureBox.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                x = (int)((((float)p.X) / (float)pictureBox.Width) * (float)image.SizeX);
                y = (int)((((float)p.Y) / (float)pictureBox.Height) * (float)image.SizeY);
                return new Point(x, y);
            }
            if (pictureBox.SizeMode == PictureBoxSizeMode.Zoom)
            {
                //First we calculate the scaling factor for image width based on picturebox width
                float fw = (float)pictureBox.Width / (float)image.SizeX;
                //Next we calculate the scaling factor for image height based on picturebox height
                float fh = (float)pictureBox.Height / (float)image.SizeY;
                //Next we calculate the (0,0) origin point of the image in the picturebox.
                float x0 = (pictureBox.Width - (fh * image.SizeX)) / 2;
                if (x0 < 0)
                    x0 = 0;
                float y0 = (pictureBox.Height - (fw * image.SizeY)) / 2;
                if (y0 < 0)
                    y0 = 0;
                float sw = (float)image.SizeX / (float)(pictureBox.Width - (x0 * 2));
                float sh = (float)image.SizeY / (float)(pictureBox.Height - (y0 * 2));
                float xp = (px - x0) * sw;
                float yp = (py - y0) * sh;
                return new Point((int)xp, (int)yp);
            }
            if (pictureBox.SizeMode == PictureBoxSizeMode.Normal || pictureBox.SizeMode == PictureBoxSizeMode.AutoSize)
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
            if (pictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                //We calculate the center position
                float cpx = (float)(pictureBox.Width / 2);
                float cpy = (float)(pictureBox.Height / 2);
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
            return new Point(-1, -1);
        }

        private Point GetImagePoint()
        {
            int x, y;
            if (pictureBox.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                x = (int)((((float)0) / (float)pictureBox.Width) * (float)image.SizeX);
                y = (int)((((float)0) / (float)pictureBox.Height) * (float)image.SizeY);
                return new Point(x, y);
            }
            if (pictureBox.SizeMode == PictureBoxSizeMode.Zoom)
            {
                //First we calculate the scaling factor for image width based on picturebox width
                float fw = (float)pictureBox.Width / (float)image.SizeX;
                //Next we calculate the scaling factor for image height based on picturebox height
                float fh = (float)pictureBox.Height / (float)image.SizeY;
                //Next we calculate the (0,0) origin point of the image in the picturebox.
                float x0 = (pictureBox.Width - (fh * image.SizeX)) / 2;
                if (x0 < 0)
                    x0 = 0;
                float y0 = (pictureBox.Height - (fw * image.SizeY)) / 2;
                if (y0 < 0)
                    y0 = 0;
                return new Point((int)x0, (int)y0);
            }
            if (pictureBox.SizeMode == PictureBoxSizeMode.Normal || pictureBox.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                return new Point(0, 0);
            }
            if (pictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                //We calculate the center position
                float cpx = (float)(pictureBox.Width / 2);
                float cpy = (float)(pictureBox.Height / 2);
                //we calculate position of image X:0 Y:0.
                float x0 = cpx - ((float)image.SizeX / 2);
                float y0 = cpy - ((float)image.SizeY / 2);
                x = (int)x0;
                y = (int)y0;
                if (x < 0)
                    x = 0;
                if (y < 0)
                    y = 0;
                if (x > image.SizeX)
                    x = image.SizeX;
                if (y > image.SizeY)
                    y = image.SizeY;
                return new Point((int)x0, (int)y0);
            }
            return new Point(-1, -1);
        }
        private Point GetImageSize()
        {
            if(image==null)
                return new Point(pictureBox.Width, pictureBox.Height);
            int x, y;
            if (pictureBox.SizeMode == PictureBoxSizeMode.StretchImage)
            {
                return new Point(pictureBox.Width, pictureBox.Height);
            }
            if (pictureBox.SizeMode == PictureBoxSizeMode.Zoom)
            {
                //First we calculate the scaling factor for image width based on picturebox width
                float fw = (float)pictureBox.Width / (float)image.SizeX;
                //Next we calculate the scaling factor for image height based on picturebox height
                float fh = (float)pictureBox.Height / (float)image.SizeY;
                //Next we calculate the (0,0) origin point of the image in the picturebox.
                float x0 = (pictureBox.Width - (fh * image.SizeX)) / 2;
                if (x0 < 0)
                    x0 = 0;
                float y0 = (pictureBox.Height - (fw * image.SizeY)) / 2;
                if (y0 < 0)
                    y0 = 0;
                float sw = (float)image.SizeX / (float)(pictureBox.Width - (x0 * 2));
                float sh = (float)image.SizeY / (float)(pictureBox.Height - (y0 * 2));

                int wi = (int)(pictureBox.Width - (x0 * 2));
                int hi = (int)(pictureBox.Height - (y0 * 2));

                if (wi > image.SizeX)
                    wi = image.SizeX;
                if (hi > image.SizeY)
                    hi = image.SizeY;
                return new Point(wi, hi);
            }
            if (pictureBox.SizeMode == PictureBoxSizeMode.Normal || pictureBox.SizeMode == PictureBoxSizeMode.AutoSize)
            {
                return new Point(image.SizeX, image.SizeY);
            }
            if (pictureBox.SizeMode == PictureBoxSizeMode.CenterImage)
            {
                //We calculate the center position
                float cpx = (float)(pictureBox.Width / 2);
                float cpy = (float)(pictureBox.Height / 2);
                //we calculate position of image X:0 Y:0.
                float x0 = cpx - ((float)image.SizeX / 2);
                float y0 = cpy - ((float)image.SizeY / 2);
                x = (int)(pictureBox.Width - (x0 * 2));
                y = (int)(pictureBox.Height - (y0 * 2));
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
            return new Point(-1, -1);
        }

        public bool showRROIs = true;
        public bool showGROIs = false;
        public bool showBROIs = false;

        private List<BioImage.Annotation> annotationsR = new List<BioImage.Annotation>();
        public List<BioImage.Annotation> AnnotationsR
        {
            get
            {
                BioImage.SZCT coord = coordinate;
                return image.GetAnnotations(coord.S, coord.Z, image.RChannel.Index, coord.T);
            }
        }
        private List<BioImage.Annotation> annotationsG = new List<BioImage.Annotation>();
        public List<BioImage.Annotation> AnnotationsG
        {
            get
            {
                BioImage.SZCT coord = coordinate;
                return image.GetAnnotations(coord.S, coord.Z, image.GChannel.Index, coord.T);
            }
        }
        private List<BioImage.Annotation> annotationsB = new List<BioImage.Annotation>();
        public List<BioImage.Annotation> AnnotationsB
        {
            get
            {
                BioImage.SZCT coord = coordinate;
                return image.GetAnnotations(coord.S, coord.Z, image.BChannel.Index, coord.T);
            }
        }

        public List<BioImage.Annotation> AnnotationsRGB
        {
            get
            {
                BioImage.SZCT coord = coordinate;
                List<BioImage.Annotation> ans = new List<BioImage.Annotation>();
                if(showRROIs)
                    ans.AddRange(AnnotationsR);
                if (showGROIs)
                    ans.AddRange(AnnotationsG);
                if (showRROIs)
                    ans.AddRange(AnnotationsB);
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
                foreach (BioImage.Annotation an in AnnotationsRGB)
                {
                    pen = new Pen(an.strokeColor, (float)an.strokeWidth);
                    if (an.selected)
                    {
                        b = new SolidBrush(Color.Magenta);
                    }
                    else
                        b = new SolidBrush(an.strokeColor);
                    PointF pc = new PointF((float)(an.BoundingBox.X + (an.BoundingBox.W / 2)), (float)(an.BoundingBox.Y + (an.BoundingBox.H / 2)));
                    if (an.type == BioImage.Annotation.Type.Point)
                    {
                        g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Line)
                    {
                        g.DrawLine(pen, an.GetPoint(0).ToPointF(), an.GetPoint(1).ToPointF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Rectangle)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.Rect.ToRectangleF();
                        g.DrawRectangles(pen, rects);
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Ellipse)
                    {
                        g.DrawEllipse(pen, an.Rect.ToRectangleF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Polygon && an.closed)
                    {
                        g.DrawPolygon(pen, an.GetPointsF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Polygon && !an.closed)
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
                    if (an.type == BioImage.Annotation.Type.Polyline)
                    {
                        g.DrawLines(pen, an.GetPointsF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    
                    else
                    if (an.type == BioImage.Annotation.Type.Freeform && an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        }
                        else
                        g.DrawPolygon(pen, an.GetPointsF());
                        if (an.selected)
                            g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Freeform && !an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        }
                        else
                            g.DrawLines(pen, points);
                        if (an.selected)
                            g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    if (an.type == BioImage.Annotation.Type.Label)
                    {
                        g.DrawString(an.Text, an.font, b, an.Point.ToPointF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    if (labels)
                        g.DrawString(an.Text, an.font, b, pc);
                    if (bounds)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.BoundingBox.ToRectangleF();
                        g.DrawRectangles(Pens.Green, rects);
                       
                    }
                    if (an.selected)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.BoundingBox.ToRectangleF();
                        g.DrawRectangles(Pens.Magenta, rects);
                    }
                    if (an.selected)
                    {
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
                    }
                    pen.Dispose();
                }
            }
            else
            {
                foreach (BioImage.Annotation an in image.GetAnnotations(Coordinate))
                {
                    pen = new Pen(an.strokeColor, (float)an.strokeWidth);
                    if (an.selected)
                    {
                        b = new SolidBrush(Color.Magenta);
                    }
                    else
                        b = new SolidBrush(an.strokeColor);
                    PointF pc = new PointF((float)(an.BoundingBox.X + (an.BoundingBox.W / 2)), (float)(an.BoundingBox.Y + (an.BoundingBox.H / 2)));
                    if (an.type == BioImage.Annotation.Type.Point)
                    {
                        g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Line)
                    {
                        g.DrawLine(pen, an.GetPoint(0).ToPointF(), an.GetPoint(1).ToPointF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Rectangle)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.Rect.ToRectangleF();
                        g.DrawRectangles(pen, rects);
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Ellipse)
                    {
                        g.DrawEllipse(pen, an.Rect.ToRectangleF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Polygon && an.closed)
                    {
                        g.DrawPolygon(pen, an.GetPointsF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Polygon && !an.closed)
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
                    if (an.type == BioImage.Annotation.Type.Polyline)
                    {
                        g.DrawLines(pen, an.GetPointsF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }

                    else
                    if (an.type == BioImage.Annotation.Type.Freeform && an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        }
                        else
                            g.DrawPolygon(pen, an.GetPointsF());
                        if (an.selected)
                            g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    else
                    if (an.type == BioImage.Annotation.Type.Freeform && !an.closed)
                    {
                        PointF[] points = an.GetPointsF();
                        if (points.Length == 1)
                        {
                            g.DrawLine(pen, an.Point.ToPointF(), new PointF((float)an.Point.X + 1, (float)an.Point.Y + 1));
                        }
                        else
                            g.DrawLines(pen, points);
                        if (an.selected)
                            g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                        
                    }
                    if (an.type == BioImage.Annotation.Type.Label)
                    {
                        g.DrawString(an.Text, an.font, b, an.Point.ToPointF());
                        g.DrawRectangles(Pens.Red, an.selectBoxs.ToArray());
                    }
                    if (labels)
                        g.DrawString(an.Text, an.font, b, pc);
                    if (bounds)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.BoundingBox.ToRectangleF();
                        g.DrawRectangles(Pens.Green, rects);
                    }
                    if (an.selected)
                    {
                        RectangleF[] rects = new RectangleF[1];
                        rects[0] = an.BoundingBox.ToRectangleF();
                        g.DrawRectangles(Pens.Magenta, rects);
                    }
                    if (an.selected)
                    {
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
                    }
                    
                }
            }
        }
        private PointF origin = new PointF(0,0);
        public PointF Origin
        {
            get { return origin; }
            set { origin = value; }
        }

        private void overlayPictureBox_Paint(object sender, PaintEventArgs e)
        {
            PointF pp = GetImagePoint();
            PointF s = GetImageSize();
            Graphics g = e.Graphics;
            g.TranslateTransform(origin.X, origin.Y);
            g.ScaleTransform(scale.Width, scale.Height);
            DrawOverlay(g);
            if (Tools.currentTool.type == Tools.Tool.Type.rectSel && down)
            {
                RectangleF[] fs = new RectangleF[1];
                fs[0] = Tools.rectSel.Selection.ToRectangleF();
                g.DrawRectangles(Pens.Magenta, fs);
            }
            else
                Tools.rectSel.Selection = new BioImage.RectangleD(0, 0, 0, 0);
        }
        private void pictureBox_Paint(object sender, PaintEventArgs e)
        {
            if (bitmap == null)
                return;
            PointF pp = GetImagePoint();
            Graphics g = e.Graphics;
            g.TranslateTransform(origin.X, origin.Y);
            g.ScaleTransform(scale.Width, scale.Height);
            PointF ss = GetImageSize();
            g.DrawImage(bitmap,pp.X, pp.Y, ss.X, ss.Y);
        }
        private void hideStatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (statusPanel.Visible == true)
            {
                statusPanel.Visible = false;
                showStatusBarToolStripMenuItem.Visible = true;
            }
            else
            {
                statusPanel.Visible = true;
                showStatusBarToolStripMenuItem.Visible = false;
            }
        }

        public static List<BioImage.Annotation> selectedAnnotations = new List<BioImage.Annotation>();
        public static BioImage selectedImage = null;

        public static ImageView viewer = null;
        public static ImageViewer app = null;

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
                foreach (BioImage.Annotation an in selectedAnnotations)
                {
                    if (an.selectedPoints.Count > 0 && an.selectedPoints.Count < an.GetPointCount())
                    {
                        if (an.type == BioImage.Annotation.Type.Rectangle || an.type == BioImage.Annotation.Type.Ellipse)
                        {
                            BioImage.RectangleD d = an.Rect;
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
                            BioImage.PointD pod = new BioImage.PointD(p.X - pd.X, p.Y - pd.Y);
                            for (int i = 0; i < an.selectedPoints.Count; i++)
                            {
                                BioImage.PointD poid = an.GetPoint(an.selectedPoints[i]);
                                an.UpdatePoint(new BioImage.PointD(poid.X + pod.X, poid.Y + pod.Y), an.selectedPoints[i]);
                            }
                        }
                    }
                    else
                    {
                        BioImage.PointD pod = new BioImage.PointD(p.X - pd.X, p.Y - pd.Y);
                        for (int i = 0; i < an.GetPointCount(); i++)
                        {
                            BioImage.PointD poid = an.PointsD[i];
                            an.UpdatePoint(new BioImage.PointD(poid.X + pod.X, poid.Y + pod.Y), i);
                        }
                    }
                }
                UpdateOverlay();
            }
            mouseColor = "";

            if (e.Button == MouseButtons.Left)
            { 
                Point s = GetImageSize();
                if ((p.X < s.X && p.Y < s.Y) || (p.X >= 0 && p.Y > 0))
                {
                    if (Mode == ViewMode.RGBImage)
                    {
                        int sc = coordinate.S;
                        int zc = coordinate.Z;
                        int cc = coordinate.C;
                        int tc = coordinate.T;
                        if (image.RGBChannelCount == 1)
                        {
                            int r = image.GetValue(sc, zc, RChannel.Index, tc, (int)p.X, (int)p.Y);
                            int g = image.GetValue(sc, zc, GChannel.Index, tc, (int)p.X, (int)p.Y);
                            int b = image.GetValue(sc, zc, BChannel.Index, tc, (int)p.X, (int)p.Y);
                            mouseColor = ", " + r + ", " + g + ", " + b;
                        }
                        else
                        {
                            int r = image.GetValueRGB(sc, zc, RChannel.Index, tc, (int)p.X, (int)p.Y, 0);
                            int g = image.GetValueRGB(sc, zc, GChannel.Index, tc, (int)p.X, (int)p.Y, 1);
                            int b = image.GetValueRGB(sc, zc, BChannel.Index, tc, (int)p.X, (int)p.Y, 2);
                            mouseColor = ", " + r + ", " + g + ", " + b;
                        }
                    }
                    else
                    if (Mode == ViewMode.Filtered)
                    {
                        int r = Buf.GetValue((int)p.X, (int)p.Y);
                        mouseColor = ", " + r.ToString();
                    }
                    else
                    if (Mode == ViewMode.Raw)
                    {
                        int r = Buf.GetValue((int)p.X, (int)p.Y);
                        mouseColor = ", " + r.ToString();
                    }
                }
            }

            if (Tools.currentTool != null)
            if(Tools.currentTool.type == Tools.Tool.Type.pencil && e.Button == MouseButtons.Left)
            if (Mode == ViewMode.RGBImage)
            {
                if (Buf.info.RGBChannelsCount > 1)
                {
                    Tools.Tool tool = Tools.currentTool;
                    if (Tools.rEnabled)
                        Buf.SetValueRGB((int)p.X, (int)p.Y, 0, tool.Color.R);
                    if (Tools.gEnabled)
                        Buf.SetValueRGB((int)p.X, (int)p.Y, 1, tool.Color.G);
                    if (Tools.bEnabled)
                        Buf.SetValueRGB((int)p.X, (int)p.Y, 2, tool.Color.B);
                }
                else
                if (Mode == ViewMode.RGBImage)
                {
                    if (Tools.rEnabled)
                        image.SetValue((int)p.X, (int)p.Y, RChannel.Index, Tools.pencil.Color.R);
                    if (Tools.gEnabled)
                        image.SetValue((int)p.X, (int)p.Y, GChannel.Index, Tools.pencil.Color.G);
                    if (Tools.bEnabled)
                        image.SetValue((int)p.X, (int)p.Y, BChannel.Index, Tools.pencil.Color.B);
                }
                else
                {
                    image.SetValue((int)p.X, (int)p.Y, GetCoordinate(), Tools.pencil.Color.R);
                }
                UpdateView();
            }
            else
            if (Mode == ViewMode.Filtered)
            {
                if (image.RGBChannelCount > 1)
                {
                    Buf.SetValueRGB((int)p.X, (int)p.Y, cBar.Value, Tools.pencil.Color.R);
                }
                else
                {
                    image.SetValue((int)p.X, (int)p.Y, GetCoordinate(), Tools.pencil.Color.R);
                }
                UpdateView();
            }
            else
            if (Mode == ViewMode.Raw)
            {
                if (image.RGBChannelCount > 1)
                {
                    Buf.SetValueRGB((int)p.X, (int)p.Y, cBar.Value, ushort.MaxValue);
                }
                else
                {
                    image.SetValue((int)p.X, (int)p.Y, GetCoordinate(), ushort.MaxValue);
                }
                UpdateView();
            }
            tools.ToolMove(p, mouseDownButtons);
            UpdateStatus();
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
            if (Tools.currentTool.type == Tools.Tool.Type.pan)
            {
                PointF pf = new PointF(mouseUp.X - mouseDown.X, mouseUp.Y - mouseDown.Y);
                origin.X += pf.X;
                origin.Y += pf.Y;
                UpdateOverlay();
                UpdateView();
            }
            tools.ToolUp(p, e.Button);
        }
        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            tools.BringToFront();
            if (!Ctrl)
            {
                foreach (BioImage.Annotation item in selectedAnnotations)
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
                foreach (BioImage.Annotation an in AnnotationsRGB)
                {
                    if (an.GetSelectBound().IntersectsWith(p.X,p.Y))
                    {
                        selectedAnnotations.Add(an);
                        an.selected = true;
                        
                        RectangleF r = new RectangleF(p.X, p.Y, 2, 2);
                        for (int i = 0; i < an.selectBoxs.Count; i++)
                        {
                            if (an.selectBoxs[i].IntersectsWith(r))
                            {
                                an.selectedPoints.Add(i);
                            }
                        }
                    }
                    else
                        if(!Ctrl)
                        an.selected = false;
                }
                UpdateOverlay();
            }

            if (e.Button == MouseButtons.Left)
            {
                Point s = GetImageSize();
                if ((p.X < s.X && p.Y < s.Y) || (p.X >= 0 && p.Y >= 0))
                {
                    if (Mode == ViewMode.RGBImage)
                    {
                        int sc = coordinate.S;
                        int zc = coordinate.Z;
                        int cc = coordinate.C;
                        int tc = coordinate.T;
                        if (image.RGBChannelCount == 1)
                        {
                            int r = image.GetValue(sc, zc, RChannel.Index, tc, (int)p.X, (int)p.Y);
                            int g = image.GetValue(sc, zc, GChannel.Index, tc, (int)p.X, (int)p.Y);
                            int b = image.GetValue(sc, zc, BChannel.Index, tc, (int)p.X, (int)p.Y);
                            mouseColor = ", " + r + "," + g + "," + b;
                        }
                        else
                        {
                            int r = image.GetValueRGB(sc, zc, RChannel.Index, tc, (int)p.X, (int)p.Y, 0);
                            int g = image.GetValueRGB(sc, zc, GChannel.Index, tc, (int)p.X, (int)p.Y, 1);
                            int b = image.GetValueRGB(sc, zc, BChannel.Index, tc, (int)p.X, (int)p.Y, 2);
                            mouseColor = ", " + r + "," + g + "," + b;
                        }
                    }
                    else
                    if (Mode == ViewMode.Filtered)
                    {
                        int r = Buf.GetValue((int)p.X, (int)p.Y);
                        mouseColor = ", " + r.ToString();
                    }
                    else
                    if (Mode == ViewMode.Raw)
                    {
                        int r = Buf.GetValue((int)p.X, (int)p.Y);
                        mouseColor = ", " + r.ToString();
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
            foreach (BioImage.Annotation item in AnnotationsRGB)
            {
                if (item.selected && (item.selectedPoints.Count == 0 || item.selectedPoints.Count == item.GetPointCount()))
                {
                    image.Annotations.Remove(item);
                }
                else
                {
                    if ((item.type == BioImage.Annotation.Type.Polygon || item.type == BioImage.Annotation.Type.Freeform || item.type == BioImage.Annotation.Type.Polyline) && item.selectedPoints.Count > 0)
                    {
                        item.RemovePoints(item.selectedPoints.ToArray());
                        
                    }
                    item.selectedPoints.Clear();
                }
            }
            UpdateOverlay();
        }
    }
}
