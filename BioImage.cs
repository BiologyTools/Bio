using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
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
        public VolumeD Volume;
        public IFormatReader reader;
        public IFormatWriter writer;
        public IMetadata meta;
        public Bitmap rgbBitmap, planeBitmap;
        public BitmapData rgbBitmapData, planeBitmapData;
        public Plane[] Planes;
        public Plane plane;
        public byte[][] Bytes;
        public string FileOrig, file, NameOrig, Name, SliceOrder;
        public bool copied, littleEndian, convertedToLittleEndian, convertedBGRtoRGB, isRGB;
        public int SizeX, SizeY, SizeZ, SizeC, SizeT, imageCount, series, seriesCount, RGBChannelsCount, bitsPerPixel, pixelType;
        public double Exposure;
        public List<Channel> Channels = new List<Channel>();
        public Channel RChannel, GChannel, BChannel;
        public Plane RPlane, GPlane, BPlane;
        public int[] rgbChannels = new int[3];
        public long frameTicks;
        public int Rz = 0;
        public int Gz = 0;
        public int Bz = 0;
        public int Time = 0;
        public int Progress;
        
        public static LevelsLinear filter8 = new LevelsLinear();
        public static LevelsLinear16bpp filter16 = new LevelsLinear16bpp();
        private ReplaceChannel replaceRFilter;
        private ReplaceChannel replaceGFilter;
        private ReplaceChannel replaceBFilter;
        private static ExtractChannel extractR = new ExtractChannel(AForge.Imaging.RGB.R);
        private static ExtractChannel extractG = new ExtractChannel(AForge.Imaging.RGB.G);
        private static ExtractChannel extractB = new ExtractChannel(AForge.Imaging.RGB.B);

        private static Stopwatch watch = new Stopwatch();
        private Stopwatch imageWatch = new Stopwatch();
        public void UpdateBitmap(int z, int channel, int time)
        {
            imageWatch.Restart();
            Rz = reader.getIndex(z, RChannel.Index, time);
            Gz = reader.getIndex(z, GChannel.Index, time);
            Bz = reader.getIndex(z, BChannel.Index, time);
            UpdateRGBChannels();
            Time = time;
            if (RGBChannelsCount == 1)
            {
                replaceRFilter.ChannelImage = RPlane.GetFiltered(RChannel.range);
                replaceGFilter.ChannelImage = GPlane.GetFiltered(GChannel.range);
                replaceBFilter.ChannelImage = BPlane.GetFiltered(BChannel.range);
                replaceRFilter.ApplyInPlace(rgbBitmap);
                replaceGFilter.ApplyInPlace(rgbBitmap);
                replaceBFilter.ApplyInPlace(rgbBitmap);
                //We dispose the image channels now that we have the combined image. Otherwise they cause high memory usage.
                replaceRFilter.ChannelImage.Dispose();
                replaceGFilter.ChannelImage.Dispose();
                replaceBFilter.ChannelImage.Dispose();
            }
            else
            {
                rgbBitmap = Planes[Rz].GetFiltered(RChannel.range, GChannel.range, BChannel.range);
            }
            frameTicks = imageWatch.ElapsedTicks;
            imageWatch.Stop();
        }
        public bool UpdatePlane(int z, int channel, int time)
        {
            imageWatch.Restart();
            Rz = z;
            Time = time;
            int i = 0;
            if (RGBChannelsCount == 1)
            {
                i = reader.getIndex(z, channel, time);
                plane = Planes[i];
                planeBitmap = plane.GetFiltered(RChannel.range);
            }
            else
            {
                plane = Planes[i];
                if (channel == 0)
                    planeBitmap = plane.GetRGBChannel(RGB.Red, RChannel.range);
                else
                if (channel == 1)
                    planeBitmap = plane.GetRGBChannel(RGB.Green, GChannel.range);
                else
                if (channel == 2)
                    planeBitmap = plane.GetRGBChannel(RGB.Blue, BChannel.range);
            }
            frameTicks = imageWatch.ElapsedTicks;
            imageWatch.Stop();
            return true;
        }
        public void UpdateRGBChannels()
        {
            RChannel = Channels[rgbChannels[0]];
            GChannel = Channels[rgbChannels[1]];
            BChannel = Channels[rgbChannels[2]];
            RPlane = Planes[Rz];
            GPlane = Planes[Gz];
            BPlane = Planes[Bz];
        }
        public int Width
        {
            get
            {
                return SizeX;
            }
        }
        public int Height
        {
            get
            {
                return SizeY;
            }
        }
        public bool TimeEnabled
        {
            get
            {
                if (SizeT > 0)
                    return true;
                else
                    return false;
            }
        }
        public struct Plane
        {
            public PixelFormat pixelFormat; 
            public byte[] bytes;
            public bool convertedToLittleEndian, convertedToRGB, isRGB;
            public int index, z, channel, time, w, h, bitsPerPixel, RGBChannelsCount, stride;
            public bool update;
            private bool bigEndian, littleEndian;
            public bool BigEndian
            {
                get
                {
                    return bigEndian;
                }
                set
                {
                    bigEndian = value;
                    if (bigEndian)
                        littleEndian = false;
                    else
                        littleEndian = true;
                }
            }
            public bool LittleEndian
            {
                get
                {
                    return littleEndian;
                }
                set
                {
                    littleEndian = value;
                    if (littleEndian)
                        bigEndian = false;
                    else
                        bigEndian = true;
                }
            }
            public unsafe Plane(IFormatReader reader, int index, PixelFormat pixelFormat, bool convertToLittleEndian)
            {
                z = 0;
                channel = 0;
                time = 0;
                update = true;
                bitsPerPixel = reader.getBitsPerPixel();
                isRGB = reader.isRGB();
                littleEndian = reader.isLittleEndian();

                if (littleEndian)
                    bigEndian = false;
                else
                    bigEndian = true;

                convertedToRGB = false;
                RGBChannelsCount = reader.getRGBChannelCount();
                this.pixelFormat = pixelFormat;
                this.index = index;
                w = reader.getSizeX();
                h = reader.getSizeY();
                if(bitsPerPixel > 8)
                    stride = w * 2 * RGBChannelsCount;
                else
                    stride = w * RGBChannelsCount;
                int[] ints = reader.getZCTCoords(index);
                this.z = ints[0];
                this.channel = ints[1];
                this.time = ints[2];
                Bitmap bitmap = null;
                bytes = reader.openBytes(index);
                if (RGBChannelsCount == 1)
                {
                    if (convertToLittleEndian && !littleEndian)
                    {
                        Array.Reverse(bytes);
                        fixed (byte* ptr = bytes)
                        {
                            bitmap = new Bitmap(w, h, stride, pixelFormat, new IntPtr(ptr));
                            bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        }
                        //we set bytes to be bigEndian
                        littleEndian = true;
                        bigEndian = false;
                        convertedToLittleEndian = true;
                        SetRawBytes(bitmap);
                        return;
                    }
                    else
                    {
                        convertedToLittleEndian = false;
                        return;
                    }
                }
                else
                {
                    fixed (byte* ptr = bytes)
                    {
                        bitmap = new Bitmap(w, h, stride, pixelFormat, new IntPtr(ptr));
                    }
                    convertedToLittleEndian = false;
                    //Bioformats stores color planes in BGR order so we need to conver it to RGB.
                    bitmap = switchRedBlue(bitmap);
                    SetRawBytes(bitmap);
                    convertedToRGB = true;
                }

            }
            public int SizeX
            {
                get
                {
                    return w;
                }
            }
            public int SizeY
            {
                get
                {
                    return h;
                }
            }
            public static byte[] BitmapToBytes(Bitmap bitmap)
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, bitmap.Width, bitmap.Height), ImageLockMode.ReadWrite, bitmap.PixelFormat);
                IntPtr ptr = data.Scan0;
                int length = data.Stride * bitmap.Height;
                byte[] bytes = new byte[length];
                Marshal.Copy(ptr, bytes, 0, length);
                bitmap.UnlockBits(data);
                return bytes;
            }
            public void Dispose()
            {
                if(bitmap!=null)
                    bitmap.Dispose();
                if (b != null)
                    b.Dispose();
                if (bi != null)
                    bi.Dispose();
                bytes = null;
            }
            public void SetRawBytes(Bitmap bitmap)
            {
                BitmapData data = bitmap.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, pixelFormat);
                IntPtr ptr = data.Scan0;
                int length = data.Stride * bitmap.Height;
                bytes = new byte[length];
                Marshal.Copy(ptr, bytes, 0, length);
                bitmap.UnlockBits(data);
            }

            private static Bitmap bitmap;
            private static BitmapData data;
            public byte[] GetEndianBytes()
            {
                //Here we get the bytes correcting for Endiannness.
                bitmap = GetBitmap();
                data = bitmap.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, pixelFormat);
                IntPtr ptr = data.Scan0;
                int length = this.bytes.Length;
                byte[] bytes = new byte[length];
                Marshal.Copy(ptr, bytes, 0, length);
                bitmap.UnlockBits(data);
                return bytes;
            }
            public unsafe Bitmap GetBitmap()
            {
                Bitmap bitmap;
                if (!littleEndian)
                {
                    fixed (byte* ptr = bytes)
                    {
                        bitmap = new Bitmap(w, h, stride, pixelFormat, new IntPtr(ptr));
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                }
                else
                {
                    fixed (byte* ptr = bytes)
                    {
                        bitmap = new Bitmap(w, h, stride, pixelFormat, new IntPtr(ptr));
                    }
                }
                return bitmap;
            }
            public unsafe Bitmap GetFiltered(IntRange range)
            {
                Bitmap bitmap = GetBitmap();
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
                Bitmap bitmap = GetBitmap();
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
                
                Bitmap bitmap = GetBitmap();
                if (bitsPerPixel > 8)
                {
                    // set ranges
                    filter16.InRed = range;
                    filter16.InGreen = range;
                    filter16.InBlue = range;
                    
                    if (rgb == RGB.Red)
                    {
                        b = extractR.Apply(bitmap);
                        bi = filter16.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                    if (rgb == RGB.Green)
                    {
                        b = extractG.Apply(bitmap);
                        bi = filter16.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                    if (rgb == RGB.Blue)
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
                    if (rgb == RGB.Red)
                    {
                        b = extractR.Apply(bitmap);
                        bi = filter8.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                    if (rgb == RGB.Green)
                    {
                        b = extractG.Apply(bitmap);
                        bi = filter8.Apply(b);
                        b.Dispose();
                        bitmap.Dispose();
                        return bi;
                    }
                    if (rgb == RGB.Blue)
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
                if (RGBChannelsCount == 1)
                    return null;
                if(rgb == RGB.Red)
                    return extractR.Apply(bitmap);
                if (rgb == RGB.Green)
                    return extractG.Apply(bitmap);
                if (rgb == RGB.Blue)
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
            public int GetValue(int ix,int iy)
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
                    x = SizeX-1;
                if (iy >= SizeY)
                    y = SizeY-1;
                if (!littleEndian)
                {
                    x = (w-1) - x;
                    y = (h-1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                        i = BitConverter.ToUInt16(bytes, index2);
                        return i;
                    }
                    else
                    {
                        int stride = w;
                        int index = (y * stridex + x) * RGBChannelsCount;
                        i = bytes[index];
                        return i;
                    }
                }
                else
                {
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                        i = BitConverter.ToUInt16(bytes, index2);
                        return i;
                    }
                    else
                    {
                        int stride = w;
                        int index = (y * stridex + x) * RGBChannelsCount;
                        i = bytes[index];
                        return i;
                    }
                }
            }
            public int GetValue(int ix, int iy, int RGBChannel)
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
                if (ix >= w)
                    x = w-1;
                if (iy >= h)
                    y = h-1;
                if (!littleEndian)
                {
                    x = (w - 1) - x;
                    y = (h - 1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                        i = BitConverter.ToUInt16(bytes, index2 + (2 * RGBChannel));
                        return i;
                    }
                    else
                    {
                        int index = (y * stridex + x) * RGBChannelsCount;
                        i = bytes[index + (2 * RGBChannel)];
                        return i;
                    }
                }
                else
                {
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                        i = BitConverter.ToUInt16(bytes, index2 + (2 * RGBChannel));
                        return i;
                    }
                    else
                    {
                        int index = (y * stridex + x) * RGBChannelsCount;
                        i = bytes[index + (2 * RGBChannel)];
                        return i;
                    }
                }
            }
            public void SetValue(int ix, int iy, ushort value)
            {
                int stridex = SizeX;
                //For 16bit (2*8bit) images we multiply buffer index by 2
                int x = ix;
                int y = iy;
                if (ix < 0)
                    x = 0;
                if (iy < 0)
                    y = 0;
                if (ix >= w)
                    x = w-1;
                if (iy >= h)
                    y = h-1;
                if (!littleEndian)
                {
                    x = (w - 1) - x;
                    y = (h - 1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stridex + x) * 2 * RGBChannelsCount;
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        bytes[index2] = upper;
                        bytes[index2+1] = lower;
                    }
                    else
                    {
                        int index = (y * stridex + x) * RGBChannelsCount;
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
                        bytes[index2] = lower;
                        bytes[index2+1] = upper;
                    }
                    else
                    {
                        int index = (y * stridex + x) * RGBChannelsCount;
                        bytes[index] = (byte)value;
                    }
                }
            }
            public void SetValue(int ix, int iy, int RGBChannel, ushort value)
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
                if (ix >= w)
                    x = w - 1;
                if (iy >= h)
                    y = h - 1;
                if (!littleEndian)
                {
                    x = (w - 1) - x;
                    y = (h - 1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = ((y * stride + x) * 2 * RGBChannelsCount) + (2 * RGBChannel);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        bytes[index2] = upper;
                        bytes[index2+1] = lower;
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
                        int index2 = ((y * stride + x) * 2 * RGBChannelsCount) + (2 * RGBChannel);
                        byte upper = (byte)(value >> 8);
                        byte lower = (byte)(value & 0xff);
                        bytes[index2] = lower;
                        bytes[index2+1] = upper;
                    }
                    else
                    {
                        int index2 = ((y * stride + x) * RGBChannelsCount) + (RGBChannel);
                        bytes[index] = (byte)value;
                    }
                }
            }
        }
        public int GetPlanePixel(RGB rgb,int x,int y)
        {
            if(rgb == RGB.Red)
                return Planes[Rz].GetValue(x, y);
            if (rgb == RGB.Green)
                return Planes[Gz].GetValue(x, y);
            if (rgb == RGB.Blue)
                return Planes[Bz].GetValue(x, y);
            return -1;
        }
        public int GetPlanePixel(int index, int x, int y)
        {
            int i = Planes[index].GetValue(x, y);
            return i;
        }
        public ColorS GetPixelColor(int x, int y)
        {
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            int r = RPlane.GetValue(x, y);
            int g = GPlane.GetValue(x, y);
            int b = BPlane.GetValue(x, y);
            ColorS s = new ColorS();
            s.R = (ushort)r;
            s.G = (ushort)g;
            s.B = (ushort)b;
            return s;
        }

        public void SetPlanePixel(int index, int x, int y, ushort value)
        {
            Planes[index].SetValue(x, y, value);
        }
        
        public void SetRGBChannelIndex(RGB rgb,int index)
        {
            if(rgb == RGB.Red)
            {
                rgbChannels[0] = index;
                Channels[index].rgb = rgb;
                RChannel = Channels[index];
            }
            else
            if (rgb == RGB.Green)
            {
                rgbChannels[1] = index;
                Channels[index].rgb = rgb;
                GChannel = Channels[index];
            }
            else
            {
                rgbChannels[2] = index;
                Channels[index].rgb = rgb;
                BChannel = Channels[index];
            }
        }
        /*
        public bool ClearRGBChannels(byte b)
        {
            Stopwatch w = new Stopwatch();
            w.Start();

            g = Graphics.FromImage(image);

            /*
            TimeSpan t = w.Elapsed;
            w.Restart();
            data = image.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, image.PixelFormat);
            unsafe
            {
                int index, x, y;
                //stride 1 is for the destination image
                //destination image will always be 3channels(RGB) with 24bits
                int stride1 = image.Width * 3;
                byte* pix = (byte*)data.Scan0;
                for (y = 0; y < image.Height; y++)
                {
                    for (x = 0; x < image.Width; x++)
                    {
                        //index is for destination image
                        index = (y * stride1 + (x * 3));
                        pix[index] = b;
                        pix[index + 1] = b;
                        pix[index + 2] = b;
                    }
                }
            }//unsafe
            image.UnlockBits(data);
            
            TimeSpan t2 = w.Elapsed;
            w.Stop();
            return true;
        }
        */
        public BioImage(string path)
        {
            Open(path);
        }

        public unsafe void Save(string savefile, int start, int count)
        {
            //Method used to save a range of an image stack defined by start & count.
            writer = new ImageWriter();
            writer.setMetadataRetrieve(meta);
            writer.setId(savefile);
            Bitmap bit = null;
            byte[] bytes = new byte[Bytes[0].Length];
            watch.Restart();
            int index = 0;
            if (littleEndian)
            {
                for (int i = start; i < (start + count); i++)
                {
                    bytes = Planes[i].GetEndianBytes();
                    //Planes[i].
                    Console.WriteLine("Saving" + " Plane=" + i);
                    if (convertedToLittleEndian)
                    {
                        Console.WriteLine("Converting to Big Endian for saving.");
                        Array.Reverse(bytes);
                        fixed (byte* ptr = bytes)
                        {
                            bit = new Bitmap(SizeX, SizeY, Planes[0].stride, Planes[0].pixelFormat, new IntPtr(ptr));
                        }
                        bit.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        bytes = BioImage.Plane.BitmapToBytes(bit);
                        writer.saveBytes(index, bytes);
                    }
                    else
                    {
                        writer.saveBytes(index, bytes);
                    }
                    Console.WriteLine("Saved plane " + i);
                    float f = (float)(i - start) / (float)(count - 1);
                    Progress = (int)(f * 100);
                    TimeSpan t = watch.Elapsed;
                    Console.WriteLine("Progress=" + Progress + "%" + " Elapsed=" + t.ToString());
                    Application.DoEvents();
                    index++;
                }
            }
            else
            {
                for (int i = start; i < (start + count); i++)
                {
                    bytes = Planes[i].GetEndianBytes();
                    //Planes[i].
                    Console.WriteLine("Saving" + " Plane=" + i);
                    if (convertedToLittleEndian)
                    {
                        Console.WriteLine("Converting to Big Endian for saving.");
                        Array.Reverse(bytes);
                        fixed (byte* ptr = bytes)
                        {
                            bit = new Bitmap(SizeX, SizeY, Planes[0].stride, Planes[0].pixelFormat, new IntPtr(ptr));
                            bit = Planes[i].switchRedBlue(bit);
                            bit.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            bytes = BioImage.Plane.BitmapToBytes(bit);
                            writer.saveBytes(index, bytes);
                        }
                    }
                    else
                    {
                        writer.saveBytes(index, bytes);
                    }
                    Console.WriteLine("Saved plane " + i);
                    float f = (float)(i - start) / (float)(count - 1);
                    Progress = (int)(f * 100);
                    TimeSpan t = watch.Elapsed;
                    Console.WriteLine("Progress=" + Progress + "%" + " Elapsed=" + t.ToString());
                    Application.DoEvents();
                    index++;
                }
            }

            Console.WriteLine("All planes saved.");
        }

        public void Dispose()
        {
            for (int i = 0; i < Planes.Length; i++)
            {
                Planes[i].Dispose();
            }
            Planes = null;
            Channels.Clear();
            Channels = null;
            Rz = 0;
            Gz = 0;
            Bz = 0;
        }

        ~BioImage()
        {
            Dispose();
        }

        public enum RGB
        {
            Red,
            Green,
            Blue,
            None
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

            public RGB rgb = RGB.Red;
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

        public class ROI
        {
            public Rectangle rectangle;
            public ColorS color;
            public List<System.Drawing.Point> points;

        }
        
        public unsafe bool AutoThresholdChannel(Channel c1)
        {
            c1.Max = c1.Min;
            int index, x, y;
            int stride = SizeX;
            byte[] bytes;

            for (int time = 0; time < SizeT; time++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    int i = reader.getIndex(z, c1.Index, time);
                    bytes = Planes[i].bytes;
                    if (!littleEndian)
                    {
                        if (bitsPerPixel > 8)
                        {
                            for (y = SizeY - 1; y > -1; y--)
                            {
                                for (x = SizeX - 1; x > -1; x--)
                                {
                                    //For 16bit (2*8bit) images we multiply buffer index by 2
                                    index = (y * stride + x) * 2;
                                    int px = BitConverter.ToUInt16(bytes, index);
                                    if (px > c1.Max)
                                        c1.Max = px;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (bitsPerPixel > 8)
                        {
                            for (y = 0; y < SizeY; y++)
                            {
                                for (x = 0; x < SizeX; x++)
                                {
                                    //For 16bit (2*8bit) images we multiply buffer index by 2
                                    index = (y * stride + x) * 2;
                                    int px = BitConverter.ToUInt16(bytes, index);
                                    if (px > c1.Max)
                                        c1.Max = px;
                                }
                            }
                        }
                    }
                }
            }
            return true;
        }
        public void AutoThresholdImage()
        {
            foreach (Channel c in Channels)
            {
                AutoThresholdChannel(c);
            }
        }
        /*
        public void Save(string path)
        {
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            //meta = service.createOMEXMLMetadata();

            // create a writer that will automatically handle any supported output format
            IFormatWriter writer = new ImageWriter();
            // give the writer a MetadataRetrieve object, which encapsulates all of the
            // dimension information for the dataset (among many other things)
            //writer.setMetadataRetrieve(meta);
            //Console.WriteLine("Save-Index=" + index);
            writer.setMetadataRetrieve(meta);
            writer.setId(path);
            Console.WriteLine("Path=" + path);
            for (int series = 0; series < seriesCount; series++)
            {
                writer.setSeries(series);
                for (int i = 0; i < imageCount; i++)
                {
                    //long ptr = long.Parse(fs[i]);
                    //Console.WriteLine("ptr=" + ptr);
                    //IntPtr iptr = (IntPtr)ptr;
                    //Object obj = GCHandle.FromIntPtr(iptr).Target;
                    byte[] bts = Planes[i].GetBytes();
                    if (convertedToLittleEndian)
                    {
                        Array.Reverse(bts);
                        Bitmap b = Planes[i].GetRawBuffer();
                        b.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        byte[] bytes = BioImage.Plane.BitmapToBytes(b);
                        bool done = false;
                        do
                        {
                            try
                            {
                                writer.saveBytes(i, bytes);
                                done = true;
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.ToString());
                            }
                        } while (!done);
                        //writer.saveBytes(i, plane.bytes);
                        Array.Reverse(bts);
                    }
                    else
                        writer.saveBytes(i, bts);
                    Application.DoEvents();
                    //float prog = i / count;
                    //Writers.progresses[index] = (int)prog;
                }
            }
        }
        */
        public bool Open(string file)
        {
            this.file = file;
            FileOrig = file;
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            meta = service.createOMEXMLMetadata();
            // create format reader
            reader = new ImageReader();
            reader.setMetadataStore(meta);

            // initialize file
            reader.setId(file);
            series = reader.getSeries();
            seriesCount = reader.getSeriesCount();
            bitsPerPixel = reader.getBitsPerPixel();

            pixelType = reader.getPixelType();
            RGBChannelsCount = reader.getRGBChannelCount();
            SizeX = reader.getSizeX();
            SizeY = reader.getSizeY();
            SizeZ = reader.getSizeZ();
            SizeC = reader.getSizeC();
            SizeT = reader.getSizeT();
            isRGB = reader.isRGB();
            imageCount = reader.getImageCount();
            bool orderCertain = reader.isOrderCertain();
            SliceOrder = reader.getDimensionOrder();
            littleEndian = reader.isLittleEndian();

            double stx = 0;
            double sty = 0; 
            double stz = 0;
            double six = 0;
            double siy = 0;
            double siz = 0;

            try
            {
                if (meta.getPixelsPhysicalSizeZ(series) != null)
                    six = meta.getPixelsPhysicalSizeX(series).value().doubleValue();
                if (meta.getPixelsPhysicalSizeZ(series) != null)
                    siy = meta.getPixelsPhysicalSizeY(series).value().doubleValue();
                if (meta.getPixelsPhysicalSizeZ(series) != null)
                    siz = meta.getPixelsPhysicalSizeZ(series).value().doubleValue();
                if (meta.getStageLabelX(series) != null)
                    stx = meta.getStageLabelX(series).value().doubleValue();
                if (meta.getStageLabelY(series) != null)
                    sty = meta.getStageLabelY(series).value().doubleValue();
                if (meta.getStageLabelZ(series) != null)
                    stz = meta.getStageLabelZ(series).value().doubleValue();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Volume = new VolumeD(new Point3D(stx, sty, stz), new Point3D(six, siy, siz));
            int count = SizeC / RGBChannelsCount;
            //Lets get the channels amd initialize them.
            for (int i = 0; i < count; i++)
            {
                Channel ch = new Channel(i,bitsPerPixel);
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
                            ch.rgb = RGB.Red;
                        if (ch.color.R == 0 && ch.color.G == 255 && ch.color.B == 0)
                            ch.rgb = RGB.Green;
                        if (ch.color.R == 0 && ch.color.G == 0 && ch.color.B == 255)
                            ch.rgb = RGB.Blue;
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
                    ch.rgb = RGB.Red;
                }
                else
                if (i == 1)
                {
                    rgbChannels[1] = 1;
                    ch.rgb = RGB.Green;
                }
                else
                if (i == 2)
                {
                    rgbChannels[2] = 2;
                    ch.rgb = RGB.Blue;
                }
                Channels.Add(ch);
            }

            if(RGBChannelsCount == 3)
            {
                //We copy the first channel so that each RGB channel has a Channel attribute,
                //despite the file not specifying them.
                Channels.Add(Channels[0].Copy());
                Channels.Add(Channels[0].Copy());
            }
            
            Planes = new Plane[imageCount];
            bool convertEndian = true;
            Bytes = new byte[imageCount][];
            for (int i = 0; i < imageCount; i++)
            {
                if (RGBChannelsCount == 1)
                {
                    if (bitsPerPixel > 8)
                        Planes[i] = new Plane(reader, i, PixelFormat.Format16bppGrayScale, convertEndian);
                    else
                        Planes[i] = new Plane(reader, i, PixelFormat.Format8bppIndexed, convertEndian);
                }
                else
                {
                    if (bitsPerPixel > 8)
                        Planes[i] = new Plane(reader, i, PixelFormat.Format48bppRgb, convertEndian);
                    else
                        Planes[i] = new Plane(reader, i, PixelFormat.Format24bppRgb, convertEndian);
                }
                Bytes[i] = Planes[i].bytes;
            }

            if (Planes[0].convertedToLittleEndian)
            {
                littleEndian = true;
                convertedToLittleEndian = true;
            }
            else
                convertedToLittleEndian = false;

            if (Planes[0].convertedToRGB)
            {
                convertedBGRtoRGB = true;
            }
            else
                convertedBGRtoRGB = false;
            //Now that we have loaded the channels we can update the RGB channels.
            UpdateRGBChannels();
            Bitmap planeBitmap;
            if (bitsPerPixel > 8)
            {
                rgbBitmap = new Bitmap(SizeX, SizeY, PixelFormat.Format48bppRgb);
                rgbBitmapData = rgbBitmap.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, rgbBitmap.PixelFormat);
                rgbBitmap.UnlockBits(rgbBitmapData);
                planeBitmap = new Bitmap(SizeX, SizeY, PixelFormat.Format16bppGrayScale);
            }
            else
            {
                rgbBitmap = new Bitmap(SizeX, SizeY, PixelFormat.Format24bppRgb);
                rgbBitmapData = rgbBitmap.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, rgbBitmap.PixelFormat);
                rgbBitmap.UnlockBits(rgbBitmapData);
                planeBitmap = new Bitmap(SizeX, SizeY, PixelFormat.Format8bppIndexed);
            }
            replaceRFilter = new ReplaceChannel(AForge.Imaging.RGB.R, planeBitmap);
            replaceGFilter = new ReplaceChannel(AForge.Imaging.RGB.G, planeBitmap);
            replaceBFilter = new ReplaceChannel(AForge.Imaging.RGB.B, planeBitmap);
            planeBitmap.Dispose();
            plane = Planes[0];
            UpdatePlane(0,0,0);
            return true;
        }

        public static Point3D GetStagePosition(string file)
        {
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            IMetadata meta = service.createOMEXMLMetadata();

            // create format reader
            ImageReader reader = new ImageReader();
            reader.setMetadataStore(meta);
            int series = reader.getSeries();
            // initialize file
            reader.setId(file);
            double stx = 0;
            double sty = 0;
            double stz = 0;
            try
            {
                if (meta.getStageLabelX(series) != null)
                    stx = meta.getStageLabelX(series).value().doubleValue();
                if (meta.getStageLabelY(series) != null)
                    sty = meta.getStageLabelY(series).value().doubleValue();
                if (meta.getStageLabelZ(series) != null)
                    stz = meta.getStageLabelZ(series).value().doubleValue();
            }
            catch (Exception)
            {
                throw new InvalidDataException("File metadata doesn't contain stage & focus position.");
            }

            return new Point3D(stx, sty, stz);
        }
        public override string ToString()
        {
            if (Name == "")
                return NameOrig + " " + Volume.Location.ToString();
            else
                return Name + " " + Volume.Location.ToString();
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
