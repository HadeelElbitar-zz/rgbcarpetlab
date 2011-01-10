using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageProcessingAssignment1.Properties;
using System.Drawing.Imaging;

namespace ImageProcessingAssignment1
{
    public partial class FrequencyDomain : Form
    {
        #region initializations
        PictureInfo PicParent,RedPic,GreenPic,BluePic;
        #endregion
        public FrequencyDomain(PictureInfo pic)
        {
            InitializeComponent();
            ImageClass IM = new ImageClass();
            PicParent = new PictureInfo(pic);
        }
        private void FrequencyDomain_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = PicParent.pictureBox.Image;
            int height = PicParent.height;
            int width = PicParent.width;
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
        }
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
            picBox.BackColor = Color.White;
        }
        private void RedBTN_CheckedChanged(object sender, EventArgs e)
        {
            DisplayImage(RedPic.width, RedPic.height, RedPic.redPixels, RedPic.greenPixels, RedPic.bluePixels, pictureBox2);
        }
        private void GreenBTN_CheckedChanged(object sender, EventArgs e)
        {
            DisplayImage(GreenPic.width, GreenPic.height, GreenPic.redPixels, GreenPic.greenPixels, GreenPic.bluePixels, pictureBox2);
        }
        private void BlueBTN_CheckedChanged(object sender, EventArgs e)
        {
            DisplayImage(BluePic.width, BluePic.height, BluePic.redPixels, BluePic.greenPixels, BluePic.bluePixels, pictureBox2);
        }
        private void FreqButton_Click(object sender, EventArgs e)
        {
            int width = PicParent.width;
            int height = PicParent.height;
            ImageClass Image = new ImageClass();
            Image.convertToFrequencyDomain(PicParent);
            RedPic = new PictureInfo(width, height, "Red.bmp", "", new PictureBox(), new byte[height, width], new byte[height, width], new byte[height, width]);
            GreenPic = new PictureInfo(width, height, "Green.bmp", "", new PictureBox(), new byte[height, width], new byte[height, width], new byte[height, width]);
            BluePic = new PictureInfo(width, height, "Blue.bmp", "", new PictureBox(), new byte[height, width], new byte[height, width], new byte[height, width]);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    RedPic.redPixels[i, j] = RedPic.greenPixels[i, j] = RedPic.bluePixels[i, j] = PicParent.redPixels[i, j];
                    GreenPic.redPixels[i, j] = GreenPic.greenPixels[i, j] = GreenPic.bluePixels[i, j] = PicParent.greenPixels[i, j];
                    BluePic.redPixels[i, j] = BluePic.greenPixels[i, j] = BluePic.bluePixels[i, j] = PicParent.bluePixels[i, j];
                }
            }
            if (RedBTN.Checked)
                DisplayImage(width, height, RedPic.redPixels, RedPic.greenPixels, RedPic.bluePixels, pictureBox2);
            else if (GreenBTN.Checked)
                DisplayImage(width, height, GreenPic.redPixels, GreenPic.greenPixels, GreenPic.bluePixels, pictureBox2);
            else if (BlueBTN.Checked)
                DisplayImage(width, height, BluePic.redPixels, BluePic.greenPixels, BluePic.bluePixels, pictureBox2);

            FreqButton.Visible = false;
            ColorGroupBox.Visible = true;
            SpatialButton.Visible = true;
        }
        private void SpatialButton_Click(object sender, EventArgs e)
        {
            int width = PicParent.width;
            int height = PicParent.height;
            ImageClass Image = new ImageClass();
            Image.ConverttoSpatialDomain(PicParent);
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox3);
            FreqButton.Visible = true;
            ColorGroupBox.Visible = false;
            SpatialButton.Visible = false;
        }
    }
}
