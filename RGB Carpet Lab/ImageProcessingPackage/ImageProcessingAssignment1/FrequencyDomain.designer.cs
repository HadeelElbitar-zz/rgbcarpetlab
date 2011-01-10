namespace ImageProcessingAssignment1
{
    partial class FrequencyDomain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrequencyDomain));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.SpatialButton = new System.Windows.Forms.Button();
            this.FreqButton = new System.Windows.Forms.Button();
            this.ColorGroupBox = new System.Windows.Forms.GroupBox();
            this.BlueBTN = new System.Windows.Forms.RadioButton();
            this.GreenBTN = new System.Windows.Forms.RadioButton();
            this.RedBTN = new System.Windows.Forms.RadioButton();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.ColorGroupBox.SuspendLayout();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(290, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 200);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Frequency Domain Picture";
            this.groupBox2.UseWaitCursor = true;
            // 
            // pictureBox2
            // 
            this.pictureBox2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox2.Location = new System.Drawing.Point(6, 17);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(238, 175);
            this.pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox2.TabIndex = 0;
            this.pictureBox2.TabStop = false;
            this.pictureBox2.UseWaitCursor = true;
            // 
            // pictureBox1
            // 
            this.pictureBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox1.Location = new System.Drawing.Point(6, 17);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(238, 175);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            this.pictureBox1.UseWaitCursor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.pictureBox1);
            this.groupBox1.ForeColor = System.Drawing.Color.White;
            this.groupBox1.Location = new System.Drawing.Point(23, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 200);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Original Picture";
            this.groupBox1.UseWaitCursor = true;
            // 
            // SpatialButton
            // 
            this.SpatialButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SpatialButton.Location = new System.Drawing.Point(685, 275);
            this.SpatialButton.Name = "SpatialButton";
            this.SpatialButton.Size = new System.Drawing.Size(117, 23);
            this.SpatialButton.TabIndex = 12;
            this.SpatialButton.Text = "Spatial Domain";
            this.SpatialButton.UseVisualStyleBackColor = true;
            this.SpatialButton.Visible = false;
            this.SpatialButton.Click += new System.EventHandler(this.SpatialButton_Click);
            // 
            // FreqButton
            // 
            this.FreqButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.FreqButton.Location = new System.Drawing.Point(685, 244);
            this.FreqButton.Name = "FreqButton";
            this.FreqButton.Size = new System.Drawing.Size(117, 23);
            this.FreqButton.TabIndex = 11;
            this.FreqButton.Text = "Frequency Domain";
            this.FreqButton.UseVisualStyleBackColor = true;
            this.FreqButton.Click += new System.EventHandler(this.FreqButton_Click);
            // 
            // ColorGroupBox
            // 
            this.ColorGroupBox.Controls.Add(this.BlueBTN);
            this.ColorGroupBox.Controls.Add(this.GreenBTN);
            this.ColorGroupBox.Controls.Add(this.RedBTN);
            this.ColorGroupBox.ForeColor = System.Drawing.Color.White;
            this.ColorGroupBox.Location = new System.Drawing.Point(290, 244);
            this.ColorGroupBox.Name = "ColorGroupBox";
            this.ColorGroupBox.Size = new System.Drawing.Size(250, 44);
            this.ColorGroupBox.TabIndex = 14;
            this.ColorGroupBox.TabStop = false;
            this.ColorGroupBox.Text = "Color";
            this.ColorGroupBox.Visible = false;
            // 
            // BlueBTN
            // 
            this.BlueBTN.AutoSize = true;
            this.BlueBTN.Location = new System.Drawing.Point(190, 17);
            this.BlueBTN.Name = "BlueBTN";
            this.BlueBTN.Size = new System.Drawing.Size(48, 17);
            this.BlueBTN.TabIndex = 17;
            this.BlueBTN.TabStop = true;
            this.BlueBTN.Text = "BLue";
            this.BlueBTN.UseVisualStyleBackColor = true;
            this.BlueBTN.CheckedChanged += new System.EventHandler(this.BlueBTN_CheckedChanged);
            // 
            // GreenBTN
            // 
            this.GreenBTN.AutoSize = true;
            this.GreenBTN.Location = new System.Drawing.Point(92, 17);
            this.GreenBTN.Name = "GreenBTN";
            this.GreenBTN.Size = new System.Drawing.Size(54, 17);
            this.GreenBTN.TabIndex = 5;
            this.GreenBTN.Text = "Green";
            this.GreenBTN.UseVisualStyleBackColor = true;
            this.GreenBTN.CheckedChanged += new System.EventHandler(this.GreenBTN_CheckedChanged);
            // 
            // RedBTN
            // 
            this.RedBTN.AutoSize = true;
            this.RedBTN.Checked = true;
            this.RedBTN.Location = new System.Drawing.Point(16, 18);
            this.RedBTN.Name = "RedBTN";
            this.RedBTN.Size = new System.Drawing.Size(44, 17);
            this.RedBTN.TabIndex = 4;
            this.RedBTN.TabStop = true;
            this.RedBTN.Text = "Red";
            this.RedBTN.UseVisualStyleBackColor = true;
            this.RedBTN.CheckedChanged += new System.EventHandler(this.RedBTN_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.pictureBox3);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(552, 29);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 200);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Spatial Domain Picture";
            this.groupBox3.UseWaitCursor = true;
            // 
            // pictureBox3
            // 
            this.pictureBox3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.pictureBox3.Location = new System.Drawing.Point(6, 17);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(238, 175);
            this.pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox3.TabIndex = 0;
            this.pictureBox3.TabStop = false;
            this.pictureBox3.UseWaitCursor = true;
            // 
            // FrequencyDomain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(830, 325);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.ColorGroupBox);
            this.Controls.Add(this.SpatialButton);
            this.Controls.Add(this.FreqButton);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "FrequencyDomain";
            this.Text = "StructureElement";
            this.Load += new System.EventHandler(this.FrequencyDomain_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.ColorGroupBox.ResumeLayout(false);
            this.ColorGroupBox.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button SpatialButton;
        private System.Windows.Forms.Button FreqButton;
        private System.Windows.Forms.RadioButton GreenBTN;
        private System.Windows.Forms.RadioButton RedBTN;
        private System.Windows.Forms.GroupBox ColorGroupBox;
        private System.Windows.Forms.RadioButton BlueBTN;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.PictureBox pictureBox3;
    }
}