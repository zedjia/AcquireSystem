using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace IDRecognize
{
    public class Util
    {
        public static Image<Gray, byte> BinImg(Image<Bgr, byte> img)
        {

            return img.Convert<Gray, byte>().ThresholdAdaptive(new Gray(255),
                AdaptiveThresholdType.GaussianC,
                ThresholdType.Binary,
                55,
                new Gray(55)).Erode(9);
        }
        public static Image<Gray, byte> BinImg(Image<Bgr, byte> img, int blockSize, int val)
        {
 
            return img.Convert<Gray, byte>().SmoothGaussian(5).ThresholdAdaptive(new Gray(255),
                AdaptiveThresholdType.GaussianC,
                ThresholdType.Binary,
                blockSize,
                new Gray(val));
        }
        /// <summary>
        /// 身份证号区域
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static RotatedRect IdRotatedRect(Image<Bgr, byte> img)
        {
            Image<Bgr, byte> a = new Image<Bgr, byte>(img.Size);
            VectorOfVectorOfPoint con = GetContours(BinImg(img));
            Point[][] con1 = con.ToArrayOfArray();
            PointF[][] con2 = Array.ConvertAll<Point[], PointF[]>(con1, new Converter<Point[], PointF[]>(PointToPointF));
            for (int i = 0; i < con.Size; i++)
            {
                RotatedRect rrec = CvInvoke.MinAreaRect(con2[i]);
                float w = rrec.Size.Width;
                float h = rrec.Size.Height;
                if (w / h > 6 && w / h < 10 && h > 20)
                {

                    PointF[] pointfs = rrec.GetVertices();
                    for (int j = 0; j < pointfs.Length; j++)
                    {
                        CvInvoke.Line(a, new Point((int)pointfs[j].X, (int)pointfs[j].Y), new Point((int)pointfs[(j + 1) % 4].X, (int)pointfs[(j + 1) % 4].Y), new MCvScalar(0, 0, 255, 255), 4);

                    }
                    return rrec;
                }
            }
            return new RotatedRect();
        }
        /// <summary>
        /// 地址区域
        /// </summary>
        /// <param name="rr"></param>
        /// <returns></returns>
        public static RotatedRect AddressRotatedRect(RotatedRect rr)
        {
            float w = (float)((rr.Size.Width * 0.8));
            float h = (float)(rr.Size.Height * 1.7);
            float px = (float)(rr.Center.X - rr.Size.Height * 3.1);
            float py = (float)(rr.Center.Y - rr.Size.Height * 2.4);
            PointF center = new PointF(px, py);
            RotatedRect rect = new RotatedRect(center, new SizeF(w, h), rr.Angle);
            return rect;
        }
        /// <summary>
        /// 年份区域
        /// </summary>
        /// <param name="rr"></param>
        /// <returns></returns>
        public static RotatedRect DateRotatedRect(RotatedRect rr)
        {
            float w = (float)(rr.Size.Width * 0.7);
            float h = (float)(rr.Size.Height * 1);
            float px = (float)(rr.Center.X - rr.Size.Height * 3.7);
            float py = (float)(rr.Center.Y - rr.Size.Height * 4);
            PointF center = new PointF(px, py);
            RotatedRect rect = new RotatedRect(center, new SizeF(w, h), rr.Angle);
            return rect;
        }
        /// <summary>
        /// 性别区域
        /// </summary>
        /// <param name="rr"></param>
        /// <returns></returns>
        public static RotatedRect SexRotatedRect(RotatedRect rr)
        {
            float w = (float)(rr.Size.Width * 0.7);
            float h = (float)(rr.Size.Height * 1);
            float px = (float)(rr.Center.X - rr.Size.Height * 3.7);
            float py = (float)(rr.Center.Y - rr.Size.Height * 5);
            PointF center = new PointF(px, py);
            RotatedRect rect = new RotatedRect(center, new SizeF(w, h), rr.Angle);
            return rect;
        }
        /// <summary>
        /// 姓名区域
        /// </summary>
        /// <param name="rr"></param>
        /// <returns></returns>
        public static RotatedRect NameRotatedRect(RotatedRect rr)
        {

            float w = (float)(rr.Size.Width * 0.3);
            float h = (float)(rr.Size.Height * 1);
            float px = (float)(rr.Center.X - rr.Size.Height * 5.1);
            float py = (float)(rr.Center.Y - rr.Size.Height * 6.3);
            PointF center = new PointF(px, py);
            RotatedRect rect = new RotatedRect(center, new SizeF(w, h), rr.Angle);
            return rect;
        }
        /// <summary>
        /// 绘制矩形区域
        /// </summary>
        /// <param name="rrec"></param>
        /// <param name="img"></param>
        public static void DrawRotatedRect(RotatedRect rrec, Image<Bgr, byte> img)
        {
            PointF[] pointfs = rrec.GetVertices();
            for (int j = 0; j < pointfs.Length; j++)
            {
                CvInvoke.Line(img, new Point((int)pointfs[j].X, (int)pointfs[j].Y), new Point((int)pointfs[(j + 1) % 4].X, (int)pointfs[(j + 1) % 4].Y), new MCvScalar(0, 0, 255, 255), 4);
                img.Draw(new CircleF(pointfs[j], 5), new Bgr(Color.Blue), 5);
            }
        }

        public static Image<Bgr, byte> Rote(Image<Bgr, byte> img, RotatedRect rect)
        {
            PointF center = new PointF();
            PointF[] pointfs = RectCode(rect);
            if (rect.Angle < 0)
            {
                center = pointfs[0];
            }
            if (rect.Angle >= 0)
            {
                center = pointfs[3];
            }
            Image<Bgr, byte> output = new Image<Bgr, byte>(new Size((int)rect.Size.Width, (int)rect.Size.Height));
            int w = (int)rect.Size.Width;
            int h = (int)rect.Size.Height;
            for (int i = (int)center.X, m = 0; i < w + (int)center.X; i++, m++)
            {
                for (int j = (int)center.Y, n = 0; j < h + (int)center.Y; j++, n++)
                {
                    {
                        Point p = PointRotate(center, new PointF(i, j), -rect.Angle);
                        if (p.X >= img.Size.Width)
                            p.X = img.Size.Width - 1;
                        if (p.Y >= img.Size.Height)
                            p.Y = img.Size.Height - 1;
                        output[n, m] = img[p.Y, p.X];
                    }
                }
            }
            if (Math.Abs(rect.Angle) > 45)
            {
                output = output.Rotate(180, new Bgr(Color.White));
            }
            return output;
        }
        /// <summary>
        /// 矩形顶点编号
        /// </summary>
        /// <param name="pointfs"></param>
        /// <param name="angle"></param>
        /// <returns></returns>
        public static PointF[] RectCode(RotatedRect rect)
        {

            PointF[] p = rect.GetVertices();
            PointF[] pointfs = new PointF[4];
            pointfs[0] = p[0];
            pointfs[1] = p[0];
            pointfs[2] = p[0];
            pointfs[3] = p[0];
            //逆时针编号
            for (int i = 1; i < 4; i++)
            {
                if (p[i].X < p[i - 1].X)
                    pointfs[0] = p[i];
                if (p[i].Y > p[i - 1].Y)
                    pointfs[1] = p[i];
                if (p[i].X > p[i - 1].X)
                    pointfs[2] = p[i];
                if (p[i].Y < p[i - 1].Y)
                    pointfs[3] = p[i];
            }

            return pointfs;
        }

        public static Point PointRotate(PointF center, PointF p1, double angle)
        {
            Point tmp = new Point();
            double angleHude = angle * Math.PI / 180;/*角度变成弧度*/
            double x1 = (p1.X - center.X) * Math.Cos(angleHude) + (p1.Y - center.Y) * Math.Sin(angleHude) + center.X;
            double y1 = -(p1.X - center.X) * Math.Sin(angleHude) + (p1.Y - center.Y) * Math.Cos(angleHude) + center.Y;
            tmp.X = (int)x1;
            tmp.Y = (int)y1;
            return tmp;
        }
        public static void BubbleSort(float[] data)
        {
            for (int i = 0; i < data.Length - 1; i++)
            {
                for (int j = data.Length - 1; j > i; j--)
                {
                    if (data[j] < data[j - 1])
                    {
                        data[j] = data[j] + data[j - 1];
                        data[j - 1] = data[j] - data[j - 1];
                        data[j] = data[j] - data[j - 1];
                    }
                }
            }
        }
        public static bool IsInside(Point inputponint, PointF[] pointfs)
        {
            GraphicsPath myGraphicsPath = new GraphicsPath();
            Region myRegion = new Region();
            myGraphicsPath.Reset();
            myGraphicsPath.AddPolygon(pointfs);
            myRegion.MakeEmpty();
            myRegion.Union(myGraphicsPath);

            return myRegion.IsVisible(inputponint);
        }

        /// <summary>
        /// 获取轮廓
        /// </summary>
        /// <param name="pic"></param>
        /// <returns></returns>
        private static VectorOfVectorOfPoint GetContours(Image<Gray, byte> pic)
        {
            Image<Gray, byte> p1 = new Image<Gray, byte>(pic.Size);
            Image<Gray, byte> p2 = pic.Canny(60, 255);
            VectorOfVectorOfPoint contours = contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(p2, contours, p1, RetrType.Ccomp, ChainApproxMethod.ChainApproxSimple);
            return contours;
        }

        private static PointF[] PointToPointF(Point[] pf)
        {
            PointF[] aaa = new PointF[pf.Length];
            int num = 0;
            foreach (var point in pf)
            {
                aaa[num].X = (int)point.X;
                aaa[num++].Y = (int)point.Y;
            }
            return aaa;
        }

        /// <summary>
        /// 规范图片大小
        /// </summary>
        /// <param name="imgToResize"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        public static Image<Bgr, byte> ResizeImage(Image imgToResize, Size size)
        {
            //获取图片宽度
            int sourceWidth = imgToResize.Width;
            //获取图片高度
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //计算宽度的缩放比例
            nPercentW = ((float)size.Width / (float)sourceWidth);
            //计算高度的缩放比例
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;
            //期望的宽度
            int destWidth = (int)(sourceWidth * nPercent);
            //期望的高度
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return new Image<Bgr, byte>(b);
        }

        public static Image ResizeImage(Image imgToResize, float width)
        {
            //获取图片宽度
            int sourceWidth = imgToResize.Width;
            //获取图片高度
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;
            //计算宽度的缩放比例
            nPercent = ((float)width / (float)sourceWidth);
            //计算高度的缩放比例
            //nPercentH = ((float)size.Height / (float)sourceHeight);

            //if (nPercentH < nPercentW)
            //    nPercent = nPercentH;
            //else
            //    nPercent = nPercentW;
            //期望的宽度
            int destWidth = (int)(sourceWidth * nPercent);
            //期望的高度
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            //绘制图像
            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();
            return b;
        }


    }
}
