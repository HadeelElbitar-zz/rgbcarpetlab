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
    public partial class Thresholding : Form
    {
        private PictureInfo PicParent;
        private UndoRedo picUndoRedoItem;
        byte[,] Red;
        byte[,] Green;
        byte[,] Blue;
        public Thresholding(PictureInfo picInfo, UndoRedo _picUndoRedoItem)
        {
            InitializeComponent();
            PicParent = picInfo;
            picUndoRedoItem = _picUndoRedoItem;
        }
        private void Thresholding_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = PicParent.pictureBox.Image;
            int height = PicParent.height;
            int width = PicParent.width;
            Red = new byte[height, width];
            Green = new byte[height, width];
            Blue = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    Red[i, j] = PicParent.redPixels[i, j];
                    Green[i, j] = PicParent.greenPixels[i, j];
                    Blue[i, j] = PicParent.bluePixels[i, j];
                }
            }
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox2);
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        private void EpsilonTrackBar_Scroll(object sender, EventArgs e)
        {
            EpsilonUpDown.Value = EpsilonTrackBar.Value;
        }
        private void EpsilonUpDown_ValueChanged(object sender, EventArgs e)
        {
            if( EpsilonTrackBar.Value != (int)EpsilonUpDown.Value)
                EpsilonTrackBar.Value = (int)EpsilonUpDown.Value;
            int Epsilon = (int)EpsilonUpDown.Value;
            ImageClass Image = new ImageClass();
            Image.BasicGlobalThresholding(PicParent, Epsilon, Red, Green, Blue);
            UpdateForm();
        }
        private void okbtn_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            this.Close();
        }
        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void Thresholding_FormClosed(object sender, FormClosedEventArgs e)
        {
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        private void ApplyChanges()
        {
            int height = PicParent.height;
            int width = PicParent.width;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    PicParent.redPixels[i, j] = Red[i, j];
                    PicParent.greenPixels[i, j] = Green[i, j];
                    PicParent.bluePixels[i, j] = Blue[i, j];
                }
            }
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
            picUndoRedoItem.UndoRedoCommands(PicParent, "Basic Global Thresholding");
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
        }
        private void UpdateForm()
        {
            DisplayImage(PicParent.width, PicParent.height, Red, Green, Blue, pictureBox2);
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
    }
}