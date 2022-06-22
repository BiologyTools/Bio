using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BioImage
{
    public partial class HistogramControl : UserControl
    {
        public HistogramControl(Statistics s)
        {
            stats = s;
            this.Dock = DockStyle.Fill;
            InitializeComponent();
            if (s.BitsPerPixel == 8)
                graphMax = 255;
            Bin = 10;
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
                pen = new Pen(Brushes.Black, value);
            }
        }
        private float min = 0;
        public float Min
        {
            get { return min;}
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
        private Pen pen;
        private void HistogramControl_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightGray);
            e.Graphics.TranslateTransform(-graphMin, 0);
            //e.Graphics.ScaleTransform(scale.Width, scale.Height);
            if (stats == null)
                return;
            float fy = ((float)this.Height) / (float)stats.Median;
            float fx = ((float)this.Width) / ((float)graphMax);
            for (float x = 0; x < graphMax; x+=bin)
            {
                float val = (float)stats.MeanValues[(int)x];
                float y = this.Height - (fy * val);
                e.Graphics.DrawLine(pen, new PointF(fx * x, this.Height), new PointF(fx * x, y));
            }
            e.Graphics.DrawLine(Pens.Green, new PointF((fx * Max), 0), new PointF((fx * Max), this.Height));
            e.Graphics.DrawLine(Pens.Green, new PointF(fx * Min, 0), new PointF(fx * Min, this.Height));
            e.Graphics.DrawLine(Pens.Red, new PointF((fx * stats.Max), 0), new PointF((fx * stats.Max), this.Height));
            e.Graphics.DrawLine(Pens.Red, new PointF(fx * stats.Min, 0), new PointF(fx * stats.Min, this.Height));
        }
    }
}
