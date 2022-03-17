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
    public partial class Tools : Form
    {
        public static bool applyToStack = false;
        public static Pencil pencil;
        public class Tool : Control
        {
            public BioImage.ColorS Color;
            public List<Point> Points;
        }
        public class Pencil : Tool
        {
            public Pencil(BioImage.ColorS col)
            {
                Color = col;
            }
        }
        public class Brush : Tool
        {
            public int BrushWidth;
        }
        public class Eraser : Tool
        {
            public int EraserWidth;
        }
        public static Tool currentTool;
        public BioImage.ColorS color;
        public Font font;
        public ColorTool ctool;
        private BioImage image;
        public BioImage Image
        {
            get
            {
                return image;
            }
            set
            {
                image = value;
            }
        }
        public Tools()
        {
            InitializeComponent();
            pencil = new Pencil(new BioImage.ColorS(ushort.MaxValue));
        }
        
        private void pencilPanel_DoubleClick(object sender, EventArgs e)
        {
            if (ctool == null)
                ctool = new ColorTool();
            if (ctool.ShowDialog() != DialogResult.OK)
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
                for (int i = 0; i < image.imageCount; i++)
                {
                    b = image.Buffers[i].GetBitmap();
                    b.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    image.Buffers[i].SetBuffer(b);
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
                    b = Image.Buffers[i].GetBitmap();
                    b.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    Image.Buffers[i].SetBuffer(b);
                }
            }
            else
            {

            }
        }

        private void pencilPanel_Paint(object sender, PaintEventArgs e)
        {

        }
    }
}
