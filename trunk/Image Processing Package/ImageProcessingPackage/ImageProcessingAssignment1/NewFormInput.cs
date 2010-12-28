using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace ImageProcessingAssignment1
{
    public partial class NewFormInput : Form
    {
        private TabControl tabControl;
        private List<PictureInfo> picList;
        public NewFormInput(List<PictureInfo> _picList, TabControl _tabControl)
        {
            picList = _picList;
            tabControl = _tabControl;
            InitializeComponent();
        }
        private void GoBtn_Click(object sender, EventArgs e)
        {
            try
            {
                int width = 0, height = 0;
                int.TryParse(Widthtext.Text, out width);
                int.TryParse(HeightText.Text, out height);
                PictureBox pic = new PictureBox();
                pic.Location = new System.Drawing.Point(100, 100);
                pic.Size = new System.Drawing.Size(width, height);
                picList.Add(new PictureInfo(width, height, "Untitled", "C:\\Untitled", pic, new byte[height, width], new byte[height, width], new byte[height, width]));
                int index = picList.Count - 1;

                if (!tabControl.Visible) tabControl.Visible = true;
                TabPage tabPage = new TabPage();

                tabControl.TabPages.Add(tabPage);
                tabPage.Controls.Add(picList[index].pictureBox);

                tabControl.TabPages[index].Text = picList[index].name;
                tabControl.SelectedIndex = index;
                tabPage.AutoScroll = true;
                DisplayImage(picList[index]);
                this.Close();
            }
            catch { }
        }
        private void DisplayImage(PictureInfo pictureInfo)
        {
            int width = pictureInfo.width;
            int height = pictureInfo.height;
            Bitmap bmp = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            pictureInfo.pictureBox.Size = new System.Drawing.Size(width, height);
            pictureInfo.pictureBox.Image = bmp;
        }
    }
}