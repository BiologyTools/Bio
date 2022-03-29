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

namespace BioImage
{
    public static class Table
    {
        private static Hashtable hashID = new Hashtable();
        private static Hashtable buffers = new Hashtable();
        private static Hashtable bioimages = new Hashtable();


        public static object GetObj(int hashid)
        {
            return buffers[hashid];
        }

        public static BioImage GetImageByID(string ids)
        {
            int hash = ids.GetHashCode();
            return (BioImage)bioimages[hash];
        }
        public static BioImage GetImageByHash(int hash, BioImage.SZCT coord)
        {
            return (BioImage)bioimages[hash];
        }

        public static BioImage.Buf GetBufferByHash(int hashid)
        {
            return (BioImage.Buf)buffers[hashid];
        }
        public static BioImage.Buf GetBufferByID(string id)
        {
            int hash = id.GetHashCode();
            return (BioImage.Buf)buffers[hash];
        }

        public static string GetStringByHash(int hashid)
        {
            return buffers[hashid].ToString();
        }
        public static string GetStringByID(string id)
        {
            int hash = id.GetHashCode();
            return buffers[hash].ToString();
        }

        public static void AddBufferByHash(int hashid, BioImage.Buf buf)
        {
            buffers.Add(hashid, buf);
        }
        public static void AddBufferByID(string id, BioImage.Buf buf)
        {
            int hash = id.GetHashCode();
            AddBufferByHash(hash, buf);
        }

        public static void AddBioImageByHash(int hashid, BioImage im)
        {
            buffers.Add(hashid, im);
        }
        public static void AddBioImageByID(string id, BioImage im)
        {
            int hash = id.GetHashCode();
            AddBioImageByHash(hash, im);
        }

    }

    public class BioImage
    {
        public const char sep = '/';
        public int GetHash(int s, int z, int c, int t)
        {
            return CreateHash(idString, s, Coords[s, z, c, t]);
        }
        public int GetHash(SZCT coord)
        {
            int h = CreateHash(idString, coord.S, Coords[coord.S, coord.Z, coord.C, coord.T]);
            return h;
        }

        public static int CreateHash(string filepath, int ser, int index)
        {
            filepath = filepath.Replace("\\", "/");
            string ids = CreateID(filepath, ser, index);
            return ids.GetHashCode();
        }
        public static string CreateID(string filepath, int ser, int index)
        {
            filepath = filepath.Replace("\\", "/");
            string s = filepath + sep + 's' + sep + ser + sep + 'i' + sep + index;
            return s;
        }

        public int[] GetSZCRTCoordInts(int i)
        {
            Buf bf = Buffers[i];
            int[] ints = new int[4];
            ints[0] = bf.serie;
            ints[1] = bf.info.Zcoord;
            ints[2] = bf.info.Ccoord;
            ints[3] = bf.info.Tcoord;
            return ints;
        }
        public SZCT GetSZCRTCoords(int i)
        {
            Buf bf = Buffers[i];
            SZCT val = new SZCT(bf.serie, bf.info.Zcoord, bf.info.Ccoord, bf.info.Tcoord);
            return val;
        }

        public int HashID
        {
            get
            {
                return idString.GetHashCode();
            }
        }
        int[,,,] Coords;
        public Hashtable bufferTable;
        public Hashtable fileHashTable;
        public Random _random = new Random();
        public ImageReader reader;
        public loci.formats.ImageWriter imageWriter;
        public loci.formats.meta.IMetadata meta;
        public List<Buf> Buffers = new List<Buf>();
        public List<Channel> Channels = new List<Channel>();
        public List<VolumeD> Volumes = new List<VolumeD>();
        public List<Annotation> Annotations = new List<Annotation>();
        public string idString;
        public int[] rgbChannels = new int[3];
        public int rGBChannelCount = 1;
        public int bitsPerPixel;
        private PixelFormat px;
        public bool convertedToLittleEndian = false;
        public string filename;

        public BioImage(int ser, string file)
        {
            serie = ser;
            rgbChannels[0] = 0;
            rgbChannels[1] = 0;
            rgbChannels[2] = 0;
            OpenSeries(file, ser);
            rgbBitmap16 = new Bitmap(SizeX, SizeY, PixelFormat.Format48bppRgb);
            rgbBitmap8 = new Bitmap(SizeX, SizeY, PixelFormat.Format24bppRgb);
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

        public class SZCT
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
                if (c1.S != c2.S || c1.Z != c2.Z || c1.C != c2.C || c1.T != c2.T)
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
                if (c1.S == c2.S && c1.Z == c2.Z && c1.C == c2.C && c1.T == c2.T && c1.X == c2.X && c2.Y == c2.Y)
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

        public int serie = 0;
        public int imageCount = 0;
        public int imagesPerSeries = 0;
        public int seriesCount = 0;
        public bool littleEndian = false;
        public bool isGroup = false;
        public long loadTimeMS = 0;
        public long loadTimeTicks = 0;
        private int sizeX, sizeY, sizeZ, sizeC, sizeT;
        public int SizeX
        {
            get { return sizeX; }
            set
            {
                sizeX = value;
            }
        }
        public int SizeY
        {
            get { return sizeY; }
            set
            {
                sizeY = value;
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

        public int RGBChannelCount
        {
            get
            {
                return rGBChannelCount;
            }
        }
        public PixelFormat PixelFormat
        {
            get
            {
                return px;
            }
        }
        public bool isRGB
        {
            get
            {
                if (rGBChannelCount == 3)
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

        public ushort GetValue(SZCTXY coord)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetValue(coord.X, coord.Y);
        }
        public ushort GetValue(int s, int z, int c, int t, int x, int y)
        {
            return GetBufByCoord(s, z, c, t).GetValue(x, y);
        }
        public ushort GetValue(SZCT coord, int x, int y)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetValue(x, y);
        }
        public ushort GetValue(SZCT coord, int x, int y, int RGBindex)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetValue(x, y, RGBindex);
        }
        public ushort GetValue(int s, int z, int c, int t, int x, int y, int RGBindex)
        {
            return GetBufByCoord(s, z, c, t).GetValue(x, y, RGBindex);
        }

        public void SetValue(SZCTXY coord, ushort val)
        {
            GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).SetValue(coord.X, coord.Y, val);
        }
        public void SetValue(int ix, int iy, int ind, ushort value)
        {
            Buffers[ind].SetValue(ix, iy, value);
        }
        public void SetValue(int ix, int iy, SZCT coord, ushort value)
        {
            int ind = Coords[coord.S, coord.Z, coord.C, coord.T];
            Buffers[ind].SetValue(ix, iy, value);
        }

        public Buf GetBufByCoord(int s, int z, int c, int t)
        {
            return Buffers[Coords[s, z, c, t]];
        }
        public Buf GetBufByCoord(SZCT coord)
        {
            return Buffers[Coords[coord.S, coord.Z, coord.C, coord.T]];
        }

        public Bitmap GetImageRGB(SZCT coord, IntRange rr, IntRange rg, IntRange rb)
        {
            if(bitsPerPixel > 8)
            {
                return AForge.Imaging.Image.Convert16bppTo8bpp(GetRGBBitmap16(coord, rr,rg, rb));
            }
            else
            {
                return GetRGBBitmap8(coord);
            }
        }
        public Bitmap GetImageRGB(SZCT coord)
        {
            return GetRGBBitmap8(coord);
        }
        public Bitmap GetImageFiltered(SZCT coord, IntRange rr)
        {
            if (bitsPerPixel > 8)
            {
                return AForge.Imaging.Image.Convert16bppTo8bpp(GetFiltered(coord, rr));
            }
            else
            {
                return GetBitmap(coord);
            }
        }
        public Bitmap GetImageRaw(SZCT coord)
        {
            if (bitsPerPixel > 8)
            {
                return AForge.Imaging.Image.Convert16bppTo8bpp(GetBitmap(coord));
            }
            else
            {
                return GetBitmap(coord);
            }
        }

        public Bitmap GetBitmap(SZCT coord)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetBitmap();
        }
        public Bitmap GetFiltered(SZCT coord, IntRange filt)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetFiltered(filt);
        }
        public Bitmap GetFiltered(SZCT coord, IntRange rr, IntRange rg, IntRange rb)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetFiltered(rr, rg, rb);
        }

        private Bitmap rgbBitmap16 = null;
        public Bitmap GetRGBBitmap16(SZCT coord, IntRange rf, IntRange gf, IntRange bf)
        {
            watch.Restart();
            int ri = reader.getIndex(coord.Z, coord.C, coord.T);
            if (RGBChannelCount == 1)
            {
                replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, Buffers[ri + RChannel.index].GetFiltered(rf, gf, bf));
                replaceRFilter.ApplyInPlace(rgbBitmap16);
                replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, Buffers[ri + GChannel.index].GetFiltered(rf, gf, bf));
                replaceGFilter.ApplyInPlace(rgbBitmap16);
                replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, Buffers[ri + BChannel.index].GetFiltered(rf, gf, bf));
                replaceBFilter.ApplyInPlace(rgbBitmap16);
            }
            else
            {
                rgbBitmap16 = Buffers[ri].GetBitmap();
            }
            watch.Stop();
            loadTimeMS = watch.ElapsedMilliseconds;
            loadTimeTicks = watch.ElapsedTicks;
            return rgbBitmap16;
        }
        public Bitmap GetRGBBitmap16(SZCT coord, IntRange rf)
        {
            watch.Restart();
            int ri = reader.getIndex(coord.Z, coord.C, coord.T);
            if (RGBChannelCount == 1)
            {
                replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, Buffers[ri].GetFiltered(rf));
                replaceRFilter.ApplyInPlace(rgbBitmap16);
            }
            else
            {
                rgbBitmap16 = Buffers[ri].GetBitmap();
            }
            watch.Stop();
            loadTimeMS = watch.ElapsedMilliseconds;
            loadTimeTicks = watch.ElapsedTicks;
            return rgbBitmap16;
        }

        private Bitmap rgbBitmap8 = null;
        public Bitmap GetRGBBitmap8(SZCT coord)
        {
            watch.Restart();
            int ri = reader.getIndex(coord.Z, coord.C, coord.T);
            if (RGBChannelCount == 1)
            {
                Bitmap rb = Buffers[ri].GetBitmap();
                Bitmap gb = Buffers[ri + 1].GetBitmap();
                Bitmap bb = Buffers[ri + 2].GetBitmap();

                replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, rb);
                replaceRFilter.ApplyInPlace(rgbBitmap8);
                replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, gb);
                replaceGFilter.ApplyInPlace(rgbBitmap8);
                replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, bb);
                replaceBFilter.ApplyInPlace(rgbBitmap8);
            }
            else
            {
                rgbBitmap8 = Buffers[ri].GetBitmap();
            }
            watch.Stop();
            loadTimeMS = watch.ElapsedMilliseconds;
            loadTimeTicks = watch.ElapsedTicks;
            return rgbBitmap8;
        }

        public static Stopwatch swatch = new Stopwatch();
        public enum RGB
        {
            R,
            G,
            B
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
            public PointF ToPointInt()
            {
                return new System.Drawing.Point((int)X, (int)Y);
            }

            public override string ToString()
            {
                return X.ToString() + ", " + Y.ToString();
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
        public List<Annotation> GetAnnotations(SZCT coord)
        {
            List<Annotation> annotations = new List<Annotation>();
            foreach (Annotation an in Annotations)
            {
                if (an.coord == coord)
                    annotations.Add(an);
            }
            return annotations;
        }
        public List<Annotation> GetAnnotations(int S, int Z, int C, int T)
        {
            List<Annotation> annotations = new List<Annotation>();
            foreach (Annotation an in Annotations)
            {
                if (an.coord.S == S && an.coord.Z == Z && an.coord.Z == Z && an.coord.C == C && an.coord.T == T)
                    annotations.Add(an);
            }
            return annotations;
        }
        public class Annotation
        {
            public float selectBoxSize = 4;
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
            public Type type;
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


            public string name = "";
            public string id = "";
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
            public SZCT coord;
            public System.Drawing.Color strokeColor;
            public System.Drawing.Color fillColor;
            public bool isFilled = false;
            public string roiID = "";
            public string roiName = "";
            private string text = "";
            public double strokeWidth = 1;
            public int shapeIndex = 0;
            public bool closed = false;
            public bool selected = false;
            public string Text
            {
                get
                {
                    return text;
                }
                set
                {
                    text = value;
                    if(type == Type.Label)
                    {
                        UpdateBoundingBox();
                        UpdateSelectBoxs();
                    }
                }
            }
            public RectangleD GetSelectBound()
            {
                double f = selectBoxSize / 2;
                return new RectangleD(BoundingBox.X - f, BoundingBox.Y - f, BoundingBox.W + f, BoundingBox.H + f);
            }
            public Annotation()
            {
                coord = new SZCT(0, 0, 0, 0);
                strokeColor = System.Drawing.Color.Yellow;
                font = SystemFonts.DefaultFont;
                BoundingBox = new RectangleD(0, 0, 1, 1);
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
                //Points.remo
                //List<car> result = GetSomeOtherList().Except(GetTheList()).ToList();
                List<PointD> inds = new List<PointD>();
                for (int i = 0; i < Points.Count; i++)
                {
                    bool found = false;
                    for (int ind = 0; ind < indexs.Length; ind++)
                    {
                        if (ind == i)
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
                    int x = int.Parse(sints[0]);
                    int y = int.Parse(sints[1]);
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
            public void UpdateSelectBoxs()
            {
                float f = selectBoxSize / 2;
                selectBoxs.Clear();
                if(type == Type.Label)
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
                        Size s = TextRenderer.MeasureText(text, font);
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
            public Annotation CreatePolyline(string points)
            {
                Annotation roi = new Annotation();
                roi.type = Type.Polyline;
                roi.AddPoints(stringToPoints(points));
                UpdateSelectBoxs();
                UpdateBoundingBox();
                return roi;
            }
            public Annotation CreateText(Font f, string s, PointD p1)
            {
                Annotation roi = new Annotation();
                roi.type = Type.Label;
                roi.font = f;
                roi.Points.Add(p1);
                return roi;
            }
            public override string ToString()
            {
                return "(" + Point.X + ", " + Point.Y + ") " + coord.ToString() + " " + type.ToString() + " " + roiName;
            }
        }
        public class Channel
        {
            public string Name = "";
            public string ID = "";
            public int index;
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
        public struct BufferInfo
        {
            public static LevelsLinear filter8 = new LevelsLinear();
            public static LevelsLinear16bpp filter16 = new LevelsLinear16bpp();
            public string stringId;
            public int HashID
            {
                get
                {
                    return stringId.GetHashCode();
                }
            }
            public bool littleEndian;
            public bool ConvertedToLittleEndian;
            public int length;
            public int SizeX, SizeY;
            public int Tcoord;
            public int Ccoord;
            public int stride;
            public int series;
            public int Zcoord;

            public int RGBChannelsCount;
            public int bitsPerPixel;
            public PixelFormat pixelFormat;
            public BioImage.SZCT Coordinate;

            public BufferInfo(string filepath, int len, int serie, int w, int h, int strid, int RGBChsCount, int bitsPerPx, PixelFormat px, int tInd, int cInd, int zInd, int index, bool litleEnd, bool convertedToLittleEndian)
            {
                stringId = CreateID(filepath, serie, index);
                littleEndian = litleEnd;
                ConvertedToLittleEndian = convertedToLittleEndian;
                length = len;
                SizeX = w;
                SizeY = h;
                Tcoord = tInd;
                Ccoord = cInd;
                Zcoord = zInd;
                stride = strid;
                series = serie;
                RGBChannelsCount = RGBChsCount;
                bitsPerPixel = bitsPerPx;
                pixelFormat = px;
                Coordinate = new SZCT(serie, zInd, cInd, tInd);
            }

            public override string ToString()
            {
                return stringId;
            }

        }
        public struct Buf
        {
            public BufferInfo info;
            public MemoryMappedFile file;
            public MemoryMappedViewStream stream;
            public BinaryWriter writer;
            public BinaryReader read;
            public int serie;
            public byte[] bytes
            {
                get
                {
                    return GetBytes();
                }
                set
                {
                    SetBytes(value);
                }
            }

            public Buf(BufferInfo inf, byte[] bts)
            {
                info = inf;
                file = MemoryMappedFile.CreateOrOpen(info.ToString(), inf.length);
                stream = file.CreateViewStream(0, inf.length, MemoryMappedFileAccess.ReadWrite);
                read = new BinaryReader(stream);
                writer = new BinaryWriter(stream);
                serie = inf.series;
                bytes = bts;

                if (info.RGBChannelsCount == 3)
                {
                    //RGB Channels are stored in BGR so we switch them to RGB
                    SetBuffer(switchRedBlue(GetBitmap()));
                    //Then turn the 48bpp buffers to 3 RGB 16bpp buffers
                }
                if (!info.littleEndian)
                {
                    //We need to convert this buffer to little endian.
                    ToLittleEndian();
                }

            }

            public static Buf GetFromID(string bid)
            {
                return Table.GetBufferByID(bid);
            }
            public Buf BufFromHash(int hash)
            {
                return Table.GetBufferByHash(hash);
            }
            public Buf BufFromID(string id)
            {
                int hash = id.GetHashCode();
                return BufFromHash(hash);
            }

            public unsafe Buf(BufferInfo inf, int ser, int index)
            {
                info = inf;
                file = MemoryMappedFile.CreateOrOpen(inf.stringId, inf.length);
                stream = file.CreateViewStream(0, inf.length, MemoryMappedFileAccess.ReadWrite);
                read = new BinaryReader(stream);
                writer = new BinaryWriter(stream);
                serie = ser;
                SetBytes(read.ReadBytes(inf.length));
                if (inf.RGBChannelsCount == 3)
                {
                    //RGB Channels are stored in BGR so we switch them to RGB
                    SetBuffer(switchRedBlue(GetBitmap()));
                    //Then turn the 48bpp buffers to 3 RGB 16bpp buffers
                }
                if (!inf.littleEndian)
                {
                    //We need to convert this buffer to little endian.
                    ToLittleEndian();
                }
            }

            public Buf Copy(Buf b)
            {
                info.Zcoord = b.info.Zcoord;
                info.ConvertedToLittleEndian = false;
                info.stringId = b.info.stringId;
                bytes = b.GetBytes();
                info.SizeX = b.info.SizeX;
                info.SizeY = b.info.SizeY;
                info.length = b.info.length;
                info.stride = b.info.stride;
                info.series = b.info.series;
                info.Tcoord = b.info.Tcoord;
                info.Ccoord = b.info.Ccoord;
                info.pixelFormat = b.info.pixelFormat;
                info.littleEndian = b.info.littleEndian;
                info.bitsPerPixel = b.info.bitsPerPixel;
                info.RGBChannelsCount = b.info.RGBChannelsCount;
                file = b.file;
                stream = b.stream;
                read = new BinaryReader(stream);
                writer = new BinaryWriter(stream);

                if (!info.littleEndian)
                {
                    //We need to convert this buffer to little endian.
                    ToLittleEndian();
                }
                return this;
            }

            public override string ToString()
            {
                return info.stringId;
            }

            public byte[] GetBytes()
            {
                read.BaseStream.Position = 0;
                return read.ReadBytes(info.length);
            }

            public void Seek0()
            {
                read.BaseStream.Position = 0;
            }

            public void SetBytes(byte[] bytes)
            {
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write(bytes, 0, info.length);
            }

            public void SetByte(byte bt, int ind)
            {
                writer.Seek(ind, SeekOrigin.Begin);
                writer.Write(bt);
            }

            public void SetBytes(byte[] bytes, int ind)
            {
                writer.Seek(ind, SeekOrigin.Begin);
                writer.Write(bytes, ind, bytes.Length);
            }

            public void SetShort(ushort val, int ind)
            {
                writer.Seek(ind, SeekOrigin.Begin);
                writer.Write(val);
            }

            public void SetBuffer(Bitmap bitmap)
            {
                BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, info.SizeX, info.SizeY), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                info.pixelFormat = data.PixelFormat;
                IntPtr ptr = data.Scan0;
                int length = data.Stride * bitmap.Height;
                byte[] bytes = new byte[length];
                Marshal.Copy(ptr, bytes, 0, length);
                SetBytes(bytes);
                bitmap.UnlockBits(data);
                bitmap.Dispose();
            }

            public void Flush()
            {
                //Here we write any changes made to the 
                writer.Flush();
            }

            public unsafe Bitmap GetBitmap()
            {
                Bitmap bitmap;
                fixed (byte* ptr = GetBytes())
                {
                    bitmap = new Bitmap(info.SizeX, info.SizeY, info.stride, info.pixelFormat, new IntPtr(ptr));
                }
                return bitmap;
            }

            public unsafe Bitmap GetBuffer(PixelFormat pixel)
            {
                Bitmap bitmap;
                fixed (byte* ptr = GetBytes())
                {
                    bitmap = new Bitmap(info.SizeX, info.SizeY, info.stride, pixel, new IntPtr(ptr));
                }
                return bitmap;
            }

            public unsafe void ToLittleEndian()
            {
                //Here we convert this buffer to little endian.
                byte[] bts = GetBytes();
                Bitmap bitmap;
                Array.Reverse(bts);
                fixed (byte* ptr = bts)
                {
                    bitmap = new Bitmap(info.SizeX, info.SizeY, info.stride, info.pixelFormat, new IntPtr(ptr));
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                SetBuffer(bitmap);
                info.ConvertedToLittleEndian = true;
                info.littleEndian = true;
            }

            public unsafe void ToBigEndian()
            {
                //Here we convert this buffer to big endian.
                byte[] bytes = GetBytes();
                Array.Reverse(bytes);
                Bitmap bitmap;
                fixed (byte* ptr = bytes)
                {
                    bitmap = new Bitmap(info.SizeX, info.SizeY, info.stride, info.pixelFormat, new IntPtr(ptr));
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                SetBuffer(bitmap);
                bitmap.Dispose();
            }

            public byte[] GetEndianBytes(int RGBChannelsCount)
            {
                Bitmap bitmap;
                if (RGBChannelsCount == 1)
                    bitmap = GetBitmap();
                else
                    bitmap = switchRedBlue(GetBitmap());
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, info.SizeX, info.SizeY), ImageLockMode.ReadWrite, info.pixelFormat);
                IntPtr ptr = data.Scan0;
                int length = this.bytes.Length;
                byte[] bytes = new byte[length];
                Marshal.Copy(ptr, bytes, 0, length);
                Array.Reverse(bytes);
                bitmap.UnlockBits(data);
                bitmap.Dispose();
                return bytes;
            }

            public unsafe Bitmap GetFiltered(IntRange range)
            {
                Bitmap bitmap = GetBitmap();
                if (info.bitsPerPixel > 8)
                {
                    // set ranges
                    filter16.InRed = range;
                    filter16.InGreen = range;
                    filter16.InBlue = range;
                    return filter16.Apply(bitmap);
                }
                else
                {
                    // set ranges
                    filter8.InRed = range;
                    filter8.InGreen = range;
                    filter8.InBlue = range;
                    return filter8.Apply(bitmap);
                }
            }
            public unsafe Bitmap GetFiltered(IntRange r, IntRange g, IntRange b)
            {
                Bitmap bitmap = GetBitmap();
                if (info.bitsPerPixel > 8)
                {
                    // set ranges
                    filter16.InRed = r;
                    filter16.InGreen = g;
                    filter16.InBlue = b;
                    return filter16.Apply(bitmap);
                }
                else
                {
                    // set ranges
                    filter8.InRed = r;
                    filter8.InGreen = g;
                    filter8.InBlue = b;
                    return filter8.Apply(bitmap);
                }
            }

            private static Bitmap b;
            private static Bitmap bi;
            public unsafe Bitmap GetRGBChannelFiltered(RGB rgb, IntRange range)
            {
                if (info.RGBChannelsCount == 1)
                {
                    throw new ArgumentException("Plane is not a RGB plane.");
                }

                //We dispose the previous images for the filters so there's no memory leak.
                if (b != null)
                    b.Dispose();
                if (bi != null)
                    bi.Dispose();

                Bitmap bitmap = GetBitmap();
                if (info.bitsPerPixel > 8)
                {
                    // set ranges
                    filter16.InRed = range;
                    filter16.InGreen = range;
                    filter16.InBlue = range;

                    if (rgb == RGB.R)
                    {
                        b = extractR.Apply(bitmap);
                        bi = filter16.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                    if (rgb == RGB.G)
                    {
                        b = extractG.Apply(bitmap);
                        bi = filter16.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                    if (rgb == RGB.B)
                    {
                        b = extractB.Apply(bitmap);
                        bi = filter16.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                }
                else
                {
                    // set ranges
                    filter8.InRed = range;
                    filter8.InGreen = range;
                    filter8.InBlue = range;
                    if (rgb == RGB.R)
                    {
                        b = extractR.Apply(bitmap);
                        bi = filter8.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                    if (rgb == RGB.G)
                    {
                        b = extractG.Apply(bitmap);
                        bi = filter8.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                    if (rgb == RGB.B)
                    {
                        b = extractB.Apply(bitmap);
                        bi = filter8.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                }
                return null;
            }
            public unsafe Bitmap GetRGBChannelRaw(RGB rgb)
            {
                Bitmap bitmap = GetBitmap();
                if (info.RGBChannelsCount == 1)
                    return null;
                if (rgb == RGB.R)
                    return extractR.Apply(bitmap);
                if (rgb == RGB.G)
                    return extractG.Apply(bitmap);
                if (rgb == RGB.B)
                    return extractB.Apply(bitmap);
                return null;
            }

            public Bitmap switchRedBlue(Bitmap image)
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
            public ushort GetValue(int ix, int iy, int index)
            {
                int i = -1;
                int stridex = info.SizeX - 1;
                //For 16bit (2*8bit) images we multiply buffer index by 2
                int x = ix;
                int y = iy;
                if (ix < 0)
                    x = 0;
                if (iy < 0)
                    y = 0;
                if (ix >= info.SizeX)
                    x = info.SizeX - 1;
                if (iy >= info.SizeY)
                    y = info.SizeY - 1;
                if (!info.littleEndian)
                {
                    x = (info.SizeX - 1) - x;
                    y = (info.SizeY - 1) - y;
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * index;
                        i = BitConverter.ToUInt16(bytes, index2);
                        return (ushort)i;
                    }
                    else
                    {
                        int stride = info.SizeX;
                        int indexb = (y * stridex + x) * index;
                        i = bytes[indexb];
                        return (ushort)i;
                    }
                }
                else
                {
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * index;
                        i = BitConverter.ToUInt16(bytes, index2);
                        return (ushort)i;
                    }
                    else
                    {
                        int stride = info.SizeX;
                        int indexb = (y * stridex + x) * index;
                        i = bytes[indexb];
                        return (ushort)i;
                    }
                }
            }
            public ushort GetValue(int ix, int iy)
            {
                int i = 0;
                int stridex = info.SizeX-1;
                //For 16bit (2*8bit) images we multiply buffer index by 2
                int x = ix;
                int y = iy;
                if (ix < 0)
                    x = 0;
                if (iy < 0)
                    y = 0;
                if (ix >= info.SizeX)
                    x = info.SizeX - 1;
                if (iy >= info.SizeY)
                    y = info.SizeY - 1;
                if (!info.littleEndian)
                {
                    x = (info.SizeX - 1) - x;
                    y = (info.SizeY - 1) - y;
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2;
                        i = BitConverter.ToUInt16(bytes, index2);
                        return (ushort)i;
                    }
                    else
                    {
                        int index = (y * stridex + x) * info.RGBChannelsCount;
                        i = bytes[index + (2 * info.RGBChannelsCount)];
                        return (ushort)i;
                    }
                }
                else
                {
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * info.RGBChannelsCount;
                        i = BitConverter.ToUInt16(bytes, index2 + (2 * info.RGBChannelsCount));
                        return (ushort)i;
                    }
                    else
                    {
                        int index = (y * stridex + x) * info.RGBChannelsCount;
                        i = bytes[index + (2 * info.RGBChannelsCount)];
                        return (ushort)i;
                    }
                }
            }
            public void SetValue(int ix, int iy, ushort value)
            {
                byte[] bts = bytes;
                int stridex = info.SizeX - 1;
                //For 16bit (2*8bit) images we multiply buffer index by 2
                int x = ix;
                int y = iy;
                if (!info.littleEndian)
                {
                    x = (info.SizeX - 1) - x;
                    y = (info.SizeY - 1) - y;
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * info.RGBChannelsCount;
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        SetBytes(BitConverter.GetBytes(value), index2);
                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);
                    }
                    else
                    {
                        int index = (y * stridex + x) * info.RGBChannelsCount;
                        SetByte((byte)value, index);
                    }
                }
                else
                {
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = ((y * stridex + x) * 2 * info.RGBChannelsCount);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        SetByte(lower, index2);
                        SetByte(upper, index2 + 1);
                    }
                    else
                    {
                        int index = (y * stridex + x) * info.RGBChannelsCount;
                        SetByte((byte)value, index);
                    }
                }
            }
            public void SetValueRGB(int ix, int iy, int RGBChannel, ushort value)
            {
                //Planes are in BGR order so we invert the RGBChannel parameter.
                if (RGBChannel == 0)
                    RGBChannel = 2;
                else
                if (RGBChannel == 2)
                    RGBChannel = 0;

                int stride = info.SizeX - 1;
                //For 16bit (2*8bit) images we multiply buffer index by 2
                int x = ix;
                int y = iy;
                if (!info.littleEndian)
                {
                    x = (info.SizeX - 1) - x;
                    y = (info.SizeY - 1) - y;
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = ((y * stride + x) * 2 * info.RGBChannelsCount) + (2 * RGBChannel);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);
                    }
                    else
                    {
                        int index = ((y * stride + x) * info.RGBChannelsCount) + (RGBChannel);
                        bytes[index] = (byte)value;
                    }
                }
                else
                {
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = ((y * stride + x) * 2 * info.RGBChannelsCount);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        SetByte(lower, index2);
                        SetByte(upper, index2 + 1);
                    }
                    else
                    {
                        int index = ((y * stride + x) * info.RGBChannelsCount) + (RGBChannel);
                        bytes[index] = (byte)value;
                    }
                }
            }
        }
        public bool SaveSeries(string path, int series, bool folder)
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

            //We set the image physical size to be same as opened image.
            omexml.setPixelsPhysicalSizeX(meta.getPixelsPhysicalSizeX(series), series);
            omexml.setPixelsPhysicalSizeY(meta.getPixelsPhysicalSizeY(series), series);
            omexml.setPixelsPhysicalSizeZ(meta.getPixelsPhysicalSizeZ(series), series);

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
                    meta.setPolygonFontSize(fl, i, series);
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
                    meta.setRectangleFontSize(fl, i, series);
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
                    meta.setLineFontSize(fl, i, series);
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
                    meta.setEllipseFontSize(fl, i, series);
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
                    meta.setLabelFontSize(fl, i, series);
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
            if (File.Exists(path))
                File.Delete(path);
            writer.setId(path);
            writer.setSeries(series);
            writer.setWriteSequentially(true);
           
            /*
            for (int im = 0; im < imageCount; im++)
            {
                writer.saveBytes(im, Buffers[im].GetEndianBytes(RGBChannelCount));
                Application.DoEvents();
            }
            writer.close();
            */
             wr = writer;
            imCount = imageCount;
            bufs = Buffers;
            rgbChans = rGBChannelCount;
            tfile = Path.GetFileName(path);
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(WriteBytes));
            t.Start();
            Progress pr = new Progress(tfile);
            pr.Show();
            do
            {
                pr.UpdateProgress((int)progress);
                Application.DoEvents();
            } while (!done);
            pr.Close();
            pr.Dispose();
            return true;
        }
        private static ImageWriter wr;
        private static int imCount;
        private static List<Buf> bufs = new List<Buf>();
        private static int rgbChans;
        private static string tfile = "";
        private static bool done = false;
        public static float progress = 0;
        public static void WriteBytes()
        {
            progress = 0;
            done = false;
            for (int im = 0; im < imCount; im++)
            {
                wr.saveBytes(im, bufs[im].GetEndianBytes(rgbChans));
                progress = ((float)im / (float)imCount)*100;
            }
            done = true;
            wr.close();
        }
        public void OpenSeries(string file, int ser)
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
            rGBChannelCount = reader.getRGBChannelCount();
            bitsPerPixel = reader.getBitsPerPixel();
            filename = file;
            SizeX = reader.getSizeX();
            SizeY = reader.getSizeY();
            sizeC = reader.getSizeC();
            sizeZ = reader.getSizeZ();
            sizeT = reader.getSizeT();
            imageCount = reader.getImageCount();
            littleEndian = reader.isLittleEndian();
            seriesCount = reader.getSeriesCount();
            imagesPerSeries = imageCount / seriesCount;
            Coords = new int[seriesCount, SizeZ, SizeC, SizeT];
            idString = file;

            if (RGBChannelCount == 1)
            {
                if (bitsPerPixel > 8)
                {
                    px = PixelFormat.Format16bppGrayScale;
                }
                else
                {
                    px = PixelFormat.Format8bppIndexed;
                }
            }
            else
            {
                if (bitsPerPixel > 8)
                    px = PixelFormat.Format48bppRgb;
                else
                    px = PixelFormat.Format24bppRgb;
            }

            int ccount = SizeC / rGBChannelCount;
            //Lets get the channels amd initialize them.
            for (int i = 0; i < ccount; i++)
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
                        ome.xml.model.primitives.Color c = meta.getChannelColor(0, i);
                        ch.color = System.Drawing.Color.FromArgb(c.getRed(), c.getGreen(), c.getBlue());
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
                SZCT co = new SZCT(0, 0, 0, 0);
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
                    BioImage.Annotation an = new Annotation();
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

            int stride;
            if (bitsPerPixel > 8)
                stride = SizeX * 2 * rGBChannelCount;
            else
                stride = SizeX * rGBChannelCount;

            bufferTable = new Hashtable();

            fileHashTable = new Hashtable();
            fileHashTable.Add(file, file.GetHashCode());

            List<string> serFiles = new List<string>();
            serFiles.AddRange(reader.getSeriesUsedFiles());
            //List<BufferInfo> BufferInfos = new List<BufferInfo>(); 
            //List<string> Files = new List<string>();
            for (int i = 0; i < imagesPerSeries; i++)
            {
                byte[] bytes = reader.openBytes(i);
                int[] ints = reader.getZCTCoords(i);
                int z = ints[0];
                int channel = ints[1];
                int time = ints[2];
                int lenbuf = bytes.Length;
                Coords[ser, z, channel, time] = i;
                //Here we generate an id for the buffer which we wish to create
                BufferInfo fi = new BufferInfo(file, lenbuf, ser, SizeX, SizeY, stride, RGBChannelCount, bitsPerPixel, px, time, channel, z, i, littleEndian, convertedToLittleEndian);
                Buf buf = new Buf(fi, bytes);
                Buffers.Add(buf);
                bufferTable.Add(fi.HashID, buf);
                Table.AddBufferByHash(fi.HashID, buf);
            }
            double stx = 0;
            double sty = 0;
            double stz = 0;
            double six = 0;
            double siy = 0;
            double siz = 0;

            try
            {

                if (meta.getPixelsPhysicalSizeX(ser) != null)
                    six = meta.getPixelsPhysicalSizeX(ser).value().doubleValue();
                if (meta.getPixelsPhysicalSizeY(ser) != null)
                    siy = meta.getPixelsPhysicalSizeY(ser).value().doubleValue();
                if (meta.getPixelsPhysicalSizeZ(ser) != null)
                    siz = meta.getPixelsPhysicalSizeZ(ser).value().doubleValue();

                //Calling these when they are not defined causes an error so we use the try catch block.
                if (meta.getStageLabelX(ser) != null)
                    stx = meta.getStageLabelX(ser).value().doubleValue();
                if (meta.getStageLabelY(ser) != null)
                    sty = meta.getStageLabelY(ser).value().doubleValue();
                if (meta.getStageLabelZ(ser) != null)
                    stz = meta.getStageLabelZ(ser).value().doubleValue();
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
                //Volume is used only for stage coordinates if error iss thrown it is because this image doens't have any size information or it is incomplete as read by Bioformats.
            }
            Table.AddBioImageByID(idString, this);

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

        public static List<Annotation> OpenROIs(string file)
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
                SZCT co = new SZCT(0, 0, 0, 0);
                int scount = meta.getShapeCount(i);
                for (int sc = 0; sc < scount; sc++)
                {
                    string type = meta.getShapeType(i, sc);
                    BioImage.Annotation an = new Annotation();
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
                            if (sc > 0)
                            {
                                an.coord = co;
                            }
                            else
                            {
                                ome.xml.model.primitives.NonNegativeInteger nz = meta.getPointTheZ(i, sc);
                                if (nz != null)
                                    co.Z = nz.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getPointTheZ(i, sc);
                                if (nc != null)
                                    co.Z = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getPointTheZ(i, sc);
                                if (nt != null)
                                    co.Z = nt.getNumberValue().intValue();
                                an.coord = co;
                            }
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
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getLineTheZ(i, sc);
                                if (nc != null)
                                    co.Z = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getLineTheZ(i, sc);
                                if (nt != null)
                                    co.Z = nt.getNumberValue().intValue();
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
                        ome.xml.model.primitives.Color colf = meta.getLineStrokeColor(i, sc);
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
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getRectangleTheZ(i, sc);
                                if (nc != null)
                                    co.Z = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getRectangleTheZ(i, sc);
                                if (nt != null)
                                    co.Z = nt.getNumberValue().intValue();
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
                        ome.xml.model.primitives.Color colf = meta.getRectangleStrokeColor(i, sc);
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
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getEllipseTheZ(i, sc);
                                if (nc != null)
                                    co.Z = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getEllipseTheZ(i, sc);
                                if (nt != null)
                                    co.Z = nt.getNumberValue().intValue();
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
                        ome.xml.model.primitives.Color colf = meta.getEllipseStrokeColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Polygon")
                    {
                        an.type = Annotation.Type.Polygon;
                        an.id = meta.getPolygonID(i, sc);
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
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getPolygonTheZ(i, sc);
                                if (nc != null)
                                    co.Z = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getPolygonTheZ(i, sc);
                                if (nt != null)
                                    co.Z = nt.getNumberValue().intValue();
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
                        ome.xml.model.primitives.Color colf = meta.getPolygonStrokeColor(i, sc);
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
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getPolylineTheZ(i, sc);
                                if (nc != null)
                                    co.Z = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getPolylineTheZ(i, sc);
                                if (nt != null)
                                    co.Z = nt.getNumberValue().intValue();
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
                        ome.xml.model.primitives.Color colf = meta.getPolylineStrokeColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    else
                    if (type == "Label")
                    {
                        an.type = Annotation.Type.Label;
                        an.Text = meta.getLabelText(i, sc);
                        an.id = meta.getLabelID(i, sc);
                        an.AddPoint(new PointD(meta.getLabelX(i, sc).doubleValue(), meta.getLabelY(i, sc).doubleValue()));
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
                                ome.xml.model.primitives.NonNegativeInteger nc = meta.getLabelTheZ(i, sc);
                                if (nc != null)
                                    co.Z = nc.getNumberValue().intValue();
                                ome.xml.model.primitives.NonNegativeInteger nt = meta.getLabelTheZ(i, sc);
                                if (nt != null)
                                    co.Z = nt.getNumberValue().intValue();
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
                        ome.xml.model.primitives.Color colf = meta.getLabelStrokeColor(i, sc);
                        if (colf != null)
                            an.fillColor = System.Drawing.Color.FromArgb(colf.getAlpha(), colf.getRed(), colf.getGreen(), colf.getBlue());
                    }
                    Annotations.Add(an);
                }
            }
            imageReader.close();
            return Annotations;
        }

        public static void ExportROIs(string filename, List<Annotation> Annotations)
        {
            string con = "";
            string cols = "ROIID,ROINAME,TYPE,ID,SHAPEINDEX,Text,S,C,Z,T,X,Y,W,H,POINTS,STROKECOLOR,STROKECOLORW,FILLCOLOR,FONTSIZE" + Environment.NewLine;
            con += cols;
            for (int i = 0; i < Annotations.Count; i++)
            {
                BioImage.Annotation an = Annotations[i];
                BioImage.PointD[] points = an.GetPoints();
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
                    an.Text + ',' + an.coord.S.ToString() + ',' + an.coord.Z.ToString() + ',' + an.coord.C.ToString() + ',' + an.coord.T.ToString() + ',' + an.X.ToString() + ',' + an.Y.ToString() + ',' +
                    an.W.ToString() + ',' + an.H.ToString() + ',' + sep.ToString() + pts + sep.ToString() + ',' + sColor + ',' + an.strokeWidth.ToString() + ',' + bColor + ',' + an.font.Size.ToString() + ',' + Environment.NewLine;
                con += line;
            }
            File.WriteAllText(filename, con);
        }

        public static void ExportROIFolder(string path, string filename)
        {
            string[] fs = Directory.GetFiles(path);
            int i = 0;
            foreach (string f in fs)
            {
                List<Annotation> annotations = OpenROIs(f);
                string ff = Path.GetFileNameWithoutExtension(f);
                ExportROIs(path + "//" + ff + "-" + i.ToString() + ".csv", annotations);
                i++;
            }
        }

        private byte[] autoBytes;
        public unsafe bool AutoThresholdChannel(Channel c1)
        {
            if (bitsPerPixel > 8)
                for (int time = 0; time < SizeT; time++)
                {
                    for (int z = 0; z < SizeZ; z++)
                    {
                        int i = reader.getIndex(z, c1.Index, time);
                        autoBytes = Buffers[i].bytes;
                        int index, index2, x, y;
                        if (!littleEndian)
                        {
                            //stride 1 is for the destination image and stride2 for source image.
                            //destination image will always be 3channels(RGB) with 24bits
                            int stride1 = SizeX * 3;
                            int stride2 = SizeX * RGBChannelCount;
                            for (y = SizeY - 1; y > -1; y--)
                            {
                                for (x = SizeX - 1; x > -1; x--)
                                {
                                    //index is for destination image and index2 for source image
                                    index = ((SizeY - y) * stride1) + ((SizeX - x) * 3);
                                    //For 16bit (2*8bit) images we multiply buffer index by 2
                                    index2 = (y * stride2 + (x * RGBChannelCount)) * 2;
                                    int px = BitConverter.ToUInt16(autoBytes, index2);
                                    if (px > c1.Max)
                                        c1.Max = px;
                                }
                            }
                        }
                        else
                        {
                            //stride 1 is for the destination image and stride2 for source image.
                            //destination image will always be 3channels(RGB) with 24bits
                            int stride1 = SizeX * 3;
                            int stride2 = SizeX * RGBChannelCount;
                            for (y = 0; y < SizeY; y++)
                            {
                                for (x = 0; x < SizeX; x++)
                                {
                                    //index is for destination image and index2 for source image
                                    index = ((SizeY - y) * stride1) + ((SizeX - x) * 3);
                                    //For 16bit (2*8bit) images we multiply buffer index by 2
                                    index2 = (y * stride2 + (x * RGBChannelCount)) * 2;
                                    int px = BitConverter.ToUInt16(autoBytes, index2);
                                    if (px > c1.Max)
                                        c1.Max = px;
                                }
                            }
                        }
                    }
                }
            return true;
        }
        public void AutoThreshold()
        {
            foreach (Channel c in Channels)
            {
                c.Max = 0;
                AutoThresholdChannel(c);
            }
            //RefreshPlane();
        }

    }

    public class BioSeries
    {
        public List<BioImage.SZCT> Coords = new List<BioImage.SZCT>();
        public Hashtable table = new Hashtable();
        public Bitmap getImage(BioImage.SZCT cor)
        {
            return (Bitmap)getImage(cor);
        }
    }

    public class View
    {
        public BioImage.Buf plane;
        public BioImage.SZCT coord;
        public View(BioImage.Buf buf, BioImage.SZCT coord)
        {
            plane = buf;
        }

        public Bitmap GetPlane(BioImage.Buf plane, BioImage.SZCT coord)
        {
            return plane.GetBitmap();
        }

    }
    public class Functions
    {
        public List<string> functions = new List<string>();
        public int progress;
        public void Internal(object image, string func)
        {
            BioImage im = (BioImage)image;
            throw new NotImplementedException();
        }

        public void External(object image, string filefunc)
        {
            BioImage im = (BioImage)image;
            ProcessStartInfo ps = new ProcessStartInfo(filefunc);
            Process.Start(ps);
        }

    }

}
