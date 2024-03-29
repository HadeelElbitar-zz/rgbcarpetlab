﻿using System;
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
    public partial class Binarization : Form
    {
        private PictureInfo PicParent, pic;
        public UndoRedo picUndoRedoItem;
        public Binarization(PictureInfo picInfo, UndoRedo _picUndoRedoItem)
        {
            InitializeComponent();
            PicParent = picInfo;
            picUndoRedoItem = _picUndoRedoItem;
        }
        private void Binarization_Load(object sender, EventArgs e)
        {
            pictureBox1.Image = PicParent.pictureBox.Image;
            int height = PicParent.height;
            int width = PicParent.width;
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox2);
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        private void LevelTrackBar_Scroll(object sender, EventArgs e)
        {
            LevelUpDown.Value = LevelTrackBar.Value;
            double Threshold = (double)LevelUpDown.Value;
            UpdateForm();
        }
        private void LevelUpDown_ValueChanged(object sender, EventArgs e)
        {
            LevelTrackBar.Value = (int)LevelUpDown.Value;
            double Threshold = (double)LevelUpDown.Value;
            ImageClass Image = new ImageClass();
            pic = new PictureInfo(PicParent);
            //Image.ConvertToBinary(PicParent.height, PicParent.width, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray, Threshold);
            Image.ConvertToBinary(pic, Threshold);
            UpdateForm();
        }

        private void okbtn_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            picUndoRedoItem.UndoRedoCommands(PicParent, "Binarization");
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
                    PicParent.redPixels[i, j] = pic.redPixels[i, j];
                    PicParent.greenPixels[i, j] = pic.greenPixels[i, j];
                    PicParent.bluePixels[i, j] = pic.bluePixels[i, j];
                }
            }
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
            DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
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
            DisplayImage(PicParent.width, PicParent.height, pic.redPixels, pic.greenPixels, pic.bluePixels, pictureBox2);
        }


        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
    }
}