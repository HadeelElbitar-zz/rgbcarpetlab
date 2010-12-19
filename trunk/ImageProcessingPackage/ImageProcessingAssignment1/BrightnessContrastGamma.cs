using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageProcessingAssignment1
{
    public partial class BrightnessContrastGamma : Form
    {
        public PictureInfo PicParent;
        public byte[,] modifiedRPixelArray;
        public byte[,] modifiedGPixelArray;
        public byte[,] modifiedBPixelArray;
        public byte[,] tempRPixelArray;
        public byte[,] tempGPixelArray;
        public byte[,] tempBPixelArray;

        public BrightnessContrastGamma(PictureInfo picInfo)
        {
            InitializeComponent();
            PicParent = picInfo;
        }
        private void BrightnessContrast_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = PicParent.pictureBox.Image;
            int height = PicParent.height;
            int width = PicParent.width;
            modifiedRPixelArray = new byte[height, width];
            modifiedGPixelArray = new byte[height, width];
            modifiedBPixelArray = new byte[height, width];
            tempRPixelArray = new byte[height, width];
            tempGPixelArray = new byte[height, width];
            tempBPixelArray = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tempRPixelArray[i, j] = modifiedRPixelArray[i, j] = PicParent.redPixels[i, j];
                    tempGPixelArray[i, j] = modifiedGPixelArray[i, j] = PicParent.greenPixels[i, j];
                    tempBPixelArray[i, j] = modifiedBPixelArray[i, j] = PicParent.bluePixels[i, j];
                }
            }
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox2);
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //Brightness
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            numericUpDown1.Value = trackBar1.Value;
            double lastBrightness = (double)numericUpDown1.Value;
            ImageClass Image = new ImageClass();
            Image.ChangeBrightness(PicParent.height, PicParent.width, lastBrightness, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray);
            UpdateForm();
        }
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            trackBar1.Value = (int)numericUpDown1.Value;
            double lastBrightness = (double)numericUpDown1.Value;
            ImageClass Image = new ImageClass();
            Image.ChangeBrightness(PicParent.height, PicParent.width, lastBrightness, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray);
            UpdateForm();
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //Contrast
        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            numericUpDown2.Value = trackBar2.Value;
            double lastContrast = (double)numericUpDown2.Value;
            ImageClass Image = new ImageClass();
            Image.ChangeContrast(PicParent.height, PicParent.width, lastContrast, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray);
            UpdateForm();
        }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            trackBar2.Value = (int)numericUpDown2.Value;
            double lastContrast = (double)numericUpDown2.Value;
            ImageClass Image = new ImageClass();
            Image.ChangeContrast(PicParent.height, PicParent.width, lastContrast, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray);
            UpdateForm();
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //Gamma Correction
        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            if (trackBar3.Value == 0) numericUpDown3.Value = 0.1M;
            else numericUpDown3.Value = trackBar3.Value;
            ImageClass Image = new ImageClass();
            Image.GammaCorrection(PicParent.height, PicParent.width, (double)numericUpDown3.Value, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray);
            UpdateForm();
        }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            trackBar3.Value = (int)numericUpDown3.Value;
            ImageClass Image = new ImageClass();
            Image.GammaCorrection(PicParent.height, PicParent.width, (double)numericUpDown3.Value, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray);
            UpdateForm();
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //Apply-Ok-Cancel Buttons -- CheckBox1
        private void button1_Click(object sender, EventArgs e)//Apply
        {
            ApplyChanges();
        }
        private void button2_Click(object sender, EventArgs e)//Ok
        {
            ApplyChanges();
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)//Cancel
        {
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
            this.Close();
        }

        //Apply Changes
        private void ApplyChanges()
        {
            int height = PicParent.height;
            int width = PicParent.width;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    PicParent.redPixels[i, j] = modifiedRPixelArray[i, j];
                    PicParent.greenPixels[i, j] = modifiedGPixelArray[i, j];
                    PicParent.bluePixels[i, j] = modifiedBPixelArray[i, j];
                }
            }
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
                DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
            else
                DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, PicParent.pictureBox);
        }
        private void BrightnessContrastGamma_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //Display Image
        private void DisplayImage(int width, int height, byte[,] Red, byte[,] Green, byte[,] Blue, PictureBox picBox)
        {
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            BitmapData bmpData = bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadWrite, bmp.PixelFormat);
            unsafe
            {
                byte* p = (byte*)bmpData.Scan0;
                int space = bmpData.Stride - width * 3;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        p[0] = Blue[i, j];
                        p[1] = Green[i, j];
                        p[2] = Red[i, j];
                        p += 3;
                    }
                    p += space;
                }
            }
            bmp.UnlockBits(bmpData);
            picBox.Image = bmp;
        }
        private void UpdateForm()
        {
            DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, pictureBox2);
            if (checkBox1.Checked == true)
            {
                DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, PicParent.pictureBox);
            }
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
    }
}