﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ImageProcessingAssignment1.Properties;
using System.Drawing.Imaging;

namespace ImageProcessingAssignment1
{
    public partial class Morphology : Form
    {
        #region initializations
        Button[,] ArrBtn;
        bool Changes;
        Size BtnSize = new Size(50, 23);
        static bool WC, HC;
        int W, H, OldW, OldH, MorphologyType = 0, IOrigin, JOrigin, Radius;
        public PictureInfo PicParent,OriginalPic;
        public byte[,] modifiedRPixelArray;
        public byte[,] modifiedGPixelArray;
        public byte[,] modifiedBPixelArray;
        public byte[,] tempRPixelArray;
        public byte[,] tempGPixelArray;
        public byte[,] tempBPixelArray;
        int[,] SE;
        UndoRedo PicUndoRedo;
        #endregion

        public Morphology(PictureInfo pic, UndoRedo _PicUndoRedo)
        {
            InitializeComponent();
            ImageClass IM = new ImageClass();
            OriginalPic = pic;
            PicParent = IM.ConvertToBinary(pic);
            PicUndoRedo = _PicUndoRedo;
        }

        #region Structure Element
        private void wTBOX_TextChanged(object sender, EventArgs e)
        {
            WC = false;
            try
            {
                OldW = W;
                OldH = H;
                W = int.Parse(wTBOX.Text);
                H = int.Parse(hTBOX.Text);
                AddBTNs();
                SetBTN.Enabled = true;
                ResetBTN.Enabled = true;
            }
            catch
            {
                WC = false;
                SetBTN.Enabled = false;
                ResetBTN.Enabled = false;
            }
        }
        private void hTBOX_TextChanged(object sender, EventArgs e)
        {
            HC = false;
            try
            {
                OldH = H;
                OldW = W;
                H = int.Parse(hTBOX.Text);
                W = int.Parse(wTBOX.Text);
                AddBTNs();
                SetBTN.Enabled = true;
                ResetBTN.Enabled = true;
            }
            catch
            {
                HC = false;
                SetBTN.Enabled = false;
                ResetBTN.Enabled = false;
            }
        }
        private void AddBTNs()
        {
            ClearBTNs();
            ArrBtn = new Button[H, W];
            SE = new int[H, W];
            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                {
                    this.ArrBtn[i, j] = new System.Windows.Forms.Button();
                    //------------------------------------------------------------------------------
                    this.ArrBtn[i, j].Location = new System.Drawing.Point(j * (BtnSize.Width - 1) + 290, i * (BtnSize.Height - 1) + 233);
                    this.ArrBtn[i, j].Name = i.ToString() + "," + j.ToString();
                    this.ArrBtn[i, j].Size = BtnSize;
                    this.ArrBtn[i, j].Text = "0";
                    this.ArrBtn[i, j].BackColor = Color.FromArgb(255, 192, 128);
                    this.ArrBtn[i, j].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                    this.ArrBtn[i, j].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    this.ArrBtn[i, j].TabStop = false;
                    this.ArrBtn[i, j].Click += new System.EventHandler(this.btn_Click);
                    this.ArrBtn[i, j].KeyUp += new System.Windows.Forms.KeyEventHandler(this.btn_KeyUp);
                    //------------------------------------------------------------------------------
                    ArrBtn[i, j].BackgroundImage = null;
                    ArrBtn[i, j].BackColor = Color.FromArgb(255, 192, 128);
                    //------------------------------------------------------------------------------
                    this.Controls.Add(this.ArrBtn[i, j]);
                }
        }
        private void btn_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int i = int.Parse(btn.Name.Split(',')[0]);
            int j = int.Parse(btn.Name.Split(',')[1]);
            if (btn.BackColor == Color.Gainsboro)
            {
                SE[i, j] = 0;
                btn.BackgroundImage = null;
                btn.BackColor = Color.FromArgb(255, 192, 128);
                btn.Text = "0";
            }
            else
            {
                SE[i, j] = 1;
                btn.BackgroundImage = Resources.delete_icon;
                btn.BackColor = Color.Gainsboro;
                btn.Text = "1";
            }
        }
        private void ClearBTNs()
        {
            try
            {
                for (int i = 0; i < OldH; i++)
                {
                    for (int j = 0; j < OldW; j++)
                    {
                        this.Controls.Remove(ArrBtn[i, j]);
                    }
                }
            }
            catch { }
        }
        private void ClearBTNs(int Wid, int Hei)
        {
            try
            {
                for (int i = 0; i < Hei; i++)
                {
                    for (int j = 0; j < Wid; j++)
                    {
                        this.Controls.Remove(ArrBtn[i, j]);
                    }
                }
            }
            catch { }
        }
        private void SetBTN_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    SE[i, j] = 1;
                    ArrBtn[i, j].Text = "1";
                    ArrBtn[i, j].BackgroundImage = Resources.delete_icon;
                    ArrBtn[i, j].BackColor = Color.Gainsboro;
                }
            }
        }
        private void ResetBTN_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    SE[i, j] = 0;
                    ArrBtn[i, j].Text = "0";
                    ArrBtn[i, j].BackgroundImage = null;
                    ArrBtn[i, j].BackColor = Color.FromArgb(255, 192, 128);
                }
            }
        }
        private void ResetOrigin()
        {
            for (int i = 0; i < H; i++)
            {
                for (int j = 0; j < W; j++)
                {
                    if (ArrBtn[i, j].BackColor == Color.RoyalBlue)
                    {
                        ArrBtn[i, j].BackColor = Color.Gainsboro;
                    }
                }
            }
        }
        private void btn_KeyUp(object sender, EventArgs e)
        {
            ResetOrigin();
            Button btn = (Button)sender;
            IOrigin = int.Parse(btn.Name.Split(',')[0]);
            JOrigin = int.Parse(btn.Name.Split(',')[1]);
            if (btn.BackColor == Color.RoyalBlue)
            {
                btn.BackColor = Color.Gainsboro;
                IOrigin = JOrigin = 0;
                return;
            }
            if (btn.BackColor == Color.Gainsboro)
            {
                btn.BackColor = Color.RoyalBlue;
            }
            else
            {
                SE[IOrigin, JOrigin] = 1;
                btn.BackgroundImage = Resources.delete_icon;
                btn.BackColor = Color.RoyalBlue;
                btn.Text = "1";
            }
        }
        private void CircularSE(int Radius)
        {
            int SZ = Radius + Radius + 1;
            double temp;
            SE = new int[SZ, SZ];
            for (int i = 0; i < SZ; i++)
            {
                for (int j = 0; j < SZ; j++)
                {
                    temp = Math.Sqrt(Math.Pow((i - SZ / 2), 2) + Math.Pow((j - SZ / 2), 2));
                    if (temp <= Radius)
                        SE[i, j] = 1;
                }
            }
            AddCircularBTNs();
        }
        private void AddCircularBTNs()
        {
            ClearBTNs();
            ArrBtn = new Button[H, W];
            for (int i = 0; i < H; i++)
                for (int j = 0; j < W; j++)
                {
                    this.ArrBtn[i, j] = new System.Windows.Forms.Button();
                    //------------------------------------------------------------------------------
                    this.ArrBtn[i, j].Location = new System.Drawing.Point(j * (BtnSize.Width - 1) + 290, i * (BtnSize.Height - 1) + 233);
                    this.ArrBtn[i, j].Name = i.ToString() + "," + j.ToString();
                    this.ArrBtn[i, j].Size = BtnSize;
                    if (SE[i, j] == 1)
                    {
                        this.ArrBtn[i, j].Text = "1";
                        this.ArrBtn[i, j].BackColor = Color.Gainsboro;
                    }
                    else
                    {
                        this.ArrBtn[i, j].Text = "0";
                        this.ArrBtn[i, j].BackColor = Color.FromArgb(255, 192, 128);
                    }

                    this.ArrBtn[i, j].BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
                    this.ArrBtn[i, j].FlatStyle = System.Windows.Forms.FlatStyle.Flat;
                    this.ArrBtn[i, j].TabStop = false;
                    this.ArrBtn[i, j].Click += new System.EventHandler(this.btn_Click);
                    this.ArrBtn[i, j].KeyUp += new System.Windows.Forms.KeyEventHandler(this.btn_KeyUp);
                    //------------------------------------------------------------------------------
                    ArrBtn[i, j].BackgroundImage = null;
                    //------------------------------------------------------------------------------
                    this.Controls.Add(this.ArrBtn[i, j]);
                }
        }
        private void TriangleSE(int w, int h)
        {
            SE = new int[h, w];
            int[] A = { 0, w / 2 };
            int[] B = { h - 1, 0 };
            int[] C = { h - 1, w - 1 };
            double AllArea = CalcArea(A, B, C);
            double SmallArea1, SmallArea2, SmallArea3;
            int[] P = new int[2];
            for (int i = 0; i < h; i++)
            {
                for (int j = 0; j < w; j++)
                {
                    P[0] = i;
                    P[1] = j;
                    SmallArea1 = CalcArea(P, B, C);
                    SmallArea2 = CalcArea(P, A, B);
                    SmallArea3 = CalcArea(P, A, C);
                    if (SmallArea1 + SmallArea2 + SmallArea3 == AllArea)
                        SE[i, j] = 1;
                }
            }
            AddCircularBTNs();
        }
        private int[] Subtract(int[] V1, int[] V2)
        {
            int[] res = new int[2];
            res[0] = V1[0] - V2[0];
            res[1] = V1[1] - V2[1];
            return res;
        }
        private double DotProduct(int[] V1, int[] V2)
        {
            double res;
            res = ((double)V1[0] * (double)V2[0]) + (double)(V1[1] * (double)V2[1]);
            return res;
        }
        private double CalcArea(int[] A, int[] B, int[] C)
        {
            double res, a, b, c, d;
            a = A[0] - C[0];
            b = B[1] - A[1];
            c = A[0] - B[0];
            d = C[1] - A[1];
            res = 0.5 * (Math.Abs((a * b) - (d * c)));
            return res;
        }
        #endregion

        #region Controls Changes
        
        private void StructureElement_Load(object sender, EventArgs e)
        {
            Changes = false;
            pictureBox1.Image = PicParent.pictureBox.Image;
            int height = PicParent.height;
            int width = PicParent.width;
            modifiedRPixelArray = new byte[height, width];
            modifiedGPixelArray = new byte[height, width];
            modifiedBPixelArray = new byte[height, width];
            tempRPixelArray = new byte[height, width];
            tempGPixelArray = new byte[height, width];
            tempBPixelArray = new byte[height, width];
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    tempRPixelArray[i, j] = modifiedRPixelArray[i, j] = PicParent.redPixels[i, j];
                    tempGPixelArray[i, j] = modifiedGPixelArray[i, j] = PicParent.greenPixels[i, j];
                    tempBPixelArray[i, j] = modifiedBPixelArray[i, j] = PicParent.bluePixels[i, j];
                }
            }
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox1);
            DisplayImage(width, height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, pictureBox2);
            wTBOX.Text = "3";
            hTBOX.Text = "3";
            hTBOX.Text = "3";
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == false)
                DisplayImage(PicParent.width, PicParent.height, PicParent.redPixels, PicParent.greenPixels, PicParent.bluePixels, PicParent.pictureBox);
            else
                DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, PicParent.pictureBox);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            ApplyChanges();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            ApplyChanges();
            this.Close();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            DisplayImage(OriginalPic.width, OriginalPic.height, OriginalPic.redPixels, OriginalPic.greenPixels, OriginalPic.bluePixels, OriginalPic.pictureBox);
            this.Close();
        }
        private void DilationRBTN_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxChanged();
        }
        private void OpeningRBTN_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxChanged();
        }
        private void ErosionRBTN_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxChanged();
        }
        private void ClosingRBTN_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxChanged();
        }
        private void BoundExtrRBTN_CheckedChanged(object sender, EventArgs e)
        {
            CheckBoxChanged();
        }
        private void CheckBoxChanged()
        {
            if (DilationRBTN.Checked)
                MorphologyType = 0; //dilation
            else if (ErosionRBTN.Checked)
                MorphologyType = 1; //erosion
            else if (OpeningRBTN.Checked)
                MorphologyType = 2; //opening
            else if (ClosingRBTN.Checked)
                MorphologyType = 3; //closing
            else if (BoundExtrRBTN.Checked)
                MorphologyType = 4; //boundary extraction
            ApplyInChanges();
        }
        private void RadiusTBOX_TextChanged(object sender, EventArgs e)
        {
            try
            {
                OldW = Radius + Radius + 1;
                OldH = OldW;
                Radius = int.Parse(RadiusTBOX.Text);
                if (Radius <= 0)
                    throw new NumberError("Radius must me a non negative value !");
                H = W = Radius + Radius + 1;
                CircularSE(Radius);
            }
            catch (NumberError ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch { }
        }
        private void TriWBOX_TextChanged(object sender, EventArgs e)
        {
            WC = false;
            try
            {
                OldW = W;
                OldH = H;
                W = int.Parse(TriWBOX.Text);
                WC = true;
                if (HC)
                {
                    H = int.Parse(TriHeiBOX.Text);
                    TriangleSE(W, H);
                }
            }
            catch
            {
                WC = false;
            }
        }
        private void TriHeiBOX_TextChanged(object sender, EventArgs e)
        {
            HC = false;
            try
            {
                OldH = H;
                OldW = W;
                H = int.Parse(TriHeiBOX.Text);
                HC = true;
                if (WC)
                {
                    W = int.Parse(TriWBOX.Text);
                    TriangleSE(W, H);
                }
            }
            catch
            {
                HC = false;
            }
        }
        private void Morphology_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!Changes)
                DisplayImage(OriginalPic.width, OriginalPic.height, OriginalPic.redPixels, OriginalPic.greenPixels, OriginalPic.bluePixels, OriginalPic.pictureBox);
        }
        #endregion

        #region functions
        private void DisplayImage(int width, int height, byte[,] Red, byte[,] Green, byte[,] Blue, PictureBox picBox)
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
                        p[0] = Blue[i, j];
                        p[1] = Green[i, j];
                        p[2] = Red[i, j];
                        p += 3;
                    }
                    p += space;
                }
            }
            bmp.UnlockBits(bmpData);
            picBox.Image = bmp;
            picBox.BackColor = Color.White;
        }
        private void UpdateForm()
        {
            DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, pictureBox2);
            if (checkBox1.Checked == true)
            {
                DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, PicParent.pictureBox);
            }
        }
        private void ApplyChanges()
        {
            Changes = true;
            int height = PicParent.height;
            int width = PicParent.width;
            ImageClass Image = new ImageClass();
            try
            {
                if (IOrigin == 0 && JOrigin == 0 && SE[0, 0] != 1)
                {
                    throw new Exception("Please Choose a valid Origin !");
                }
                DateTime BeginTime = DateTime.Now;
                Image.IMorphology(height, width, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray, SE, MorphologyType, IOrigin, JOrigin, W, H);
                DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, pictureBox1);

                DateTime EndTime = DateTime.Now;
                TimeSpan TimeTaken = EndTime - BeginTime;
                DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, pictureBox2);
                for (int i = 0; i < height; i++)
                {
                    for (int j = 0; j < width; j++)
                    {
                        PicParent.redPixels[i, j] = OriginalPic.redPixels[i, j] = modifiedRPixelArray[i, j];
                        PicParent.greenPixels[i, j] = OriginalPic.greenPixels[i, j] = modifiedGPixelArray[i, j];
                        PicParent.bluePixels[i, j] = OriginalPic.bluePixels[i, j] = modifiedBPixelArray[i, j];
                    }
                }
                TimeForm Time = new TimeForm("Dilation", TimeTaken.ToString());
                Time.Show();
                if (checkBox1.Checked)
                    DisplayImage(OriginalPic.width, OriginalPic.height, OriginalPic.redPixels, OriginalPic.greenPixels, OriginalPic.bluePixels, OriginalPic.pictureBox);
                PicUndoRedo.UndoRedoCommands(PicParent, "Morphology");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void ApplyInChanges()
        {
            int height = PicParent.height;
            int width = PicParent.width;
            ImageClass Image = new ImageClass();
            try
            {
                if (IOrigin == 0 && JOrigin == 0 && SE[0, 0] != 1)
                    throw new Exception("Please Choose a valid Origin !");
                Image.IMorphology(height, width, tempRPixelArray, tempGPixelArray, tempBPixelArray, ref modifiedRPixelArray, ref modifiedGPixelArray, ref modifiedBPixelArray, SE, MorphologyType, IOrigin, JOrigin, W, H);
                DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, pictureBox2);
                if (checkBox1.Checked)
                    DisplayImage(PicParent.width, PicParent.height, modifiedRPixelArray, modifiedGPixelArray, modifiedBPixelArray, PicParent.pictureBox);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #region built-in SE
        private void circularToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OldH = H; OldW = W;
            ClearBTNs();
            groupBox5.Visible = false;
            groupBox7.Visible = false;
            groupBox6.Visible = true;
        }
        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OldH = H; OldW = W;
            ClearBTNs();
            groupBox5.Visible = true;
            groupBox7.Visible = false;
            groupBox6.Visible = false;
            wTBOX.Text = "";
            hTBOX.Text = "";
        }
        private void triangleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HC = WC = false;
            OldH = H; OldW = W;
            ClearBTNs();
            groupBox5.Visible = false;
            groupBox7.Visible = true;
            groupBox6.Visible = false;
            wTBOX.Text = "";
            hTBOX.Text = "";
        }
        #endregion

        

    }
}
