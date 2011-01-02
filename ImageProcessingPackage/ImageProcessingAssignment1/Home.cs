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
        List<UndoRedo> PicUndoRedo;
        GroupBox inputGroupBox;
        int TabPagesCount;
        public Home()
        {
            InitializeComponent();
            Thread t = new Thread(new ThreadStart(SplashScreen));
            t.Start();
            Thread.Sleep(2000);
            PicturesList = new List<PictureInfo>();
            PicUndoRedo = new List<UndoRedo>();
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
            inputGroupBox.Location = new System.Drawing.Point(5, 5);
            inputGroupBox.Size = new System.Drawing.Size(280, 240);
            inputGroupBox.ForeColor = Color.White;
            TabPagesCount = 0;
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region UpdateForm
        private void DisplayImage(PictureInfo pic)
        {
            int width = pic.width;
            int height = pic.height;
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
                        p[0] = pic.bluePixels[i, j];
                        p[1] = pic.greenPixels[i, j];
                        p[2] = pic.redPixels[i, j];
                        p += 3;
                    }
                    p += space;
                }
            }
            bmp.UnlockBits(bmpData);
            ImageStatusLabel.Text = pic.width.ToString() + " X " + pic.height.ToString() + " || " + pic.path.ToString();
            pic.pictureBox.Size = new System.Drawing.Size(width, height);
            pic.pictureBox.Image = bmp;
            UpdatePictureBoxLocation();
            UpdateHistogram(pic);
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
            HistogramZedGraphControl.GraphPane.CurveList.Clear();
            GraphPane HistogramPane = HistogramZedGraphControl.GraphPane;
            if (checkBox2.Checked)
            {
                PointPairList RedPoints = new PointPairList(X_axis, R);
                LineItem RedLine = HistogramPane.AddCurve("", RedPoints, Color.Red, SymbolType.None);
                RedLine.Line.Fill = new Fill(Color.White, Color.Red, 45F);
                RedLine.Line.Width = 1.0F;
            }
            if (checkBox3.Checked)
            {
                PointPairList GreenPoints = new PointPairList(X_axis, G);
                LineItem GreenLine = HistogramPane.AddCurve("", GreenPoints, Color.Green, SymbolType.None);
                GreenLine.Line.Fill = new Fill(Color.White, Color.Green, 45F);
                GreenLine.Line.Width = 1.0F;
            }
            if (checkBox4.Checked)
            {
                PointPairList BluePoints = new PointPairList(X_axis, B);
                LineItem BlueLine = HistogramPane.AddCurve("", BluePoints, Color.Blue, SymbolType.None);
                BlueLine.Line.Fill = new Fill(Color.White, Color.Blue, 45F);
                BlueLine.Line.Width = 1.0F;
            }
            if (checkBox1.Checked)
            {
                PointPairList GrayPoints = new PointPairList(X_axis, Gray);
                LineItem GrayLine = HistogramPane.AddCurve("", GrayPoints, Color.Gray, SymbolType.None);
                GrayLine.Line.Fill = new Fill(Color.White, Color.Gray, 45F);
                GrayLine.Line.Width = 1.0F;
            }

            HistogramPane.Title.Text = "";
            HistogramZedGraphControl.Refresh();
            HistogramZedGraphControl.AxisChange();
            HistogramZedGraphControl.Invalidate();
            HistogramPane.XAxis.IsVisible = false;
            HistogramPane.YAxis.IsVisible = false;
            HistogramPictureBox.Image = HistogramZedGraphControl.GetImage();
            //Bitmap bmp = new Bitmap(HistogramPictureBox.Image);
            //HistogramPictureBox.Image = bmp.Clone(new Rectangle(20, 20, 100, 100), bmp.PixelFormat);
        }

        private void Zoom(PictureInfo pic, int ratio)
        {
            zoomLabel.Text = "Zoom: " + ratio.ToString() + "%";
            pic.pictureBox.Width = ((ratio * pic.width) / 100);
            pic.pictureBox.Height = ((ratio * pic.height) / 100);
            int picIndex = ImageTabControl.SelectedIndex;
            UpdatePictureBoxLocation();
        }

        private void UpdatePictureBoxLocation()
        {
            int picIndex = ImageTabControl.SelectedIndex;
            int picBoxWidth = PicturesList[picIndex].pictureBox.Width;
            int picBoxHeight = PicturesList[picIndex].pictureBox.Height;
            int tabPageWidth = ImageTabControl.TabPages[picIndex].Width;
            int tabPageHeight = ImageTabControl.TabPages[picIndex].Height;
            int widthDim = tabPageWidth / 2 - picBoxWidth / 2;
            int heightDim = tabPageHeight / 2 - picBoxHeight / 2;
            if (widthDim < 0) widthDim = 0;
            if (heightDim < 0) heightDim = 0;
            PicturesList[picIndex].pictureBox.Location = new System.Drawing.Point(widthDim, heightDim);
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region MDIMenuItems
        private void ImageTabControl_ControlAdded(object sender, ControlEventArgs e)
        {
            if (TabPagesCount != ImageTabControl.TabPages.Count)
                TabPagesCount = ImageTabControl.TabPages.Count;
            if (PicUndoRedo.Count != TabPagesCount)
            {
                PicUndoRedo.Add(new UndoRedo(PicturesList[TabPagesCount - 1], "Load"));
                HistoryTabPage.Controls.Clear();
                HistoryTabPage.Controls.Add(PicUndoRedo[TabPagesCount - 1].undoRedoListBox);
                PicUndoRedo[TabPagesCount - 1].undoRedoListBox.Location = new System.Drawing.Point(7, 10);
                PicUndoRedo[TabPagesCount - 1].undoRedoListBox.SelectedIndexChanged += new EventHandler(undoRedoListBox_SelectedIndexChanged);
            }
        }
        //Undo Redo
        public void undoRedoListBox_ControlAdded(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[ImageTabControl.SelectedIndex]);
        }
        public void undoRedoListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (TabPagesCount > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                int count = PicUndoRedo[picIndex].Pointer - 1 - PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex;
                if (count >= 0)
                    for (int i = 0; i < count + 1; i++)
                        UndoAction();
                else
                    for (int i = 0; i < Math.Abs(count + 1); i++)
                        RedoAction();
                if (PicUndoRedo[picIndex].undo.Count == 0) undoToolStripMenuItem.Enabled = false;
                else undoToolStripMenuItem.Enabled = true;
                if (PicUndoRedo[picIndex].redo.Count == 0) redoToolStripMenuItem.Enabled = false;
                else redoToolStripMenuItem.Enabled = true;
                Zoom(PicUndoRedo[picIndex].selectedPic[PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex], ZoomTrackBar.Value);
                UpdateHistogram(PicturesList[picIndex]);
            }
        }

        private void UndoAction()
        {
            if (TabPagesCount > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                if (PicUndoRedo[picIndex].undo.Count > 0)
                {
                    PicUndoRedo[picIndex].redo.Push(PicUndoRedo[picIndex].undo.Pop());
                    PicturesList[picIndex] = new PictureInfo(PicUndoRedo[picIndex].selectedPic[PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex]);
                    DisplayImage(PicturesList[picIndex]);
                    PicUndoRedo[picIndex].Pointer--;
                }
            }
        }
        private void RedoAction()
        {
            if (TabPagesCount > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                if (PicUndoRedo[picIndex].redo.Count > 0)
                {
                    PicUndoRedo[picIndex].undo.Push(PicUndoRedo[picIndex].redo.Pop());
                    PicturesList[picIndex] = new PictureInfo(PicUndoRedo[picIndex].selectedPic[PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex]);
                    DisplayImage(PicturesList[picIndex]);
                    PicUndoRedo[picIndex].Pointer++;
                }
            }
        }
        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TabPagesCount > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                if (PicUndoRedo[picIndex].undo.Count > 0)
                    if (PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex > 0)
                        PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex--;
            }
        }
        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (TabPagesCount > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                if (PicUndoRedo[picIndex].redo.Count > 0)
                    if (PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex < PicUndoRedo[picIndex].undoRedoListBox.Items.Count - 1)
                        PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex++;
            }
        }

        private void restoreOriginalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                int length = PicUndoRedo[picIndex].selectedPic.Count - 1;
                PicturesList[picIndex] = new PictureInfo(PicUndoRedo[picIndex].selectedPic[0]);
                for (int i = length; i > 0; i--)
                {
                    PicUndoRedo[picIndex].done.RemoveAt(i);
                    PicUndoRedo[picIndex].Pointer--;
                    PicUndoRedo[picIndex].undo.Pop();
                    PicUndoRedo[picIndex].selectedPic.RemoveAt(i);
                }
                PicUndoRedo[picIndex].undoRedoListBox.Items.Clear();
                PicUndoRedo[picIndex].undoRedoListBox.Items.Add("Load");
                PicUndoRedo[picIndex].undoRedoListBox.SelectedIndex = 0;
                DisplayImage(PicturesList[picIndex]);
                UpdateHistogram(PicturesList[picIndex]);
            }
        }
        //Zooming
        private void ZoomTrackBar_ValueChanged(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
                Zoom(PicturesList[ImageTabControl.SelectedIndex], ZoomTrackBar.Value);
            else
                ZoomTrackBar.Value = 5;
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)//Close
        {
            int PicIndex = ImageTabControl.SelectedIndex;
            ImageTabControl.TabPages.RemoveAt(PicIndex);
            PicUndoRedo.RemoveAt(PicIndex);
            HistoryTabPage.Controls.Clear();
            PicturesList.RemoveAt(PicIndex);
            if (ImageTabControl.TabPages.Count > 0)
            {
                if (PicIndex != 0)
                    ImageTabControl.SelectedIndex = PicIndex - 1;
            }
            else
            {
                HistogramPictureBox.Visible = false;
                checkBox1.Visible = false;
                checkBox2.Visible = false;
                checkBox3.Visible = false;
                checkBox4.Visible = false;
                ImageTabControl.Visible = false;
                ImageStatusLabel.Text = "No Image..";
            }

        }
        private void toolStripMenuItem1_Click(object sender, EventArgs e)//CloseAll
        {
            ImageTabControl.TabPages.Clear();
            ImageTabControl.Visible = false;
            PicturesList.Clear();
            PicUndoRedo.Clear();
            HistoryTabPage.Controls.Clear();
            HistogramZedGraphControl.GraphPane.CurveList.Clear();
            HistogramPictureBox.Visible = false;
            checkBox1.Visible = false;
            checkBox2.Visible = false;
            checkBox3.Visible = false;
            checkBox4.Visible = false;
            ImageStatusLabel.Text = "No Image..";
        }
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string[] PicturePath = new string[20];
                OpenFileDialog Picture = new OpenFileDialog();
                Picture.Filter = "All Files (*.*)|*.*";
                Picture.Multiselect = true;
                if (Picture.ShowDialog() == DialogResult.OK)
                    PicturePath = Picture.FileNames;

                int count = PicturePath.Count();
                for (int k = 0; k < count; k++)
                {
                    PictureInfo newPictureItem = new PictureInfo();
                    PictureBox picBox = new PictureBox();
                    picBox.SizeMode = PictureBoxSizeMode.Zoom;
                    string PictureName = PicturePath[k].Substring(PicturePath[k].LastIndexOf('\\') + 1);
                    int offset = PictureName.LastIndexOf('.') + 1;
                    string type = PictureName.Substring(offset, PictureName.Length - offset);
                    ImageClass image = new ImageClass();
                    if (type.ToLower() == "ppm")
                        image.openPPM(PicturePath[k], ref newPictureItem, PictureName, picBox);
                    else
                        image.OpenImage(PicturePath[k], ref newPictureItem, PictureName, picBox);
                    PicturesList.Add(newPictureItem);
                    int index = PicturesList.Count - 1;
                    if (!ImageTabControl.Visible) ImageTabControl.Visible = true;
                    TabPage tabPage = new TabPage();
                    HistogramPictureBox.Visible = true;
                    PicturesList[index].pictureBox.Size = new System.Drawing.Size(PicturesList[index].width, PicturesList[index].height);
                    ImageTabControl.TabPages.Add(tabPage);
                    tabPage.BackColor = System.Drawing.Color.FromArgb(100, 100, 100);
                    ImageTabControl.TabPages[index].Text = PictureName;
                    ImageTabControl.SelectedIndex = index;
                    tabPage.AutoScroll = true;
                    tabPage.Controls.Add(PicturesList[index].pictureBox);
                    DisplayImage(PicturesList[index]);
                }
                ZoomTrackBar.Value = 100;
            }
            catch { }
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int picIndex = ImageTabControl.SelectedIndex;
            string type = PicturesList[picIndex].name.Substring(PicturesList[picIndex].name.LastIndexOf('.') + 1);
            string path = PicturesList[picIndex].path;
            if (type.ToLower() == "bmp")
                PicturesList[picIndex].pictureBox.Image.Save(path, ImageFormat.Bmp);
            else if (type.ToLower() == "jpeg")
                PicturesList[picIndex].pictureBox.Image.Save(path, ImageFormat.Jpeg);
            else if (type.ToLower() == "ppm")
            {
                ImageClass Image = new ImageClass();
                Image.SaveAsPPM(path, PicturesList[picIndex], PicturesList[picIndex].type);
            }
        }
        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int picIndex = ImageTabControl.SelectedIndex;
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
                    Image.SaveAsPPM(PicturePath, PicturesList[picIndex], PicturesList[picIndex].type);
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
            NewFormInput NFI = new NewFormInput(PicturesList, ImageTabControl);
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

        private void HistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistogramTabControl.Visible = HistogramToolStripMenuItem.Checked;
        }

        private void HistoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            HistoryTabControl.Visible = HistoryToolStripMenuItem.Checked;
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
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Image operations
        //Translating
        private void translateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Image Tanslation";
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
        }
        private void TranslateButton_Click(object sender, EventArgs e, TextBox xTranslation, TextBox yTranslation)
        {
            int PicIndex = ImageTabControl.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.TranslateImage(PicturesList[PicIndex], int.Parse(xTranslation.Text), int.Parse(yTranslation.Text));
            PicUndoRedo[PicIndex].UndoRedoCommands(PicturesList[PicIndex], "Translate");
            DisplayImage(PicturesList[PicIndex]);
        }

        //Rotating
        private void rotateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Image Rotation";
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
        }
        private void RotateButton_Click(object sender, EventArgs e, TextBox Theta)
        {
            int PicIndex = ImageTabControl.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.RotateImage(PicturesList[PicIndex], double.Parse(Theta.Text));
            PicUndoRedo[PicIndex].UndoRedoCommands(PicturesList[PicIndex], "Rotate");
            DisplayImage(PicturesList[PicIndex]);
        }

        //Shearing
        private void shearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
                ShearBtn.Location = new System.Drawing.Point(102, 85);
                ShearBtn.Size = new System.Drawing.Size(75, 23);
                ShearBtn.Click += delegate(object sender1, EventArgs e1) { ShearButton_Click(sender1, e1, shearTxt); };
                inputGroupBox.Controls.Add(ShearBtn);
            }
        }
        private void ShearButton_Click(object sender, EventArgs e, TextBox Shear)
        {
            int PicIndex = ImageTabControl.SelectedIndex;
            ImageClass Image = new ImageClass();
            Image.ShearImage(PicturesList[PicIndex], int.Parse(Shear.Text));
            PicUndoRedo[PicIndex].UndoRedoCommands(PicturesList[PicIndex], "Shear");
            DisplayImage(PicturesList[PicIndex]);
        }

        //Flipping
        private void horizontalFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.FlipImage(PicturesList[picIndex], 0);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Horizontal Flip");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void verticalFlipToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.FlipImage(PicturesList[picIndex], 1);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Vertical Flip");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        //Resizing
        private void bilinearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Image Resizing";

                //Height Text Box
                TextBox hTxt = new TextBox();
                hTxt.Location = new System.Drawing.Point(15, 42);
                hTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(hTxt);
                //Height Label
                System.Windows.Forms.Label hLabel = new System.Windows.Forms.Label();
                hLabel.Location = new System.Drawing.Point(12, 26);
                hLabel.Text = "New Width";
                inputGroupBox.Controls.Add(hLabel);

                //Width Text Box
                TextBox wTxt = new TextBox();
                wTxt.Location = new System.Drawing.Point(158, 42);
                wTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(wTxt);
                //Width Label
                System.Windows.Forms.Label wLabel = new System.Windows.Forms.Label();
                wLabel.Location = new System.Drawing.Point(155, 26);
                wLabel.Text = "New Height";
                inputGroupBox.Controls.Add(wLabel);

                //Button
                Button ResizeBtn = new Button();
                ResizeBtn.Text = "Resize";
                ResizeBtn.Location = new System.Drawing.Point(102, 85);
                ResizeBtn.Size = new System.Drawing.Size(75, 23);
                ResizeBtn.Click += delegate(object sender1, EventArgs e1) { ResizeButton_Click(sender1, e1, wTxt, hTxt); };
                inputGroupBox.Controls.Add(ResizeBtn);
            }
        }
        private void ResizeButton_Click(object sender, EventArgs e, TextBox NewHBox, TextBox NewWBox)
        {
            int PicIndex = ImageTabControl.SelectedIndex;
            ImageClass Image = new ImageClass();
            Zoom(PicturesList[PicIndex], 100);
            DateTime BeginTime = DateTime.Now;
            Image.ResizeImage(PicturesList[PicIndex], int.Parse(NewHBox.Text), int.Parse(NewWBox.Text));
            DisplayImage(PicturesList[PicIndex]);
            DateTime EndTime = DateTime.Now;
            PicUndoRedo[PicIndex].UndoRedoCommands(PicturesList[PicIndex], "Resize");
            TimeSpan TimeTaken = EndTime - BeginTime;
            TimeForm Time = new TimeForm("Resize", TimeTaken.ToString());
            Time.Show();
        }

        //Logical Operations -- NOT
        private void reverseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int PictureIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.ReverseColors(PicturesList[PictureIndex]);
                PicUndoRedo[PictureIndex].UndoRedoCommands(PicturesList[PictureIndex], "Reverse Colors");
                DisplayImage(PicturesList[PictureIndex]);
            }
        }

        //Gray Scale
        private void grayScaleToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.GrayScale(PicturesList[picIndex]);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Gray Scale");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        //Equalization
        //Global
        private void globalEqualizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.histogramEqualization(PicturesList[picIndex], PicturesList[ImageTabControl.SelectedIndex]);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Histogram Equalization");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        //Local using Matlab...
        private void localEqualizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Local Histogram Equalization";
                //Window Size Text Box
                TextBox sizeTxt = new TextBox();
                sizeTxt.Location = new System.Drawing.Point(15, 42);
                sizeTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(sizeTxt);
                //Size Label
                System.Windows.Forms.Label sizeLabel = new System.Windows.Forms.Label();
                sizeLabel.Location = new System.Drawing.Point(12, 26);
                sizeLabel.Text = "Window Size";
                inputGroupBox.Controls.Add(sizeLabel);
                //Button
                Button applyBtn = new Button();
                applyBtn.Text = "Apply";
                applyBtn.Location = new System.Drawing.Point(102, 85);
                applyBtn.Size = new System.Drawing.Size(75, 23);
                applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyLocalHE_Click(sender1, e1, sizeTxt); };
                inputGroupBox.Controls.Add(applyBtn);
            }
        }
        private void ApplyLocalHE_Click(object sender, EventArgs e, TextBox sizeText)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = int.Parse(sizeText.Text);
                if (length % 2 == 0) length++;
                ImageClass Image = new ImageClass();
                int picIndex = ImageTabControl.SelectedIndex;
                Image.LocalHE(PicturesList[picIndex], length);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Local Histogram Equalization");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        //In Other Forms
        //Brightness/Contrast
        private void brightnessContrastToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Form br = new BrightnessContrastGamma(PicturesList[picIndex], PicUndoRedo[picIndex]);
                br.Show();
            }
        }

        //Calculations
        private void arithmeticOperationsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                PictureBox pic = new PictureBox();
                pic.Size = new System.Drawing.Size(PicturesList[0].width, PicturesList[0].height);
                pic.Location = new System.Drawing.Point(100, 100);
                PicturesList.Add(new PictureInfo());
                PicturesList[PicturesList.Count - 1].pictureBox = pic;
                Form Calc = new Calculations(PicturesList, ImageTabControl);
                Calc.Show();
            }
        }

        //Quantization
        private void quantizationToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Form Quantize = new Quantization(PicturesList[picIndex], PicUndoRedo[picIndex]);
                Quantize.Show();
            }
        }

        //Histogram Matching
        private void histogramMatchingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                PictureBox pic = new PictureBox();
                pic.Location = new System.Drawing.Point(100, 100);
                PicturesList.Add(new PictureInfo());
                int count = PicturesList.Count - 1;
                PicturesList[count].pictureBox = pic;
                HistogramMathcing HG = new HistogramMathcing(PicturesList, ImageTabControl);
                HG.Show();
            }
        }

        //Binarization
        private void binarizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Form binarization = new Binarization(PicturesList[picIndex], PicUndoRedo[picIndex]);
                binarization.Show();
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Histogram Controls
        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                int picIndex = ImageTabControl.SelectedIndex;
                UpdateHistogram(PicturesList[picIndex]);
                ImageStatusLabel.Text = PicturesList[picIndex].width.ToString() + " X " + PicturesList[picIndex].height.ToString() + " || " + PicturesList[picIndex].path.ToString();
                HistoryTabPage.Controls.Clear();
                HistoryTabPage.Controls.Add(PicUndoRedo[ImageTabControl.SelectedIndex].undoRedoListBox);
                ZoomTrackBar.Value = (int)Math.Ceiling((PicturesList[picIndex].pictureBox.Width * 100.0) / PicturesList[picIndex].width);
            }
            catch { }
        }
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[ImageTabControl.SelectedIndex]);
        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[ImageTabControl.SelectedIndex]);
        }
        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[ImageTabControl.SelectedIndex]);
        }
        private void checkBox4_CheckedChanged(object sender, EventArgs e)
        {
            UpdateHistogram(PicturesList[ImageTabControl.SelectedIndex]);
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Converting between Spatial & Frequency Domains
        private void convertToSpatialDomainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                if (PicturesList[picIndex].frequency)
                {
                    ImageClass Image = new ImageClass();
                    Image.ConverttoSpatialDomain(PicturesList[picIndex]);
                    PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Convert to Spatial Domain");
                    DisplayImage(PicturesList[picIndex]);
                }
                else
                    MessageBox.Show("Image must be in frequency domain..");
            }
        }
        private void convertToFrequencyDomainToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                if (!PicturesList[picIndex].frequency)
                {
                    ImageClass Image = new ImageClass();
                    Image.convertToFrequencyDomain(PicturesList[picIndex]);
                    PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Convert to Frequency Domain");
                    DisplayImage(PicturesList[picIndex]);
                }
                else
                    MessageBox.Show("Image must be in spatial domain..");
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Filters

        //Mean Filter
        private void meanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyMeanFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int picIndex = ImageTabControl.SelectedIndex;
            Filter filter = new Filter();
            //DateTime BeginTime = DateTime.Now;
            int length = int.Parse(sizeText.Text);
            if (length % 2 == 0) length++;
            double[] Mask = new double[length];
            for (int i = 0; i < length; i++)
                Mask[i] = 1.0 / length;
            filter.Apply1DFilter("Mean", length, Mask, Mask, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels);
            DisplayImage(PicturesList[picIndex]);
            //DateTime EndTime = DateTime.Now;
            //TimeSpan TimeTaken = EndTime - BeginTime;
            PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Mean Filter");
            //TimeForm Time = new TimeForm("Mean Filter", TimeTaken.ToString());
            //Time.Show();
        }

        //Gaussian Filter
        private void gaussianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
                sizeLabel.Text = "Sigma";
                inputGroupBox.Controls.Add(sizeLabel);
                //Button
                Button applyBtn = new Button();
                applyBtn.Text = "Apply";
                applyBtn.Location = new System.Drawing.Point(102, 85);
                applyBtn.Size = new System.Drawing.Size(75, 23);
                applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplyGaussianFilter_Click(sender1, e1, sizeTxt); };
                inputGroupBox.Controls.Add(applyBtn);
            }
        }
        private void ApplyGaussianFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            int count = ImageTabControl.SelectedIndex;
            int picIndex = ImageTabControl.SelectedIndex;
            Filter filter = new Filter();
            //DateTime BeginTime = DateTime.Now;
            double sigma = double.Parse(sizeText.Text);
            int length = (int)(3.7 * sigma - 0.5);
            length = length * 2 + 1;
            double[] Mask = new double[length];
            double x = -length / 2;
            double sum = 0;
            for (int i = 0; i < length; i++, x++)
            {
                Mask[i] = (1 / (Math.Sqrt(2 * Math.PI) * sigma) * Math.Exp(-(x * x) / (2 * sigma * sigma)));
                sum += Mask[i];
            }
            filter.Apply1DFilter("Gaussian", length, Mask, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
            DisplayImage(PicturesList[picIndex]);
            //DateTime EndTime = DateTime.Now;
            //TimeSpan TimeTaken = EndTime - BeginTime;
            PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Gaussian Filter");
            //TimeForm Time = new TimeForm("Gaussian Filter", TimeTaken.ToString());
            //Time.Show();
        }
        //Sharpening Filters
        private void laplacianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                int picIndex = ImageTabControl.SelectedIndex;
                DateTime BeginTime = DateTime.Now;
                double[,] Mask = new double[3, 3] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } };
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                DisplayImage(PicturesList[picIndex]);
                DateTime EndTime = DateTime.Now;
                TimeSpan TimeTaken = EndTime - BeginTime;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Laplacian Sharpening Filter");
                TimeForm Time = new TimeForm("Laplacian Sharpening Filter", TimeTaken.ToString());
                Time.Show();
            }
        }
        private void horizontalFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { 0, 1, 0 }, { 0, 1, 0 }, { 0, -1, 0 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex; PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Horizontal Sharpening Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void verticalFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { 0, 0, 0 }, { 1, 1, -1 }, { 0, 0, 0 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Vertical Sharpening Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void rightDiagonalFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { 1, 0, 0 }, { 0, 1, 0 }, { 0, 0, -1 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Right Diagonal Sharpening Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void leftDiagonalFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { 0, 0, 1 }, { 0, 1, 0 }, { -1, 0, 0 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Left Diagonal Sharpening Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        //Sobel Filter
        private void verticalFilterToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            double[] Mask1 = new double[3] { -1, 0, 1 };
            double[] Mask2 = new double[3] { 1, 2, 1 };
            ApplySobelFilter(Mask1, Mask2, 0);
        }
        private void horizontalFilterToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[] Mask1 = new double[3] { -1, 0, 1 };
                double[] Mask2 = new double[3] { 1, 2, 1 };
                ApplySobelFilter(Mask2, Mask1, 1);
            }
        }
        private void ApplySobelFilter(double[] Mask1, double[] Mask2, int type)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int count = ImageTabControl.SelectedIndex;
                int picIndex = ImageTabControl.SelectedIndex;
                Filter filter = new Filter();
                int length = 3;
                filter.Apply1DFilter("Sobel", length, Mask1, Mask2, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                DisplayImage(PicturesList[picIndex]);
                if (type == 0)
                    PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Vertical Sobel Filter");
                else
                    PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Horizontal Sobel Filter");
            }
        }

        //Edge Detection Filters
        private void laplacianFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { -1, -1, -1 }, { -1, 8, -1 }, { -1, -1, -1 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Laplacian Edge Detection Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void horizontalFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { 1, 1, 1 }, { 1, -2, 1 }, { -1, -1, -1 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Horizontal Edge Detection Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void verticalFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { -1, 1, 1 }, { -1, -2, 1 }, { -1, 1, 1 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Vetical Edge Detection Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void rightDiagonalFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { 1, 1, 1 }, { 1, -2, -1 }, { 1, -1, -1 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Right Diagonal Edge Detection Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void leftDiagonalFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double[,] Mask = new double[3, 3] { { 1, 1, 1 }, { -1, -2, 1 }, { -1, -1, 1 } };
                Filter filter = new Filter();
                int count = ImageTabControl.SelectedIndex;
                filter.Apply2DFilter(3, 3, Mask, PicturesList[count], ref PicturesList[count].redPixels, ref PicturesList[count].greenPixels, ref PicturesList[count].bluePixels);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Left Diagonal Edge Detection Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        //Convolution
        private void customFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                CustomFilter CF = new CustomFilter(PicturesList, PicUndoRedo);
                CF.Show();
            }
        }

        //High-Pass & Low-Pass Filters Menu Item
        private void idealToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void butterworthFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void gaussianFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void idealFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void butterworthFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void gaussianFilterToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }

        //High-Pass & Low-Pass Filters
        private void lowFilterBtn_Click(object sender, EventArgs e, TextBox dTxt, int filterType)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Filter filter = new Filter();
                filter.LowPassFilters(PicturesList[picIndex], filterType, double.Parse(dTxt.Text), 0);
                if (filterType == 0) PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Ideal Low Pass Filter");
                else PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Gauusian Low Pass Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void lowButterFilterBtn_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt, int filterType)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Filter filter = new Filter();
                filter.LowPassFilters(PicturesList[picIndex], filterType, double.Parse(dTxt.Text), double.Parse(nTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Butterworth Low Pass Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void HighFilterBtn_Click(object sender, EventArgs e, TextBox dTxt, int filterType)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Filter filter = new Filter();
                filter.HighPassFilters(PicturesList[picIndex], filterType, double.Parse(dTxt.Text), 0);
                if (filterType == 0) PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Ideal High Pass Filter");
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Gaussian High Pass Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void HighButterFilterBtn_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt, int filterType)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Filter filter = new Filter();
                filter.HighPassFilters(PicturesList[picIndex], filterType, double.Parse(dTxt.Text), double.Parse(nTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Butterworth High Pass Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        //Noise Removing Filters
        private void geometricMeanFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyGMeanFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = int.Parse(sizeText.Text);
                if (length % 2 == 0) length++;
                Filter filter = new Filter();
                int picIndex = ImageTabControl.SelectedIndex;
                DateTime BeginTime = DateTime.Now;
                filter.Apply2DGMeanFilter(length, length, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels);
                DisplayImage(PicturesList[picIndex]);
                DateTime EndTime = DateTime.Now;
                TimeSpan TimeTaken = EndTime - BeginTime;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Geometric Mean Filter");
                TimeForm Time = new TimeForm("Geometric Mean Filter", TimeTaken.ToString());
                Time.Show();
            }
        }

        private void weightedFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = 3;
                Filter filter = new Filter();
                int picIndex = ImageTabControl.SelectedIndex;
                DateTime BeginTime = DateTime.Now;

                double x = 1.0 / 4.0, y = 1.0 / 8.0, z = 1.0 / 16.0;
                double[,] Mask = new double[3, 3] { { z, y, z }, { y, x, y }, { z, y, z } };
                filter.Apply2DFilter(length, length, Mask, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels);
                DisplayImage(PicturesList[picIndex]);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Weighted Mean Filter");
            }
        }

        private void medianFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyMedianFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = int.Parse(sizeText.Text);
                if (length % 2 == 0) length++;
                Filter filter = new Filter();
                int picIndex = ImageTabControl.SelectedIndex;
                DateTime BeginTime = DateTime.Now;
                filter.ApplyOrderStatFilter(length, length, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels, "Median");
                DisplayImage(PicturesList[picIndex]);
                DateTime EndTime = DateTime.Now;
                TimeSpan TimeTaken = EndTime - BeginTime;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Median Filter");
                TimeForm Time = new TimeForm("Median Filter", TimeTaken.ToString());
                Time.Show();
            }
        }

        private void minimumFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyMinimumFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = int.Parse(sizeText.Text);
                if (length % 2 == 0) length++;
                Filter filter = new Filter();
                int picIndex = ImageTabControl.SelectedIndex;
                DateTime BeginTime = DateTime.Now;
                filter.ApplyOrderStatFilter(length, length, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels, "Minimum");
                DisplayImage(PicturesList[picIndex]);
                DateTime EndTime = DateTime.Now;
                TimeSpan TimeTaken = EndTime - BeginTime;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Minimum Filter");
                TimeForm Time = new TimeForm("Minimum Filter", TimeTaken.ToString());
                Time.Show();
            }
        }

        private void maximumFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyMaximumFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = int.Parse(sizeText.Text);
                if (length % 2 == 0) length++;
                Filter filter = new Filter();
                filter.ApplyOrderStatFilter(length, length, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels, "Maximum");
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Maximum Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void midPointFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyMidPointFilter_Click(object sender, EventArgs e, TextBox sizeText)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = int.Parse(sizeText.Text);
                if (length % 2 == 0) length++;
                Filter filter = new Filter();
                filter.ApplyOrderStatFilter(length, length, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels, "MidPoint");
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Mid-Point Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void contraharmonicMeanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyContraharmonicFilter_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = int.Parse(dTxt.Text);
                if (length % 2 == 0) length++;
                double Q = double.Parse(nTxt.Text);
                Filter filter = new Filter();
                filter.Apply2DContraharmonicFilter(length, length, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels, Q);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Contraharmonic Mean Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void alphaTrimmedMeanFilterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Alpha-Trimmed Mean Filter";
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
        }
        private void ApplyAlphaTrimmedFilter_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int length = int.Parse(dTxt.Text);
                if (length % 2 == 0) length++;
                double D = double.Parse(nTxt.Text);
                Filter filter = new Filter();
                filter.ApplyAlphaTrimmedFilter(length, length, PicturesList[ImageTabControl.SelectedIndex], ref PicturesList[ImageTabControl.SelectedIndex].redPixels, ref PicturesList[ImageTabControl.SelectedIndex].greenPixels, ref PicturesList[ImageTabControl.SelectedIndex].bluePixels, D);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Alpha-Trimmed Mean Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }


        private void idealToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyIdealBandRejectFilter_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double D = double.Parse(dTxt.Text);
                double w = double.Parse(nTxt.Text);
                Filter filter = new Filter();
                filter.BandFilters(PicturesList[ImageTabControl.SelectedIndex], 0, D, w);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Ideal Band Reject Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void idealToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Ideal Band Pass Filter";
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
        }
        private void ApplyIdealBandPassFilter_Click(object sender, EventArgs e, TextBox dTxt, TextBox nTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double D = double.Parse(dTxt.Text);
                double w = double.Parse(nTxt.Text);
                Filter filter = new Filter();
                filter.BandFilters(PicturesList[ImageTabControl.SelectedIndex], 1, D, w);
                int picIndex = ImageTabControl.SelectedIndex;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Ideal Band Pass Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void idealToolStripMenuItem4_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyIdealNotchRejectBtn_Click(object sender, EventArgs e, TextBox muTxt, TextBox sigmaTxt, TextBox NoisePercentageTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Filter filter = new Filter();
                filter.NotchFilters(PicturesList[picIndex], 0, double.Parse(muTxt.Text), double.Parse(sigmaTxt.Text), double.Parse(NoisePercentageTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Ideal Notch Reject Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void idealToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyIdealNotchPassBtn_Click(object sender, EventArgs e, TextBox muTxt, TextBox sigmaTxt, TextBox NoisePercentageTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Filter filter = new Filter();
                filter.NotchFilters(PicturesList[picIndex], 1, double.Parse(muTxt.Text), double.Parse(sigmaTxt.Text), double.Parse(NoisePercentageTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Ideal Notch Pass Filter");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void medianFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void meanFilterToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void AdaptiveFilterBtn_Click(object sender, EventArgs e, TextBox MaxWinSize, int type)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Filter filter = new Filter();
                DateTime BeginTime = DateTime.Now;
                filter.AdaptiveFilter(PicturesList[picIndex], int.Parse(MaxWinSize.Text), type);
                DisplayImage(PicturesList[picIndex]);
                DateTime EndTime = DateTime.Now;
                TimeSpan TimeTaken = EndTime - BeginTime;
                if (type == 0)
                {
                    PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Adaptive Median Filter");
                    TimeForm Time = new TimeForm("Adaptive Median Filter", TimeTaken.ToString());
                    Time.Show();
                }
                else
                {
                    PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Adaptive Mean Filter");
                }
            }
        }

        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Adding Noise

        private void saltPepperToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplySaltPepperBtn_Click(object sender, EventArgs e, TextBox saltTxt, TextBox pepperTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                DateTime BeginTime = DateTime.Now;
                Image.AddSaltPepperNoise(PicturesList[picIndex], double.Parse(saltTxt.Text), double.Parse(pepperTxt.Text));
                DisplayImage(PicturesList[picIndex]);
                DateTime EndTime = DateTime.Now;
                TimeSpan TimeTaken = EndTime - BeginTime;
                TimeForm Time = new TimeForm("Add Salt and Pepper Noise", TimeTaken.ToString());
                Time.Show();
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Salt and Pepper Noise");
            }
        }

        private void unifromNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyUnifromNoiseBtn_Click(object sender, EventArgs e, TextBox aTxt, TextBox bTxt, TextBox NoisePercentageTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.AddAdditiveNoise(PicturesList[picIndex], "Uniform", int.Parse(aTxt.Text), int.Parse(bTxt.Text), double.Parse(NoisePercentageTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Uniform Noise");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void gaussianNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ApplyGaussianNoiseBtn_Click(object sender, EventArgs e, TextBox muTxt, TextBox sigmaTxt, TextBox NoisePercentageTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.AddAdditiveNoise(PicturesList[picIndex], "Gaussian", int.Parse(muTxt.Text), int.Parse(sigmaTxt.Text), double.Parse(NoisePercentageTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Gaussian Noise");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void rayleighNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void RayleighBtn_Click(object sender, EventArgs e, TextBox aTxt, TextBox bTxt, TextBox NoisePercentageTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.AddAdditiveNoise(PicturesList[picIndex], "Rayleigh", int.Parse(aTxt.Text), int.Parse(bTxt.Text), double.Parse(NoisePercentageTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Rayleigh Noise");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void gammaNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void GammaNoiseBtn_Click(object sender, EventArgs e, TextBox aTxt, TextBox bTxt, TextBox NoisePercentageTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.AddAdditiveNoise(PicturesList[picIndex], "Gamma", int.Parse(aTxt.Text), int.Parse(bTxt.Text), double.Parse(NoisePercentageTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Gamma Noise");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void exponentialNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
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
        }
        private void ExponentialNoiseBtn_Click(object sender, EventArgs e, TextBox aTxt, TextBox NoisePercentageTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.AddAdditiveNoise(PicturesList[picIndex], "Exponential", double.Parse(aTxt.Text), 0, double.Parse(NoisePercentageTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Exponential Noise");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        private void periodicNoiseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int height = PicturesList[ImageTabControl.SelectedIndex].height;
                int width = PicturesList[ImageTabControl.SelectedIndex].width;
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
        }
        private void AddPeriodicNoiseBtn_Click(object sender, EventArgs e, double Amp, double xFreq, double yFreq, double xPhase, double yPhase)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.AddPeriodicNoise(PicturesList[picIndex], Amp, xFreq, yFreq, xPhase, yPhase);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Periodic Noise");
                DisplayImage(PicturesList[picIndex]);
            }
        }

        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Segmentation
        //Otsu Thresholding
        private void otsuThresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                DateTime BeginTime = DateTime.Now;
                Image.OtsuSegmentation(PicturesList[picIndex]);
                DisplayImage(PicturesList[picIndex]);
                DateTime EndTime = DateTime.Now;
                TimeSpan TimeTaken = EndTime - BeginTime;
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Otsu Thresholding");
                TimeForm Time = new TimeForm("Otsu Thresholding", TimeTaken.ToString());
                Time.Show();
            }
        }
        private void basicGlobalThresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int PicIndex = ImageTabControl.SelectedIndex;
                Thresholding thresholding = new Thresholding(PicturesList[PicIndex], PicUndoRedo[PicIndex]);
                thresholding.Show();
            }
        }
        private void adaptiveThresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Adaptive Thresholding";
                //Epsilon Text Box
                TextBox WinSizeTxt = new TextBox();
                WinSizeTxt.Location = new System.Drawing.Point(15, 42);
                WinSizeTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(WinSizeTxt);
                //Window Size Label
                System.Windows.Forms.Label WinSizeLabel = new System.Windows.Forms.Label();
                WinSizeLabel.Location = new System.Drawing.Point(12, 26);
                WinSizeLabel.Text = "Window Size";
                inputGroupBox.Controls.Add(WinSizeLabel);
                //MeanOffset Text Box
                TextBox MeanOffsetTxt = new TextBox();
                MeanOffsetTxt.Location = new System.Drawing.Point(158, 42);
                MeanOffsetTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(MeanOffsetTxt);
                //MeanOffset Label
                System.Windows.Forms.Label MeanOffsetLabel = new System.Windows.Forms.Label();
                MeanOffsetLabel.Location = new System.Drawing.Point(155, 26);
                MeanOffsetLabel.Text = "Mean Offset";
                inputGroupBox.Controls.Add(MeanOffsetLabel);
                //Button
                Button applyBtn = new Button();
                applyBtn.Text = "Apply";
                applyBtn.Location = new System.Drawing.Point(102, 85);
                applyBtn.Size = new System.Drawing.Size(75, 23);
                applyBtn.Click += delegate(object sender1, EventArgs e1) { AdaptiveThresholdButton_Click(sender1, e1, WinSizeTxt, MeanOffsetTxt); };
                inputGroupBox.Controls.Add(applyBtn);
            }
        }
        private void AdaptiveThresholdButton_Click(object sender, EventArgs e, TextBox WinSize, TextBox MeanOffset)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.AdaptiveThresholding(PicturesList[picIndex], int.Parse(WinSize.Text), int.Parse(MeanOffset.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Adaptive Thresholding");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Morphology
        private void morphologyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                Morphology M = new Morphology(PicturesList[picIndex], PicUndoRedo[picIndex]);
                M.Show();
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

        #region Matlab Tasks
        private void localStatisticsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int height = PicturesList[ImageTabControl.SelectedIndex].height;
                int width = PicturesList[ImageTabControl.SelectedIndex].width;
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Local Statistics";
                //K0 Label
                System.Windows.Forms.Label K0Label = new System.Windows.Forms.Label();
                K0Label.Location = new System.Drawing.Point(27, 26);
                K0Label.Size = new System.Drawing.Size(50, 20);
                K0Label.Text = "K0";
                inputGroupBox.Controls.Add(K0Label);
                //K0 NumericUpDown
                TextBox K0NumericUpDown = new TextBox();
                K0NumericUpDown.Location = new System.Drawing.Point(110, 24);
                K0NumericUpDown.Size = new System.Drawing.Size(100, 20);
                K0NumericUpDown.Text = "0.4";
                inputGroupBox.Controls.Add(K0NumericUpDown);

                //K1 Label
                System.Windows.Forms.Label K1Label = new System.Windows.Forms.Label();
                K1Label.Location = new System.Drawing.Point(27, 56);
                K1Label.Size = new System.Drawing.Size(50, 20);
                K1Label.Text = "K1";
                inputGroupBox.Controls.Add(K1Label);
                //K1 NumericUpDown
                TextBox K1NumericUpDown = new TextBox();
                K1NumericUpDown.Location = new System.Drawing.Point(110, 54);
                K1NumericUpDown.Size = new System.Drawing.Size(100, 20);
                K1NumericUpDown.Text = "0.02";
                inputGroupBox.Controls.Add(K1NumericUpDown);

                //K2 Label
                System.Windows.Forms.Label K2Label = new System.Windows.Forms.Label();
                K2Label.Location = new System.Drawing.Point(27, 86);
                K2Label.Size = new System.Drawing.Size(50, 20);
                K2Label.Text = "K2";
                inputGroupBox.Controls.Add(K2Label);
                //K2 NumericUpDown
                TextBox K2NumericUpDown = new TextBox();
                K2NumericUpDown.Location = new System.Drawing.Point(110, 84);
                K2NumericUpDown.Size = new System.Drawing.Size(100, 20);
                K2NumericUpDown.Text = "0.4";
                inputGroupBox.Controls.Add(K2NumericUpDown);

                //E Label
                System.Windows.Forms.Label ELabel = new System.Windows.Forms.Label();
                ELabel.Location = new System.Drawing.Point(27, 116);
                ELabel.Size = new System.Drawing.Size(50, 20);
                ELabel.Text = "E";
                inputGroupBox.Controls.Add(ELabel);
                //E NumericUpDown
                TextBox ENumericUpDown = new TextBox();
                ENumericUpDown.Location = new System.Drawing.Point(110, 114);
                ENumericUpDown.Size = new System.Drawing.Size(100, 20);
                ENumericUpDown.Text = "4";
                inputGroupBox.Controls.Add(ENumericUpDown);

                //WinSize Label
                System.Windows.Forms.Label WinSizeLabel = new System.Windows.Forms.Label();
                WinSizeLabel.Location = new System.Drawing.Point(27, 146);
                WinSizeLabel.Size = new System.Drawing.Size(80, 20);
                WinSizeLabel.Text = "Window Size";
                inputGroupBox.Controls.Add(WinSizeLabel);
                //WinSize NumericUpDown
                TextBox WinSizeNumericUpDown = new TextBox();
                WinSizeNumericUpDown.Location = new System.Drawing.Point(110, 144);
                WinSizeNumericUpDown.Text = "3";
                inputGroupBox.Controls.Add(WinSizeNumericUpDown);

                //Button
                Button applyBtn = new Button();
                applyBtn.Text = "Apply";
                applyBtn.Location = new System.Drawing.Point(102, 180);
                applyBtn.Size = new System.Drawing.Size(75, 23);
                applyBtn.Click += delegate(object sender1, EventArgs e1) { LocalStatBtn_Click(sender1, e1, double.Parse(K0NumericUpDown.Text), double.Parse(K1NumericUpDown.Text), double.Parse(K2NumericUpDown.Text), double.Parse(ENumericUpDown.Text), int.Parse(WinSizeNumericUpDown.Text)); };
                inputGroupBox.Controls.Add(applyBtn);
            }
        }
        private void LocalStatBtn_Click(object sender, EventArgs e, double K0, double K1, double K2, double E, int WinSize)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.LocalStat(PicturesList[picIndex], K0, K1, K2, E, WinSize);
                DisplayImage(PicturesList[picIndex]);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Local Statistics");
            }
        }
        private void histogramSlicingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Histogram Slicing";
                //Min Text Box
                TextBox MinTxt = new TextBox();
                MinTxt.Location = new System.Drawing.Point(15, 42);
                MinTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(MinTxt);
                //Min Label
                System.Windows.Forms.Label MinLabel = new System.Windows.Forms.Label();
                MinLabel.Location = new System.Drawing.Point(12, 26);
                MinLabel.Text = "Minimum Color";
                inputGroupBox.Controls.Add(MinLabel);
                //Max Text Box
                TextBox MaxTxt = new TextBox();
                MaxTxt.Location = new System.Drawing.Point(158, 42);
                MaxTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(MaxTxt);
                //Max Label
                System.Windows.Forms.Label MaxLabel = new System.Windows.Forms.Label();
                MaxLabel.Location = new System.Drawing.Point(155, 26);
                MaxLabel.Text = "Maximum Color";
                inputGroupBox.Controls.Add(MaxLabel);

                //NewRange Text Box
                TextBox NewRangeTxt = new TextBox();
                NewRangeTxt.Location = new System.Drawing.Point(90, 91);
                NewRangeTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(NewRangeTxt);
                //NewRange Label
                System.Windows.Forms.Label NewRangeLabel = new System.Windows.Forms.Label();
                NewRangeLabel.Location = new System.Drawing.Point(87, 75);
                NewRangeLabel.Text = "New Range";
                inputGroupBox.Controls.Add(NewRangeLabel);

                //Button
                Button applyBtn = new Button();
                applyBtn.Text = "Apply";
                applyBtn.Location = new System.Drawing.Point(102, 130);
                applyBtn.Size = new System.Drawing.Size(75, 23);
                applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplySlicingBtn_Click(sender1, e1, MinTxt, MaxTxt, NewRangeTxt); };
                inputGroupBox.Controls.Add(applyBtn);
            }
        }
        private void ApplySlicingBtn_Click(object sender, EventArgs e, TextBox MinTxt, TextBox MaxTxt, TextBox NewRangeTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                int picIndex = ImageTabControl.SelectedIndex;
                ImageClass Image = new ImageClass();
                Image.Slicing(PicturesList[picIndex], int.Parse(MinTxt.Text), int.Parse(MaxTxt.Text), int.Parse(NewRangeTxt.Text));
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Histogram Slicing");
                DisplayImage(PicturesList[picIndex]);
            }
        }
        private void singleScaleRetinexToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                inputGroupBox.Controls.Clear();
                inputGroupBox.Text = "Single Scale Retinex";
                //Sigma Text Box
                TextBox SigmaTxt = new TextBox();
                SigmaTxt.Location = new System.Drawing.Point(15, 42);
                SigmaTxt.Size = new System.Drawing.Size(100, 20);
                inputGroupBox.Controls.Add(SigmaTxt);
                //Sigma Label
                System.Windows.Forms.Label SigmaLabel = new System.Windows.Forms.Label();
                SigmaLabel.Location = new System.Drawing.Point(12, 26);
                SigmaLabel.Text = "Sigma";
                inputGroupBox.Controls.Add(SigmaLabel);
                //Button
                Button applyBtn = new Button();
                applyBtn.Text = "Apply";
                applyBtn.Location = new System.Drawing.Point(102, 85);
                applyBtn.Size = new System.Drawing.Size(75, 23);
                applyBtn.Click += delegate(object sender1, EventArgs e1) { ApplySSR_Click(sender1, e1, SigmaTxt); };
                inputGroupBox.Controls.Add(applyBtn);
            }
        }
        private void ApplySSR_Click(object sender, EventArgs e, TextBox SigmaTxt)
        {
            if (ImageTabControl.TabPages.Count > 0)
            {
                double Sigma = double.Parse(SigmaTxt.Text);
                ImageClass Image = new ImageClass();
                int picIndex = ImageTabControl.SelectedIndex;
                Image.SSR(PicturesList[picIndex], Sigma);
                DisplayImage(PicturesList[picIndex]);
                PicUndoRedo[picIndex].UndoRedoCommands(PicturesList[picIndex], "Single Scale Retinex");
            }
        }
        #endregion

        //=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*=*

    }
}