using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ImageProcessingAssignment1
{
    public class Pixel
    {
        private byte R;
        private byte G;
        private byte B;
        public Pixel(int r, int g, int b)
        {
            R = (byte)r;
            G = (byte)g;
            B = (byte)b;
        }
        public Pixel(byte r, byte g, byte b)
        {
            R = r;
            G = g;
            B = b;
        }
        public byte GetR()
        {
            return this.R;
        }
        public byte GetG()
        {
            return this.G;
        }
        public byte GetB()
        {
            return this.B;
        }
    }
}
