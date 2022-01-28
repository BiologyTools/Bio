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
using AForge.Imaging;
using loci.formats;
using loci.formats.meta;
using loci.common.services;
using loci.formats.services;
using System.Management;
using System.Runtime.InteropServices;

namespace BioImage
{
    public class BioImage
    {
        public VolumeD volume;
        public IFormatReader reader;
        public IFormatWriter writer;
        public Bitmap bitmap;
        public BitmapData data;
        public IMetadata meta;
        public Plane[] Planes;
        public Plane plane;
        public string FileOrig;
        public string file;
        public string NameOrig;
        public string Name;
        public string SliceOrder;
        public bool copied, littleEndian;
        public bool loadedAlltoMemory = false;
        public int imageCount, series, seriesCount;
        public int RGBChannelsCount, bitsPerPixel, pixelType;
        public int SizeX, SizeY, SizeZ, SizeC, SizeT;
        public double Exposure;
        public List<Channel> Channels = new List<Channel>();
        private int[] rgbChannels = new int[3];
        private Stopwatch watch = new Stopwatch();
        public TimeSpan frameTime;

        public Channel RChannel;
        public Channel GChannel;
        public Channel BChannel;
        public Plane RPlane;
        public Plane GPlane;
        public Plane BPlane;
        public int Rz, Gz, Bz;
        public void UpdateRGBChannels()
        {
            RChannel = Channels[rgbChannels[0]];
            GChannel = Channels[rgbChannels[1]];
            BChannel = Channels[rgbChannels[2]];
        }
        public unsafe void UpdateImage(int z, int time)
        {
            Rz = reader.getIndex(z, RChannel.Index, time);
            Gz = reader.getIndex(z, GChannel.Index, time);
            Bz = reader.getIndex(z, BChannel.Index, time);
            RPlane = Planes[Rz];
            GPlane = Planes[Gz];
            BPlane = Planes[Bz];
            //byte[] openBytes(int no, int x, int y, int w, int h)
            int index, index2, x, y;
            if (!littleEndian)
            {
                //stride 1 is for the destination image and stride2 for source image.
                //destination image will always be 3channels(RGB) with 24bits
                int stride1 = SizeX * 3;
                int stride2 = SizeX * 1;
                byte* pix = (byte*)data.Scan0;
                float p16z1, p16z2, p16z3, p8z1, p8z2, p8z3;
                if (bitsPerPixel > 8)
                {
                    for (y = SizeY - 1; y > -1; y--)
                    {
                        for (x = SizeX - 1; x > -1; x--)
                        {
                            //index is for destination image and index2 for source image
                            index = ((SizeY - (y + 1)) * stride1) + ((SizeX - (x + 1)) * 3);
                            //For 16bit (2*8bit) images we multiply buffer index by 2
                            index2 = (y * stride2 + (x * 1)) * 2;
                            p16z1 = BitConverter.ToUInt16(RPlane.bytes, index2);
                            p8z1 = ((p16z1 - RChannel.min) / (RChannel.max)) * 255;
                            p16z2 = BitConverter.ToUInt16(GPlane.bytes, index2);
                            p8z2 = ((p16z2 - GChannel.min) / (GChannel.max)) * 255;
                            p16z3 = BitConverter.ToUInt16(BPlane.bytes, index2);
                            p8z3 = ((p16z3 - BChannel.min) / (BChannel.max)) * 255;
                            pix[index] = (byte)p8z1;
                            pix[index + 1] = (byte)p8z2;
                            pix[index + 2] = (byte)p8z3;
                        }
                    }
                }
            }
            else
            {
                //stride 1 is for the destination image and stride2 for source image.
                //destination image will always be 3channels(RGB) with 24bits
                int stride1 = SizeX * 3;
                int stride2 = SizeX * 1;
                byte* pix = (byte*)data.Scan0;
                float p16z1, p16z2, p16z3, p8z1, p8z2, p8z3;
                if (bitsPerPixel > 8)
                {
                    for (y = 0; y < SizeY; y++)
                    {
                        for (x = 0; x < SizeX; x++)
                        {
                            //index is for destination image and index2 for source image
                            index = (y * stride1) + (x * 3);
                            //For 16bit (2*8bit) images we multiply buffer index by 2
                            index2 = (y * stride2 + (x * 1)) * 2;
                            p16z1 = BitConverter.ToUInt16(RPlane.bytes, index2);
                            p8z1 = ((p16z1 - RChannel.min) / (RChannel.max)) * 255;
                            p16z2 = BitConverter.ToUInt16(GPlane.bytes, index2);
                            p8z2 = ((p16z2 - GChannel.min) / (GChannel.max)) * 255;
                            p16z3 = BitConverter.ToUInt16(BPlane.bytes, index2);
                            p8z3 = ((p16z3 - BChannel.min) / (BChannel.max)) * 255;
                            pix[index] = (byte)p8z1;
                            pix[index + 1] = (byte)p8z2;
                            pix[index + 2] = (byte)p8z3;
                        }
                    }
                }
            }
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
        public class Plane
        {
            public IFormatReader reader;
            public IFormatWriter writer;
            public byte[] bytes;
            public unsafe Bitmap GetBitmap()
            {
                int stride = w * 2;
                if (!littleEndian)
                {
                    //Array.Reverse(bt);
                    fixed (byte* ptr = bytes)
                    {
                        Bitmap bitmap = new Bitmap(w, h, stride, pixelFormat, new IntPtr(ptr));
                        bitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        return bitmap;
                    }
                }
                else
                {
                    fixed (byte* ptr = bytes)
                    {
                        Bitmap bitmap = new Bitmap(w, h, stride, pixelFormat, new IntPtr(ptr));
                        return bitmap;
                    }
                }
            }
            public PixelFormat pixelFormat; 
            public bool littleEndian;
            public int bitsPerPixel;
            public int index = 0;
            public int z = 0;
            public int channel = 0;
            public int time = 0;
            public int w, h;
            public bool update = true;
            public Plane(IFormatReader reader, IFormatWriter writer, int index, int bitsPerPixel, bool littleEndian, PixelFormat pixelFormat)
            {
                this.bitsPerPixel = bitsPerPixel;
                this.littleEndian = littleEndian;
                this.pixelFormat = pixelFormat;
                int[] ints = reader.getZCTCoords(index);
                this.index = index;
                this.z = ints[0];
                this.channel = ints[1];
                this.time = ints[2];
                this.reader = reader;
                this.writer = writer;
                w = reader.getSizeX();
                h = reader.getSizeY();
                Init();
            }
            public void Init()
            {
                byte[] bt = reader.openBytes(index);
                int stride = w * 2;
                if (!littleEndian)
                {
                    Array.Reverse(bt);
                }
                bytes = bt;
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
            public int GetValue(int ix,int iy)
            {
                int stride2 = SizeX;
                //For 16bit (2*8bit) images we multiply buffer index by 2
                int x = ix;  
                int y = iy;
                if (ix < 0)
                    x = 0;
                if (iy < 0)
                    y = 0;  
                if (!littleEndian)
                {
                    x = (w-1) - x;
                    y = (h-1) - y;
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stride2 + x) * 2;
                        int i = BitConverter.ToUInt16(bytes, index2);
                        return i;
                    }
                    else
                    {
                        int stride = w;
                        int index = (y * stride + x);
                        int i = bytes[index];
                        return i;
                    }
                }
                else
                {
                    if (bitsPerPixel > 8)
                    {
                        int index2 = (y * stride2 + x) * 2; 
                        int i = BitConverter.ToUInt16(bytes, index2);
                        return i;
                    }
                    else
                    {
                        int stride = w;
                        int index = (y * stride + x);
                        int i = bytes[index];
                        return i;
                    }
                }
            }

        }
        public int GetPlanePixel(int x,int y)
        {
            int i = Planes[zcur].GetValue(x, y);
            return i;
        }
        public int GetPlanePixel(int x, int y,int index)
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
        public bool Save(string path)
        {
            writer.setId(path);
            for (int series = 0; series < reader.getSeriesCount(); series++)
            {
                //reader.setSeries(series);
                writer.setSeries(series);
                for (int i = 0; i < imageCount; i++)
                {
                    writer.saveBytes(i, reader.openBytes(i));
                    Application.DoEvents();
                }
            }
            return true;
        }

        public BioImage(string path)
        {
            FromFile(path);
            UpdateRGBChannels();
            bitmap = new Bitmap(SizeX, SizeY, PixelFormat.Format24bppRgb);
            data = bitmap.LockBits(new Rectangle(0, 0, SizeX, SizeY), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            bitmap.UnlockBits(data);
            UpdateImage(0, 0);
            //rgbimage = new RGBImage(reader, writer, bitsPerPixel, SizeX, SizeY, RChannel, GChannel, BChannel);
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
            public int max = 65535;
            public int min = 0;
            public string LightSource;
            public double LightSourceIntensity;
            public int LightSourceWavelength;
            public string ContrastMethod;
            public string IlluminationType;

            public int Index
            {
                get
                {
                    return index;
                }
            }
            
            public RGB rgb = RGB.Red;

            public Channel(int index)
            {
                this.index = index;
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
            public override string ToString()
            {
                return R + "," + G + "," + B;
            }
        }

        public void RefreshPlane()
        {
            UpdatePlane(plane);
        }
        public bool UpdatePlane(Plane plane)
        {
            this.plane = plane;
            zcur = plane.index;
            tcur = plane.time;
            int i = reader.getIndex(plane.z, plane.channel, plane.time);
            plane = Planes[i];
            return true;
        }
        public bool UpdatePlane(int z, int channel, int time)
        {
            zcur = z;
            tcur = time;
            int i = reader.getIndex(z,channel, time);
            plane = Planes[i];
            return true;
        }

        private int zcur = 0;
        private int tcur = 0;
        private byte[] bytes;
        public unsafe bool AutoThresholdChannel(Channel c1)
        {
            for (int time = 0; time < SizeT; time++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    int i = reader.getIndex(z, c1.Index, time);
                    bytes = Planes[i].bytes;
                    int index, index2, x, y;
                    if (!littleEndian)
                    {
                        //stride 1 is for the destination image and stride2 for source image.
                        //destination image will always be 3channels(RGB) with 24bits
                        int stride1 = SizeX * 3;
                        int stride2 = SizeX * RGBChannelsCount;
                        if (bitsPerPixel > 8)
                        {
                            for (y = SizeY - 1; y > -1; y--)
                            {
                                for (x = SizeX - 1; x > -1; x--)
                                {
                                    //index is for destination image and index2 for source image
                                    index = ((SizeY - y) * stride1) + ((SizeX - x) * 3);
                                    //For 16bit (2*8bit) images we multiply buffer index by 2
                                    index2 = (y * stride2 + (x * RGBChannelsCount)) * 2;
                                    int px = BitConverter.ToUInt16(bytes, index2);
                                    if (px > c1.max)
                                        c1.max = px;
                                }
                            }
                        }
                    }
                    else
                    {
                        //stride 1 is for the destination image and stride2 for source image.
                        //destination image will always be 3channels(RGB) with 24bits
                        int stride1 = SizeX * 3;
                        int stride2 = SizeX * RGBChannelsCount;
                        if (bitsPerPixel > 8)
                        {
                            for (y = 0; y < SizeY; y++)
                            {
                                for (x = 0; x < SizeX; x++)
                                {
                                    //index is for destination image and index2 for source image
                                    index = ((SizeY - y) * stride1) + ((SizeX - x) * 3);
                                    //For 16bit (2*8bit) images we multiply buffer index by 2
                                    index2 = (y * stride2 + (x * RGBChannelsCount)) * 2;
                                    int px = BitConverter.ToUInt16(bytes, index2);
                                    if (px > c1.max)
                                        c1.max = px;
                                }
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
                c.max = 0;
                AutoThresholdChannel(c);
            }
            RefreshPlane();
        }
        /*
        public ColorS GetPixel(int x,int y)
        {
            if (x < 0)
                x = 0;
            if (y < 0)
                y = 0;
            if (x >= SizeX)
            {
                x = SizeX-1;
            }
            if (y >= SizeY)
            {
                y = SizeY-1;
            }
            int h = SizeX;
            int w = SizeY;
            int ix = x;
            int iy = y;

            CurrentPlane.GetValue(x, y);
            reader.getIndex(z, c1.Index, t);
            CurrentPlane.GetValue(x, y);

            ColorS color = new ColorS();
            int z1 = reader.getIndex(zcur, ChannelR.Index, tcur);
            //byte[] openBytes(int no, int x, int y, int w, int h)
            byte[] pixelsz1 = reader.openBytes(z1);
            byte[] pixelsz2 = reader.openBytes(z2);
            byte[] pixelsz3 = reader.openBytes(z3);
            
            int index2;
            if (!littleEndian)
            {
                ix = (w - 1) - x;
                iy = (h - 1) - y;
                Array.Reverse(pixelsz1);
                Array.Reverse(pixelsz2);
                Array.Reverse(pixelsz3);
                unsafe
                {
                    int stride2 = w * RGBChannelsCount;
                    byte* pix = (byte*)data.Scan0;
                    if (bitsPerPixel > 8)
                    {
                        //For 16bit (2*8bit) images we multiply buffer index by 2
                        index2 = (iy * stride2 + (ix * RGBChannelsCount)) * 2;
                        float p16z1 = BitConverter.ToUInt16(pixelsz1, index2);
                        float p16z2 = BitConverter.ToUInt16(pixelsz2, index2);
                        float p16z3 = BitConverter.ToUInt16(pixelsz3, index2);
                        color.R = (ushort)p16z1;
                        color.G = (ushort)p16z2;
                        color.B = (ushort)p16z3;
                    }
                }//unsafe
            }
            else
            {
                unsafe
                {
                    //destination image will always be 3channels(RGB) with 24bits
                    int stride2 = w * RGBChannelsCount;
                    byte* pix = (byte*)data.Scan0;
                    if (bitsPerPixel > 8)
                    {
                        //For 16bit (2*8bit) images we multiply buffer index by 2
                        index2 = (y * stride2 + (x * RGBChannelsCount)) * 2;
                        float p16z1 = BitConverter.ToUInt16(pixelsz1, index2);
                        float p16z2 = BitConverter.ToUInt16(pixelsz2, index2);
                        float p16z3 = BitConverter.ToUInt16(pixelsz3, index2);
                        color.R = (ushort)p16z1;
                        color.G = (ushort)p16z2;
                        color.B = (ushort)p16z3;
                    }
                }//unsafe
            }
            return color;
        }
        */
        public bool FromFile(string file)
        {
            this.file = file;
            // create OME-XML metadata store
            ServiceFactory factory = new ServiceFactory();
            OMEXMLService service = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            IMetadata meta = service.createOMEXMLMetadata();
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
            imageCount = reader.getImageCount();
            bool orderCertain = reader.isOrderCertain();
            SliceOrder = reader.getDimensionOrder();
            littleEndian = reader.isLittleEndian();
            
            string str = reader.getFormat();
            string[] st = reader.getDomains();
            bool complete = reader.isMetadataComplete();
            bool populated = reader.isOriginalMetadataPopulated();

            // create OME-XML metadata store
            ServiceFactory factory2 = new ServiceFactory();
            OMEXMLService service2 = (OMEXMLService)factory.getInstance(typeof(OMEXMLService));
            IMetadata meta2 = service2.createOMEXMLMetadata();

            if (!file.EndsWith(".czi"))
            {
                // create a writer that will automatically handle any supported output format
                writer = new ImageWriter();
                // give the writer a MetadataRetrieve object, which encapsulates all of the
                // dimension information for the dataset (among many other things)
                writer.setMetadataRetrieve(service2.asRetrieve(reader.getMetadataStore()));
                //string s = Path.GetFileNameWithoutExtension(file);
                //string f = s + "-temp" + Path.GetExtension(file);
                writer.setId(file);
            }
            bool isRGB = reader.isRGB();
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
            catch (Exception)
            {
                
            }
            
            int count = SizeC / RGBChannelsCount;
            for (int i = 0; i < count; i++)
            {
                Channel ch = new Channel(i);
                if (bitsPerPixel == 16)
                    ch.max = 65535;
                if (bitsPerPixel == 14)
                    ch.max = 16383;
                if (bitsPerPixel == 12)
                    ch.max = 4096;
                if (bitsPerPixel == 10)
                    ch.max = 1024;
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

                }  
                if (i == 0)
                    ch.rgb = RGB.Red;
                else
                if (i == 1)
                    ch.rgb = RGB.Green;
                else
                if (i == 2)
                    ch.rgb = RGB.Blue;
                Channels.Add(ch);
            }
            volume = new VolumeD(new Point3D(stx, sty, stz), new Point3D(six, siy, siz));

            Planes = new Plane[imageCount];
            for (int i = 0; i < imageCount; i++)
            {
                if(bitsPerPixel > 8)
                    Planes[i] = new Plane(reader, writer, i, bitsPerPixel, littleEndian, PixelFormat.Format16bppGrayScale);
                else
                    Planes[i] = new Plane(reader, writer, i, bitsPerPixel, littleEndian, PixelFormat.Format8bppIndexed);
            }
            plane = Planes[0];
            UpdatePlane(plane);
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
                return NameOrig + " " + volume.Location.ToString();
            else
                return Name + " " + volume.Location.ToString();
        }

    }
}
