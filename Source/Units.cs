using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Globalization;

namespace Bio
{    
    public struct Point2D
    {
        private static double minX = -80000;
        private static double minY = -58000;
        private static double maxX = 80000;
        private static double maxY = 58000;

        public static double MinX
        {
            get
            {
                return minX;
            }
        }
        public static double MinY
        {
            get
            {
                return minY;
            }
        }
        public static double MaxX
        {
            get
            {
                return maxX;
            }
        }
        public static double MaxY
        {
            get
            {
                return maxY;
            }
        }

        public static Point2D Min
        {
            get
            {
                return new Point2D(MinX, MinY);
            }
        }

        public static Point2D Max
        {
            get
            {
                return new Point2D(MaxX, MaxY);
            }
        }

        public static bool InLimits(Point2D p)
        {
            if (p.X < minX || p.X > maxX)
            {
                return false;
            }
            if (p.X < minY || p.X > maxY)
            {
                return false;
            }
            return true;
        }

        public static bool InLimitX(double px)
        {
            if (px < minX || px > maxX)
            {
                return false;
            }
            return true;
        }

        public static bool InLimitY(double py)
        {
            if (py < minY || py > maxY)
            {
                return false;
            }
            return true;
        }

        private double x, y;
        public static void SetLimits(double pminX, double pmaxX, double pminY, double pmaxY)
        {
            minX = pminX;
            minY = pminY;

            maxX = pmaxX;
            maxY = pmaxY;
        }

        public double X
        {
            set
            {
                if (value < minX || value > maxX)
                {
                    throw new Exception("Point X coordinate " + value.ToString() + " is outside x-Axis range");
                }
                x = value;
            }
            get
            {
                return x;
            }
        }
        public double Y
        {
            set
            {
                if (value < minY || value > maxY)
                {
                    throw new Exception("Point Y coordinate " + value.ToString() + " is outside y-Axis range");
                }
                y = value;
            }
            get
            {
                return y;
            }
        }

        public Point2D(double xx, double yy)
        {
            x = xx;
            y = yy;
        }

        public static Point2D Parse(string s)
        {
            string[] st = s.Split(',');
            double xd = double.Parse(st[0], CultureInfo.InvariantCulture);
            double yd = double.Parse(st[1], CultureInfo.InvariantCulture);
            return new Point2D(xd, yd);
        }

        public override string ToString()
        {
            return X.ToString() + "," + Y.ToString();
        }
    }

    public struct Point3D
    {
        private static double minX = -80000;
        private static double minY = -58000;
        private static double maxX = 80000;
        private static double maxY = 58000;
        private static double maxZ = 25000;
        private static double minZ = -25000;
        public static string File = "";
        public static void SetLimits(double pminX,double pmaxX,double pminY,double pmaxY,double pminZ,double pmaxZ)
        {
            minX = pminX;
            minY = pminY;
            minZ = pminZ;

            maxX = pmaxX;
            maxY = pmaxY;
            maxZ = pmaxZ;
        }

        public static double MinX
        {
            get
            {
                return minX;
            }
        }
        public static double MinY
        {
            get
            {
                return minY;
            }
        }
        public static double MinZ
        {
            get
            {
                return minZ;
            }
        }

        public static double MaxX
        {
            get
            {
                return maxX;
            }
        }
        public static double MaxY
        {
            get
            {
                return maxY;
            }
        }
        public static double MaxZ
        {
            get
            {
                return maxZ;
            }
        }

        private double x, y, z;
        public double X
        {
            set
            {
                if(value < minX || value > maxX)
                {
                    throw new Exception("Point X coordinate " + value.ToString() + " is outside x-Axis range");
                }
                x = value;
            }
            get
            {
                return x;
            }
        }
        public double Y
        {
            set
            {
                if (value < minY || value > maxY)
                {
                    throw new Exception("Point Y coordinate " + value.ToString() + " is outside y-Axis range");
                }
                y = value;
            }
            get
            {
                return y;
            }
        }
        public double Z
        {
            set
            {
                if (value < minZ || value > maxZ)
                {
                    throw new Exception("Point Z coordinate " + value.ToString() + " is outside Basic Focus range");
                }
                z = value;
            }
            get
            {
                return z;
            }
        }

        public static bool InLimits(Point3D p)
        {
            if (p.X < minX || p.X > maxX)
            {
                return false;
            }
            if (p.X < minY || p.X > maxY)
            {
                return false;
            }
            if (p.X < minZ || p.X > maxZ)
            {
                return false;
            }
            return true;
        }

        public static bool InLimitX(double px)
        {
            if (px < minX || px > maxX)
            {
                return false;
            }
            return true;
        }

        public static bool InLimitY(double py)
        {
            if (py < minY || py > maxY)
            {
                return false;
            }
            return true;
        }

        public static bool InLimitZ(double pz)
        {
            if (pz < minY || pz > maxZ)
            {
                return false;
            }
            return true;
        }

        public Point3D(double xd, double yd,double zd)
        {
            x = xd;
            y = yd;
            z = zd;
        }

        public Point3D(double xd, double yd, double zd,string f)
        {
            x = xd;
            y = yd;
            z = zd;
            File = f;
        }

        public static Point3D Parse(string s)
        {
            string[] st = s.Split(',');
            double xd = double.Parse(st[0], CultureInfo.InvariantCulture);
            double yd = double.Parse(st[1], CultureInfo.InvariantCulture);
            double zd = double.Parse(st[2], CultureInfo.InvariantCulture);
            return new Point3D(xd, yd, zd);
        }

        public override string ToString()
        {
            if (File == "")
                return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString();
            else
                return X.ToString() + ", " + Y.ToString() + ", " + Z.ToString() + ", " + File;
        }

        public static double Distance(Point3D p0,Point3D p1)
        {
            double deltaX = p1.X - p0.X;
            double deltaY = p1.Y - p0.Y;
            double deltaZ = p1.Z - p0.Z;
            float distance = (float)Math.Sqrt(deltaX * deltaX + deltaY * deltaY + deltaZ * deltaZ);
            return distance;
        }

    }

    public struct VolumeD
    {
        public VolumeD(Point3D loc, Point3D s)
        {
            location = loc;
            size = s;
        }

        private Point3D location;
        public Point3D Location
        {
            get
            {
                return location;
            }
            set
            {
                location = value;
            }
        }

        private Point3D size;
        public Point3D Size
        {
            get
            {
                return size;
            }
            set
            {
                size = value;
            }
        }
        public double Width
        {
            get
            {
                return size.X;
            }
            set
            {
                size.X = value;
            }
        }
        public double Height
        {
            get
            {
                return size.Y;
            }
            set
            {
                size.Y = value;
            }
        }
        public double Depth
        {
            get
            {
                return size.Z;
            }
            set
            {
                size.Z = value;
            }
        }

        public bool Intersects(PointF p)
        {
            if ((p.X > Location.X && p.X < (Location.X + Width)) && (p.Y > Location.Y && p.Y < (Location.Y + Height)))
            {
                return true;
            }
            else
                return false;
        }
        public bool Intersects(Point2D p)
        {
            if ((p.X > Location.X && p.X < (Location.X + Width)) && (p.Y > Location.Y && p.Y < (Location.Y + Height)))
            {
                return true;
            }
            else
                return false;
        }
        public bool Intersects(Point3D p)
        {
            if ((p.X > Location.X && p.X < (Location.X + Width)) && (p.Y > Location.Y && p.Y < (Location.Y + Height)) && (p.Z > Location.Z && p.Z < (Location.Z + Depth)))
            {
                return true;
            }
            else
                return false;
        }

    }

}
