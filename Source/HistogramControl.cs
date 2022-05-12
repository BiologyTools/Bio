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
        public HistogramControl()
        {
            InitializeComponent();
        }

        private Histogram hist = null;
        public Histogram Histogram
        {
            get { return hist; }
            set { hist = value; }
        }
        private void HistogramControl_Paint(object sender, PaintEventArgs e)
        {
            if (hist == null)
                return;
            int max = 255;
            if (hist.Type == RGB.Gray)
                max = 65535;
            for (float y = 0; y < max; y++)
            {
                float h = (float)this.Height / (float)hist.Values[(int)y];
                float x = (float)this.Width / max;
                e.Graphics.DrawLine(new Pen(Color.Black), new PointF((int)x, 0), new PointF(x, h));
            }
        }
    }
}
