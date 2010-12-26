namespace ImageProcessingAssignment1
{
    partial class Thresholding
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Thresholding));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.okbtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.EpsilonTrackBar = new System.Windows.Forms.TrackBar();
            this.EpsilonUpDown = new System.Windows.Forms.NumericUpDown();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EpsilonTrackBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.EpsilonUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            this.groupBox1.UseWaitCursor = true;
            // 
            // pictureBox1
            // 
            resources.ApplyResources(this.pictureBox1, "pictureBox1");
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.TabStop = false;
            this.pictureBox1.UseWaitCursor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            this.groupBox2.UseWaitCursor = true;
            // 
            // pictureBox2
            // 
            resources.ApplyResources(this.pictureBox2, "pictureBox2");
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.TabStop = false;
            this.pictureBox2.UseWaitCursor = true;
            // 
            // okbtn
            // 
            resources.ApplyResources(this.okbtn, "okbtn");
            this.okbtn.Name = "okbtn";
            this.okbtn.UseVisualStyleBackColor = true;
            this.okbtn.Click += new System.EventHandler(this.okbtn_Click);
            // 
            // cancelBtn
            // 
            resources.ApplyResources(this.cancelBtn, "cancelBtn");
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.UseVisualStyleBackColor = true;
            this.cancelBtn.Click += new System.EventHandler(this.cancelBtn_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.EpsilonTrackBar);
            this.groupBox3.Controls.Add(this.EpsilonUpDown);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            resources.ApplyResources(this.groupBox3, "groupBox3");
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.TabStop = false;
            // 
            // EpsilonTrackBar
            // 
            resources.ApplyResources(this.EpsilonTrackBar, "EpsilonTrackBar");
            this.EpsilonTrackBar.Maximum = 255;
            this.EpsilonTrackBar.Name = "EpsilonTrackBar";
            this.EpsilonTrackBar.Value = 1;
            this.EpsilonTrackBar.Scroll += new System.EventHandler(this.EpsilonTrackBar_Scroll);
            // 
            // EpsilonUpDown
            // 
            resources.ApplyResources(this.EpsilonUpDown, "EpsilonUpDown");
            this.EpsilonUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.EpsilonUpDown.Name = "EpsilonUpDown";
            this.EpsilonUpDown.ValueChanged += new System.EventHandler(this.EpsilonUpDown_ValueChanged);
            // 
            // Thresholding
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okbtn);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "Thresholding";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Thresholding_FormClosed);
            this.Load += new System.EventHandler(this.Thresholding_Load);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.EpsilonTrackBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.EpsilonUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Button okbtn;
        private System.Windows.Forms.Button cancelBtn;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TrackBar EpsilonTrackBar;
        private System.Windows.Forms.NumericUpDown EpsilonUpDown;
    }
}