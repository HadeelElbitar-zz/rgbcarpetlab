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
    public partial class CustomFilter : Form
    {
        List<PictureInfo> picList;
        DataTable FilterValue;
        static bool W, H;
        class EvenNumber : ApplicationException
        {
            public EvenNumber() : base() { }
            public EvenNumber(string s) : base(s) { }
            public EvenNumber(string s, Exception ex)
                : base(s, ex) { }
        }
        public CustomFilter()
        {
            InitializeComponent();
        }
        public CustomFilter(List<PictureInfo> PicturesList)
        {
            picList = PicturesList;
            FilterValue = new DataTable();
            InitializeComponent();
        }
        private void CustomFilter_Load(object sender, EventArgs e)
        {
            try
            {
                int Size = picList.Count;
                for (int i = 0; i < Size; i++)
                    comboBox1.Items.Add(picList[i].name);
                comboBox1.SelectedIndex = 0;
            }
            catch { }
        }
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
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int FirstPicIndex = comboBox1.SelectedIndex;
            DisplayImage(picList[FirstPicIndex].width, picList[FirstPicIndex].height, picList[FirstPicIndex].redPixels, picList[FirstPicIndex].greenPixels, picList[FirstPicIndex].bluePixels, PicBox1);
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            W = false;
            try
            {
                if (int.Parse(textBox1.Text) % 2 == 0)
                    throw new EvenNumber("Width mush be odd value !");
                else
                {
                    W = true;
                    if (H == true)
                    {
                        FilterValue = new DataTable();
                        int FilterW = int.Parse(textBox1.Text);
                        int FilterH = int.Parse(textBox2.Text);
                        for (int i = 0; i < FilterW; i++)
                            FilterValue.Columns.Add("", typeof(int));
                        for (int i = 0; i < FilterH; i++)
                            FilterValue.Rows.Add(0);
                        FilterGrid.DataSource = FilterValue;
                    }
                }
            }
            catch (EvenNumber ex)
            {
                MessageBox.Show(ex.Message);
                W = false;
            }
            catch { W = false; }
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            H = false;
            try
            {
                if (int.Parse(textBox2.Text) % 2 == 0)
                    throw new EvenNumber("Height mush be odd value !");
                else
                {
                    H = true;
                    if (W == true)
                    {
                        FilterValue = new DataTable();
                        int FilterW = int.Parse(textBox1.Text);
                        int FilterH = int.Parse(textBox2.Text);
                        for (int i = 0; i < FilterW; i++)
                            FilterValue.Columns.Add("", typeof(int));
                        for (int i = 0; i < FilterH; i++)
                            FilterValue.Rows.Add(0);
                        FilterGrid.DataSource = FilterValue;
                    }
                }
            }
            catch (EvenNumber ex)
            {
                MessageBox.Show(ex.Message);
                H = false;
            }
            catch { H = false; }
        }
        private void ApplyBTN_Click(object sender, EventArgs e)
        {
            try
            {
                int FilterW = int.Parse(textBox1.Text);
                int FilterH = int.Parse(textBox2.Text);
                PictureInfo OldPic = picList[comboBox1.SelectedIndex];
                int height = OldPic.height, width = OldPic.width;
                double[,] CustomFilter = new double[FilterH, FilterW];
                for (int i = 0; i < FilterH; i++)
                    for (int j = 0; j < FilterW; j++)
                        CustomFilter[i, j] = double.Parse(FilterGrid[j, i].Value.ToString());
                byte[,] Red = new byte[height, width];
                byte[,] Green = new byte[height, width];
                byte[,] Blue = new byte[height, width];
                Filter filter = new Filter();
                filter.Apply2DFilter(FilterW, FilterH, CustomFilter, OldPic, ref Red, ref Green, ref Blue);
                DisplayImage(width, height, Red, Green, Blue, PicBox2);
            }
            catch { }
        }
    }
}