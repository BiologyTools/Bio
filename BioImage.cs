using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Globalization;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using System.Threading;
using System.Management;
using System.Runtime.InteropServices;
using AForge;
using AForge.Imaging;
using AForge.Imaging.Filters;
using loci.formats;
using loci.formats.meta;
using loci.common.services;
using loci.formats.services;
using loci.formats.tiff;
using loci.formats.@out;

namespace BioImage
{

    public class BioImage
    {
        public Hashtable table;
        public MemoryMappedFile file;
        public Random _random = new Random();
        public ImageReader imageReader;
        public loci.formats.ImageWriter imageWriter;
        public IMetadata meta;
        public List<Buf> Buffers = new List<Buf>();
        public List<Channel> Channels = new List<Channel>();
        public List<VolumeD> Volumes = new List<VolumeD>();
        public int[] rgbChannels = new int[3];
        private int sizeTime = 0;
        private int rGBCount = 1;
        private int sizeZ = 1;
        private int bitsPerPixel = 16;
        private PixelFormat px = PixelFormat.Format16bppGrayScale;
        public bool convertedToLittleEndian = false;
        public BioImage(bool autoThreshold)
        {
            if (autoThreshold)
            {
                AutoThreshold();
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
        }
        public int imageCount = 0;
        public int imagesPerSeries = 0;
        public int seriesCount = 0;
        public bool littleEndian = false;
        public long loadTimeMS = 0;
        public long loadTimeTicks = 0;
        public Stopwatch watch = new Stopwatch();


        public int RGBCount
        {
            get
            {
                return rGBCount;
            }
        }
        public int SizeX
        {
            get
            {
                return Buffers[0].SizeX;
            }
        }
        public int SizeY
        {
            get
            {
                return Buffers[0].SizeY;
            }
        }
        public int SizeC
        {
            get
            {
                return Channels.Count;
            }
        }
        public int SizeZ
        {
            get
            {
                return sizeZ;
            }
        }
        public int SizeT
        {
            get
            {
                return sizeTime;
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

        public Buf GetBuffer(ZCT coord)
        {
            int i = imageReader.getIndex(coord.Z, coord.C, coord.T);
            return Buffers[i];
            /*
            if (RGBCount == 1)
            {
                if(bitsPerPixel == 16)
                    return Buffers[i].GetBuffer(PixelFormat.Format16bppGrayScale);
                else
                    return Buffers[i].GetBuffer(PixelFormat.Format8bppIndexed);
            }
            else
                return Buffers[i].GetBuffer(PixelFormat.Format48bppRgb);
            */
        }

        public Bitmap GetBitmap(ZCT coord)
        {
            int i = imageReader.getIndex(coord.Z, coord.C, coord.T);
            return Buffers[i].GetBuffer();
            /*
            if (RGBCount == 1)
            {
                if(bitsPerPixel == 16)
                    return Buffers[i].GetBuffer(PixelFormat.Format16bppGrayScale);
                else
                    return Buffers[i].GetBuffer(PixelFormat.Format8bppIndexed);
            }
            else
                return Buffers[i].GetBuffer(PixelFormat.Format48bppRgb);
            */
        }

        public Bitmap GetFiltered(ZCT coord, IntRange filt)
        {
            int i = imageReader.getIndex(coord.Z, coord.C, coord.T);
            return Buffers[i].GetFiltered(filt);
            /*
            if (RGBCount == 1)
            {
                if (bitsPerPixel == 16)
                    return Buffers[i].GetBuffer(PixelFormat.Format16bppGrayScale);
                else
                    return Buffers[i].GetBuffer(PixelFormat.Format8bppIndexed);
            }
            else
                return Buffers[i].GetBuffer(PixelFormat.Format48bppRgb);
            */
        }

        private Bitmap rgbBitmap = null;
        public Bitmap GetRGBBitmap(ZCT coord, IntRange rf, IntRange gf, IntRange bf)
        {
            watch.Restart();
            if (bitsPerPixel > 8)
                rgbBitmap = new Bitmap(SizeX, SizeY, PixelFormat.Format48bppRgb);
            else
                rgbBitmap = new Bitmap(SizeX, SizeY, PixelFormat.Format24bppRgb);

            int ri = imageReader.getIndex(coord.Z, coord.C, coord.T);
            Bitmap rb = Buffers[ri].GetFiltered(rf);
            Bitmap gb = Buffers[ri+1].GetFiltered(gf);
            Bitmap bb = Buffers[ri+2].GetFiltered(bf);

            replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, rb);
            replaceRFilter.ApplyInPlace(rgbBitmap);
            replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, gb);
            replaceGFilter.ApplyInPlace(rgbBitmap);
            replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, bb);
            replaceBFilter.ApplyInPlace(rgbBitmap);
            watch.Stop();
            loadTimeMS = watch.ElapsedMilliseconds;
            loadTimeTicks = watch.ElapsedTicks;
            return rgbBitmap;
        }
        public static Stopwatch swatch = new Stopwatch();

        public Buf GetPlane(ZCT coord)
        {
            int i = imageReader.getIndex(coord.Z, coord.C, coord.T);
            return Buffers[i];
        }
        public enum RGB
        {
            R,
            G,
            B
        }
        public class Channel
        {
            public string Name = "";
            public string ID = "";
            public string Fluor = "";
            private int index;
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
            public Channel(int index)
            {
                range = new IntRange(0, ushort.MaxValue);
                Max = 65535;
                Min = 0;
                this.index = index;
            }
            public Channel(int index, int bitsPerPixel)
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
                this.index = index;
            }

            public Channel Copy()
            {
                Channel c = new Channel(index);
                c.Name = Name;
                c.ID = ID;
                c.Fluor = Fluor;
                c.SamplesPerPixel = SamplesPerPixel;
                c.color = color;
                c.Emission = Emission;
                c.Excitation = Excitation;
                c.Exposure = Exposure;
                c.LightSource = LightSource;
                c.LightSourceIntensity = LightSourceIntensity;
                c.LightSourceWavelength = LightSourceWavelength;
                c.ContrastMethod = ContrastMethod;
                c.IlluminationType = IlluminationType;
                c.range = range;
                return c;
            }

            public override string ToString()
            {
                if(Name == "")
                    return index + ", No Name";
                else
                    return index + ", " + Name;
            }
        }
        public struct Buf
        {
            public string id;
            public bool littleEndian;
            public int length;
            public int SizeX, SizeY;
            public int time;
            public int channel;
            public int stride;
            public int series;
            public int index;
            public PixelFormat pixelFormat;
            public MemoryMappedFile file;
            public MemoryMappedViewStream stream;
            public BinaryWriter writer;
            public BinaryReader read;
            
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
            
            public int RGBChannelsCount;
            public int bitsPerPixel;
            public bool ConvertedToLittleEndian;
            public Buf(string id, byte[] bts,int ind, int l, int w, int h, int st, int series, int time, int channel, PixelFormat px, int RGBChannels,int bitsPerPixel, bool littleEndian)
            {
                index = ind;
                ConvertedToLittleEndian = false;
                this.id = id;
                //bytes = buffer;
                this.SizeX = w;
                this.SizeY = h;
                this.length = l;
                this.stride = st;
                this.series = series;
                this.time = time;
                this.channel = channel;
                this.pixelFormat = px;
                this.littleEndian = littleEndian;
                this.bitsPerPixel = bitsPerPixel;
                RGBChannelsCount = RGBChannels;
                file = MemoryMappedFile.CreateNew(id, bts.Length);
                stream = file.CreateViewStream(0, length, MemoryMappedFileAccess.ReadWrite);
                read = new BinaryReader(stream);
                writer = new BinaryWriter(stream);
                SetBytes(bts);
                if (!littleEndian)
                {
                    //We need to convert this buffer to little endian.
                    ToLittleEndian();
                }
                
            }

            public byte[] GetBytes()
            {
                read.BaseStream.Position = 0;
                return read.ReadBytes(length);
            }

            public void SetBytes(byte[] bytes)
            {
                writer.Seek(0, SeekOrigin.Begin);
                writer.Write(bytes, 0, length);
            }

            public void SetByte(byte bt, int ind)
            {
                writer.Seek(ind, SeekOrigin.Begin);
                writer.Write(bt);
            }

            public void SetByte(byte[] bytes, int ind)
            {
                writer.Seek(ind, SeekOrigin.Begin);
                writer.Write(bytes, ind, length);
            }

            public void SetBuffer(Bitmap bitmap)
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                pixelFormat = data.PixelFormat;
                IntPtr ptr = data.Scan0;
                int length = data.Stride * bitmap.Height;
                byte[] bytes = new byte[length];
                Marshal.Copy(ptr, bytes, 0, length);
                SetBytes(bytes);
                /*
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                IntPtr ptr = data.Scan0;
                int length = data.Stride * bitmap.Height;
                bytes = new byte[length];
                Marshal.Copy(ptr, bytes, 0, length);
                bitmap.UnlockBits(data);

                int length = stride * SizeY;
                MemoryMappedFile mmf = MemoryMappedFile.OpenExisting(id);
                MemoryMappedViewStream stream = mmf.CreateViewStream(0, length);
                BinaryWriter writer = new BinaryWriter(stream);
                writer.Flush();
                writer.Write()
                    */
                bitmap.UnlockBits(data);
                bitmap.Dispose();
            }

            public void Flush()
            {
                //Here we write any changes made to the 
                writer.Flush();
            }

            public unsafe Bitmap GetBuffer()
            {
                Bitmap bitmap;
                fixed (byte* ptr = GetBytes())
                {
                    bitmap = new Bitmap(SizeX, SizeY, stride, pixelFormat, new IntPtr(ptr));
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
                    bitmap = new Bitmap(SizeX, SizeY, stride, pixelFormat, new IntPtr(ptr));
                    bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                }
                SetBuffer(bitmap);
                ConvertedToLittleEndian = true;
                littleEndian = true;
            }

            public unsafe void ToBigEndian()
            {
                //Here we convert this buffer to big endian.
                byte[] bytes = GetBytes();
                Array.Reverse(bytes);
                Bitmap bitmap;
                fixed (byte* ptr = bytes)
                {
                    bitmap = new Bitmap(SizeX, SizeY, stride, pixelFormat, new IntPtr(ptr));
                }
                bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                SetBuffer(bitmap);
                bitmap.Dispose();
            }

            private static Bitmap bitmap;
            private static BitmapData data;
            public byte[] GetEndianBytes()
            {
                //Here we get the bytes correcting for Endiannness.
                bitmap = GetBuffer();
                data = bitmap.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, pixelFormat);
                IntPtr ptr = data.Scan0;
                int length = this.bytes.Length;
                byte[] bytes = new byte[length];
                Marshal.Copy(ptr, bytes, 0, length);
                bitmap.UnlockBits(data);
                return bytes;
            }

            public unsafe Bitmap GetFiltered(IntRange range)
            {
                Bitmap bitmap = GetBuffer();
                if (bitsPerPixel > 8)
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
                Bitmap bitmap = GetBuffer();
                if (bitsPerPixel > 8)
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
            public unsafe Bitmap GetRGBChannel(RGB rgb, IntRange range)
            {
                if (RGBChannelsCount == 1)
                {
                    throw new ArgumentException("Plane is not a RGB plane.");
                }

                //We dispose the previous images for the filters so there's no memory leak.
                if (b != null)
                    b.Dispose();
                if (bi != null)
                    bi.Dispose();

                Bitmap bitmap = GetBuffer();
                if (bitsPerPixel > 8)
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
                Bitmap bitmap = GetBuffer();
                if (RGBChannelsCount == 1)
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
            public int GetValue(int ix, int iy, int index)
            {
                int i = -1;
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
                if (!littleEndian)
                {
                    x = (SizeX - 1) - x;
                    y = (SizeY - 1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * index;
                        i = BitConverter.ToUInt16(bytes, index2);
                        return i;
                    }
                    else
                    {
                        int stride = SizeX;
                        int indexb = (y * stridex + x) * index;
                        i = bytes[indexb];
                        return i;
                    }
                }
                else
                {
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * index;
                        i = BitConverter.ToUInt16(bytes, index2);
                        return i;
                    }
                    else
                    {
                        int stride = SizeX;
                        int indexb = (y * stridex + x) * index;
                        i = bytes[indexb];
                        return i;
                    }
                }
            }
            public int GetValue(int ix, int iy)
            {
                if (RGBChannelsCount > 1)
                    throw new ArgumentException("Get Value(int x, int y) only supports non-RGB planes");
                int i = -1;
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
                if (!littleEndian)
                {
                    x = (SizeX - 1) - x;
                    y = (SizeY - 1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2;
                        i = BitConverter.ToUInt16(bytes, index2);
                        return i;
                    }
                    else
                    {
                        int index = (y * stridex + x) * RGBChannelsCount;
                        i = bytes[index + (2 * RGBChannelsCount)];
                        return i;
                    }
                }
                else
                {
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                        i = BitConverter.ToUInt16(bytes, index2 + (2 * RGBChannelsCount));
                        return i;
                    }
                    else
                    {
                        int index = (y * stridex + x) * RGBChannelsCount;
                        i = bytes[index + (2 * RGBChannelsCount)];
                        return i;
                    }
                }
            }
            public void SetValue(int ix, int iy, ushort value)
            {
                byte[] bts = bytes;
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
                if (!littleEndian)
                {
                    x = (SizeX - 1) - x;
                    y = (SizeY - 1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);

                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);

                        //bytes[index2] = upper;
                        //bytes[index2 + 1] = lower;
                    }
                    else
                    {
                        int index = (y * stridex + x) * RGBChannelsCount;
                        SetByte((byte)value, index);
                        bytes[index] = (byte)value;
                    }
                }
                else
                {
                    if (bitsPerPixel > 8)
                    {
                        int index2 = ((y * stridex + x) * 2 * RGBChannelsCount);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);

                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);

                        //bytes[index2] = lower;
                        //bytes[index2 + 1] = upper;
                    }
                    else
                    {
                        int index = (y * stridex + x) * RGBChannelsCount;
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

                int stride = SizeX;
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
                if (!littleEndian)
                {
                    x = (SizeX - 1) - x;
                    y = (SizeY - 1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = ((y * stride + x) * 2 * RGBChannelsCount) + (2 * RGBChannel);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);
                        bytes[index2] = upper;
                        bytes[index2 + 1] = lower;
                    }
                    else
                    {
                        int index = ((y * stride + x) * RGBChannelsCount) + (RGBChannel);
                        bytes[index] = (byte)value;
                    }
                }
                else
                {
                    if (bitsPerPixel > 8)
                    {
                        
                        int index2 = ((y * stride + x) * 2 * RGBChannelsCount);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        SetByte(upper, index2);
                        SetByte(lower, index2 + 1);
                        bytes[index2] = lower;
                        bytes[index2 + 1] = upper;
                    }
                    else
                    {
                        int index = ((y * stride + x) * RGBChannelsCount) + (RGBChannel);
                        bytes[index] = (byte)value;
                    }
                }
            }
        }

        public unsafe void Save(string savefile, int start, int count, IMetadata meta)
        {
            //Method used to save a range of an image stack defined by start & count.
            loci.formats.ImageWriter writer = new loci.formats.ImageWriter();
            writer.setMetadataRetrieve(meta);
            writer.setId(savefile);
            Buf buffer;
            Bitmap bitmap;
            long ticks = 0;
            int index = 0;
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            for (int i = start; i < (start + count); i++)
            {
                buffer = Buffers[i];
                bitmap = buffer.GetBuffer();
                byte[] bytes;
                swatch.Restart();
                if (littleEndian)
                {
                    bytes = buffer.GetEndianBytes();
                    //Planes[i].
                    Console.WriteLine("Saving" + " Plane=" + i);
                    if(imageReader.isLittleEndian() != littleEndian)
                    {
                        Console.WriteLine("Converting to Big Endian for saving.");
                        Array.Reverse(bytes);
                        fixed (byte* ptr = bytes)
                        {
                            bitmap = new Bitmap(SizeX, SizeY, Buffers[i].stride, Buffers[i].pixelFormat, new IntPtr(ptr));
                            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            writer.saveBytes(index, bytes);
                        }
                    }
                    else
                    {
                        writer.saveBytes(index, bytes);
                    }
                    Console.WriteLine("Saved plane " + i);
                    float f = (float)(i - start) / (float)(count - 1);
                    int Progress = (int)(f * 100);

                    Console.WriteLine("Progress=" + Progress + "%" + " Elapsed=" + swatch.Elapsed.ToString());
                    Application.DoEvents();
                }
                else
                {
                    bytes = buffer.GetBytes();
                    //Planes[i].
                    Console.WriteLine("Saving" + " Plane=" + i);
                    if (imageReader.isLittleEndian() != littleEndian)
                    {
                        Console.WriteLine("Converting to Big Endian for saving.");
                        Array.Reverse(bytes);
                        fixed (byte* ptr = bytes)
                        {
                            bitmap = new Bitmap(SizeX, SizeY, Buffers[i].stride, Buffers[i].pixelFormat, new IntPtr(ptr));
                            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            writer.saveBytes(index, bytes);
                        }
                    }
                    else
                    {
                        writer.saveBytes(index, bytes);
                    }
                    Console.WriteLine("Saved plane " + i);
                    float f = (float)(i - start) / (float)(count - 1);
                    int Progress = (int)(f * 100);
                    Console.WriteLine("Progress=" + Progress + "%" + " Elapsed=" + swatch.Elapsed.ToString());
                    Application.DoEvents();
                }
                ticks = swatch.ElapsedTicks;
            }
            swatch.Stop();
            Console.WriteLine("All planes saved. Elapsed:" + swatch.Elapsed);
        }

        public void SetValue(int ix, int iy, int ind, ushort value)
        {
            Buffers[ind].SetValue(ix, iy, value);
        }

        public void SetValue(int ix, int iy, ZCT coord, ushort value)
        {
            int ind = imageReader.getIndex(coord.Z, coord.C, coord.T);
            Buffers[ind].SetValue(ix, iy, value);
        }

        public class ColorS
        {
            public ushort R;
            public ushort G;
            public ushort B;
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

        // Generates a random string with a given size.    
        public string RandomString(int size)
        {
            var builder = new StringBuilder(size);

            // Unicode/ASCII Letters are divided into two blocks
            // (Letters 65–90 / 97–122):
            // The first group containing the uppercase letters and
            // the second group containing the lowercase.  

            // char is a single Unicode character  
            char offset = 'A';
            const int lettersOffset = 26; // A...Z or a..z: length=26  

            for (var i = 0; i < size; i++)
            {
                var @char = (char)_random.Next(offset, offset + lettersOffset);
                builder.Append(@char);
            }

            return builder.ToString();
        }

        public BioImage(string file)
        {
            Open(file);
        }
        public void Open(string file)
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

            int w = imageReader.getSizeX();
            int h = imageReader.getSizeY();
            sizeTime = imageReader.getSizeT();
            sizeZ = imageReader.getSizeZ();
            int stride;
            int rgbCount = imageReader.getRGBChannelCount();
            if (imageReader.getBitsPerPixel() > 8)
                stride = w * 2 * rgbCount;
            else
                stride = w * rgbCount;
            imageCount = imageReader.getImageCount();
            table = new Hashtable();
            string filename = Path.GetFileName(file);
            table.Add(filename, file);

            table.Add(filename + "/p/" + imageReader.getImageCount(), imageCount);

            string[] sts = imageReader.getSeriesUsedFiles();
            seriesCount = imageReader.getSeriesCount();
            imagesPerSeries = imageCount / seriesCount;
            
            for (int s= 0; s < seriesCount; s++)
            {
                int ind = imageReader.seriesToCoreIndex(s);
                imageReader.setSeries(ind);
                
                for (int i = 0; i < imagesPerSeries; i++)
                {
                    byte[] bytes;
                    string r = RandomString(10);
                    string id = filename + "/s/" + ind + "/i/" + r;
                    bytes = imageReader.openBytes(i);
                    int[] ints = imageReader.getZCTCoords(i);
                    int z = ints[0];
                    int channel = ints[1];
                    int time = ints[2];
                    int l = bytes.Length;
                    table.Add(id, id);
                    Buffers.Add(new Buf(id, bytes, i, l, w, h, stride, ind, time,channel, px, rgbCount, bitsPerPixel, littleEndian));
                }
                double stx = 0;
                double sty = 0;
                double stz = 0;
                double six = 0;
                double siy = 0;
                double siz = 0;

                try
                {
                    if (meta.getPixelsPhysicalSizeZ(ind) != null)
                        six = meta.getPixelsPhysicalSizeX(ind).value().doubleValue();
                    if (meta.getPixelsPhysicalSizeZ(ind) != null)
                        siy = meta.getPixelsPhysicalSizeY(ind).value().doubleValue();
                    if (meta.getPixelsPhysicalSizeZ(ind) != null)
                        siz = meta.getPixelsPhysicalSizeZ(ind).value().doubleValue();
                    if (meta.getStageLabelX(ind) != null)
                        stx = meta.getStageLabelX(ind).value().doubleValue();
                    if (meta.getStageLabelY(ind) != null)
                        sty = meta.getStageLabelY(ind).value().doubleValue();
                    if (meta.getStageLabelZ(ind) != null)
                        stz = meta.getStageLabelZ(ind).value().doubleValue();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
                Volumes.Add(new VolumeD(new Point3D(stx, sty, stz), new Point3D(six, siy, siz)));
            }
            int SizeC = imageReader.getSizeC();
            int ccount = SizeC / imageReader.getRGBChannelCount();
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

            if (imageReader.getRGBChannelCount() == 3)
            {
                //We copy the first channel so that each RGB channel has a Channel attribute,
                //despite the file not specifying them.
                Channels.Add(Channels[0].Copy());
                Channels.Add(Channels[0].Copy());
            }

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
                        int stride2 = SizeX * RGBCount;
                        for (y = SizeY - 1; y > -1; y--)
                        {
                            for (x = SizeX - 1; x > -1; x--)
                            {
                                //index is for destination image and index2 for source image
                                index = ((SizeY - y) * stride1) + ((SizeX - x) * 3);
                                //For 16bit (2*8bit) images we multiply buffer index by 2
                                index2 = (y * stride2 + (x * RGBCount)) * 2;
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
                        int stride2 = SizeX * RGBCount;
                        for (y = 0; y < SizeY; y++)
                        {
                            for (x = 0; x < SizeX; x++)
                            {
                                //index is for destination image and index2 for source image
                                index = ((SizeY - y) * stride1) + ((SizeX - x) * 3);
                                //For 16bit (2*8bit) images we multiply buffer index by 2
                                index2 = (y * stride2 + (x * RGBCount)) * 2;
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
    public class View
    {
        public BioImage.Buf plane;
        public BioImage.ZCT coord;
        public View(BioImage.Buf buf, BioImage.ZCT coord)
        {
            plane = buf;
        }

        public Bitmap GetPlane(BioImage.Buf plane, BioImage.ZCT coord)
        {
            return plane.GetBuffer();
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
