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
        List<UndoRedo> picUndoRedo;
        DataTable FilterValue;
        static bool W, H;
        public CustomFilter()
        {
            InitializeComponent();
        }
        public CustomFilter(List<PictureInfo> PicturesList, List<UndoRedo> _picUndoRedo)
        {
            picList = PicturesList;
            picUndoRedo = _picUndoRedo;
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
                textBox1.Text = "3";
                textBox2.Text = "3";
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
                    throw new NumberError("Width mush be odd value !");
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
                            FilterValue.Rows.Add(1);
                        FilterValue.Rows.Add(1);
                        FilterGrid.DataSource = FilterValue;
                    }
                }
            }
            catch (NumberError ex)
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
                    throw new NumberError("Height mush be odd value !");
                else
                {
                    H = true;
                    if (W == true)
                    {
                        FilterValue = new DataTable();
                        int FilterW = int.Parse(textBox1.Text);
                        int FilterH = int.Parse(textBox2.Text);
                        for (int i = 0; i < FilterW; i++)
                        {
                            FilterValue.Columns.Add("", typeof(int));
                        }
                        for (int i = 0; i < FilterH; i++)
                            FilterValue.Rows.Add();


                        for (int i = 0; i < FilterH; i++)
                            for (int j = 0; j < FilterW; j++)
                                FilterValue.Rows[i][j] = 1;

                        FilterGrid.DataSource = FilterValue;
                    }
                }
            }
            catch (NumberError ex)
            {
                MessageBox.Show(ex.Message);
                H = false;
            }
            catch { H = false; }
        }
        private void ApplyBTN_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }
        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void OkBtn_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            int picIndex = comboBox1.SelectedIndex;
            picUndoRedo[picIndex].UndoRedoCommands(picList[picIndex], "Custom Filter");
            this.Close();
        }
        private void ApplyChanges()
        {
            try
            {
                int FilterW = int.Parse(textBox1.Text);
                int FilterH = int.Parse(textBox2.Text);
                int picIndex = comboBox1.SelectedIndex;
                int height = picList[picIndex].height, width = picList[picIndex].width;
                double[,] CustomFilter = new double[FilterH, FilterW];
                for (int i = 0; i < FilterH; i++)
                    for (int j = 0; j < FilterW; j++)
                        CustomFilter[i, j] = double.Parse(FilterGrid[j, i].Value.ToString());
                Filter filter = new Filter();
                filter.Apply2DCustomFilter(FilterW, FilterH, CustomFilter, picList[picIndex], ref picList[picIndex].redPixels, ref picList[picIndex].greenPixels, ref picList[picIndex].bluePixels);
                DisplayImage(width, height, picList[picIndex].redPixels, picList[picIndex].greenPixels, picList[picIndex].bluePixels, PicBox2);
                DisplayImage(width, height, picList[picIndex].redPixels, picList[picIndex].greenPixels, picList[picIndex].bluePixels, picList[picIndex].pictureBox);
            }
            catch { }
        }
    }
}