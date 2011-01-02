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
        #region Replicate & Unreplicate
        public byte[,] ReplicateImage(int Fheight, int Fwidth, int height, int width, byte[,] Array)
        {
            int N = ((Fheight - 1) / 2), M = ((Fwidth - 1) / 2);
            int mEven = M;
            if ((Fwidth % 2) == 0) mEven++;
            int nEven = N;
            if ((Fheight % 2) == 0) nEven++;
            int newHeight = height + Fheight - 1;
            int newWidth = width + Fwidth - 1;
            byte[,] returnArray = new byte[newHeight, newWidth];
            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth; j++)
                {
                    if (i <= N)
                    {
                        if (j <= mEven) returnArray[i, j] = Array[0, 0];
                        else if (j < newWidth - mEven) returnArray[i, j] = Array[0, j - M];
                        else if (j < newWidth) returnArray[i, j] = Array[0, width - 1];
                    }
                    else if (i < newHeight - nEven)
                    {
                        if (j <= mEven) returnArray[i, j] = Array[i - N, 0];
                        else if (j < newWidth - mEven) returnArray[i, j] = Array[i - N, j - M];
                        else if (j < newWidth) returnArray[i, j] = Array[i - N, width - 1];
                    }
                    else if (i < newHeight)
                    {
                        if (j <= mEven) returnArray[i, j] = Array[height - 1, 0];
                        else if (j < newWidth - mEven) returnArray[i, j] = Array[height - 1, j - M];
                        else if (j < newWidth) returnArray[i, j] = Array[height - 1, width - 1];
                    }
                }
            }
            return returnArray;
        }
        public byte[,] unreplicateImage(int Fheight, int Fwidth, int height, int width, byte[,] repArray)
        {
            byte[,] unrepArray = new byte[height, width];
            int N = ((Fheight - 1) / 2), M = ((Fwidth - 1) / 2);
            for (int i = N; i < N + height; i++)
            {
                for (int j = M; j < M + width; j++)
                {
                    unrepArray[i - N, j - M] = repArray[i, j];
                }
            }
            return unrepArray;
        }
        #endregion

        #region Helping Functions
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
        private int BitMixed(byte R, byte G, byte B)
        {
            int[] ArrBits = { 128, 64, 32, 16, 8, 4, 2, 1 };
            int res = 0;
            for (int i = 7, j = 0; i >= 0; i--)
            {
                if ((R & ArrBits[i]) == ArrBits[i])
                    res += (int)Math.Pow(2, j++);
                else
                    j++;
                if ((G & ArrBits[i]) == ArrBits[i])
                    res += (int)Math.Pow(2, j++);
                else
                    j++;
                if ((B & ArrBits[i]) == ArrBits[i])
                    res += (int)Math.Pow(2, j++);
                else
                    j++;
            }
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
        

        #endregion

        #region Sorting
        private int[,] CalcBitMixed(PictureInfo pic)
        {
            int W = pic.width;
            int H = pic.height;
            int[,] BitMixed = new int[H, W];
            int[] ArrBits = { 128, 64, 32, 16, 8, 4, 2, 1 };
            int res;
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    res = 0;
                    for (int k = 7, c = 0; k >= 0; k--)
                    {
                        if ((pic.redPixels[i, j] & ArrBits[k]) == ArrBits[k])
                            res += (int)Math.Pow(2, c++);
                        else
                            c++;
                        if ((pic.greenPixels[i, j] & ArrBits[k]) == ArrBits[k])
                            res += (int)Math.Pow(2, c++);
                        else
                            c++;
                        if ((pic.bluePixels[i, j] & ArrBits[k]) == ArrBits[k])
                            res += (int)Math.Pow(2, c++);
                        else
                            c++;
                    }
                    BitMixed[i, j] = res;
                }
            }
            return BitMixed;
        }
        private void ParallelSort(ref byte[] R, ref byte[] G, ref byte[] B, int Fw, int Fh, int indexI, int indexJ, int[,] BitMixedArray)
        {
            // int size = R.Length;
            Dictionary<int, int> L = new Dictionary<int, int>();
            //int[] ArrBits = { 128, 64, 32, 16, 8, 4, 2, 1 };
            //int index = 0;
            int[] BitMixedHistogram = new int[16777216];
            for (int i = 0; i < Fh; i++)
            {
                for (int j = 0; j < Fw; j++)
                {
                    //L.Add(index++, BitMixedArray[indexI + i, indexJ + j]);
                    BitMixedHistogram[BitMixedArray[indexI + i, indexJ + j]]++;
                }
            }
            //List<KeyValuePair<int, int>> result = new List<KeyValuePair<int, int>>(L);
            //result.Sort(delegate(KeyValuePair<int, int> first, KeyValuePair<int, int> second)
            //{
            //    return second.Value.CompareTo(first.Value);
            //});
            int size = Fw * Fh;
            byte[] RTemp = new byte[size];
            byte[] GTemp = new byte[size];
            byte[] BTemp = new byte[size];
            int MinIndex, MaxIndex, MedianIndix;
            int c = 0;
            while (true)
            {
                if (BitMixedHistogram[c] == 0)
                    c++;
                else
                {
                    MinIndex = BitMixedHistogram[c];
                    break;
                }
            }
            c = 16777215;
            while (true)
            {
                if (BitMixedHistogram[c] == 0)
                    c--;
                else
                {
                    MaxIndex = BitMixedHistogram[c];
                    break;
                }
            }
            c = 0; int MedValue = size / 2 + 1; int count = 0;
            while (count < MedValue)
            {
                count += BitMixedHistogram[c++];
            }
            //foreach (KeyValuePair<int, int> item in result)
            //{
            //    RTemp[c] = R[item.Key];
            //    GTemp[c] = G[item.Key];
            //    BTemp[c] = B[item.Key];
            //    c++;
            //}
            R = RTemp;
            G = GTemp;
            B = BTemp;
        }
        private void GetRGBfromBitMixed(int BitMixedValue, ref byte R, ref byte G, ref byte B)
        {
            R = 0; G = 0; B = 0;
            int[] ArrBits = { 8388608, 4194304, 2097152, 1048576, 524288, 262144, 131072, 65536, 32768, 16384, 8192, 4096, 2048, 1024, 512, 256, 128, 64, 32, 16, 8, 4, 2, 1 };
            for (int i = 23, j = 0; i >= 0; j++)
            {
                if ((BitMixedValue & ArrBits[i]) == ArrBits[i])
                    R += (byte)ArrBits[23 - j];
                i--;
                if ((BitMixedValue & ArrBits[i]) == ArrBits[i])
                    G += (byte)ArrBits[23 - j];
                i--;
                if ((BitMixedValue & ArrBits[i]) == ArrBits[i])
                    B += (byte)ArrBits[23 - j];
                i--;
            }
        }
        private void CountingSort(int[] Array, byte[] R, byte[] G, byte[] B, int ArrayLength, int Max)
        {
            int[] SortedArray = new int[ArrayLength];
            byte[] rSorted = new byte[ArrayLength];
            byte[] gSorted = new byte[ArrayLength];
            byte[] bSorted = new byte[ArrayLength];
            int[] ArrayAux = new int[Max + 1];
            for (int i = 0; i <= Max; i++)
                ArrayAux[i] = 0;
            for (int i = 0; i < ArrayLength; i++)
                ArrayAux[Array[i]]++;
            for (int i = 1; i <= Max; i++)
                ArrayAux[i] += ArrayAux[i - 1];
            for (int i = ArrayLength - 1; i >= 0; i--)
            {
                rSorted[ArrayAux[Array[i]] - 1] = R[i];
                gSorted[ArrayAux[Array[i]] - 1] = G[i];
                bSorted[ArrayAux[Array[i]] - 1] = B[i];
                SortedArray[ArrayAux[Array[i]] - 1] = Array[i];
                ArrayAux[Array[i]]--;
            }

            Array = SortedArray;
            R = rSorted;
            G = gSorted;
            B = bSorted;
        }
        private int QuickSelect(int[] arr, int n)
        {
            int low, high, temp;
            int median;
            int middle, ll, hh;
            low = 0; high = n - 1; median = (low + high) / 2;
            while (true)
            {
                if (high <= low) /* One element only */
                    return arr[median];
                if (high == low + 1)
                { /* Two elements only */
                    if (arr[low] > arr[high])
                    {
                        temp = arr[low];
                        arr[low] = arr[high];
                        arr[high] = temp;
                    }
                    return arr[median];
                }
                /* Find median of low, middle and high items; swap into position low */
                middle = (low + high) / 2;
                if (arr[middle] > arr[high])
                {
                    //swap(arr[middle], arr[high]);
                    temp = arr[middle];
                    arr[middle] = arr[high];
                    arr[high] = temp;
                }
                if (arr[low] > arr[high])
                {
                    //swap(arr[low], arr[high]);
                    temp = arr[low];
                    arr[low] = arr[high];
                    arr[high] = temp;
                }
                if (arr[middle] > arr[low])
                {
                    //swap(arr[middle], arr[low]);
                    temp = arr[middle];
                    arr[middle] = arr[low];
                    arr[low] = temp;
                }
                /* Swap low item (now in position middle) into position (low+1) */
                //swap(arr[middle], arr[low + 1]);
                temp = arr[middle];
                arr[middle] = arr[low + 1];
                arr[low + 1] = temp;
                /* Nibble from each end towards middle, swapping items when stuck */
                ll = low + 1;
                hh = high;
                while (true)
                {
                    do ll++; while (arr[low] > arr[ll]);
                    do hh--; while (arr[hh] > arr[low]);
                    if (hh < ll)
                        break;
                    temp = arr[ll];
                    arr[ll] = arr[hh];
                    arr[hh] = temp;
                    //swap(arr[ll], arr[hh]);
                }
                /* Swap middle item (in position low) back into correct position */
                //swap(arr[low], arr[hh]);
                temp = arr[low];
                arr[low] = arr[hh];
                arr[hh] = temp;
                /* Re-set active partition */
                if (hh <= median)
                    low = ll;
                if (hh >= median)
                    high = hh - 1;
            }
        }
        private void QuickSort(ref int[] Arr, int start, int end)
        {
            if (start == end || start > end)
                return;
            int i = start, temp, pivot = Arr[start];
            for (int j = start + 1; j <= end; j++)
            {
                if (Arr[j] <= pivot)
                {
                    i++;
                    if (i != j)
                    {
                        temp = Arr[i];
                        Arr[i] = Arr[j];
                        Arr[j] = temp;
                    }
                }
            }
            temp = Arr[start];
            Arr[start] = Arr[i];
            //Arr[i] = Arr[start];// <------------- swap 3'alt :P :P :P :P :P
            Arr[i] = temp;
            QuickSort(ref Arr, start, i - 1);
            QuickSort(ref Arr, i + 1, end);
        }
        #endregion

        #region Apply 1D & 2D Filter
        public void Apply1DFilter(string type, int length, double[] Filter1, double[] Filter2, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue)
        {
            int height = OldPic.height, width = OldPic.width;
            int newHeight = height + length - 1;
            int newWidth = width + length - 1;
            byte[,] repRPixels = ReplicateImage(length, length, height, width, OldPic.redPixels);
            byte[,] repGPixels = ReplicateImage(length, length, height, width, OldPic.greenPixels);
            byte[,] repBPixels = ReplicateImage(length, length, height, width, OldPic.bluePixels);
            double[,] NewPicR = new double[newHeight, newWidth];
            double[,] NewPicG = new double[newHeight, newWidth];
            double[,] NewPicB = new double[newHeight, newWidth];
            double[,] TNewPicR = new double[newHeight, newWidth];
            double[,] TNewPicG = new double[newHeight, newWidth];
            double[,] TNewPicB = new double[newHeight, newWidth];

            byte[,] NormPicR = new byte[newHeight, newWidth];
            byte[,] NormPicG = new byte[newHeight, newWidth];
            byte[,] NormPicB = new byte[newHeight, newWidth];
            int N = (length - 1) / 2;
            double rMin = double.MaxValue, rMax = double.MinValue;
            double gMin = double.MaxValue, gMax = double.MinValue;
            double bMin = double.MaxValue, bMax = double.MinValue;


            for (int i = 0; i < newHeight; i++)
            {
                for (int j = 0; j < newWidth - length + 1; j++)
                {
                    double Rsum = 0;
                    double Gsum = 0;
                    double Bsum = 0;
                    for (int k = 0; k < length; k++)
                    {
                        Rsum += (Filter1[k] * repRPixels[i, j + k]);
                        Gsum += (Filter1[k] * repGPixels[i, j + k]);
                        Bsum += (Filter1[k] * repBPixels[i, j + k]);
                    }
                    TNewPicR[i, j + N] = Rsum;
                    TNewPicG[i, j + N] = Gsum;
                    TNewPicB[i, j + N] = Bsum;
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
                        Rsum += (Filter2[k] * TNewPicR[i + k, j]);
                        Gsum += (Filter2[k] * TNewPicG[i + k, j]);
                        Bsum += (Filter2[k] * TNewPicB[i + k, j]);
                    }
                    rMin = Math.Min(rMin, Rsum);
                    gMin = Math.Min(gMin, Gsum);
                    bMin = Math.Min(bMin, Bsum);
                    rMax = Math.Max(rMax, Rsum);
                    gMax = Math.Max(gMax, Gsum);
                    bMax = Math.Max(bMax, Bsum);
                    NewPicR[i + N, j] = Rsum;
                    NewPicG[i + N, j] = Gsum;
                    NewPicB[i + N, j] = Bsum;
                    NormPicR[i + N, j] = (byte)NewPicR[i + N, j];
                    NormPicG[i + N, j] = (byte)NewPicG[i + N, j];
                    NormPicB[i + N, j] = (byte)NewPicB[i + N, j];
                }
            }
            if (type == "Sobel")
            {
                NormPicR = Normalize(NewPicR, newHeight, newWidth);
                NormPicG = Normalize(NewPicG, newHeight, newWidth);
                NormPicB = Normalize(NewPicB, newHeight, newWidth);
            }
            Red = unreplicateImage(length, length, height, width, NormPicR);
            Green = unreplicateImage(length, length, height, width, NormPicG);
            Blue = unreplicateImage(length, length, height, width, NormPicB);
        }
        public void Apply2DFilter(int Fwidth, int Fheight, double[,] Filter, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue)
        {
            int height = OldPic.height, width = OldPic.width;
            double SumOfFilter = Summation(Filter, Fwidth, Fheight);
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            byte[,] repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            byte[,] repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
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
        public void Apply2DCustomFilter(int Fwidth, int Fheight, double[,] Filter, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue)
        {
            int height = OldPic.height, width = OldPic.width;
            double SumOfFilter = Summation(Filter, Fwidth, Fheight);
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            byte[,] repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            byte[,] repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
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
                    Rsum /= Fsize;
                    Gsum /= Fsize;
                    Bsum /= Fsize;
                    if ((int)(SumOfFilter) == (Fheight * Fwidth))
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
            if ((int)(SumOfFilter) != (Fheight * Fwidth))
            {
                NewPicR = Normalize(TNewPicR, newHeight, newWidth);
                NewPicG = Normalize(TNewPicG, newHeight, newWidth);
                NewPicB = Normalize(TNewPicB, newHeight, newWidth);
            }
            Red = unreplicateImage(Fheight, Fwidth, height, width, NewPicR);
            Green = unreplicateImage(Fheight, Fwidth, height, width, NewPicG);
            Blue = unreplicateImage(Fheight, Fwidth, height, width, NewPicB);
        }
        #endregion

        #region LowPass & High Pass Filters
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
                    double temp;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
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
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
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
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
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
                    double temp;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
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
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
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
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
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
        #endregion

        #region Mean-OrderStat-Contra-Alpha
        public void Apply2DGMeanFilter(int Fwidth, int Fheight, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue)
        {
            int height = OldPic.height, width = OldPic.width;
            double Power = (double)(1 / (double)((double)Fwidth * (double)Fheight));
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            byte[,] repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            byte[,] repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
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
                            if (repRPixels[i + c, j + k] != 0)
                                Rmul *= (double)repRPixels[i + c, j + k];
                            if (repGPixels[i + c, j + k] != 0)
                                Gmul *= (double)repGPixels[i + c, j + k];
                            if (repBPixels[i + c, j + k] != 0)
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
            byte[,] repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            byte[,] repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            byte[,] repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
            PictureInfo ReplectPic = new PictureInfo(newWidth - 1, newHeight - 1, "ay7aga", "", null, repRPixels, repGPixels, repBPixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];
            int[,] BitMixed = CalcBitMixed(ReplectPic);
            int M = (Fheight - 1) / 2, N = (Fwidth - 1) / 2, FSize = Fheight * Fwidth;
            int[] SortedArray = new int[FSize];
            byte Rval = 0, Gval = 0, Bval = 0;
            switch (Filter)
            {
                #region MedianFilter
                case "Median":
                    {
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                int F = 0;
                                for (int c = 0; c < Fheight; c++)
                                {
                                    for (int k = 0; k < Fwidth; k++)
                                    {
                                        SortedArray[F++] = BitMixed[i + c, j + k];
                                    }
                                }
                                int BitMixedMedian = QuickSelect(SortedArray, FSize);
                                GetRGBfromBitMixed(BitMixedMedian, ref Rval, ref Gval, ref Bval);
                                NewPicR[i + M, j + N] = Rval;
                                NewPicG[i + M, j + N] = Gval;
                                NewPicB[i + M, j + N] = Bval;
                            }
                        }
                        break;
                    }
                #endregion
                #region Maximum
                case "Maximum":
                    {
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                int F = 0;
                                for (int c = 0; c < Fheight; c++)
                                {
                                    for (int k = 0; k < Fwidth; k++)
                                    {
                                        SortedArray[F++] = BitMixed[i + c, j + k];
                                    }
                                }
                                QuickSort(ref SortedArray, 0, FSize - 1);
                                int BitMixedMedian = SortedArray[FSize - 1];
                                GetRGBfromBitMixed(BitMixedMedian, ref Rval, ref Gval, ref Bval);
                                NewPicR[i + M, j + N] = Rval;
                                NewPicG[i + M, j + N] = Gval;
                                NewPicB[i + M, j + N] = Bval;
                            }
                        }
                        break;
                    }
                #endregion
                #region Minimum
                case "Minimum":
                    {
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                int F = 0;
                                for (int c = 0; c < Fheight; c++)
                                {
                                    for (int k = 0; k < Fwidth; k++)
                                    {
                                        SortedArray[F++] = BitMixed[i + c, j + k];
                                    }
                                }
                                QuickSort(ref SortedArray, 0, FSize - 1);
                                int BitMixedMedian = SortedArray[0];
                                GetRGBfromBitMixed(BitMixedMedian, ref Rval, ref Gval, ref Bval);
                                NewPicR[i + M, j + N] = Rval;
                                NewPicG[i + M, j + N] = Gval;
                                NewPicB[i + M, j + N] = Bval;
                            }
                        }
                        break;
                    }
                #endregion
                #region MidPoint
                case "MidPoint":
                    {
                        for (int i = 0; i < height; i++)
                        {
                            for (int j = 0; j < width; j++)
                            {
                                int F = 0;
                                for (int c = 0; c < Fheight; c++)
                                {
                                    for (int k = 0; k < Fwidth; k++)
                                    {
                                        SortedArray[F++] = BitMixed[i + c, j + k];
                                    }
                                }
                                QuickSort(ref SortedArray, 0, FSize - 1);
                                int BitMixedMedian = (SortedArray[FSize - 1] + SortedArray[0]) / 2;
                                GetRGBfromBitMixed(BitMixedMedian, ref Rval, ref Gval, ref Bval);
                                NewPicR[i + M, j + N] = Rval;
                                NewPicG[i + M, j + N] = Gval;
                                NewPicB[i + M, j + N] = Bval;
                            }
                        }
                        break;
                    }
                #endregion
            }
            Red = unreplicateImage(Fheight, Fwidth, height, width, NewPicR);
            Green = unreplicateImage(Fheight, Fwidth, height, width, NewPicG);
            Blue = unreplicateImage(Fheight, Fwidth, height, width, NewPicB);
        }
        public void Apply2DContraharmonicFilter(int Fwidth, int Fheight, PictureInfo OldPic, ref byte[,] Red, ref byte[,] Green, ref byte[,] Blue, double Q)
        {
            int height = OldPic.height, width = OldPic.width;
            int newHeight = height + Fheight;
            int newWidth = width + Fwidth;
            byte[,] repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            byte[,] repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            byte[,] repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
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
            byte[,] repRPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.redPixels);
            byte[,] repGPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.greenPixels);
            byte[,] repBPixels = ReplicateImage(Fheight, Fwidth, height, width, OldPic.bluePixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];
            PictureInfo ReplectPic = new PictureInfo(newWidth - 1, newHeight - 1, "ay7aga", "", null, repRPixels, repGPixels, repBPixels);
            int[,] BitMixed = CalcBitMixed(ReplectPic);
            int M = (Fheight - 1) / 2, N = (Fwidth - 1) / 2, FSize = Fheight * Fwidth;
            int[] SortedArray = new int[FSize];
            int BitMixedMedian;
            int index = (int)Math.Ceiling(D / 2);
            byte R = 0, G = 0, B = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    //byte[] R = new byte[FSize];
                    //byte[] G = new byte[FSize];
                    //byte[] B = new byte[FSize];
                    int F = 0;
                    for (int c = 0; c < Fheight; c++)
                    {
                        for (int k = 0; k < Fwidth; k++)
                        {
                            //R[F] = repRPixels[i + c, j + k];
                            //G[F] = repGPixels[i + c, j + k];
                            //B[F++] = repBPixels[i + c, j + k];
                            SortedArray[F++] = BitMixed[i + c, j + k];
                        }
                    }

                    double Rval = 0, Gval = 0, Bval = 0;

                    QuickSort(ref SortedArray, 0, FSize - 1);

                    for (int H = index; H < FSize - index; H++)
                    {
                        BitMixedMedian = SortedArray[H];
                        GetRGBfromBitMixed(BitMixedMedian, ref R, ref G, ref B);
                        Rval += R;
                        Gval += G;
                        Bval += B;
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
        #endregion

        #region Periodic Filters
        public void BandFilters(PictureInfo pic, int filterType, double D, double W)
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
                case 0: //ideal band reject 
                    double temp;
                    double Rdis = D - (W / 2), Ldis = D + (W / 2);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            temp = Math.Sqrt(Math.Pow((i - width / 2), 2) + Math.Pow((j - height / 2), 2));
                            if (temp <= Ldis && temp >= Rdis)
                                Filter[i, j] = 0;
                            else
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
                case 1: // ideal band pass
                    Rdis = D - (W / 2); Ldis = D + (W / 2);
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            temp = Math.Sqrt(Math.Pow((i - width / 2), 2) + Math.Pow((j - height / 2), 2));
                            if (temp <= Ldis && temp >= Rdis)
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
        public void NotchFilters(PictureInfo pic, int filterType, double X, double Y, double R)
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
                case 0: //ideal notch reject 
                    double temp1, temp2;
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            temp1 = Math.Sqrt(Math.Pow((i - (width / 2 + X)), 2) + Math.Pow((j - (height / 2 + Y)), 2));
                            temp2 = Math.Sqrt(Math.Pow((i - (width / 2 - X)), 2) + Math.Pow((j - (height / 2 - Y)), 2));
                            if (temp1 < R || temp2 < R)
                                Filter[i, j] = 0;
                            else
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
                case 1: // ideal notch pass
                    for (int i = 0; i < height; i++)
                    {
                        for (int j = 0; j < width; j++)
                        {
                            temp1 = Math.Sqrt(Math.Pow((i - (width / 2 + X)), 2) + Math.Pow((j - (height / 2 + Y)), 2));
                            temp2 = Math.Sqrt(Math.Pow((i - (width / 2 - X)), 2) + Math.Pow((j - (height / 2 - Y)), 2));
                            if (temp1 < R || temp2 < R)
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
        #endregion

        #region Adaptive Filters
        public void AdaptiveFilter(PictureInfo OldPic, int MaxWinSize, int type)
        {
            int height = OldPic.height, width = OldPic.width;
            int newHeight = height + MaxWinSize;
            int newWidth = width + MaxWinSize;
            byte[,] repRPixels = ReplicateImage(MaxWinSize, MaxWinSize, height, width, OldPic.redPixels);
            byte[,] repGPixels = ReplicateImage(MaxWinSize, MaxWinSize, height, width, OldPic.greenPixels);
            byte[,] repBPixels = ReplicateImage(MaxWinSize, MaxWinSize, height, width, OldPic.bluePixels);
            PictureInfo ReplectPic = new PictureInfo(newWidth - 1, newHeight - 1, "ay7aga", "", null, repRPixels, repGPixels, repBPixels);
            byte[,] NewPicR = new byte[newHeight, newWidth];
            byte[,] NewPicG = new byte[newHeight, newWidth];
            byte[,] NewPicB = new byte[newHeight, newWidth];
            int[,] BitMixed = CalcBitMixed(ReplectPic);
            if (type == 0) AdaptiveMedianFilter(height, width, MaxWinSize, repRPixels, repGPixels, repBPixels, ref NewPicR, ref NewPicG, ref NewPicB, BitMixed);
            else AdaptiveMeanFilter(height, width, MaxWinSize, repRPixels, repGPixels, repBPixels, NewPicR, NewPicG, NewPicB, BitMixed);
            OldPic.redPixels = unreplicateImage(MaxWinSize, MaxWinSize, height, width, NewPicR);
            OldPic.greenPixels = unreplicateImage(MaxWinSize, MaxWinSize, height, width, NewPicG);
            OldPic.bluePixels = unreplicateImage(MaxWinSize, MaxWinSize, height, width, NewPicB);
        }
        private void AdaptiveMedianFilter(int height, int width, int MaxWinSize, byte[,] repRPixels, byte[,] repGPixels, byte[,] repBPixels, ref byte[,] NewPicR, ref byte[,] NewPicG, ref byte[,] NewPicB, int[,] BitMixed)
        {
            byte Rval = 0, Gval = 0, Bval = 0;
            int BitMixedMedian = 0;
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    int h = 3;
                    while (true)
                    {
                        int M = (h - 1) / 2, N = (h - 1) / 2, FSize = h * h;
                        int[] SortedArray = new int[FSize];
                        int MaxPixel = int.MinValue, MinPixel = int.MaxValue;
                        int F = 0;
                        for (int c = 0; c < h; c++)
                        {
                            for (int k = 0; k < h; k++)
                            {
                                SortedArray[F++] = BitMixed[i + c, j + k];
                                MaxPixel = Math.Max(MaxPixel, BitMixed[i + c, j + k]);
                                MinPixel = Math.Min(MinPixel, BitMixed[i + c, j + k]);
                            }
                        }
                        int Center = BitMixed[i + M, j + N]; //before sorting
                        BitMixedMedian = QuickSelect(SortedArray, FSize);
                        if (BitMixedMedian > MinPixel && BitMixedMedian < MaxPixel) //sorted center not noise
                        {
                            if (Center > MinPixel && Center < MaxPixel) //old center is not noise
                            {
                                NewPicR[i + M, j + N] = repRPixels[i + M, j + N];
                                NewPicG[i + M, j + N] = repGPixels[i + M, j + N];
                                NewPicB[i + M, j + N] = repBPixels[i + M, j + N];
                                break;
                            }
                            else //old center noise ... 
                            {
                                GetRGBfromBitMixed(BitMixedMedian, ref Rval, ref Gval, ref Bval);
                                NewPicR[i + M, j + N] = Rval;
                                NewPicG[i + M, j + N] = Gval;
                                NewPicB[i + M, j + N] = Bval;
                                break;
                            }
                        }
                        else //sorted center is noise 
                        {
                            if (!(Center > MinPixel) || !(Center < MaxPixel)) //old center is noise--get another median
                            {
                                h += 2;
                                if (h > MaxWinSize)
                                {
                                    GetRGBfromBitMixed(BitMixedMedian, ref Rval, ref Gval, ref Bval);
                                    NewPicR[i + M, j + N] = Rval;
                                    NewPicG[i + M, j + N] = Gval;
                                    NewPicB[i + M, j + N] = Bval;
                                    break;
                                }
                            }
                            else //old center not noise .. don't replace
                            {
                                NewPicR[i + M, j + N] = repRPixels[i + M, j + N];
                                NewPicG[i + M, j + N] = repGPixels[i + M, j + N];
                                NewPicB[i + M, j + N] = repBPixels[i + M, j + N];
                                break;
                            }
                        }
                    }
                }
            }
        }
        private void AdaptiveMeanFilter(int height, int width, int MaxWinSize, byte[,] repRPixels, byte[,] repGPixels, byte[,] repBPixels, byte[,] NewPicR, byte[,] NewPicG, byte[,] NewPicB, int[,] bitMixed)
        {
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bool repeat = false;
                    for (int h = 3; h <= MaxWinSize; h += 2)
                    {
                        int M = (h - 1) / 2, N = (h - 1) / 2, FSize = h * h;
                        NewPicR[i + M, j + N] = repRPixels[i + M, j + N];
                        NewPicG[i + M, j + N] = repGPixels[i + M, j + N];
                        NewPicB[i + M, j + N] = repBPixels[i + M, j + N];
                        double Rmul = 0;
                        double Gmul = 0;
                        double Bmul = 0;
                        int MaxPixel = int.MinValue, MinPixel = int.MaxValue;
                        for (int c = 0; c < h; c++)
                        {
                            for (int k = 0; k < h; k++)
                            {
                                Rmul += (double)repRPixels[i + c, j + k];
                                Gmul += (double)repGPixels[i + c, j + k];
                                Bmul += (double)repBPixels[i + c, j + k];
                                //int temp = BitMixed(repRPixels[i + c, j + k], repRPixels[i + c, j + k], repRPixels[i + c, j + k]);
                                MaxPixel = Math.Max(MaxPixel, bitMixed[i + c, j + k]);
                                MinPixel = Math.Min(MinPixel, bitMixed[i + c, j + k]);
                            }
                        }
                        int rCenter = repRPixels[i + (h / 2), j + (h / 2)];
                        int gCenter = repGPixels[i + (h / 2), j + (h / 2)];
                        int bCenter = repBPixels[i + (h / 2), j + (h / 2)];
                        int Center = BitMixed((byte)rCenter, (byte)gCenter, (byte)bCenter);
                        double rPiX = Rmul / h;
                        double gPiX = Gmul / h;
                        double bPiX = Bmul / h;
                        int Pix = BitMixed((byte)Rmul, (byte)gPiX, (byte)bPiX);
                        if (Pix > MinPixel && Pix < MaxPixel)
                        {
                            if (!(Center > MinPixel && Center < MaxPixel))
                            {
                                NewPicR[i + M, j + N] = (byte)rPiX;
                                NewPicG[i + M, j + N] = (byte)gPiX;
                                NewPicB[i + M, j + N] = (byte)bPiX;
                            }
                        }
                        else
                            repeat = true;
                        if (repeat == false)
                            break;
                    }
                }
            }
        }
        #endregion
    }
}