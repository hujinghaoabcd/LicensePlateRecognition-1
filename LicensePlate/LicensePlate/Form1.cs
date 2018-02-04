using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.UI;
using Emgu.CV.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LicensePlate
{
    public partial class Form1 : Form
    {
        Image<Bgr, byte> inputImage;

        public Form1()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog fdlg = new OpenFileDialog
            {
                Title = "C# Corner Open File Dialog",
                Filter = "All files (*.*)|*.*|All files (*.*)|*.*",
                FilterIndex = 2,
                RestoreDirectory = true
            };

            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                tableLayoutPanel3.Controls.Clear();
                inputImage = new Image<Bgr, byte>(fdlg.FileName);
                imageBox1.Image = inputImage;
                FindLicensePlate(inputImage);
            }


        }


        private void FindLicensePlate(Image<Bgr, byte> image)
        {
            Image<Gray, byte> sobel = image.Convert<Gray, byte>().Sobel(1, 0, 3).AbsDiff(new Gray(0.0)).Convert<Gray, byte>();

            Image<Gray, byte> binImg = sobel.ThresholdBinary(new Gray(50), new Gray(255));
            //Image<Gray, byte> binImg = sobel.ThresholdAdaptive(new Gray(255), AdaptiveThresholdType.MeanC, ThresholdType.Binary, 7, new Gray(0));

            Mat structuringElem = CvInvoke.GetStructuringElement(Emgu.CV.CvEnum.ElementShape.Rectangle, new Size(10, 2), new Point(-1, -1));
            binImg = binImg.MorphologyEx(Emgu.CV.CvEnum.MorphOp.Dilate, structuringElem, new Point(-1, -1),1, Emgu.CV.CvEnum.BorderType.Reflect, new MCvScalar(255));
            
            Emgu.CV.Util.VectorOfVectorOfPoint contours = new Emgu.CV.Util.VectorOfVectorOfPoint();
            Mat m = new Mat();

            CvInvoke.FindContours(binImg, contours, m, Emgu.CV.CvEnum.RetrType.External, Emgu.CV.CvEnum.ChainApproxMethod.ChainApproxSimple);



            for (int i = 0; i < contours.Size; i++)
            {
                Rectangle rectangle = CvInvoke.BoundingRectangle(contours[i]);
                double area = CvInvoke.ContourArea(contours[i]);
                double ratio = rectangle.Width / rectangle.Height;

                if (ratio > 2.5 && ratio<6 && area > 2000 && area < 4500)
                {
                    CvInvoke.Rectangle(image, rectangle, new MCvScalar(255,0,0), 3);
                    Image<Bgr, byte> outputImage = inputImage.Copy(rectangle);
                    try
                    {
                        ImageBox imgbox = new ImageBox();
                        ImageBox imgbox2 = new ImageBox();
                        tableLayoutPanel3.Controls.Add(imgbox);
                        imgbox.ClientSize = outputImage.Size;
                        imgbox.Image = outputImage;
                    }
                    catch
                    {

                    }
                }
            }




        }
    }
}
