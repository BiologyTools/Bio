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
        public BioImage image;
        private string filePath = "";
        public ImageView(string path)
        {
            InitializeComponent();
            if (path == "" || path == null)
                return;
            LoadImage(path);
            image.AutoThresholdImage();
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
            if (image.SizeC == 1 && !image.isRGB)
            {
                Mode = ViewMode.Plane;
            }
            else
                Mode = ViewMode.RGBImage;
            UpdateView();
        }
        ~ImageView()
        {
            image.Dispose();
        }

        public int minSizeX = 60;
        public int minSizeY = 20;
        public enum ViewMode
        {
            Plane,
            RGBImage
        }
        private ViewMode viewMode = ViewMode.RGBImage;
        public ViewMode Mode
        {
            get
            {
                return viewMode;
            }
            set
            {
                if(value == ViewMode.Plane)
                {
                    rgbBoxsPanel.SendToBack();
                    cPanel.BringToFront();
                    cPanel.Show();
                    cPanel.Visible = true;
                    cBar.Visible = true;
                    rgbBoxsPanel.Hide();
                    if(!timeEnabled)
                    {
                        cPanel.Location = new Point(0, timeBar.Location.Y);
                    }
                }
                if (value == ViewMode.RGBImage)
                {
                    cPanel.Hide();
                    cPanel.Visible = false;
                    cBar.Enabled = true;
                    cBar.Visible = false;
                    cPanel.SendToBack();
                    rgbBoxsPanel.BringToFront();
                    rgbBoxsPanel.Show();
                    rgbBoxsPanel.Visible = true;
                    labelRGB.Visible = true;
                    if (!timeEnabled)
                    {
                        rgbBoxsPanel.Location = new Point(0, timeBar.Location.Y);
                    }
                }
                viewMode = value;
            }
        }
        public string Path
        {
            get
            {
                return filePath;
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
            image = new BioImage(filePath);
            zBar.Maximum = image.SizeZ-1;
            cBar.Maximum = image.SizeC-1;
            if (image.SizeT > 1)
            {
                timeBar.Maximum = image.SizeT - 1;
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
            return true;
        }

        public void UpdateView()
        {
            if (Mode == ViewMode.RGBImage)
            {
                image.UpdateBitmap(zBar.Value, cBar.Value, timeBar.Value);
                rgbPictureBox.Image = image.rgbBitmap;
            }
            else
            if (Mode == ViewMode.Plane)
            {

                image.UpdatePlane(zBar.Value, cBar.Value, timeBar.Value);
                rgbPictureBox.Image = image.planeBitmap;

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

            float ms = image.frameTicks / 1000;
            statusLabel.Text += " FrameTime:" + ms + " ms";

        }

        private void channelBoxR_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxR.SelectedIndex == -1)
                return;
            BioImage.Channel ch = (BioImage.Channel)channelBoxR.SelectedItem;
            image.SetRGBChannelIndex(BioImage.RGB.Red, ch.Index);
            //image.rgbimage.RChannel = ch;
            UpdateView();
        }

        private void channelBoxG_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxG.SelectedIndex == -1)
                return;
            BioImage.Channel ch = (BioImage.Channel)channelBoxG.SelectedItem;
            image.SetRGBChannelIndex(BioImage.RGB.Green, ch.Index);
            //image.rgbimage.GChannel = ch;
            UpdateView();
        }

        private void channelBoxB_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (channelBoxB.SelectedIndex == -1)
                return;
            BioImage.Channel ch = (BioImage.Channel)channelBoxB.SelectedItem;
            image.SetRGBChannelIndex(BioImage.RGB.Blue, ch.Index);
            //image.rgbimage.BChannel = ch;
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
                if (image.RGBChannelsCount == 1)
                {
                    int r = image.RPlane.GetValue(p.X, p.Y);
                    int g = image.GPlane.GetValue(p.X, p.Y);
                    int b = image.BPlane.GetValue(p.X, p.Y);
                    mouseColor = "(" + p.X + "," + p.Y + "), " + r + "," + g + "," + b;
                }
                else
                {
                    int r = image.plane.GetValue(p.X, p.Y, 0);
                    int g = image.plane.GetValue(p.X, p.Y, 1);
                    int b = image.plane.GetValue(p.X, p.Y, 2);
                    mouseColor = "(" + p.X + "," + p.Y + "), " + r + "," + g + "," + b;
                }
                UpdateStatus();
            }
            else
            if(Mode == ViewMode.Plane)
            {
                int r = image.plane.GetValue(p.X, p.Y);
                mouseColor = "(" + p.X + "," + p.Y + "), " + r;
                if (e.Button == MouseButtons.Left)
                {
                    if (image.plane.RGBChannelsCount > 0)
                    {
                        image.plane.SetValue(p.X, p.Y, cBar.Value,ushort.MaxValue); 
                    }
                    else
                    {
                        image.plane.SetValue(p.X, p.Y, ushort.MaxValue);
                    }
                    UpdateView();
                }  
            }
   
        }

        private void autoContrastChannelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.AutoThresholdImage();
            UpdateView();
        }

        private void playSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = null;
            if(Mode == ViewMode.RGBImage)
             sp = new PlaySpeed(timeEnabled, false, ZFps, TimeFps, CFps);
            if (Mode == ViewMode.Plane)
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
            if (Mode == ViewMode.Plane)
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
            if (Mode == ViewMode.Plane)
                sp = new PlaySpeed(timeEnabled, true, ZFps, TimeFps, CFps);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            ZFps = sp.ZFps;
            TimeFps = sp.TimeFps;
            CFps = sp.CFps;
        }
        private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Mode == ViewMode.RGBImage)
                Clipboard.SetImage(image.rgbBitmap);
            if (Mode != ViewMode.Plane)
                Clipboard.SetImage(image.planeBitmap);
        }

        private void ImageView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.C && e.Control)
            {
                if(Mode == ViewMode.RGBImage)   
                    Clipboard.SetImage(image.rgbBitmap);
                if(Mode != ViewMode.Plane)
                    Clipboard.SetImage(image.planeBitmap);
            }
        }

        public void GetRange()
        {
            RangeTool t;
            if (Mode == ViewMode.Plane)
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

        private void planeModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(planeModeToolStripMenuItem.Checked)
            {
                Mode = ViewMode.Plane;
            }
            else
            {
                Mode = ViewMode.RGBImage;
            }
            UpdateView();
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
            if (Mode != ViewMode.Plane)
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
            if (playZToolStripMenuItem.Checked)
            {
                if (zBar.Maximum >= zBar.Value + 1)
                    zBar.Value++;
                else
                    zBar.Value = zBar.Minimum;
            }
        }
        private void zTimer_Tick(object sender, EventArgs e)
        {
            if(playZToolStripMenuItem.Checked)
            {
                if (zBar.Maximum >= zBar.Value + 1)
                    zBar.Value++;
                else
                    zBar.Value = zBar.Minimum;
            }
        }
        private void timer_Tick(object sender, EventArgs e)
        {           
            if (playTimeToolStripMenu.Checked)
            {
                if (timeBar.Maximum >= timeBar.Value + 1)
                    timeBar.Value++;
                else
                    timeBar.Value = timeBar.Minimum;
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

        
    }
}
