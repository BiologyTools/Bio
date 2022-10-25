using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace Bio.Graphics
{
    public struct Pen : IDisposable
    {
        public ColorS color;
        public int width;
        public Pen(ColorS col, int w)
        {
            color = col;
            width = w;
        }
        public void Dispose()
        {
            color.Dispose();
        }
    }
    public class Graphics : IDisposable
    {
        public BufferInfo buf;
        public static Graphics FromImage(BufferInfo b)
        {
            Graphics g = new Graphics();
            g.buf = b;
            return g;
        }
        public void DrawLine(PointF p, PointF p2, Pen pe)
        {
            float w = Math.Abs(p.X - p2.X);
            float h = Math.Abs(p.Y - p2.Y);
            float dx = (p.X - p2.X) * -1;
            float dy = (p.Y - p2.Y) * -1;
            buf.SetPixel((int)(p.X), (int)(p.Y), pe.color);
            for (int x = 1; x < w+1; x++)
            {
                for (int i = 0; i < pe.width; i++)
                {
                    buf.SetPixel((int)(p.X + (dx * (x / w)))+i, (int)(p.Y + (dy * (x / w)))+i, pe.color);
                }
            }
        }
        public void FillRectangle(Rectangle r, ColorS col)
        {
            for (int x = r.X; x < r.Width + r.X; x++)
            {
                for (int y = r.Y; y < r.Height + r.Y; y++)
                {
                    buf.SetPixel(x,y, col);
                }
            }
        }
        public void FillRectangle(RectangleF r, ColorS col)
        {
            FillRectangle(new Rectangle((int)Math.Ceiling(r.X), (int)Math.Ceiling(r.Y), (int)Math.Ceiling(r.Width), (int)Math.Ceiling(r.Height)), col);
        }
        public void DrawRectangle(Rectangle r, Pen pe)
        {
            for (int x = r.X; x < r.Width + r.X; x++)
            {
                for (int i = 0; i < pe.width; i++)
                {
                    buf.SetPixel(x + i, r.Y, pe.color);
                    buf.SetPixel(x + i, r.Y + r.Height, pe.color);
                }
            }
            for (int y = r.Y; y < r.Height + r.Y; y++)
            {
                for (int i = 0; i < pe.width; i++)
                {
                    buf.SetPixel(r.X, y + i, pe.color);
                    buf.SetPixel(r.X + r.Width, y + i, pe.color);
                }
            }
        }
        public void DrawRectangle(RectangleF r, Pen pe)
        {
            DrawRectangle(new Rectangle((int)Math.Ceiling(r.X), (int)Math.Ceiling(r.Y), (int)Math.Ceiling(r.Width), (int)Math.Ceiling(r.Height)), pe);
        }

        public void DrawEllipse(Rectangle r, Pen pe)
        {
            if (r.Width == 1 && r.Height == 1)
                buf.SetPixel(r.X, r.Y, pe.color);
            double radiusx = r.Width / 2;
            double radiusy = r.Height / 2;
            int x, y;
            for (double a = 0.0; a < 360.0; a += 0.1)
            {
                double angle = a * System.Math.PI / 180;
                for (int i = 0; i < pe.width; i++)
                {
                    x = (int)(radiusx * System.Math.Cos(angle) + radiusx + r.X);
                    y = (int)(radiusy * System.Math.Sin(angle) + radiusy + r.Y);
                    buf.SetPixel(x+i, y+i, pe.color);
                }
            }
        }
        public void DrawEllipse(RectangleF r, Pen pe)
        {
            DrawEllipse(new Rectangle((int)Math.Ceiling(r.X), (int)Math.Ceiling(r.Y), (int)Math.Ceiling(r.Width), (int)Math.Ceiling(r.Height)), pe);
        }
        public void FillEllipse(Rectangle r, ColorS c)
        {
            if(r.Width == 1 && r.Height == 1)
                buf.SetPixel(r.X, r.Y, c);
            double radiusx = r.Width / 2;
            double radiusy = r.Height / 2;
            int x, y;
            for (double a = 90; a < 270; a += 0.1)
            {
                double angle = a * System.Math.PI / 180;
                x = (int)(radiusx * System.Math.Cos(angle) + radiusx + r.X);
                y = (int)(radiusy * System.Math.Sin(angle) + radiusy + r.Y);
                double angle2 = (a+180) * System.Math.PI / 180;
                int x2 = (int)(radiusx * System.Math.Cos(angle2) + radiusx + r.X);
                DrawScanline(x,x2,y,c);
            }
        }
        public void FillEllipse(RectangleF r, ColorS c)
        {
            FillEllipse(new Rectangle((int)Math.Ceiling(r.X), (int)Math.Ceiling(r.Y), (int)Math.Ceiling(r.Width), (int)Math.Ceiling(r.Height)), c);
        }
        public void DrawScanline(int x, int x2, int line, ColorS col)
        {
            for (int xx = x; xx < x2; xx++)
            {
                buf.SetPixel(xx, line, col);
            }
        }
        public void Dispose()
        {
           
        }
    }
}
