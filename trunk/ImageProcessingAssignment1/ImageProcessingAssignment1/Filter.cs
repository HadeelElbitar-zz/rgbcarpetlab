using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathWorks.MATLAB.NET.Utility;
using MathWorks.MATLAB.NET.Arrays;
using MatlabProject;
using System.Collections;

namespace ImageProcessingAssignment1
{
    class Filter
    {
        private byte[,] ReplicateImage(int Fheight, int Fwidth, int height, int width, byte[,] Array)
        {
            int N = (Fheight - 1) / 2, M = (Fwidth - 1) / 2;
            int newHeight = height + Fheight - 1;
            int newWidth = width + Fwidth - 1;
            byte[,] returnArray = new byte[newHeight, newWidth];
            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth ; j++)
                {
                    if (i <= N)
                    {
                        if (j <= M) returnArray[i, j] = Array[0, 0];
                        else if (j < newWidth - M) returnArray[i, j] = Array[0, j - M];
                        else if (j < newWidth) returnArray[i, j] = Array[0, width - 1];
                    }
                    else if (i < newHeight - N)
                    {
                        if (j <= M) returnArray[i, j] = Array[i - N, 0];
                        else if (j < newWidth - M) returnArray[i, j] = Array[i - N, j - M];
                        else if (j < newWidth) returnArray[i, j] = Array[i - N, width - 1];
                    }
                    else if (i < newHeight)
                    {
                        if (j <= M) returnArray[i, j] = Array[height - 1, 0];
                        else if (j < newWidth - M) returnArray[i, j] = Array[height - 1, j - M];
                        else if (j < newWidth) returnArray[i, j] = Array[height - 1, width - 1];
                    }
                }
            }
            return returnArray;
        }
        private byte[,] unreplicateImage(int Fheight, int Fwidth, int height, int width, byte[,] repArray)
        {
            byte[,] unrepArray = new byte[height, width];
            int N = (Fheight - 1) / 2, M = (Fwidth - 1) / 2;
            for (int i = N; i < N + height; i++)
            {
                for (int j = M; j < M + width; j++)
                {
                    unrepArray[i - N, j - M] = repArray[i, j];
                }
            }
            return unrepArray;
        }
        private double Summation(double[,] filter, int w, int h)
        {
            double sum = 0;
            for (int i = 0; i < h; i++)
                for (int j = 0; j < w; j++)
                    sum += filter[i, j];
            return sum;
        }
        private double CutOff(double Value)
        {
            if (Value > 255) return 255;
            else if (Value < 0) return 0;
            else return Value;
        }
        private byte[,] Normalize(double[,] Value, int H, int W)
        {
            byte[,] res = new byte[H, W];
            double OldMin = double.MaxValue, OldMax = double.MinValue;
            double NewMin = 0, NewMax = 255;
            GetMinMax(Value, W, H, ref OldMin, ref OldMax);
            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                    res[i, j] = (byte)(((Value[i, j] - OldMin) / (OldMax - OldMin)) * ((NewMax - NewMin) + NewMin));
            return res;
        }
        private void GetMinMax(byte[,] A, int w, int h, ref int min, ref int max)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (A[i, j] > max) max = A[i, j];
                    else if (A[i, j] < min) min = A[i, j];
                }
            }
        }
        private void GetMinMax(double[,] A, int w, int h, ref double min, ref double max)
        {
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    if (A[i, j] > max) max = (int)A[i, j];
                    else if (A[i, j] < min) min = (int)A[i, j];
                }
            }
        }
        public void Apply1DFilter(int length, double[] Filter, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue)
        {
            int height = OldPic.height, width = OldPic.width;
            int newHeight = height + length - 1;
            int newWidth = width + length - 1;
            byte[,] repRPixels = new byte[newHeight, newWidth];//Replicated Borders
            byte[,] repGPixels = new byte[newHeight, newWidth];
            byte[,] repBPixels = new byte[newHeight, newWidth];
            repRPixels = ReplicateImage(length, length, height, width, OldPic.redPixels);
            repGPixels = ReplicateImage(length, length, height, width, OldPic.greenPixels);
            repBPixels = ReplicateImage(length, length, height, width, OldPic.bluePixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];
            double[,] TNewPicR = new double[newHeight, newWidth];
            double[,] TNewPicG = new double[newHeight, newWidth];
            double[,] TNewPicB = new double[newHeight, newWidth];
            int N = (length - 1) / 2;

            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth - length + 1; j++)
                {
                    double Rsum = 0;
                    double Gsum = 0;
                    double Bsum = 0;
                    for (int k = 0; k < length; k++)
                    {
                        Rsum += (Filter[k] * repRPixels[i, j + k]);
                        Gsum += (Filter[k] * repGPixels[i, j + k]);
                        Bsum += (Filter[k] * repBPixels[i, j + k]);
                    }
                    TNewPicR[i, j + N] = (byte)Rsum;
                    TNewPicG[i, j + N] = (byte)Gsum;
                    TNewPicB[i, j + N] = (byte)Bsum;
                }
            }
            for (int i = 0; i < newHeight - length + 1; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    double Rsum = 0;
                    double Gsum = 0;
                    double Bsum = 0;
                    for (int k = 0; k < length; k++)
                    {
                        Rsum += (Filter[k] * TNewPicR[i + k, j]);
                        Gsum += (Filter[k] * TNewPicG[i + k, j]);
                        Bsum += (Filter[k] * TNewPicB[i + k, j]);
                    }
                    NewPicR[i + N, j] = (byte)Rsum;
                    NewPicG[i + N, j] = (byte)Gsum;
                    NewPicB[i + N, j] = (byte)Bsum;
                }
            }
            Red = unreplicateImage(length, length, height, width, NewPicR);
            Green = unreplicateImage(length, length, height, width, NewPicG);
            Blue = unreplicateImage(length, length, height, width, NewPicB);
        }
        public void Apply2DFilter(int Fwidth, int Fheight, double[,] Filter, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue)
        {
            int height = OldPic.height, width = OldPic.width;
            double SumOfFilter = Summation(Filter, Fwidth, Fheight);
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = new byte[newHeight, newWidth];//Replicated Borders
            byte[,] repGPixels = new byte[newHeight, newWidth];
            byte[,] repBPixels = new byte[newHeight, newWidth];
            repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];
            double[,] TNewPicR = new double[newHeight, newWidth];
            double[,] TNewPicG = new double[newHeight, newWidth];
            double[,] TNewPicB = new double[newHeight, newWidth];
            int M = (Fheight - 1) / 2, N = (Fwidth - 1) / 2, Fsize = Fheight * Fwidth;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double Rsum = 0;
                    double Gsum = 0;
                    double Bsum = 0;
                    for (int c = 0; c < Fheight; c++)
                    {
                        for (int k = 0; k < Fwidth; k++)
                        {
                            Rsum += (Filter[c, k] * (double)repRPixels[i + c, j + k]);
                            Gsum += (Filter[c, k] * (double)repGPixels[i + c, j + k]);
                            Bsum += (Filter[c, k] * (double)repBPixels[i + c, j + k]);
                        }
                    }
                    //Rsum /= Fsize;
                    //Gsum /= Fsize;
                    //Bsum /= Fsize;
                    if ((int)(SumOfFilter) == 1)
                    {
                        Rsum = CutOff(Rsum);
                        Gsum = CutOff(Gsum);
                        Bsum = CutOff(Bsum);
                        NewPicR[i + M, j + N] = (byte)Rsum;
                        NewPicG[i + M, j + N] = (byte)Gsum;
                        NewPicB[i + M, j + N] = (byte)Bsum;
                    }
                    else
                    {
                        TNewPicR[i + M, j + N] = Rsum;
                        TNewPicG[i + M, j + N] = Gsum;
                        TNewPicB[i + M, j + N] = Bsum;
                    }
                }
            }
            if (SumOfFilter == 0)
            {
                NewPicR = Normalize(TNewPicR, newHeight, newWidth);
                NewPicG = Normalize(TNewPicG, newHeight, newWidth);
                NewPicB = Normalize(TNewPicB, newHeight, newWidth);
            }
            Red = unreplicateImage(Fheight, Fwidth, height, width, NewPicR);
            Green = unreplicateImage(Fheight, Fwidth, height, width, NewPicG);
            Blue = unreplicateImage(Fheight, Fwidth, height, width, NewPicB);
        }
        public void LowPassFilters(PictureInfo pic, int filterType, double D, double N)
        {
            int height = pic.height;
            int width = pic.width;
            MatlabClass matlabClass = new MatlabClass();
            byte[,] NewR = new byte[height, width];
            byte[,] NewG = new byte[height, width];
            byte[,] NewB = new byte[height, width];
            
            MWArray[] MNewR = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.redPixels));
            MWArray[] MNewG = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.greenPixels));
            MWArray[] MNewB = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.bluePixels));
            pic.redReal = (double[,])((MWNumericArray)(MNewR[0])).ToArray(MWArrayComponent.Real);
            pic.greenReal = (double[,])((MWNumericArray)(MNewG[0])).ToArray(MWArrayComponent.Real);
            pic.blueReal = (double[,])((MWNumericArray)(MNewB[0])).ToArray(MWArrayComponent.Real);
            pic.redImag = (double[,])((MWNumericArray)(MNewR[1])).ToArray(MWArrayComponent.Real);
            pic.greenImag = (double[,])((MWNumericArray)(MNewG[1])).ToArray(MWArrayComponent.Real);
            pic.blueImag = (double[,])((MWNumericArray)(MNewB[1])).ToArray(MWArrayComponent.Real);
            double[,] Filter = new double[height, width];

            switch (filterType)
            {
                case 0:
                    //D = 160;
                    //NewR = (double[,])((MWNumericArray)(matlabClass.IdealLFP((MWNumericArray)pic.redPixels, D))).ToArray(MWArrayComponent.Real);
                    //NewG = (double[,])((MWNumericArray)(matlabClass.IdealLFP((MWNumericArray)pic.greenPixels, D))).ToArray(MWArrayComponent.Real);
                    //NewB = (double[,])((MWNumericArray)(matlabClass.IdealLFP((MWNumericArray)pic.bluePixels, D))).ToArray(MWArrayComponent.Real);
                    double temp;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            //Temp = sqrt((i - (W / 2)) ^ 2 + (j - (H / 2)) ^ 2);
                            temp = Math.Sqrt(Math.Pow((i - width / 2), 2) + Math.Pow((j - height / 2), 2));
                            if (temp <= D)
                                Filter[i, j] = 1;
                            pic.redReal[i, j] *= Filter[i, j];
                            pic.greenReal[i, j] *= Filter[i, j];
                            pic.blueReal[i, j] *= Filter[i, j];
                            pic.redImag[i, j] *= Filter[i, j];
                            pic.greenImag[i, j] *= Filter[i, j];
                            pic.blueImag[i, j] *= Filter[i, j];
                        }
                    }
                    break;
                case 1:
                    //N = 1;
                    //D = 30;
                    //NewR = (double[,])((MWNumericArray)(matlabClass.ButterWorthLPF((MWNumericArray)pic.redPixels, D, N))).ToArray(MWArrayComponent.Real);
                    //NewG = (double[,])((MWNumericArray)(matlabClass.ButterWorthLPF((MWNumericArray)pic.greenPixels, D, N))).ToArray(MWArrayComponent.Real);
                    //NewB = (double[,])((MWNumericArray)(matlabClass.ButterWorthLPF((MWNumericArray)pic.bluePixels, D, N))).ToArray(MWArrayComponent.Real);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            //Temp = sqrt((i - (W / 2)) ^ 2 + (j - (H / 2)) ^ 2);
                            //Temp = Temp / D;
                            //power = 2 * N;
                            //Temp = Temp ^ power;
                            //Temp = Temp + 1;
                            //Filter(i, j) = 1 / Temp;
                            temp = Math.Sqrt(Math.Pow((i - width / 2), 2) + Math.Pow((j - height / 2), 2));
                            temp /= D;
                            temp = Math.Pow(temp, 2 * N);
                            temp++;
                            Filter[i, j] = 1 / temp;
                            pic.redReal[i, j] *= Filter[i, j];
                            pic.greenReal[i, j] *= Filter[i, j];
                            pic.blueReal[i, j] *= Filter[i, j];
                            pic.redImag[i, j] *= Filter[i, j];
                            pic.greenImag[i, j] *= Filter[i, j];
                            pic.blueImag[i, j] *= Filter[i, j];
                        }
                    }
                    break;
                case 2:
                    //D = 100;
                    //NewR = (double[,])((MWNumericArray)(matlabClass.GaussianLPF((MWNumericArray)pic.redPixels, D))).ToArray(MWArrayComponent.Real);
                    //NewG = (double[,])((MWNumericArray)(matlabClass.GaussianLPF((MWNumericArray)pic.greenPixels, D))).ToArray(MWArrayComponent.Real);
                    //NewB = (double[,])((MWNumericArray)(matlabClass.GaussianLPF((MWNumericArray)pic.bluePixels, D))).ToArray(MWArrayComponent.Real);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            //Temp = sqrt((i - (W / 2)) ^ 2 + (j - (H / 2)) ^ 2);
                            //Temp = Temp ^ 2;
                            //N = D ^ 2;
                            //N = N * 2;
                            //Temp = Temp / N;
                            //Temp = -1 * Temp;
                            //Filter(i, j) = exp(Temp);
                            temp = Math.Sqrt(Math.Pow((i - width / 2), 2) + Math.Pow((j - height / 2), 2));
                            temp = Math.Pow(temp, 2);
                            double temp2 = Math.Pow(D, 2) * 2;
                            temp /= temp2;
                            temp *= -1;
                            Filter[i, j] = Math.Exp(temp);
                            pic.redReal[i, j] *= Filter[i, j];
                            pic.greenReal[i, j] *= Filter[i, j];
                            pic.blueReal[i, j] *= Filter[i, j];
                            pic.redImag[i, j] *= Filter[i, j];
                            pic.greenImag[i, j] *= Filter[i, j];
                            pic.blueImag[i, j] *= Filter[i, j];
                        }
                    }
                    break;
            }
            double[,] NMNewR = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.redReal, (MWNumericArray)pic.redImag))).ToArray(MWArrayComponent.Real);
            double[,] NMNewG = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.greenReal, (MWNumericArray)pic.greenImag))).ToArray(MWArrayComponent.Real);
            double[,] NMNewB = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.blueReal, (MWNumericArray)pic.blueImag))).ToArray(MWArrayComponent.Real);
            NewR = Normalize(NMNewR, height, width);
            NewG = Normalize(NMNewG, height, width);
            NewB = Normalize(NMNewB, height, width);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pic.redPixels[i, j] = NewR[i, j];
                    pic.greenPixels[i, j] = NewG[i, j];
                    pic.bluePixels[i, j] = NewB[i, j];
                }
            }
        }
        public void HighPassFilters(PictureInfo pic, int filterType, double D, double N)
        {
            int height = pic.height;
            int width = pic.width;
            MatlabClass matlabClass = new MatlabClass();
            byte[,] NewR = new byte[height, width];
            byte[,] NewG = new byte[height, width];
            byte[,] NewB = new byte[height, width];

            MWArray[] MNewR = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.redPixels));
            MWArray[] MNewG = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.greenPixels));
            MWArray[] MNewB = (MWArray[])(matlabClass.ConvertToFrequencyDomain(2, (MWNumericArray)pic.bluePixels));
            pic.redReal = (double[,])((MWNumericArray)(MNewR[0])).ToArray(MWArrayComponent.Real);
            pic.greenReal = (double[,])((MWNumericArray)(MNewG[0])).ToArray(MWArrayComponent.Real);
            pic.blueReal = (double[,])((MWNumericArray)(MNewB[0])).ToArray(MWArrayComponent.Real);
            pic.redImag = (double[,])((MWNumericArray)(MNewR[1])).ToArray(MWArrayComponent.Real);
            pic.greenImag = (double[,])((MWNumericArray)(MNewG[1])).ToArray(MWArrayComponent.Real);
            pic.blueImag = (double[,])((MWNumericArray)(MNewB[1])).ToArray(MWArrayComponent.Real);
            double[,] Filter = new double[height, width];

            switch (filterType)
            {
                case 0:
                    //double D = 80;
                    //NewR = (double[,])((MWNumericArray)(matlabClass.IdealHPF((MWNumericArray)pic.redPixels, D))).ToArray(MWArrayComponent.Real);
                    //NewG = (double[,])((MWNumericArray)(matlabClass.IdealHPF((MWNumericArray)pic.greenPixels, D))).ToArray(MWArrayComponent.Real);
                    //NewB = (double[,])((MWNumericArray)(matlabClass.IdealHPF((MWNumericArray)pic.bluePixels, D))).ToArray(MWArrayComponent.Real);
                    double temp;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            //Temp = sqrt((i - (W / 2)) ^ 2 + (j - (H / 2)) ^ 2);
                            temp = Math.Sqrt(Math.Pow((i - width / 2), 2) + Math.Pow((j - height / 2), 2));
                            if (temp > D)
                                Filter[i, j] = 1;
                            pic.redReal[i, j] *= Filter[i, j];
                            pic.greenReal[i, j] *= Filter[i, j];
                            pic.blueReal[i, j] *= Filter[i, j];
                            pic.redImag[i, j] *= Filter[i, j];
                            pic.greenImag[i, j] *= Filter[i, j];
                            pic.blueImag[i, j] *= Filter[i, j];
                        }
                    }
                    break;
                case 1:
                    //double N = 2;
                    //D = 160;
                    //NewR = (double[,])((MWNumericArray)(matlabClass.ButterWorthHPF((MWNumericArray)pic.redPixels, D, N))).ToArray(MWArrayComponent.Real);
                    //NewG = (double[,])((MWNumericArray)(matlabClass.ButterWorthHPF((MWNumericArray)pic.greenPixels, D, N))).ToArray(MWArrayComponent.Real);
                    //NewB = (double[,])((MWNumericArray)(matlabClass.ButterWorthHPF((MWNumericArray)pic.bluePixels, D, N))).ToArray(MWArrayComponent.Real);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            //Temp = sqrt((i - (W / 2)) ^ 2 + (j - (H / 2)) ^ 2);
                            //Temp = D / Temp;
                            //power = 2 * N;
                            //Temp = Temp ^ power;
                            //Temp = Temp + 1;
                            //Filter(i, j) = 1 / Temp;
                            temp = Math.Sqrt(Math.Pow((i - width / 2), 2) + Math.Pow((j - height / 2), 2));
                            temp = D / temp;
                            temp = Math.Pow(temp, 2 * N);
                            temp++;
                            Filter[i, j] = 1 / temp;
                            pic.redReal[i, j] *= Filter[i, j];
                            pic.greenReal[i, j] *= Filter[i, j];
                            pic.blueReal[i, j] *= Filter[i, j];
                            pic.redImag[i, j] *= Filter[i, j];
                            pic.greenImag[i, j] *= Filter[i, j];
                            pic.blueImag[i, j] *= Filter[i, j];
                        }
                    }
                    break;
                case 2:
                    //D = 60;
                    //NewR = (double[,])((MWNumericArray)(matlabClass.GaussianHPF((MWNumericArray)pic.redPixels, D))).ToArray(MWArrayComponent.Real);
                    //NewG = (double[,])((MWNumericArray)(matlabClass.GaussianHPF((MWNumericArray)pic.greenPixels, D))).ToArray(MWArrayComponent.Real);
                    //NewB = (double[,])((MWNumericArray)(matlabClass.GaussianHPF((MWNumericArray)pic.bluePixels, D))).ToArray(MWArrayComponent.Real);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            //Temp = sqrt((i - (W / 2)) ^ 2 + (j - (H / 2)) ^ 2);
                            //Temp = Temp ^ 2;
                            //N = D ^ 2;
                            //N = N * 2;
                            //Temp = Temp / N;
                            //Temp = -1 * Temp;
                            //Filter(i, j) = 1 - exp(Temp);
                            temp = Math.Sqrt(Math.Pow((i - width / 2), 2) + Math.Pow((j - height / 2), 2));
                            temp = Math.Pow(temp, 2);
                            double temp2 = Math.Pow(D, 2) * 2;
                            temp /= temp2;
                            temp *= -1;
                            Filter[i, j] = 1 - Math.Exp(temp);
                            pic.redReal[i, j] *= Filter[i, j];
                            pic.greenReal[i, j] *= Filter[i, j];
                            pic.blueReal[i, j] *= Filter[i, j];
                            pic.redImag[i, j] *= Filter[i, j];
                            pic.greenImag[i, j] *= Filter[i, j];
                            pic.blueImag[i, j] *= Filter[i, j];
                        }
                    }
                    break;
            }
            double[,] NMNewR = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.redReal, (MWNumericArray)pic.redImag))).ToArray(MWArrayComponent.Real);
            double[,] NMNewG = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.greenReal, (MWNumericArray)pic.greenImag))).ToArray(MWArrayComponent.Real);
            double[,] NMNewB = (double[,])((MWNumericArray)(matlabClass.ConvertToSpatial((MWNumericArray)pic.blueReal, (MWNumericArray)pic.blueImag))).ToArray(MWArrayComponent.Real);
            NewR = Normalize(NMNewR, height, width);
            NewG = Normalize(NMNewG, height, width);
            NewB = Normalize(NMNewB, height, width);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    pic.redPixels[i, j] = NewR[i, j];
                    pic.greenPixels[i, j] = NewG[i, j];
                    pic.bluePixels[i, j] = NewB[i, j];
                }
            }
        }
        public void Apply2DGMeanFilter(int Fwidth, int Fheight, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue)
        {
            int height = OldPic.height, width = OldPic.width;
            double Power = (double)(1 / (double)((double)Fwidth * (double)Fheight));
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = new byte[newHeight, newWidth];//Replicated Borders
            byte[,] repGPixels = new byte[newHeight, newWidth];
            byte[,] repBPixels = new byte[newHeight, newWidth];
            repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];

            int M = (Fheight - 1) / 2, N = (Fwidth - 1) / 2;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double Rmul = 1;
                    double Gmul = 1;
                    double Bmul = 1;
                    for (int c = 0; c < Fheight; c++)
                    {
                        for (int k = 0; k < Fwidth; k++)
                        {
                            Rmul *= (double)repRPixels[i + c, j + k];
                            Gmul *= (double)repGPixels[i + c, j + k];
                            Bmul *= (double)repBPixels[i + c, j + k];
                        }
                    }

                    NewPicR[i + M, j + N] = (byte)(Math.Pow(Rmul, Power));
                    NewPicG[i + M, j + N] = (byte)(Math.Pow(Gmul, Power));
                    NewPicB[i + M, j + N] = (byte)(Math.Pow(Bmul, Power));
                }
            }
            
            Red = unreplicateImage(Fheight, Fwidth, height, width, NewPicR);
            Green = unreplicateImage(Fheight, Fwidth, height, width, NewPicG);
            Blue = unreplicateImage(Fheight, Fwidth, height, width, NewPicB);
        }
        public void ApplyOrderStatFilter(int Fwidth, int Fheight, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue, string Filter)
        {
            int height = OldPic.height, width = OldPic.width;
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = new byte[newHeight, newWidth];//Replicated Borders
            byte[,] repGPixels = new byte[newHeight, newWidth];
            byte[,] repBPixels = new byte[newHeight, newWidth];
            repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];

            int M = (Fheight - 1) / 2, N = (Fwidth - 1) / 2, FSize = Fheight * Fwidth;
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte[] R = new byte[FSize];
                    byte[] G = new byte[FSize];
                    byte[] B = new byte[FSize];
                    int F = 0;
                    for (int c = 0; c < Fheight; c++)
                    {
                        for (int k = 0; k < Fwidth; k++)
                        {
                            R[F] = repRPixels[i + c, j + k];
                            G[F] = repGPixels[i + c, j + k];
                            B[F++] = repBPixels[i + c, j + k];
                        }
                    }
                   #region sorting 
                   // // int size = R.Length;
                   // Dictionary<int, int> L = new Dictionary<int, int>();
                   // for (int T = 0; T < FSize; T++)
                   // {
                   //     int res;
                   //     string RT = Convert.ToString((int)R[T], 2), GT = Convert.ToString((int)G[T], 2), BT = Convert.ToString((int)B[T], 2);
                   //     string t = "";
                   //     for (int O = 0; O < 8; O++)
                   //     {
                   //         try
                   //         {
                   //             t += RT[O].ToString();
                   //         }
                   //         catch
                   //         {
                   //             t += "0";
                   //         }
                   //         try
                   //         {
                   //             t += GT[O].ToString();
                   //         }
                   //         catch
                   //         {
                   //             t += "0";
                   //         }
                   //         try
                   //         {
                   //             t += BT[O].ToString();
                   //         }
                   //         catch
                   //         {
                   //             t += "0";
                   //         }
                   //     }
                   //     res = Convert.ToInt32(t, 2);
                   //     L.Add(T, res);
                   // }
                   // List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>(L);
                   // result.Sort(delegate(KeyValuePair<int, int> first, KeyValuePair<int, int> second)
                   // {
                   //     return second.Value.CompareTo(first.Value);
                   // });
                   // byte[] RTemp = new byte[FSize];
                   // byte[] GTemp = new byte[FSize];
                   // byte[] BTemp = new byte[FSize];
                   // int S = 0;
                   // foreach (KeyValuePair<int, int> item in result)
                   // {
                   //     RTemp[S] = R[item.Key];
                   //     GTemp[S] = G[item.Key];
                   //     BTemp[S] = B[item.Key];
                   //     S++;
                   // }
                   // R = RTemp;
                   // G = GTemp;
                   // B = BTemp;
                    #endregion
                    PrallelSort(ref R, ref G, ref B);
                    byte Rval = 0, Gval = 0, Bval = 0;
                    int index;
                    switch (Filter)
                    {
                        case "Median":
                            {
                                index = FSize / 2;
                                Rval = R[index];
                                Gval = G[index];
                                Bval = B[index];
                                break;
                            }
                        case "Maximum":
                            {
                                index = 0;
                                Rval = R[index];
                                Gval = G[index];
                                Bval = B[index];
                                break;
                            }
                        case "Minimum":
                            {
                                index = FSize - 1;
                                Rval = R[index];
                                Gval = G[index];
                                Bval = B[index];
                                break;
                            }
                        case "MidPoint":
                            {
                                //index = FSize / 2;
                                Rval = (byte)(((double)R[0] + (double)R[FSize - 1]) / 2);
                                Gval = (byte)(((double)G[0] + (double)G[FSize - 1]) / 2);
                                Bval = (byte)(((double)B[0] + (double)B[FSize - 1]) / 2);
                                break;
                            }
                    }
                    NewPicR[i + M, j + N] = Rval;
                    NewPicG[i + M, j + N] = Gval;
                    NewPicB[i + M, j + N] = Bval;
                }
            }

            Red = unreplicateImage(Fheight, Fwidth, height, width, NewPicR);
            Green = unreplicateImage(Fheight, Fwidth, height, width, NewPicG);
            Blue = unreplicateImage(Fheight, Fwidth, height, width, NewPicB);
        }
        private void PrallelSort(ref byte[] R, ref byte[] G, ref byte[] B)
        {
            int size = R.Length;
            Dictionary<int, int> L = new Dictionary<int, int>();
            for (int i = 0; i < size; i++)
            {
                L.Add(i, BitMixed(R[i], G[i], B[i]));
            }
            List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>(L);
            result.Sort(delegate(KeyValuePair<int, int> first, KeyValuePair<int, int> second)
              {
                  return second.Value.CompareTo(first.Value);
              });
            byte[] RTemp = new byte[size];
            byte[] GTemp = new byte[size];
            byte[] BTemp = new byte[size];
            int j = 0;
            foreach (KeyValuePair<int,int> item in result)
            {
                RTemp[j] = R[item.Key];
                GTemp[j] = G[item.Key];
                BTemp[j] = B[item.Key];
                j++;
            }
            R = RTemp;
            G = GTemp;
            B = BTemp;
        }
        //private int BitMixed(byte R, byte G, byte B)
        //{
        //    int res;
        //    string RT = Convert.ToString((int)R, 2), GT = Convert.ToString((int)G, 2), BT = Convert.ToString((int)B, 2);
        //    string t = "";
        //    for (int i = 0; i < 8;i++)
        //    {
        //        try
        //        {
        //            t += RT[i].ToString();
        //        }
        //        catch
        //        {
        //            t += "0";
        //        }
        //        try
        //        {
        //            t += GT[i].ToString();
        //        }
        //        catch
        //        {
        //            t += "0";
        //        }
        //        try
        //        {
        //            t += BT[i].ToString();
        //        }
        //        catch
        //        {
        //            t += "0";
        //        }
        //    }
        //    res = Convert.ToInt32(t, 2);
        //    return res;
        //}
        public void Apply2DContraharmonicFilter(int Fwidth, int Fheight, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue , double Q)
        {
            int height = OldPic.height, width = OldPic.width;
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = new byte[newHeight, newWidth];//Replicated Borders
            byte[,] repGPixels = new byte[newHeight, newWidth];
            byte[,] repBPixels = new byte[newHeight, newWidth];
            repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];

            int M = (Fheight - 1) / 2, N = (Fwidth - 1) / 2;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    double Rsum = 0;
                    double Gsum = 0;
                    double Bsum = 0;
                    double Rsum2 = 0;
                    double Gsum2 = 0;
                    double Bsum2 = 0;
                    for (int c = 0; c < Fheight; c++)
                    {
                        for (int k = 0; k < Fwidth; k++)
                        {
                            Rsum += Math.Pow((double)repRPixels[i + c, j + k], Q + 1);
                            Gsum += Math.Pow((double)repGPixels[i + c, j + k], Q + 1);
                            Bsum += Math.Pow((double)repBPixels[i + c, j + k], Q + 1);
                            Rsum2 += Math.Pow((double)repRPixels[i + c, j + k], Q);
                            Gsum2 += Math.Pow((double)repGPixels[i + c, j + k], Q);
                            Bsum2 += Math.Pow((double)repBPixels[i + c, j + k], Q);
                        }
                    }

                    NewPicR[i + M, j + N] = (byte)(Rsum / Rsum2);
                    NewPicG[i + M, j + N] = (byte)(Gsum / Gsum2);
                    NewPicB[i + M, j + N] = (byte)(Bsum / Bsum2);
                }
            }

            Red = unreplicateImage(Fheight, Fwidth, height, width, NewPicR);
            Green = unreplicateImage(Fheight, Fwidth, height, width, NewPicG);
            Blue = unreplicateImage(Fheight, Fwidth, height, width, NewPicB);
        }
        public void ApplyAlphaTrimmedFilter(int Fwidth, int Fheight, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue, double D)
        {
            int height = OldPic.height, width = OldPic.width;
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = new byte[newHeight, newWidth];//Replicated Borders
            byte[,] repGPixels = new byte[newHeight, newWidth];
            byte[,] repBPixels = new byte[newHeight, newWidth];
            repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];

            int M = (Fheight - 1) / 2, N = (Fwidth - 1) / 2, FSize = Fheight * Fwidth;
            
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    byte[] R = new byte[FSize];
                    byte[] G = new byte[FSize];
                    byte[] B = new byte[FSize];
                    int F = 0;
                    for (int c = 0; c < Fheight; c++)
                    {
                        for (int k = 0; k < Fwidth; k++)
                        {
                            R[F] = repRPixels[i + c, j + k];
                            G[F] = repGPixels[i + c, j + k];
                            B[F++] = repBPixels[i + c, j + k];
                        }
                    }
                    #region sorting 
                   // int size = R.Length;
                    Dictionary<int, int> L = new Dictionary<int, int>();
                    for (int T = 0; T < FSize; T++)
                    {
                        int res;
                        string RT = Convert.ToString((int)R[T], 2), GT = Convert.ToString((int)G[T], 2), BT = Convert.ToString((int)B[T], 2);
                        string t = "";
                        for (int O = 0; O < 8; O++)
                        {
                            try
                            {
                                t += RT[O].ToString();
                            }
                            catch
                            {
                                t += "0";
                            }
                            try
                            {
                                t += GT[O].ToString();
                            }
                            catch
                            {
                                t += "0";
                            }
                            try
                            {
                                t += BT[O].ToString();
                            }
                            catch
                            {
                                t += "0";
                            }
                        }
                        res = Convert.ToInt32(t, 2);
                        L.Add(T, res);
                    }
                    List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>(L);
                    result.Sort(delegate(KeyValuePair<int, int> first, KeyValuePair<int, int> second)
                    {
                        return second.Value.CompareTo(first.Value);
                    });
                    byte[] RTemp = new byte[FSize];
                    byte[] GTemp = new byte[FSize];
                    byte[] BTemp = new byte[FSize];
                    int S = 0;
                    foreach (KeyValuePair<int, int> item in result)
                    {
                        RTemp[S] = R[item.Key];
                        GTemp[S] = G[item.Key];
                        BTemp[S] = B[item.Key];
                        S++;
                    }
                    R = RTemp;
                    G = GTemp;
                    B = BTemp;
                    #endregion
                    //PrallelSort(R, G, B);
                    double Rval = 0, Gval = 0, Bval = 0;
                    int index = (int)Math.Ceiling(D/2);
                    for (int H =index ; H < FSize - index; H++)
			        {
			            Rval += R[H];
                        Gval += G[H];
                        Bval += B[H];
			        }
                    NewPicR[i + M, j + N] = (byte)(Rval / ((double)FSize - D));
                    NewPicG[i + M, j + N] = (byte)(Gval / ((double)FSize - D));
                    NewPicB[i + M, j + N] = (byte)(Bval / ((double)FSize - D));
                }
            }

            Red = unreplicateImage(Fheight, Fwidth, height, width, NewPicR);
            Green = unreplicateImage(Fheight, Fwidth, height, width, NewPicG);
            Blue = unreplicateImage(Fheight, Fwidth, height, width, NewPicB);
        }
        private int BitMixed(byte R, byte G, byte B)
        {
            int res;
            byte[] TempR = BitConverter.GetBytes((int)R);
            BitArray R_bits = new BitArray(TempR);
            byte[] TempG = BitConverter.GetBytes((int)G);
            BitArray G_bits = new BitArray(TempG);
            byte[] TempB = BitConverter.GetBytes((int)B);
            BitArray B_bits = new BitArray(TempB);
            BitArray Temp = new BitArray(24);

            for (int i = 0, j=0; i < 24;j++)
            {
                Temp[i++] = R_bits[j];
                Temp[i++] = G_bits[j];
                Temp[i++] = B_bits[j];
            }
            //for (int i = 0; i < 24; i++)
            //{
            //    bool x = Temp[i];
            //}
            
            res = ToInt(Temp);
            return res;
        }
        private int ToInt(BitArray bits)
        {
            int res = 0;
            for (int i = 0; i < 24; i++)
            {
                if (bits[i])
                {
                    res += (int)Math.Pow(2, i);
                }
            }
            return res;
        }
    }
}