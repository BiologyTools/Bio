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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using BitMiracle.LibTiff.Classic;

namespace BioImage
{
    public static class Table
    {
        public static Hashtable hashID = new Hashtable();
        public static Hashtable images = new Hashtable();
        public static Hashtable viewers = new Hashtable();
        public static BioImage GetImage(string ids)
        {
            int hash = ids.GetHashCode();
            return (BioImage)images[hash];
        }
        public static BioImage GetImageByHash(int hash, ZCT coord)
        {
            return (BioImage)images[hash];
        }
        public static void AddImage(BioImage im)
        {
            int hash = im.HashID;
            if (!images.ContainsKey(im.HashID))
            {
                images.Add(im.HashID, im);
            }
        }
        public static int GetImageCount(string s)
        {
            int i = 0;
            string f = Path.GetFileNameWithoutExtension(s);
            foreach (BioImage im in images.Values)
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
            images.Remove(im.HashID);
        }
        public static void AddViewer(ImageViewer v)
        {
            if (!viewers.ContainsKey(v.Text))
                viewers.Add(v.Text, v);
        }
        public static void RemoveViewer(ImageViewer v)
        {
            viewers.Remove(v.Text);
        }
        public static void RemoveViewer(string name)
        {
            viewers.Remove(name);
        }
        public static ImageViewer GetViewer(string s)
        {
            return (ImageViewer)viewers[s];
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
    public struct SZCT
    {
        public int S, Z, C, T;
        public SZCT(int s, int z, int c, int t)
        {
            S = s;
            Z = z;
            C = c;
            T = t;
        }
        public static bool operator ==(SZCT c1, SZCT c2)
        {
            if (c1.S == c2.S && c1.Z == c2.Z && c1.C == c2.C && c1.T == c2.T)
                return true;
            else
                return false;
        }
        public static bool operator !=(SZCT c1, SZCT c2)
        {
            if (c1.S == c2.S || c1.Z != c2.Z || c1.C != c2.C || c1.T != c2.T)
                return false;
            else
                return true;
        }
        public override string ToString()
        {
            return S + "," + Z + "," + C + "," + T;
        }
    }
    public struct SZCTXY
    {
        public int S, Z, C, T, X, Y;
        public SZCTXY(int s, int z, int c, int t, int x, int y)
        {
            S = s;
            Z = z;
            C = c;
            T = t;
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return S + "," + Z + "," + C + "," + T + "," + X + "," + Y;
        }

        public static bool operator ==(SZCTXY c1, SZCTXY c2)
        {
            if (c1.S == c2.S && c1.Z == c2.Z && c1.C == c2.C && c1.T == c2.T && c1.X == c2.X && c1.Y == c2.Y)
                return true;
            else
                return false;
        }
        public static bool operator !=(SZCTXY c1, SZCTXY c2)
        {
            if (c1.S != c2.S || c1.Z != c2.Z || c1.C != c2.C || c1.T != c2.T || c1.X != c2.X || c1.Y != c2.Y)
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
    public class BufferInfo
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
                bytes[index2+1] = upper;
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
        public static int CreateHash(string filepath, int ser, int index)
        {
            filepath = filepath.Replace("\\", "/");
            string ids = CreateID(filepath, index);
            return ids.GetHashCode();
        }
        public static string CreateID(string filepath, int index)
        {
            const char sep = '/';
            filepath = filepath.Replace("\\", "/");
            string s = filepath + sep + 'i' + sep + index;
            return s;
        }
        public static LevelsLinear filter8 = new LevelsLinear();
        public static LevelsLinear16bpp filter16 = new LevelsLinear16bpp();
        public string ID;

        private string file;
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
        //private int length;
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
                     s = SizeX * 4;
                
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
        private PixelFormat pixelFormat;
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
        public ZCT Coordinate;

        private byte[] bytes;
        public byte[] Bytes
        {
            get { return bytes; }
            set { bytes = value; }
        }

        private Image image;
        public Image Image
        {
            get 
            {
                if(pixelFormat != PixelFormat.Format8bppIndexed && pixelFormat != PixelFormat.Format16bppGrayScale)
                    return image;
                else
                    return GetBitmap(SizeX, SizeY, Stride, PixelFormat, Bytes);
            }
            set 
            {
                PixelFormat = value.PixelFormat;
                SizeX = value.Width;
                SizeY = value.Height;
                bytes = GetBuffer((Bitmap)value);
            }
        }

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

        private static byte[] GetPaddedBuffer(byte[] bts,int w, int h, int stride)
        {
            int newstride = GetStridePadded(stride);
            if (newstride == stride)
                return bts;
            byte[] newbts = new byte[newstride * h];
            for (int y = 0; y < h; ++y)
            {
                for (int x = 0; x < w * 2; ++x)
                {
                    int index = y * stride + x;
                    int index2 = y * newstride + x;
                    newbts[index2] = bts[index];
                }
            }
            return newbts;
        }
        public static unsafe Bitmap GetBitmap(int w, int h, int stride, PixelFormat px, byte[] bts)
        {
            fixed (byte* numPtr1 = bts)
            {
                if (stride % 4 == 0)
                    return new Bitmap(w, h, stride, px, new IntPtr((void*)numPtr1));
                int newstride = GetStridePadded(stride);
                byte[] newbts = GetPaddedBuffer(bts, w, h, stride);
                fixed (byte* numPtr2 = newbts)
                {
                    return new Bitmap(w, h, newstride, px, new IntPtr((void*)numPtr2));
                }
            }
        }
        public BufferInfo(string file, int w, int h, PixelFormat px, byte[] bts, ZCT coord, int index)
        {
            ID = CreateID(file, index);
            SizeX = w;
            SizeY = h;
            pixelFormat = px;
            Coordinate = coord;
            bytes = bts;
            bts = GetPaddedBuffer(bts, w, h,Stride);
            if (px != PixelFormat.Format8bppIndexed && px != PixelFormat.Format16bppGrayScale)
            {
                //SwitchRedBlue();
            }
        }
        public BufferInfo(string file, Image im, ZCT coord, int index)
        {
            ID = CreateID(file, index);
            SizeX = im.Width;
            SizeY = im.Height;
            pixelFormat = im.PixelFormat;
            Coordinate = coord;
            image = im;
            bytes = GetBuffer((Bitmap)Image);
            //bts = GetPaddedBuffer(bts, w, h,Stride);
            if (PixelFormat != PixelFormat.Format8bppIndexed && PixelFormat != PixelFormat.Format16bppGrayScale)
            {
                //SwitchRedBlue();
            }
        }
        public BufferInfo(string file, int w, int h, PixelFormat px, byte[] bts, ZCT coord, string id)
        {
            ID = id;
            SizeX = w;
            SizeY = h;
            pixelFormat = px;
            Coordinate = coord;
            bytes = bts;
            if (px != PixelFormat.Format8bppIndexed && px != PixelFormat.Format16bppGrayScale)
            {
                //SwitchRedBlue();
            }
        }
        public BufferInfo(int w, int h, PixelFormat px, byte[] bts, ZCT coord, string id)
        {
            ID = id;
            SizeX = w;
            SizeY = h;
            pixelFormat = px;
            Coordinate = coord;
            bytes = bts;
        }
        public void SwitchRedBlue()
        {
            if(RGBChannelsCount == 3)
            for (int y = 0; y < SizeY; y++)
            {
                int p = 0;
                for (int x = 0; x < SizeX; x++)
                {
                    int r = y * Stride + p;
                    int g = y * Stride + p++;
                    int b = y * Stride + p++;
                    byte bb = bytes[r];
                    bytes[r] = bytes[b];
                    bytes[r] = bb;
                }
            }
            else
                for (int y = 0; y < SizeY; y++)
                {
                    int p = 0;
                    for (int x = 0; x < SizeX; x++)
                    {
                        int r = y * Stride + p;
                        int g = y * Stride + p++;
                        int b = y * Stride + p++;
                        p++;
                        byte bb = bytes[r];
                        bytes[r] = bytes[b];
                        bytes[r] = bb;
                    }
                }
        }
        public static BufferInfo SwitchRedBlue(BufferInfo b)
        {
            if (b.PixelFormat == PixelFormat.Format8bppIndexed || b.PixelFormat == PixelFormat.Format16bppGrayScale)
                throw new ArgumentException("Can't switch Red & Blue in non RGB image");
            BufferInfo bf = new BufferInfo(b.SizeX, b.SizeY, b.PixelFormat, b.bytes, b.Coordinate, b.ID);
            for (int y = 0; y < b.SizeY; y++)
            {
                for (int x = 0; x < b.Stride; x += 3)
                {
                    int i = y * b.Stride + x;
                    byte bb = b.bytes[i + 2];
                    b.bytes[i + 2] = b.bytes[i];
                    b.bytes[i] = bb;
                }
            }
            return bf;
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
        public byte[] GetSaveBytes()
        {
            Bitmap bitmap;
            if (RGBChannelsCount == 1)
                bitmap = (Bitmap)Image;
            else
                bitmap = SwitchRedBlue((Bitmap)Image);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
            BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, PixelFormat);
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
            Bitmap bm = new Bitmap(b.Width, b.Height, PixelFormat.Format32bppRgb);
            Graphics g = Graphics.FromImage(bm);
            g.DrawImage(b, 0, 0);
            return bm;
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
            BufferInfo bf = new BufferInfo(ID, SizeX, SizeY, PixelFormat, bt, Coordinate, ID);
            return bf;
        }
        public void To8Bit()
        {
            Image = AForge.Imaging.Image.Convert16bppTo8bpp((Bitmap)Image);
        }
        public void To16Bit()
        {
            Image = AForge.Imaging.Image.Convert8bppTo16bpp((Bitmap)Image);
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
        public static BioImage Apply2(string id, string id2, string name)
        {
            BioImage c = Table.GetImage(id);
            BioImage c2 = Table.GetImage(id2);
            BioImage img = BioImage.Copy(c);
            try
            {
                Filt f = filters[name];
                for (int i = 0; i < img.Buffers.Count; i++)
                {
                    BaseFilter2 fi = (BaseFilter2)f.filt;
                    fi.OverlayImage = (Bitmap)c2.Buffers[i].Image;
                    img.Buffers[i].Image = f.filt.Apply((Bitmap)img.Buffers[i].Image);
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
            Recorder.AddLine("Filters.Apply2(" + '"' + id + '"' + "," + '"' + id2 + '"' + "," + '"' + name + '"' + ", false, ImageView.viewer.Index");
            return img;
        }
        public static void Init()
        {
            //Base Filters
            Filt f = new Filt("BayerFilter", new BayerFilter(), Filt.Type.Base);
            filters.Add(f.name, f);
            f = new Filt("BayerFilterOptimized", new BayerFilterOptimized(), Filt.Type.Base);
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
            f = new Filt("BottomHat", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("BradleyLocalThresholding", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasCrop", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasFill", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("CanvasMove", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("FillHoles", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
            filters.Add(f.name, f);
            f = new Filt("FlatFieldCorrection", new BackwardQuadrilateralTransformation(), Filt.Type.InPlace);
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
            //f = new Filt("ApplyMask", new ApplyMask(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);
            f = new Filt("BrightnessCorrection", new BrightnessCorrection(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("ChannelFiltering", new ChannelFiltering(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("ColorFiltering", new ColorFiltering(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("ColorRemapping", new ColorRemapping(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("ContrastCorrection", new ContrastCorrection(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("ContrastStretch", new ContrastStretch(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            //f = new Filt("ErrorDiffusionDithering", new ErrorDiffusionDithering(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);
            f = new Filt("EuclideanColorFiltering", new EuclideanColorFiltering(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("GammaCorrection", new GammaCorrection(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("HistogramEqualization", new HistogramEqualization(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("HorizontalRunLengthSmoothing", new HorizontalRunLengthSmoothing(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("HSLFiltering", new HSLFiltering(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("HueModifier", new HueModifier(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Invert", new Invert(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("LevelsLinear", new LevelsLinear(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("LevelsLinear16bpp", new LevelsLinear16bpp(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            //f = new Filt("MaskedFilter", new MaskedFilter(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);
            //f = new Filt("Mirror", new Mirror(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);
            f = new Filt("OrderedDithering", new OrderedDithering(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("OtsuThreshold", new OtsuThreshold(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Pixellate", new Pixellate(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("PointedColorFloodFill", new PointedColorFloodFill(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("PointedMeanFloodFill", new PointedMeanFloodFill(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("ReplaceChannel", new Invert(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("RotateChannels", new LevelsLinear(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("SaltAndPepperNoise", new LevelsLinear16bpp(), Filt.Type.InPlace2);
            filters.Add(f.name, f);

            f = new Filt("SaturationCorrection", new SaturationCorrection(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("Sepia", new Sepia(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("SimplePosterization", new SimplePosterization(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("SISThreshold", new SISThreshold(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            //f = new Filt("Texturer", new Texturer(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);
            //f = new Filt("Threshold", new Threshold(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);
            f = new Filt("ThresholdWithCarry", new ThresholdWithCarry(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("VerticalRunLengthSmoothing", new VerticalRunLengthSmoothing(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("YCbCrFiltering", new YCbCrFiltering(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            f = new Filt("YCbCrLinear", new YCbCrLinear(), Filt.Type.InPlace2);
            filters.Add(f.name, f);
            //f = new Filt("YCbCrReplaceChannel", new YCbCrReplaceChannel(), Filt.Type.InPlace2);
            //filters.Add(f.name, f);


        }

    }

    public class Histogram
    {
        private int[] values;
        public int[] Values
        {
            get { return values; }
            set { values = value; }
        }
        private RGB type;
        public RGB Type
        {
            get
            {
                return type;
            }
        }
        public static Histogram FromBytes(byte[] bts, int w, int h, int bitstPerPixel, int stride)
        {
            Histogram histogram = new Histogram();
            if (bitstPerPixel == 16)
            {
                int[] inds = new int[65535];
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x+=2)
                    {
                        ushort s = BitConverter.ToUInt16(bts,(y * stride) + x);
                        inds[s]++;
                    }
                }
                histogram.values = inds;
            }
            else
            {
                int[] inds = new int[65535];
                for (int y = 0; y < h; y++)
                {
                    for (int x = 0; x < w; x++)
                    {
                        byte s = bts[(y * stride) + x];
                        inds[s]++;
                    }
                }
                histogram.values = inds;
            }
            return histogram;
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
    public class BioImage
    {
        public int HashID
        {
            get
            {
                return id.GetHashCode();
            }
        }
        public int[,,] Coords;
        public Hashtable fileHashTable = new Hashtable();
        public Random random = new Random();
        public ImageReader reader;
        public loci.formats.ImageWriter imageWriter;
        public loci.formats.meta.IMetadata meta;

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
        public int rGBChannelCount = 1;
        public int RGBChannelCount
        {
            get
            {
                    return rGBChannelCount;
            }
            set { rGBChannelCount = value; }
        }
        public int bitsPerPixel;
        public int serie = 0;
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
        
        public void To8Bit()
        {

        }
        public void To16Bit()
        {

        }
        public void To24Bit()
        {

        }
        public void To32Bit()
        {

        }
        public void To42Bit()
        {

        }
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
            bi.serie = b.serie;
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
                bi.Channels.Add(c);
            }

            bi.Coords = b.Coords;
            bi.sizeZ = b.sizeZ;
            bi.sizeC = b.sizeC;
            bi.sizeT = b.sizeT;
            bi.serie = b.serie;
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

        public BioImage(string id)
        {
            ID = Table.GetImageName(id);
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
            Table.AddImage(this);
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
            RGBChannelCount = orig.RGBChannelCount;
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
                        int ind = Coords[zs + zi, cs + ci, ts + ti];
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
            Recorder.AddLine("BioImage.Substack(" + orig.Filename + "," + ser + "," + "," + zs + "," + ze + "," + cs + "," + ce + "," + ts + "," + te + ");");
        }
        public BioImage(string file, int ser)
        {
            ID = Table.GetImageName(file);
            serie = ser;
            if (file.EndsWith("ome.tif"))
            {
                OpenOME(file, ser);
                Recorder.AddLine("BioImage.OpenOME(" + file + "," + ser + ");");
            }
            else
            if (file.EndsWith("tif") || file.EndsWith("tiff") || file.EndsWith("png") || file.EndsWith("bmp") || file.EndsWith("jpg") || file.EndsWith("jpeg") || file.EndsWith("gif") ||
                file.EndsWith("TIF") || file.EndsWith("TIFF") || file.EndsWith("PNG") || file.EndsWith("BMP") || file.EndsWith("JPG") || file.EndsWith("JPEG") || file.EndsWith("GIF"))
            {
                Open(file);
                Recorder.AddLine("BioImage.Open(" + file + ");");
            }
            else
            {
                OpenOME(file, ser);
                Recorder.AddLine("BioImage.OpenOME(" + file + "," + ser + ");");
            }
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
            rgbBitmap16 = new Bitmap(SizeX, SizeY, System.Drawing.Imaging.PixelFormat.Format48bppRgb);
            rgbBitmap8 = new Bitmap(SizeX, SizeY, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            Table.AddImage(this);
        }
        public static BioImage MergeChannels(BioImage b2, BioImage b)
        {
            Recorder.AddLine("MergeChannels(" + '"' + b.Filename + '"' + "," + '"' + b2.Filename + '"' + ");");
            BioImage res = new BioImage(b2.ID, b2.SizeX, b2.SizeY);
            res.ID = Table.GetImageName(b2.ID);
            res.serie = b2.serie;
            res.sizeZ = b2.SizeZ;
            int cOrig = b2.SizeC;
            res.sizeC = b2.SizeC + b.SizeC;
            res.sizeT = b2.SizeT;
            res.RGBChannelCount = b2.RGBChannelCount;
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
            return res;
        }
        public static BioImage MergeChannels(string bname, string b2name)
        {
            BioImage b = Table.GetImage(bname);
            BioImage b2 = Table.GetImage(b2name);
            Recorder.AddLine("BioImage.MergeChannels(" + '"' + bname + '"' + "," + '"' + b2name + '"' + ");");
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
        public void SplitChannels(bool showDialog)
        {
            Recorder.AddLine("BioImage.SplitChannels(" + '"' + Filename + '"' + "," + showDialog + ");");
            for (int c = 0; c < SizeC; c++)
            {
                BioImage b = new BioImage(this, 0, 0, SizeZ, c, c + 1, 0, SizeT);
                ImageViewer iv = new ImageViewer(b);
                if (showDialog)
                    iv.ShowDialog();
                else
                    iv.Show();
            }
        }
        public static void SplitChannels(BioImage bb, bool showDialog)
        {
            bb.SplitChannels(showDialog);
        }
        public static void SplitChannels(string name, bool showDialog)
        {
            SplitChannels(Table.GetImage(name), showDialog);
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
                            images = int.Parse(sp[1]);
                        if (sp[0] == "channels")
                            channels = int.Parse(sp[1]);
                        if (sp[0] == "slices")
                            slices = int.Parse(sp[1]);
                        if (sp[0] == "frames")
                            frames = int.Parse(sp[1]);
                        if (sp[0] == "hyperstack")
                            hyperstack = bool.Parse(sp[1]);
                        if (sp[0] == "mode")
                            mode = sp[1];
                        if (sp[0] == "unit")
                            unit = sp[1];
                        if (sp[0] == "finterval")
                            finterval = double.Parse(sp[1]);
                        if (sp[0] == "spacing")
                            spacing = double.Parse(sp[1]);
                        if (sp[0] == "loop")
                            loop = bool.Parse(sp[1]);
                        if (sp[0] == "min")
                            min = double.Parse(sp[1]);
                        if (sp[0] == "max")
                            max = double.Parse(sp[1]);
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
            get { return Buffers[0].SizeX; }
        }
        public int SizeY
        {
            get { return Buffers[0].SizeY; }
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
            if (!littleEndian)
            {
                x = (SizeX - 1) - x;
                y = (SizeY - 1) - y;
                if (bitsPerPixel > 8)
                {
                    return (y * stridex + x) * 2;
                }
                else
                {
                    return (y * stridex + x);
                }
            }
            else
            {
                if (bitsPerPixel > 8)
                {
                    return (y * stridex + x) * 2;
                }
                else
                {
                    return (y * stridex + x);
                }
            }
        }
        public int GetIndexRGB(int ix, int iy, int index)
        {
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (!littleEndian)
            {
                x = (SizeX - 1) - x;
                y = (SizeY - 1) - y;
                if (bitsPerPixel > 8)
                {
                    return (y * stridex + x) * 2 * index;
                }
                else
                {
                    return (y * stridex + x) * index;
                }
            }
            else
            {
                if (bitsPerPixel > 8)
                {
                    return (y * stridex + x) * 2 * index;
                }
                else
                {
                    return (y * stridex + x) * index;
                }
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
                    return GetValueRGB(coord, coord.X, coord.Y, 0);
                else if (coord.C == 1)
                    return GetValueRGB(coord, coord.X, coord.Y, 1);
                else if (coord.C == 2)
                    return GetValueRGB(coord, coord.X, coord.Y, 2);
            }
            else
                return GetValueRGB(coord, coord.X, coord.Y, 0);
            return 0;
        }
        public ushort GetValueRGB(ZCTXY coord, int ix, int iy, int index)
        {
            int i = -1;
            int ind = Coords[coord.Z, coord.C, coord.T];
            byte[] bytes = Buffers[ind].Bytes;
            int stridex = SizeX;
            //For 16bit (2*8bit) images we multiply buffer index by 2
            int x = ix;
            int y = iy;
            if (bitsPerPixel > 8)
            {
                int index2 = (y * stridex + x) * 2 * index;
                i = BitConverter.ToUInt16(bytes, index2);
                return (ushort)i;
            }
            else
            {
                
                int stride = SizeX;
                System.Drawing.Color c = ((Bitmap)Buffers[index].Image).GetPixel(ix, iy);
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
        public ushort GetValue(ZCTXY coord,int ix, int iy)
        {
            int ind = Coords[coord.Z, coord.C, coord.T];
            byte[] bytes = Buffers[ind].Bytes;
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
            if (bitsPerPixel > 8)
            {
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
                if (RGBindex == 0)
                    return (GetBitmap(c.Z, c.C, c.T)).GetPixel(c.X, c.Y).R;
                else if (RGBindex == 1)
                    return (GetBitmap(c.Z, c.C, c.T)).GetPixel(c.X, c.Y).G;
                else if (RGBindex == 2)
                    return (GetBitmap(c.Z, c.C, c.T)).GetPixel(c.X, c.Y).B;
            }
            else
                return ((Bitmap)GetBitmap(coord.Z, coord.C, coord.T)).GetPixel(SizeX, SizeY).R;
            return 0;
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
        public void SetValueRGB( int z, int c, int t, int x, int y, int RGBindex, ushort value)
        {
            try
            {
                if(RGBindex == 0)
                    GetBitmap(z,c,t).SetPixel(x,y,System.Drawing.Color.FromArgb(value, 0, 0, 0));
                else if(RGBindex == 1)
                    GetBitmap(z, c, t).SetPixel(x, y, System.Drawing.Color.FromArgb(0, value, 0, 0));
                else if(RGBindex == 2)
                    GetBitmap(z, c, t).SetPixel(x, y, System.Drawing.Color.FromArgb(0, 0, value, 0));
                else if (RGBindex == 3)
                    GetBitmap(z, c, t).SetPixel(x, y, System.Drawing.Color.FromArgb(0, 0, 0, value));
            }
            catch (Exception)
            {

                throw;
            }
            
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
                //return GetFiltered(SizeX, SizeY, Buffers[ind].Stride, Bytes[ind], r);
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
                if (SizeC > 0)
                    replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, GetFiltered(ri + RChannel.Index, rf, gf, bf));
                if (SizeC > 1)
                    replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, GetFiltered(ri + GChannel.Index, rf, gf, bf));
                if (SizeC > 2)
                    replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, GetFiltered(ri + BChannel.Index, rf, gf, bf));
            }

            if (RGBChannelCount == 1)
            {
                if (SizeC > 0)
                {
                    replaceRFilter.ChannelImage = GetFiltered(ri + RChannel.Index, rf, gf, bf);
                    replaceRFilter.ApplyInPlace(rgbBitmap16);
                    replaceRFilter.ChannelImage.Dispose();
                }
                if (SizeC > 1)
                {
                    replaceGFilter.ChannelImage = GetFiltered(ri + GChannel.Index, gf, gf, bf);
                    replaceGFilter.ApplyInPlace(rgbBitmap16);
                    replaceGFilter.ChannelImage.Dispose();
                }
                if (SizeC > 2)
                {
                    replaceBFilter.ChannelImage = GetFiltered(ri + BChannel.Index, bf, gf, bf);
                    replaceBFilter.ApplyInPlace(rgbBitmap16);
                    replaceBFilter.ChannelImage.Dispose();
                }
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
        public static void Save(BioImage im, string file, int ser)
        {
            im.SaveOME(file, ser);
        }
        public static void Save(string id, string file, int ser)
        {
            BioImage b = Table.GetImage(id);
            b.SaveOME(file, ser);
        }
        public bool SaveOME(string file, int series)
        {
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            loci.formats.meta.IMetadata omexml = service.createOMEXMLMetadata();
            omexml.setImageID("Image:0", series);
            omexml.setPixelsID("Pixels:0", series);
            if (littleEndian)
                omexml.setPixelsBinDataBigEndian(java.lang.Boolean.TRUE, 0, 0);
            else
                omexml.setPixelsBinDataBigEndian(java.lang.Boolean.FALSE, 0, 0);

            omexml.setPixelsDimensionOrder(ome.xml.model.enums.DimensionOrder.XYCZT, series);
            if (bitsPerPixel > 8)
                omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT16, series);
            else
                omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT8, series);
            omexml.setPixelsSizeX(new PositiveInteger(java.lang.Integer.valueOf(SizeX)), series);
            omexml.setPixelsSizeY(new PositiveInteger(java.lang.Integer.valueOf(SizeY)), series);
            omexml.setPixelsSizeZ(new PositiveInteger(java.lang.Integer.valueOf(SizeZ)), series);
            int samples = 1;
            if (isRGB)
                samples = 3;
            omexml.setPixelsSizeC(new PositiveInteger(java.lang.Integer.valueOf(SizeC * samples)), series);
            omexml.setPixelsSizeT(new PositiveInteger(java.lang.Integer.valueOf(SizeT)), series);

            if (physicalSizeX != -1)
            {
                ome.units.quantity.Length p = new ome.units.quantity.Length(java.lang.Double.valueOf(physicalSizeX), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeX(p, series);
            }
            if (physicalSizeY != -1)
            {
                ome.units.quantity.Length p = new ome.units.quantity.Length(java.lang.Double.valueOf(physicalSizeY), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeY(p, series);
            }
            if (physicalSizeZ != -1)
            {
                ome.units.quantity.Length p = new ome.units.quantity.Length(java.lang.Double.valueOf(physicalSizeZ), ome.units.UNITS.MICROMETER);
                omexml.setPixelsPhysicalSizeZ(p, series);
            }
            if (stageSizeX != -1)
            {
                ome.units.quantity.Length s = new ome.units.quantity.Length(java.lang.Double.valueOf(stageSizeX), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelX(s, series);
            }
            if (stageSizeY != -1)
            {
                ome.units.quantity.Length s = new ome.units.quantity.Length(java.lang.Double.valueOf(stageSizeY), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelY(s, series);
            }
            if (stageSizeX != -1)
            {
                ome.units.quantity.Length s = new ome.units.quantity.Length(java.lang.Double.valueOf(stageSizeZ), ome.units.UNITS.MICROMETER);
                omexml.setStageLabelZ(s, series);
            }

            for (int channel = 0; channel < Channels.Count; channel++)
            {
                Channel c = Channels[channel];
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
                    ome.xml.model.enums.IlluminationType cm = (ome.xml.model.enums.IlluminationType)Enum.Parse(typeof(ome.xml.model.enums.IlluminationType), c.IlluminationType);
                    omexml.setChannelIlluminationType(cm, series, channel);
                }
                if (c.LightSourceIntensity != -1)
                {
                    ome.units.quantity.Power fl = new ome.units.quantity.Power(java.lang.Double.valueOf(c.LightSourceIntensity), ome.units.UNITS.VOLT);
                    omexml.setLightEmittingDiodePower(fl, series, channel);
                }
            }

            int i = 0;
            foreach (Annotation an in Annotations)
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
                    meta.setPointFontSize(fl, i, series);
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

            //Method used to save a range of an image stack defined by start & count.
            loci.formats.ImageWriter writer = new loci.formats.ImageWriter();
            writer.setMetadataRetrieve(omexml);
            //We delete the file so we don't just add more images to an existing file;
            if (File.Exists(file))
                File.Delete(file);
            writer.setId(file);
            writer.setSeries(series);
            writer.setWriteSequentially(true);

            wr = writer;
            threadFile = Path.GetFileName(file);
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(SaveOME));
            t.Start();
            threadImage = this;
            Progress pr = new Progress(threadFile, "Saving");
            pr.Show();
            do
            {
                pr.UpdateProgress((int)threadProgress);
                Application.DoEvents();
            } while (!done);
            pr.Close();
            pr.Dispose();
            Recorder.AddLine("BioImage.SaveOME(" + '"' + Filename + '"' + "," + '"' + file + '"' + "," + series + ");");
            return true;
        }

        private static BioImage threadImage = null;
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
            do
            {
                pr.UpdateProgress((int)threadProgress);
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
            do
            {
                pr.UpdateProgress((int)threadProgress);
                Application.DoEvents();
            } while (!done);
            pr.Close();

        }
        private static void Save()
        {
            string file = threadFile;
            BioImage b = threadImage;
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
            using (Tiff image = Tiff.Open(file, "w"))
            {
                int stride = b.Buffers[0].Stride;
                int im = 0;
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
                            if (im % 2 == 0)
                                image.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISBLACK);
                            else
                                image.SetField(TiffTag.PHOTOMETRIC, Photometric.MINISWHITE);
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
                            Image ima = b.Buffers[im].Image;
                            byte[] buffer = BufferInfo.GetBuffer((Bitmap)ima);
                            image.SetField(TiffTag.PAGENUMBER, c, b.Buffers.Count);
                            for (int i = 0, offset = 0; i < b.SizeY; i++)
                            {
                                image.WriteScanline(buffer, offset, i, 0);
                                offset += stride;
                            }
                            image.WriteDirectory();
                            threadProgress = ((float)im / (float)b.ImageCount) * 100;
                            im++;
                        }
                    }
                }
            }
            done = true;
        }
        private static void Open()
        {
            string file = threadFile;
            BioImage b = threadImage;
            b.serie = 0;
            done = false;
            string fn = Path.GetFileNameWithoutExtension(file);
            string dir = Path.GetDirectoryName(file);
            if (File.Exists(fn + ".csv"))
            {
                string f = dir + "//" + fn + ".csv";
                b.Annotations = BioImage.ImportROIsCSV(f);
            }
            if(file.EndsWith("tif") || file.EndsWith("tiff") || file.EndsWith("TIF") || file.EndsWith("TIFF"))
                using (Tiff image = Tiff.Open(file, "r"))
            {
                int SizeX = image.GetField(TiffTag.IMAGEWIDTH)[0].ToInt();
                int SizeY = image.GetField(TiffTag.IMAGELENGTH)[0].ToInt();
                b.bitsPerPixel = image.GetField(TiffTag.BITSPERSAMPLE)[0].ToInt();
                b.littleEndian = image.IsBigEndian();
                b.RGBChannelCount = image.GetField(TiffTag.SAMPLESPERPIXEL)[0].ToInt();
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
                if (b.RGBChannelCount == 1)
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
                if (b.RGBChannelCount == 3)
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
                    PixelFormat = PixelFormat.Format32bppRgb;

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
                int cs = b.RGBChannelCount;
                    b.Coords = new int[b.SizeZ, b.SizeC, b.SizeT];

                if(b.RGBChannelCount == 1)
                for (int i = 0; i < b.SizeC; i++)
                {
                    Channel ch = new Channel(i, b.bitsPerPixel);
                    b.Channels.Add(ch);
                }
                else
                    for (int i = 0; i < b.RGBChannelCount; i++)
                    {
                        Channel ch = new Channel(i, b.bitsPerPixel);
                        b.Channels.Add(ch);
                    }

                int z = 0;
                int c = 0;
                int t = 0;
                b.Buffers = new List<BufferInfo>();

                if (b.bitsPerPixel == 8)
                {
                    Bitmap bitmap = (Bitmap)Image.FromFile(file);
                    int pages = bitmap.GetFrameCount(FrameDimension.Page);
                    for (int p = 0; p < pages; p++)
                    {
                        // save each frame to a bytestream
                        bitmap.SelectActiveFrame(FrameDimension.Page, p);
                        MemoryStream byteStream = new MemoryStream();
                        bitmap.Save(byteStream, ImageFormat.Tiff);
                        BufferInfo inf = new BufferInfo(file, Image.FromStream(byteStream), new ZCT(0, 0, 0), 0);
                        b.Buffers.Add(inf);
                        threadProgress = (int)((double)p / (double)pages);
                    }
                }
                else
                {
                    int numberOfStrips = image.NumberOfStrips();
                    int pages = image.NumberOfDirectories();
                    for (int p = 0; p < pages; p++)
                    {
                        image.SetDirectory((short)p);

                        byte[] bytes = new byte[image.ScanlineSize() * SizeY];
                        for (int i = 0, offset = 0; i < SizeY; i++)
                        {
                            image.ReadScanline(bytes, offset, i, 0);
                            offset += image.ScanlineSize();
                        }
                        BufferInfo inf = new BufferInfo(file, SizeX, SizeY, PixelFormat, bytes, new ZCT(0, 0, 0),0);
                        b.Buffers.Add(inf);
                        threadProgress = (int)((double)p / (double)pages);
                    }
                }
                for (int im = 0; im < b.Buffers.Count; im++)
                {
                    ZCT co = new ZCT(z, c, t);
                    int index = b.Coords[z, c, t];
                    b.Coords[co.Z, co.C, co.T] = im;
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
            }
            else
            {
                b.bitsPerPixel = 8;
                b.littleEndian = BitConverter.IsLittleEndian;
                Image im = Image.FromFile(file);
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
            done = true;
        }

        private static ImageWriter wr;
        private static string threadFile = "";
        private static bool done = false;
        public static float threadProgress = 0;
        public static void SaveOME()
        {
            threadProgress = 0;
            done = false;
            for (int im = 0; im < threadImage.ImageCount; im++)
            {
                threadImage.Buffers[im].Image.Save(threadFile);
                threadProgress = ((float)im / (float)threadImage.ImageCount) * 100;
            }
            done = true;
            wr.close();
        }
        public static BioImage Open(string file, int ser)
        {
            BioImage res = new BioImage(file, ser);
            return res;
        }
        public void OpenOME(string file, int ser)
        {
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            meta = service.createOMEXMLMetadata();
            // create format reader
            reader = new ImageReader();
            reader.setMetadataStore(meta);
            // initialize file
            reader.setId(file);
            reader.setSeries(ser);
            RGBChannelCount = reader.getRGBChannelCount();
            bitsPerPixel = reader.getBitsPerPixel();
            id = file;
            int SizeX = reader.getSizeX();
            int SizeY = reader.getSizeY(); 
            sizeC = reader.getSizeC();
            sizeZ = reader.getSizeZ();
            sizeT = reader.getSizeT();
            littleEndian = reader.isLittleEndian();
            seriesCount = reader.getSeriesCount();
            imagesPerSeries = reader.getImageCount() / seriesCount;
            Coords = new int[SizeZ, SizeC, SizeT];
            PixelFormat PixelFormat;
            int stride = 0;
            if (RGBChannelCount == 1)
            {
                if (bitsPerPixel > 8)
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
                if (bitsPerPixel > 8)
                {
                    PixelFormat = PixelFormat.Format48bppRgb;
                    stride = SizeX * 2 * 3;
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

            //Lets get the channels amd initialize them.
            for (int i = 0; i < SizeC; i++)
            {
                Channel ch = new Channel(i, reader.getBitsPerPixel());
                try
                {
                    if (meta.getChannelName(0, i) != null)
                        ch.Name = meta.getChannelName(0, i);
                    if (meta.getChannelSamplesPerPixel(0, i) != null)
                    {
                        int s = meta.getChannelSamplesPerPixel(0, i).getNumberValue().intValue();
                        ch.SamplesPerPixel = s;
                    }
                    if (meta.getChannelID(0, i) != null)
                        ch.ID = meta.getChannelID(0, i);
                    if (meta.getChannelFluor(0, i) != null)
                        ch.Fluor = meta.getChannelFluor(0, i);
                    if (meta.getChannelColor(0, i) != null)
                    {
                        ome.xml.model.primitives.Color cc = meta.getChannelColor(0, i);
                        ch.color = System.Drawing.Color.FromArgb(cc.getRed(), cc.getGreen(), cc.getBlue());
                        if (ch.color.Value.R == 255 && ch.color.Value.G == 0 && ch.color.Value.B == 0)
                            ch.rgb = RGB.R;
                        if (ch.color.Value.R == 0 && ch.color.Value.G == 255 && ch.color.Value.B == 0)
                            ch.rgb = RGB.G;
                        if (ch.color.Value.R == 0 && ch.color.Value.G == 0 && ch.color.Value.B == 255)
                            ch.rgb = RGB.B;
                    }
                    if (meta.getChannelIlluminationType(0, i) != null)
                        ch.IlluminationType = meta.getChannelIlluminationType(0, i).toString();
                    if (meta.getChannelContrastMethod(0, i) != null)
                        ch.ContrastMethod = meta.getChannelContrastMethod(0, i).toString();
                    if (meta.getPlaneExposureTime(0, i) != null)
                        ch.Exposure = meta.getPlaneExposureTime(0, i).value().intValue();
                    if (meta.getChannelEmissionWavelength(0, i) != null)
                        ch.Emission = meta.getChannelEmissionWavelength(0, i).value().intValue();
                    if (meta.getChannelExcitationWavelength(0, i) != null)
                        ch.Excitation = meta.getChannelExcitationWavelength(0, i).value().intValue();
                    if (meta.getChannelLightSourceSettingsAttenuation(0, i) != null)
                        ch.LightSourceIntensity = meta.getChannelLightSourceSettingsAttenuation(0, i).getNumberValue().doubleValue();
                    if (meta.getLightEmittingDiodePower(0, i) != null)
                        ch.LightSourceIntensity = meta.getLightEmittingDiodePower(0, i).value().doubleValue();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                if (i == 0)
                {
                    rgbChannels[0] = 0;
                    ch.rgb = RGB.R;
                }
                else
                if (i == 1)
                {
                    rgbChannels[1] = 1;
                    ch.rgb = RGB.G;
                }
                else
                if (i == 2)
                {
                    rgbChannels[2] = 2;
                    ch.rgb = RGB.B;
                }
                Channels.Add(ch);
            }

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
                        if (ImageCount > 1)
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
                        if (ImageCount > 1)
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
                        if (ImageCount > 1)
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
                        if (ImageCount > 1)
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
                        if (ImageCount > 1)
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
                        if (ImageCount > 1)
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

                        if (ImageCount > 1)
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

            fileHashTable.Add(file, file.GetHashCode());

            List<string> serFiles = new List<string>();
            serFiles.AddRange(reader.getSeriesUsedFiles());
            //List<BufferInfo> BufferInfos = new List<BufferInfo>(); 
            //List<string> Files = new List<string>();
            Buffers = new List<BufferInfo>();
            // read the image data bytes
            int numberOfStrips = SizeY;
            for (int p = 0; p < reader.getImageCount(); p++)
            {
                byte[] bytes = reader.openBytes(p);
                BufferInfo inf = new BufferInfo(file, SizeX, SizeY, PixelFormat, bytes, new ZCT(0, 0, 0), 0);
                Buffers.Add(inf);
                threadProgress = (int)((double)p / (double)reader.getImageCount());
            }
            int z = 0;
            int c = 0;
            int t = 0;
            for (int im = 0; im < Buffers.Count; im++)
            {
                ZCT co = Buffers[im].Coordinate;
                Coords[co.Z, co.C, co.T] = im;
                
            }
            //pr.UpdateProgressF((float)(im) / (float)images.Count);
            Application.DoEvents();
            //threadProgress = ((float)im / (float)b.ImageCount) * 100;
        
            double stx = 0;
            double sty = 0;
            double stz = 0;
            double six = 0;
            double siy = 0;
            double siz = 0;

            try
            {
                if (meta.getPixelsPhysicalSizeX(ser) != null)
                    physicalSizeX = meta.getPixelsPhysicalSizeX(ser).value().doubleValue();
                if (meta.getPixelsPhysicalSizeY(ser) != null)
                    physicalSizeY = meta.getPixelsPhysicalSizeY(ser).value().doubleValue();
                if (meta.getPixelsPhysicalSizeZ(ser) != null)
                    physicalSizeZ = meta.getPixelsPhysicalSizeZ(ser).value().doubleValue();

                //Calling these when they are not defined causes an error so we use the try catch block.
                if (meta.getStageLabelX(ser) != null)
                    stageSizeX = meta.getStageLabelX(ser).value().doubleValue();
                if (meta.getStageLabelY(ser) != null)
                    stageSizeY = meta.getStageLabelY(ser).value().doubleValue();
                if (meta.getStageLabelZ(ser) != null)
                    stageSizeZ = meta.getStageLabelZ(ser).value().doubleValue();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            try
            {
                Volumes.Add(new VolumeD(new Point3D(stx, sty, stz), new Point3D(six * SizeX, siy * SizeY, siz * SizeZ)));
            }
            catch (Exception)
            {
                //Volume is used only for stage coordinates if error is thrown it is because this image doens't have any size information or it is incomplete as read by Bioformats.
            }
            Table.AddImage(this);
            reader.close();
            //pr.Close();
            Recorder.AddLine("BioImage.OpenOME(" + '"' + file + '"' + "," + ser + ");");
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
        public bool AutoThresholdChannel(Channel c1)
        {
            if (bitsPerPixel > 8)
            {
                for (int time = 0; time < SizeT; time++)
                {
                    for (int z = 0; z < SizeZ; z++)
                    {
                        int i = Coords[z, c1.Index, time];
                        int index2, x, y;
                        int stride2 = SizeX * RGBChannelCount;
                        for (y = 0; y < SizeY; y++)
                        {
                            for (x = 0; x < SizeX; x++)
                            {
                                //For 16bit (2*8bit) images we multiply buffer index by 2
                                index2 = (y * stride2 + (x * RGBChannelCount)) * 2;
                                int px = BitConverter.ToUInt16(Buffers[i].Bytes, index2);
                                if (px > c1.Max)
                                    c1.Max = px;
                            }
                        }
                    }
                }
            }
            else
                for (int time = 0; time < SizeT; time++)
                {
                    for (int z = 0; z < SizeZ; z++)
                    {
                        int i = Coords[z, c1.Index, time];
                        int index, x, y;
                        int stride = SizeX * RGBChannelCount;
                        for (y = 0; y < SizeY; y++)
                        {
                            for (x = 0; x < SizeX; x++)
                            {
                                //For 16bit (2*8bit) images we multiply buffer index by 2
                                index = (y * stride + (x * RGBChannelCount));
                                int px = Buffers[i].Bytes[index];
                                if (px > c1.Max)
                                    c1.Max = px;
                            }
                            
                        }
                    }
                }

            return true;
        } 
        public void AutoThreshold()
        {
            if(bitsPerPixel>8)
            foreach (Channel c in Channels)
            {
                c.Max = 0;
                AutoThresholdChannel(c);
            }
            //RefreshPlane();
        }
        public void Dispose()
        {
            Table.RemoveImage(this);
            for (int i = 0; i < Buffers.Count; i++)
            {
                Buffers[i].Image.Dispose();
            }
        }
        public override string ToString()
        {
            return Filename.ToString();
        }

    }

}
