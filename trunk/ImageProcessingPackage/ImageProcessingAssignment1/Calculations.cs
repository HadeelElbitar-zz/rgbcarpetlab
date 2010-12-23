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
    public partial class Calculations : Form
    {
        List<PictureInfo> picList;
        TabControl tabControl;
        public Calculations(List<PictureInfo> PicturesList, TabControl _tabControl)
        {
            picList = PicturesList;
            tabControl = _tabControl;
            InitializeComponent();
        }
        private void AddSubtract_Load(object sender, EventArgs e)
        {
            try
            {
                int Size = picList.Count - 1;
                for (int i = 0; i < Size; i++)
                {
                    comboBox1.Items.Add(picList[i].name);
                    comboBox3.Items.Add(picList[i].name);
                }
                comboBox2.Items.Add("Add");
                comboBox2.Items.Add("Subtract");
                comboBox2.Items.Add("Product");
                comboBox2.Items.Add("AND");
                comboBox2.Items.Add("OR");
                int index = picList.Count - 1;
                picList[index] = new PictureInfo(picList[0].width, picList[0].height, "Untitled.bmp", "C:\\Untitled.bmp", picList[index].pictureBox, picList[0].redPixels, picList[0].greenPixels, picList[0].bluePixels);
                comboBox1.SelectedIndex = 0;
                comboBox2.SelectedIndex = 0;
                comboBox3.SelectedIndex = 0;
            }
            catch { }
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        private void comboBox1_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1)
                comboBox2.SelectedIndex = 0;
            if (comboBox3.SelectedIndex == -1)
                comboBox3.SelectedIndex = 0;
            int FirstPicIndex = comboBox1.SelectedIndex;
            DisplayImage(picList[FirstPicIndex].width, picList[FirstPicIndex].height, picList[FirstPicIndex].redPixels, picList[FirstPicIndex].greenPixels, picList[FirstPicIndex].bluePixels, pictureBox1);
            DisplayChanges();
        }
        private void comboBox3_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            int SecondPicIndex = comboBox3.SelectedIndex;
            DisplayImage(picList[SecondPicIndex].width, picList[SecondPicIndex].height, picList[SecondPicIndex].redPixels, picList[SecondPicIndex].greenPixels, picList[SecondPicIndex].bluePixels, pictureBox2);
            DisplayChanges();
        }
        private void comboBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == -1)
                comboBox3.SelectedIndex = 0;
            DisplayChanges();
        }
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            DisplayChanges();
        }
        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            DisplayChanges();
        }
        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            if (radioButton3.Checked == true)
                groupBox5.Visible = true;
            else
                groupBox5.Visible = false;
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
            tabControl.SelectedIndex = count;
            tabPage.AutoScroll = true;
            picList[count].pictureBox.Size = new System.Drawing.Size(picList[count].width, picList[count].height);
            picList[count].pictureBox.Location = new System.Drawing.Point(tabPage.Width / 2 - picList[count].width / 2, tabPage.Height / 2 - picList[count].height / 2);
            DisplayImage(picList[count].width, picList[count].height, picList[count].redPixels, picList[count].greenPixels, picList[count].bluePixels, picList[count].pictureBox);
            //try
            //{
            //    Bitmap bmpsave = new Bitmap(picList[count].pictureBox.Image);
            //    bmpsave.Save(picList[count].path, ImageFormat.Bmp);
            //}
            //catch { }
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)//Cancel
        {
            picList.RemoveAt(picList.Count - 1);
            this.Close();
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //new Height and Width
        private void button1_Click(object sender, EventArgs e)
        {
            newHeightAndWidth();
        }
        private void newHeightAndWidth()
        {
            int newHeight = int.Parse(textBox1.Text);
            int newWidth = int.Parse(textBox2.Text);
            byte[,] firstRed = new byte[newHeight, newWidth];
            byte[,] firstGreen = new byte[newHeight, newWidth];
            byte[,] firstBlue = new byte[newHeight, newWidth];
            byte[,] secondRed = new byte[newHeight, newWidth];
            byte[,] secondGreen = new byte[newHeight, newWidth];
            byte[,] secondBlue = new byte[newHeight, newWidth];
            ImageClass Image = new ImageClass();
            Image.ResizeImage(picList[comboBox1.SelectedIndex], newHeight, newWidth, ref firstRed, ref firstGreen, ref firstBlue);
            Image.ResizeImage(picList[comboBox3.SelectedIndex], newHeight, newWidth, ref secondRed, ref secondGreen, ref secondBlue);
            int count = picList.Count - 1;
            Image.AddSubtractTwoPictures(newHeight, newWidth, firstRed, firstGreen, firstBlue, secondRed, secondGreen, secondBlue, picList[count], comboBox2.SelectedIndex);
            DisplayImage(newWidth, newHeight, picList[count].redPixels, picList[count].greenPixels, picList[count].bluePixels, pictureBox3);
            picList[count].width = newWidth;
            picList[count].height = newHeight;
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
                        p[0] = blue[i, j];
                        p[1] = green[i, j];
                        p[2] = red[i, j];
                        p += 3;
                    }
                    p += space;
                }
            }
            bmp.UnlockBits(bmpData);
            picBox.Image = bmp;
        }
        public void DisplayChanges()
        {
            int FirstPicIndex = comboBox1.SelectedIndex;
            int SecondPicIndex = comboBox3.SelectedIndex;
            int Operation = comboBox2.SelectedIndex;
            int count = picList.Count - 1;
            if (picList[FirstPicIndex].width != picList[SecondPicIndex].width ||
                picList[FirstPicIndex].height != picList[SecondPicIndex].height)
            {
                groupBox4.Visible = true;
                if (radioButton3.Checked)
                {
                    groupBox5.Visible = true;
                    try { newHeightAndWidth(); }
                    catch { }
                }
                else
                {
                    int newWidth = picList[FirstPicIndex].width;
                    int newHeight = picList[FirstPicIndex].height;
                    if (radioButton2.Checked)
                    {
                        byte[,] firstRed = new byte[newHeight, newWidth];
                        byte[,] firstGreen = new byte[newHeight, newWidth];
                        byte[,] firstBlue = new byte[newHeight, newWidth];
                        ImageClass Image = new ImageClass();
                        Image.ResizeImage(picList[comboBox1.SelectedIndex], newHeight, newWidth, ref firstRed, ref firstGreen, ref firstBlue);
                        Image.AddSubtractTwoPictures(newHeight, newWidth, firstRed, firstGreen, firstBlue, picList[SecondPicIndex].redPixels, picList[SecondPicIndex].greenPixels, picList[SecondPicIndex].bluePixels, picList[count], Operation);
                        DisplayImage(newWidth, newHeight, picList[count].redPixels, picList[count].greenPixels, picList[count].bluePixels, pictureBox3);
                    }
                    else if (radioButton1.Checked)
                    {
                        byte[,] secondRed = new byte[newHeight, newWidth];
                        byte[,] secondGreen = new byte[newHeight, newWidth];
                        byte[,] secondBlue = new byte[newHeight, newWidth];
                        ImageClass Image = new ImageClass();
                        Image.ResizeImage(picList[comboBox3.SelectedIndex], newHeight, newWidth, ref secondRed, ref secondGreen, ref secondBlue);
                        Image.AddSubtractTwoPictures(newHeight, newWidth, picList[FirstPicIndex].redPixels, picList[FirstPicIndex].greenPixels, picList[FirstPicIndex].bluePixels, secondRed, secondGreen, secondBlue, picList[count], comboBox2.SelectedIndex);
                        DisplayImage(newWidth, newHeight, picList[count].redPixels, picList[count].greenPixels, picList[count].bluePixels, pictureBox3);
                    }
                    picList[count].width = newWidth;
                    picList[count].height = newHeight;
                }
            }
            else//Widths and Heights are equal
            {
                groupBox4.Visible = false;
                ImageClass Image = new ImageClass();
                Image.AddSubtractTwoPictures(picList[FirstPicIndex].height, picList[FirstPicIndex].width, picList[FirstPicIndex].redPixels, picList[FirstPicIndex].greenPixels, picList[FirstPicIndex].bluePixels, picList[SecondPicIndex].redPixels, picList[SecondPicIndex].greenPixels, picList[SecondPicIndex].bluePixels, picList[count], Operation);
                DisplayImage(picList[FirstPicIndex].width, picList[FirstPicIndex].height, picList[count].redPixels, picList[count].greenPixels, picList[count].bluePixels, pictureBox3);
                picList[count].width = picList[FirstPicIndex].width;
                picList[count].height = picList[FirstPicIndex].height;
            }
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

    }
}