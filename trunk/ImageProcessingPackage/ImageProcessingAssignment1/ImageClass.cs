using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Forms;
using MathWorks.MATLAB.NET.Utility;
using MathWorks.MATLAB.NET.Arrays;
using MatlabProject;
using MatLabFunction;

namespace ImageProcessingAssignment1
{
    class ImageClass
    {
        #region Open Image
        public void OpenImage(string picPath, ref PictureInfo newPictureItem, string PictureName, PictureBox picBox)
        {
            Bitmap Bmp = new Bitmap(picPath);
            int width = Bmp.Width;
            int height = Bmp.Height;
            newPictureItem = new PictureInfo(width, height, PictureName, picPath, picBox, new byte[height, width], new byte[height, width], new byte[height, width]);
            BitmapData bmpData = Bmp.LockBits(new Rectangle(0, 0, width, height), System.Drawing.Imaging.ImageLockMode.ReadOnly, Bmp.PixelFormat);
            unsafe
            {
                byte* p = (byte*)bmpData.Scan0;
                if (Bmp.PixelFormat == PixelFormat.Format64bppArgb || Bmp.PixelFormat == PixelFormat.Format64bppPArgb)
                {
                    int space = bmpData.Stride - width * 8;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            newPictureItem.bluePixels[i, j] = p[0];
                            newPictureItem.greenPixels[i, j] = p[1];
                            newPictureItem.redPixels[i, j] = p[2];
                            p += 8;
                        }
                        p += space;
                    }
                }
                if (Bmp.PixelFormat == PixelFormat.Format32bppArgb || Bmp.PixelFormat == PixelFormat.Format32bppRgb)
                {
                    int space = bmpData.Stride - width * 4;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            newPictureItem.bluePixels[i, j] = p[0];
                            newPictureItem.greenPixels[i, j] = p[1];
                            newPictureItem.redPixels[i, j] = p[2];
                            p += 4;
                        }
                        p += space;
                    }
                }
                else if (Bmp.PixelFormat == PixelFormat.Format24bppRgb)
                {
                    int space = bmpData.Stride - width * 3;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            newPictureItem.bluePixels[i, j] = p[0];
                            newPictureItem.greenPixels[i, j] = p[1];
                            newPictureItem.redPixels[i, j] = p[2];
                            p += 3;
                        }
                        p += space;
                    }
                }
                else if (Bmp.PixelFormat == PixelFormat.Format16bppRgb555)
                {
                    int space = bmpData.Stride - width * 2;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            newPictureItem.bluePixels[i, j] = p[0];
                            newPictureItem.greenPixels[i, j] = p[1];
                            newPictureItem.redPixels[i, j] = p[2];
                            p += 2;
                        }
                        p += space;
                    }
                }
                else if (Bmp.PixelFormat == PixelFormat.Format8bppIndexed)
                {
                    int space = bmpData.Stride - width;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            newPictureItem.bluePixels[i, j] = newPictureItem.greenPixels[i, j] = newPictureItem.redPixels[i, j] = p[0];
                            p++;
                        }
                        p += space;
                    }
                }
                else if (Bmp.PixelFormat == PixelFormat.Format4bppIndexed)
                {
                    int space = bmpData.Stride - ((width / 2) + (width % 2));
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            int e = ((j + 1) % 2 == 0) ? p[0] >> 4 : p[0] & 0x0F;
                            newPictureItem.bluePixels[i, j] = newPictureItem.greenPixels[i, j] = newPictureItem.redPixels[i, j] = p[e];
                            p += ((j + 1) % 2);
                        }
                        p += space;
                    }
                }
                else if (Bmp.PixelFormat == PixelFormat.Format1bppIndexed)
                {
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            byte* ptr = (byte*)bmpData.Scan0 + (i * bmpData.Stride) + (j / 8);
                            byte b = *ptr;
                            byte mask = Convert.ToByte(0x80 >> (j % 8));
                            if ((b & mask) != 0)
                                newPictureItem.bluePixels[i, j] = newPictureItem.greenPixels[i, j] = newPictureItem.redPixels[i, j] = 255;
                            else
                                newPictureItem.bluePixels[i, j] = newPictureItem.greenPixels[i, j] = newPictureItem.redPixels[i, j] = 0;
                        }
                    }
                }
            }
            Bmp.UnlockBits(bmpData);
        }
        public void openPPM(string picPath, ref PictureInfo newPictureItem, string PictureName, PictureBox picBox)
        {
            FileStream FS = new FileStream(picPath, FileMode.Open);
            StreamReader SR = new StreamReader(FS);
            string type = SR.ReadLine();
            int Size_P6 = type.Length + 1;
            string temp;
            string MetaData = "";
            while (true)
            {
                temp = SR.ReadLine();
                if (temp[0] == '#')
                {
                    MetaData += temp;
                    Size_P6 += temp.Length + 1;
                }
                else
                    break;
            }
            string Width_Height = temp;
            Size_P6 += temp.Length + 2;
            int FirstIndex = Width_Height.IndexOf(' ');
            int width = int.Parse(Width_Height.Substring(0, FirstIndex));
            int height = int.Parse(Width_Height.Substring(FirstIndex + 1, Width_Height.Length - FirstIndex - 1));
            int Size = width * height * 3;
            temp = SR.ReadLine();
            int MaxValue = int.Parse(temp);
            Size_P6 += type.Length + 1;
            newPictureItem = new PictureInfo(width, height, PictureName, picPath, picBox, new byte[height, width], new byte[height, width], new byte[height, width]);
            if (type.ToLower() == "p3")
            {
                newPictureItem.type = "p3";
                string Values = SR.ReadToEnd();
                string[] tempValues = Values.Split(new char[] { ' ' });
                int ind = 0;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        newPictureItem.redPixels[i, j] = new byte();
                        newPictureItem.redPixels[i, j] = byte.Parse(tempValues[ind++]);
                        newPictureItem.greenPixels[i, j] = new byte();
                        newPictureItem.greenPixels[i, j] = byte.Parse(tempValues[ind++]);
                        newPictureItem.bluePixels[i, j] = new byte();
                        newPictureItem.bluePixels[i, j] = byte.Parse(tempValues[ind++]);
                    }
                }
            }
            else if (type.ToLower() == "p6")
            {
                newPictureItem.type = "p6";
                FS.Position = Size_P6;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        newPictureItem.redPixels[i, j] = new byte();
                        newPictureItem.redPixels[i, j] = (byte)FS.ReadByte();
                        newPictureItem.greenPixels[i, j] = new byte();
                        newPictureItem.greenPixels[i, j] = (byte)FS.ReadByte();
                        newPictureItem.bluePixels[i, j] = new byte();
                        newPictureItem.bluePixels[i, j] = (byte)FS.ReadByte();
                    }
                }
            }
            FS.Close();
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Save asPPM
        public void SaveAsPPM(string PicturePath, PictureInfo pic, string type)
        {
            FileStream FS = new FileStream(PicturePath, FileMode.Create);
            StreamWriter SW = new StreamWriter(FS);
            int height = pic.height;
            int width = pic.width;
            int MaxValue = 255;
            if (type == "p3")
                SaveAsP3(SW, height, width, MaxValue, pic);
            else
                SaveAsP6(FS, height, width, MaxValue, pic);
            FS.Close();

        }
        public void SaveAsP3(StreamWriter SW, int height, int width, int MaxValue, PictureInfo pic)
        {
            try
            {
                SW.WriteLine("P3");
                SW.WriteLine(width.ToString() + " " + height.ToString());
                SW.WriteLine(MaxValue.ToString());
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        string PixString = pic.redPixels[i, j].ToString() + " ";
                        PixString += (pic.greenPixels[i, j].ToString() + " ");
                        PixString += (pic.bluePixels[i, j].ToString() + " ");
                        SW.Write(PixString);
                    }
                }
                SW.Close();
            }
            catch { }
        }
        public void SaveAsP6(FileStream FS, int height, int width, int MaxValue, PictureInfo pic)
        {
            try
            {
                string str = "P6\n" + width.ToString() + " " + height.ToString() + "\n" + MaxValue.ToString() + "\n";
                int Length = str.Length;
                byte[] Header = new byte[Length];
                for (int i = 0; i < Length; i++)
                {
                    Header[i] = (byte)(str[i]);
                }
                FS.Write(Header, 0, Length);

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        FS.WriteByte(pic.redPixels[i, j]);
                        FS.WriteByte(pic.greenPixels[i, j]);
                        FS.WriteByte(pic.bluePixels[i, j]);
                    }
                }
            }
            catch { }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Image operations
        public void TranslateImage(PictureInfo pic, int xTranslation, int yTranslation)
        {
            int height = pic.height;
            int width = pic.width;
            byte[,] Red = new byte[height, width];
            byte[,] Green = new byte[height, width];
            byte[,] Blue = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int newYCoor = (i + yTranslation) % height;
                    int newXCoor = (j + xTranslation) % width;
                    if (newYCoor < 0) newYCoor += height;
                    if (newXCoor < 0) newXCoor += width;
                    Red[newYCoor, newXCoor] = pic.redPixels[i, j];
                    Green[newYCoor, newXCoor] = pic.greenPixels[i, j];
                    Blue[newYCoor, newXCoor] = pic.bluePixels[i, j];
                }
            }
            pic.redPixels = Red;
            pic.greenPixels = Green;
            pic.bluePixels = Blue;
        }
        public void RotateImage(PictureInfo pic, double _Theta)
        {
            int height = pic.height;
            int width = pic.width;
            _Theta *= -1;
            double Theta = _Theta * Math.PI / 180;
            double cosTheta = Math.Cos(Theta);
            double sinTheta = Math.Sin(Theta);
            int newHeight = 0, newWidth = 0;
            int xValue = 0, yValue = 0;
            if (_Theta < 0)
            {
                xValue = -(int)(width * sinTheta);
                newHeight = (int)(height * cosTheta) + xValue + 1;
                newWidth = (int)(-height * sinTheta + width * cosTheta);
            }
            else
            {
                yValue = (int)(height * sinTheta);
                newHeight = (int)(height * cosTheta + width * sinTheta);
                newWidth = (int)(width * cosTheta) + yValue + 1;
            }
            byte[,] Red = new byte[newHeight, newWidth];
            byte[,] Green = new byte[newHeight, newWidth];
            byte[,] Blue = new byte[newHeight, newWidth];

            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    int _X = (int)Math.Floor(i * cosTheta + j * (-sinTheta));
                    int _Y = (int)Math.Floor(i * sinTheta + j * cosTheta);
                    if (_X > -1 && _X < pic.height && _Y > -1 && _Y < pic.width)
                    {
                        Red[i + xValue, j + yValue] = pic.redPixels[_X, _Y];
                        Green[i + xValue, j + yValue] = pic.greenPixels[_X, _Y];
                        Blue[i + xValue, j + yValue] = pic.bluePixels[_X, _Y];
                    }
                }
            }
            pic.height = newHeight;
            pic.width = newWidth;
            pic.redPixels = Red;
            pic.greenPixels = Green;
            pic.bluePixels = Blue;
        }
        public void ShearImage(PictureInfo pic, int Shear)
        {
            int height = pic.height;
            int width = pic.width;
            int newHeight = 0;
            int newWidth = 0;
            int index = 0;
            newHeight = height;
            if (Shear < 0) newWidth = height * (-Shear) + width;
            else newWidth = height * Shear + width;
            byte[,] Red = new byte[newHeight, newWidth];
            byte[,] Green = new byte[newHeight, newWidth];
            byte[,] Blue = new byte[newHeight, newWidth];
            if (Shear > 0)
            {
                for (int i = height - 1; i >= 0; i--)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int newX = i;
                        int newY = j;
                        newY = index * Shear + j;
                        Red[newX, newY] = pic.redPixels[i, j];
                        Green[newX, newY] = pic.greenPixels[i, j];
                        Blue[newX, newY] = pic.bluePixels[i, j];
                    }
                    index++;
                }
            }
            else
            {
                Shear *= -1;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int newX = i;
                        int newY = j;
                        newY = index * Shear + j;
                        Red[newX, newY] = pic.redPixels[i, j];
                        Green[newX, newY] = pic.greenPixels[i, j];
                        Blue[newX, newY] = pic.bluePixels[i, j];
                    }
                    index++;
                }
            }
            pic.height = newHeight;
            pic.width = newWidth;
            pic.redPixels = Red;
            pic.greenPixels = Green;
            pic.bluePixels = Blue;
        }
        public void FlipImage(PictureInfo pic, int flipType)
        {
            int height = pic.height;
            int width = pic.width;
            byte[,] Red = new byte[height, width];
            byte[,] Green = new byte[height, width];
            byte[,] Blue = new byte[height, width];
            if (flipType == 0)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        Red[i, width - j - 1] = pic.redPixels[i, j];
                        Green[i, width - j - 1] = pic.greenPixels[i, j];
                        Blue[i, width - j - 1] = pic.bluePixels[i, j];
                    }
                }
            }
            else
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        Red[height - i - 1, j] = pic.redPixels[i, j];
                        Green[height - i - 1, j] = pic.greenPixels[i, j];
                        Blue[height - i - 1, j] = pic.bluePixels[i, j];
                    }
                }
            }
            pic.redPixels = Red;
            pic.greenPixels = Green;
            pic.bluePixels = Blue;
        }
        public void ResizeImage(PictureInfo PicInfo, int NewH, int NewW)
        {
            int height = PicInfo.height;
            int width = PicInfo.width;
            byte[,] Red = new byte[NewH, NewW];
            byte[,] Green = new byte[NewH, NewW];
            byte[,] Blue = new byte[NewH, NewW];
            double W_Ratio = (double)width / (double)NewW;
            double H_Ratio = (double)height / (double)NewH;
            for (int j = 0; j < NewH; j++)
            {
                for (int i = 0; i < NewW; i++)
                {
                    //Calculate the corresponding location in the original buffer
                    double OldX = i * W_Ratio;
                    double OldY = j * H_Ratio;

                    //Find 4 adjacent pixels
                    int X_1 = (int)Math.Floor(OldX);
                    int X_2 = Math.Min((int)Math.Ceiling(OldX), width - 1);
                    int Y_1 = (int)Math.Floor(OldY);
                    int Y_2 = Math.Min((int)Math.Ceiling(OldY), height - 1);

                    Pixel Point_1 = new Pixel(PicInfo.redPixels[Y_1, X_1], PicInfo.greenPixels[Y_1, X_1], PicInfo.bluePixels[Y_1, X_1]);
                    Pixel Point_2 = new Pixel(PicInfo.redPixels[Y_1, X_2], PicInfo.greenPixels[Y_1, X_2], PicInfo.bluePixels[Y_1, X_2]);
                    Pixel Point_3 = new Pixel(PicInfo.redPixels[Y_2, X_1], PicInfo.greenPixels[Y_2, X_1], PicInfo.bluePixels[Y_2, X_1]);
                    Pixel Point_4 = new Pixel(PicInfo.redPixels[Y_2, X_2], PicInfo.greenPixels[Y_2, X_2], PicInfo.bluePixels[Y_2, X_2]);

                    //Calculate X, Y fractions
                    double X_Fraction = OldX - X_1;
                    double Y_Fraction = OldY - Y_1;

                    //Interpolate in X-Direction
                    //float Z_1 = Point_1 * (1 - X_Fraction) + Point_2 * X_Fraction;
                    //float Z_2 = Point_3 * (1 - X_Fraction) + Point_4 * X_Fraction;
                    double Z_1_R = Point_1.GetR() * (1 - X_Fraction) + Point_2.GetR() * X_Fraction;
                    double Z_1_G = Point_1.GetG() * (1 - X_Fraction) + Point_2.GetG() * X_Fraction;
                    double Z_1_B = Point_1.GetB() * (1 - X_Fraction) + Point_2.GetB() * X_Fraction;

                    //Interpolate in Y-Direction
                    //int NewPixel = Z_1 * (1 - Y_Fraction) + Z_2 * Y_Fraction;
                    double Z_2_R = Point_3.GetR() * (1 - X_Fraction) + Point_4.GetR() * X_Fraction;
                    double Z_2_G = Point_3.GetG() * (1 - X_Fraction) + Point_4.GetG() * X_Fraction;
                    double Z_2_B = Point_3.GetB() * (1 - X_Fraction) + Point_4.GetB() * X_Fraction;

                    //Add the New Pixel
                    Red[j, i] = (byte)(Z_1_R * (1 - Y_Fraction) + Z_2_R * Y_Fraction);
                    Green[j, i] = (byte)(Z_1_G * (1 - Y_Fraction) + Z_2_G * Y_Fraction);
                    Blue[j, i] = (byte)(Z_1_B * (1 - Y_Fraction) + Z_2_B * Y_Fraction);
                }
            }
            PicInfo.pictureBox.Width = PicInfo.width = NewW;
            PicInfo.pictureBox.Height = PicInfo.height = NewH;
            PicInfo.redPixels = Red;
            PicInfo.greenPixels = Green;
            PicInfo.bluePixels = Blue;
        }
        public void ResizeImage(PictureInfo PicInfo, int NewH, int NewW, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue)
        {
            int height = PicInfo.height;
            int width = PicInfo.width;
            double W_Ratio = (double)width / (double)NewW;
            double H_Ratio = (double)height / (double)NewH;
            for (int j = 0; j < NewH; j++)
            {
                for (int i = 0; i < NewW; i++)
                {
                    //Calculate the corresponding location in the original buffer
                    double OldX = i * W_Ratio;
                    double OldY = j * H_Ratio;

                    //Find 4 adjacent pixels
                    int X_1 = (int)Math.Floor(OldX);
                    int X_2 = Math.Min((int)Math.Ceiling(OldX), width - 1);
                    int Y_1 = (int)Math.Floor(OldY);
                    int Y_2 = Math.Min((int)Math.Ceiling(OldY), height - 1);

                    Pixel Point_1 = new Pixel(PicInfo.redPixels[Y_1, X_1], PicInfo.greenPixels[Y_1, X_1], PicInfo.bluePixels[Y_1, X_1]);
                    Pixel Point_2 = new Pixel(PicInfo.redPixels[Y_1, X_2], PicInfo.greenPixels[Y_1, X_2], PicInfo.bluePixels[Y_1, X_2]);
                    Pixel Point_3 = new Pixel(PicInfo.redPixels[Y_2, X_1], PicInfo.greenPixels[Y_2, X_1], PicInfo.bluePixels[Y_2, X_1]);
                    Pixel Point_4 = new Pixel(PicInfo.redPixels[Y_2, X_2], PicInfo.greenPixels[Y_2, X_2], PicInfo.bluePixels[Y_2, X_2]);

                    //Calculate X, Y fractions
                    double X_Fraction = OldX - X_1;
                    double Y_Fraction = OldY - Y_1;

                    //Interpolate in X-Direction
                    //float Z_1 = Point_1 * (1 - X_Fraction) + Point_2 * X_Fraction;
                    //float Z_2 = Point_3 * (1 - X_Fraction) + Point_4 * X_Fraction;
                    double Z_1_R = Point_1.GetR() * (1 - X_Fraction) + Point_2.GetR() * X_Fraction;
                    double Z_1_G = Point_1.GetG() * (1 - X_Fraction) + Point_2.GetG() * X_Fraction;
                    double Z_1_B = Point_1.GetB() * (1 - X_Fraction) + Point_2.GetB() * X_Fraction;

                    //Interpolate in Y-Direction
                    //int NewPixel = Z_1 * (1 - Y_Fraction) + Z_2 * Y_Fraction;
                    double Z_2_R = Point_3.GetR() * (1 - X_Fraction) + Point_4.GetR() * X_Fraction;
                    double Z_2_G = Point_3.GetG() * (1 - X_Fraction) + Point_4.GetG() * X_Fraction;
                    double Z_2_B = Point_3.GetB() * (1 - X_Fraction) + Point_4.GetB() * X_Fraction;

                    //Add the New Pixel
                    Red[j, i] = (byte)(Z_1_R * (1 - Y_Fraction) + Z_2_R * Y_Fraction);
                    Green[j, i] = (byte)(Z_1_G * (1 - Y_Fraction) + Z_2_G * Y_Fraction);
                    Blue[j, i] = (byte)(Z_1_B * (1 - Y_Fraction) + Z_2_B * Y_Fraction);
                }
            }
        }
        public void ReverseColors(PictureInfo pic)
        {
            int height = pic.height;
            int width = pic.width;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pic.redPixels[i, j] = (byte)(255 - (int)pic.redPixels[i, j]);
                    pic.greenPixels[i, j] = (byte)(255 - (int)pic.greenPixels[i, j]);
                    pic.bluePixels[i, j] = (byte)(255 - (int)pic.bluePixels[i, j]);
                }
            }
        }
        public void GrayScale(PictureInfo pic)
        {
            int width = pic.width;
            int height = pic.height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int temp = ((int)pic.redPixels[i, j] + (int)pic.greenPixels[i, j] + (int)pic.bluePixels[i, j]) / 3;
                    pic.redPixels[i, j] = (byte)temp;
                    pic.greenPixels[i, j] = (byte)temp;
                    pic.bluePixels[i, j] = (byte)temp;
                }
            }
        }
        public void histogramEqualization(PictureInfo pic, PictureInfo picSelected)
        {
            int height = pic.height;
            int width = pic.width;
            double[] RedHistogram = new double[256];
            double[] GreenHistogram = new double[256];
            double[] BlueHistogram = new double[256];
            GetHistogram(ref RedHistogram, ref GreenHistogram, ref BlueHistogram, picSelected);
            RunningSumToRound(ref RedHistogram);
            RunningSumToRound(ref GreenHistogram);
            RunningSumToRound(ref BlueHistogram);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pic.redPixels[i, j] = (byte)RedHistogram[pic.redPixels[i, j]];
                    pic.greenPixels[i, j] = (byte)GreenHistogram[pic.greenPixels[i, j]];
                    pic.bluePixels[i, j] = (byte)BlueHistogram[pic.bluePixels[i, j]];
                }
            }
        }

        public void ConvertToBinary(PictureInfo pic, double Threshold)
        {
            GrayScale(pic);
            int width = pic.width;
            int height = pic.height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (pic.redPixels[i, j] < Threshold)
                    {
                        pic.redPixels[i, j] = 0;
                        pic.greenPixels[i, j] = 0;
                        pic.bluePixels[i, j] = 0;
                    }
                    else
                    {
                        pic.redPixels[i, j] = 255;
                        pic.greenPixels[i, j] = 255;
                        pic.bluePixels[i, j] = 255;
                    }
                }
            }
        }
        public PictureInfo ConvertToBinary(PictureInfo pic)
        {
            GrayScale(pic);
            int width = pic.width;
            int height = pic.height;
            PictureInfo NewPic = new PictureInfo(pic);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (pic.redPixels[i, j] < 128)
                    {
                        NewPic.redPixels[i, j] = 0;
                        NewPic.greenPixels[i, j] = 0;
                        NewPic.bluePixels[i, j] = 0;
                    }
                    else
                    {
                        NewPic.redPixels[i, j] = 255;
                        NewPic.greenPixels[i, j] = 255;
                        NewPic.bluePixels[i, j] = 255;
                    }
                }
            }
            return NewPic;
        }
        public void ConvertToBinary(int height, int width, byte[,] tempRPixelArray, byte[,] tempGPixelArray, byte[,] tempBPixelArray, ref byte[,] modifiedRPixelArray, ref byte[,] modifiedGPixelArray, ref byte[,] modifiedBPixelArray, double Threshold)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int temp = ((int)tempRPixelArray[i, j] + (int)tempGPixelArray[i, j] + (int)tempBPixelArray[i, j]) / 3;
                    tempRPixelArray[i, j] = (byte)temp;
                    tempGPixelArray[i, j] = (byte)temp;
                    tempBPixelArray[i, j] = (byte)temp;
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (tempRPixelArray[i, j] <= Threshold)
                    {
                        modifiedRPixelArray[i, j] = 0;
                        modifiedGPixelArray[i, j] = 0;
                        modifiedBPixelArray[i, j] = 0;
                    }
                    else
                    {
                        modifiedRPixelArray[i, j] = 255;
                        modifiedGPixelArray[i, j] = 255;
                        modifiedBPixelArray[i, j] = 255;
                    }
                }
            }
        }

        //Brigthness
        public void ChangeBrightness(int height, int width, double lastBrightness, ref byte[,] modifiedRPixelArray, ref byte[,] modifiedGPixelArray, ref byte[,] modifiedBPixelArray)
        {
            modifiedRPixelArray = BrightnessHelp(height, width, lastBrightness, modifiedRPixelArray);
            modifiedGPixelArray = BrightnessHelp(height, width, lastBrightness, modifiedGPixelArray);
            modifiedBPixelArray = BrightnessHelp(height, width, lastBrightness, modifiedBPixelArray);
        }
        private byte[,] BrightnessHelp(int height, int width, double lastBrightness, byte[,] modifiedPixelArray)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    modifiedPixelArray[i, j] = (byte)(CutOffValue((double)modifiedPixelArray[i, j] + lastBrightness));
            return modifiedPixelArray;
        }

        //Contrast
        public void ChangeContrast(int height, int width, double lastContrast, ref byte[,] modifiedRPixelArray, ref byte[,] modifiedGPixelArray, ref byte[,] modifiedBPixelArray)
        {
            modifiedRPixelArray = ContrastHelp(height, width, lastContrast, modifiedRPixelArray);
            modifiedGPixelArray = ContrastHelp(height, width, lastContrast, modifiedGPixelArray);
            modifiedBPixelArray = ContrastHelp(height, width, lastContrast, modifiedBPixelArray);
        }
        private byte[,] ContrastHelp(int height, int width, double lastContrast, byte[,] PixelArray)
        {
            double oldMin = double.MaxValue, oldMax = double.MinValue;
            double[,] Contrast = new double[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    Contrast[i, j] = (double)PixelArray[i, j];
            getMaxAndMin(height, width, ref oldMin, ref oldMax, Contrast);
            double newMin = oldMin - lastContrast, newMax = oldMax + lastContrast;
            Normalization(height, width, oldMin, oldMax, newMax, newMin, Contrast);
            CutOff(height, width, 255, 0, Contrast);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    PixelArray[i, j] = (byte)Contrast[i, j];
            return PixelArray;
        }

        //Gamma Correction
        public void GammaCorrection(int height, int width, double Gamma, ref byte[,] modifiedRPixelArray, ref byte[,] modifiedGPixelArray, ref byte[,] modifiedBPixelArray)
        {
            modifiedRPixelArray = GammaHelp(height, width, Gamma, modifiedRPixelArray);
            modifiedGPixelArray = GammaHelp(height, width, Gamma, modifiedGPixelArray);
            modifiedBPixelArray = GammaHelp(height, width, Gamma, modifiedBPixelArray);
        }
        private byte[,] GammaHelp(int height, int width, double lastGamma, byte[,] PixelArray)
        {
            double oldMin = double.MaxValue, oldMax = double.MinValue;
            double newMin = 0, newMax = 255;
            double[,] Gamma = new double[height, width];
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    Gamma[i, j] = Math.Pow((double)PixelArray[i, j], 1 / lastGamma);
            getMaxAndMin(height, width, ref oldMin, ref oldMax, Gamma);
            Normalization(height, width, oldMin, oldMax, newMax, newMin, Gamma);
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    PixelArray[i, j] = (byte)Gamma[i, j];
            return PixelArray;
        }

        public void HistogramMatching(int height, int width, PictureInfo FirstPic, PictureInfo SecondPic, List<PictureInfo> picList)
        {
            double[] RedHistogram1 = new double[256];
            double[] GreenHistogram1 = new double[256];
            double[] BlueHistogram1 = new double[256];
            double[] RedHistogram2 = new double[256];
            double[] GreenHistogram2 = new double[256];
            double[] BlueHistogram2 = new double[256];
            int length = picList.Count - 1;
            GetHistogram(ref RedHistogram1, ref GreenHistogram1, ref BlueHistogram1, FirstPic);
            GetHistogram(ref RedHistogram2, ref GreenHistogram2, ref BlueHistogram2, SecondPic);
            RunningSumToRound(ref RedHistogram1);
            RunningSumToRound(ref GreenHistogram1);
            RunningSumToRound(ref BlueHistogram1);
            RunningSumToRound(ref RedHistogram2);
            RunningSumToRound(ref GreenHistogram2);
            RunningSumToRound(ref BlueHistogram2);

            picList[length].redPixels = new byte[height, width];
            picList[length].greenPixels = new byte[height, width];
            picList[length].bluePixels = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //hena fe moshkela :S
                    picList[length].redPixels[i, j] = (byte)GetClose(RedHistogram1[FirstPic.redPixels[i, j]], RedHistogram2);
                    picList[length].greenPixels[i, j] = (byte)GetClose(GreenHistogram1[FirstPic.greenPixels[i, j]], GreenHistogram2);
                    picList[length].bluePixels[i, j] = (byte)GetClose(BlueHistogram1[FirstPic.bluePixels[i, j]], BlueHistogram2);
                }
            }
        }
        private double GetClose(double Value, double[] Array)
        {
            int count = Array.Count();
            for (int i = 0; i < count; i++)
                if (Array[i] == Value || Array[i] == Value - 1 || Array[i] == Value + 1)
                    return i;
            return Value;
        }
        public void ApplyQuantization(int height, int width, int binary, byte[,] modifiedRPixelArray, byte[,] modifiedGPixelArray, byte[,] modifiedBPixelArray, PictureInfo PicParent)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    modifiedRPixelArray[i, j] = (byte)((int)PicParent.redPixels[i, j] & binary);
                    modifiedGPixelArray[i, j] = (byte)((int)PicParent.greenPixels[i, j] & binary);
                    modifiedBPixelArray[i, j] = (byte)((int)PicParent.bluePixels[i, j] & binary);
                }
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Arithmetic Operations
        public void AddSubtractTwoPictures(int height, int width, byte[,] fRed, byte[,] fGreen, byte[,] fBlue, byte[,] sRed, byte[,] sGreen, byte[,] sBlue, PictureInfo pic, int operation)
        {
            pic.height = height;
            pic.width = width;
            pic.redPixels = new byte[height, width];
            pic.greenPixels = new byte[height, width];
            pic.bluePixels = new byte[height, width];

            double rMin = double.MaxValue, rMax = double.MinValue, newMin = 0, newMax = 255;
            double gMin = double.MaxValue, gMax = double.MinValue;
            double bMin = double.MaxValue, bMax = double.MinValue;

            double[,] TempR = new double[height, width];
            double[,] TempG = new double[height, width];
            double[,] TempB = new double[height, width];
            if (operation == 0)//Add
            {
                Add(height, width, ref TempR, ref fRed, ref sRed);
                Add(height, width, ref TempG, ref fGreen, ref sGreen);
                Add(height, width, ref TempB, ref fBlue, ref sBlue);
            }
            else if (operation == 1)//Subtract
            {
                Subract(height, width, ref TempR, ref fRed, ref sRed);
                Subract(height, width, ref TempG, ref fGreen, ref sGreen);
                Subract(height, width, ref TempB, ref fBlue, ref sBlue);
            }
            else if (operation == 2)//Product
            {
                Product(height, width, ref TempR, ref fRed, ref sRed);
                Product(height, width, ref TempG, ref fGreen, ref sGreen);
                Product(height, width, ref TempB, ref fBlue, ref sBlue);
            }
            else if (operation == 3)//AND
            {
                AND(height, width, ref TempR, ref fRed, ref sRed);
                AND(height, width, ref TempG, ref fGreen, ref sGreen);
                AND(height, width, ref TempB, ref fBlue, ref sBlue);
            }
            else if (operation == 4)//OR
            {
                OR(height, width, ref TempR, ref fRed, ref sRed);
                OR(height, width, ref TempG, ref fGreen, ref sGreen);
                OR(height, width, ref TempB, ref fBlue, ref sBlue);
            }
            getMaxAndMin(height, width, ref rMin, ref rMax, TempR);
            getMaxAndMin(height, width, ref gMin, ref gMax, TempG);
            getMaxAndMin(height, width, ref bMin, ref bMax, TempB);
            Normalization(height, width, rMin, rMax, newMax, newMin, TempR);
            Normalization(height, width, gMin, gMax, newMax, newMin, TempG);
            Normalization(height, width, bMin, bMax, newMax, newMin, TempB);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pic.redPixels[i, j] = (byte)TempR[i, j];
                    pic.greenPixels[i, j] = (byte)TempG[i, j];
                    pic.bluePixels[i, j] = (byte)TempB[i, j];
                }
            }
        }
        private void Add(int height, int width, ref double[,] temp, ref byte[,] first, ref byte[,] second)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    temp[i, j] = (double)first[i, j] + (double)second[i, j];
        }
        private void Subract(int height, int width, ref double[,] temp, ref byte[,] first, ref byte[,] second)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    temp[i, j] = (double)first[i, j] - (double)second[i, j];
        }
        private void Product(int height, int width, ref double[,] temp, ref byte[,] first, ref byte[,] second)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    temp[i, j] = (double)first[i, j] * (double)second[i, j];
        }
        private void AND(int height, int width, ref double[,] temp, ref byte[,] first, ref byte[,] second)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    temp[i, j] = first[i, j] & second[i, j];
        }
        private void OR(int height, int width, ref double[,] temp, ref byte[,] first, ref byte[,] second)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    temp[i, j] = first[i, j] | second[i, j];
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Histogram
        public void GetHistogram(ref double[] R, ref double[] G, ref double[] B, PictureInfo pic)
        {
            int NR, NG, NB;
            int width = pic.width;
            int height = pic.height;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    NR = (int)++R[pic.redPixels[i, j]];
                    NG = (int)++G[pic.greenPixels[i, j]];
                    NB = (int)++B[pic.bluePixels[i, j]];
                }
            }
        }
        private void GetLocalHistogram(ref double[] R, ref double[] G, ref double[] B, byte[,] LocalWindowR, byte[,] LocalWindowG, byte[,] LocalWindowB, int WinSize)
        {
            int NR, NG, NB;
            for (int i = 0; i < WinSize; i++)
            {
                for (int j = 0; j < WinSize; j++)
                {
                    NR = (int)++R[LocalWindowR[i, j]];
                    NG = (int)++G[LocalWindowG[i, j]];
                    NB = (int)++B[LocalWindowB[i, j]];
                }
            }
        }
        public int GetMin(double[] A)
        {
            for (int i = 0; i < 256; i++)
                if (A[i] > 0) return i;
            return 0;
        }
        public int GetMax(double[] A)
        {
            for (int i = 255; i >= 0; i--)
                if (A[i] > 0) return i;
            return 0;
        }
        public void RunningSumToRound(ref double[] Array)
        {
            int length = Array.Count();
            for (int i = 1; i < length; i++)
                Array[i] += Array[i - 1];
            for (int i = 0; i < length; i++)
                Array[i] = Math.Round((Array[i] / Array[length - 1]) * 255);
        }
        private void NormalizeHistogram(ref double[] R, ref double[] G, ref double[] B, int W_H)
        {
            for (int i = 0; i < 256; i++)
            {
                R[i] /= W_H;
                G[i] /= W_H;
                B[i] /= W_H;
            }
        }
        public void LocalHE(PictureInfo pic, int WinSize)
        {
            //MFunctions MatLabFn = new MFunctions();
            //MWArray[] NewR = (MWArray[])(MatLabFn.LocalHE(1, (MWNumericArray)pic.redPixels, WinSize));
            //MWArray[] NewG = (MWArray[])(MatLabFn.LocalHE(1, (MWNumericArray)pic.greenPixels, WinSize));
            //MWArray[] NewB = (MWArray[])(MatLabFn.LocalHE(1, (MWNumericArray)pic.bluePixels, WinSize));
            //pic.redPixels = (byte[,])((MWNumericArray)(NewR[0])).ToArray(MWArrayComponent.Real);
            //pic.greenPixels = (byte[,])((MWNumericArray)(NewG[0])).ToArray(MWArrayComponent.Real);
            //pic.bluePixels = (byte[,])((MWNumericArray)(NewB[0])).ToArray(MWArrayComponent.Real);

            int height = pic.height;
            int width = pic.width;
            int HalfWinSize = WinSize / 2;
            byte[,] NewR = new byte[height, width];
            byte[,] NewG = new byte[height, width];
            byte[,] NewB = new byte[height, width];
            byte[,] LocalWindowR = new byte[WinSize, WinSize];
            byte[,] LocalWindowG = new byte[WinSize, WinSize];
            byte[,] LocalWindowB = new byte[WinSize, WinSize];
            for (int i = HalfWinSize; i < height - HalfWinSize - 1; i++)
            {
                for (int j = HalfWinSize; j < width - HalfWinSize - 1; j++)
                {
                    int indexJ = j - HalfWinSize;
                    int indexI = i - HalfWinSize;
                    for (int c = 0; c < WinSize; c++)
                    {
                        for (int k = 0; k < WinSize; k++)
                        {
                            LocalWindowR[c, k] = pic.redPixels[indexI, indexJ];
                            LocalWindowG[c, k] = pic.greenPixels[indexI, indexJ];
                            LocalWindowB[c, k] = pic.bluePixels[indexI, indexJ++];
                        }
                        indexI++;
                        indexJ = j - HalfWinSize;
                    }
                    double[] RedHistogram = new double[256];
                    double[] GreenHistogram = new double[256];
                    double[] BlueHistogram = new double[256];
                    GetLocalHistogram(ref RedHistogram, ref GreenHistogram, ref BlueHistogram, LocalWindowR, LocalWindowG, LocalWindowB, WinSize);
                    RunningSumToRound(ref RedHistogram);
                    RunningSumToRound(ref GreenHistogram);
                    RunningSumToRound(ref BlueHistogram);
                    for (int c = 0; c < WinSize; c++)
                    {
                        for (int k = 0; k < WinSize; k++)
                        {
                            LocalWindowR[c, k] = (byte)RedHistogram[LocalWindowR[c, k]];
                            LocalWindowG[c, k] = (byte)GreenHistogram[LocalWindowG[c, k]];
                            LocalWindowB[c, k] = (byte)BlueHistogram[LocalWindowB[c, k]];
                        }
                    }
                    NewR[i, j] = LocalWindowR[HalfWinSize, HalfWinSize];
                    NewG[i, j] = LocalWindowG[HalfWinSize, HalfWinSize];
                    NewB[i, j] = LocalWindowB[HalfWinSize, HalfWinSize];
                }
            }
            pic.redPixels = NewR;
            pic.greenPixels = NewG;
            pic.bluePixels = NewB;
        }

        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Helping Functions
        private int[,] ReflectSE(int[,] SE, int width, int height, ref int IOrigin, ref int JOrigin)
        {
            int[,] Temp = new int[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    Temp[i, width - j - 1] = SE[i, j];
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                    Temp[height - i - 1, j] = SE[i, j];
            }
            //Find New Origin
            IOrigin = height - (IOrigin + 1);
            JOrigin = width - (JOrigin + 1);
            return Temp;
        }
        private void Normalization(int height, int width, double oldMin, double oldMax, double newMax, double newMin, double[,] Array)
        {
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    Array[i, j] = ((Array[i, j] - oldMin) / (oldMax - oldMin)) * (newMax - newMin) + newMin;
        }
        private void CutOff(int height, int width, double Max, double Min, double[,] Array)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (Array[i, j] < Min) Array[i, j] = Min;
                    if (Array[i, j] > Max) Array[i, j] = Max;
                }
            }
        }
        private double CutOffValue(double Value)
        {
            if (Value > 255) return 255;
            if (Value < 0) return 0;
            return Value;
        }
        private void getMaxAndMin(int height, int width, ref double oldMin, ref double oldMax, double[,] doublePixels)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (doublePixels[i, j] > oldMax && doublePixels[i, j] < double.MaxValue) oldMax = doublePixels[i, j];
                    if (doublePixels[i, j] < oldMin) oldMin = doublePixels[i, j];
                }
            }
        }
        private void getNonZeroIndex(double[] R, double[] G, double[] B, ref int S, ref int F)
        {
            //int first , last;
            for (int i = 0; i < 255; i++)
            {
                if (R[i] != 0 || G[i] != 0 || B[i] != 0)
                {
                    S = i;
                    break;
                }
            }
            for (int i = 254; i >= 0; i--)
            {
                if (R[i] != 0 || G[i] != 0 || B[i] != 0)
                {
                    F = i;
                    break;
                }
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Converting between Spatial & frequency domain
        public void ConverttoSpatialDomain(PictureInfo pic)
        {
            int height = pic.height;
            int width = pic.width;
            pic.frequency = false;
            MatlabClass matlabClass = new MatlabClass();
            double[,] NewR = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.redReal, (MWNumericArray)pic.redImag))).ToArray(MWArrayComponent.Real);
            double[,] NewG = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.greenReal, (MWNumericArray)pic.greenImag))).ToArray(MWArrayComponent.Real);
            double[,] NewB = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.blueReal, (MWNumericArray)pic.blueImag))).ToArray(MWArrayComponent.Real);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (NewR[i, j] > 255) NewR[i, j] = 255;
                    else if (NewR[i, j] < 0) NewR[i, j] = 0;
                    pic.redPixels[i, j] = (byte)NewR[i, j];
                    if (NewG[i, j] > 255) NewG[i, j] = 255;
                    else if (NewG[i, j] < 0) NewG[i, j] = 0;
                    pic.greenPixels[i, j] = (byte)NewG[i, j];
                    if (NewB[i, j] > 255) NewB[i, j] = 255;
                    else if (NewB[i, j] < 0) NewB[i, j] = 0;
                    pic.bluePixels[i, j] = (byte)NewB[i, j];
                }
            }
        }
        public void convertToFrequencyDomain(PictureInfo pic)
        {
            int height = pic.height;
            int width = pic.width;
            pic.frequency = true;
            MatlabClass matlabClass = new MatlabClass();
            MWArray[] NewR = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.redPixels));
            MWArray[] NewG = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.greenPixels));
            MWArray[] NewB = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.bluePixels));
            pic.redReal = (double[,])((MWNumericArray)(NewR[0])).ToArray(MWArrayComponent.Real);
            pic.greenReal = (double[,])((MWNumericArray)(NewG[0])).ToArray(MWArrayComponent.Real);
            pic.blueReal = (double[,])((MWNumericArray)(NewB[0])).ToArray(MWArrayComponent.Real);
            pic.redImag = (double[,])((MWNumericArray)(NewR[1])).ToArray(MWArrayComponent.Real);
            pic.greenImag = (double[,])((MWNumericArray)(NewG[1])).ToArray(MWArrayComponent.Real);
            pic.blueImag = (double[,])((MWNumericArray)(NewB[1])).ToArray(MWArrayComponent.Real);
            //Normalization
            double[,] tempR = new double[height, width];
            double[,] tempG = new double[height, width];
            double[,] tempB = new double[height, width];
            double rMax = double.MinValue;
            double gMax = double.MinValue;
            double bMax = double.MinValue;
            double rMin = double.MaxValue;
            double gMin = double.MaxValue;
            double bMin = double.MaxValue;
            double cnst = 0.1;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tempR[i, j] = Math.Sqrt((pic.redReal[i, j] * pic.redReal[i, j]) + (pic.redImag[i, j] * pic.redImag[i, j]));
                    tempR[i, j] = Math.Log(tempR[i, j] + cnst);
                    if (tempR[i, j] > rMax) rMax = tempR[i, j];
                    else if (tempR[i, j] < rMin) rMin = tempR[i, j];

                    tempG[i, j] = Math.Sqrt((pic.greenReal[i, j] * pic.greenReal[i, j]) + (pic.greenImag[i, j] * pic.greenImag[i, j]));
                    tempG[i, j] = Math.Log(tempG[i, j] + cnst);
                    if (tempG[i, j] > gMax) gMax = tempG[i, j];
                    else if (tempG[i, j] < gMin) gMin = tempG[i, j];

                    tempB[i, j] = Math.Sqrt((pic.blueReal[i, j] * pic.blueReal[i, j]) + (pic.blueImag[i, j] * pic.blueImag[i, j]));
                    tempB[i, j] = Math.Log(tempB[i, j] + cnst);
                    if (tempB[i, j] > bMax) bMax = tempB[i, j];
                    else if (tempB[i, j] < bMin) bMin = tempB[i, j];
                }
            }
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pic.redPixels[i, j] = (byte)(((tempR[i, j] - rMin) / (rMax - rMin)) * 255);
                    pic.greenPixels[i, j] = (byte)(((tempG[i, j] - gMin) / (gMax - gMin)) * 255);
                    pic.bluePixels[i, j] = (byte)(((tempB[i, j] - bMin) / (bMax - bMin)) * 255);
                }
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Add Noise
        public void AddSaltPepperNoise(PictureInfo pic, double salt, double pepper)
        {
            if ((salt + pepper) > 100)
                MessageBox.Show("Summation of Salt and Pepper Percentages should be less than 100!");
            else
            {
                int height = pic.height;
                int width = pic.width;
                int Size = width * height;
                Point[] Noise = new Point[Size];
                int index = 0;
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        Noise[index++] = new Point(i, j);

                int saltNumber = (int)(salt * Size / 100);
                int pepperNumber = (int)(pepper * Size / 100);
                Random rand = new Random();
                index = Size;
                for (int i = 0; i < saltNumber; i++)
                {
                    int ChosenIndex = rand.Next(0, index--);
                    Point point = new Point(Noise[index].X, Noise[index].Y);
                    Noise[index] = new Point(Noise[ChosenIndex].X, Noise[ChosenIndex].Y);
                    Noise[ChosenIndex] = new Point(point.X, point.Y);
                    int x = Noise[index].X;
                    int y = Noise[index].Y;
                    pic.redPixels[x, y] = 255;
                    pic.greenPixels[x, y] = 255;
                    pic.bluePixels[x, y] = 255;
                }
                for (int i = 0; i < pepperNumber; i++)
                {
                    int ChosenIndex = rand.Next(0, index--);
                    Point point = new Point(Noise[index].X, Noise[index].Y);
                    Noise[index] = new Point(Noise[ChosenIndex].X, Noise[ChosenIndex].Y);
                    Noise[ChosenIndex] = new Point(point.X, point.Y);
                    int x = Noise[index].X;
                    int y = Noise[index].Y;
                    pic.redPixels[x, y] = 0;
                    pic.greenPixels[x, y] = 0;
                    pic.bluePixels[x, y] = 0;
                }
            }
        }
        public void AddAdditiveNoise(PictureInfo pic, string type, double a, double b, double NoisePercentage)
        {
            if (NoisePercentage > 100)
                MessageBox.Show("The percentage should be less than 100!");
            else
            {
                int height = pic.height;
                int width = pic.width;
                int Size = width * height;

                Random rand = new Random();
                Point[] Noise = new Point[Size];
                int index = 0;
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                        Noise[index++] = new Point(i, j);
                double rMin = double.MaxValue, rMax = double.MinValue;
                double gMin = double.MaxValue, gMax = double.MinValue;
                double bMin = double.MaxValue, bMax = double.MinValue;
                double[,] rPixels = new double[height, width];
                double[,] gPixels = new double[height, width];
                double[,] bPixels = new double[height, width];
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        rPixels[i, j] = pic.redPixels[i, j];
                        gPixels[i, j] = pic.greenPixels[i, j];
                        bPixels[i, j] = pic.bluePixels[i, j];
                    }
                }

                #region Uniform Noise
                if (type == "Uniform")
                {
                    int NoiseNumber = (int)((Size * NoisePercentage) / (100.0 * (b - a + 1)));
                    for (int j = (int)a; j <= b; j++)
                    {
                        for (int i = 0; i < NoiseNumber; i++)
                        {
                            int ChosenIndex = rand.Next(0, index--);
                            Point point = new Point(Noise[index].X, Noise[index].Y);
                            Noise[index] = new Point(Noise[ChosenIndex].X, Noise[ChosenIndex].Y);
                            Noise[ChosenIndex] = new Point(point.X, point.Y);
                            int x = Noise[index].X;
                            int y = Noise[index].Y;
                            rPixels[x, y] += j;
                            gPixels[x, y] += j;
                            bPixels[x, y] += j;
                        }
                    }
                }
                #endregion

                #region Gaussian Noise
                else if (type == "Gaussian")
                {
                    int min = 0, max = 255;
                    for (int i = min; i <= max; i++)
                    {
                        double Const = Math.Pow(((i - a) / b), 2) * (-0.5);
                        double denominator = (Math.Sqrt(2 * Math.PI) * b);
                        double NoiseNumber = ((1 / denominator) * (Math.Exp(Const)));
                        NoiseNumber = NoiseNumber * Size * (NoisePercentage / 100.0);
                        for (int j = 0; j < (int)NoiseNumber; j++)
                        {
                            int ChosenIndex = rand.Next(0, index--);
                            Point point = new Point(Noise[index].X, Noise[index].Y);
                            Noise[index] = new Point(Noise[ChosenIndex].X, Noise[ChosenIndex].Y);
                            Noise[ChosenIndex] = new Point(point.X, point.Y);
                            int x = Noise[index].X;
                            int y = Noise[index].Y;
                            rPixels[x, y] += i;
                            gPixels[x, y] += i;
                            bPixels[x, y] += i;
                        }
                    }
                }
                #endregion

                #region Rayleigh Noise
                else if (type == "Rayleigh")
                {
                    for (int j = (int)a; j <= 255; j++)
                    {
                        double Const = (-(j - a) * (j - a)) / b;
                        int NoiseNumber = (int)(((2.0 / b) * (j - a) * (Math.Pow(Math.E, Const)) * Size * (NoisePercentage / 100.0)));
                        for (int i = 0; i < NoiseNumber; i++)
                        {
                            int ChosenIndex = rand.Next(0, index--);
                            Point point = new Point(Noise[index].X, Noise[index].Y);
                            Noise[index] = new Point(Noise[ChosenIndex].X, Noise[ChosenIndex].Y);
                            Noise[ChosenIndex] = new Point(point.X, point.Y);
                            int x = Noise[index].X;
                            int y = Noise[index].Y;
                            rPixels[x, y] += j;
                            gPixels[x, y] += j;
                            bPixels[x, y] += j;
                        }
                    }
                }
                #endregion

                #region Gamma Noise
                else if (type == "Gamma")
                {
                    for (int j = (int)a; j <= 255; j++)
                    {
                        double t1 = Math.Pow(a, b);
                        double t2 = Math.Pow(j, b - 1);
                        double t3 = Factorial(b - 1);
                        double t4 = Math.Pow(Math.E, (-a * j));
                        int NoiseNumber = (int)((((t1 * t2 * t4) / t3) * Size * (NoisePercentage / 100.0)));
                        for (int i = 0; i < NoiseNumber; i++)
                        {
                            int ChosenIndex = rand.Next(0, index--);
                            Point point = new Point(Noise[index].X, Noise[index].Y);
                            Noise[index] = new Point(Noise[ChosenIndex].X, Noise[ChosenIndex].Y);
                            Noise[ChosenIndex] = new Point(point.X, point.Y);
                            int x = Noise[index].X;
                            int y = Noise[index].Y;
                            rPixels[x, y] += j;
                            gPixels[x, y] += j;
                            bPixels[x, y] += j;
                        }
                    }
                }
                #endregion

                #region Exponential Noise
                else if (type == "Exponential")
                {
                    for (int j = 0; j <= 255; j++)
                    {
                        double t1 = Math.Exp(-a * j);
                        int NoiseNumber = (int)(((a * t1) * Size * (NoisePercentage / 100.0)));
                        if (index< NoiseNumber)
                            break;
                        for (int i = 0; i < NoiseNumber; i++)
                        {
                            int ChosenIndex = rand.Next(0, index--);
                            Point point = new Point(Noise[index].X, Noise[index].Y);
                            Noise[index] = new Point(Noise[ChosenIndex].X, Noise[ChosenIndex].Y);
                            Noise[ChosenIndex] = new Point(point.X, point.Y);
                            int x = Noise[index].X;
                            int y = Noise[index].Y;
                            rPixels[x, y] += j;
                            gPixels[x, y] += j;
                            bPixels[x, y] += j;
                        }
                    }
                }
                #endregion

                #region Normalization
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        rMin = Math.Min(rPixels[i, j], rMin);
                        rMax = Math.Max(rPixels[i, j], rMax);
                        gMin = Math.Min(gPixels[i, j], gMin);
                        gMax = Math.Max(gPixels[i, j], gMax);
                        bMin = Math.Min(bPixels[i, j], bMin);
                        bMax = Math.Max(bPixels[i, j], bMax);
                    }
                }
                if (rMax > 255 || rMin < 0) Normalization(height, width, rMin, rMax, 255, 0, rPixels);
                if (gMax > 255 || gMin < 0) Normalization(height, width, gMin, gMax, 255, 0, gPixels);
                if (bMax > 255 || bMin < 0) Normalization(height, width, bMin, bMax, 255, 0, bPixels);
                #endregion

                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        pic.redPixels[i, j] = (byte)rPixels[i, j];
                        pic.greenPixels[i, j] = (byte)gPixels[i, j];
                        pic.bluePixels[i, j] = (byte)bPixels[i, j];
                    }
                }
            }
        }
        public double Factorial(double num)
        {
            double result = 1;
            for (double i = num; i >= 2; i--)
            {
                result *= i;
            }
            return result;
        }
        public void AddPeriodicNoise(PictureInfo pic, double Amp, double xFreq, double yFreq, double xPhase, double yPhase)
        {
            int height = pic.height;
            int width = pic.width;
            double xConst = 2 * Math.PI * xFreq / width;
            double yConst = 2 * Math.PI * yFreq / height;
            double[,] Red = new double[height, width];
            double[,] Green = new double[height, width];
            double[,] Blue = new double[height, width];

            double rMin = double.MaxValue, rMax = double.MinValue;
            double gMin = double.MaxValue, gMax = double.MinValue;
            double bMin = double.MaxValue, bMax = double.MinValue;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double x = xConst * (j + xPhase);
                    double y = yConst * (i + yPhase);
                    Red[i, j] = (double)(pic.redPixels[i, j]) + Amp * (Math.Sin(x + y));
                    Green[i, j] = (double)(pic.greenPixels[i, j]) + Amp * (Math.Sin(x + y));
                    Blue[i, j] = (double)(pic.bluePixels[i, j]) + Amp * (Math.Sin(x + y));
                    rMin = Math.Min(rMin, Red[i, j]);
                    rMax = Math.Max(rMax, Red[i, j]);
                    gMin = Math.Min(gMin, Green[i, j]);
                    gMax = Math.Max(gMax, Green[i, j]);
                    bMin = Math.Min(bMin, Blue[i, j]);
                    bMax = Math.Max(bMax, Blue[i, j]);
                }
            }
            if (rMin < 0 || rMax > 255) Normalization(height, width, rMin, rMax, 255, 0, Red);
            if (gMin < 0 || gMax > 255) Normalization(height, width, gMin, gMax, 255, 0, Green);
            if (bMin < 0 || bMax > 255) Normalization(height, width, bMin, bMax, 255, 0, Blue);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pic.redPixels[i, j] = (byte)Red[i, j];
                    pic.greenPixels[i, j] = (byte)Green[i, j];
                    pic.bluePixels[i, j] = (byte)Blue[i, j];
                }
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Image Segmentation
        public void OtsuSegmentation(PictureInfo pic)
        {
            //convert to gray scale
            GrayScale(pic);
            //Get Histogram
            double[] R = new double[256], G = new double[256], B = new double[256];
            GetHistogram(ref R, ref G, ref B, pic);
            int Start = 0, End = 0;
            getNonZeroIndex(R, G, B, ref Start, ref End); //get start and end points for iterating on K
            //Normalize el Histogram
            int size = pic.width * pic.height;
            NormalizeHistogram(ref R, ref G, ref B, size); //P(i)
            double[] CummulateR = new double[256], CummulateG = new double[256], CummulateB = new double[256];
            CummulateR[0] = R[0];
            for (int i = 1; i <= 255; i++)
            {
                CummulateR[i] = CummulateR[i - 1] + R[i];
            }
            double[] CummulateMeanR = new double[257], CummulateMeanG = new double[256], CummulateMeanB = new double[256];
            CummulateMeanR[0] = 0;
            for (int i = 0, j = 1; i <= 255; i++, j++)
            {
                CummulateMeanR[j] = CummulateMeanR[j - 1] + R[i] * i;
            }
            // get K 
            double MaxR = int.MinValue;
            int ArraySZ = End - Start + 1;
            double[] SigmaR = new double[ArraySZ], SigmaG = new double[ArraySZ], SigmaB = new double[ArraySZ];
            Dictionary<int, double> Sigma = new Dictionary<int, double>();
            for (int i = Start + 1, j = 0; i < End; i++, j++)
            {
                SigmaR[j] = Math.Pow((CummulateMeanR[254] * CummulateR[i] - CummulateMeanR[i]), 2) / (CummulateR[i] * (1 - CummulateR[i]));
                Sigma.Add(i, SigmaR[j]);
                if (SigmaR[j] > MaxR)
                    MaxR = SigmaR[j];
            }
            double FinalKr = 0.0, Rcount = 0.0;
            foreach (KeyValuePair<int, double> item in Sigma)
            {
                if (item.Value == MaxR)
                {
                    Rcount++;
                    FinalKr += item.Key;
                }
            }
            //177 , 178
            FinalKr /= Rcount;
            for (int i = 0; i < pic.height; i++)
            {
                for (int j = 0; j < pic.width; j++)
                {
                    if (pic.redPixels[i, j] <= FinalKr)
                    {
                        pic.redPixels[i, j] = 0;
                        pic.greenPixels[i, j] = 0;
                        pic.bluePixels[i, j] = 0;
                    }
                    else
                    {
                        pic.redPixels[i, j] = 255;
                        pic.greenPixels[i, j] = 255;
                        pic.bluePixels[i, j] = 255;
                    }
                }
            }
        }
        public void BasicGlobalThresholding(PictureInfo pic, int Epsilon, byte[,] Red, byte[,] Green, byte[,] Blue)
        {
            int height = pic.height;
            int width = pic.width;
            int L = 256;
            int Threshold = L / 2, TempThreshold = 0;
            int firstMean = 0, secondMean = 0, firstSpace = 0, secondSpace = 0;
            GrayScale(pic);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int value = (int)pic.redPixels[i, j];
                    firstMean += value;
                    firstSpace++;
                }
            }
            Threshold = firstMean / firstSpace;
            firstMean = firstSpace = 0;
            bool repeat = true;
            while (repeat)
            {
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        int value = (int)pic.redPixels[i, j];
                        if ((int)value < Threshold)
                        {
                            firstMean += value;
                            firstSpace++;
                        }
                        else
                        {
                            secondMean += value;
                            secondSpace++;
                        }
                    }
                }
                firstMean /= firstSpace;
                secondMean /= secondSpace;
                TempThreshold = (firstMean + secondMean) / 2;
                firstMean = firstSpace = 0;
                secondMean = secondSpace = 0;

                if (Math.Abs(TempThreshold - Threshold) <= Epsilon)
                    repeat = false;
                Threshold = TempThreshold;
            }
            for (int i = 0; i < height; i++)
                for (int j = 0; j < width; j++)
                    if (pic.redPixels[i, j] > TempThreshold)
                        Red[i, j] = Green[i, j] = Blue[i, j] = 255;
                    else
                        Red[i, j] = Green[i, j] = Blue[i, j] = 0;
        }
        public void AdaptiveThresholding(PictureInfo pic, int WinSize, int meanOffset)
        {
            int height = pic.height, width = pic.width;
            int newHeight = height + WinSize, newWidth = width + WinSize;
            Filter filter = new Filter();
            GrayScale(pic);
            byte[,] repRPixels = filter.ReplicateImage(WinSize, WinSize, pic.height, pic.width, pic.redPixels);
            byte[,] repGPixels = filter.ReplicateImage(WinSize, WinSize, pic.height, pic.width, pic.greenPixels);
            byte[,] repBPixels = filter.ReplicateImage(WinSize, WinSize, pic.height, pic.width, pic.bluePixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];
            int M = (WinSize - 1) / 2, N = (WinSize - 1) / 2, Fsize = WinSize * WinSize;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double sum = 0;
                    for (int c = 0; c < WinSize; c++)
                        for (int k = 0; k < WinSize; k++)
                            sum += (double)repRPixels[i + c, j + k];
                    sum /= Fsize;
                    if ((repRPixels[i + M, j + N] - sum) < meanOffset)
                    {
                        NewPicR[i + M, j + N] = (byte)255;
                        NewPicG[i + M, j + N] = (byte)255;
                        NewPicB[i + M, j + N] = (byte)255;
                    }
                    else
                    {
                        NewPicR[i + M, j + N] = (byte)0;
                        NewPicG[i + M, j + N] = (byte)0;
                        NewPicB[i + M, j + N] = (byte)0;
                    }
                }
            }
            pic.redPixels = filter.unreplicateImage(WinSize, WinSize, height, width, NewPicR);
            pic.greenPixels = filter.unreplicateImage(WinSize, WinSize, height, width, NewPicG);
            pic.bluePixels = filter.unreplicateImage(WinSize, WinSize, height, width, NewPicB);
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Morphology
        public void IMorphology(int height, int width, byte[,] tempRPixelArray, byte[,] tempGPixelArray, byte[,] tempBPixelArray, ref byte[,] modifiedRPixelArray, ref byte[,] modifiedGPixelArray, ref byte[,] modifiedBPixelArray, int[,] StructerElement, int type, int IOrigin, int JOrigin, int widthSE, int heightSE)
        {
            #region Pre-processing
            Filter Replecation = new Filter(); //da 3ashan el padding :D
            //pad the image
            byte[,] TempR = Replecation.ReplicateImage(heightSE, widthSE, height, width, tempRPixelArray);
            byte[,] TempG = Replecation.ReplicateImage(heightSE, widthSE, height, width, tempGPixelArray);
            byte[,] TempB = Replecation.ReplicateImage(heightSE, widthSE, height, width, tempBPixelArray);
            int newHeight = height + heightSE;
            int newWidth = width + widthSE;
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];
            #endregion

            #region Dilation
            if (type == 0) //Dilation
            {
                //reflect SE and find the new origin :D
                StructerElement = ReflectSE(StructerElement, widthSE, heightSE, ref IOrigin, ref JOrigin);
                //convolut with SE   
                bool flag = false;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        flag = false;
                        for (int c = 0; c < heightSE; c++)
                        {
                            for (int k = 0; k < widthSE; k++)
                            {
                                if (StructerElement[c, k] == 1 && TempR[i + c, j + k] == 255)
                                {
                                    NewPicR[i + IOrigin, j + JOrigin] = 255;
                                    NewPicG[i + IOrigin, j + JOrigin] = 255;
                                    NewPicB[i + IOrigin, j + JOrigin] = 255;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                }
            }
            #endregion

            #region Erosion
            else if (type == 1) //Erosion
            {
                bool flag = false;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        flag = false;
                        for (int c = 0; c < heightSE; c++)
                        {
                            for (int k = 0; k < widthSE; k++)
                            {
                                if (StructerElement[c, k] == 1 && TempR[i + c, j + k] == 0)
                                {
                                    NewPicR[i + IOrigin, j + JOrigin] = 0;
                                    NewPicG[i + IOrigin, j + JOrigin] = 0;
                                    NewPicB[i + IOrigin, j + JOrigin] = 0;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                            NewPicR[i + IOrigin, j + JOrigin] = 255;
                            NewPicG[i + IOrigin, j + JOrigin] = 255;
                            NewPicB[i + IOrigin, j + JOrigin] = 255;
                        }
                    }
                }
            }
            #endregion

            #region Opening
            else if (type == 2)
            {
                bool flag = false;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        flag = false;
                        for (int c = 0; c < heightSE; c++)
                        {
                            for (int k = 0; k < widthSE; k++)
                            {
                                if (StructerElement[c, k] == 1 && TempR[i + c, j + k] == 0)
                                {
                                    NewPicR[i + IOrigin, j + JOrigin] = 0;
                                    NewPicG[i + IOrigin, j + JOrigin] = 0;
                                    NewPicB[i + IOrigin, j + JOrigin] = 0;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                            NewPicR[i + IOrigin, j + JOrigin] = 255;
                            NewPicG[i + IOrigin, j + JOrigin] = 255;
                            NewPicB[i + IOrigin, j + JOrigin] = 255;
                        }
                    }
                }
                //un-replicate
                byte[,] TNewPicR = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicR);
                byte[,] TNewPicG = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicG);
                byte[,] TNewPicB = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicB);
                TNewPicR = Replecation.ReplicateImage(heightSE, widthSE, height, width, TNewPicR);
                TNewPicG = Replecation.ReplicateImage(heightSE, widthSE, height, width, TNewPicG);
                TNewPicB = Replecation.ReplicateImage(heightSE, widthSE, height, width, TNewPicB);
                //byte[,] TNewPicR = NewPicR;
                //byte[,] TNewPicG = NewPicG;
                //byte[,] TNewPicB = NewPicB;
                //dilate
                StructerElement = ReflectSE(StructerElement, widthSE, heightSE, ref IOrigin, ref JOrigin);
                flag = false;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        flag = false;
                        for (int c = 0; c < heightSE; c++)
                        {
                            for (int k = 0; k < widthSE; k++)
                            {
                                if (StructerElement[c, k] == 1 && TNewPicR[i + c, j + k] == 255)
                                {
                                    NewPicR[i + IOrigin, j + JOrigin] = 255;
                                    NewPicG[i + IOrigin, j + JOrigin] = 255;
                                    NewPicB[i + IOrigin, j + JOrigin] = 255;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                }
            }
            #endregion

            #region Closing
            else if (type == 3)
            {
                //dilate
                int NewI = IOrigin, NewJ = JOrigin;
                int[,] NewSE = ReflectSE(StructerElement, widthSE, heightSE, ref IOrigin, ref JOrigin);
                bool flag = false;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        flag = false;
                        for (int c = 0; c < heightSE; c++)
                        {
                            for (int k = 0; k < widthSE; k++)
                            {
                                if (NewSE[c, k] == 1 && TempR[i + c, j + k] == 255)
                                {
                                    NewPicR[i + IOrigin, j + JOrigin] = 255;
                                    NewPicG[i + IOrigin, j + JOrigin] = 255;
                                    NewPicB[i + IOrigin, j + JOrigin] = 255;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                        }
                    }
                }
                //un-replicate
                byte[,] TNewPicR = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicR);
                byte[,] TNewPicG = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicG);
                byte[,] TNewPicB = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicB);
                TNewPicR = Replecation.ReplicateImage(heightSE, widthSE, height, width, TNewPicR);
                TNewPicG = Replecation.ReplicateImage(heightSE, widthSE, height, width, TNewPicG);
                TNewPicB = Replecation.ReplicateImage(heightSE, widthSE, height, width, TNewPicB);
                //erosion
                flag = false;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        flag = false;
                        for (int c = 0; c < heightSE; c++)
                        {
                            for (int k = 0; k < widthSE; k++)
                            {
                                if (StructerElement[c, k] == 1 && TNewPicR[i + c, j + k] == 0)
                                {
                                    NewPicR[i + NewI, j + NewJ] = 0;
                                    NewPicG[i + NewI, j + NewJ] = 0;
                                    NewPicB[i + NewI, j + NewJ] = 0;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                            NewPicR[i + NewI, j + NewJ] = 255;
                            NewPicG[i + NewI, j + NewJ] = 255;
                            NewPicB[i + NewI, j + NewJ] = 255;
                        }
                    }
                }
            }
            #endregion

            #region Boundary Extraction
            else if (type == 4)
            {
                bool flag = false;
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        flag = false;
                        for (int c = 0; c < heightSE; c++)
                        {
                            for (int k = 0; k < widthSE; k++)
                            {
                                if (StructerElement[c, k] == 1 && TempR[i + c, j + k] == 0)
                                {
                                    NewPicR[i + IOrigin, j + JOrigin] = 0;
                                    NewPicG[i + IOrigin, j + JOrigin] = 0;
                                    NewPicB[i + IOrigin, j + JOrigin] = 0;
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                break;
                            NewPicR[i + IOrigin, j + JOrigin] = 255;
                            NewPicG[i + IOrigin, j + JOrigin] = 255;
                            NewPicB[i + IOrigin, j + JOrigin] = 255;
                        }
                    }
                }
                modifiedRPixelArray = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicR);
                modifiedGPixelArray = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicG);
                modifiedBPixelArray = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicB);
                for (int i = 0; i < height; i++)
                    for (int j = 0; j < width; j++)
                    {
                        modifiedRPixelArray[i, j] = (byte)(tempRPixelArray[i, j] - modifiedRPixelArray[i, j]);
                        modifiedGPixelArray[i, j] = (byte)(tempGPixelArray[i, j] - modifiedGPixelArray[i, j]);
                        modifiedBPixelArray[i, j] = (byte)(tempBPixelArray[i, j] - modifiedBPixelArray[i, j]);
                    }
                return;
            }
            #endregion

            #region post-processing
            //un-pad it :D
            modifiedRPixelArray = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicR);
            modifiedGPixelArray = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicG);
            modifiedBPixelArray = Replecation.unreplicateImage(heightSE, widthSE, height, width, NewPicB);
            #endregion
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Matlab Task
        public void LocalStat(PictureInfo pic, double K0, double K1, double K2, double E, int WindowSize)
        {
            MatlabClass MatLabFn = new MatlabClass();
            MWArray[] NewR = (MWArray[])(MatLabFn.MLocalStat(1, (MWNumericArray)pic.redPixels, WindowSize, E, K0, K1, K2));
            MWArray[] NewG = (MWArray[])(MatLabFn.MLocalStat(1, (MWNumericArray)pic.greenPixels, WindowSize, E, K0, K1, K2));
            MWArray[] NewB = (MWArray[])(MatLabFn.MLocalStat(1, (MWNumericArray)pic.bluePixels, WindowSize, E, K0, K1, K2));
            pic.redPixels = (byte[,])((MWNumericArray)(NewR[0])).ToArray(MWArrayComponent.Real);
            pic.greenPixels = (byte[,])((MWNumericArray)(NewG[0])).ToArray(MWArrayComponent.Real);
            pic.bluePixels = (byte[,])((MWNumericArray)(NewB[0])).ToArray(MWArrayComponent.Real);
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
    }
}