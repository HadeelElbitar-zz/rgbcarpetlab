using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Drawing.Imaging;
using ZedGraph;
using MathWorks.MATLAB.NET.Utility;
using MathWorks.MATLAB.NET.Arrays;
using MatlabProject;
using System.Threading;

namespace ImageProcessingAssignment1
{
    public partial class Home : Form
    {
        #region Form Related
        List<PictureInfo> PicturesList;
        GroupBox inputGroupBox;
        public Home()
        {
            InitializeComponent();
            Thread t = new Thread(new ThreadStart(SplashScreen));
            t.Start();
            Thread.Sleep(2000);
            PicturesList = new List<PictureInfo>();
            t.Abort();
        }
        private void SplashScreen()
        {
            Application.Run(new SplashScreenImage());
        }
        private void MDIParent1_Load(object sender, EventArgs e)
        {
            foreach (Control ctl in this.Controls)
            {
                if (ctl is MdiClient)
                {
                    ctl.BackColor = System.Drawing.Color.FromArgb(50, 50, 50);
                    break;
                }
            }
            inputGroupBox = new GroupBox();
            InputPanel.Controls.Add(inputGroupBox);
            inputGroupBox.Location = new System.Drawing.Point(7, 5);
            inputGroupBox.Size = new System.Drawing.Size(280, 240);
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
        
        #region UpdateForm
        //Display Image
        private void DisplayImage(PictureInfo pictureInfo)
        {
            int width = pictureInfo.width;
            int height = pictureInfo.height;
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
                        p[0] = pictureInfo.bluePixels[i, j];
                        p[1] = pictureInfo.greenPixels[i, j];
                        p[2] = pictureInfo.redPixels[i, j];
                        p += 3;
                    }
                    p += space;
                }
            }
            bmp.UnlockBits(bmpData);
            pictureInfo.pictureBox.Size = new System.Drawing.Size(width, height);
            pictureInfo.pictureBox.Image = bmp;
            UpdateHistogram(pictureInfo);
        }
        private void UpdateHistogram(PictureInfo pic)
        {
            ImageClass Image = new ImageClass();
            if (checkBox1.Visible == false) checkBox1.Visible = true;
            if (checkBox2.Visible == false) checkBox2.Visible = true;
            if (checkBox3.Visible == false) checkBox3.Visible = true;
            if (checkBox4.Visible == false) checkBox4.Visible = true;
            double[] R = new double[256];
            double[] G = new double[256];
            double[] B = new double[256];
            Image.GetHistogram(ref R, ref G, ref B, pic);
            int HistogramHeight = 5;
            int R_Max = 0, G_Max = 0, B_Max = 0;
            double[] Gray = new double[256];
            double[] X_axis = new double[256];
            R_Max = Image.GetMax(R);
            G_Max = Image.GetMax(G);
            B_Max = Image.GetMax(B);
            //Normalization
            for (int j = 0; j < 256; j++)
            {
                R[j] = R[j] / R_Max * HistogramHeight;
                G[j] = G[j] / G_Max * HistogramHeight;
                B[j] = B[j] / B_Max * HistogramHeight;
                Gray[j] = (R[j] + G[j] + B[j]) / 3;
                X_axis[j] = (double)j;
            }
            //Draw
            zedGraphControl1.GraphPane.CurveList.Clear();
            GraphPane HistogramPane = zedGraphControl1.GraphPane;
            if (checkBox2.Checked)
            {
                PointPairList RedPoints = new PointPairList(X_axis, R);
                LineItem RedLine = HistogramPane.AddCurve("Red", RedPoints, Color.Red, SymbolType.None);
                RedLine.Line.Width = 1.0F;
            }
            if (checkBox3.Checked)
            {
                PointPairList GreenPoints = new PointPairList(X_axis, G);
                LineItem GreenLine = HistogramPane.AddCurve("Green", GreenPoints, Color.Green, SymbolType.None);
                GreenLine.Line.Width = 1.0F;
            }
            if (checkBox4.Checked)
            {
                PointPairList BluePoints = new PointPairList(X_axis, B);
                LineItem BlueLine = HistogramPane.AddCurve("Blue", BluePoints, Color.Blue, SymbolType.None);
                BlueLine.Line.Width = 1.0F;
            }
            if (checkBox1.Checked)
            {
                PointPairList GrayPoints = new PointPairList(X_axis, Gray);
                LineItem GrayLine = HistogramPane.AddCurve("Gray", GrayPoints, Color.Gray, SymbolType.None);
                GrayLine.Line.Width = 1.0F;
            }
            HistogramPane.Title.Text = "Histogram";
            zedGraphControl1.Refresh();
            zedGraphControl1.AxisChange();
            zedGraphControl1.Invalidate();
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region MDIMenuItems
        private void toolStripMenuItem2_Click(object sender, EventArgs e)//Close
        {
            int PicIndex = tabControl1.SelectedIndex;
            tabControl1.TabPages.RemoveAt(PicIndex);
            if (tabControl1.TabPages.Count > 0 && PicIndex != 0)
                tabControl1.SelectedIndex = PicIndex - 1;
            PicturesList.RemoveAt(PicIndex);
        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)//CloseAll
        {
            tabControl1.TabPages.Clear();
            tabControl1.Visible = false;
            PicturesList.Clear();
            zedGraphControl1.GraphPane.CurveList.Clear();
            zedGraphControl1.Visible = false;
        }

        //Open Image
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string[] PicturePath = new string[20];
                OpenFileDialog Picture = new OpenFileDialog();
                Picture.Filter = "BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpg)|*.jpg|PPM Files (*.ppm)|*.ppm";
                Picture.Multiselect = true;
                if (Picture.ShowDialog() == DialogResult.OK)
                    PicturePath = Picture.FileNames;
                int count = PicturePath.Count();
                for (int k = 0; k < count; k++)
                {
                    PictureInfo newPictureItem = new PictureInfo();
                    PictureBox picBox = new PictureBox();
                    picBox.BorderStyle = BorderStyle.FixedSingle;
                    picBox.Location = new System.Drawing.Point(100, 100);
                    string PictureName = PicturePath[k].Substring(PicturePath[k].LastIndexOf('\\') + 1);
                    ImageClass image = new ImageClass();

                    if (Picture.FilterIndex == 1 || Picture.FilterIndex == 2)//BMP||JPEG
                        image.openBmpJpeg(PicturePath[k], ref newPictureItem, PictureName, picBox);
                    else//PPM
                        image.openPPM(PicturePath[k], ref newPictureItem, PictureName, picBox);
                    PicturesList.Add(newPictureItem);
                    int index = PicturesList.Count - 1;
                    if (!tabControl1.Visible) tabControl1.Visible = true;
                    TabPage tabPage = new TabPage();
                    tabControl1.TabPages.Add(tabPage);
                    tabPage.Controls.Add(PicturesList[index].pictureBox);
                    tabControl1.TabPages[index].Text = PictureName;
                    tabControl1.SelectedIndex = index;
                    tabPage.AutoScroll = true;
                    zedGraphControl1.Visible = true;
                    DisplayImage(PicturesList[index]);
                }
            }
            catch { }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int picIndex = tabControl1.SelectedIndex;
            SaveFileDialog PictureDialog = new SaveFileDialog();
            PictureDialog.Filter = "BMP Files (*.bmp)|*.bmp|JPEG Files (*.jpg)|*.jpg|P3 Files (*.ppm)|*.ppm|P6 Files (*.ppm)|*.ppm";
            try
            {
                string PicturePath = "";
                PictureDialog.AddExtension = true;
                PictureDialog.DefaultExt = ".bmp";
                if (PictureDialog.ShowDialog() == DialogResult.OK) PicturePath = PictureDialog.FileName;
                if (PictureDialog.FilterIndex == 1)
                {
                    try
                    {
                        PicturesList[picIndex].pictureBox.Image.Save(PicturePath, ImageFormat.Bmp);
                    }
                    catch { }
                }
                else if (PictureDialog.FilterIndex == 2)
                {
                    try
                    {
                        PicturesList[picIndex].pictureBox.Image.Save(PicturePath, ImageFormat.Bmp);
                    }
                    catch { }
                }
                else if (PictureDialog.FilterIndex == 3 || PictureDialog.FilterIndex == 4)//PPM-P3-P6
                {
                    ImageClass Image = new ImageClass();
                    Image.SaveAsPPM(PicturePath, PicturesList[picIndex], PictureDialog.FilterIndex);
                }
            }
            catch { }
        }
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox AB = new AboutBox();
            AB.Show();
        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewFormInput NFI = new NewFormInput(PicturesList, tabControl1);
            NFI.Show();
        }
        private void ExitToolsStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        private void CutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void PasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
        private void ToolBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toolStrip.Visible = toolBarToolStripMenuItem.Checked;
        }
        private void StatusBarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            statusStrip.Visible = statusBarToolStripMenuItem.Checked;
        }
        private void CascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }
        private void TileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }
        private void TileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }
        private void ArrangeIconsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.ArrangeIcons);
        }
        private void CloseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            foreach (Form childForm in MdiChildren)
            {
                childForm.Close();
            }
        }

        private void tabControl1_ControlRemoved(object sender, ControlEventArgs e)
        {
            if (tabControl1.TabPages.Count == 0)
                DisableMenus();
        }
        private void tabControl1_ControlAdded(object sender, ControlEventArgs e)
        {
            if (tabControl1.SelectedIndex == 1)
                EnableMenus();
        }
        private void DisableMenus()
        {
            imageToolStripMenuItem.Visible = false;
            histogramToolStripMenuItem2.Enabled = false;
            filtersToolStripMenuItem.Enabled = false;
            matlabToolStripMenuItem.Enabled = false;
            addNoiseToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            closeAllToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;
            saveToolStripButton1.Enabled = false;
        }
        private void EnableMenus()
        {
            imageToolStripMenuItem.Enabled = false;
            histogramToolStripMenuItem2.Enabled = false;
            filtersToolStripMenuItem.Enabled = false;
            matlabToolStripMenuItem.Enabled = false;
            addNoiseToolStripMenuItem.Enabled = false;
            saveAsToolStripMenuItem.Enabled = false;
            saveToolStripMenuItem.Enabled = false;
            closeAllToolStripMenuItem.Enabled = false;
            closeToolStripMenuItem.Enabled = false;
            saveToolStripButton1.Enabled = false;
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Image operations
        //Translating
        private void translateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Image Taslation";
            //X-Translation Width Text Box
            TextBox xTxt = new TextBox();
            xTxt.Location = new System.Drawing.Point(15, 42);
            xTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(xTxt);
            //X-Translation Label
            System.Windows.Forms.Label xLabel = new System.Windows.Forms.Label();
            xLabel.Location = new System.Drawing.Point(12, 26);
            xLabel.Text = "X-Translation";
            inputGroupBox.Controls.Add(xLabel);
            //Y-Translation Text Box
            TextBox yTxt = new TextBox();
            yTxt.Location = new System.Drawing.Point(158, 42);
            yTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(yTxt);
            //Y-Translation Label
            System.Windows.Forms.Label yLabel = new System.Windows.Forms.Label();
            yLabel.Location = new System.Drawing.Point(155, 26);
            yLabel.Text = "Y-Translation";
            inputGroupBox.Controls.Add(yLabel);
            //Button
            Button TranslateBtn = new Button();
            TranslateBtn.Text = "Translate";
            TranslateBtn.Location = new System.Drawing.Point(102, 85);
            TranslateBtn.Size = new System.Drawing.Size(75, 23);
            TranslateBtn.Click += delegate(object sender1, EventArgs e1) { TranslateButton_Click(sender1, e1, xTxt, yTxt); };
            inputGroupBox.Controls.Add(TranslateBtn);
        }
        private void TranslateButton_Click(object sender, EventArgs e, TextBox xTranslation, TextBox yTranslation)
        {
            int PicIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.TranslateImage(PicturesList[PicIndex], int.Parse(xTranslation.Text), int.Parse(yTranslation.Text));
            DisplayImage(PicturesList[PicIndex]);
        }

        //Rotating
        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Image Taslation";
            //Theta Text Box
            TextBox thetaTxt = new TextBox();
            thetaTxt.Location = new System.Drawing.Point(15, 42);
            thetaTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(thetaTxt);
            //Theta Label
            System.Windows.Forms.Label thetaLabel = new System.Windows.Forms.Label();
            thetaLabel.Location = new System.Drawing.Point(12, 26);
            thetaLabel.Text = "Theta";
            inputGroupBox.Controls.Add(thetaLabel);
            //Button
            Button RotateBtn = new Button();
            RotateBtn.Text = "Rotate";
            RotateBtn.Location = new System.Drawing.Point(102, 85);
            RotateBtn.Size = new System.Drawing.Size(75, 23);
            RotateBtn.Click += delegate(object sender1, EventArgs e1) { RotateButton_Click(sender1, e1, thetaTxt); };
            inputGroupBox.Controls.Add(RotateBtn);
        }
        private void RotateButton_Click(object sender, EventArgs e, TextBox Theta)
        {
            int PicIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.RotateImage(PicturesList[PicIndex], double.Parse(Theta.Text));
            DisplayImage(PicturesList[PicIndex]);
        }

        //Shearing
        private void shearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Image Shearing";
            //Shearing Value Text Box
            TextBox shearTxt = new TextBox();
            shearTxt.Location = new System.Drawing.Point(15, 42);
            shearTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(shearTxt);
            //shear Label
            System.Windows.Forms.Label shearLabel = new System.Windows.Forms.Label();
            shearLabel.Location = new System.Drawing.Point(12, 26);
            shearLabel.Text = "Shearing Value";
            inputGroupBox.Controls.Add(shearLabel);
            //Button
            Button ShearBtn = new Button();
            ShearBtn.Text = "Shear";
            ShearBtn.Location = new System.Drawing.Point(102, 165);
            ShearBtn.Size = new System.Drawing.Size(75, 23);
            ShearBtn.Click += delegate(object sender1, EventArgs e1) { ShearButton_Click(sender1, e1, shearTxt); };
            inputGroupBox.Controls.Add(ShearBtn);
        }
        private void ShearButton_Click(object sender, EventArgs e, TextBox Shear)
        {
            int PicIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.ShearImage(PicturesList[PicIndex], int.Parse(Shear.Text));
            DisplayImage(PicturesList[PicIndex]);
        }

        //Flipping
        private void horizontalFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.FlipImage(PicturesList[picIndex], 0);
            DisplayImage(PicturesList[picIndex]);
        }
        private void verticalFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.FlipImage(PicturesList[picIndex], 1);
            DisplayImage(PicturesList[picIndex]);
        }

        //Resizing
        private void bilinearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Image Resizing";
            //Width Text Box
            TextBox wTxt = new TextBox();
            wTxt.Location = new System.Drawing.Point(15, 42);
            wTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(wTxt);
            //Width Label
            System.Windows.Forms.Label wLabel = new System.Windows.Forms.Label();
            wLabel.Location = new System.Drawing.Point(12, 26);
            wLabel.Text = "New Width";
            inputGroupBox.Controls.Add(wLabel);
            //Hight Text Box
            TextBox hTxt = new TextBox();
            hTxt.Location = new System.Drawing.Point(158, 42);
            hTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(hTxt);
            //Height Label
            System.Windows.Forms.Label hLabel = new System.Windows.Forms.Label();
            hLabel.Location = new System.Drawing.Point(155, 26);
            hLabel.Text = "New Height";
            inputGroupBox.Controls.Add(hLabel);
            //Button
            Button ResizeBtn = new Button();
            ResizeBtn.Text = "Resize";
            ResizeBtn.Location = new System.Drawing.Point(102, 85);
            ResizeBtn.Size = new System.Drawing.Size(75, 23);
            ResizeBtn.Click += delegate(object sender1, EventArgs e1) { ResizeButton_Click(sender1, e1, wTxt, hTxt); };
            inputGroupBox.Controls.Add(ResizeBtn);
        }
        private void ResizeButton_Click(object sender, EventArgs e, TextBox NewHBox, TextBox NewWBox)
        {
            int PicIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.ResizeImage(PicturesList[PicIndex], int.Parse(NewHBox.Text), int.Parse(NewWBox.Text));
            DisplayImage(PicturesList[PicIndex]);
        }

        //Logical Operations -- NOT
        private void reverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int PictureIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.ReverseColors(PicturesList[PictureIndex]);
            DisplayImage(PicturesList[PictureIndex]);
        }

        //Gray Scale
        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int pictureIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.GrayScale(PicturesList[pictureIndex]);
            DisplayImage(PicturesList[pictureIndex]);
        }

        //Equalization
        private void histogramEqualizationToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.histogramEqualization(PicturesList[picIndex], PicturesList[tabControl1.SelectedIndex]);
            DisplayImage(PicturesList[picIndex]);
        }

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //In Other Forms
        //Brightness/Contrast
        private void brightnessContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                int picIndex = tabControl1.SelectedIndex;
                Form br = new BrightnessContrastGamma(PicturesList[picIndex]);
                br.Show();
            }
            catch { }
        }

        //Calculations
        private void arithmeticOperationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox pic = new PictureBox();
            pic.Size = new System.Drawing.Size(PicturesList[0].width, PicturesList[0].height);
            pic.Location = new System.Drawing.Point(100, 100);
            PicturesList.Add(new PictureInfo());
            PicturesList[PicturesList.Count - 1].pictureBox = pic;
            Form Calc = new AddSubtract(PicturesList, tabControl1);
            Calc.Show();
        }

        //Quantization
        private void quantizationToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Form br = new Quantization(PicturesList[tabControl1.SelectedIndex]);
            br.Show();
        }

        //Histogram Matching
        private void histogramMatchingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PictureBox pic = new PictureBox();
            pic.Location = new System.Drawing.Point(100, 100);
            PicturesList.Add(new PictureInfo());
            int count = PicturesList.Count - 1;
            PicturesList[count].pictureBox = pic;
            PicturesList[count].name = "untitled";
            HistogramMathcing HG = new HistogramMathcing(PicturesList, tabControl1);
            HG.Show();
        }

        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Histogram Controls
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                UpdateHistogram(PicturesList[tabControl1.SelectedIndex]);
            }
            catch { }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[tabControl1.SelectedIndex]);
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[tabControl1.SelectedIndex]);
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[tabControl1.SelectedIndex]);
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[tabControl1.SelectedIndex]);
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Converting between Spatial & Frequency Domains
        private void convertToSpatialDomainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int picIndex = tabControl1.SelectedIndex;
            if (PicturesList[picIndex].frequency)
            {
                ImageClass Image = new ImageClass();
                Image.ConverttoSpatialDomain(PicturesList[picIndex]);
                DisplayImage(PicturesList[picIndex]);
            }
            else
                MessageBox.Show("Image must be in frequency domain..");
        }
        private void convertToFrequencyDomainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int picIndex = tabControl1.SelectedIndex;
            if (!PicturesList[picIndex].frequency)
            {
                ImageClass Image = new ImageClass();
                Image.convertToFrequencyDomain(PicturesList[picIndex]);
                DisplayImage(PicturesList[picIndex]);
            }
            else
                MessageBox.Show("Image must be in spatial domain..");
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
        
        #region Filters

        //Mean Filter
        private void meanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Mean Filter";
            //Size Text Box
            TextBox sizeTxt = new TextBox();
            sizeTxt.Location = new System.Drawing.Point(15, 42);
            sizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sizeTxt);
            //Size Label
            System.Windows.Forms.Label sizeLabel = new System.Windows.Forms.Label();
            sizeLabel.Location = new System.Drawing.Point(12, 26);
            sizeLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(sizeLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyMeanFilter_Click(sender1, e1, sizeTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyMeanFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int length = int.Parse(sizeText.Text);
            if (length % 2 == 0) length++;
            double[] Mask = new double[length];
            for (int i = 0; i < length; i++)
                Mask[i] = 1.0 / length;
            Filter filter = new Filter();
            filter.Apply1DFilter(length, Mask, PicturesList[tabControl1.SelectedIndex], ref PicturesList[tabControl1.SelectedIndex].redPixels, ref PicturesList[tabControl1.SelectedIndex].greenPixels, ref PicturesList[tabControl1.SelectedIndex].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        //Gaussian Filter
        private void gaussianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Gaussian Filter";
            //Size Text Box
            TextBox sizeTxt = new TextBox();
            sizeTxt.Location = new System.Drawing.Point(15, 42);
            sizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sizeTxt);
            //Size Label
            System.Windows.Forms.Label sizeLabel = new System.Windows.Forms.Label();
            sizeLabel.Location = new System.Drawing.Point(12, 26);
            sizeLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(sizeLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyGaussianFilter_Click(sender1, e1, sizeTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyGaussianFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int count = tabControl1.SelectedIndex;
            double sigma = double.Parse(sizeText.Text);
            int length = (int)(3.7 * sigma - 0.5);
            length = length * 2 + 1;
            double[] Mask = new double[length];
            //double x = length / 2;
            double x = 0;

            for (int i = 0; i < length; i++, x--)
            {
                Mask[i] = (1 / (Math.Sqrt(2 * Math.PI * sigma)) * Math.Exp(-(x * x) / (sigma * sigma)));
            }
            Filter filter = new Filter();
            filter.Apply1DFilter(length, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[count]);
        }

        //Sharpening Filters
        private void laplacianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }
        private void horizontalFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { 0, 1, 0 }, { 0, 1, 0 }, { 0, -1, 0 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }
        private void verticalFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { 0, 0, 0 }, { 1, 1, -1 }, { 0, 0, 0 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }
        private void rightDiagonalFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { 0, 0, 1 }, { 0, 1, 0 }, { -1, 0, 0 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }
        private void leftDiagonalFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, -1 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        //Edge Detection Filters
        private void laplacianFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }
        private void horizontalFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { 1, 1, 1 }, { 1, -2, 1 }, { -1, -1, -1 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }
        private void verticalFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { -1, 1, 1 }, { -1, -2, 1 }, { -1, 1, 1 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }
        private void rightDiagonalFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { 1, 1, 1 }, { -1, -2, 1 }, { -1, -1, 1 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }
        private void leftDiagonalFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            double[,] Mask = new double[3, 3] { { 1, 1, 1 }, { 1, -2, -1 }, { 1, -1, -1 } };
            Filter filter = new Filter();
            int count = tabControl1.SelectedIndex;
            filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        //Convolution
        private void customFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CustomFilter CF = new CustomFilter(PicturesList);
            CF.Show();
        }

        //High-Pass & Low-Pass Filters Menu Item
        private void idealToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Ideal Low-Pass Filter";
            //D Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //D Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "D";
            inputGroupBox.Controls.Add(dLabel);
            //Button
            Button lowBtn = new Button();
            lowBtn.Text = "Apply";
            lowBtn.Location = new System.Drawing.Point(102, 85);
            lowBtn.Size = new System.Drawing.Size(75, 23);
            lowBtn.Click += delegate(object sender1, EventArgs e1) { lowFilterBtn_Click(sender1, e1, dTxt, 0); };
            inputGroupBox.Controls.Add(lowBtn);
        }
        private void butterworthFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Butterworth Low-Pass Filter";
            //D Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //D Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "D";
            inputGroupBox.Controls.Add(dLabel);
            //N Text Box
            TextBox nTxt = new TextBox();
            nTxt.Location = new System.Drawing.Point(158, 42);
            nTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(nTxt);
            //N Label
            System.Windows.Forms.Label nLabel = new System.Windows.Forms.Label();
            nLabel.Location = new System.Drawing.Point(155, 26);
            nLabel.Text = "N";
            inputGroupBox.Controls.Add(nLabel);
            //Button
            Button lowBtn = new Button();
            lowBtn.Text = "Apply";
            lowBtn.Location = new System.Drawing.Point(102, 85);
            lowBtn.Size = new System.Drawing.Size(75, 23);
            lowBtn.Click += delegate(object sender1, EventArgs e1) { lowButterFilterBtn_Click(sender1, e1, dTxt, nTxt, 1); };
            inputGroupBox.Controls.Add(lowBtn);
        }
        private void gaussianFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Gaussian Low-Pass Filter";
            //D Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //D Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "D";
            inputGroupBox.Controls.Add(dLabel);
            //Button
            Button lowBtn = new Button();
            lowBtn.Text = "Apply";
            lowBtn.Location = new System.Drawing.Point(102, 85);
            lowBtn.Size = new System.Drawing.Size(75, 23);
            lowBtn.Click += delegate(object sender1, EventArgs e1) { lowFilterBtn_Click(sender1, e1, dTxt, 2); };
            inputGroupBox.Controls.Add(lowBtn);
        }
        private void idealFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Ideal High-Pass Filter";
            //D Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //D Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "D";
            inputGroupBox.Controls.Add(dLabel);
            //Button
            Button HighBtn = new Button();
            HighBtn.Text = "Apply";
            HighBtn.Location = new System.Drawing.Point(102, 85);
            HighBtn.Size = new System.Drawing.Size(75, 23);
            HighBtn.Click += delegate(object sender1, EventArgs e1) { HighFilterBtn_Click(sender1, e1, dTxt, 0); };
            inputGroupBox.Controls.Add(HighBtn);
        }
        private void butterworthFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Butterworth High-Pass Filter";
            //D Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //D Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "D";
            inputGroupBox.Controls.Add(dLabel);
            //N Text Box
            TextBox nTxt = new TextBox();
            nTxt.Location = new System.Drawing.Point(158, 42);
            nTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(nTxt);
            //N Label
            System.Windows.Forms.Label nLabel = new System.Windows.Forms.Label();
            nLabel.Location = new System.Drawing.Point(155, 26);
            nLabel.Text = "N";
            inputGroupBox.Controls.Add(nLabel);
            //Button
            Button HighBtn = new Button();
            HighBtn.Text = "Apply";
            HighBtn.Location = new System.Drawing.Point(102, 85);
            HighBtn.Size = new System.Drawing.Size(75, 23);
            HighBtn.Click += delegate(object sender1, EventArgs e1) { HighButterFilterBtn_Click(sender1, e1, dTxt, nTxt, 1); };
            inputGroupBox.Controls.Add(HighBtn);
        }
        private void gaussianFilterToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Gaussian High-Pass Filter";
            //D Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //D Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "D";
            inputGroupBox.Controls.Add(dLabel);
            //Button
            Button HighBtn = new Button();
            HighBtn.Text = "Apply";
            HighBtn.Location = new System.Drawing.Point(102, 85);
            HighBtn.Size = new System.Drawing.Size(75, 23);
            HighBtn.Click += delegate(object sender1, EventArgs e1) { HighFilterBtn_Click(sender1, e1, dTxt, 2); };
            inputGroupBox.Controls.Add(HighBtn);
        }

        //High-Pass & Low-Pass Filters
        private void lowFilterBtn_Click(object sender, EventArgs e, TextBox dTxt, int filterType)
        {
            int picIndex = tabControl1.SelectedIndex;
            Filter filter = new Filter();
            filter.LowPassFilters(PicturesList[picIndex], filterType, double.Parse(dTxt.Text), 0);
            DisplayImage(PicturesList[picIndex]);
        }
        private void lowButterFilterBtn_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt, int filterType)
        {
            int picIndex = tabControl1.SelectedIndex;
            Filter filter = new Filter();
            filter.LowPassFilters(PicturesList[picIndex], filterType, double.Parse(dTxt.Text), double.Parse(nTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }
        private void HighFilterBtn_Click(object sender, EventArgs e, TextBox dTxt, int filterType)
        {
            int picIndex = tabControl1.SelectedIndex;
            Filter filter = new Filter();
            filter.HighPassFilters(PicturesList[picIndex], filterType, double.Parse(dTxt.Text), 0);
            DisplayImage(PicturesList[picIndex]);
        }
        private void HighButterFilterBtn_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt, int filterType)
        {
            int picIndex = tabControl1.SelectedIndex;
            Filter filter = new Filter();
            filter.HighPassFilters(PicturesList[picIndex], filterType, double.Parse(dTxt.Text), double.Parse(nTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }


        //Noise Removing Filters
        private void geometricMeanFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Geometric Mean Filter";
            //Size Text Box
            TextBox sizeTxt = new TextBox();
            sizeTxt.Location = new System.Drawing.Point(15, 42);
            sizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sizeTxt);
            //Size Label
            System.Windows.Forms.Label sizeLabel = new System.Windows.Forms.Label();
            sizeLabel.Location = new System.Drawing.Point(12, 26);
            sizeLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(sizeLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyGMeanFilter_Click(sender1, e1, sizeTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyGMeanFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int length = int.Parse(sizeText.Text);
            if (length % 2 == 0) length++;
            Filter filter = new Filter();
            filter.Apply2DGMeanFilter(length, length, PicturesList[tabControl1.SelectedIndex], ref PicturesList[tabControl1.SelectedIndex].redPixels, ref PicturesList[tabControl1.SelectedIndex].greenPixels, ref PicturesList[tabControl1.SelectedIndex].bluePixels);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        private void medianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Median Filter";
            //Size Text Box
            TextBox sizeTxt = new TextBox();
            sizeTxt.Location = new System.Drawing.Point(15, 42);
            sizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sizeTxt);
            //Size Label
            System.Windows.Forms.Label sizeLabel = new System.Windows.Forms.Label();
            sizeLabel.Location = new System.Drawing.Point(12, 26);
            sizeLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(sizeLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyMedianFilter_Click(sender1, e1, sizeTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyMedianFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int length = int.Parse(sizeText.Text);
            if (length % 2 == 0) length++;
            Filter filter = new Filter();
            filter.ApplyOrderStatFilter(length, length, PicturesList[tabControl1.SelectedIndex], ref PicturesList[tabControl1.SelectedIndex].redPixels, ref PicturesList[tabControl1.SelectedIndex].greenPixels, ref PicturesList[tabControl1.SelectedIndex].bluePixels, "Median");
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        private void minimumFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Minimum Filter";
            //Size Text Box
            TextBox sizeTxt = new TextBox();
            sizeTxt.Location = new System.Drawing.Point(15, 42);
            sizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sizeTxt);
            //Size Label
            System.Windows.Forms.Label sizeLabel = new System.Windows.Forms.Label();
            sizeLabel.Location = new System.Drawing.Point(12, 26);
            sizeLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(sizeLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyMinimumFilter_Click(sender1, e1, sizeTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyMinimumFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int length = int.Parse(sizeText.Text);
            if (length % 2 == 0) length++;
            Filter filter = new Filter();
            filter.ApplyOrderStatFilter(length, length, PicturesList[tabControl1.SelectedIndex], ref PicturesList[tabControl1.SelectedIndex].redPixels, ref PicturesList[tabControl1.SelectedIndex].greenPixels, ref PicturesList[tabControl1.SelectedIndex].bluePixels, "Minimum");
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        private void maximumFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Maximum Filter";
            //Size Text Box
            TextBox sizeTxt = new TextBox();
            sizeTxt.Location = new System.Drawing.Point(15, 42);
            sizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sizeTxt);
            //Size Label
            System.Windows.Forms.Label sizeLabel = new System.Windows.Forms.Label();
            sizeLabel.Location = new System.Drawing.Point(12, 26);
            sizeLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(sizeLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyMaximumFilter_Click(sender1, e1, sizeTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyMaximumFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int length = int.Parse(sizeText.Text);
            if (length % 2 == 0) length++;
            Filter filter = new Filter();
            filter.ApplyOrderStatFilter(length, length, PicturesList[tabControl1.SelectedIndex], ref PicturesList[tabControl1.SelectedIndex].redPixels, ref PicturesList[tabControl1.SelectedIndex].greenPixels, ref PicturesList[tabControl1.SelectedIndex].bluePixels, "Maximum");
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        private void midPointFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Mid-Point Filter";
            //Size Text Box
            TextBox sizeTxt = new TextBox();
            sizeTxt.Location = new System.Drawing.Point(15, 42);
            sizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sizeTxt);
            //Size Label
            System.Windows.Forms.Label sizeLabel = new System.Windows.Forms.Label();
            sizeLabel.Location = new System.Drawing.Point(12, 26);
            sizeLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(sizeLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyMidPointFilter_Click(sender1, e1, sizeTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyMidPointFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int length = int.Parse(sizeText.Text);
            if (length % 2 == 0) length++;
            Filter filter = new Filter();
            filter.ApplyOrderStatFilter(length, length, PicturesList[tabControl1.SelectedIndex], ref PicturesList[tabControl1.SelectedIndex].redPixels, ref PicturesList[tabControl1.SelectedIndex].greenPixels, ref PicturesList[tabControl1.SelectedIndex].bluePixels, "MidPoint");
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        private void contraharmonicMeanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Contraharmonic Mean Filter";
            //Size Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //Size Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(dLabel);
            //Q Text Box
            TextBox nTxt = new TextBox();
            nTxt.Location = new System.Drawing.Point(158, 42);
            nTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(nTxt);
            //Q Label
            System.Windows.Forms.Label nLabel = new System.Windows.Forms.Label();
            nLabel.Location = new System.Drawing.Point(155, 26);
            nLabel.Text = "Q";
            inputGroupBox.Controls.Add(nLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyContraharmonicFilter_Click(sender1, e1, dTxt, nTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyContraharmonicFilter_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt)
        {
            int length = int.Parse(dTxt.Text);
            if (length % 2 == 0) length++;
            double Q = double.Parse(nTxt.Text);
            Filter filter = new Filter();
            filter.Apply2DContraharmonicFilter(length, length, PicturesList[tabControl1.SelectedIndex], ref PicturesList[tabControl1.SelectedIndex].redPixels, ref PicturesList[tabControl1.SelectedIndex].greenPixels, ref PicturesList[tabControl1.SelectedIndex].bluePixels, Q);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        private void alphaTrimmedMeanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Contraharmonic Mean Filter";
            //Size Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //Size Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "Mask Size";
            inputGroupBox.Controls.Add(dLabel);
            //Q Text Box
            TextBox nTxt = new TextBox();
            nTxt.Location = new System.Drawing.Point(158, 42);
            nTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(nTxt);
            //Q Label
            System.Windows.Forms.Label nLabel = new System.Windows.Forms.Label();
            nLabel.Location = new System.Drawing.Point(155, 26);
            nLabel.Text = "D";
            inputGroupBox.Controls.Add(nLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyAlphaTrimmedFilter_Click(sender1, e1, dTxt, nTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyAlphaTrimmedFilter_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt)
        {
            int length = int.Parse(dTxt.Text);
            if (length % 2 == 0) length++;
            double D = double.Parse(nTxt.Text);
            Filter filter = new Filter();
            filter.ApplyAlphaTrimmedFilter(length, length, PicturesList[tabControl1.SelectedIndex], ref PicturesList[tabControl1.SelectedIndex].redPixels, ref PicturesList[tabControl1.SelectedIndex].greenPixels, ref PicturesList[tabControl1.SelectedIndex].bluePixels, D);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }


        private void idealToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Ideal Band Reject Filter";
            //D Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //D Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "D";
            inputGroupBox.Controls.Add(dLabel);
            //W Text Box
            TextBox nTxt = new TextBox();
            nTxt.Location = new System.Drawing.Point(158, 42);
            nTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(nTxt);
            //W Label
            System.Windows.Forms.Label nLabel = new System.Windows.Forms.Label();
            nLabel.Location = new System.Drawing.Point(155, 26);
            nLabel.Text = "Band Width";
            inputGroupBox.Controls.Add(nLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyIdealBandRejectFilter_Click(sender1, e1, dTxt, nTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyIdealBandRejectFilter_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt)
        {
            double D = double.Parse(dTxt.Text);
            double w = double.Parse(nTxt.Text);
            Filter filter = new Filter();
            filter.BandFilters(PicturesList[tabControl1.SelectedIndex], 0, D, w);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        private void idealToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Ideal Band Reject Filter";
            //D Text Box
            TextBox dTxt = new TextBox();
            dTxt.Location = new System.Drawing.Point(15, 42);
            dTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(dTxt);
            //D Label
            System.Windows.Forms.Label dLabel = new System.Windows.Forms.Label();
            dLabel.Location = new System.Drawing.Point(12, 26);
            dLabel.Text = "D";
            inputGroupBox.Controls.Add(dLabel);
            //W Text Box
            TextBox nTxt = new TextBox();
            nTxt.Location = new System.Drawing.Point(158, 42);
            nTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(nTxt);
            //W Label
            System.Windows.Forms.Label nLabel = new System.Windows.Forms.Label();
            nLabel.Location = new System.Drawing.Point(155, 26);
            nLabel.Text = "Band Width";
            inputGroupBox.Controls.Add(nLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyIdealBandPassFilter_Click(sender1, e1, dTxt, nTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplyIdealBandPassFilter_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt)
        {
            double D = double.Parse(dTxt.Text);
            double w = double.Parse(nTxt.Text);
            Filter filter = new Filter();
            filter.BandFilters(PicturesList[tabControl1.SelectedIndex], 1, D, w);
            DisplayImage(PicturesList[tabControl1.SelectedIndex]);
        }

        private void idealToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Ideal Notch Reject Filter";
            //X Text Box
            TextBox muTxt = new TextBox();
            muTxt.Location = new System.Drawing.Point(15, 42);
            muTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(muTxt);
            //X Label
            System.Windows.Forms.Label muLabel = new System.Windows.Forms.Label();
            muLabel.Location = new System.Drawing.Point(12, 26);
            muLabel.Text = "X";
            inputGroupBox.Controls.Add(muLabel);
            //Y Text Box
            TextBox sigmaTxt = new TextBox();
            sigmaTxt.Location = new System.Drawing.Point(158, 42);
            sigmaTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sigmaTxt);
            //Y Label
            System.Windows.Forms.Label sigmaLabel = new System.Windows.Forms.Label();
            sigmaLabel.Location = new System.Drawing.Point(155, 26);
            sigmaLabel.Text = "Y";
            inputGroupBox.Controls.Add(sigmaLabel);

            //R Text Box
            TextBox NoisePercentageTxt = new TextBox();
            NoisePercentageTxt.Location = new System.Drawing.Point(90, 91);
            NoisePercentageTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(NoisePercentageTxt);
            //R Label
            System.Windows.Forms.Label NoisePercentageLabel = new System.Windows.Forms.Label();
            NoisePercentageLabel.Location = new System.Drawing.Point(87, 75);
            NoisePercentageLabel.Text = "Radius";
            inputGroupBox.Controls.Add(NoisePercentageLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 130);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyIdealNotchRejectBtn_Click(sender1, e1, muTxt, sigmaTxt, NoisePercentageTxt); };
            inputGroupBox.Controls.Add(applyBtn);

        }
        private void ApplyIdealNotchRejectBtn_Click(object sender, EventArgs e, TextBox muTxt, TextBox sigmaTxt, TextBox NoisePercentageTxt)
        {
            int picIndex = tabControl1.SelectedIndex;
            Filter filter = new Filter();
            filter.NotchFilters(PicturesList[picIndex], 0, double.Parse(muTxt.Text), double.Parse(sigmaTxt.Text), double.Parse(NoisePercentageTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }

        private void idealToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Ideal Notch Pass Filter";
            //X Text Box
            TextBox muTxt = new TextBox();
            muTxt.Location = new System.Drawing.Point(15, 42);
            muTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(muTxt);
            //X Label
            System.Windows.Forms.Label muLabel = new System.Windows.Forms.Label();
            muLabel.Location = new System.Drawing.Point(12, 26);
            muLabel.Text = "X";
            inputGroupBox.Controls.Add(muLabel);
            //Y Text Box
            TextBox sigmaTxt = new TextBox();
            sigmaTxt.Location = new System.Drawing.Point(158, 42);
            sigmaTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sigmaTxt);
            //Y Label
            System.Windows.Forms.Label sigmaLabel = new System.Windows.Forms.Label();
            sigmaLabel.Location = new System.Drawing.Point(155, 26);
            sigmaLabel.Text = "Y";
            inputGroupBox.Controls.Add(sigmaLabel);

            //R Text Box
            TextBox NoisePercentageTxt = new TextBox();
            NoisePercentageTxt.Location = new System.Drawing.Point(90, 91);
            NoisePercentageTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(NoisePercentageTxt);
            //R Label
            System.Windows.Forms.Label NoisePercentageLabel = new System.Windows.Forms.Label();
            NoisePercentageLabel.Location = new System.Drawing.Point(87, 75);
            NoisePercentageLabel.Text = "Radius";
            inputGroupBox.Controls.Add(NoisePercentageLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 130);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyIdealNotchPassBtn_Click(sender1, e1, muTxt, sigmaTxt, NoisePercentageTxt); };
            inputGroupBox.Controls.Add(applyBtn);

        }
        private void ApplyIdealNotchPassBtn_Click(object sender, EventArgs e, TextBox muTxt, TextBox sigmaTxt, TextBox NoisePercentageTxt)
        {
            int picIndex = tabControl1.SelectedIndex;
            Filter filter = new Filter();
            filter.NotchFilters(PicturesList[picIndex], 1, double.Parse(muTxt.Text), double.Parse(sigmaTxt.Text), double.Parse(NoisePercentageTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }

        private void medianFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Adaptive Median Filter";
            //MaxWinSize Text Box
            TextBox MaxWinSizeTxt = new TextBox();
            MaxWinSizeTxt.Location = new System.Drawing.Point(15, 42);
            MaxWinSizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(MaxWinSizeTxt);
            //MaxWinSize Label
            System.Windows.Forms.Label MaxWinSizeLabel = new System.Windows.Forms.Label();
            MaxWinSizeLabel.Location = new System.Drawing.Point(12, 26);
            MaxWinSizeLabel.Size = new System.Drawing.Size(114, 13);
            MaxWinSizeLabel.Text = "Maximum Window Size";
            inputGroupBox.Controls.Add(MaxWinSizeLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 90);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { AdaptiveFilterBtn_Click(sender1, e1, MaxWinSizeTxt, 0); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void meanFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Adaptive Mean Filter";
            //MaxWinSize Text Box
            TextBox MaxWinSizeTxt = new TextBox();
            MaxWinSizeTxt.Location = new System.Drawing.Point(15, 42);
            MaxWinSizeTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(MaxWinSizeTxt);
            //MaxWinSize Label
            System.Windows.Forms.Label MaxWinSizeLabel = new System.Windows.Forms.Label();
            MaxWinSizeLabel.Location = new System.Drawing.Point(12, 26);
            MaxWinSizeLabel.Size = new System.Drawing.Size(114, 13);
            MaxWinSizeLabel.Text = "Maximum Window Size";
            inputGroupBox.Controls.Add(MaxWinSizeLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 90);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { AdaptiveFilterBtn_Click(sender1, e1, MaxWinSizeTxt, 1); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void AdaptiveFilterBtn_Click(object sender, EventArgs e, TextBox MaxWinSize, int type)
        {
            int picIndex = tabControl1.SelectedIndex;
            Filter filter = new Filter();
            filter.AdaptiveFilter(PicturesList[picIndex], int.Parse(MaxWinSize.Text), type);
            DisplayImage(PicturesList[picIndex]);
        }

        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*
        
        #region Adding Noise

        private void saltPepperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Salt and Pepper Noise";
            //Salt Text Box
            TextBox saltTxt = new TextBox();
            saltTxt.Location = new System.Drawing.Point(15, 42);
            saltTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(saltTxt);
            //salt Label
            System.Windows.Forms.Label saltLabel = new System.Windows.Forms.Label();
            saltLabel.Location = new System.Drawing.Point(12, 26);
            saltLabel.Text = "Salt Percentage";
            inputGroupBox.Controls.Add(saltLabel);
            //Pepper Text Box
            TextBox pepperTxt = new TextBox();
            pepperTxt.Location = new System.Drawing.Point(158, 42);
            pepperTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(pepperTxt);
            //Pepper Label
            System.Windows.Forms.Label pepperLabel = new System.Windows.Forms.Label();
            pepperLabel.Location = new System.Drawing.Point(155, 26);
            pepperLabel.Text = "Pepper Percentage";
            inputGroupBox.Controls.Add(pepperLabel);
            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 85);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplySaltPepperBtn_Click(sender1, e1, saltTxt, pepperTxt); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void ApplySaltPepperBtn_Click(object sender, EventArgs e, TextBox saltTxt, TextBox pepperTxt)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.AddSaltPepperNoise(PicturesList[picIndex], double.Parse(saltTxt.Text), double.Parse(pepperTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }

        private void unifromNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Uniform Noise";
            //a Text Box
            TextBox aTxt = new TextBox();
            aTxt.Location = new System.Drawing.Point(15, 42);
            aTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(aTxt);
            //a Label
            System.Windows.Forms.Label aLabel = new System.Windows.Forms.Label();
            aLabel.Location = new System.Drawing.Point(12, 26);
            aLabel.Text = "a";
            inputGroupBox.Controls.Add(aLabel);
            //b Text Box
            TextBox bTxt = new TextBox();
            bTxt.Location = new System.Drawing.Point(158, 42);
            bTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(bTxt);
            //b Label
            System.Windows.Forms.Label bLabel = new System.Windows.Forms.Label();
            bLabel.Location = new System.Drawing.Point(155, 26);
            bLabel.Text = "b";
            inputGroupBox.Controls.Add(bLabel);

            //NoisePercentage Text Box
            TextBox NoisePercentageTxt = new TextBox();
            NoisePercentageTxt.Location = new System.Drawing.Point(90, 91);
            NoisePercentageTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(NoisePercentageTxt);
            //NoisePercentage Label
            System.Windows.Forms.Label NoisePercentageLabel = new System.Windows.Forms.Label();
            NoisePercentageLabel.Location = new System.Drawing.Point(87, 75);
            NoisePercentageLabel.Text = "Noise Percentage";
            inputGroupBox.Controls.Add(NoisePercentageLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 130);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyUnifromNoiseBtn_Click(sender1, e1, aTxt, bTxt, NoisePercentageTxt); };
            inputGroupBox.Controls.Add(applyBtn);

        }
        private void ApplyUnifromNoiseBtn_Click(object sender, EventArgs e, TextBox aTxt, TextBox bTxt, TextBox NoisePercentageTxt)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.AddUnifromNoise(PicturesList[picIndex], int.Parse(aTxt.Text), int.Parse(bTxt.Text), double.Parse(NoisePercentageTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }

        private void gaussianNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Gaussian Noise";
            //Mu Text Box
            TextBox muTxt = new TextBox();
            muTxt.Location = new System.Drawing.Point(15, 42);
            muTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(muTxt);
            //Mu Label
            System.Windows.Forms.Label muLabel = new System.Windows.Forms.Label();
            muLabel.Location = new System.Drawing.Point(12, 26);
            muLabel.Text = "Mean";
            inputGroupBox.Controls.Add(muLabel);
            //sigma Text Box
            TextBox sigmaTxt = new TextBox();
            sigmaTxt.Location = new System.Drawing.Point(158, 42);
            sigmaTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(sigmaTxt);
            //sigma Label
            System.Windows.Forms.Label sigmaLabel = new System.Windows.Forms.Label();
            sigmaLabel.Location = new System.Drawing.Point(155, 26);
            sigmaLabel.Text = "Sigma";
            inputGroupBox.Controls.Add(sigmaLabel);

            //NoisePercentage Text Box
            TextBox NoisePercentageTxt = new TextBox();
            NoisePercentageTxt.Location = new System.Drawing.Point(90, 91);
            NoisePercentageTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(NoisePercentageTxt);
            //NoisePercentage Label
            System.Windows.Forms.Label NoisePercentageLabel = new System.Windows.Forms.Label();
            NoisePercentageLabel.Location = new System.Drawing.Point(87, 75);
            NoisePercentageLabel.Text = "Noise Percentage";
            inputGroupBox.Controls.Add(NoisePercentageLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 130);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyGaussianNoiseBtn_Click(sender1, e1, muTxt, sigmaTxt, NoisePercentageTxt); };
            inputGroupBox.Controls.Add(applyBtn);

        }
        private void ApplyGaussianNoiseBtn_Click(object sender, EventArgs e, TextBox muTxt, TextBox sigmaTxt, TextBox NoisePercentageTxt)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.AddGaussianNoise(PicturesList[picIndex], int.Parse(muTxt.Text), int.Parse(sigmaTxt.Text), double.Parse(NoisePercentageTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }

        private void rayleighNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {

            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Rayleigh Noise";
            //a Text Box
            TextBox aTxt = new TextBox();
            aTxt.Location = new System.Drawing.Point(15, 42);
            aTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(aTxt);
            //a Label
            System.Windows.Forms.Label aLabel = new System.Windows.Forms.Label();
            aLabel.Location = new System.Drawing.Point(12, 26);
            aLabel.Text = "a";
            inputGroupBox.Controls.Add(aLabel);
            //b Text Box
            TextBox bTxt = new TextBox();
            bTxt.Location = new System.Drawing.Point(158, 42);
            bTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(bTxt);
            //b Label
            System.Windows.Forms.Label bLabel = new System.Windows.Forms.Label();
            bLabel.Location = new System.Drawing.Point(155, 26);
            bLabel.Text = "b";
            inputGroupBox.Controls.Add(bLabel);

            //NoisePercentage Text Box
            TextBox NoisePercentageTxt = new TextBox();
            NoisePercentageTxt.Location = new System.Drawing.Point(90, 91);
            NoisePercentageTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(NoisePercentageTxt);
            //NoisePercentage Label
            System.Windows.Forms.Label NoisePercentageLabel = new System.Windows.Forms.Label();
            NoisePercentageLabel.Location = new System.Drawing.Point(87, 75);
            NoisePercentageLabel.Text = "Noise Percentage";
            inputGroupBox.Controls.Add(NoisePercentageLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 130);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { RayleighBtn_Click(sender1, e1, aTxt, bTxt, NoisePercentageTxt); };
            inputGroupBox.Controls.Add(applyBtn);

        }
        private void RayleighBtn_Click(object sender, EventArgs e, TextBox aTxt, TextBox bTxt, TextBox NoisePercentageTxt)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.AddRayleighNoise(PicturesList[picIndex], int.Parse(aTxt.Text), int.Parse(bTxt.Text), double.Parse(NoisePercentageTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }

        private void gammaNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Gamma Noise";
            //a Text Box
            TextBox aTxt = new TextBox();
            aTxt.Location = new System.Drawing.Point(15, 42);
            aTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(aTxt);
            //a Label
            System.Windows.Forms.Label aLabel = new System.Windows.Forms.Label();
            aLabel.Location = new System.Drawing.Point(12, 26);
            aLabel.Text = "a";
            inputGroupBox.Controls.Add(aLabel);
            //b Text Box
            TextBox bTxt = new TextBox();
            bTxt.Location = new System.Drawing.Point(158, 42);
            bTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(bTxt);
            //b Label
            System.Windows.Forms.Label bLabel = new System.Windows.Forms.Label();
            bLabel.Location = new System.Drawing.Point(155, 26);
            bLabel.Text = "b";
            inputGroupBox.Controls.Add(bLabel);

            //NoisePercentage Text Box
            TextBox NoisePercentageTxt = new TextBox();
            NoisePercentageTxt.Location = new System.Drawing.Point(90, 91);
            NoisePercentageTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(NoisePercentageTxt);
            //NoisePercentage Label
            System.Windows.Forms.Label NoisePercentageLabel = new System.Windows.Forms.Label();
            NoisePercentageLabel.Location = new System.Drawing.Point(87, 75);
            NoisePercentageLabel.Text = "Noise Percentage";
            inputGroupBox.Controls.Add(NoisePercentageLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 130);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { GammaNoiseBtn_Click(sender1, e1, aTxt, bTxt, NoisePercentageTxt); };
            inputGroupBox.Controls.Add(applyBtn);

        }
        private void GammaNoiseBtn_Click(object sender, EventArgs e, TextBox aTxt, TextBox bTxt, TextBox NoisePercentageTxt)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.AddGammaNoise(PicturesList[picIndex], int.Parse(aTxt.Text), int.Parse(bTxt.Text), double.Parse(NoisePercentageTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }

        private void exponentialNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Exponential Noise";
            //a Text Box
            TextBox aTxt = new TextBox();
            aTxt.Location = new System.Drawing.Point(15, 42);
            aTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(aTxt);
            //a Label
            System.Windows.Forms.Label aLabel = new System.Windows.Forms.Label();
            aLabel.Location = new System.Drawing.Point(12, 26);
            aLabel.Text = "a";
            inputGroupBox.Controls.Add(aLabel);

            //NoisePercentage Text Box
            TextBox NoisePercentageTxt = new TextBox();
            NoisePercentageTxt.Location = new System.Drawing.Point(90, 91);
            NoisePercentageTxt.Size = new System.Drawing.Size(100, 20);
            inputGroupBox.Controls.Add(NoisePercentageTxt);
            //NoisePercentage Label
            System.Windows.Forms.Label NoisePercentageLabel = new System.Windows.Forms.Label();
            NoisePercentageLabel.Location = new System.Drawing.Point(87, 75);
            NoisePercentageLabel.Text = "Noise Percentage";
            inputGroupBox.Controls.Add(NoisePercentageLabel);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 130);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { ExponentialNoiseBtn_Click(sender1, e1, aTxt, NoisePercentageTxt); };
            inputGroupBox.Controls.Add(applyBtn);

        }
        private void ExponentialNoiseBtn_Click(object sender, EventArgs e, TextBox aTxt, TextBox NoisePercentageTxt)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.AddExponentialNoise(PicturesList[picIndex], int.Parse(aTxt.Text), double.Parse(NoisePercentageTxt.Text));
            DisplayImage(PicturesList[picIndex]);
        }

        private void periodicNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int height = PicturesList[tabControl1.SelectedIndex].height;
            int width = PicturesList[tabControl1.SelectedIndex].width;
            inputGroupBox.Controls.Clear();
            inputGroupBox.Text = "Periodic Noise";
            //Amplitude Label
            System.Windows.Forms.Label AmplitudeLabel = new System.Windows.Forms.Label();
            AmplitudeLabel.Location = new System.Drawing.Point(27, 26);
            AmplitudeLabel.Text = "Amplitude";
            inputGroupBox.Controls.Add(AmplitudeLabel);
            //Amplitude NumericUpDown
            NumericUpDown AmpNumericUpDown = new NumericUpDown();
            AmpNumericUpDown.Location = new System.Drawing.Point(130, 24);
            AmpNumericUpDown.DecimalPlaces = 2;
            AmpNumericUpDown.Minimum = 0;
            AmpNumericUpDown.Maximum = 255;
            inputGroupBox.Controls.Add(AmpNumericUpDown);

            //xFreq Label
            System.Windows.Forms.Label xFreqLabel = new System.Windows.Forms.Label();
            xFreqLabel.Location = new System.Drawing.Point(27, 56);
            xFreqLabel.Text = "X-Frequency";
            inputGroupBox.Controls.Add(xFreqLabel);
            //xFreq NumericUpDown
            NumericUpDown xFreqNumericUpDown = new NumericUpDown();
            xFreqNumericUpDown.Location = new System.Drawing.Point(130, 54);
            xFreqNumericUpDown.DecimalPlaces = 2;
            xFreqNumericUpDown.Minimum = 0;
            xFreqNumericUpDown.Maximum = width / 2;
            inputGroupBox.Controls.Add(xFreqNumericUpDown);

            //yFreq Label
            System.Windows.Forms.Label yFreqLabel = new System.Windows.Forms.Label();
            yFreqLabel.Location = new System.Drawing.Point(27, 86);
            yFreqLabel.Text = "Y-Frequency";
            inputGroupBox.Controls.Add(yFreqLabel);
            //yFreq NumericUpDown
            NumericUpDown yFreqNumericUpDown = new NumericUpDown();
            yFreqNumericUpDown.Location = new System.Drawing.Point(130, 84);
            yFreqNumericUpDown.DecimalPlaces = 2;
            yFreqNumericUpDown.Minimum = 0;
            yFreqNumericUpDown.Maximum = height / 2;
            inputGroupBox.Controls.Add(yFreqNumericUpDown);

            //xPhase Label
            System.Windows.Forms.Label xPhaseLabel = new System.Windows.Forms.Label();
            xPhaseLabel.Location = new System.Drawing.Point(27, 116);
            xPhaseLabel.Text = "X-Phase Shift";
            inputGroupBox.Controls.Add(xPhaseLabel);
            //xFreq NumericUpDown
            NumericUpDown xPhaseNumericUpDown = new NumericUpDown();
            xPhaseNumericUpDown.Location = new System.Drawing.Point(130, 114);
            xPhaseNumericUpDown.DecimalPlaces = 2;
            xPhaseNumericUpDown.Minimum = 0;
            xPhaseNumericUpDown.Maximum = (int)(2 * Math.PI);
            inputGroupBox.Controls.Add(xPhaseNumericUpDown);

            //yPhase Label
            System.Windows.Forms.Label yPhaseLabel = new System.Windows.Forms.Label();
            yPhaseLabel.Location = new System.Drawing.Point(27, 146);
            yPhaseLabel.Text = "Y-Phase Shift";
            inputGroupBox.Controls.Add(yPhaseLabel);
            //yFreq NumericUpDown
            NumericUpDown yPhaseNumericUpDown = new NumericUpDown();
            yPhaseNumericUpDown.Location = new System.Drawing.Point(130, 144);
            yPhaseNumericUpDown.DecimalPlaces = 2;
            yPhaseNumericUpDown.Minimum = 0;
            yPhaseNumericUpDown.Maximum = (int)(2 * Math.PI);
            inputGroupBox.Controls.Add(yPhaseNumericUpDown);

            //Button
            Button applyBtn = new Button();
            applyBtn.Text = "Apply";
            applyBtn.Location = new System.Drawing.Point(102, 180);
            applyBtn.Size = new System.Drawing.Size(75, 23);
            applyBtn.Click += delegate(object sender1, EventArgs e1) { AddPeriodicNoiseBtn_Click(sender1, e1, (double)AmpNumericUpDown.Value, (double)xFreqNumericUpDown.Value, (double)yFreqNumericUpDown.Value, (double)xPhaseNumericUpDown.Value, (double)yPhaseNumericUpDown.Value); };
            inputGroupBox.Controls.Add(applyBtn);
        }
        private void AddPeriodicNoiseBtn_Click(object sender, EventArgs e, double Amp, double xFreq, double yFreq, double xPhase, double yPhase)
        {
            int picIndex = tabControl1.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.AddPeriodicNoise(PicturesList[picIndex], Amp, xFreq, yFreq, xPhase, yPhase);
            DisplayImage(PicturesList[picIndex]);
        }

        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

    }
}