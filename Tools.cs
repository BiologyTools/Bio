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
        public static ColorTool colorTool;
        public static bool rEnabled = true;
        public static bool gEnabled = true;
        public static bool bEnabled = true;
        public static Pencil pencil;
        public class Tool : Control
        {
            public enum ToolType
            {
                color,
                annotation,
                select,
                function
            }
            public BioImage.ColorS Color;
            public List<Point> Points;
            public ToolType type;
        }
        public class Pencil : Tool
        {
            public Pencil(BioImage.ColorS col)
            {
                Color = col;
                type = ToolType.color;
            }
        }
        public class Brush : Tool
        {
            public int BrushWidth;
            public Brush(BioImage.ColorS col,int BrushWidth)
            {
                this.BrushWidth = BrushWidth;
                this.Color = col;
            }
        }
        public class Bucket : Tool
        {
            
        }
        public class Eraser : Tool
        {
            public int EraserWidth;
        }

        public static Tool currentTool;
        public Font font;
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
            currentTool = pencil;
            colorTool = new ColorTool();
        }
        
        public static void SetColor(BioImage.ColorS col)
        {
            currentTool.Color = col;
        }

        private void pencilPanel_DoubleClick(object sender, EventArgs e)
        {
            currentTool = pencil;
            colorTool.Show();
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

        private void pencilPanel_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            colorTool.Show();
        }
    }
}
