using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageProcessingAssignment1
{
    public class PictureInfo
    {
        public int width;
        public int height;
        public string name;
        public string path;
        public PictureBox pictureBox;
        public byte[,] redPixels;
        public byte[,] greenPixels;
        public byte[,] bluePixels;
        public double[,] redReal;
        public double[,] greenReal;
        public double[,] blueReal;
        public double[,] redImag;
        public double[,] greenImag;
        public double[,] blueImag;
        public bool frequency;
        public PictureInfo()
        {
            frequency = false;
        }
        public PictureInfo(int _width, int _height, string _name, string _path, PictureBox _picBox, byte[,] _redPixels, byte[,] _greenPixels, byte[,] _bluePixels)
        {
            width = _width;
            height = _height;
            name = _name;
            path = _path;
            pictureBox = _picBox;
            redPixels = _redPixels;
            greenPixels = _greenPixels;
            bluePixels = _bluePixels;
            frequency = false;
        }
        public PictureInfo(PictureInfo pic)
        {
            width = pic.width;
            height = pic.height;
            name = pic.name;
            pictureBox = pic.pictureBox;
            redPixels = new byte[height, width];
            greenPixels = new byte[height, width];
            bluePixels = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    redPixels[i, j] = pic.redPixels[i, j];
                    greenPixels[i, j] = pic.greenPixels[i, j];
                    bluePixels[i, j] = pic.bluePixels[i, j];
                }
            }
            frequency = false;
        }
    }
}