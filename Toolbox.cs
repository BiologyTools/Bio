using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using BioImage;

namespace BioImage
{
    public partial class Toolbox : Form
    {
        public bool applyToStack = false;
        public Pencil pencil;
        private BioImage image;

        public BioImage Image
        {
            get { return image; }
            set { image = value; }
        }
        public class Tool
        {
            public BioImage.ColorS Color;
            public List<Point> Points;
        }
        public class Pencil : Tool
        {
            
        }
        public class Brush : Tool
        {
            public int Width;
        }
        public class Eraser : Tool
        {
            public int Width;
        }


        public static Tool currentTool;
        public BioImage.ColorS color;
        public BioImage.ColorS colorg;
        public Font font;
        public Toolbox()
        {
            InitializeComponent();
        }

        private void pencilPanel_DoubleClick(object sender, EventArgs e)
        {
            if (colorDialog.ShowDialog() != DialogResult.OK)
                return;
            Color col = colorDialog.Color;
            color = BioImage.ColorS.FromColor(col);
        }

        private void textPanel_DoubleClick(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;
            font = fontDialog.Font;
        }

        private void pencilPanel_Click(object sender, EventArgs e)
        {
            currentTool = pencil;
        }

        private void stackBox_CheckedChanged(object sender, EventArgs e)
        {
            applyToStack = stackApplyBox.Checked;
        }

        private void flipXPanel_Paint(object sender, PaintEventArgs e)
        {
            Bitmap b;
            if(applyToStack)
            {
                for (int i = 0; i < Image.imageCount; i++)
                {
                    b = Image.Planes[i].GetBitmap();
                    b.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    Image.Planes[i].SetRawBytes(b);
                }
            }
            else
            {

            }
        }

        private void flipYPanel_Paint(object sender, PaintEventArgs e)
        {
            Bitmap b;
            if (applyToStack)
            {
                for (int i = 0; i < Image.imageCount; i++)
                {
                    b = Image.Planes[i].GetBitmap();
                    b.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    Image.Planes[i].SetRawBytes(b);
                }
            }
            else
            {

            }
        }
    }
}
