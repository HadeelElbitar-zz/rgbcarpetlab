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
    public partial class Quantization : Form
    {
        PictureInfo PicParent;
        byte[,] modifiedRPixelArray;
        byte[,] modifiedGPixelArray;
        byte[,] modifiedBPixelArray;
        int[] BinaryCode;
        public Quantization(PictureInfo picInfo)
        {
            InitializeComponent();
            PicParent = picInfo;
        }
        private void Quantization_Load(object sender, EventArgs e)
        {
            int height = PicParent.height;
            int width = PicParent.width;
            pictureBox1.Image = PicParent.pictureBox.Image;
            modifiedRPixelArray = new byte[height, width];
            modifiedGPixelArray = new byte[height, width];
            modifiedBPixelArray = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    modifiedRPixelArray[i, j] = PicParent.redPixels[i, j];
                    modifiedGPixelArray[i, j] = PicParent.greenPixels[i, j];
                    modifiedBPixelArray[i, j] = PicParent.bluePixels[i, j];
                }
            }
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox2);
            BinaryCode = new int[8];
            int BinaryTemp = (int)Math.Pow(2.0, 8);
            for (int i = 0; i < 8; i++)
            {
                BinaryCode[i] = BinaryTemp - (int)Math.Pow(2.0, i);
                comboBox1.Items.Add(Math.Pow(2, 8 - i).ToString());
            }
            comboBox1.SelectedIndex = 0;
        }
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int height = PicParent.height;
            int width = PicParent.width;
            ImageClass Image = new ImageClass();
            Image.ApplyQuantization(height, width, BinaryCode[comboBox1.SelectedIndex],modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray,PicParent);
            DisplayImage(width, height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, pictureBox2);
            if (checkBox1.Checked == true)
                DisplayImage(width, height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, PicParent.pictureBox);
        }
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
        private void Quantization_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //Display Image
        private void DisplayImage(int width, int height, byte[,] redPix, byte[,] greenPix, byte[,] bluePix, PictureBox picBox)
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
                        p[0] = bluePix[i, j];
                        p[1] = greenPix[i, j];
                        p[2] = redPix[i, j];
                        p += 3;
                    }
                    p += space;
                }
            }
            bmp.UnlockBits(bmpData);
            picBox.Image = bmp;
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

    }
}