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
        public static PencilTool pencil;
        public static MoveTool move;
        public static BrushTool brush;
        public static BucketTool bucket;
        public static EraserTool eraser;
        public static PointTool point;
        public static LineTool line;
        public static RectTool rect;
        public static EllipseTool ellipse;
        public static PolygonTool polygon;
        public static TextTool text;

        public class Tool : Control
        {
            public enum ToolType
            {
                color,
                annotation,
                select,
                function
            }
            public enum Type
            {
                pencil,
                brush,
                bucket,
                eraser,
                move,
                point,
                line,
                rect,
                ellipse,
                polyline,
                polygon,
                text,
            }

            public BioImage.ColorS Color;
            public List<Point> Points;
            public ToolType toolType;
            public Type type;
        }
        public class PencilTool : Tool
        {
            public PencilTool(BioImage.ColorS col)
            {
                Color = col;
                toolType = ToolType.color;
                type = Type.pencil;
            }
        }
        public class BrushTool : Tool
        {
            public int BrushWidth;
            public BrushTool(BioImage.ColorS col,int BrushWidth)
            {
                this.BrushWidth = BrushWidth;
                this.Color = col;
                type = Type.brush;
            }
        }
        public class BucketTool : Tool
        {
            public BucketTool(BioImage.ColorS col)
            {
                this.Color = col;
                type = Type.bucket;
            }
        }
        public class EraserTool : Tool
        {
            public int EraserWidth;
            public EraserTool(BioImage.ColorS col, int EraserWidth)
            {
                this.EraserWidth = EraserWidth;
                this.Color = col;
                type = Type.brush;
            }
        }
        public class MoveTool : Tool
        {
            public MoveTool()
            {
                toolType = ToolType.annotation;
                type = Type.move;
            }
        }
        public class PointTool : Tool
        {
            public PointTool()
            {
                toolType = ToolType.annotation;
                type = Type.point;
            }
        }
        public class LineTool : Tool
        {
            public LineTool()
            {
                toolType = ToolType.annotation;
                type = Type.line;
            }
        }
        public class RectTool : Tool
        {
            public RectTool()
            {
                toolType = ToolType.annotation;
                type = Type.rect;
            }
        }
        public class EllipseTool : Tool
        {
            public EllipseTool()
            {
                toolType = ToolType.annotation;
                type = Type.ellipse;
            }
        }
        public class PolygonTool : Tool
        {
            public PolygonTool()
            {
                toolType = ToolType.annotation;
                type = Type.polygon;
            }
        }
        public class TextTool : Tool
        {
            public TextTool()
            {
                toolType = ToolType.annotation;
                type = Type.text;
            }
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
            BioImage.ColorS col = new BioImage.ColorS(ushort.MaxValue);
            pencil = new PencilTool(col);
            move = new MoveTool();
            currentTool = move;
            colorTool = new ColorTool();
            brush = new BrushTool(col,5);
            bucket = new BucketTool(col);
            eraser = new EraserTool(col, 5);
            move = new MoveTool();
            point = new PointTool();
            line = new LineTool();
            rect = new RectTool();
            ellipse = new EllipseTool();
            polygon = new PolygonTool();
            text = new TextTool();
        }

        public void UpdateView()
        {
            ImageView.viewer.UpdateView();
        }
        public void UpdateSelected()
        {
            foreach (Control item in this.Controls)
            {
                item.BackColor = Color.White;
            }
        }

        BioImage.Annotation anno = new BioImage.Annotation();
        public void ToolDown(object sender, MouseEventArgs e)
        {
            if (currentTool == null)
                return;
            if (currentTool.type == Tool.Type.point)
            {
                anno.type = BioImage.Annotation.Type.Point;
                anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                anno.coord = ImageView.Coordinate;
                if (anno.GetPointCount() == 2 && currentTool.type == Tool.Type.line)
                {
                    anno.type = BioImage.Annotation.Type.Line;
                    ImageView.selectedImage.Annotations.Add(anno);
                    anno = new BioImage.Annotation();
                }
            }
            if (currentTool.type == Tool.Type.line)
            {
                anno.type = BioImage.Annotation.Type.Line;
                anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                anno.coord = ImageView.Coordinate;
                if (anno.GetPointCount() == 2 && currentTool.type == Tool.Type.line)
                {
                    anno.type = BioImage.Annotation.Type.Line;
                    ImageView.selectedImage.Annotations.Add(anno);
                    anno = new BioImage.Annotation();
                }
            }
            else
            if (currentTool.type == Tool.Type.polygon)
            {
                anno.type = BioImage.Annotation.Type.Polygon;
                anno.AddPoint(new BioImage.PointD(e.X,e.Y));
                anno.coord = ImageView.Coordinate;
                if (anno.GetPointCount() == 1)
                {
                    ImageView.selectedImage.Annotations.Add(anno);
                }
                else
                {
                    //If we click on a point 1 we close this polygon
                    BioImage.RectangleD d = new BioImage.RectangleD(e.X, e.Y, anno.selectBoxSize, anno.selectBoxSize);
                    if (d.IntersectsWith(anno.Point))
                    {
                        anno.closed = true;
                        anno = new BioImage.Annotation();
                    }
                }
            }
            else
            if (currentTool.type == Tool.Type.rect)
            {
                anno.type = BioImage.Annotation.Type.Rectangle;
                anno.Rect = new BioImage.RectangleD(e.X, e.Y, 1, 1);
                anno.coord = ImageView.Coordinate;
            }
            else
            if (currentTool.type == Tool.Type.ellipse)
            {
                anno.type = BioImage.Annotation.Type.Ellipse;
                anno.Rect = new BioImage.RectangleD(e.X, e.Y, 1, 1);
                anno.coord = ImageView.Coordinate;
            }
            UpdateView();
        }
        
        public void ToolUp(object sender, MouseEventArgs e)
        {
            if (currentTool == null)
                return;
            if (currentTool.type == Tool.Type.point)    
            {
                BioImage.Annotation an = new BioImage.Annotation();
                an.Point = new BioImage.PointD(e.X, e.Y);
                an.type = BioImage.Annotation.Type.Point;
                an.coord = ImageView.Coordinate;
                ImageView.selectedImage.Annotations.Add(an);
            }
            else
            if (currentTool.type == Tool.Type.line && anno.type == BioImage.Annotation.Type.Line)
            {
                anno.AddPoint(new BioImage.PointD(e.X, e.Y));
                ImageView.selectedImage.Annotations.Add(anno);
                anno = new BioImage.Annotation();
            }
            else
            if (currentTool.type == Tool.Type.rect && anno.type == BioImage.Annotation.Type.Rectangle)
            {
                anno.Rect = new BioImage.RectangleD(anno.X, anno.Y, e.X - anno.X, e.Y - anno.Y);
                ImageView.selectedImage.Annotations.Add(anno);
                anno = new BioImage.Annotation();
            }
            else
            if (currentTool.type == Tool.Type.ellipse && anno.type == BioImage.Annotation.Type.Ellipse)
            {
                anno.Rect = new BioImage.RectangleD(anno.X, anno.Y, e.X - anno.X, e.Y - anno.Y);
                ImageView.selectedImage.Annotations.Add(anno);
                anno = new BioImage.Annotation();
            }
            UpdateView();
        }
        public void ToolDoubleClick(object sender, MouseEventArgs e)
        {
            if (currentTool == null)
                return;
            
        }

        public void ToolMove(object sender, MouseEventArgs e)
        {
            
        }


        public static void SetColor(BioImage.ColorS col)
        {
            currentTool.Color = col;
        }

        private void pencilPanel_DoubleClick(object sender, EventArgs e)
        {
            colorTool.Show();
            currentTool = pencil;
            UpdateSelected();
            pencilPanel.BackColor = Color.DarkGray;
        }

        private void pencilPanel_Click(object sender, EventArgs e)
        {
            currentTool = pencil;
            UpdateSelected();
            pencilPanel.BackColor = Color.LightGray;
        }

        private void stackBox_CheckedChanged(object sender, EventArgs e)
        {
            applyToStack = stackApplyBox.Checked;
        }

        private void movePanel_Click(object sender, EventArgs e)
        {
            currentTool = move;
            UpdateSelected();
            movePanel.BackColor = Color.LightGray;
        }

        private void textPanel_Click(object sender, EventArgs e)
        {
            currentTool = text;
            UpdateSelected();
            textPanel.BackColor = Color.LightGray;
        }

        private void textPanel_DoubleClick(object sender, EventArgs e)
        {
            currentTool = text;
            UpdateSelected();
            textPanel.BackColor = Color.LightGray;

            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;
            font = fontDialog.Font;
        }
        private void pointPanel_Click(object sender, EventArgs e)
        {
            currentTool = point;
            UpdateSelected();
            pointPanel.BackColor = Color.LightGray;
        }
        private void linePanel_Click(object sender, EventArgs e)
        {
            currentTool = line;
            UpdateSelected();
            linePanel.BackColor = Color.LightGray;
            
        }
        private void rectPanel_Click(object sender, EventArgs e)
        {
            currentTool = rect;
            UpdateSelected();
            rectPanel.BackColor = Color.LightGray;
        }
        private void ellipsePanel_Click(object sender, EventArgs e)
        {
            currentTool = ellipse;
            UpdateSelected();
            ellipsePanel.BackColor = Color.LightGray;
        }

        private void polyPanel_Click(object sender, EventArgs e)
        {
            currentTool = polygon;
            UpdateSelected();
            polyPanel.BackColor = Color.LightGray;
        }
    }
}
