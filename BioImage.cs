using AForge;
using AForge.Imaging.Filters;
using loci.common.services;
using loci.formats;
using loci.formats.meta;
using loci.formats.services;
using ome.xml.meta;
using ome.xml;
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
        int [,,,] Coords;
        public Hashtable bufferTable;
        public Hashtable fileHashTable;
        public Random _random = new Random();
        public ImageReader imageReader;
        public loci.formats.ImageWriter imageWriter;
        public loci.formats.meta.IMetadata meta;
        public List<Buf> Buffers = new List<Buf>();
        public List<Channel> Channels = new List<Channel>();
        public List<VolumeD> Volumes = new List<VolumeD>();
        public string idString;
        public int[] rgbChannels = new int[3];
        public int rGBChannelCount = 1;
        public int bitsPerPixel;
        private PixelFormat px;
        public bool convertedToLittleEndian = false;

        public BioImage(int ser, string file)
        {
            serie = ser;
            Load(file);
            rgbBitmap16 = new Bitmap(SizeX, SizeY, PixelFormat.Format48bppRgb);
            rgbBitmap8 = new Bitmap(SizeX, SizeY, PixelFormat.Format24bppRgb);
        }

        public BioImage.Channel RChannel
        {
            get
            {
                return Channels[rgbChannels[0]];
            }
        }
        public BioImage.Channel GChannel
        {
            get
            {
                return Channels[rgbChannels[1]];
            }
        }
        public BioImage.Channel BChannel
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
        }
        public struct SZCTXY
        {
            public int S, Z, C, T, X, Y;
            public SZCTXY(int s,int z, int c, int t, int x, int y)
            {
                S = s;
                Z = z;
                C = c;
                T = t;
                X = x;
                Y = y;
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
        public int SizeX, SizeY, SizeZ, SizeC, SizeT;
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
            return Buffers[Coords[s,z,c,t]];
        }
        public Buf GetBufByCoord(SZCT coord)
        {
            return Buffers[Coords[coord.S, coord.Z, coord.C, coord.T]];
        }

        public Bitmap GetBitmap(SZCT coord)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetBitmap();
        }

        public Bitmap GetFiltered(SZCT coord,  IntRange filt)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetFiltered(filt);
        }
        public Bitmap GetFiltered(SZCT coord, IntRange rr, IntRange rg)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetFiltered(rr, rg);
        }
        public Bitmap GetFiltered(SZCT coord, IntRange rr, IntRange rg, IntRange rb)
        {
            return GetBufByCoord(coord.S, coord.Z, coord.C, coord.T).GetFiltered(rr,rg,rb);
        }

        private Bitmap rgbBitmap16 = null;
        public Bitmap GetRGBBitmap16(SZCT coord, IntRange rf, IntRange gf, IntRange bf)
        {
            watch.Restart();
            int ri = imageReader.getIndex(coord.Z, coord.C, coord.T);
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
        public Bitmap GetRGBBitmap16(SZCT coord, IntRange rf, IntRange gf)
        {
            watch.Restart();
            int ri = imageReader.getIndex(coord.Z, coord.C, coord.T);
            if (RGBChannelCount == 1)
            {
                replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, Buffers[ri + RChannel.index].GetFiltered(rf, gf));
                replaceRFilter.ApplyInPlace(rgbBitmap16);
                replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, Buffers[ri + GChannel.index].GetFiltered(rf, gf));
                replaceGFilter.ApplyInPlace(rgbBitmap16);
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
            int ri = imageReader.getIndex(coord.Z, coord.C, coord.T);
            if (RGBChannelCount == 1)
            {
                replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R,Buffers[ri].GetFiltered(rf));
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
            int ri = imageReader.getIndex(coord.Z, coord.C, coord.T);
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
            public static ColorS FromColor(Color col)
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
            public static Color ToColor(ColorS col)
            {
                float r = (((float)col.R) / ushort.MaxValue) * 255;
                float g = (((float)col.G) / ushort.MaxValue) * 255;
                float b = (((float)col.B) / ushort.MaxValue) * 255;
                Color c = Color.FromArgb((byte)r, (byte)g, (byte)b);
                return c;
            }
            public override string ToString()
            {
                return R + "," + G + "," + B;
            }
        }
        public class Channel
        {
            public string Name = "";
            public string ID = "";
            public int index;

            public string Fluor = "";
            public int SamplesPerPixel;
            public Color color;
            public int Emission;
            public int Excitation;
            public int Exposure;
            public string LightSource;
            public double LightSourceIntensity;
            public int LightSourceWavelength;
            public string ContrastMethod;
            public string IlluminationType;
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
                
                if(info.RGBChannelsCount == 3)
                {
                    if (info.bitsPerPixel > 8)
                    {
                        //RGB Channels are stored in BGR so we switch them to RGB
                        switchRedBlue(GetBitmap());
                        //Then turn the 48bpp buffers to 3 RGB 16bpp buffers
                    }
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
                    if (inf.bitsPerPixel > 8)
                    {
                        //RGB Channels are stored in BGR so we switch them to RGB
                        switchRedBlue(GetBitmap());
                        //Then turn the 48bpp buffers to 3 RGB 16bpp buffers
                    }
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

            public void SetByte(byte[] bytes, int ind)
            {
                writer.Seek(ind, SeekOrigin.Begin);
                writer.Write(bytes, ind, info.length);
            }

            public void SetBuffer(Bitmap bitmap)
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, info.SizeX, info.SizeY), ImageLockMode.ReadWrite, bitmap.PixelFormat);
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

            public byte[] GetEndianBytes()
            {
                Bitmap bitmap = GetBitmap();
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, info.SizeX, info.SizeY), ImageLockMode.ReadWrite, info.pixelFormat);
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
            public unsafe Bitmap GetFiltered(IntRange r, IntRange g)
            {
                Bitmap bitmap = GetBitmap();
                if (info.bitsPerPixel > 8)
                {
                    // set ranges
                    filter16.InRed = r;
                    filter16.InGreen = g;
                    return filter16.Apply(bitmap);
                }
                else
                {
                    // set ranges
                    filter8.InRed = r;
                    filter8.InGreen = g;
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
                int stridex = info.SizeX;
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
                int i = -1;
                int stridex = info.SizeX;
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
                int stridex = info.SizeX;
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
                        int index2 = (y * stridex + x) * 2 * info.RGBChannelsCount;
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);

                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);

                        //bytes[index2] = upper;
                        //bytes[index2 + 1] = lower;
                    }
                    else
                    {
                        int index = (y * stridex + x) * info.RGBChannelsCount;
                        SetByte((byte)value, index);
                        bytes[index] = (byte)value;
                    }
                }
                else
                {
                    if (info.bitsPerPixel > 8)
                    {
                        int index2 = ((y * stridex + x) * 2 * info.RGBChannelsCount);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);

                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);

                        //bytes[index2] = lower;
                        //bytes[index2 + 1] = upper;
                    }
                    else
                    {
                        int index = (y * stridex + x) * info.RGBChannelsCount;
                        SetByte((byte)value, index);
                        //bytes[index] = (byte)value;
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

                int stride = info.SizeX;
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
                        int index2 = ((y * stride + x) * 2 * info.RGBChannelsCount) + (2 * RGBChannel);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);
                        bytes[index2] = upper;
                        bytes[index2 + 1] = lower;
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
                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);
                        bytes[index2] = lower;
                        bytes[index2 + 1] = upper;
                    }
                    else
                    {
                        int index = ((y * stride + x) * info.RGBChannelsCount) + (RGBChannel);
                        bytes[index] = (byte)value;
                    }
                }
            }
        }

        public bool Save(string path)
        {
            //Method used to save a range of an image stack defined by start & count.
            loci.formats.ImageWriter writer = new loci.formats.ImageWriter();
            writer.setMetadataRetrieve(meta);
            writer.setId(path);
            int sc = imageReader.getSeriesCount();
            for (int series = 0; series < sc; series++)
            {
                //reader.setSeries(series);
                writer.setSeries(series);

                //writer.setWriteSequentially(true);
                for (int i = 0; i < imageCount; i++)
                {
                    Buf b = Buffers[i];
                    writer.saveBytes(i, b.GetEndianBytes());
                    Application.DoEvents();
                }
            }
            return true;
        }

        public bool SaveSeries(string path, int series)
        {
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            loci.formats.meta.IMetadata omexml = service.createOMEXMLMetadata();

            omexml.setImageID("Image:0", series);
            omexml.setPixelsID("Pixels:0", series);
            // specify that the pixel data is stored in big-endian order
            // replace 'TRUE' with 'FALSE' to specify little-endian order
            if (littleEndian)
                omexml.setPixelsBinDataBigEndian(java.lang.Boolean.TRUE, 0, 0);
            else
                omexml.setPixelsBinDataBigEndian(java.lang.Boolean.FALSE, 0, 0);

            omexml.setPixelsDimensionOrder(ome.xml.model.enums.DimensionOrder.XYCZT, series);
            if(bitsPerPixel > 8)
                omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT16, series);
            else
                omexml.setPixelsType(ome.xml.model.enums.PixelType.UINT8, series);
            omexml.setPixelsSizeX(new ome.xml.model.primitives.PositiveInteger(java.lang.Integer.valueOf(SizeX)), series);
            omexml.setPixelsSizeY(new ome.xml.model.primitives.PositiveInteger(java.lang.Integer.valueOf(SizeY)), series);
            omexml.setPixelsSizeZ(new ome.xml.model.primitives.PositiveInteger(java.lang.Integer.valueOf(SizeZ)), series);
            int samples = 1;
            if (isRGB)
                samples = 3;
            omexml.setPixelsSizeC(new ome.xml.model.primitives.PositiveInteger(java.lang.Integer.valueOf(SizeC * samples)), series);
            omexml.setPixelsSizeT(new ome.xml.model.primitives.PositiveInteger(java.lang.Integer.valueOf(SizeT)), series);

            //ome.units.unit.Unit unit = ome.units.UNITS.MICROMETER;
            //java.lang.Number intg = new java.lang.Integer(meta.getPixelsPhysicalSizeX(0));
            omexml.setPixelsPhysicalSizeX(meta.getPixelsPhysicalSizeX(series), series);
            omexml.setPixelsPhysicalSizeY(meta.getPixelsPhysicalSizeY(series), series);
            omexml.setPixelsPhysicalSizeZ(meta.getPixelsPhysicalSizeZ(series), series);

            for (int channel = 0; channel < Channels.Count; channel++)
            {
                omexml.setChannelID("Channel:" + series + ":" + channel, series, channel);
                omexml.setChannelSamplesPerPixel(new ome.xml.model.primitives.PositiveInteger(java.lang.Integer.valueOf(samples)), series, channel);
            }
            //Method used to save a range of an image stack defined by start & count.
            loci.formats.ImageWriter writer = new loci.formats.ImageWriter();
            writer.setMetadataRetrieve(omexml);
            string str = Path.GetDirectoryName(path) + "\\TestWrite.tif";
            //We delete the file so we don't just add more images to an existing file;
            if (File.Exists(str))
                File.Delete(str);
            writer.setId(str);
            //reader.setSeries(series);
            writer.setSeries(series);
            writer.setWriteSequentially(true);
            for (int i = 0; i < imageCount; i++)
            {
                writer.saveBytes(i, Buffers[i].GetEndianBytes());
                Application.DoEvents();
            }
            writer.close();
            return true;
        }
        public void Load(string file)
        {
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            meta = service.createOMEXMLMetadata();
            // create format reader
            imageReader = new ImageReader();
            imageReader.setMetadataStore(meta);
            // initialize file
            imageReader.setId(file);
            rGBChannelCount = imageReader.getRGBChannelCount();
            bitsPerPixel = imageReader.getBitsPerPixel();

            SizeX = imageReader.getSizeX();
            SizeY = imageReader.getSizeY();
            SizeC = imageReader.getSizeC();
            SizeZ = imageReader.getSizeZ();
            SizeT = imageReader.getSizeT();
            imageCount = imageReader.getImageCount();
            littleEndian = imageReader.isLittleEndian();
            seriesCount = imageReader.getSeriesCount();
            imagesPerSeries = imageCount / seriesCount;
            Coords = new int[seriesCount ,SizeZ, SizeC, SizeT];
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
                if(bitsPerPixel > 8)
                    px = PixelFormat.Format48bppRgb;
                else
                    px = PixelFormat.Format24bppRgb;
            }
            
            int ccount = SizeC / rGBChannelCount;
            //Lets get the channels amd initialize them.
            for (int i = 0; i < ccount; i++)
            {
                Channel ch = new Channel(i, imageReader.getBitsPerPixel());
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
                        ch.color = Color.FromArgb(c.getRed(), c.getGreen(), c.getBlue());
                        if (ch.color.R == 255 && ch.color.G == 0 && ch.color.B == 0)
                            ch.rgb = RGB.R;
                        if (ch.color.R == 0 && ch.color.G == 255 && ch.color.B == 0)
                            ch.rgb = RGB.G;
                        if (ch.color.R == 0 && ch.color.G == 0 && ch.color.B == 255)
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

            int stride;
            if (bitsPerPixel > 8)
                stride = SizeX * 2 * rGBChannelCount;
            else
                stride = SizeX * rGBChannelCount;

            bufferTable = new Hashtable();
            //id = CreateHash(file, serie, 0).ToString();

            //bufferTable.Add(id, file);
            //bufferTable.Add(id.GetHashCode(), file);

            fileHashTable = new Hashtable();
            fileHashTable.Add(file, file.GetHashCode());
            bool gr = imageReader.isGroupFiles();
            for (int ser= 0; ser < seriesCount; ser++)
            {
                imageReader.setSeries(ser);

                List<string> serFiles = new List<string>();
                serFiles.AddRange(imageReader.getSeriesUsedFiles());
                //List<BufferInfo> BufferInfos = new List<BufferInfo>(); 
                //List<string> Files = new List<string>();
                for (int i = 0; i < imagesPerSeries; i++)
                {
                    byte[] bytes = imageReader.openBytes(i);
                    int[] ints = imageReader.getZCTCoords(i);
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

                Volumes.Add(new VolumeD(new Point3D(stx, sty, stz), new Point3D(six * SizeX, siy * SizeY, siz * SizeZ)));
            }

            Table.AddBioImageByID(idString, this);

            /*
            if (imageReader.getRGBChannelCount() == 3)
            {
                //We copy the first channel so that each RGB channel has a Channel attribute,
                //despite the file not specifying them.
                Channels.Add(Channels[0].Copy());
                Channels[1].rgb = RGB.G;
                Channels[1].index = 1;
                Channels.Add(Channels[0].Copy());
                Channels[2].index = 2;
                Channels[2].rgb = RGB.B;
            }
            */

        }

        public List<string> OpenFolder(string path, int ser, out string nameId, char c)
        {
            string ext = Path.GetExtension(path);
            List<string> ims = new List<string>();
            List<string> ids = new List<string>();
            string p = Path.GetDirectoryName(path);
            

            foreach (string file in Directory.GetFiles(p))
            {
                if(file.EndsWith(ext))
                {
                    ims.Add(file);
                }
            }

            string folderName = System.IO.Path.GetDirectoryName(ims[0]);
            nameId = Path.GetFileName(folderName);

            foreach (string file in ims)
            {
                imageCount = ims.Count;
                nameId = Path.GetFileName(file).Split(c)[0];
                string idn = file + "/s" + ser + "i" + ims;
                imagesPerSeries = ims.Count;
                Buffers = new List<Buf>();

                imageReader.setId(file);

                SizeZ = 0;
                SizeC = 0;
                SizeT = 0;

                SizeX = imageReader.getSizeX();
                SizeY = imageReader.getSizeY();

                for (int i = 0; i < ims.Count; i++)
                {
                    string[] bre = Path.GetFileNameWithoutExtension(ims[i]).Split(c);
                    nameId = Path.GetFileName(bre[0]);

                    string ch1 = bre[1][0].ToString();
                    string ch2 = bre[2][0].ToString();
                    string ch3 = bre[3][0].ToString();

                    string cs = bre[1].Remove(0, 1);
                    string cs2 = bre[2].Remove(0, 1);
                    string cs3 = bre[3].Remove(0, 1);

                    int iz = int.Parse(cs);
                    int ic = int.Parse(cs2);
                    int it = int.Parse(cs3);

                    if (iz > SizeZ)
                    {
                        SizeZ = iz;
                    }
                    if (ic > SizeC)
                    {
                        SizeC = ic;
                    }
                    if (it > SizeT)
                    {
                        SizeT = it;
                    }

                    //int stride = 0;
                    int bytesPerPixel = (bitsPerPixel + 7) / 8;
                    int stride = 4 * ((SizeX * bytesPerPixel + 3) / 4);

                    int len = stride * SizeY;
                    string fn = Path.GetFileName(ims[i]);
                    string idstring = CreateID(ims[i], ser, Coords[ser, iz, ic, it]);
                    ids.Add(idstring);
                    int planeHash = idstring.GetHashCode();
                    idString = idstring;
                    BufferInfo fi = new BufferInfo(idstring, len, ser, SizeX, SizeY, stride, RGBChannelCount, bitsPerPixel, px, it, ic, iz, i, littleEndian, convertedToLittleEndian);
                    //imageReader.setId(ims[i]);

                    Buf bff = new Buf(fi, ser, i);
                    Buffers.Add(bff);
                    bufferTable.Add(planeHash, Buffers[i]);
                    Table.AddBufferByID(idstring, bff);
                }
            }
            return ids;
        }

        private byte[] autoBytes;
        public unsafe bool AutoThresholdChannel(Channel c1)
        {
            if(bitsPerPixel > 8)
            for (int time = 0; time < SizeT; time++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    int i = imageReader.getIndex(z, c1.Index, time);
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
