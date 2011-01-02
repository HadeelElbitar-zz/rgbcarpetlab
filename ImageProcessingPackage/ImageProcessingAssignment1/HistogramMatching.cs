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
    public partial class HistogramMathcing : Form
    {
        List<PictureInfo> picList;
        TabControl tabControl;
        public HistogramMathcing(List<PictureInfo> PicturesList,TabControl _tabControl)
        {
            picList = PicturesList;
            tabControl = _tabControl;
            InitializeComponent();
        }
        private void HistogramMathcing_Load_1(object sender, EventArgs e)
        {
            try
            {
                int Size = picList.Count - 1;
                for (int i = 0; i < Size; i++)
                {
                    comboBox1.Items.Add(picList[i].name);
                    comboBox3.Items.Add(picList[i].name);
                }
                //comboBox1.SelectedIndex = 0;
            }
            catch { }
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int FirstPicIndex = comboBox1.SelectedIndex;
            DisplayImage(picList[FirstPicIndex].width, picList[FirstPicIndex].height, picList[FirstPicIndex].redPixels, picList[FirstPicIndex].greenPixels, picList[FirstPicIndex].bluePixels, pictureBox1);
            if (comboBox3.SelectedIndex != -1)
                DisplayChanges();
        }
        private void comboBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int SecondPicIndex = comboBox3.SelectedIndex;
            DisplayImage(picList[SecondPicIndex].width, picList[SecondPicIndex].height, picList[SecondPicIndex].redPixels, picList[SecondPicIndex].greenPixels, picList[SecondPicIndex].bluePixels, pictureBox2);
            DisplayChanges();
        }
        private void button4_Click(object sender, EventArgs e)//Ok
        {
            TabPage tabPage = new TabPage();
            tabControl.TabPages.Add(tabPage);
            int count = tabControl.TabPages.Count - 1;
            tabPage.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
            tabPage.Controls.Add(picList[count].pictureBox);
            tabControl.TabPages[count].Text = "Untitled.bmp";
            picList[count].name = "Untitled.bmp";
            picList[count].path = "C:\\Untitled.bmp";
            tabControl.SelectedIndex = count;
            tabPage.AutoScroll = true;
            picList[count].pictureBox.Size = new System.Drawing.Size(picList[count].width, picList[count].height);
            picList[count].pictureBox.Location = new System.Drawing.Point(tabPage.Width / 2 - picList[count].width / 2, tabPage.Height / 2 - picList[count].height / 2);
            DisplayImage(picList[count].width, picList[count].height, picList[count].redPixels, picList[count].greenPixels, picList[count].bluePixels, picList[count].pictureBox);
            this.Close();

        }
        private void button3_Click(object sender, EventArgs e)//Cancel
        {
            picList.RemoveAt(picList.Count - 1);
            this.Close();
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
        //Display Image

        private void DisplayImage(int width, int height, byte[,] red, byte[,] green, byte[,] blue, PictureBox picBox)
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
                        p[0] = blue[i, j];//Blue
                        p[1] = green[i, j];//Green
                        p[2] = red[i, j];//Red
                        p += 3;
                    }
                    p += space;
                }
            }
            bmp.UnlockBits(bmpData);
            picBox.Image = bmp;
        }
        private void DisplayChanges()
        {
            int FirstPicIndex = comboBox1.SelectedIndex;
            int SecondPicIndex = comboBox3.SelectedIndex;
            int count = picList.Count - 1;
            picList[count].width = picList[FirstPicIndex].width;
            picList[count].height = picList[FirstPicIndex].height;
            ImageClass Image = new ImageClass();
            DateTime BeginTime = DateTime.Now;
            Image.HistogramMatching(picList[FirstPicIndex].height, picList[FirstPicIndex].width, picList[FirstPicIndex], picList[SecondPicIndex], picList);
            DisplayImage(picList[FirstPicIndex].width, picList[FirstPicIndex].height, picList[count].redPixels, picList[count].greenPixels, picList[count].bluePixels, pictureBox3);
            DateTime EndTime = DateTime.Now;
            TimeSpan TimeTaken = EndTime - BeginTime;
            TimeForm Time = new TimeForm("Histogram Matching", TimeTaken.ToString());
            Time.Show();
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
    }
}