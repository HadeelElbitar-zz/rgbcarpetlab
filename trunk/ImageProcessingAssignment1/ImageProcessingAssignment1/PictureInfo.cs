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
        public PictureInfo(int _width, int _height, string _name, PictureBox _picBox, byte[,] _redPixels, byte[,] _greenPixels, byte[,] _bluePixels)
        {
            width = _width;
            height = _height;
            name = _name;
            pictureBox = _picBox;
            redPixels = _redPixels;
            greenPixels = _greenPixels;
            bluePixels = _bluePixels;
            frequency = false;
        }
    }
}