using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bio
{
    public partial class HistogramControl : UserControl
    {
        public HistogramControl(Statistics s)
        {
            stats = s;
            this.Dock = DockStyle.Fill;
            InitializeComponent();
            min = ImageView.viewer.image.Channels[0].Min;
            max = ImageView.viewer.image.Channels[0].Max;
            if (s.BitsPerPixel == 8)
            {
                graphMax = 255;
                Bin = 1;
            }
            else
            {
                graphMax = ushort.MaxValue;
                Bin = 10;
            }
            
        }
        private Statistics stats = null;
        public Statistics Statistics
        {
            get { return stats; }
            set { stats = value; }
        }
        private float bin = 10;
        public float Bin
        {
            get
            {
                return bin;
            }
            set
            {
                bin = value;
            }
        }
        private float min = 0;
        public float Min
        {
            get { return min; }
            set { min = value; }
        }
        private int max = ushort.MaxValue;
        public int Max
        {
            get { return max; }
            set { max = value; }
        }
        private int graphMax = ushort.MaxValue;
        public int GraphMax
        {
            get { return graphMax; }
            set { graphMax = value; }
        }
        private int graphMin = 0;
        public int GraphMin
        {
            get { return graphMin; }
            set { graphMin = value; }
        }
        private void HistogramControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightGray);
            e.Graphics.TranslateTransform(-graphMin, 0);
            //e.Graphics.ScaleTransform(scale.Width, scale.Height);
            if (stats == null)
                return;
            float fy = ((float)this.Height) / (float)stats.StackMedian;
            float fx = ((float)this.Width) / ((float)graphMax);
            Pen black = new Pen(Color.FromArgb(150,0,0,0), bin * fx);
            Pen blue = new Pen(Color.FromArgb(150,0, 0, 255), bin * fx);
            float sumbins = 0;
            float sumbin = 0;
            int binind = 0;
            int bininds = 0;

            for (float x = 0; x < graphMax; x++)
            {
                //Lets draw the stack histogram
                float val = (float)ImageView.viewer.image.Statistics.StackValues[(int)x];
                sumbin += val;
                if (binind == bin)
                {
                    float v = sumbin / binind;
                    float yy = this.Height - (fy * v);
                    e.Graphics.DrawLine(black, new PointF(fx * x, this.Height), new PointF(fx * x, yy));
                    binind = 0;
                    sumbin = 0;
                }

                //Lets draw the channel histogram on top of the stack histogram.
                float vals = (float)stats.StackValues[(int)x];
                sumbins += vals;
                if (bininds == bin)
                {
                    float v = sumbins / bininds;
                    float yy = this.Height - (fy * v);
                    e.Graphics.DrawLine(blue, new PointF(fx * x, this.Height), new PointF(fx * x, yy));
                    bininds = 0;
                    sumbins = 0;
                }
                binind++;
                bininds++;
            }

            e.Graphics.DrawLine(Pens.Green, new PointF((fx * Max), 0), new PointF((fx * Max), this.Height));
            e.Graphics.DrawLine(Pens.Green, new PointF(fx * Min, 0), new PointF(fx * Min, this.Height));
            e.Graphics.DrawLine(Pens.Red, new PointF((fx * stats.StackMax), 0), new PointF((fx * stats.StackMax), this.Height));
            e.Graphics.DrawLine(Pens.Red, new PointF(fx * stats.StackMin, 0), new PointF(fx * stats.StackMin, this.Height));
            blue.Dispose();
            black.Dispose();
        }
    }
}
