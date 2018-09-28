using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.OCR;
using Emgu.CV.Structure;
using Emgu.CV.Util;

namespace IDRecognize
{
    public partial class MainForm : Form
    {
        
        public MainForm()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Tesseract _ocr = new Tesseract("", "chi_sim", OcrEngineMode.TesseractOnly);
            Image<Gray, byte> imgGray = new Image<Gray, byte>((Bitmap)pictureBox9.Image);
            _ocr.SetImage(imgGray);
            _ocr.Recognize();

            String text = _ocr.GetUTF8Text();
            this.textBox1.Text = text;

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = @"请选择图片";
            of.Filter = @"图片文件(*.jpg,*.gif,*.bmp,*.tif)|*.jpg;*.gif;*.bmp;*.tif";
            if (of.ShowDialog() == DialogResult.OK)
            {
                string file = of.FileName;
                Image img = Image.FromFile(file);
                img = Util.ResizeImage(img, 424);
                pictureBox1.Image = img;
            }
            

        }

        private void pictureBox99_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = @"请选择图片";
            of.Filter = @"图片文件(*.jpg,*.gif,*.bmp,*.tif)|*.jpg;*.gif;*.bmp;*.tif";
            if (of.ShowDialog() == DialogResult.OK)
            {
                string file = of.FileName;
                Image img = Image.FromFile(file);
                //img = Util.ResizeImage(img, 424);
                pictureBox99.Image = img;
                _currentImg.Clear();
            }
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
        /// 旋转校正
        /// </summary>
        /// <param name="imageInput"></param>
        /// <returns></returns>
        private Image<Gray, Byte> randon(Image<Gray, Byte> imageInput) //图像投影旋转法倾斜校正子函数定义
        {
            int nwidth = imageInput.Width;
            int nheight = imageInput.Height;
            int sum;
            int SumOfCha;
            int SumOfChatemp = 0;
            int[] sumhang = new int[nheight];
            Image<Gray, Byte> resultImage = imageInput;
            Image<Gray, Byte> ImrotaImage;
            //20度范围内的调整
            for (int ang = -20; ang < 20; ang = ang + 1)
            {
                ImrotaImage = imageInput.Rotate(ang, new Gray(55));
                for (int i = 0; i < nheight; i++)
                {
                    sum = 0;
                    for (int j = 0; j < nwidth; j++)
                    {
                        sum += ImrotaImage.Data[i, j, 0];
                    }
                    sumhang[i] = sum;
                }
                SumOfCha = 0;
                for (int k = 0; k < nheight - 1; k++)
                {
                    SumOfCha = SumOfCha + (Math.Abs(sumhang[k] - sumhang[k + 1]));
                }
                if (SumOfCha > SumOfChatemp)
                {
                    resultImage = ImrotaImage;
                    SumOfChatemp = SumOfCha;
                }
            }
            return resultImage;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap) this.pictureBox11.Image;
            Image<Bgr, byte> img = new Image<Bgr, byte>(bitmap);
            img = Util.ResizeImage(img.Bitmap, new Size(424, 281));   //512, 384

            RotatedRect idRect = Util.IdRotatedRect(img);
            // Util.DrawRotatedRect(idRect, img);

            RotatedRect addressRect = Util.AddressRotatedRect(idRect);
            //  Util.DrawRotatedRect(addressRect, img);

            RotatedRect dateRect = Util.DateRotatedRect(idRect);
            //  Util.DrawRotatedRect(dateRect, img);

            RotatedRect sexRect = Util.SexRotatedRect(idRect);
            //  Util.DrawRotatedRect(sexRect, img);

            RotatedRect nameRect = Util.NameRotatedRect(idRect);
            //  Util.DrawRotatedRect(nameRect, img);

            double idArea = idRect.Size.Width * idRect.Size.Height;

            Bitmap id = Util.BinImg(Util.Rote(img, idRect), 15, (int) (0.00111 * idArea)).Bitmap;
            Bitmap address = Util.BinImg(Util.Rote(img, addressRect), 15, (int) (0.00111 * idArea)).Bitmap;
            Bitmap date = Util.BinImg(Util.Rote(img, dateRect), 15, (int) (0.001111 * idArea)).Bitmap;
            Bitmap sex = Util.BinImg(Util.Rote(img, sexRect), 15, (int) (0.00111 * idArea)).Bitmap;
            Bitmap name = Util.BinImg(Util.Rote(img, nameRect), 15, (int) (0.00111 * idArea)).Bitmap;

            pictureBox3.Image = name;
            pictureBox4.Image = sex;
            pictureBox5.Image = date;
            pictureBox6.Image = address;
            pictureBox7.Image = id;

            string idText = GetNumber(GetText(id));

            dataGridView1.Rows.Clear();
            dataGridView1.Rows.Add(new object[] { "姓名", GetChinese(GetText(name)) });
            dataGridView1.Rows.Add(new object[] { "性别", GetSex(idText) });
            dataGridView1.Rows.Add(new object[] { "名族", "汉" });
            dataGridView1.Rows.Add(new object[] { "出生日期", GetDate(idText) });
            dataGridView1.Rows.Add(new object[] { "住址", GetChinese(GetText(address)) });
            dataGridView1.Rows.Add(new object[] { "身份证号", idText });

        }

        private void button3_Click(object sender, EventArgs e)
        {
            ProcessingImgDemo();
            //ProcessingImg2();
        }

        private void ProcessingImgDemo()
        {
            Bitmap bitmap = (Bitmap)this.pictureBox1.Image;
            Image<Bgr, Byte> imageSource = new Image<Bgr, byte>(bitmap);
            this.pictureBox2.Image = imageSource.ToBitmap();

            //Image<Bgr, Byte> imageThreshold3 = imageSource.ThresholdBinary(new Bgr(100, 100, 100),new Bgr(255,255,255) );
            //this.pictureBox8.Image = imageThreshold3.ToBitmap();

            //imageGrayscale = randon(imageGrayscale);

            Image<Gray, Byte> imageGrayscale = imageSource.Convert<Gray, Byte>();
            imageGrayscale = randon(imageGrayscale);

            this.pictureBox2.Image = imageGrayscale.ToBitmap();


            Image<Gray, byte> imgGray = new Image<Gray, byte>(bitmap.Size);
            CvInvoke.CvtColor(imageSource, imgGray, ColorConversion.Bgr2Gray);
            this.pictureBox8.Image = imgGray.ToBitmap();

            //imgGray.thr

            Image<Gray, Byte> imageThreshold = imgGray.ThresholdToZero(new Gray(100));
            this.pictureBox9.Image = imageThreshold.ToBitmap();


            Image<Gray, Byte> imageThreshold2 = imgGray.ThresholdToZeroInv(new Gray(100));
            this.pictureBox10.Image = imageThreshold2.ToBitmap();

            Image<Gray, Byte> imageThreshold3 = imageThreshold.ThresholdBinary(new Gray(100), new Gray(255));
            this.pictureBox11.Image = imageThreshold3.ToBitmap();

            Image<Gray, Byte> imageThreshold4 = imageThreshold2.ThresholdBinary(new Gray(100), new Gray(255));
            this.pictureBox12.Image = imageThreshold4.ToBitmap();




        }



        private void ProcessingImg()
        {
            Bitmap bitmap = (Bitmap)this.pictureBox1.Image;
            Image<Bgr, Byte> imageSource = new Image<Bgr, byte>(bitmap);
            Image<Gray, Byte> imageGrayscale = imageSource.Convert<Gray, Byte>();

            imageGrayscale = randon(imageGrayscale);

            Image<Gray, Byte>  imageThreshold = imageGrayscale.ThresholdBinary(new Gray(90), new Gray(255));
            this.pictureBox2.Image = imageThreshold.ToBitmap();


            // 灰度化

            Image<Gray, byte> imgGray = new Image<Gray, byte>(bitmap.Size);
            CvInvoke.CvtColor(imageSource, imgGray, ColorConversion.Bgr2Gray);
            this.pictureBox8.Image = imgGray.ToBitmap();


            //截断阈值化
            Image<Gray, byte> imgThresholdTrunc = new Image<Gray, byte>(imgGray.Size);
            CvInvoke.Threshold(imgGray, imgThresholdTrunc, 60, 255, ThresholdType.Trunc);
            this.pictureBox9.Image = imgThresholdTrunc.ToBitmap();

            // 中值模糊
            Image<Gray, byte> imgMedian = imgThresholdTrunc.SmoothMedian(7); //使用5*5的卷积核
            this.pictureBox10.Image = imgMedian.ToBitmap();

            // 高斯模糊
            Image<Gray, byte> imgGaussian = imgMedian.SmoothGaussian(5);
            this.pictureBox11.Image = imgGaussian.ToBitmap();

            // 膨胀，消除杂点
            Mat oMat2 = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle,
                new System.Drawing.Size(5, 5), new System.Drawing.Point(0, 0));
            Image<Gray, byte> imgErode = new Image<Gray, byte>(imgGray.Size);
            CvInvoke.Erode(imgGaussian, imgErode, oMat2, new System.Drawing.Point(0, 0), 4,
                BorderType.Default, new MCvScalar(255, 0, 0, 255));
            this.pictureBox12.Image = imgErode.ToBitmap();

            // 腐蚀，消除杂点
            Image<Gray, byte> imgDilate = new Image<Gray, byte>(imgGray.Size);
            CvInvoke.Dilate(imgErode, imgDilate, oMat2, new System.Drawing.Point(0, 0), 4,
                BorderType.Default, new MCvScalar(255, 0, 0, 255));
            this.pictureBox13.Image = imgDilate.ToBitmap();

            // Otsu二值化
            Image<Gray, byte> imgThresholdOtsu = new Image<Gray, byte>(imgGray.Size);
            CvInvoke.Threshold(imgDilate, imgThresholdOtsu, 0, 255, ThresholdType.Otsu);
            this.pictureBox14.Image = imgDilate.ToBitmap();



        }


        private void ProcessingImg2()
        {
            Bitmap bitmap = (Bitmap)this.pictureBox1.Image;
            Image<Bgr, Byte> imageSource = new Image<Bgr, byte>(bitmap);
            //Image<Gray, Byte> imageGrayscale = imageSource.Convert<Gray, Byte>();

            //imageGrayscale = randon(imageGrayscale);
            // 灰度化
            Image<Gray, byte> imgGray = new Image<Gray, byte>(bitmap.Size);
            CvInvoke.CvtColor(imageSource, imgGray, ColorConversion.Bgr2Gray);
            this.pictureBox15.Image = imgGray.ToBitmap();


            //截断阈值化
            Image<Gray, byte> imgThresholdTrunc = new Image<Gray, byte>(imgGray.Size);
            CvInvoke.Threshold(imgGray, imgThresholdTrunc, 60, 255, ThresholdType.Trunc);
            this.pictureBox16.Image = imgThresholdTrunc.ToBitmap();


            // 消除裂缝
            Mat oMat1 = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle,
                new System.Drawing.Size(6, 6), new System.Drawing.Point(0, 0));
            Image<Gray, byte> imgMorphologyEx = new Image<Gray, byte>(imgGray.Size);
            CvInvoke.MorphologyEx(imgThresholdTrunc, imgMorphologyEx, Emgu.CV.CvEnum.MorphOp.Close, oMat1,
                new System.Drawing.Point(0, 0), 1, BorderType.Default,
                new MCvScalar(255, 0, 0, 255));
            this.pictureBox17.Image = imgMorphologyEx.ToBitmap();


            // Otsu二值化
            Image<Gray, byte> imgThresholdOtsu = new Image<Gray, byte>(imgGray.Size);
            CvInvoke.Threshold(imgMorphologyEx, imgThresholdOtsu, 0, 255, ThresholdType.Otsu);

            List<RotatedRect> boxList = new List<RotatedRect>(); //a box is a rotated rectangle
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            CvInvoke.FindContours(imgThresholdOtsu, contours, null, RetrType.List,
                ChainApproxMethod.ChainApproxSimple);

            Image<Bgr, byte> imgResult = new Image<Bgr, byte>(imgGray.Size);
            CvInvoke.CvtColor(imgThresholdOtsu, imgResult, ColorConversion.Gray2Bgr);
            MCvScalar oScaler = new MCvScalar(40, 255, 255, 255);
            int count = contours.Size;
            for (int i = 0; i < count; i++)
            {
                using (VectorOfPoint contour = contours[i])
                using (VectorOfPoint approxContour = new VectorOfPoint())
                {
                    CvInvoke.ApproxPolyDP(contour, approxContour, CvInvoke.ArcLength(contour, true) * 0.05, true);
                    double dArea = CvInvoke.ContourArea(approxContour, false);
                    if (dArea > imgThresholdOtsu.Rows * imgThresholdOtsu.Cols / 3d)
                    {
                        if (approxContour.Size == 4)
                        {
                            #region determine if all the angles in the contour are within [80, 100] degree
                            bool isRectangle = true;
                            System.Drawing.Point[] pts = approxContour.ToArray();
                            LineSegment2D[] edges = Emgu.CV.PointCollection.PolyLine(pts, true);

                            for (int j = 0; j < edges.Length; j++)
                            {
                                double angle = Math.Abs(
                                    edges[(j + 1) % edges.Length].GetExteriorAngleDegree(edges[j]));
                                if (angle < 80 || angle > 100)
                                {
                                    isRectangle = false;
                                    break;
                                }
                            }
                            #endregion

                            if (isRectangle)
                                CvInvoke.DrawContours(imgResult, contours, i, oScaler, 3);
                        }
                    }
                }
            }
            this.pictureBox18.Image = imgResult.ToBitmap();


        }

        //示例代码
        //1.灰度化，竖向边缘检测
        //2.自适应二值化处理
        //3.形态学处理（膨胀和腐蚀）
        //4.轮廓查找与筛选
        void ProcessingImg3()
        {
            //Image<Bgr, Byte> simage = img;    //new Image<Bgr, byte>("license-plate.jpg");
            ////Image<Bgr, Byte> simage = sizeimage.Resize(400, 300, Emgu.CV.CvEnum.INTER.CV_INTER_NN);
            //Image<Gray, Byte> GrayImg = new Image<Gray, Byte>(simage.Width, simage.Height);
            //IntPtr GrayImg1 = CvInvoke.cvCreateImage(simage.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_8U, 1);
            ////灰度化
            //CvInvoke.CvtColor(simage.Ptr, GrayImg1, Emgu.CV.CvEnum.COLOR_CONVERSION.BGR2GRAY);
            ////首先创建一张16深度有符号的图像区域
            //IntPtr Sobel = CvInvoke.cvCreateImage(simage.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_16S, 1);
            ////X方向的Sobel算子检测
            //CvInvoke.cvSobel(GrayImg1, Sobel, 2, 0, 3);
            //IntPtr temp = CvInvoke.cvCreateImage(simage.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_8U, 1);
            //CvInvoke.cvConvertScale(Sobel, temp, 0.00390625, 0);
            //////int it = ComputeThresholdValue(GrayImg.ToBitmap());
            //////二值化处理
            //////Image<Gray, Byte> dest = GrayImg.ThresholdBinary(new Gray(it), new Gray(255));
            //Image<Gray, Byte> dest = new Image<Gray, Byte>(simage.Width, simage.Height);
            ////二值化处理
            //CvInvoke.cvThreshold(temp, dest, 0, 255, Emgu.CV.CvEnum.THRESH.CV_THRESH_OTSU);
            //IntPtr temp1 = CvInvoke.cvCreateImage(simage.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_8U, 1);
            //Image<Gray, Byte> dest1 = new Image<Gray, Byte>(simage.Width, simage.Height);
            //CvInvoke.cvCreateStructuringElementEx(3, 1, 1, 0, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT, temp1);
            //CvInvoke.Dilate(dest, dest1, temp1, 6);
            //CvInvoke.cvErode(dest1, dest1, temp1, 7);
            //CvInvoke.cvDilate(dest1, dest1, temp1, 1);
            //CvInvoke.cvCreateStructuringElementEx(1, 3, 0, 1, Emgu.CV.CvEnum.CV_ELEMENT_SHAPE.CV_SHAPE_RECT, temp1);
            //CvInvoke.cvErode(dest1, dest1, temp1, 2);
            //CvInvoke.cvDilate(dest1, dest1, temp1, 2);
            //IntPtr dst = CvInvoke.cvCreateImage(simage.Size, Emgu.CV.CvEnum.IPL_DEPTH.IPL_DEPTH_8U, 3);
            //CvInvoke.cvZero(dst);
            ////dest.Dilate(10);
            ////dest.Erode(5);
            //using (MemStorage stor = new MemStorage())
            //{
            //    Contour<Point> contours = dest1.FindContours(
            //        Emgu.CV.CvEnum.CHAIN_APPROX_METHOD.CV_CHAIN_APPROX_SIMPLE,
            //        Emgu.CV.CvEnum.RETR_TYPE.CV_RETR_CCOMP,
            //        stor);
            //    for (; contours != null; contours = contours.HNext)
            //    {

            //        Rectangle box = contours.BoundingRectangle;
            //        Image<Bgr, Byte> test = simage.CopyBlank();
            //        test.SetValue(255.0);
            //        double whRatio = (double)box.Width / box.Height;
            //        int area = (int)box.Width * box.Height;
            //        if (area > 1000 && area < 10000)
            //        {
            //            if ((3.0 < whRatio && whRatio < 6.0))
            //            {
            //                test.Draw(box, new Bgr(Color.Red), 2);
            //                simage.Draw(box, new Bgr(Color.Red), 2);//CvInvoke.cvNamedWindow("dst");
            //                //CvInvoke.cvShowImage("dst", dst);
            //                imageBox1.Image = simage;
            //            }
            //        }
            //    }
            //}
        }

        #region   方法

        private string GetText(Bitmap img)
        {

            using (Tesseract _ocr = new Tesseract("", "chi_sim", OcrEngineMode.TesseractOnly))
            {
                Image<Bgr, Byte> imageSource = new Image<Bgr, byte>(img);

                _ocr.SetImage(imageSource);
                _ocr.Recognize();

                String text = _ocr.GetUTF8Text();
                return text;
            }
        }

        public string GetDate(string idStr)
        {
            // str = GetNumber(str);
            try
            {
                string year = idStr.Substring(6, 4);
                string month = idStr.Substring(10, 2);
                string day = idStr.Substring(12, 2);
                return year + "年" + month + "月" + day + "日";
            }
            catch { return null; }
        }

        public string GetSex(string idStr)
        {
            if (Convert.ToInt32(idStr.ToCharArray()[idStr.Length - 2]) % 2 == 0)
                return "女";


            else return "男";
        }
        public string GetNumber(string str)
        {
            Regex r = new Regex("X|x|\\d+\\.?\\d*");
            bool ismatch = r.IsMatch(str);
            MatchCollection mc = r.Matches(str);

            string result = string.Empty;
            for (int i = 0; i < mc.Count; i++)
            {
                result += mc[i];
            }
            return result;
        }

        public string GetChinese(string str)
        {
            Regex r = new Regex("[\u4e00-\u9fa5]");
            bool ismatch = r.IsMatch(str);
            MatchCollection mc = r.Matches(str);

            string result = string.Empty;
            for (int i = 0; i < mc.Count; i++)
            {
                result += mc[i];
            }
            return result;
        }

        /// <summary>
        /// 身份证号验证
        /// </summary>
        /// <param name="idNumber"></param>
        /// <returns></returns>
        private bool CheckIDCard(string idNumber)
        {
            long n = 0;
            if (long.TryParse(idNumber.Remove(17), out n) == false
            || n < Math.Pow(10, 16) || long.TryParse(idNumber.Replace('x', '0').Replace('X', '0'), out n) == false)
            {
                return false;//数字验证  
            }
            string address = "11x22x35x44x53x12x23x36x45x54x13x31x37x46x61x14x32x41x50x62x15x33x42x51x63x21x34x43x52x64x65x71x81x82x91";
            if (address.IndexOf(idNumber.Remove(2)) == -1)
            {
                return false;//省份验证  
            }
            string birth = idNumber.Substring(6, 8).Insert(6, "-").Insert(4, "-");
            DateTime time = new DateTime();
            if (DateTime.TryParse(birth, out time) == false)
            {
                return false;//生日验证  
            }
            string[] arrVarifyCode = ("1,0,x,9,8,7,6,5,4,3,2").Split(',');
            string[] Wi = ("7,9,10,5,8,4,2,1,6,3,7,9,10,5,8,4,2").Split(',');
            char[] Ai = idNumber.Remove(17).ToCharArray();
            int sum = 0;
            for (int i = 0; i < 17; i++)
            {
                sum += int.Parse(Wi[i]) * int.Parse(Ai[i].ToString());
            }
            int y = -1;
            Math.DivRem(sum, 11, out y);
            if (arrVarifyCode[y] != idNumber.Substring(17, 1).ToLower())
            {
                return false;//校验码验证  
            }
            return true;
        }

        void CreatePicBoxInPanel(Bitmap img)
        {
            PictureBox pb = new PictureBox
            {
                BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
            };
            pb.SizeMode = PictureBoxSizeMode.AutoSize;
            this.flowLayoutPanel2.Controls.Add(pb);
            pb.Image = img;
        }


        #endregion

        private void button4_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox1.Image;
            //Image<Hsv, Byte> imageSource = new Image<Hsv, Byte>(bitmap);
            Image<Bgr, Byte> imageSource = new Image<Bgr, byte>(bitmap);
            var _l = imageSource.Split();
            foreach (var image in _l)
            {
                CreatePicBoxInPanel(image.Bitmap);
            }
        }

        #region tab 1

        private Stack<Image<Gray, Byte>> _currentImg = new Stack<Image<Gray, Byte>>();

        /// <summary>
        /// 返回上一步操作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button10_Click(object sender, EventArgs e)
        {
            //Image<Gray, Byte> imageSource = new Image<Gray, byte>(_currentImg);
            if (_currentImg.Count == 1)
            {
                this.pictureBox99.Image = _currentImg.Last().ToBitmap();
                return;
            }
            if (_currentImg.Count >0 )
            {
                this.pictureBox99.Image = _currentImg.Pop().ToBitmap();
            }
            
        }

        void setImages(Image<Gray, Byte> imageSource)
        {
            this.pictureBox99.Image = imageSource.ToBitmap();

            //_currentImg = new Image<Gray, Byte>(imageSource.Size);
            _currentImg.Push(imageSource.Copy());
        }

        private void button7_Click(object sender, EventArgs e)
        {
            Tesseract _ocr = new Tesseract("", "chi_sim", OcrEngineMode.TesseractOnly);
            Image<Gray, byte> imgGray = new Image<Gray, byte>((Bitmap)pictureBox99.Image);
            _ocr.SetImage(imgGray);
            _ocr.Recognize();

            String text = _ocr.GetUTF8Text();
            this.textBox2.Text = text;
        }

        /// <summary>
        /// tab1 灰化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox99.Image;
            Image<Gray, Byte> imageSource = new Image<Gray, byte>(bitmap);
            imageSource = randon(imageSource);
            setImages(imageSource);
            

        }

        /// <summary>
        /// tab 1 二值化
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button8_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox99.Image;
            Image<Gray, Byte> imageSource = new Image<Gray, byte>(bitmap);
            Image<Gray, Byte> imageThreshold = imageSource.ThresholdBinary(new Gray(100),new Gray(255));
            setImages(imageThreshold);
        }

        /// <summary>
        /// tab1 滤噪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button9_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox99.Image;
            Image<Gray, Byte> imageSource = new Image<Gray, byte>(bitmap);
            imageSource = imageSource.PyrDown();
            setImages(imageSource);
        }
        

        /// <summary>
        /// tab1 滤噪
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button11_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox99.Image;
            Image<Gray, Byte> imageSource = new Image<Gray, byte>(bitmap);
            imageSource = imageSource.PyrUp();
            setImages(imageSource);
        }

        /// <summary>
        /// 腐蚀
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button12_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox99.Image;
            Image<Gray, Byte> imageSource = new Image<Gray, byte>(bitmap);
            imageSource = imageSource.Erode(9);
            setImages(imageSource);
            imageSource.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.MeanC, ThresholdType.Binary, 3,new Gray());
        }
        /// <summary>
        /// 自适应阈值
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button13_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox99.Image;
            Image<Gray, Byte> imageSource = new Image<Gray, byte>(bitmap);
            imageSource = imageSource.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.MeanC, ThresholdType.Binary, 3, new Gray(160));
            setImages(imageSource);
        }
        /// <summary>
        /// 去除噪点
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button14_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox99.Image;
            Image<Gray, Byte> imageSource = new Image<Gray, byte>(bitmap);
            int _parm = Convert.ToInt32(textBox3.Text.Trim());
            imageSource = imageSource.SmoothMedian(_parm);  //参数为7去噪，然后腐蚀
            //imageSource = imageSource.SmoothGaussian(_parm);  //高斯模糊
              
            setImages(imageSource);
        }


        #endregion


        /// <summary>
        /// 轮廓描边
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button15_Click(object sender, EventArgs e)
        {
            Bitmap bitmap = (Bitmap)this.pictureBox99.Image;
            Image<Gray, Byte> imageSource = new Image<Gray, byte>(bitmap);
            imageSource=imageSource.Canny(100, 500);
            setImages(imageSource);

            
        }

        #region 轮廓描边

        /// <summary>
        /// 获取轮廓
        /// </summary>
        /// <param name="pic"></param>
        /// <returns></returns>
        //private static VectorOfVectorOfPoint GetContours(Image<Gray, byte> pic)
        //{
        //    Image<Gray, byte> p1 = new Image<Gray, byte>(pic.Size);
        //    Image<Gray, byte> p2 = pic.Canny(60, 255);
        //    VectorOfVectorOfPoint contours =  new VectorOfVectorOfPoint();
        //    CvInvoke.FindContours(p2, contours, p1, RetrType.Ccomp, ChainApproxMethod.ChainApproxSimple);
        //    return contours;
        //}

        ////筛选矩形
        //public static RotatedRect RotatedRect(Image<Bgr, byte> img)
        //{
        //    Image<Bgr, byte> a = new Image<Bgr, byte>(img.Size);
        //    VectorOfVectorOfPoint con = GetContours(BinImg(img,3,100));
        //    Point[][] con1 = con.ToArrayOfArray();
        //    PointF[][] con2 = Array.ConvertAll<Point[], PointF[]>(con1, new Converter<Point[], PointF[]>(PointToPointF));
        //    for (int i = 0; i < con.Size; i++)
        //    {
        //        RotatedRect rrec = CvInvoke.MinAreaRect(con2[i]);
        //        float w = rrec.Size.Width;
        //        float h = rrec.Size.Height;
        //        if (w / h > 6 && w / h < 10 && h > 20)
        //        {

        //            PointF[] pointfs = rrec.GetVertices();
        //            for (int j = 0; j < pointfs.Length; j++)
        //                CvInvoke.Line(a, new Point((int)pointfs[j].X, (int)pointfs[j].Y), new Point((int)pointfs[(j + 1) % 4].X, (int)pointfs[(j + 1) % 4].Y), new MCvScalar(0, 0, 255, 255), 4);
        //            return rrec;
        //        }

        //    }
        //    return new RotatedRect();
        //}

        ////绘制矩形
        //public static void DrawRotatedRect(RotatedRect rrec, Image<Bgr, byte> img)
        //{
        //    PointF[] pointfs = rrec.GetVertices();
        //    for (int j = 0; j < pointfs.Length; j++)
        //        CvInvoke.Line(img, new Point((int)pointfs[j].X, (int)pointfs[j].Y), new Point((int)pointfs[(j + 1) % 4].X, (int)pointfs[(j + 1) % 4].Y), new MCvScalar(0, 0, 255, 255), 4);
        //}

        #endregion


    }
}
