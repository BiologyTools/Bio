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

        Histogram hist = new Histogram();
        private void HistogramControl_Paint(object sender, PaintEventArgs e)
        {
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
