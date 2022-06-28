using AForge;
using AForge.Imaging.Filters;
using loci.common.services;
using loci.formats;
using loci.formats.services;
using ome.xml.model.primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BitMiracle.LibTiff.Classic;

namespace BioImage
{
    public static class Table
    {
        public static Hashtable hashID = new Hashtable();
        public static List<BioImage> images = new List<BioImage>();
        public static Hashtable viewers = new Hashtable();
        public static BioImage GetImage(string ids)
        {
            for (int i = 0; i < images.Count; i++)
            {
                if (images[i].ID == ids)
                    return images[i];
            }
            return null;
        }
        public static void AddImage(BioImage im)
        {
            images.Add(im);
            //NodeView.viewer.AddTab(im);
        }
        public static int GetImageCount(string s)
        {
            int i = 0;
            string f = Path.GetFileNameWithoutExtension(s);
            foreach (BioImage im in images)
            {
                if (im.Filename.StartsWith(f))
                    i++;
            }
            return i;
        }
        public static string GetImageName(string s)
        {
            int i = Table.GetImageCount(s);
            if (i == 0)
                return s;
            else
                return Path.GetFileNameWithoutExtension(s) + "-" + i + Path.GetExtension(s);
        }
        public static void RemoveImage(BioImage im)
        {
            RemoveImage(im.ID);
        }
        public static void RemoveImage(string id)
        {
            BioImage im = GetImage(id);
            if (im == null)
                return;
            images.Remove(im);
            im.Dispose();
            GC.Collect();
            Recorder.AddLine("Table.RemoveImage(" + '"' + id + '"' + ");");
        }
        public static void AddViewer(ImageView v)
        {
            if (!viewers.ContainsKey(v.image.ID))
                viewers.Add(v.image.ID, v);
        }
        public static void RemoveViewer(ImageView v)
        {
            viewers.Remove(v.image.ID);
            v.Dispose();
        }
        public static ImageView GetViewer(string s)
        {
            if (s.Contains("/i/"))
            {
                int ind = s.LastIndexOf('/');
                string sub = s.Substring(0, ind - 2);
                return (ImageView)viewers[sub];
            }
            else
                return (ImageView)viewers[s];
        }
    }
    public struct ZCT
    {
        public int Z, C, T;
        public ZCT(int z, int c, int t)
        {
            Z = z;
            C = c;
            T = t;
        }
        public static bool operator ==(ZCT c1, ZCT c2)
        {
            if (c1.Z == c2.Z && c1.C == c2.C && c1.T == c2.T)
                return true;
            else
                return false;
        }
        public static bool operator !=(ZCT c1, ZCT c2)
        {
            if (c1.Z != c2.Z || c1.C != c2.C || c1.T != c2.T)
                return false;
            else
                return true;
        }
        public override string ToString()
        {
            return Z + "," + C + "," + T;
        }
    }
    public struct ZCTXY
    {
        public int Z, C, T, X, Y;
        public ZCTXY( int z, int c, int t, int x, int y)
        {
            Z = z;
            C = c;
            T = t;
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return Z + "," + C + "," + T + "," + X + "," + Y;
        }

        public static bool operator ==(ZCTXY c1, ZCTXY c2)
        {
            if (c1.Z == c2.Z && c1.C == c2.C && c1.T == c2.T && c1.X == c2.X && c1.Y == c2.Y)
                return true;
            else
                return false;
        }
        public static bool operator !=(ZCTXY c1, ZCTXY c2)
        {
            if (c1.Z != c2.Z || c1.C != c2.C || c1.T != c2.T || c1.X != c2.X || c1.Y != c2.Y)
                return false;
            else
                return true;
        }
    }
    public enum RGB
    {
        R,
        G,
        B,
        Gray
    }
    public class ColorS
    {
        public ushort R = 0;
        public ushort G = 0;
        public ushort B = 0;
        public ColorS()
        {

        }
        public ColorS(ushort s)
        {
            R = s;
            G = s;
            B = s;
        }
        public ColorS(ushort r, ushort g, ushort b)
        {
            R = r;
            G = g;
            B = b;
        }
        public static ColorS FromColor(System.Drawing.Color col)
        {
            float r = (((float)col.R) / 255) * ushort.MaxValue;
            float g = (((float)col.G) / 255) * ushort.MaxValue;
            float b = (((float)col.B) / 255) * ushort.MaxValue;
            ColorS color = new ColorS();
            color.R = (ushort)r;
            color.G = (ushort)g;
            color.B = (ushort)b;
            return color;
        }
        public static System.Drawing.Color ToColor(ColorS col)
        {
            float r = ((float)(col.R) / 65535) * 255;
            float g = ((float)(col.G) / 65535) * 255;
            float b = ((float)(col.B) / 65535) * 255;
            System.Drawing.Color c = System.Drawing.Color.FromArgb((byte)r, (byte)g, (byte)b);
            return c;
        }
        public override string ToString()
        {
            return R + "," + G + "," + B;
        }
    }
    public struct PointD
    {
        public double X;
        public double Y;
        public PointD(double x, double y)
        {
            X = x;
            Y = y;
        }
        public PointF ToPointF()
        {
            return new PointF((float)X, (float)Y);
        }
        public System.Drawing.Point ToPointInt()
        {
            return new System.Drawing.Point((int)X, (int)Y);
        }

        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString();
        }
        public static bool operator ==(PointD p1, PointD p2)
        {
            return (p1.X == p2.X && p1.Y == p2.Y);
        }
        public static bool operator !=(PointD p1, PointD p2)
        {
            return (p1.X != p2.X && p1.Y != p2.Y);
        }
    }
    public struct RectangleD
    {
        public double X;
        public double Y;
        public double W;
        public double H;

        public RectangleD(double x, double y, double w, double h)
        {
            X = x;
            Y = y;
            W = w;
            H = h;
        }
        public System.Drawing.Rectangle ToRectangleInt()
        {
            return new System.Drawing.Rectangle((int)X, (int)Y, (int)W, (int)H);
        }
        public bool IntersectsWith(PointD p)
        {
            if (X <= p.X && (X + W) >= p.X && Y <= p.Y && (Y + H) >= p.Y)
                return true;
            else
                return false;
        }
        public bool IntersectsWith(double x, double y)
        {
            if (X <= x && (X + W) >= x && Y <= y && (Y + H) >= y)
                return true;
            else
                return false;
        }
        public RectangleF ToRectangleF()
        {
            return new RectangleF((float)X, (float)Y, (float)W, (float)H);
        }
        public override string ToString()
        {
            return X.ToString() + ", " + Y.ToString() + ", " + W.ToString() + ", " + H.ToString();
        }

    }
    public class Annotation
    {
        public enum Type
        {
            Rectangle,
            Point,
            Line,
            Polygon,
            Polyline,
            Freeform,
            Ellipse,
            Label
        }
        public PointD Point
        {
            get
            {
                if (type == Type.Line || type == Type.Ellipse || type == Type.Label || type == Type.Freeform)
                    return new PointD(BoundingBox.X, BoundingBox.Y);
                return Points[0];
            }
            set
            {
                if (Points.Count == 0)
                {
                    AddPoint(value);
                }
                else
                    UpdatePoint(value, 0);
                UpdateSelectBoxs();
                UpdateBoundingBox();
            }
        }
        public RectangleD Rect
        {
            get
            {
                if (type == Type.Line || type == Type.Polyline || type == Type.Polygon || type == Type.Freeform || type == Type.Label)
                    return BoundingBox;
                if (type == Type.Rectangle || type == Type.Ellipse)
                    return new RectangleD(Points[0].X, Points[0].Y, Points[1].X - Points[0].X, Points[2].Y - Points[0].Y);
                else
                    return new RectangleD(Points[0].X, Points[0].Y, 1, 1);
            }
            set
            {
                if (type == Type.Line || type == Type.Polyline || type == Type.Polygon || type == Type.Freeform)
                {
                    BoundingBox = value;
                }
                else
                if (Points.Count < 4 && (type == Type.Rectangle || type == Type.Ellipse))
                {
                    AddPoint(new PointD(value.X, value.Y));
                    AddPoint(new PointD(value.X + value.W, value.Y));
                    AddPoint(new PointD(value.X, value.Y + value.H));
                    AddPoint(new PointD(value.X + value.W, value.Y + value.H));
                }
                else
                if (type == Type.Rectangle || type == Type.Ellipse)
                {
                    Points[0] = new PointD(value.X, value.Y);
                    Points[1] = new PointD(value.X + value.W, value.Y);
                    Points[2] = new PointD(value.X, value.Y + value.H);
                    Points[3] = new PointD(value.X + value.W, value.Y + value.H);
                }
                UpdateSelectBoxs();
                UpdateBoundingBox();
            }
        }
        public double X
        {
            get
            {
                return Point.X;
            }
            set
            {
                Rect = new RectangleD(value, Y, W, H);
            }
        }
        public double Y
        {
            get
            {
                return Point.Y;
            }
            set
            {
                Rect = new RectangleD(X, value, W, H);
            }
        }
        public double W
        {
            get
            {
                if (type == Type.Point)
                    return strokeWidth;
                else
                    return BoundingBox.W;
            }
            set
            {
                Rect = new RectangleD(X, Y, value, H);
            }
        }
        public double H
        {
            get
            {
                if (type == Type.Point)
                    return strokeWidth;
                else
                    return BoundingBox.H;
            }
            set
            {
                Rect = new RectangleD(X, Y, W, value);
            }
        }

        public Type type;
        public float selectBoxSize = 4;
        private List<PointD> Points = new List<PointD>();
        public List<PointD> PointsD
        {
            get
            {
                return Points;
            }
        }
        public List<RectangleF> selectBoxs = new List<RectangleF>();
        public List<int> selectedPoints = new List<int>();
        public RectangleD BoundingBox;
        public Font font = System.Drawing.SystemFonts.DefaultFont;
        public ZCT coord;
        public System.Drawing.Color strokeColor;
        public System.Drawing.Color fillColor;
        public bool isFilled = false;
        public string id = "";
        public string roiID = "";
        public string roiName = "";
        private string text = "";

        public double strokeWidth = 1;
        public int shapeIndex = 0;
        public bool closed = false;
        public bool selected = false;

        public Annotation Copy()
        {
            Annotation copy = new Annotation();
            copy.id = id;
            copy.roiID = roiID;
            copy.roiName = roiName;
            copy.text = text;
            copy.strokeWidth = strokeWidth;
            copy.strokeColor = strokeColor;
            copy.fillColor = fillColor;
            copy.Points = Points;
            copy.selected = selected;
            copy.shapeIndex = shapeIndex;
            copy.closed = closed;
            copy.font = font;
            copy.selectBoxs = selectBoxs;
            copy.BoundingBox = BoundingBox;
            copy.isFilled = isFilled;
            copy.coord = coord;
            copy.selectedPoints = selectedPoints;

            return copy;
        }
        public Annotation Copy(ZCT cord)
        {
            Annotation copy = new Annotation();
            copy.type = type;
            copy.selectBoxSize = selectBoxSize;
            copy.id = id;
            copy.roiID = roiID;
            copy.roiName = roiName;
            copy.text = text;
            copy.strokeWidth = strokeWidth;
            copy.strokeColor = strokeColor;
            copy.fillColor = fillColor;
            copy.Points.AddRange(Points);
            copy.selected = selected;
            copy.shapeIndex = shapeIndex;
            copy.closed = closed;
            copy.font = font;
            copy.selectBoxs.AddRange(selectBoxs);
            copy.BoundingBox = BoundingBox;
            copy.isFilled = isFilled;
            copy.coord = cord;
            copy.selectedPoints = selectedPoints;
            return copy;
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
                if (type == Type.Label)
                {
                    UpdateBoundingBox();
                    UpdateSelectBoxs();
                }
            }
        }
        public Size TextSize
        {
            get
            {
                return TextRenderer.MeasureText(text, font);
            }
        }
        public RectangleD GetSelectBound()
        {
            double f = selectBoxSize / 2;
            return new RectangleD(BoundingBox.X - f, BoundingBox.Y - f, BoundingBox.W + f, BoundingBox.H + f);
        }
        public Annotation()
        {
            coord = new ZCT(0, 0, 0);
            strokeColor = System.Drawing.Color.Yellow;
            font = SystemFonts.DefaultFont;
            BoundingBox = new RectangleD(0, 0, 1, 1);
        }

        public static Annotation CreatePoint(ZCT coord, double x, double y)
        {
            Annotation an = new Annotation();
            an.coord = coord;
            an.AddPoint(new PointD(x, y));
            an.type = Type.Point;
            return an;
        }
        public static Annotation CreateLine(ZCT coord, PointD x1, PointD x2)
        {
            Annotation an = new Annotation();
            an.coord = coord;
            an.type = Type.Line;
            an.AddPoint(x1);
            an.AddPoint(x2);
            return an;
        }
        public static Annotation CreateRectangle(ZCT coord, double x, double y, double w, double h)
        {
            Annotation an = new Annotation();
            an.coord = coord;
            an.type = Type.Rectangle;
            an.Rect = new RectangleD(x, y, w, h);
            return an;
        }
        public static Annotation CreateEllipse(ZCT coord, double x, double y, double w, double h)
        {
            Annotation an = new Annotation();
            an.coord = coord;
            an.type = Type.Ellipse;
            an.Rect = new RectangleD(x, y, w, h);
            return an;
        }
        public static Annotation CreatePolygon(ZCT coord, PointD[] pts)
        {
            Annotation an = new Annotation();
            an.coord = coord;
            an.type = Type.Polygon;
            an.AddPoints(pts);
            an.closed = true;
            return an;
        }
        public static Annotation CreateFreeform(ZCT coord, PointD[] pts)
        {
            Annotation an = new Annotation();
            an.coord = coord;
            an.type = Type.Freeform;
            an.AddPoints(pts);
            an.closed = true;
            return an;
        }

        public void UpdatePoint(PointD p, int i)
        {
            if (i < Points.Count)
            {
                Points[i] = p;
            }
            UpdateBoundingBox();
            UpdateSelectBoxs();
        }
        public PointD GetPoint(int i)
        {
            return Points[i];
        }
        public PointD[] GetPoints()
        {
            return Points.ToArray();
        }
        public PointF[] GetPointsF()
        {
            PointF[] pfs = new PointF[Points.Count];
            for (int i = 0; i < Points.Count; i++)
            {
                pfs[i].X = (float)Points[i].X;
                pfs[i].Y = (float)Points[i].Y;
            }
            return pfs;
        }
        public void AddPoint(PointD p)
        {
            Points.Add(p);
            UpdateSelectBoxs();
            UpdateBoundingBox();
        }
        public void AddPoints(PointD[] p)
        {
            Points.AddRange(p);
            UpdateSelectBoxs();
            UpdateBoundingBox();
        }
        public void RemovePoints(int[] indexs)
        {
            List<PointD> inds = new List<PointD>();
            for (int i = 0; i < Points.Count; i++)
            {
                bool found = false;
                for (int ind = 0; ind < indexs.Length; ind++)
                {
                    if (indexs[ind] == i)
                        found = true;
                }
                if (!found)
                    inds.Add(Points[i]);
            }
            Points = inds;
            UpdateBoundingBox();
            UpdateSelectBoxs();
        }
        public int GetPointCount()
        {
            return Points.Count;
        }
        public PointD[] stringToPoints(string s)
        {
            List<PointD> pts = new List<PointD>();
            string[] ints = s.Split(' ');
            for (int i = 0; i < ints.Length; i++)
            {
                string[] sints = ints[i].Split(',');
                double x = double.Parse(sints[0]);
                double y = double.Parse(sints[1]);
                pts.Add(new PointD(x, y));
            }
            return pts.ToArray();
        }
        public string PointsToString()
        {
            string pts = "";
            for (int j = 0; j < Points.Count; j++)
            {
                if (j == Points.Count - 1)
                    pts += Points[j].X.ToString() + "," + Points[j].Y.ToString();
                else
                    pts += Points[j].X.ToString() + "," + Points[j].Y.ToString() + " ";
            }
            return pts;
        }
        public string PointsToString(PointD[] Points)
        {
            string pts = "";
            for (int j = 0; j < Points.Length; j++)
            {
                if (j == Points.Length - 1)
                    pts += Points[j].X.ToString() + "," + Points[j].Y.ToString();
                else
                    pts += Points[j].X.ToString() + "," + Points[j].Y.ToString() + " ";
            }
            return pts;
        }
        public void UpdateSelectBoxs()
        {
            float f = selectBoxSize / 2;
            selectBoxs.Clear();
            if (type == Type.Label)
            {
                selectBoxs.Add(new RectangleF((float)Points[0].X - f, (float)Points[0].Y - f, selectBoxSize, selectBoxSize));
            }
            else
                for (int i = 0; i < Points.Count; i++)
                {
                    selectBoxs.Add(new RectangleF((float)Points[i].X - f, (float)Points[i].Y - f, selectBoxSize, selectBoxSize));
                }
        }
        public void UpdateBoundingBox()
        {
            if (type == Type.Label)
            {
                if (text != "")
                {
                    Size s = TextSize;
                    BoundingBox = new RectangleD(Points[0].X, Points[0].Y, s.Width, s.Height);
                }
            }
            else
            {
                RectangleD r = new RectangleD(float.MaxValue, float.MaxValue, 0, 0);
                foreach (PointD p in Points)
                {
                    if (r.X > p.X)
                        r.X = p.X;
                    if (r.Y > p.Y)
                        r.Y = p.Y;
                    if (r.W < p.X)
                        r.W = p.X;
                    if (r.H < p.Y)
                        r.H = p.Y;
                }
                r.W = r.W - r.X;
                r.H = r.H - r.Y;
                if (r.W == 0)
                    r.W = 1;
                if (r.H == 0)
                    r.H = 1;
                BoundingBox = r;
            }
        }
        public override string ToString()
        {
            return type.ToString() + ", " + Text + " (" + Point.X + ", " + Point.Y + ") " + coord.ToString();
        }
    }
    public class Channel
    {
        public string Name = "";
        public string ID = "";
        private int index = 0;
        public string Fluor = "";
        public int SamplesPerPixel;
        public System.Drawing.Color? color;
        public int Emission = -1;
        public int Excitation = -1;
        public int Exposure = -1;
        public string LightSource = "";
        public double LightSourceIntensity = -1;
        public int LightSourceWavelength = -1;
        public string ContrastMethod = "";
        public string IlluminationType = "";
        public int bitsPerPixel;

        public IntRange range;
        public Statistics stats;
        public int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }

        }
        public int Max
        {
            get
            {
                return range.Max;
            }
            set
            {
                range.Max = value;
            }
        }
        public int Min
        {
            get
            {
                return range.Min;
            }
            set
            {
                range.Min = value;
            }
        }
        public RGB rgb = RGB.R;
        public Channel(int ind, int bitsPerPixel)
        {
            if (bitsPerPixel == 16)
                Max = 65535;
            if (bitsPerPixel == 14)
                Max = 16383;
            if (bitsPerPixel == 12)
                Max = 4096;
            if (bitsPerPixel == 10)
                Max = 1024;
            if (bitsPerPixel == 8)
                Max = byte.MaxValue;
            range = new IntRange(0, Max);
            Min = 0;
            index = ind;
        }
        public Channel Copy()
        {
            Channel c = new Channel(index, bitsPerPixel);
            c.Name = Name;
            c.ID = ID;
            c.range = range;
            c.color = color;
            c.Fluor = Fluor;
            c.SamplesPerPixel = SamplesPerPixel;
            c.Emission = Emission;
            c.Excitation = Excitation;
            c.Exposure = Exposure;
            c.LightSource = LightSource;
            c.LightSourceIntensity = LightSourceIntensity;
            c.LightSourceWavelength = LightSourceWavelength;
            c.ContrastMethod = ContrastMethod;
            c.IlluminationType = IlluminationType;
            return c;
        }
        public override string ToString()
        {
            if (Name == "")
                return index.ToString();
            else
                return index + ", " + Name;
        }
    }
    public class BufferInfo : IDisposable
    {
        public ushort GetValueRGB(int ix, int iy, int index)
        {
            int i = -1;
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (BitsPerPixel > 8)
            {
                int index2 = (y * stridex + x) * 2 * index;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                int stride = SizeX;
                int indexb = (y * stridex + x) * index;
                i = bytes[indexb];
                return (ushort)i;
            }
        }
        public ushort GetValue(int ix, int iy)
        {
            int i = 0;
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (ix < 0)
                x = 0;
            if (iy < 0)
                y = 0;
            if (ix >= SizeX)
                x = SizeX - 1;
            if (iy >= SizeY)
                y = SizeY - 1;

            if (BitsPerPixel > 8)
            {
                int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                int index = (y * stridex + x) * RGBChannelsCount;
                i = bytes[index];
                return (ushort)i;
            }

        }
        public void SetValue(int ix, int iy, ushort value)
        {
            byte[] bts = bytes;
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (BitsPerPixel > 8)
            {
                int index2 = ((y * stridex + x) * 2 * RGBChannelsCount);
                byte upper = (byte)(value >> 8);
                byte lower = (byte)(value & 0xff);
                bytes[index2] = lower;
                bytes[index2 + 1] = upper;
            }
            else
            {
                int index = (y * stridex + x) * RGBChannelsCount;
                bytes[index] = (byte)value;
            }
        }
        public void SetValueRGB(int ix, int iy, int RGBChannel, ushort value)
        {
            int stride = SizeX;
            int x = ix;
            int y = iy;
            if (BitsPerPixel > 8)
            {
                int index2 = ((y * stride + x) * 2 * RGBChannelsCount);
                byte upper = (byte)(value >> 8);
                byte lower = (byte)(value & 0xff);
                bytes[index2] = lower;
                bytes[index2 + 1] = upper;
            }
            else
            {
                int index = ((y * stride + x) * RGBChannelsCount) + (RGBChannel);
                bytes[index] = (byte)value;
            }
        }
        public long GetIndex(int x, int y)
        {
            if (BitsPerPixel > 8)
            {
                return (y * Stride + x) * 2 * RGBChannelsCount;
            }
            else
            {
                return (y * Stride + x) * RGBChannelsCount;
            }
        }
        public static string CreateID(string filepath, int index)
        {
            const char sep = '/';
            filepath = filepath.Replace("\\", "/");
            string s = filepath + sep + 'i' + sep + index;
            return s;
        }
        public string ID;
        public string File
        {
            get { return file; }
            set { file = value; }
        }
        public int HashID
        {
            get
            {
                return ID.GetHashCode();
            }
        }
        public int SizeX, SizeY;
        public int Stride
        {
            get
            {
                int s = 0;
                if (pixelFormat == PixelFormat.Format8bppIndexed)
                    s = SizeX;
                else
                if (pixelFormat == PixelFormat.Format16bppGrayScale)
                    s = SizeX * 2;
                else
                if (pixelFormat == PixelFormat.Format24bppRgb)
                    s = SizeX * 3;
                else
                    if (pixelFormat == PixelFormat.Format32bppRgb || pixelFormat == PixelFormat.Format32bppArgb)
                    s = SizeX * 4;
                else
                    s = SizeX * 3 * 2;
                return s;
            }
        }
        public int PaddedStride
        {
            get
            {
                return GetStridePadded(Stride);
            }
        }
        public bool LittleEndian
        {
            get
            {
                return BitConverter.IsLittleEndian;
            }
        }
        public int Length
        {
            get
            {
                return bytes.Length;
            }
        }
        public int RGBChannelsCount
        {
            get
            {
                if (PixelFormat == PixelFormat.Format24bppRgb || PixelFormat == PixelFormat.Format48bppRgb)
                    return 3;
                else
                if (PixelFormat == PixelFormat.Format8bppIndexed || PixelFormat == PixelFormat.Format16bppGrayScale)
                    return 1;
                else
                    return 4;
            }
        }
        public int BitsPerPixel
        {
            get
            {
                if (PixelFormat == PixelFormat.Format16bppGrayScale || PixelFormat == PixelFormat.Format48bppRgb)
                {
                    return 16;
                }
                else
                    return 8;
            }
        }
        public ZCT Coordinate;
        public PixelFormat PixelFormat
        {
            get
            {
                return pixelFormat;
            }
            set
            {
                pixelFormat = value;
            }
        }
        public byte[] Bytes
        {
            get { return bytes; }
            set { bytes = value; }
        }
        private Bitmap bitmap = null;
        public Image Image
        {
            get
            { 
                bitmap = GetBitmap(SizeX, SizeY, Stride, PixelFormat, Bytes);
                return bitmap;
            }
            set
            {
                Bitmap b = (Bitmap)value;
                b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                PixelFormat = value.PixelFormat;
                SizeX = value.Width;
                SizeY = value.Height;
                if(isRGB)
                b = BufferInfo.SwitchRedBlue(b);
                bytes = GetBuffer((Bitmap)b);
            }
        }
        private PixelFormat pixelFormat;
        public Statistics Statistics
        {
            get { return statistics; }
        }
        private Statistics statistics;
        private byte[] bytes;
        private string file;
        private static int GetStridePadded(int stride)
        {
            if (stride % 4 == 0)
                return stride;
            int newstride = stride + 2;
            if (stride % 3 == 0 && stride % 2 != 0)
            {
                newstride = stride + 1;
                if (newstride % 4 != 0)
                    newstride = stride + 3;
            }
            if (newstride % 4 != 0)
                throw new InvalidOperationException("Stride padding failed");
            return newstride;
        }
        private static byte[] GetPaddedBuffer(byte[] bts, int w, int h, int stride, PixelFormat px)
        {
            int newstride = GetStridePadded(stride);
            if (newstride == stride)
                return bts;
            byte[] newbts = new byte[newstride * h];
            if (px == PixelFormat.Format24bppRgb || px == PixelFormat.Format32bppArgb)
            {
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w; ++x)
                    {
                        int index = (y * stride) + x;
                        int index2 = (y * newstride) + x;
                        newbts[index2] = bts[index];
                    }
                }
            }
            else
            {
                for (int y = 0; y < h; ++y)
                {
                    for (int x = 0; x < w * 2; ++x)
                    {
                        int index = (y * stride) + x;
                        int index2 = (y * newstride) + x;
                        newbts[index2] = bts[index];
                    }
                }
            }
            return newbts;
        }
        public static BufferInfo[] RGB48To16(string file, int w, int h, int stride, byte[] bts, ZCT coord)
        {
            BufferInfo[] bfs = new BufferInfo[3];
            //opening a 8 bit per pixel jpg image
            Bitmap bmpr = new Bitmap(w, h, PixelFormat.Format16bppGrayScale);
            Bitmap bmpg = new Bitmap(w, h, PixelFormat.Format16bppGrayScale);
            Bitmap bmpb = new Bitmap(w, h, PixelFormat.Format16bppGrayScale);
            //creating the bitmapdata and lock bits
            System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, w, h);
            BitmapData bmdr = bmpr.LockBits(rec, ImageLockMode.ReadWrite, bmpr.PixelFormat);
            BitmapData bmdg = bmpg.LockBits(rec, ImageLockMode.ReadWrite, bmpg.PixelFormat);
            BitmapData bmdb = bmpb.LockBits(rec, ImageLockMode.ReadWrite, bmpb.PixelFormat);
            unsafe
            {
                //iterating through all the pixels in y direction
                for (int y = 0; y < h; y++)
                {
                    //getting the pixels of current row
                    byte* rowr = (byte*)bmdr.Scan0 + (y * bmdr.Stride);
                    byte* rowg = (byte*)bmdg.Scan0 + (y * bmdg.Stride);
                    byte* rowb = (byte*)bmdb.Scan0 + (y * bmdb.Stride);
                    int rowRGB = y * stride;
                    //iterating through all the pixels in x direction
                    for (int x = 0; x < w; x++)
                    {
                        int indexRGB = x * 6;
                        int index16 = x * 2;
                        //R
                        rowr[index16 + 1] = bts[rowRGB + indexRGB];
                        rowr[index16] = bts[rowRGB + indexRGB + 1];
                        //G
                        rowg[index16 + 1] = bts[rowRGB + indexRGB + 2];
                        rowg[index16] = bts[rowRGB + indexRGB + 3];
                        //B
                        rowb[index16 + 1] = bts[rowRGB + indexRGB + 4];
                        rowb[index16] = bts[rowRGB + indexRGB + 5];

                    }
                }
            }
            bmpr.UnlockBits(bmdr);
            bmpg.UnlockBits(bmdg);
            bmpb.UnlockBits(bmdb);
            bfs[0] = new BufferInfo(file, bmpr, new ZCT(coord.Z, 0, coord.T), 0);
            bfs[1] = new BufferInfo(file, bmpg, new ZCT(coord.Z, 0, coord.T), 0);
            bfs[2] = new BufferInfo(file, bmpb, new ZCT(coord.Z, 0, coord.T), 0);
            return bfs;
        }
        public static unsafe Bitmap GetBitmap(int w, int h, int stride, PixelFormat px, byte[] bts)
        {
            if (px == PixelFormat.Format24bppRgb)
            {
                //opening a 8 bit per pixel jpg image
                Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                //creating the bitmapdata and lock bits
                System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, w, h);
                BitmapData bmd = bmp.LockBits(rec, ImageLockMode.ReadWrite, bmp.PixelFormat);
                unsafe
                {
                    //iterating through all the pixels in y direction
                    for (int y = 0; y < h; y++)
                    {
                        //getting the pixels of current row
                        byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                        int rowRGB = y * stride;
                        //iterating through all the pixels in x direction
                        for (int x = 0; x < w; x++)
                        {
                            int indexRGB = x * 3;
                            int indexRGBA = x * 4;
                            row[indexRGBA + 3] = byte.MaxValue;//byte A
                            row[indexRGBA + 2] = bts[rowRGB + indexRGB + 2];//byte R
                            row[indexRGBA + 1] = bts[rowRGB + indexRGB + 1];//byte G
                            row[indexRGBA] = bts[rowRGB + indexRGB];//byte B
                        }
                    }
                }
                //unlocking bits and disposing image
                bmp.UnlockBits(bmd);
                return bmp;
            }
            else
            if (px == PixelFormat.Format48bppRgb)
            {
                //opening a 8 bit per pixel jpg image
                Bitmap bmp = new Bitmap(w, h, PixelFormat.Format32bppArgb);
                //creating the bitmapdata and lock bits
                System.Drawing.Rectangle rec = new System.Drawing.Rectangle(0, 0, w, h);
                BitmapData bmd = bmp.LockBits(rec, ImageLockMode.ReadWrite, bmp.PixelFormat);
                unsafe
                {
                    //iterating through all the pixels in y direction
                    for (int y = 0; y < h; y++)
                    {
                        //getting the pixels of current row
                        byte* row = (byte*)bmd.Scan0 + (y * bmd.Stride);
                        int rowRGB = y * stride;
                        //iterating through all the pixels in x direction
                        for (int x = 0; x < w; x++)
                        {
                            int indexRGB = x * 6;
                            int indexRGBA = x * 4;
                            int b = (int)Math.Floor(((float)BitConverter.ToUInt16(bts, rowRGB + indexRGB)) / 255);
                            int g = (int)Math.Floor(((float)BitConverter.ToUInt16(bts, rowRGB + indexRGB + 2)) / 255);
                            int r = (int)Math.Floor(((float)BitConverter.ToUInt16(bts, rowRGB + indexRGB + 4)) / 255);
                            row[indexRGBA + 3] = 255;//byte A
                            row[indexRGBA + 2] = (byte)(b);//byte R
                            row[indexRGBA + 1] = (byte)(g);//byte G
                            row[indexRGBA] =     (byte)(r);//byte B
                        }
                    }
                }
                bmp.UnlockBits(bmd);
                return bmp;
            }
            fixed (byte* numPtr1 = bts)
            {
                if (stride % 4 == 0)
                { 
                    return new Bitmap(w, h, stride, px, new IntPtr((void*)numPtr1));
                }
                int newstride = GetStridePadded(stride);
                byte[] newbts = GetPaddedBuffer(bts, w, h, stride,px);
                fixed (byte* numPtr2 = newbts)
                {
                    return new Bitmap(w, h, newstride, px, new IntPtr((void*)numPtr2));
                }
            }
        }
        public void SetImageRaw(Bitmap b)
        {
            b.RotateFlip(RotateFlipType.Rotate180FlipNone);
            PixelFormat = b.PixelFormat;
            SizeX = b.Width;
            SizeY = b.Height;
            b = BufferInfo.SwitchRedBlue(b);
            bytes = GetBuffer((Bitmap)b);
        }
        public void Crop(Rectangle r)
        {
            //This crop function supports 16 bit images unlike Bitmap class.
            if (BitsPerPixel > 8)
            {
                byte[] bts = null;
                int bytesPer = 2;
                int stridenew = r.Width * bytesPer;
                int strideold = Stride;
                bts = new byte[(stridenew * r.Height)];
                for (int y = 0; y < r.Height; y++)
                {
                    for (int x = 0; x < stridenew; x += bytesPer)
                    {
                        int indexnew = (y * stridenew + x) * RGBChannelsCount;
                        int indexold = (((y + r.Y) * strideold + (x + (r.X * bytesPer))) * RGBChannelsCount);// + r.X;
                        bts[indexnew] = bytes[indexold];
                        bts[indexnew + 1] = bytes[indexold + 1];
                    }
                }
                bytes = bts;
            }
            else
            {
                Image = ((Bitmap)Image).Clone(r, PixelFormat);
            }           
            SizeX = r.Width;
            SizeY = r.Height;
        }
        public BufferInfo(string file, int w, int h, PixelFormat px, byte[] bts, ZCT coord, int index)
        {
            ID = CreateID(file, index);
            SizeX = w;
            SizeY = h;
            pixelFormat = px;

            Coordinate = coord;
            bytes = bts;
            if (isRGB)
                SwitchRedBlue();
        }
        public BufferInfo(string file, Image im, ZCT coord, int index)
        {
            ID = CreateID(file, index);
            SizeX = im.Width;
            SizeY = im.Height;
            pixelFormat = im.PixelFormat;
            Coordinate = coord;
            Image = im;
            if (isRGB)
                SwitchRedBlue();
        }
        public BufferInfo(int w, int h, PixelFormat px, byte[] bts, ZCT coord, string id)
        {
            ID = id;
            SizeX = w;
            SizeY = h;
            pixelFormat = px;
            Coordinate = coord;
            bytes = bts;
            if (isRGB)
                SwitchRedBlue();
        }
        public Statistics UpdateStatistics()
        {
            statistics = Statistics.FromBytes(bytes, SizeX, SizeY, RGBChannelsCount, BitsPerPixel, Stride);
            return statistics;
        }
        public static Bitmap SwitchRedBlue(Bitmap image)
        {
            ExtractChannel cr = new ExtractChannel(AForge.Imaging.RGB.R);
            ExtractChannel cb = new ExtractChannel(AForge.Imaging.RGB.B);
            // apply the filter
            Bitmap rImage = cr.Apply(image);
            Bitmap bImage = cb.Apply(image);

            ReplaceChannel replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, bImage);
            replaceRFilter.ApplyInPlace(image);

            ReplaceChannel replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, rImage);
            replaceBFilter.ApplyInPlace(image);
            rImage.Dispose();
            bImage.Dispose();
            return image;
        }
        public void SwitchRedBlue()
        {
            if (PixelFormat == PixelFormat.Format8bppIndexed || PixelFormat == PixelFormat.Format16bppGrayScale)
                throw new ArgumentException("Can't switch Red & Blue in non RGB image");
            //BufferInfo bf = new BufferInfo(SizeX, SizeY,PixelFormat, bytes, Coordinate, ID);
            if(PixelFormat == PixelFormat.Format24bppRgb)
            for (int y = 0; y < SizeY; y++)
            {
                for (int x = 0; x < Stride; x += 3)
                {
                    int i = y * Stride + x;
                    byte bb = bytes[i + 2];
                    bytes[i + 2] = bytes[i];
                    bytes[i] = bb;
                }
            }
            if (PixelFormat == PixelFormat.Format32bppArgb)
                for (int y = 0; y < SizeY; y++)
                {
                    for (int x = 0; x < Stride; x += 4)
                    {
                        int i = y * Stride + x;
                        byte bb = bytes[i + 2];
                        bytes[i + 2] = bytes[i];
                        bytes[i] = bb;
                    }
                }
            /*
            Bitmap b = (Bitmap)Image;
            ExtractChannel cr = new ExtractChannel(AForge.Imaging.RGB.R);
            ExtractChannel cb = new ExtractChannel(AForge.Imaging.RGB.B);
            // apply the filter
            Bitmap rImage = cr.Apply(b);
            Bitmap bImage = cb.Apply(b);

            ReplaceChannel replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, bImage);
            replaceRFilter.ApplyInPlace(b);

            ReplaceChannel replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, rImage);
            replaceBFilter.ApplyInPlace(b);
            rImage.Dispose();
            bImage.Dispose();
            */
        }
        public byte[] GetSaveBytes()
        {
            BitmapData data;
            Bitmap bitmap = null;
            if (RGBChannelsCount == 1)
                return Bytes;
            else
            if (RGBChannelsCount == 3 && BitsPerPixel > 8)
            {
                bitmap = SwitchRedBlue((Bitmap)Image);
                bitmap = SwitchRedBlue((Bitmap)Image);
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            if (RGBChannelsCount == 4)
            {
                bitmap = (Bitmap)Image.Clone();
                bitmap = SwitchChannels(bitmap, 0, 3);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipX);
                bitmap.RotateFlip(RotateFlipType.RotateNoneFlipY);
            }
            else
            {
                bitmap = (Bitmap)Image;
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            }
            data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, PixelFormat);
            IntPtr ptr = data.Scan0;
            int length = this.bytes.Length;
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            Array.Reverse(bytes);
            bitmap.UnlockBits(data);
            bitmap.Dispose();
            return bytes;
        }
        public static byte[] GetBuffer(Bitmap bitmap)
        {
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadOnly, bitmap.PixelFormat);
            IntPtr ptr = data.Scan0;
            int length = data.Stride * bitmap.Height;
            byte[] bytes = new byte[length];
            Marshal.Copy(ptr, bytes, 0, length);
            Array.Reverse(bytes);
            bitmap.UnlockBits(data);
            return bytes;
        }
        public static Bitmap RGBTo24Bit(Bitmap b)
        {
            Bitmap bm = new Bitmap(b.Width, b.Height, PixelFormat.Format24bppRgb);
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(b, 0, 0);
            return bm;
        }
        public static Bitmap RGBTo32Bit(Bitmap b)
        {
            Bitmap bm = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppArgb);
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(b, 0, 0);
            return bm;
        }
        public void RGBTo32Bit()
        {
            Bitmap bm = new Bitmap(SizeX, SizeY, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage((Bitmap)Image, 0, 0);
            Image = bm;
        }
        public static Bitmap SwitchChannels(Bitmap image, int c1, int c2)
        {
            ExtractChannel cr = new ExtractChannel((short)c1);
            ExtractChannel cb = new ExtractChannel((short)c2);
            // apply the filter
            Bitmap rImage = cr.Apply(image);
            Bitmap bImage = cb.Apply(image);
            ReplaceChannel replaceRFilter = new ReplaceChannel((short)c1, bImage);
            replaceRFilter.ApplyInPlace(image);
            ReplaceChannel replaceBFilter = new ReplaceChannel((short)c2, rImage);
            replaceBFilter.ApplyInPlace(image);
            rImage.Dispose();
            bImage.Dispose();
            return image;
        }
        public BufferInfo Copy()
        {
            byte[] bt = new byte[Bytes.Length];
            for (int i = 0; i < bt.Length; i++)
            {
                bt[i] = bytes[i];
            }
            BufferInfo bf = new BufferInfo(SizeX, SizeY, PixelFormat, bt, Coordinate,ID);
            return bf;
        }
        public void To8Bit()
        {
            Bitmap bm = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)Image);
            bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
            Image = bm;
        }
        public void To16Bit()
        {
            Bitmap bm = AForge.Imaging.Image.Convert8bppTo16bpp((Bitmap)Image);
            bm.RotateFlip(RotateFlipType.Rotate180FlipNone);
            Image = bm;
        }
        public bool isRGB
        {
            get
            {
                if (pixelFormat == PixelFormat.Format8bppIndexed || pixelFormat == PixelFormat.Format16bppGrayScale)
                    return false;
                else
                    return true;
            }
        }
        public override string ToString()
        {
            return ID;
        }
        public void Dispose()
        {
            bytes = null;
            if(bitmap!=null)
            bitmap.Dispose();
        }
    }
    public class Filt
    {
        public enum Type
        {
            Base,
            Base2,
            InPlace,
            InPlace2,
            InPlacePartial,
            Resize,
            Rotate,
            Transformation,
            Copy
        }
        public string name;
        public IFilter filt;
        public Type type;
        public Filt(string s, IFilter f, Type t)
        {
            name = s;
            filt = f;
            type = t;
        }
    }
    public static class Filters
    {
        public static Dictionary<string, Filt> filters = new Dictionary<string, Filt>();
        /*
        public static string ApplyInPlace(BioImage im, string name)
        {
            try
            {
                Filt f = filters[name];
                for (int i = 0; i < im.Buffers.Count; i++)
                {
                    im.Buffers[i].Image = f.filt.Apply((Bitmap)im.Buffers[i].Image);
                }
               
            }
            catch (Exception e)
            {
                return e.ToString();
            }
            return "OK";
        }
        public static BioImage Apply(string id, string name)
        {
            BioImage c = Table.GetImage(id);
            BioImage img = BioImage.Copy(c);
            try
            {
                Filt f = filters[name];
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    Bitmap b = (Bitmap)img.Buffers[i].Image;
                    img.Buffers[i].Image = f.filt.Apply(b);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            Table.AddImage(img);
            ImageViewer iv = new ImageViewer(img);
            Table.AddViewer(iv);
            iv.Show();
            Recorder.AddLine("Filters.Apply(" + '"' + ImageView.viewer.image.ID + '"' + "," + '"' + name + '"' + ", false, ImageView.viewer.Index");
            return img;
        }
        */
        public static BioImage BaseFilter(string id, string name, bool inPlace)
        {
            BioImage img = Table.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseFilter fi = (BaseFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                {
                    Table.AddImage(img);
                    ImageView iv = new ImageView(img);
                    Table.AddViewer(iv);
                    iv.Show();
                }
                //Recorder.AddLine("Filters." + (" + '"' + id + '"' + ", " + '"' + name + '"' + "," + inPlace + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage BaseFilter2(string id, string id2, string name, bool inPlace)
        {
            BioImage c2 = Table.GetImage(id);
            BioImage img = Table.GetImage(id2);
            if(!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseFilter2 fi = (BaseFilter2)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    fi.OverlayImage = (Bitmap)c2.Buffers[i].Image;
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                { 
                    Table.AddImage(img);
                    ImageView iv = new ImageView(img);
                    Table.AddViewer(iv);
                    iv.Show();
                }
                //Recorder.AddLine("Filters.BaseFilter2(" + '"' + id + '"' + "," + '"' + id2 + '"' + ", " +'"' + name + '"' + "," + inPlace + ");");
                return img;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage BaseInPlaceFilter(string id, string name, bool inPlace)
        {
            BioImage img = Table.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseInPlaceFilter fi = (BaseInPlaceFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                {
                    Table.AddImage(img);
                    ImageView iv = new ImageView(img);
                    Table.AddViewer(iv);
                    iv.Show();
                }
                //Recorder.AddLine("Filters.BaseInPlaceFilter(" + '"' + id + '"' + "," + '"' + name + '"' + "," + inPlace + ");");
                return img;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage BaseInPlaceFilter2(string id, string id2, string name, bool inPlace)
        {
            BioImage c2 = Table.GetImage(id);
            BioImage img = Table.GetImage(id2);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseInPlaceFilter2 fi = (BaseInPlaceFilter2)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    fi.OverlayImage = (Bitmap)c2.Buffers[i].Image;
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                {
                    Table.AddImage(img);
                    ImageView iv = new ImageView(img);
                    Table.AddViewer(iv);
                    iv.Show();
                }
                //Recorder.AddLine("Filters.BaseFilter2(" + '"' + id + '"' + "," + '"' + id2 + '"' + ", " + '"' + name + '"' + "," + inPlace + ");");
                return img;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage BaseInPlacePartialFilter(string id, string name, bool inPlace)
        {
            BioImage img = Table.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseInPlacePartialFilter fi = (BaseInPlacePartialFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                {
                    Table.AddImage(img);
                    ImageView iv = new ImageView(img);
                    Table.AddViewer(iv);
                    iv.Show();
                }
                //Recorder.AddLine("Filters.BaseInPlaceFilter(" + '"' + id + '"' + "," + '"' + name + '"' + "," + inPlace + ");");
                return img;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage BaseResizeFilter(string id, string name, bool inPlace, int w, int h)
        {
            BioImage img = Table.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseResizeFilter fi = (BaseResizeFilter)f.filt;
                fi.NewHeight = h;
                fi.NewWidth = w;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                {
                    Table.AddImage(img);
                    ImageView iv = new ImageView(img);
                    Table.AddViewer(iv);
                    iv.Show();
                }
                //Recorder.AddLine("Filters.BaseResizeFilter(" + '"' + id + '"' + ", " + '"' + name + '"' + "," +
                //    inPlace + "," + w + "," + h + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage BaseRotateFilter(string id, string name, bool inPlace, float angle, System.Drawing.Color fill)
        {
            BioImage img = Table.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseRotateFilter fi = (BaseRotateFilter)f.filt;
                fi.Angle = angle;
                fi.FillColor = fill;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                {
                    Table.AddImage(img);
                    ImageView iv = new ImageView(img);
                    Table.AddViewer(iv);
                    iv.Show();
                }
                //Recorder.AddLine("Filters.BaseResizeFilter(" + '"' + id + '"' + ", " + '"' + name + '"' + "," +
                //    inPlace + "," + w + "," + h + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;

        }
        public static BioImage BaseTransformationFilter(string id, string name, bool inPlace, float angle)
        {
            BioImage img = Table.GetImage(id);
            if (!inPlace)
                img = BioImage.Copy(img);
            try
            {
                Filt f = filters[name];
                BaseTransformationFilter fi = (BaseTransformationFilter)f.filt;
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    img.Buffers[i].Image = fi.Apply((Bitmap)img.Buffers[i].Image);
                }
                if (!inPlace)
                {
                    Table.AddImage(img);
                    ImageView iv = new ImageView(img);
                    Table.AddViewer(iv);
                    iv.Show();
                }
                //Recorder.AddLine("Filters.BaseResizeFilter(" + '"' + id + '"' + ", " + '"' + name + '"' + "," +
                //    inPlace + "," + w + "," + h + ");");
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString(), "Filter Error");
            }
            return img;
        }
        public static BioImage Crop(string id, int x, int y, int w, int h)
        {
            BioImage c = Table.GetImage(id);
            BioImage img = BioImage.Copy(c);
            Filt f = filters["Crop"];
            for (int i = 0; i < img.Buffers.Count; i++)
            {
                img.Buffers[i].Crop(new Rectangle(x, y, w, h));
                //AForge.Imaging.Filters.Crop cr = (Crop)f.filt;
                //cr.Rectangle = new Rectangle(x,y,w,h);
                //img.Buffers[i].SetImageRaw(cr.Apply((Bitmap)img.Buffers[i].Image));

            }
            Table.AddImage(img);
            Recorder.AddLine("Filters.Crop(" + '"' + id + '"' + "," + x + "," + y + "," + w + "," + h + ");");
            return img;
        }
        public static BioImage Crop(string id, Rectangle r)
        {
            return Crop(id, r.X, r.Y, r.Width, r.Height);
        }
        public static void Init()
        {
            //Base Filters
            Filt f = new Filt("AdaptiveSmoothing", new AdaptiveSmoothing(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("BayerFilter", new BayerFilter(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("BayerFilterOptimized", new BayerFilterOptimized(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("BayerDithering", new BayerDithering(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("ConnectedComponentsLabeling", new ConnectedComponentsLabeling(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("ExtractChannel", new ExtractChannel(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("ExtractNormalizedRGBChannel", new ExtractNormalizedRGBChannel(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("Grayscale", new Grayscale(0.2125, 0.7154, 0.0721), Filt.Type.Base);
            filters.Add(f.name, f);
            //f = new Filt("TexturedFilter", new TexturedFilter());
            //filters.Add(f.name, f);
            f = new Filt("WaterWave", new WaterWave(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("YCbCrExtractChannel", new YCbCrExtractChannel(), Filt.Type.Base);
            filters.Add(f.name, f);

            //BaseFilter2
            f = new Filt("ThresholdedDifference", new ThresholdedDifference(), Filt.Type.Base2);
            filters.Add(f.name, f);
            f = new Filt("ThresholdedEuclideanDifference", new ThresholdedDifference(), Filt.Type.Base2);
            filters.Add(f.name, f);


            //BaseInPlaceFilter
            f = new Filt("BackwardQuadrilateralTransformation", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("BlobsFiltering", new BlobsFiltering(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("BottomHat", new BottomHat(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("BradleyLocalThresholding", new BradleyLocalThresholding(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasCrop", new CanvasCrop(Rectangle.Empty), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasFill", new CanvasFill(Rectangle.Empty), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasMove", new CanvasMove(new IntPoint()), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("FillHoles", new FillHoles(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("FlatFieldCorrection", new FlatFieldCorrection(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("TopHat", new TopHat(), Filt.Type.InPlace);
            filters.Add(f.name, f);

            //BaseInPlaceFilter2
            f = new Filt("Add", new Add(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Difference", new Difference(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Intersect", new Intersect(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Merge", new Merge(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Morph", new Morph(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("MoveTowards", new MoveTowards(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("StereoAnaglyph", new StereoAnaglyph(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Subtract", new Subtract(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            //f = new Filt("Add", new TexturedMerge(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);

            //BaseInPlacePartialFilter
            f = new Filt("AdditiveNoise", new AdditiveNoise(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);

            //f = new Filt("ApplyMask", new ApplyMask(), Filt.Type.InPlacePartial2);
            //filters.Add(f.name, f);
            f = new Filt("BrightnessCorrection", new BrightnessCorrection(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ChannelFiltering", new ChannelFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ColorFiltering", new ColorFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ColorRemapping", new ColorRemapping(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ContrastCorrection", new ContrastCorrection(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ContrastStretch", new ContrastStretch(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            //f = new Filt("ErrorDiffusionDithering", new ErrorDiffusionDithering(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            f = new Filt("EuclideanColorFiltering", new EuclideanColorFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("GammaCorrection", new GammaCorrection(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("HistogramEqualization", new HistogramEqualization(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("HorizontalRunLengthSmoothing", new HorizontalRunLengthSmoothing(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("HSLFiltering", new HSLFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("HueModifier", new HueModifier(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("Invert", new Invert(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("LevelsLinear", new LevelsLinear(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("LevelsLinear16bpp", new LevelsLinear16bpp(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            //f = new Filt("MaskedFilter", new MaskedFilter(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            //f = new Filt("Mirror", new Mirror(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            f = new Filt("OrderedDithering", new OrderedDithering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("OtsuThreshold", new OtsuThreshold(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("Pixellate", new Pixellate(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("PointedColorFloodFill", new PointedColorFloodFill(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("PointedMeanFloodFill", new PointedMeanFloodFill(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("ReplaceChannel", new Invert(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("RotateChannels", new LevelsLinear(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("SaltAndPepperNoise", new LevelsLinear16bpp(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("SaturationCorrection", new SaturationCorrection(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("Sepia", new Sepia(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("SimplePosterization", new SimplePosterization(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("SISThreshold", new SISThreshold(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            //f = new Filt("Texturer", new Texturer(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            //f = new Filt("Threshold", new Threshold(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);
            f = new Filt("ThresholdWithCarry", new ThresholdWithCarry(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("VerticalRunLengthSmoothing", new VerticalRunLengthSmoothing(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("YCbCrFiltering", new YCbCrFiltering(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            f = new Filt("YCbCrLinear", new YCbCrLinear(), Filt.Type.InPlacePartial);
            filters.Add(f.name, f);
            //f = new Filt("YCbCrReplaceChannel", new YCbCrReplaceChannel(), Filt.Type.InPlacePartial);
            //filters.Add(f.name, f);

            //BaseResizeFilter
            f = new Filt("ResizeBicubic", new ResizeBicubic(0,0), Filt.Type.Resize);
            filters.Add(f.name, f);
            f = new Filt("ResizeBilinear", new ResizeBilinear(0,0), Filt.Type.Resize);
            filters.Add(f.name, f);
            f = new Filt("ResizeNearestNeighbor", new ResizeNearestNeighbor(0,0), Filt.Type.Resize);
            filters.Add(f.name, f);
            //BaseRotateFilter
            f = new Filt("RotateBicubic", new RotateBicubic(0), Filt.Type.Rotate);
            filters.Add(f.name, f);
            f = new Filt("RotateBilinear", new RotateBilinear(0), Filt.Type.Rotate);
            filters.Add(f.name, f);
            f = new Filt("RotateNearestNeighbor", new RotateNearestNeighbor(0), Filt.Type.Rotate);
            filters.Add(f.name, f);

            //Transformation
            f = new Filt("Crop", new Crop(Rectangle.Empty), Filt.Type.Transformation);
            filters.Add(f.name, f);

            f = new Filt("QuadrilateralTransformation", new QuadrilateralTransformation(), Filt.Type.Transformation);
            filters.Add(f.name, f);
            //f = new Filt("QuadrilateralTransformationBilinear", new QuadrilateralTransformationBilinear(), Filt.Type.Transformation);
            //filters.Add(f.name, f);
            //f = new Filt("QuadrilateralTransformationNearestNeighbor", new QuadrilateralTransformationNearestNeighbor(), Filt.Type.Transformation);
            //filters.Add(f.name, f);
            f = new Filt("Shrink", new Shrink(), Filt.Type.Transformation);
            filters.Add(f.name, f);
            f = new Filt("SimpleQuadrilateralTransformation", new SimpleQuadrilateralTransformation(), Filt.Type.Transformation);
            filters.Add(f.name, f);
            f = new Filt("TransformFromPolar", new TransformFromPolar(), Filt.Type.Transformation);
            filters.Add(f.name, f);
            f = new Filt("TransformToPolar", new TransformToPolar(), Filt.Type.Transformation);
            filters.Add(f.name, f);

            //BaseUsingCopyPartialFilter 
            f = new Filt("BinaryDilatation3x3", new BinaryDilatation3x3(), Filt.Type.Copy);
            filters.Add(f.name, f);
            f = new Filt("BilateralSmoothing ", new BilateralSmoothing(), Filt.Type.Copy);
            filters.Add(f.name, f);
            f = new Filt("BinaryErosion3x3 ", new BinaryErosion3x3(), Filt.Type.Copy);
            filters.Add(f.name, f);
            
        }
    }
    public class Statistics
    {
        private int[] values;
        public int[] Values
        {
            get { return values; }
            set { values = value; }
        }
        private int bitsPerPixel;
        private int min;
        private int max;
        private int mean = 0;
        private double median = 0;
        public int Min
        {
            get { return min; }
        }
        public int Max
        {
            get { return max; }
        }
        public double Mean
        {
            get { return mean; }
        }
        public double Median
        {
            get { return median; }
        }

        public int BitsPerPixel
        {
            get { return bitsPerPixel; }
        }
        private int count = 0;
        private double meansum = 0;
        private double[] meanvalues;
        public double[] MeanValues
        {
            get { return meanvalues; }
        }
        public Statistics(bool bit16)
        {
            if (bit16)
            {
                meanvalues = new double[ushort.MaxValue+1];
                values = new int[ushort.MaxValue+1];
                bitsPerPixel = 16;
            }
            else
            {
                meanvalues = new double[256];
                values = new int[256];
                bitsPerPixel = 8;
            }
        }
        public static Statistics FromBytes(byte[] bts, int w, int h, int rGBChannels, int BitsPerPixel, int stride)
        {
            Statistics st;
            if (BitsPerPixel > 8)
                st = new Statistics(true);
            else
                st = new Statistics(false);
            st.max = ushort.MinValue;
            st.min = ushort.MaxValue;
            st.bitsPerPixel = BitsPerPixel;
            if (BitsPerPixel > 8)
            {
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w * rGBChannels; x += 2)
                    {
                        ushort s = BitConverter.ToUInt16(bts, (y * stride) + x);
                        if (st.max < s)
                            st.max = s;
                        if (st.min > s)
                            st.min = s;
                        st.values[s]++;
                    }
                }
            }
            else
            {
                for (int i = 0; i < bts.Length; i++)
                {
                    byte s = bts[i];
                    if (st.max < s)
                        st.max = s;
                    if (st.min > s)
                        st.min = s;
                    st.values[s]++;
                }
            }
            st.mean = (st.min + st.max) / 2;
            return st;
        }
        public void AddStatistics(Statistics s)
        {
            if (max < s.max)
                max = s.max;
            if (min > s.min)
                min = s.min;
            meansum += s.mean;
            for (int i = 0; i < s.meanvalues.Length; i++)
            {
                meanvalues[i] += s.values[i];
            }
            count++;
        }
        public void MeanHistogram()
        {
            for (int i = 0; i < meanvalues.Length; i++)
            {
                meanvalues[i] /= (double)count;
                if (median < meanvalues[i])
                    median = meanvalues[i];
            }
        }
    }
    public class SizeInfo
    {
        bool HasPhysicalXY = false;
        bool HasPhysicalXYZ = false;
        private double physicalSizeX = -1;
        private double physicalSizeY = -1;
        private double physicalSizeZ = -1;
        public double PhysicalSizeX
        {
            get { return physicalSizeX; }
            set
            {
                physicalSizeX = value;
                HasPhysicalXY = true;
            }
        }
        public double PhysicalSizeY
        {
            get { return physicalSizeY; }
            set
            {
                physicalSizeY = value;
                HasPhysicalXY = true;
            }
        }
        public double PhysicalSizeZ
        {
            get { return physicalSizeZ; }
            set
            {
                physicalSizeZ = value;
                HasPhysicalXYZ = true;
            }
        }

        bool HasStageXY = false;
        bool HasStageXYZ = false;
        public double stageSizeX = -1;
        public double stageSizeY = -1;
        public double stageSizeZ = -1;
        public double StageSizeX
        {
            get { return stageSizeX; }
            set
            {
                stageSizeX = value;
                HasStageXY = true;
            }
        }
        public double StageSizeY
        {
            get { return stageSizeY; }
            set
            {
                stageSizeY = value;
                HasStageXY = true;
            }
        }
        public double StageSizeZ
        {
            get { return stageSizeZ; }
            set
            {
                stageSizeZ = value;
                HasStageXYZ = true;
            }
        }

        public SizeInfo Copy()
        {
            SizeInfo inf = new SizeInfo();
            inf.PhysicalSizeX = PhysicalSizeX;
            inf.PhysicalSizeY = PhysicalSizeY;
            inf.PhysicalSizeZ = PhysicalSizeZ;
            inf.StageSizeX = StageSizeX;
            inf.StageSizeY = StageSizeY;
            inf.StageSizeZ = StageSizeZ;
            inf.HasPhysicalXY = HasPhysicalXY;
            inf.HasPhysicalXYZ = HasPhysicalXYZ;
            inf.StageSizeX = StageSizeX;
            inf.StageSizeY = StageSizeY;
            inf.StageSizeZ = StageSizeZ;
            inf.HasStageXY = HasStageXY;
            inf.HasStageXYZ = HasStageXYZ;
            return inf;
        }

    }
    public class Worker
    {
        private Thread thread;
        public static void Save()
        {
            foreach (string file in files)
            {
                if (file.EndsWith("ome.tif"))
                {
                    BioImage.WriteOME(file);
                }
                else
                if (file.EndsWith("tif") || file.EndsWith("tiff") || file.EndsWith("png") || file.EndsWith("bmp") || file.EndsWith("jpg") || file.EndsWith("jpeg") || file.EndsWith("gif") ||
                file.EndsWith("TIF") || file.EndsWith("TIFF") || file.EndsWith("PNG") || file.EndsWith("BMP") || file.EndsWith("JPG") || file.EndsWith("JPEG") || file.EndsWith("GIF"))
                {
                    BioImage.Write(file);
                }
                else
                {
                    BioImage.WriteOME(file);
                }
                Table.RemoveImage(file);
            }
                
        }
        public static void Open()
        {
            foreach (string file in files)
            {
                if (file.EndsWith("ome.tif"))
                {
                    Table.AddImage(BioImage.ReadOME(file));
                }
                else
                if (file.EndsWith("tif") || file.EndsWith("tiff") || file.EndsWith("png") || file.EndsWith("bmp") || file.EndsWith("jpg") || file.EndsWith("jpeg") || file.EndsWith("gif") ||
                file.EndsWith("TIF") || file.EndsWith("TIFF") || file.EndsWith("PNG") || file.EndsWith("BMP") || file.EndsWith("JPG") || file.EndsWith("JPEG") || file.EndsWith("GIF"))
                {
                    Table.AddImage(BioImage.Read(file));
                }
                else
                {
                    Table.AddImage(BioImage.ReadOME(file));
                }
            }
            
        }
        public static List<string> files = new List<string>();
        public Worker(string[] sts, bool open)
        {
            if(open)
                thread = new Thread(Open);
            else
                thread = new Thread(Save);
            files.AddRange(sts);
            thread.Start();
        }
    }
    public class BioImage : IDisposable
    {
        public int[,,] Coords;
        private string id;
        public List<Channel> Channels = new List<Channel>();
        public List<BufferInfo> Buffers = new List<BufferInfo>();
        public List<VolumeD> Volumes = new List<VolumeD>();
        public List<Annotation> Annotations = new List<Annotation>();

        public string filename = "";
        public string Filename
        {
            get
            {
                return Path.GetFileName(id);
            }
            set
            {
                filename = value;
            }
        }
        public int[] rgbChannels = new int[3];
        public int RGBChannelCount
        {
            get
            {
                return Buffers[0].RGBChannelsCount;
            }
        }
        public int bitsPerPixel;
        public int series = 0;
        public int imagesPerSeries = 0;
        public int seriesCount = 1;
        public double frameInterval = 0;
        public bool littleEndian = false;
        public bool isGroup = false;
        public long loadTimeMS = 0;
        public long loadTimeTicks = 0;
        public bool selected = false;
        private int sizeZ, sizeC, sizeT;
        private Bitmap rgbBitmap8 = null;
        private Bitmap rgbBitmap16 = null;
        
        SizeInfo sizeInfo = new SizeInfo();
        public static BioImage Copy(BioImage b)
        {
            BioImage bi = new BioImage(b.ID);
            foreach (Annotation an in b.Annotations)
            {
                bi.Annotations.Add(an);
            }
            foreach (BufferInfo bf in b.Buffers)
            {
                bi.Buffers.Add(bf.Copy());
            }
            foreach (Channel c in b.Channels)
            {
                bi.Channels.Add(c);
            }
            foreach (VolumeD vol in b.Volumes)
            {
                bi.Volumes.Add(vol);
            }
            bi.Coords = b.Coords;
            bi.sizeZ = b.sizeZ;
            bi.sizeC = b.sizeC;
            bi.sizeT = b.sizeT;
            bi.series = b.series;
            bi.seriesCount = b.seriesCount;
            bi.frameInterval = b.frameInterval;
            bi.littleEndian = b.littleEndian;
            bi.isGroup = b.isGroup;
            bi.sizeInfo = b.sizeInfo;
            bi.bitsPerPixel = b.bitsPerPixel;
            bi.rgbBitmap16 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            bi.rgbBitmap8 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            return bi;
        }
        public BioImage Copy()
        {
            return BioImage.Copy(this);
        }
        public static BioImage CopyInfo(BioImage b)
        {
            BioImage bi = new BioImage(b.ID);
            foreach (Annotation an in b.Annotations)
            {
                bi.Annotations.Add(an);
            }

            foreach (Channel c in b.Channels)
            {
                bi.Channels.Add(c.Copy());
            }

            bi.Coords = b.Coords;
            bi.sizeZ = b.sizeZ;
            bi.sizeC = b.sizeC;
            bi.sizeT = b.sizeT;
            bi.series = b.series;
            bi.seriesCount = b.seriesCount;
            bi.frameInterval = b.frameInterval;
            bi.littleEndian = b.littleEndian;
            bi.isGroup = b.isGroup;
            bi.sizeInfo = b.sizeInfo;
            bi.bitsPerPixel = b.bitsPerPixel;
            bi.rgbBitmap16 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            bi.rgbBitmap8 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            return bi;
        }
        public string ID
        {
            get { return id; }
            set
            {
                id = value;
            }
        }
        public int ImageCount
        {
            get
            {
                return Buffers.Count;
            }
        }
        public double physicalSizeX
        {
            get { return sizeInfo.PhysicalSizeX; }
            set { sizeInfo.PhysicalSizeX = value; }
        }
        public double physicalSizeY
        {
            get { return sizeInfo.PhysicalSizeY; }
            set { sizeInfo.PhysicalSizeY = value; }
        }
        public double physicalSizeZ
        {
            get { return sizeInfo.PhysicalSizeZ; }
            set { sizeInfo.PhysicalSizeZ = value; }
        }
        public double stageSizeX
        {
            get { return sizeInfo.StageSizeX; }
            set { sizeInfo.StageSizeX = value; }
        }
        public double stageSizeY
        {
            get { return sizeInfo.StageSizeY; }
            set { sizeInfo.StageSizeY = value; }
        }
        public double stageSizeZ
        {
            get { return sizeInfo.StageSizeZ; }
            set { sizeInfo.StageSizeZ = value; }
        }
        static bool initialized = false;
        public static bool Initialized
        {
            get
            {
                return initialized;
            }
        }
        public void To8Bit()
        {
            if (bitsPerPixel == 8)
                return;
            for (int i = 0; i < Buffers.Count; i++)
            {
                Bitmap b = GetFiltered(i, RChannel.range, GChannel.range, BChannel.range);
                b = AForge.Imaging.Image.Convert16bppTo8bpp(b);
                Buffers[i].Image = b;
            }
            foreach (Channel c in Channels)
            {
                c.Max = 255;
            }

            bitsPerPixel = 8;
            AutoThreshold();
            Recorder.AddLine("To8Bit();");
        }
        public void To16Bit()
        {
            if (bitsPerPixel > 8)
                return;
            for (int i = 0; i < Buffers.Count; i++)
            {
                Bitmap b = GetFiltered(i, RChannel.range, GChannel.range, BChannel.range);
                b = AForge.Imaging.Image.Convert8bppTo16bpp(b);
                Buffers[i].Image = b;
            }
            foreach (Channel c in Channels)
            {
                c.Max = ushort.MaxValue;
            }
            bitsPerPixel = 16;
            AutoThreshold();
            Recorder.AddLine("To16Bit();");
        }
        public void To24Bit()
        {
            if (SizeC == 1)
            {
                for (int i = 0; i < Buffers.Count; i++)
                {
                    Buffers[i].Image = BufferInfo.RGBTo24Bit((Bitmap)Buffers[i].Image);
                }
            }
            else
            {
                if (SizeC != 3)
                {
                    MessageBox.Show("24 bit RGB conversion requires an image with 3, 8 bit channels. Use stack tools to create 3 channel image.");
                }
                BioImage bi = CopyInfo(this);
                bi.sizeC = 1;
                int index = 0;
                bi.Coords = new int[sizeZ, 1, SizeT];
                for (int i = 0; i < Buffers.Count; i += 3)
                {
                    Bitmap b = GetRGBBitmap(i, RChannel.range, GChannel.range, BChannel.range);
                    if (bitsPerPixel > 8)
                        b = AForge.Imaging.Image.Convert16bppTo8bpp(b);
                    BufferInfo bf = new BufferInfo(Table.GetImageName(ID), b, Buffers[i].Coordinate, index);
                    bi.Buffers.Add(bf);
                    bi.Coords[Buffers[i].Coordinate.Z, 0, Buffers[i].Coordinate.T] = index;
                    index++;
                }
                foreach (Channel c in bi.Channels)
                {
                    c.Max = 255;
                }
                bi.bitsPerPixel = 8;
            }
            //Table.AddViewer(bi);
            Recorder.AddLine("To24Bit();");
        }
        public void To32Bit()
        {
            if (bitsPerPixel > 8)
                return;
            for (int i = 0; i < Buffers.Count; i++)
            {
                Bitmap b = BufferInfo.RGBTo32Bit((Bitmap)Buffers[i].Image);
                Buffers[i].Image = b;
            }
            Recorder.AddLine("To32Bit();");
        }
        public void To48Bit()
        {
            if (SizeC != 3)
            {
                MessageBox.Show("48 bit RGB conversion requires an image with 3, 16 bit channels. Use stack tools to create 3 channel image.");
            }
            BioImage bi = CopyInfo(this);
            bi.sizeC = 1;
            int index = 0;
            bi.Coords = new int[SizeZ, 3, SizeT];
            for (int i = 0; i < Buffers.Count; i += 3)
            {
                Bitmap b = GetRGBBitmap(i, RChannel.range, GChannel.range, BChannel.range);
                BufferInfo bf = new BufferInfo(Table.GetImageName(ID), b, Buffers[i].Coordinate, index);
                bi.Buffers.Add(bf);
                bi.Coords[Buffers[i].Coordinate.Z, Buffers[i].Coordinate.C, Buffers[i].Coordinate.T] = index;
                index++;
            }
            /*
            foreach (Channel c in bi.Channels)
            {
                c.Max = 255;
            }
            */
            bi.bitsPerPixel = 16;
            AutoThreshold();
            Table.AddImage(bi);
            Recorder.AddLine("To48Bit();");

        }
        public BioImage(string id)
        {
            ID = Table.GetImageName(id);
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
        }
        public BioImage(string id, int SizeX, int SizeY)
        {
            ID = Table.GetImageName(id);
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
            rgbBitmap16 = new Bitmap(SizeX, SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            rgbBitmap8 = new Bitmap(SizeX, SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Table.AddImage(this);
        }
        public BioImage(BioImage orig, int ser, int zs, int ze, int cs, int ce, int ts, int te)
        {
            ID = Table.GetImageName(orig.ID);
            serie = ser;
            sizeZ = (int)ze - zs;
            sizeC = (int)ce - cs;
            sizeT = (int)te - ts;
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
            bitsPerPixel = orig.bitsPerPixel;

            sizeInfo = orig.sizeInfo;
            littleEndian = orig.littleEndian;
            seriesCount = orig.seriesCount;
            imagesPerSeries = ImageCount / seriesCount;
            Coords = new int[SizeZ, SizeC, SizeT];

            fileHashTable.Add(Filename, Filename.GetHashCode());
            int i = 0;
            for (int ti = 0; ti < SizeT; ti++)
            {
                for (int zi = 0; zi < SizeZ; zi++)
                {
                    for (int ci = 0; ci < SizeC; ci++)
                    {
                        int ind = orig.Coords[zs + zi, cs + ci, ts + ti];
                        Buffers.Add(new BufferInfo(Table.GetImageName(orig.id),orig.SizeX, orig.SizeY, orig.Buffers[0].PixelFormat,orig.Buffers[ind].Bytes,new ZCT(zi, ci, ti),i));
                        Coords[zi, ci, ti] = i;
                        //Lets copy the ROI's from the original image.
                        List<Annotation> anns = orig.GetAnnotations(zs + zi, cs + ci, ts + ti);
                        if (anns.Count > 0)
                            Annotations.AddRange(anns);
                        i++;
                    }
                }
            }
            for (int ci = 0; ci < SizeC; ci++)
            {
                Channels.Add(orig.Channels[ci].Copy());
            }
            Table.AddImage(this);
            rgbBitmap16 = new Bitmap(SizeX, SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            rgbBitmap8 = new Bitmap(SizeX, SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Recorder.AddLine("BioImage.Substack(" + '"' + orig.Filename + '"' + "," + ser + "," + "," + zs + "," + ze + "," + cs + "," + ce + "," + ts + "," + te + ");");
        }
        public BioImage(string file, int ser)
        {
            //threadImage = this;
            ID = Table.GetImageName(file);
            serie = ser;
            /*
            if (file.EndsWith("ome.tif"))
            {
                BioImage.OpenOME(file,ser);
            }
            else
            if (file.EndsWith("tif") || file.EndsWith("tiff") || file.EndsWith("png") || file.EndsWith("bmp") || file.EndsWith("jpg") || file.EndsWith("jpeg") || file.EndsWith("gif") ||
                file.EndsWith("TIF") || file.EndsWith("TIFF") || file.EndsWith("PNG") || file.EndsWith("BMP") || file.EndsWith("JPG") || file.EndsWith("JPEG") || file.EndsWith("GIF"))
            {
                BioImage.Open(file);
            }
            else
            {
                BioImage.OpenOME(file,ser);
            }
            */
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
        }
        public static BioImage MergeChannels(BioImage b2, BioImage b)
        {         
            BioImage res = new BioImage(b2.ID, b2.SizeX, b2.SizeY);
            res.ID = Table.GetImageName(b2.ID);
            res.series = b2.series;
            res.sizeZ = b2.SizeZ;
            int cOrig = b2.SizeC;
            res.sizeC = b2.SizeC + b.SizeC;
            res.sizeT = b2.SizeT;
            res.bitsPerPixel = b2.bitsPerPixel;

            res.sizeInfo = b2.sizeInfo;

            //res.imageCount = res.SizeZ * res.SizeC * res.SizeT;
            res.littleEndian = b2.littleEndian;
            res.seriesCount = b2.seriesCount;

            res.imagesPerSeries = res.ImageCount / res.seriesCount;
            res.Coords = new int[res.SizeZ, res.SizeC, res.SizeT];
            //res.IdString = Path.GetFileNameWithoutExtension(b2.filename) + "-1" + Path.GetExtension();

            res.fileHashTable.Add(res.Filename, res.Filename.GetHashCode());
            int i = 0;
            int cc = 0;
            for (int ti = 0; ti < res.SizeT; ti++)
            {
                for (int zi = 0; zi < res.SizeZ; zi++)
                {
                    for (int ci = 0; ci < res.SizeC; ci++)
                    {
                        ZCT co = new ZCT(zi, ci, ti);
                        if (ci < cOrig)
                        {
                            //If this channel is part of the image b1 we add planes from it.
                            BufferInfo copy = new BufferInfo(b2.id,b2.SizeX,b2.SizeY,b2.Buffers[0].PixelFormat, b2.Buffers[i].Bytes,co, i);
                            res.Coords[zi, ci, ti] = i;
                            res.Buffers.Add(b2.Buffers[i]);
                            res.Buffers.Add(copy);
                            //Lets copy the ROI's from the original image.
                            List<Annotation> anns = b2.GetAnnotations(zi, ci, ti);
                            if (anns.Count > 0)
                                res.Annotations.AddRange(anns);
                        }
                        else
                        {
                            //This plane is not part of b1 so we add the planes from b2 channels.
                            BufferInfo copy = new BufferInfo(b.id, b.SizeX, b.SizeY, b.Buffers[0].PixelFormat, b.Buffers[i].Bytes, co, i);
                            res.Coords[zi, ci, ti] = i;
                            res.Buffers.Add(b.Buffers[i]);
                            res.Buffers.Add(copy);

                            //Lets copy the ROI's from the original image.
                            List<Annotation> anns = b.GetAnnotations(zi, cc, ti);
                            if (anns.Count > 0)
                                res.Annotations.AddRange(anns);
                        }
                        i++;
                    }
                }
            }
            for (int ci = 0; ci < res.SizeC; ci++)
            {
                if (ci < cOrig)
                {
                    res.Channels.Add(b2.Channels[ci].Copy());
                }
                else
                {
                    res.Channels.Add(b.Channels[cc].Copy());
                    res.Channels[cOrig + cc].Index = ci;
                    cc++;
                }
            }
            Table.AddImage(res);
            Recorder.AddLine("MergeChannels(" + '"' + b.ID + '"' + "," + '"' + b2.ID + '"' + ");");
            return res;
        }
        public static BioImage MergeChannels(string bname, string b2name)
        {
            BioImage b = Table.GetImage(bname);
            BioImage b2 = Table.GetImage(b2name);
            return MergeChannels(b, b2);
        }
        public BioImage Substack(int ser, int zs, int ze, int cs, int ce, int ts, int te)
        {
            return new BioImage(this, ser, zs, ze, cs, ce, ts, te);
        }
        public static BioImage Substack(BioImage orig, int ser, int zs, int ze, int cs, int ce, int ts, int te)
        {
            return new BioImage(orig, ser, zs, ze, cs, ce, ts, te);
        }
        public void SplitChannels()
        {
            for (int c = 0; c < SizeC; c++)
            {
                BioImage b = new BioImage(this, 0, 0, SizeZ, c, c + 1, 0, SizeT);
                Table.AddImage(b);
            }
            Recorder.AddLine("BioImage.SplitChannels(" + '"' + Filename + '"' + ");");
        }
        public static void SplitChannels(BioImage bb)
        {
            bb.SplitChannels();
        }
        public static void SplitChannels(string name)
        {
            SplitChannels(Table.GetImage(name));
        }
        public Channel RChannel
        {
            get
            {
                return Channels[rgbChannels[0]];
            }
        }
        public Channel GChannel
        {
            get
            {
                return Channels[rgbChannels[1]];
            }
        }
        public Channel BChannel
        {
            get
            {
                return Channels[rgbChannels[2]];
            }
        }

        public class ImageJDesc
        {
            public string ImageJ;
            public int images = 0;
            public int channels = 0;
            public int slices = 0;
            public int frames = 0;
            public bool hyperstack;
            public string mode;
            public string unit;
            public double finterval = 0;
            public double spacing = 0;
            public bool loop;
            public double min = 0;
            public double max = 0;
            public int count;
            public bool bit8color = false;

            public ImageJDesc FromImage(BioImage b)
            {
                ImageJ = "";
                images = b.ImageCount;
                channels = b.SizeC;
                slices = b.SizeZ;
                frames = b.SizeT;
                hyperstack = true;
                mode = "grayscale";
                unit = "micron";
                finterval = b.frameInterval;
                spacing = b.physicalSizeZ;
                loop = false;
                /*
                double dmax = double.MinValue;
                double dmin = double.MaxValue;
                foreach (Channel c in b.Channels)
                {
                    if(dmax < c.Max)
                        dmax = c.Max;
                    if(dmin > c.Min)
                        dmin = c.Min;
                }
                min = dmin;
                max = dmax;
                */
                min = b.Channels[0].Min;
                max = b.Channels[0].Max;
                return this;
            }
            public string GetString()
            {
                string s = "";
                s += "ImageJ=" + ImageJ + "\n";
                s += "images=" + images + "\n";
                s += "channels=" + channels.ToString() + "\n";
                s += "slices=" + slices.ToString() + "\n";
                s += "frames=" + frames.ToString() + "\n";
                s += "hyperstack=" + hyperstack.ToString() + "\n";
                s += "mode=" + mode.ToString() + "\n";
                s += "unit=" + unit.ToString() + "\n";
                s += "finterval=" + finterval.ToString() + "\n";
                s += "spacing=" + spacing.ToString() + "\n";
                s += "loop=" + loop.ToString() + "\n";
                s += "min=" + min.ToString() + "\n";
                s += "max=" + max.ToString() + "\n";
                return s;
            }
            public void SetString(string desc)
            {
                string[] lines = desc.Split(new string[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
                int maxlen = 13;
                for (int i = 0; i < lines.Length; i++)
                {
                    if (i < maxlen)
                    {
                        string[] sp = lines[i].Split('=');
                        if (sp[0] == "ImageJ")
                            ImageJ = sp[1];
                        if (sp[0] == "images")
                            images = int.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "channels")
                            channels = int.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "slices")
                            slices = int.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "frames")
                            frames = int.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "hyperstack")
                            hyperstack = bool.Parse(sp[1]);
                        if (sp[0] == "mode")
                            mode = sp[1];
                        if (sp[0] == "unit")
                            unit = sp[1];
                        if (sp[0] == "finterval")
                            finterval = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "spacing")
                            spacing = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "loop")
                            loop = bool.Parse(sp[1]);
                        if (sp[0] == "min")
                            min = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "max")
                            max = double.Parse(sp[1], CultureInfo.InvariantCulture);
                        if (sp[0] == "8bitcolor")
                            bit8color = bool.Parse(sp[1]);
                    }
                    else
                        return;
                }

            }
        }

        public int SizeX
        {
            get 
            {
                if (Buffers.Count > 0)
                    return Buffers[0].SizeX;
                else return 0;
            }
        }
        public int SizeY
        {
            get
            {
                if (Buffers.Count > 0)
                    return Buffers[0].SizeY;
                else return 0;
            }
        }
        public int SizeZ
        {
            get { return sizeZ; }
        }
        public int SizeC
        {
            get { return sizeC; }
        }
        public int SizeT
        {
            get { return sizeT; }
        }

        public Stopwatch watch = new Stopwatch();
        public bool isRGB
        {
            get
            {
                if (RGBChannelCount == 3 || RGBChannelCount == 4)
                    return true;
                else
                    return false;
            }
        }
        public bool isTime
        {
            get
            {
                if (SizeT > 1)
                    return true;
                else
                    return false;
            }
        }

        public static LevelsLinear filter8 = new LevelsLinear();
        public static LevelsLinear16bpp filter16 = new LevelsLinear16bpp();
        private ReplaceChannel replaceRFilter;
        private ReplaceChannel replaceGFilter;
        private ReplaceChannel replaceBFilter;
        private static ExtractChannel extractR = new ExtractChannel(AForge.Imaging.RGB.R);
        private static ExtractChannel extractG = new ExtractChannel(AForge.Imaging.RGB.G);
        private static ExtractChannel extractB = new ExtractChannel(AForge.Imaging.RGB.B);

        public Image GetImageByCoord(int z, int c, int t)
        {
            return Buffers[Coords[z, c, t]].Image;
        }
        public Bitmap GetBitmap( int z, int c, int t)
        {
            return (Bitmap)Buffers[Coords[z, c, t]].Image;
        }
        public int GetIndex(int ix, int iy)
        {
            int stridex = SizeX;
            int x = ix;
            int y = iy;
            if (bitsPerPixel > 8)
            {
                return (y * stridex + x) * 2;
            }
            else
            {
                return (y * stridex + x);
            }
        }
        public int GetIndexRGB(int ix, int iy, int index)
        {
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (bitsPerPixel > 8)
            {
                return (y * stridex + x) * 2 * index;
            }
            else
            {
                return (y * stridex + x) * index;
            }
        }
        public ushort GetValue(ZCTXY coord)
        {
            if(coord.X < 0 || coord.Y < 0 || coord.X > SizeX || coord.Y > SizeY)
            {
                return 0;
            }
            if (isRGB)
            {
                if (coord.C == 0)
                    return GetValueRGB(coord, 0);
                else if (coord.C == 1)
                    return GetValueRGB(coord, 1);
                else if (coord.C == 2)
                    return GetValueRGB(coord, 2);
            }
            else
                return GetValueRGB(coord, 0);
            return 0;
        }
        public ushort GetValueRGB(ZCTXY coord, int index)
        {
            int i = -1;
            int ind = Coords[coord.Z, coord.C, coord.T];
            byte[] bytes = Buffers[ind].Bytes;
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = coord.X;
            int y = coord.Y;
            if (bitsPerPixel > 8)
            {
                int index2 = (y * stridex + x) * 2 * index;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                
                int stride = SizeX;
                System.Drawing.Color c = ((Bitmap)Buffers[ind].Image).GetPixel(x, y);
                if (index == 0)
                    return c.R;
                else
                if (index == 1)
                    return c.G;
                else
                if (index == 2)
                    return c.B;
                else
                    return c.A;
            }
        }
        public ushort GetValue(ZCT coord,int ix, int iy)
        {
            int ind = Coords[coord.Z, coord.C, coord.T];
            byte[] bytes = Buffers[ind].Bytes;
            int i = 0;
            int stridex = SizeX;
            
            int x = ix;
            int y = iy;
            if (ix < 0)
                x = 0;
            if (iy < 0)
                y = 0;
            if (ix >= SizeX)
                x = SizeX - 1;
            if (iy >= SizeY)
                y = SizeY - 1;
            if (bitsPerPixel > 8)
            {
                //For 16bit (2*8bit) images we multiply buffer index by 2
                int index2 = (y * stridex + x) * 2 * RGBChannelCount;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                int stride = SizeX;
                System.Drawing.Color c = ((Bitmap)Buffers[ind].Image).GetPixel(ix, iy);
                return c.R;
            }
        }
        public ushort GetValue( int z, int c, int t, int x, int y)
        {
            return GetValue(new ZCTXY(z, c, t, x, y));
        }
        public ushort GetValueRGB(ZCT coord, int x, int y, int RGBindex)
        {
            ZCTXY c = new ZCTXY(coord.Z, coord.C, coord.T, x, y);
            if (isRGB)
            { 
                return GetValueRGB(c, RGBindex);
            }
            else
                return GetValue(coord,x,y);
        }
        public ushort GetValueRGB( int z, int c, int t, int x, int y, int RGBindex)
        {
            return GetValueRGB(new ZCT(z,c,t),x, y, RGBindex);
        }
        public void SetValue(ZCTXY coord, ushort value)
        {
            int i = Coords[coord.Z,coord.C,coord.T];
            Buffers[i].SetValue(coord.X, coord.Y,value);
        }
        public void SetValue(int x, int y, int ind, ushort value)
        {
            Buffers[ind].SetValue(x, y, value);
        }
        public void SetValue(int x, int y, ZCT coord, ushort value)
        {
            SetValue(x, y, Coords[coord.Z, coord.C, coord.T], value);
        }
        public void SetValueRGB(ZCTXY coord, int RGBindex, ushort value)
        {
            int i = -1;
            int ind = Coords[coord.Z, coord.C, coord.T];
            Buffers[ind].SetValueRGB(coord.X, coord.Y, RGBindex, value);
        }
        public Bitmap GetBitmap(ZCT coord)
        {
            return (Bitmap)GetImageByCoord(coord.Z, coord.C, coord.T);
        }
        public Bitmap GetFiltered(ZCT coord, IntRange r, IntRange g, IntRange b)
        {
            int index = Coords[coord.Z, coord.C, coord.T];
            return GetFiltered(index, r, g, b);
        }
        public Bitmap GetFiltered(int ind, IntRange r, IntRange g, IntRange b)
        {
            if (bitsPerPixel > 8)
            {
                BioImage.filter16.InRed = r;
                BioImage.filter16.InGreen = g;
                BioImage.filter16.InBlue = b;
                return BioImage.filter16.Apply((Bitmap)Buffers[ind].Image);
            }
            else
            {
                // set ranges
                BioImage.filter8.InRed = r;
                BioImage.filter8.InGreen = g;
                BioImage.filter8.InBlue = b;
                return BioImage.filter8.Apply((Bitmap)Buffers[ind].Image);
            }
        }
        public Bitmap GetChannelImage(int ind, RGB rGB)
        {
            BufferInfo bf = Buffers[ind];
            if (bf.isRGB)
            {
                if (rGB == RGB.R)
                    return extractR.Apply((Bitmap)Buffers[ind].Image);
                else
                if (rGB == RGB.G)
                    return extractG.Apply((Bitmap)Buffers[ind].Image);
                else
                    return extractB.Apply((Bitmap)Buffers[ind].Image);
            }
            else
                throw new InvalidOperationException();
        }
        public Bitmap GetRGBBitmap(int index, IntRange rf, IntRange gf, IntRange bf)
        {
            if (bitsPerPixel > 8)
                return GetRGBBitmap16(index, rf, gf, bf);
            else
                return GetRGBBitmap8(index);
        }
        public Bitmap GetRGBBitmap(int index, IntRange rf, IntRange gf, IntRange bf, RGB rgb)
        {
            if (bitsPerPixel > 8)
                return GetRGBBitmap16(index, rf, gf, bf);
            else
                return GetRGBBitmap8(index);
        }
        public Bitmap GetRGBBitmap(ZCT coord, IntRange rf, IntRange gf, IntRange bf)
        {
            int index = Coords[coord.Z, coord.C, coord.T];
            if (bitsPerPixel > 8)
                return GetRGBBitmap16(index, rf, gf, bf);
            else
                return GetRGBBitmap8(index);
        }
        public Bitmap GetRGBBitmap16(int ri, IntRange rf, IntRange gf, IntRange bf)
        {
            watch.Restart();
            if (replaceRFilter == null || replaceGFilter == null || replaceBFilter == null)
            {
                //if (SizeC > 0)
                    replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, GetFiltered(ri + RChannel.Index, rf, gf, bf));
                //if (SizeC > 1)
                    replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, GetFiltered(ri + GChannel.Index, rf, gf, bf));
                //if (SizeC > 2)
                    replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, GetFiltered(ri + BChannel.Index, rf, gf, bf));
            }
            if (rgbBitmap16.Width != SizeX || rgbBitmap16.Height != SizeY)
                rgbBitmap16 = new Bitmap(SizeX, SizeY, PixelFormat.Format48bppRgb);
            if (RGBChannelCount == 1)
            {
                //if (SizeC > 0)
                //{
                    replaceRFilter.ChannelImage = GetFiltered(ri + RChannel.Index, rf, gf, bf);
                    replaceRFilter.ApplyInPlace(rgbBitmap16);
                    replaceRFilter.ChannelImage.Dispose();
                //}
                //if (SizeC > 1)
                //{
                    replaceGFilter.ChannelImage = GetFiltered(ri + GChannel.Index, gf, gf, bf);
                    replaceGFilter.ApplyInPlace(rgbBitmap16);
                    replaceGFilter.ChannelImage.Dispose();
                //}
                //if (SizeC > 2)
                //{
                    replaceBFilter.ChannelImage = GetFiltered(ri + BChannel.Index, bf, gf, bf);
                    replaceBFilter.ApplyInPlace(rgbBitmap16);
                    replaceBFilter.ChannelImage.Dispose();
                //}
            }
            else
            {
                rgbBitmap16 = (Bitmap)Buffers[ri].Image;
            }
            watch.Stop();
            loadTimeMS = watch.ElapsedMilliseconds;
            loadTimeTicks = watch.ElapsedTicks;
            return rgbBitmap16;
        }
        public Bitmap GetRGBBitmap8(int ri)
        {
            watch.Restart();
            if (rgbBitmap8.Width != SizeX || rgbBitmap8.Height != SizeY)
                rgbBitmap8 = new Bitmap(SizeX, SizeY, PixelFormat.Format24bppRgb);
            if (RGBChannelCount == 1)
            {
                if (replaceRFilter == null || replaceGFilter == null || replaceBFilter == null)
                {
                    if(SizeC > 0)
                    replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, (Bitmap)Buffers[ri + RChannel.Index].Image);
                    if(SizeC > 1)
                    replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, (Bitmap)Buffers[ri + GChannel.Index].Image);
                    if(SizeC > 2)
                    replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, (Bitmap)Buffers[ri + BChannel.Index].Image);
                }
                if (SizeC > 0)
                {
                    replaceRFilter.ChannelImage = (Bitmap)Buffers[ri + RChannel.Index].Image;
                    replaceRFilter.ApplyInPlace(rgbBitmap8);
                }
                if (SizeC > 1)
                {
                    replaceGFilter.ChannelImage = (Bitmap)Buffers[ri + GChannel.Index].Image;
                    replaceGFilter.ApplyInPlace(rgbBitmap8);
                }
                if (SizeC > 2)
                {
                    replaceBFilter.ChannelImage = (Bitmap)Buffers[ri + BChannel.Index].Image;
                    replaceBFilter.ApplyInPlace(rgbBitmap8);
                }
            }
            else
            {
                rgbBitmap8 = (Bitmap)Buffers[ri].Image;
            }
            watch.Stop();
            loadTimeMS = watch.ElapsedMilliseconds;
            loadTimeTicks = watch.ElapsedTicks;
            return rgbBitmap8;
        }

        public static Stopwatch swatch = new Stopwatch();
        public List<Annotation> GetAnnotations(ZCT coord)
        {
            List<Annotation> annotations = new List<Annotation>();
            foreach (Annotation an in Annotations)
            {
                if (an.coord == coord)
                    annotations.Add(an);
            }
            return annotations;
        }
        public List<Annotation> GetAnnotations( int Z, int C, int T)
        {
            List<Annotation> annotations = new List<Annotation>();
            foreach (Annotation an in Annotations)
            {
                if (an.coord.Z == Z && an.coord.Z == Z && an.coord.C == C && an.coord.T == T)
                    annotations.Add(an);
            }
            return annotations;
        }
 
        private static ImageWriter wr;
        private static int serie;
        private static bool done = false;
        public static float threadProgress = 0;
        /*
        public void Save(string file)
        {
            //This is the default saving mode we save the roi's in CSV and save tiff fast with BitMiracle.
            threadImage = this;
            threadFile = file;
            threadProgress = 0;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Save));
            t.Start();
            Progress pr = new Progress(threadFile, "Saving");
            pr.Show();
            done = false;
            do
            {
                pr.UpdateProgressF(threadProgress);
                Application.DoEvents();
            } while (!done);
            pr.Close();
        }
        public void Open(string file)
        {
            //This is the default opening mode we load the roi's in CSV and open tiff fast with BitMiracle.
            threadImage = this;
            threadFile = file;
            threadProgress = 0;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Open));
            t.Start();
            Progress pr = new Progress(threadFile, "Opening");
            pr.Show();
            done = false;
            do
            {
                pr.UpdateProgressF(threadProgress);
                Application.DoEvents();
            } while (!done);

            pr.Close();

        }
        
        public void SaveOME(string file, int series)
        {
            threadImage = this;
            threadFile = file;
            threadProgress = 0;
            serie = series;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(SaveOME));
            t.Start();
            Progress pr = new Progress(threadFile, "Saving");
            pr.Show();
            done = false;
            do
            {
                pr.UpdateProgressF(threadProgress);
                Application.DoEvents();
            } while (!done);
            pr.Close();
        }
        public void OpenOME(string file, int series)
        {
            //This is the default opening mode we load the roi's in CSV and open tiff fast with BitMiracle.
            threadImage = this;
            threadFile = file;
            threadProgress = 0;
            serie = series;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(OpenOME));
            t.Start();
            Progress pr = new Progress(threadFile, "Opening");
            pr.Show();
            done = false;
            do
            {
                pr.UpdateProgressF(threadProgress);
                Application.DoEvents();
            } while (!done);
            t.Abort();
            pr.Close();
        }
        */
        public bool Loading = false;
        public static void AddToSavePool(string[] files)
        {
            Worker w = new Worker(files, false);
        }
        public static void AddToOpenPool(string[] files)
        {
            Worker w = new Worker(files, true);
        }
        /*
        public static void SaveOME(string file, int series)
        {
            BioImage b = Table.GetImage(file);
            //threadFile = file;
            threadProgress = 0;
            serie = series;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(WriteOME));
            
            t.Start();
            Progress pr = new Progress(file, "Saving");
            pr.Show();
            done = false;
            do
            {
                pr.UpdateProgressF(threadProgress);
                Application.DoEvents();
            } while (!done);
            pr.Close();
        }
        public static void OpenOME(string file, int series, int index)
        {
            BioImage b = ims[index];
            threadProgress = 0;
            serie = series;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(ReadOME));
            t.Start();
            Progress pr = new Progress(file, "Opening");
            pr.Show();
            done = false;
            do
            {
                pr.UpdateProgressF(threadProgress);
                Application.DoEvents();
            } while (!done);
            t.Abort();
            pr.Close();
            b.rgbBitmap16 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            b.rgbBitmap8 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Table.AddImage(b);
        }
        */
        public static void Initialize()
        {
            //We initialize OME on a seperate thread so the user doesn't have to wait for initialization to
            //view images. 
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(InitOME));
            t.Start();
        }
        private static void InitOME()
        {
            Stopwatch sto = new Stopwatch();
            sto.Start();
            factory = new ServiceFactory();
            service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            reader = new ImageReader();
            writer = new ImageWriter();
            initialized = true;
            sto.Stop();
        }
        /*
        public static void Save(string file)
        {
            //This is the default saving mode we save the roi's in CSV and save tiff fast with BitMiracle.
            BioImage b = Table.GetImage(file);
            threadProgress = 0;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Save));
            t.Start();
            Progress pr = new Progress(file, "Saving");
            pr.Show();
            done = false;
            do
            {
                pr.UpdateProgressF(threadProgress);
                Application.DoEvents();
            } while (!done);
            pr.Close();
        }
        
        public static void Open(string file, int index)
        {
            BioImage b = new BioImage(file);
            ims.Add(b);
            //This is the default opening mode we load the roi's in CSV and open tiff fast with BitMiracle.
            threadProgress = 0;
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(Open));
            t.Start();
            Progress pr = new Progress(file, "Opening");
            pr.Show();
            done = false;
            do
            {
                pr.UpdateProgressF(threadProgress);
                Application.DoEvents();
            } while (!done);
            pr.Close();
            b.rgbBitmap16 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            b.rgbBitmap8 = new Bitmap(b.SizeX, b.SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Table.AddImage(b);
        }
        */
        public static BioImage Write(string file)
        {
            BioImage b = Table.GetImage(file);
            done = false;
            string fn = Path.GetFileNameWithoutExtension(file);
            string dir = Path.GetDirectoryName(file);
            //Save ROIs to CSV file.
            if (b.Annotations.Count > 0)
            {
                string f = dir + "//" + fn + ".csv";
                ExportROIsCSV(f, b.Annotations);
            }
            ImageJDesc j = new ImageJDesc();
            j.FromImage(b);
            string desc = j.GetString();
            desc += NewLine + "BioImage-ROIs" + NewLine;
            //Embed ROI's to image description.
            for (int i = 0; i < b.Annotations.Count; i++)
            {
                desc += ROItoString(b.Annotations[i]);
            }
            
            Tiff image = Tiff.Open(file, "w");
            int stride = b.Buffers[0].Stride;
            int im = 0;
            /*
            for (int i = 0; i < b.Buffers.Count; i++)
            {
                b.Buffers[i].Image.Save()
            }
            */
            for (int c = 0; c < b.SizeC; c++)
            {
                for (int z = 0; z < b.SizeZ; z++)
                {
                    for (int t = 0; t < b.SizeT; t++)
                    {
                        image.SetDirectory((short)im);
                        image.SetField(TiffTag.IMAGEWIDTH, b.SizeX);
                        image.SetField(TiffTag.IMAGEDESCRIPTION, desc);
                        image.SetField(TiffTag.IMAGELENGTH, b.SizeY);
                        image.SetField(TiffTag.BITSPERSAMPLE, b.bitsPerPixel);
                        image.SetField(TiffTag.SAMPLESPERPIXEL, b.RGBChannelCount);
                        image.SetField(TiffTag.ROWSPERSTRIP, b.SizeY);
                        /*
                        if (im % 2 == 0)
                            image.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                        else
                            image.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE)
                        */ 
                        image.SetField(TiffTag.ORIENTATION, BitMiracle.LibTiff.Classic.Orientation.TOPLEFT);
                        image.SetField(TiffTag.PLANARCONFIG, PlanarConfig.CONTIG);
                        image.SetField(TiffTag.ROWSPERSTRIP, image.DefaultStripSize(0));
                        if (b.physicalSizeX != -1 && b.physicalSizeY != -1)
                        {
                            image.SetField(TiffTag.XRESOLUTION, (b.physicalSizeX * b.SizeX) / ((b.physicalSizeX * b.SizeX) * b.physicalSizeX));
                            image.SetField(TiffTag.YRESOLUTION, (b.physicalSizeY * b.SizeY) / ((b.physicalSizeY * b.SizeY) * b.physicalSizeY));
                            image.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.NONE);
                        }
                        else
                        {
                            image.SetField(TiffTag.XRESOLUTION, 100.0);
                            image.SetField(TiffTag.YRESOLUTION, 100.0);
                            image.SetField(TiffTag.RESOLUTIONUNIT, ResUnit.INCH);
                        }
                        // specify that it's a page within the multipage file
                        image.SetField(TiffTag.SUBFILETYPE, FileType.PAGE);
                        // specify the page number
                        byte[] buffer = b.Buffers[im].GetSaveBytes();
                        image.SetField(TiffTag.PAGENUMBER, c, b.Buffers.Count);
                        for (int i = 0, offset = 0; i < b.SizeY; i++)
                        {
                            image.WriteScanline(buffer, offset, i, 0);
                            offset += stride;
                        }
                        image.WriteDirectory();
                        threadProgress = ((float)im / (float)b.ImageCount);
                        im++;
                    }
                }
            }
            image.Dispose();
            done = true;
            Recorder.AddLine("BioImage.Save(" + '"' + file + '"' + ");");
            return b;
        }
        public static BioImage Read(string file)
        {
            BioImage b = new BioImage(file);
            b.series = 0;
            done = false;
            string fn = Path.GetFileNameWithoutExtension(file);
            string dir = Path.GetDirectoryName(file);
            if (File.Exists(fn + ".csv"))
            {
                string f = dir + "//" + fn + ".csv";
                b.Annotations = BioImage.ImportROIsCSV(f);
            }
            if (file.EndsWith("tif") || file.EndsWith("tiff") || file.EndsWith("TIF") || file.EndsWith("TIFF"))
            {
                Tiff image = Tiff.Open(file, "r");
                int SizeX = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int SizeY = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                b.bitsPerPixel = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                b.littleEndian = image.IsBigEndian();   
                int RGBChannelCount = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
                string desc = "";
                try
                {
                    desc = image.GetField(TiffTag.IMAGEDESCRIPTION)[0].ToString();
                }
                catch (Exception)
                {

                }

                ImageJDesc imDesc = new ImageJDesc();
                if (desc.StartsWith("ImageJ"))
                {
                    imDesc.SetString(desc);
                    if (imDesc.channels != 0)
                        b.sizeC = imDesc.channels;
                    else
                        b.sizeC = 1;
                    if (imDesc.slices != 0)
                        b.sizeZ = imDesc.slices;
                    else
                        b.sizeZ = 1;
                    if (imDesc.frames != 0)
                        b.sizeT = imDesc.frames;
                    else
                        b.sizeT = 1;
                    if (imDesc.finterval != 0)
                        b.frameInterval = imDesc.finterval;
                    else
                        b.frameInterval = 1;
                    if (imDesc.spacing != 0)
                        b.physicalSizeZ = imDesc.spacing;
                    else
                        b.physicalSizeZ = 1;
                }
                else
                {
                    //This is a non ImageJ image.
                    b.sizeC = 1;
                    b.sizeT = 1;
                    b.sizeZ = 1;
                 }
                string[] sts = desc.Split('\n');
                bool start = false;
                for (int i = 0; i < sts.Length; i++)
                {
                    if (start)
                    {
                        if (sts[i].Length > 1)
                            b.Annotations.Add(StringToROI(sts[i]));
                    }
                    if (sts[i].Contains("BioImage-ROI"))
                    {
                        start = true;
                    }
                }

                PixelFormat PixelFormat;
                if (RGBChannelCount == 1)
                {
                    if (b.bitsPerPixel > 8)
                    {
                        PixelFormat = PixelFormat.Format16bppGrayScale;
                    }
                    else
                    {
                        PixelFormat = PixelFormat.Format8bppIndexed;
                    }
                }
                else
                if (RGBChannelCount == 3)
                {
                    if (b.bitsPerPixel > 8)
                    {
                        PixelFormat = PixelFormat.Format48bppRgb;
                    }
                    else
                    {
                        PixelFormat = PixelFormat.Format24bppRgb;
                    }
                }
                else
                    PixelFormat = PixelFormat.Format32bppArgb;

                try
                {
                    string unit = (string)image.GetField(TiffTag.RESOLUTIONUNIT)[0].ToString();
                    if (unit == "CENTIMETER")
                    {
                        b.physicalSizeX = image.GetField(TiffTag.XRESOLUTION)[0].ToDouble() / 1000;
                        b.physicalSizeX = image.GetField(TiffTag.YRESOLUTION)[0].ToDouble() / 1000;
                    }
                    else
                    if (unit == "INCH")
                    {
                        //inch to centimeter
                        b.physicalSizeX = (2.54 / image.GetField(TiffTag.XRESOLUTION)[0].ToDouble()) / 1000;
                        b.physicalSizeY = (2.54 / image.GetField(TiffTag.YRESOLUTION)[0].ToDouble()) / 1000;
                    }
                    else
                    if (unit == "NONE")
                    {
                        if (imDesc.unit == "micron")
                        {
                            //size micron
                            b.physicalSizeX = (b.SizeX / image.GetField(TiffTag.XRESOLUTION)[0].ToDouble()) / b.SizeX;
                            b.physicalSizeY = (b.SizeY / image.GetField(TiffTag.YRESOLUTION)[0].ToDouble()) / b.SizeY;
                        }
                        else
                        {
                            throw new InvalidDataException("Image has unknown size unit.");
                        }
                    }
                }
                catch (Exception)
                {

                }
                b.Coords = new int[b.SizeZ, b.SizeC, b.SizeT];
                for (int i = 0; i < b.SizeC; i++)
                {
                    Channel ch = new Channel(i, b.bitsPerPixel);
                    b.Channels.Add(ch);
                }
                int z = 0;
                int c = 0;
                int t = 0;
                b.Buffers = new List<BufferInfo>();
                int pages = image.NumberOfDirectories();
                int stride = image.ScanlineSize();

                for (int p = 0; p < pages; p++)
                {
                    image.SetDirectory((short)p);
                    byte[] bytes = new byte[stride * SizeY];
                    for (int i = 0, offset = 0; i < SizeY; i++)
                    {
                        image.ReadScanline(bytes, offset, i, 0);
                        offset += stride;
                    }
                    BufferInfo inf = new BufferInfo(file, SizeX, SizeY, PixelFormat, bytes, new ZCT(0, 0, 0), p);
                    b.Buffers.Add(inf);
                    threadProgress = (int)((double)p / (double)pages);
                }
                for (int im = 0; im < b.Buffers.Count; im++)
                {
                    ZCT co = new ZCT(z, c, t);
                    int index = b.Coords[z, c, t];
                    b.Coords[co.Z, co.C, co.T] = im;
                    b.Buffers[im].Coordinate = co;
                    if (c < b.SizeC - 1)
                        c++;
                    else
                    {
                        c = 0;
                        if (z < b.SizeZ - 1)
                            z++;
                        else
                        {
                            z = 0;
                            if (t < b.SizeT - 1)
                                t++;
                            else
                                t = 0;
                        }
                    }
                }
                image.Dispose();
            }
            else
            {
                b.bitsPerPixel = 8;
                b.littleEndian = BitConverter.IsLittleEndian;
                b.sizeZ = 1;
                b.sizeC = 1;
                b.sizeT = 1;
                BufferInfo inf;
                inf = new BufferInfo(file, Image.FromFile(file), new ZCT(0, 0, 0), 0);
                b.Buffers.Add(inf);
                Channel ch = new Channel(0, 8);
                b.Channels.Add(ch);
                b.Coords = new int[b.SizeZ, b.SizeC, b.sizeT];
            }
            Recorder.AddLine("BioImage.Open(" + '"' + file + '"' + ");");
            done = true;
            return b;
        }
        public static BioImage WriteOME(string file)
        {
            int series = serie;
            BioImage b = new BioImage(file);
            // create OME-XML metadata store
            loci.formats.meta.IMetadata omexml = service.createOMEXMLMetadata();
            omexml.setImageID("Image:0", series);
            omexml.setPixelsID("Pixels:0", series);
            if (b.littleEndian)
                omexml.setPixelsBinDataBigEndian(java.lang.Boolean.TRUE, 0, 0);
            else
                omexml.setPixelsBinDataBigEndian(java.lang.Boolean.FALSE, 0, 0);

            omexml.setPixelsDimensionOrder(ome.xml.model.enums.DimensionOrder.XYCZT, series);
            if (b.bitsPerPixel > 8)
                omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT16, series);
            else
                omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT8, series);
            omexml.setPixelsSizeX(new PositiveInteger(java.lang.Integer.valueOf(b.SizeX)), series);
            omexml.setPixelsSizeY(new PositiveInteger(java.lang.Integer.valueOf(b.SizeY)), series);
            omexml.setPixelsSizeZ(new PositiveInteger(java.lang.Integer.valueOf(b.SizeZ)), series);
            int samples = 1;
            if (b.isRGB)
                samples = 3;
            omexml.setPixelsSizeC(new PositiveInteger(java.lang.Integer.valueOf(b.SizeC * samples)), series);
            omexml.setPixelsSizeT(new PositiveInteger(java.lang.Integer.valueOf(b.SizeT)), series);

            if (b.physicalSizeX != -1)
            {
                ome.units.quantity.Length p = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeX), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeX(p, series);
            }
            if (b.physicalSizeY != -1)
            {
                ome.units.quantity.Length p = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeY), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeY(p, series);
            }
            if (b.physicalSizeZ != -1)
            {
                ome.units.quantity.Length p = new ome.units.quantity.Length(java.lang.Double.valueOf(b.physicalSizeZ), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeZ(p, series);
            }
            if (b.stageSizeX != -1)
            {
                ome.units.quantity.Length s = new ome.units.quantity.Length(java.lang.Double.valueOf(b.stageSizeX), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelX(s, series);
            }
            if (b.stageSizeY != -1)
            {
                ome.units.quantity.Length s = new ome.units.quantity.Length(java.lang.Double.valueOf(b.stageSizeY), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelY(s, series);
            }
            if (b.stageSizeX != -1)
            {
                ome.units.quantity.Length s = new ome.units.quantity.Length(java.lang.Double.valueOf(b.stageSizeZ), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelZ(s, series);
            }

            for (int channel = 0; channel < b.Channels.Count; channel++)
            {
                Channel c = b.Channels[channel];
                omexml.setChannelID("Channel:" + channel + ":" + series, series, channel);
                omexml.setChannelSamplesPerPixel(new PositiveInteger(java.lang.Integer.valueOf(samples)), series, channel);
                if (c.Name != "")
                    omexml.setChannelName(c.Name, series, channel);
                if (c.color != null)
                {
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(c.color.Value.R, c.color.Value.G, c.color.Value.B, c.color.Value.A);
                    omexml.setChannelColor(col, series, channel);
                }
                if (c.Emission != -1)
                {
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Emission), ome.units.UNITS.NANOMETER);
                    omexml.setChannelEmissionWavelength(fl, series, channel);
                }
                if (c.Excitation != -1)
                {
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Excitation), ome.units.UNITS.NANOMETER);
                    omexml.setChannelEmissionWavelength(fl, series, channel);
                }
                if (c.Exposure != -1)
                {
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(c.Exposure), ome.units.UNITS.MILLISECOND);
                    omexml.setChannelEmissionWavelength(fl, series, channel);
                }
                if (c.ContrastMethod != "")
                {
                    ome.xml.model.enums.ContrastMethod cm = (ome.xml.model.enums.ContrastMethod)Enum.Parse(typeof(ome.xml.model.enums.ContrastMethod), c.ContrastMethod);
                    omexml.setChannelContrastMethod(cm, series, channel);
                }
                if (c.Fluor != "")
                {
                    omexml.setChannelFluor(c.Fluor, series, channel);
                }
                if (c.IlluminationType != "")
                {
                    string tr = c.IlluminationType.ToUpper();

                    ome.xml.model.enums.IlluminationType cm = (ome.xml.model.enums.IlluminationType)Enum.Parse(typeof(ome.xml.model.enums.IlluminationType), tr);
                    omexml.setChannelIlluminationType(cm, series, channel);
                }
                if (c.LightSourceIntensity != -1)
                {
                    ome.units.quantity.Power fl = new ome.units.quantity.Power(java.lang.Double.valueOf(c.LightSourceIntensity), ome.units.UNITS.VOLT);
                    omexml.setLightEmittingDiodePower(fl, series, channel);
                }
            }

            int i = 0;
            foreach (Annotation an in b.Annotations)
            {
                if (an.roiID == "")
                    omexml.setROIID("ROI:" + i.ToString() + ":" + series, i);
                else
                    omexml.setROIID(an.roiID, i);
                omexml.setROIName(an.roiName, i);
                if (an.type == Annotation.Type.Point)
                {
                    if (an.id == "")
                        omexml.setPointID(an.id, i, series);
                    else
                        omexml.setPointID("Shape:" + i + ":" + series, i, series);
                    omexml.setPointX(java.lang.Double.valueOf(an.X), i, series);
                    omexml.setPointY(java.lang.Double.valueOf(an.Y), i, series);
                    omexml.setPointTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setPointTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setPointTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    if (an.Text != "")
                        omexml.setPointText(an.Text, i, series);
                    else
                        omexml.setPointText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    b.meta.setPointFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setPointStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setPointStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setPointFillColor(colf, i, series);
                }
                else
                if (an.type == Annotation.Type.Polygon || an.type == Annotation.Type.Freeform)
                {
                    if (an.id == "")
                        omexml.setPolygonID(an.id, i, series);
                    else
                        omexml.setPolygonID("Shape:" + i + ":" + series, i, series);
                    omexml.setPolygonPoints(an.PointsToString(), i, series);
                    omexml.setPolygonTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setPolygonTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setPolygonTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    if (an.Text != "")
                        omexml.setPolygonText(an.Text, i, series);
                    else
                        omexml.setPolygonText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setPolygonFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setPolygonStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setPolygonStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setPolygonFillColor(colf, i, series);
                }
                else
                if (an.type == Annotation.Type.Rectangle)
                {
                    if (an.id != "")
                        omexml.setRectangleID(an.id, i, series);
                    else
                        omexml.setRectangleID("Shape:" + i + ":" + series, i, series);
                    omexml.setRectangleWidth(java.lang.Double.valueOf(an.W), i, series);
                    omexml.setRectangleHeight(java.lang.Double.valueOf(an.H), i, series);
                    omexml.setRectangleX(java.lang.Double.valueOf(an.Rect.X), i, series);
                    omexml.setRectangleY(java.lang.Double.valueOf(an.Rect.Y), i, series);
                    omexml.setRectangleTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setRectangleTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setRectangleTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    omexml.setRectangleText(i.ToString(), i, series);
                    if (an.Text != "")
                        omexml.setRectangleText(an.Text, i, series);
                    else
                        omexml.setRectangleText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setRectangleFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setRectangleStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setRectangleStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setRectangleFillColor(colf, i, series);
                }
                else
                if (an.type == Annotation.Type.Line)
                {
                    if (an.id == "")
                        omexml.setLineID(an.id, i, series);
                    else
                        omexml.setLineID("Shape:" + i + ":" + series, i, series);
                    omexml.setLineX1(java.lang.Double.valueOf(an.GetPoint(0).X), i, series);
                    omexml.setLineY1(java.lang.Double.valueOf(an.GetPoint(0).Y), i, series);
                    omexml.setLineX2(java.lang.Double.valueOf(an.GetPoint(1).X), i, series);
                    omexml.setLineY2(java.lang.Double.valueOf(an.GetPoint(1).Y), i, series);
                    omexml.setLineTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setLineTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setLineTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    if (an.Text != "")
                        omexml.setLineText(an.Text, i, series);
                    else
                        omexml.setLineText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setLineFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setLineStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setLineStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setLineFillColor(colf, i, series);
                }
                else
                if (an.type == Annotation.Type.Ellipse)
                {

                    if (an.id == "")
                        omexml.setEllipseID(an.id, i, series);
                    else
                        omexml.setEllipseID("Shape:" + i + ":" + series, i, series);
                    //We need to change System.Drawing.Rectangle to ellipse radius;
                    double w = (double)an.W / 2;
                    double h = (double)an.H / 2;
                    omexml.setEllipseRadiusX(java.lang.Double.valueOf(w), i, series);
                    omexml.setEllipseRadiusY(java.lang.Double.valueOf(h), i, series);

                    double x = an.Point.X + w;
                    double y = an.Point.Y + h;
                    omexml.setEllipseX(java.lang.Double.valueOf(x), i, series);
                    omexml.setEllipseY(java.lang.Double.valueOf(y), i, series);
                    omexml.setEllipseTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setEllipseTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setEllipseTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    if (an.Text != "")
                        omexml.setEllipseText(an.Text, i, series);
                    else
                        omexml.setEllipseText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setEllipseFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setEllipseStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setEllipseStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setEllipseFillColor(colf, i, series);
                }
                else
                if (an.type == Annotation.Type.Label)
                {
                    if (an.id != "")
                        omexml.setLabelID(an.id, i, series);
                    else
                        omexml.setLabelID("Shape:" + i + ":" + series, i, series);
                    omexml.setLabelX(java.lang.Double.valueOf(an.Rect.X), i, series);
                    omexml.setLabelY(java.lang.Double.valueOf(an.Rect.Y), i, series);
                    omexml.setLabelTheZ(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.Z)), i, series);
                    omexml.setLabelTheC(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.C)), i, series);
                    omexml.setLabelTheT(new NonNegativeInteger(java.lang.Integer.valueOf(an.coord.T)), i, series);
                    omexml.setLabelText(i.ToString(), i, series);
                    if (an.Text != "")
                        omexml.setLabelText(an.Text, i, series);
                    else
                        omexml.setLabelText(i.ToString(), i, series);
                    ome.units.quantity.Length fl = new ome.units.quantity.Length(java.lang.Double.valueOf(an.font.Size), ome.units.UNITS.PIXEL);
                    omexml.setLabelFontSize(fl, i, series);
                    ome.xml.model.primitives.Color col = new ome.xml.model.primitives.Color(an.strokeColor.R, an.strokeColor.G, an.strokeColor.B, an.strokeColor.A);
                    omexml.setLabelStrokeColor(col, i, series);
                    ome.units.quantity.Length sw = new ome.units.quantity.Length(java.lang.Double.valueOf(an.strokeWidth), ome.units.UNITS.PIXEL);
                    omexml.setLabelStrokeWidth(sw, i, series);
                    ome.xml.model.primitives.Color colf = new ome.xml.model.primitives.Color(an.fillColor.R, an.fillColor.G, an.fillColor.B, an.fillColor.A);
                    omexml.setLabelFillColor(colf, i, series);
                }
                i++;
            }

            
            writer.setMetadataRetrieve(omexml);
            //We delete the file so we don't just add more images to an existing file;
            if (File.Exists(file))
                File.Delete(file);
            writer.setId(file);
            writer.setSeries(series);
            writer.setWriteSequentially(true);
            wr = writer;
            Progress pr = new Progress(file, "Saving");
            pr.Show();
            for (int bu = 0; bu < b.Buffers.Count; bu++)
            {
                writer.saveBytes(bu,b.Buffers[bu].GetSaveBytes());
                threadProgress = (float)bu / b.Buffers.Count;
                pr.UpdateProgress((int)threadProgress);
                Application.DoEvents();
            }
            pr.Close();
            pr.Dispose();
            writer.close();
            Recorder.AddLine("BioImage.SaveOME(" + '"' + file + '"' + "," + series + ");");
            return b;
        }
        public static BioImage ReadOME(string file)
        {
            st.Start();
            done = false;
            BioImage b = new BioImage(file);
            b.Loading = true;
            b.meta = service.createOMEXMLMetadata();
            reader.setMetadataStore(b.meta);
            // initialize file
            reader.setId(file);
            reader.setSeries(serie);
            int RGBChannelCount = reader.getRGBChannelCount();
            b.bitsPerPixel = reader.getBitsPerPixel();
            b.id = file;
            int SizeX = reader.getSizeX();
            int SizeY = reader.getSizeY();
            b.sizeC = reader.getSizeC();
            b.sizeZ = reader.getSizeZ();
            b.sizeT = reader.getSizeT();
            b.littleEndian = reader.isLittleEndian();
            b.seriesCount = reader.getSeriesCount();
            b.imagesPerSeries = reader.getImageCount() / b.seriesCount;
            b.Coords = new int[b.SizeZ, b.SizeC, b.SizeT];
            b.series = serie;
            string order = reader.getDimensionOrder();
            PixelFormat PixelFormat;
            bool bit48 = false;
            int stride = 0;
            if (RGBChannelCount == 1)
            {
                if (b.bitsPerPixel > 8)
                {
                    PixelFormat = PixelFormat.Format16bppGrayScale;
                    stride = SizeX * 2;
                }
                else
                {
                    PixelFormat = PixelFormat.Format8bppIndexed;
                    stride = SizeY;
                }
            }
            else
            if (RGBChannelCount == 3)
            {
                if (b.bitsPerPixel > 8)
                {
                    PixelFormat = PixelFormat.Format48bppRgb;
                    stride = SizeX * 2 * 3;
                    bit48 = true;
                }
                else
                {
                    PixelFormat = PixelFormat.Format24bppRgb;
                    stride = SizeX * 3;
                }
            }
            else
            {
                PixelFormat = PixelFormat.Format32bppRgb;
                stride = SizeX * 4;
            }
            long ms1 = st.ElapsedMilliseconds;
            //Lets get the channels amd initialize them.
            for (int i = 0; i < b.SizeC; i++)
            {
                Channel ch = new Channel(i, b.bitsPerPixel);
                try
                {
                    if (b.meta.getChannelName(0, i) != null)
                        ch.Name = b.meta.getChannelName(0, i);
                    if (b.meta.getChannelSamplesPerPixel(0, i) != null)
                    {
                        int s = b.meta.getChannelSamplesPerPixel(0, i).getNumberValue().intValue();
                        ch.SamplesPerPixel = s;
                    }
                    if (b.meta.getChannelID(0, i) != null)
                        ch.ID = b.meta.getChannelID(0, i);
                    if (b.meta.getChannelFluor(0, i) != null)
                        ch.Fluor = b.meta.getChannelFluor(0, i);
                    if (b.meta.getChannelColor(0, i) != null)
                    {
                        ome.xml.model.primitives.Color cc = b.meta.getChannelColor(0, i);
                        ch.color = System.Drawing.Color.FromArgb(cc.getRed(), cc.getGreen(), cc.getBlue());
                        if (ch.color.Value.R == 255 && ch.color.Value.G == 0 && ch.color.Value.B == 0)
                            ch.rgb = RGB.R;
                        if (ch.color.Value.R == 0 && ch.color.Value.G == 255 && ch.color.Value.B == 0)
                            ch.rgb = RGB.G;
                        if (ch.color.Value.R == 0 && ch.color.Value.G == 0 && ch.color.Value.B == 255)
                            ch.rgb = RGB.B;
                    }
                    if (b.meta.getChannelIlluminationType(0, i) != null)
                        ch.IlluminationType = b.meta.getChannelIlluminationType(0, i).toString();
                    if (b.meta.getChannelContrastMethod(0, i) != null)
                        ch.ContrastMethod = b.meta.getChannelContrastMethod(0, i).toString();
                    if (b.meta.getPlaneExposureTime(0, i) != null)
                        ch.Exposure = b.meta.getPlaneExposureTime(0, i).value().intValue();
                    if (b.meta.getChannelEmissionWavelength(0, i) != null)
                        ch.Emission = b.meta.getChannelEmissionWavelength(0, i).value().intValue();
                    if (b.meta.getChannelExcitationWavelength(0, i) != null)
                        ch.Excitation = b.meta.getChannelExcitationWavelength(0, i).value().intValue();
                    if (b.meta.getChannelLightSourceSettingsAttenuation(0, i) != null)
                        ch.LightSourceIntensity = b.meta.getChannelLightSourceSettingsAttenuation(0, i).getNumberValue().doubleValue();
                    if (b.meta.getLightEmittingDiodePower(0, i) != null)
                        ch.LightSourceIntensity = b.meta.getLightEmittingDiodePower(0, i).value().doubleValue();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (i == 0)
                {
                    b.rgbChannels[0] = 0;
                    ch.rgb = RGB.R;
                }
                else
                if (i == 1)
                {
                    b.rgbChannels[1] = 1;
                    ch.rgb = RGB.G;
                }
                else
                if (i == 2)
                {
                    b.rgbChannels[2] = 2;
                    ch.rgb = RGB.B;
                }
                b.Channels.Add(ch);
            }

            int rc = b.meta.getROICount();
            for (int i = 0; i < rc; i++)
            {
                string roiID = b.meta.getROIID(i);
                string roiName = b.meta.getROIName(i);
                ZCT co = new ZCT(0, 0, 0);
                int scount = 1;
                try
                {
                    scount = b.meta.getShapeCount(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                }


                for (int sc = 0; sc < scount; sc++)
                {
                    string type = b.meta.getShapeType(i, sc);
                    Annotation an = new Annotation();
                    an.roiID = roiID;
                    an.roiName = roiName;
                    an.shapeIndex = sc;
                    if (type == "Point")
                    {
                        an.type = Annotation.Type.Point;
                        an.id = b.meta.getPointID(i, sc);
                        double dx = b.meta.getPointX(i, sc).doubleValue();
                        double dy = b.meta.getPointY(i, sc).doubleValue();
                        an.AddPoint(new PointD(dx, dy));
                        if (b.ImageCount > 1)
                        {
                            ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getPointTheZ(i, sc);
                            if (nz != null)
                                co.Z = nz.getNumberValue().intValue();
                            ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getPointTheC(i, sc);
                            if (nc != null)
                                co.C = nc.getNumberValue().intValue();
                            ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getPointTheT(i, sc);
                            if (nt != null)
                                co.T = nt.getNumberValue().intValue();
                            an.coord = co;

                        }

                        an.Text = b.meta.getPointText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getPointFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getPointStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getPointStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getPointStrokeColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Line")
                    {
                        an.type = Annotation.Type.Line;
                        an.id = b.meta.getLineID(i, sc);
                        double px1 = b.meta.getLineX1(i, sc).doubleValue();
                        double py1 = b.meta.getLineY1(i, sc).doubleValue();
                        double px2 = b.meta.getLineX2(i, sc).doubleValue();
                        double py2 = b.meta.getLineY2(i, sc).doubleValue();
                        an.AddPoint(new PointD(px1, py1));
                        an.AddPoint(new PointD(px2, py2));
                        if (b.ImageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getLineTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getLineTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getLineTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = b.meta.getLineText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getLineFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getLineStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getLineStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getLineFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Rectangle")
                    {
                        an.type = Annotation.Type.Rectangle;
                        an.id = b.meta.getRectangleID(i, sc);
                        double px = b.meta.getRectangleX(i, sc).doubleValue();
                        double py = b.meta.getRectangleY(i, sc).doubleValue();
                        double pw = b.meta.getRectangleWidth(i, sc).doubleValue();
                        double ph = b.meta.getRectangleHeight(i, sc).doubleValue();
                        an.Rect = new RectangleD(px, py, pw, ph);
                        if (b.ImageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getRectangleTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getRectangleTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getRectangleTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = b.meta.getRectangleText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getRectangleFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getRectangleStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getRectangleStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getRectangleFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                        ome.xml.model.enums.FillRule fr = b.meta.getRectangleFillRule(i, sc);
                    }
                    else
                    if (type == "Ellipse")
                    {
                        an.type = Annotation.Type.Ellipse;
                        an.id = b.meta.getEllipseID(i, sc);
                        double px = b.meta.getEllipseX(i, sc).doubleValue();
                        double py = b.meta.getEllipseY(i, sc).doubleValue();
                        double ew = b.meta.getEllipseRadiusX(i, sc).doubleValue();
                        double eh = b.meta.getEllipseRadiusY(i, sc).doubleValue();
                        //We convert the ellipse radius to System.Drawing.Rectangle
                        double w = ew * 2;
                        double h = eh * 2;
                        double x = px - ew;
                        double y = py - eh;
                        an.Rect = new RectangleD(x, y, w, h);
                        if (b.ImageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getEllipseTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getEllipseTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getEllipseTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = b.meta.getEllipseText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getEllipseFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getEllipseStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getEllipseStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getEllipseFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polygon")
                    {
                        an.type = Annotation.Type.Polygon;
                        an.id = b.meta.getPolygonID(i, sc);
                        an.closed = true;
                        string pxs = b.meta.getPolygonPoints(i, sc);
                        PointD[] pts = an.stringToPoints(pxs);
                        if (pts.Length > 100)
                        {
                            an.type = Annotation.Type.Freeform;
                        }
                        an.AddPoints(pts);
                        if (b.ImageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getPolygonTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getPolygonTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getPolygonTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = b.meta.getPolygonText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getPolygonFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getPolygonStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getPolygonStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getPolygonFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polyline")
                    {
                        an.type = Annotation.Type.Polyline;
                        an.id = b.meta.getPolylineID(i, sc);
                        string pxs = b.meta.getPolylinePoints(i, sc);
                        an.AddPoints(an.stringToPoints(pxs));
                        if (b.ImageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getPolylineTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getPolylineTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getPolylineTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = b.meta.getPolylineText(i, sc);
                        ome.units.quantity.Length fl = b.meta.getPolylineFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getPolylineStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getPolylineStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getPolylineFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Label")
                    {
                        an.type = Annotation.Type.Label;
                        an.id = b.meta.getLabelID(i, sc);

                        if (b.ImageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = b.meta.getLabelTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = b.meta.getLabelTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = b.meta.getLabelTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }

                        ome.units.quantity.Length fl = b.meta.getLabelFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = b.meta.getLabelStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = b.meta.getLabelStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = b.meta.getLabelFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                        //We set this last so the text is measured correctly.
                        an.AddPoint(new PointD(b.meta.getLabelX(i, sc).doubleValue(), b.meta.getLabelY(i, sc).doubleValue()));
                        an.Text = b.meta.getLabelText(i, sc);
                    }
                    b.Annotations.Add(an);
                }
            }

            List<string> serFiles = new List<string>();
            serFiles.AddRange(reader.getSeriesUsedFiles());
            //List<BufferInfo> BufferInfos = new List<BufferInfo>(); 
            //List<string> Files = new List<string>();
            b.Buffers = new List<BufferInfo>();
            // read the image data bytes
            int pages = reader.getImageCount();
            for (int p = 0; p < pages; p++)
            {
                byte[] bytes = reader.openBytes(p);
                if (PixelFormat == PixelFormat.Format48bppRgb)
                {
                    //We convert 48bpp plane to 3 16bpp planes.
                    BufferInfo[] bfs = BufferInfo.RGB48To16(file, SizeX, SizeY, stride, bytes, new ZCT(0, 0, 0));
                    b.Buffers.AddRange(bfs);
                }
                else
                {
                    BufferInfo bf = new BufferInfo(file, SizeX, SizeY, PixelFormat, bytes, new ZCT(0, 0, 0), 0);
                    b.Buffers.Add(bf);
                }
                threadProgress = ((float)p / (float)pages)*100;

            }
            long ms2 = st.ElapsedMilliseconds - ms1;
            int z = 0;
            int c = 0;
            int t = 0;
            //"XYCZT"
            //some images have a different order
            if(bit48)
            {
                for (int im = 0; im < b.Buffers.Count; im+=3)
                {
                    b.Coords[z, 0, t] = im;
                    b.Coords[z, 1, t] = im + 1;
                    b.Coords[z, 2, t] = im + 2;
                    b.Buffers[im].Coordinate = new ZCT(z, 0, t);
                    b.Buffers[im+1].Coordinate = new ZCT(z, 1, t);
                    b.Buffers[im+2].Coordinate = new ZCT(z, 2, t);
                    if (z < b.SizeZ - 1)
                        z++;
                    else
                    {
                        z = 0;
                        if (t < b.SizeT - 1)
                            t++;
                        else
                            t = 0;
                    }
                }
            }
            else
            for (int im = 0; im < b.Buffers.Count; im++)
            {
                b.Coords[z, c, t] = im;
                b.Buffers[im].Coordinate = new ZCT(z, c, t);
                if (c < b.SizeC - 1)
                c++;
                else
                {
                    c = 0;
                    if (z < b.SizeZ - 1)
                        z++;
                    else
                    {
                        z = 0;
                        if (t < b.SizeT - 1)
                            t++;
                        else
                            t = 0;
                    }
                }
            }
            
            double stx = 0;
            double sty = 0;
            double stz = 0;
            double six = 0;
            double siy = 0;
            double siz = 0;

            try
            {
                if (b.meta.getPixelsPhysicalSizeX(b.series) != null)
                    b.physicalSizeX = b.meta.getPixelsPhysicalSizeX(b.series).value().doubleValue();
                if (b.meta.getPixelsPhysicalSizeY(b.series) != null)
                    b.physicalSizeY = b.meta.getPixelsPhysicalSizeY(b.series).value().doubleValue();
                if (b.meta.getPixelsPhysicalSizeZ(b.series) != null)
                    b.physicalSizeZ = b.meta.getPixelsPhysicalSizeZ(b.series).value().doubleValue();

                //Calling these when they are not defined causes an error so we use the try catch block.
                if (b.meta.getStageLabelX(b.series) != null)
                    b.stageSizeX = b.meta.getStageLabelX(b.series).value().doubleValue();
                if (b.meta.getStageLabelY(b.series) != null)
                    b.stageSizeY = b.meta.getStageLabelY(b.series).value().doubleValue();
                if (b.meta.getStageLabelZ(b.series) != null)
                    b.stageSizeZ = b.meta.getStageLabelZ(b.series).value().doubleValue();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                b.Volumes.Add(new VolumeD(new Point3D(stx, sty, stz), new Point3D(six * SizeX, siy * SizeY, siz * b.SizeZ)));
            }
            catch (Exception)
            {
                //Volume is used only for stage coordinates if error is thrown it is because this image doens't have any size information or it is incomplete as read by Bioformats.
            }
            reader.close();
            Recorder.AddLine("BioImage.OpenOME(" + '"' + file + '"' + "," + b.series + ");");
            done = true;
            b.Loading = false;
            return b;
        }

        private static Stopwatch st = new Stopwatch();
        private static ServiceFactory factory;
        private static OMEXMLService service;
        private static ImageReader reader;
        private static ImageWriter writer;
        private loci.formats.meta.IMetadata meta;
        public static bool OMEInit
        {
            get 
            {
                if (reader == null)
                {
                    return false;
                }
                else return true;
            }
        }

        public int GetSeriesCount(string file)
        {
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            loci.formats.ome.OMEXMLMetadata meta = service.createOMEXMLMetadata();
            // create format reader
            ImageReader imageReader = new ImageReader();
            imageReader.setMetadataStore(meta);
            // initialize file
            imageReader.setId(file);
            int c = imageReader.getSeriesCount();
            imageReader.close();
            return c;
        }

        //We use UNIX type line endings since they are supported by ImageJ & BioImage.
        public const char NewLine = '\n';
        public const string columns = "ROIID,ROINAME,TYPE,ID,SHAPEINDEX,TEXT,S,C,Z,T,X,Y,W,H,POINTS,STROKECOLOR,STROKECOLORW,FILLCOLOR,FONTSIZE\n";
        public static List<Annotation> OpenOMEROIs(string file)
        {
            List<Annotation> Annotations = new List<Annotation>();
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            loci.formats.ome.OMEXMLMetadata meta = service.createOMEXMLMetadata();
            // create format reader
            ImageReader imageReader = new ImageReader();
            imageReader.setMetadataStore(meta);
            // initialize file
            imageReader.setId(file);
            int imageCount = imageReader.getImageCount();
            int seriesCount = imageReader.getSeriesCount();

            int rc = meta.getROICount();
            for (int i = 0; i < rc; i++)
            {
                string roiID = meta.getROIID(i);
                string roiName = meta.getROIName(i);
                ZCT co = new ZCT(0, 0, 0);
                int scount = 1;
                try
                {
                    scount = meta.getShapeCount(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message.ToString());
                }
                for (int sc = 0; sc < scount; sc++)
                {
                    string type = meta.getShapeType(i, sc);
                    Annotation an = new Annotation();
                    an.roiID = roiID;
                    an.roiName = roiName;
                    an.shapeIndex = sc;
                    if (type == "Point")
                    {
                        an.type = Annotation.Type.Point;
                        an.id = meta.getPointID(i, sc);
                        double dx = meta.getPointX(i, sc).doubleValue();
                        double dy = meta.getPointY(i, sc).doubleValue();
                        an.AddPoint(new PointD(dx, dy));
                        if (imageCount > 1)
                        {
                            ome.xml.model.primitives.NonNegativeInteger nz = meta.getPointTheZ(i, sc);
                            if (nz != null)
                                co.Z = nz.getNumberValue().intValue();
                            ome.xml.model.primitives.NonNegativeInteger nc = meta.getPointTheC(i, sc);
                            if (nc != null)
                                co.C = nc.getNumberValue().intValue();
                            ome.xml.model.primitives.NonNegativeInteger nt = meta.getPointTheT(i, sc);
                            if (nt != null)
                                co.T = nt.getNumberValue().intValue();
                            an.coord = co;

                        }

                        an.Text = meta.getPointText(i, sc);
                        ome.units.quantity.Length fl = meta.getPointFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getPointStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getPointStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getPointStrokeColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Line")
                    {
                        an.type = Annotation.Type.Line;
                        an.id = meta.getLineID(i, sc);
                        double px1 = meta.getLineX1(i, sc).doubleValue();
                        double py1 = meta.getLineY1(i, sc).doubleValue();
                        double px2 = meta.getLineX2(i, sc).doubleValue();
                        double py2 = meta.getLineY2(i, sc).doubleValue();
                        an.AddPoint(new PointD(px1, py1));
                        an.AddPoint(new PointD(px2, py2));
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getLineTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getLineTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getLineTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getLineText(i, sc);
                        ome.units.quantity.Length fl = meta.getLineFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getLineStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getLineStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getLineFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Rectangle")
                    {
                        an.type = Annotation.Type.Rectangle;
                        an.id = meta.getRectangleID(i, sc);
                        double px = meta.getRectangleX(i, sc).doubleValue();
                        double py = meta.getRectangleY(i, sc).doubleValue();
                        double pw = meta.getRectangleWidth(i, sc).doubleValue();
                        double ph = meta.getRectangleHeight(i, sc).doubleValue();
                        an.Rect = new RectangleD(px, py, pw, ph);
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getRectangleTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getRectangleTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getRectangleTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getRectangleText(i, sc);
                        ome.units.quantity.Length fl = meta.getRectangleFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getRectangleStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getRectangleStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getRectangleFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                        ome.xml.model.enums.FillRule fr = meta.getRectangleFillRule(i, sc);
                    }
                    else
                    if (type == "Ellipse")
                    {
                        an.type = Annotation.Type.Ellipse;
                        an.id = meta.getEllipseID(i, sc);
                        double px = meta.getEllipseX(i, sc).doubleValue();
                        double py = meta.getEllipseY(i, sc).doubleValue();
                        double ew = meta.getEllipseRadiusX(i, sc).doubleValue();
                        double eh = meta.getEllipseRadiusY(i, sc).doubleValue();
                        //We convert the ellipse radius to System.Drawing.Rectangle
                        double w = ew * 2;
                        double h = eh * 2;
                        double x = px - ew;
                        double y = py - eh;
                        an.Rect = new RectangleD(x, y, w, h);
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getEllipseTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getEllipseTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getEllipseTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getEllipseText(i, sc);
                        ome.units.quantity.Length fl = meta.getEllipseFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getEllipseStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getEllipseStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getEllipseFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polygon")
                    {
                        an.type = Annotation.Type.Polygon;
                        an.id = meta.getPolygonID(i, sc);
                        an.closed = true;
                        string pxs = meta.getPolygonPoints(i, sc);
                        PointD[] pts = an.stringToPoints(pxs);
                        if (pts.Length > 100)
                        {
                            an.type = Annotation.Type.Freeform;
                        }
                        an.AddPoints(pts);
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getPolygonTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getPolygonTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getPolygonTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getPolygonText(i, sc);
                        ome.units.quantity.Length fl = meta.getPolygonFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getPolygonStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getPolygonStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getPolygonFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polyline")
                    {
                        an.type = Annotation.Type.Polyline;
                        an.id = meta.getPolylineID(i, sc);
                        string pxs = meta.getPolylinePoints(i, sc);
                        an.AddPoints(an.stringToPoints(pxs));
                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getPolylineTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getPolylineTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getPolylineTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }
                        an.Text = meta.getPolylineText(i, sc);
                        ome.units.quantity.Length fl = meta.getPolylineFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getPolylineStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getPolylineStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getPolylineFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Label")
                    {
                        an.type = Annotation.Type.Label;
                        an.id = meta.getLabelID(i, sc);

                        if (imageCount > 1)
                        {
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getLabelTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getLabelTheC(i, sc);
                                if (nc != null)
                                    co.C = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getLabelTheT(i, sc);
                                if (nt != null)
                                    co.T = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
                        }

                        ome.units.quantity.Length fl = meta.getLabelFontSize(i, sc);
                        if (fl != null)
                            an.font = new Font(SystemFonts.DefaultFont.FontFamily, (float)fl.value().doubleValue(), FontStyle.Regular);
                        ome.xml.model.primitives.Color col = meta.getLabelStrokeColor(i, sc);
                        if (col != null)
                            an.strokeColor = System.Drawing.Color.FromArgb(col.getAlpha(), col.getRed(), col.getGreen(), col.getBlue());
                        ome.units.quantity.Length fw = meta.getLabelStrokeWidth(i, sc);
                        if (fw != null)
                            an.strokeWidth = (float)fw.value().floatValue();
                        ome.xml.model.primitives.Color colf = meta.getLabelFillColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                        //We set this last so the text is measured correctly.
                        an.AddPoint(new PointD(meta.getLabelX(i, sc).doubleValue(), meta.getLabelY(i, sc).doubleValue()));
                        an.Text = meta.getLabelText(i, sc);
                    }
                    Annotations.Add(an);
                }
            }
            imageReader.close();
            return Annotations;
        }
        public static string ROIsToString(List<Annotation> Annotations)
        {
            string s = "";
            for (int i = 0; i < Annotations.Count; i++)
            {
                s += ROItoString(Annotations[i]);
            }
            return s;
        }
        public static string ROItoString(Annotation an)
        {
            PointD[] points = an.GetPoints();
            string pts = "";
            for (int j = 0; j < points.Length; j++)
            {
                if (j == points.Length - 1)
                    pts += points[j].X.ToString() + "," + points[j].Y.ToString();
                else
                    pts += points[j].X.ToString() + "," + points[j].Y.ToString() + " ";
            }

            char sep = (char)34;
            string sColor = sep.ToString() + an.strokeColor.A.ToString() + ',' + an.strokeColor.R.ToString() + ',' + an.strokeColor.G.ToString() + ',' + an.strokeColor.B.ToString() + sep.ToString();
            string bColor = sep.ToString() + an.fillColor.A.ToString() + ',' + an.fillColor.R.ToString() + ',' + an.fillColor.G.ToString() + ',' + an.fillColor.B.ToString() + sep.ToString();

            string line = an.roiID + ',' + an.roiName + ',' + an.type.ToString() + ',' + an.id + ',' + an.shapeIndex.ToString() + ',' +
                an.Text + ',' + an.coord.Z.ToString() + ',' + an.coord.C.ToString() + ',' + an.coord.T.ToString() + ',' + an.X.ToString() + ',' + an.Y.ToString() + ',' +
                an.W.ToString() + ',' + an.H.ToString() + ',' + sep.ToString() + pts + sep.ToString() + ',' + sColor + ',' + an.strokeWidth.ToString() + ',' + bColor + ',' + an.font.Size.ToString() + ',' + NewLine;
            return line;
        }
        public static void ExportROIsCSV(string filename, List<Annotation> Annotations)
        {
            string con = columns;
            con += ROIsToString(Annotations);
            File.WriteAllText(filename, con);
        }
        public static Annotation StringToROI(string sts)
        {
            Annotation an = new Annotation();
            string val = "";
            bool inSep = false;
            int col = 0;
            double x = 0;
            double y = 0;
            double w = 0;
            double h = 0;
            string line = sts;

            for (int i = 0; i < line.Length; i++)
            {
                char c = line[i];
                if (c == (char)34)
                {
                    if (!inSep)
                    {
                        inSep = true;
                    }
                    else
                        inSep = false;
                    continue;
                }

                if (c == ',' && !inSep)
                {
                    //ROIID,ROINAME,TYPE,ID,SHAPEINDEX,TEXT,S,C,Z,T,X,Y,W,H,POINTS,STROKECOLOR,STROKECOLORW,FILLCOLOR,FONTSIZE
                    if (col == 0)
                    {
                        //ROIID
                        an.roiID = val;
                    }
                    else
                    if (col == 1)
                    {
                        //ROINAME
                        an.roiName = val;
                    }
                    else
                    if (col == 2)
                    {
                        //TYPE
                        an.type = (Annotation.Type)Enum.Parse(typeof(Annotation.Type), val);
                    }
                    else
                    if (col == 3)
                    {
                        //ID
                        an.id = val;
                    }
                    else
                    if (col == 4)
                    {
                        //SHAPEINDEX/
                        an.shapeIndex = int.Parse(val);
                    }
                    else
                    if (col == 5)
                    {
                        //TEXT/
                        an.Text = val;
                    }
                    else
                    if (col == 6)
                    {
                        an.coord.Z = int.Parse(val);
                    }
                    else
                    if (col == 7)
                    {
                        an.coord.C = int.Parse(val);
                    }
                    else
                    if (col == 8)
                    {
                        an.coord.T = int.Parse(val);
                    }
                    else
                    if (col == 9)
                    {
                        x = double.Parse(val);
                    }
                    else
                    if (col == 10)
                    {
                        y = double.Parse(val);
                    }
                    else
                    if (col == 11)
                    {
                        w = double.Parse(val);
                    }
                    else
                    if (col == 12)
                    {
                        h = double.Parse(val);
                    }
                    else
                    if (col == 13)
                    {
                        //POINTS
                        an.AddPoints(an.stringToPoints(val));
                        an.Rect = new RectangleD(x, y, w, h);
                    }
                    else
                    if (col == 14)
                    {
                        //STROKECOLOR
                        string[] st = val.Split(',');
                        an.strokeColor = System.Drawing.Color.FromArgb(int.Parse(st[0]), int.Parse(st[1]), int.Parse(st[2]), int.Parse(st[3]));
                    }
                    else
                    if (col == 15)
                    {
                        //STROKECOLORW
                        an.strokeWidth = double.Parse(val);
                    }
                    else
                    if (col == 16)
                    {
                        //FILLCOLOR
                        string[] st = val.Split(',');
                        an.fillColor = System.Drawing.Color.FromArgb(int.Parse(st[0]), int.Parse(st[1]), int.Parse(st[2]), int.Parse(st[3]));
                    }
                    else
                    if (col == 17)
                    {
                        //FONTSIZE
                        double s = double.Parse(val);
                        an.font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.FontFamily, (float)s, System.Drawing.FontStyle.Regular);
                    }
                    col++;
                    val = "";
                }
                else
                    val += c;
            }

            return an;
        }
        public static List<Annotation> ImportROIsCSV(string filename)
        {
            List<Annotation> list = new List<Annotation>();
            if(!File.Exists(filename))
                return list;
            string[] sts = File.ReadAllLines(filename);
            //We start reading from line 1.
            for (int i = 1; i < sts.Length; i++)
            {
                list.Add(StringToROI(sts[i]));
            }
            return list;
        }
        public static void ExportROIFolder(string path, string filename)
        {
            string[] fs = Directory.GetFiles(path);
            int i = 0;
            foreach (string f in fs)
            {
                List<Annotation> annotations = OpenOMEROIs(f);
                string ff = Path.GetFileNameWithoutExtension(f);
                ExportROIsCSV(path + "//" + ff + "-" + i.ToString() + ".csv", annotations);
                i++;
            }
        }
        private Statistics statistics;
        public Statistics Statistics
        {
            get { return statistics; }
        }
        public Statistics UpdateStatistics()
        {
            if (bitsPerPixel > 8)
                statistics = new Statistics(true);
            else
                statistics = new Statistics(false);
            for (int i = 0; i < Buffers.Count; i++)
            {
                statistics.AddStatistics(Buffers[i].UpdateStatistics());
            }
            if (Buffers.Count > 0)
            {
                Statistics st;
                for (int c = 0; c < Channels.Count; c++)
                {
                    if (bitsPerPixel > 8)
                        st = new Statistics(true);
                    else
                        st = new Statistics(false);
                    for (int z = 0; z < SizeZ; z++)
                    {
                        for (int t = 0; t < SizeT; t++)
                        {
                            int ind = Coords[z, c, t];
                            st.AddStatistics(Buffers[ind].Statistics);
                        }
                    }
                    st.MeanHistogram();
                    Channels[c].stats = st;
                }
            }

            statistics.MeanHistogram();
            return statistics;
        }
        public void AutoThreshold()
        {
            UpdateStatistics();
            for (int c = 0; c < Channels.Count; c++)
            {
                Channels[c].Min = Channels[c].stats.Min;
                Channels[c].Max = Channels[c].stats.Max;
            }
        }
        public void Dispose()
        {
            for (int i = 0; i < Buffers.Count; i++)
            {
                Buffers[i] = null;
            }
            rgbBitmap8.Dispose();
            rgbBitmap16.Dispose();
            Table.RemoveImage(this);
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
        public override string ToString()
        {
            return Filename.ToString();
        }
    }
}
