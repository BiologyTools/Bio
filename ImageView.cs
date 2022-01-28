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
            LoadImage(path);
            //if (path.EndsWith(".tif") || path.EndsWith(".tiff"))
            image.AutoThreshold();
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(ImageView_MouseWheel);
            zBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(ZTrackBar_MouseWheel);
            //We set the trackbar event to handled so that it only scrolls one tick not the default multiple.
            zBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            timeBar.MouseWheel += new System.Windows.Forms.MouseEventHandler(TimeTrackBar_MouseWheel);
            //We set the trackbar event to handled so that it only scrolls one tick not the default multiple.
            timeBar.MouseWheel += (sender, e) => ((HandledMouseEventArgs)e).Handled = true;
            UpdateView();
        }
        public string Path
        {
            get
            {
                return filePath;
            }
            set
            {
                filePath = value;
                image.FromFile(filePath);
                rgbPictureBox.Image = image.plane.GetBitmap();
                timeBar.Maximum = image.SizeZ - 1;
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

        public bool LoadImage(string path)
        {
            filePath = path;
            statusLabel.Text = timeBar.Value.ToString();
            image = new BioImage(filePath);
            zBar.Maximum = image.SizeZ-1;
            zBar.Value = 0;
            if (image.SizeT > 1)
            {
                timeBar.Maximum = image.SizeT - 1;
                TimeEnabled = true;
            }
            else
            {
                TimeEnabled = false;
            }
            rgbPictureBox.Image = image.plane.GetBitmap();
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
            if (timeEnabled)
            {
                statusLabel.Text = zBar.Value + "/" + zBar.Maximum + ", " + timeBar.Value + "/" + timeBar.Maximum + ", " + mouseColor ;
            }
            else
            {
                statusLabel.Text = zBar.Value + "/" + zBar.Maximum + ", " + mouseColor;
            }
            //image.UpdatePlane(zBar.Value,0,timeBar.Value);
            //rgbPictureBox.Image = image.plane.GetBitmap();
            image.UpdateImage(zBar.Value, timeBar.Value);
            rgbPictureBox.Image = image.bitmap;
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

        private void channelTrackBar_Scroll(object sender, EventArgs e)
        {

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
            if (splitContainer.Height <= 0)
                return;
            if (timeEnabled)
                splitContainer.SplitterDistance = splitContainer.Height - 90;
            else
                splitContainer.SplitterDistance = splitContainer.Height - 70;
        }
        
        private void ImageView_Resize(object sender, EventArgs e)
        {
            if (splitContainer.Height <= 0)
                return;
            if(timeEnabled)
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
        Stopwatch st = new Stopwatch();
        private void zBar_ValueChanged(object sender, EventArgs e)
        {
            st.Restart();
            UpdateView();
            st.Stop();
        }

        private void timeBar_ValueChanged(object sender, EventArgs e)
        {
            UpdateView();
        }

        public string mouseColor = "";

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
                //Drawn picture width will be the same as height in Zoom mode.
                int w = rgbPictureBox.Height;
                int x0 = (rgbPictureBox.Width - rgbPictureBox.Height) / 2;
                int xp = p.X - x0;
                int yp = p.Y;
                x = (int)((((float)xp) / (float)w) * (float)image.SizeX);
                y = (int)((((float)yp) / (float)rgbPictureBox.Height) * (float)image.SizeY);
                return new Point(x, y);
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

        private void rgbPictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            Point p = toImagePoint(e.Location.X, e.Location.Y);
            int r = image.RPlane.GetValue(p.X, p.Y);
            int g = image.GPlane.GetValue(p.X, p.Y);
            int b = image.BPlane.GetValue(p.X, p.Y);
            mouseColor = "(" + p.X + "," + p.Y + "), " + r + "," + g + "," + b;
            //this.color = pxcolor;
            UpdateView();
        }

        private void autoContrastChannelsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            image.AutoThreshold();
            UpdateView();
        }

        private void playSpeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = new PlaySpeed(timeEnabled,zTimer.Interval, timelineTimer.Interval);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            zTimer.Interval = sp.ZPlayspeed;
            timelineTimer.Interval = sp.TimePlayspeed;
        }
        private void playSpeedToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            PlaySpeed sp = new PlaySpeed(timeEnabled,zTimer.Interval, timelineTimer.Interval);
            if (sp.ShowDialog() != DialogResult.OK)
                return;
            zTimer.Interval = sp.ZPlayspeed;
            timelineTimer.Interval = sp.TimePlayspeed;
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

        private void copyImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Clipboard.SetImage(image.plane.GetBitmap());
        }

        private void ImageView_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.C && e.Control)
                Clipboard.SetImage(image.plane.GetBitmap());
        }

        private void setValueRangeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RangeTool t = new RangeTool(timeEnabled,zBar.Minimum, zBar.Maximum, timeBar.Minimum, timeBar.Maximum);
            if (t.ShowDialog() != DialogResult.OK)
                return;
            zBar.Minimum = t.ZMin;
            zBar.Maximum = t.ZMax;
            timeBar.Minimum = t.TimeMin;
            timeBar.Maximum = t.TimeMax;
        }

        private void setValueRangeToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RangeTool t = new RangeTool(timeEnabled,zBar.Minimum, zBar.Maximum, timeBar.Minimum, timeBar.Maximum);
            if (t.ShowDialog() != DialogResult.OK)
                return;
            zBar.Minimum = t.ZMin;
            zBar.Maximum = t.ZMax;
            timeBar.Minimum = t.TimeMin;
            timeBar.Maximum = t.TimeMax;
        }

        private void zPlayMenuStrip_Opening(object sender, CancelEventArgs e)
        {

        }

        private void rgbPictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            /*
            Graphics gr = image.CreateGraphics();
            Point p = toImagePoint(e.X, e.Y);
            gr.FillEllipse(Brushes.Orange, new Rectangle(p.X, p.Y,20, 20));
            gr.Save();
            image.FinalizeGraphics();
            */
        }

        private void rgbPictureBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.S && e.Control)
            {
                if (saveFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                image.Save(saveFileDialog.FileName);
            }
        }

        private void backgroundWorker_DoWork(object sender, DoWorkEventArgs e)
        {

        }
    }
}
