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
    public partial class ColorTool : Form
    {
        private BioImage.ColorS color = new BioImage.ColorS();
        public BioImage.ColorS Color
        {
            get
            {
                return color;
            }
            set
            {
                color = value;
            }
        }

        public ColorTool()
        {
            InitializeComponent();
            DialogResult = DialogResult.None;
        }
        public ColorTool(BioImage.ColorS col)
        {
            InitializeComponent();
            color = col;
            DialogResult = DialogResult.OK;
        }

        private void redBox_ValueChanged(object sender, EventArgs e)
        {
            color.R = (ushort)redBox.Value;
            colorPanel.BackColor = BioImage.ColorS.ToColor(color);
        }

        private void greenBox_ValueChanged(object sender, EventArgs e)
        {
            color.G = (ushort)greenBox.Value;
            colorPanel.BackColor = BioImage.ColorS.ToColor(color);
        }

        private void blueBox_ValueChanged(object sender, EventArgs e)
        {
            color.B = (ushort)blueBox.Value;
            colorPanel.BackColor = BioImage.ColorS.ToColor(color);
        }

        private void colorPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
