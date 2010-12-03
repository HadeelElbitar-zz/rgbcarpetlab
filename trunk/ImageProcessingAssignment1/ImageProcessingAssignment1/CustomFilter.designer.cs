namespace ImageProcessingAssignment1
{
    partial class CustomFilter
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
            this.OriginalBox = new System.Windows.Forms.GroupBox();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.PicBox1 = new System.Windows.Forms.PictureBox();
            this.ModifiedBox = new System.Windows.Forms.GroupBox();
            this.PicBox2 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.ApplyBTN = new System.Windows.Forms.Button();
            this.FilterGrid = new System.Windows.Forms.DataGridView();
            this.OriginalBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox1)).BeginInit();
            this.ModifiedBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.PicBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FilterGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // OriginalBox
            // 
            this.OriginalBox.Controls.Add(this.comboBox1);
            this.OriginalBox.Controls.Add(this.PicBox1);
            this.OriginalBox.Location = new System.Drawing.Point(12, 12);
            this.OriginalBox.Name = "OriginalBox";
            this.OriginalBox.Size = new System.Drawing.Size(309, 262);
            this.OriginalBox.TabIndex = 0;
            this.OriginalBox.TabStop = false;
            this.OriginalBox.Text = "Original Picture";
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(6, 232);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(297, 21);
            this.comboBox1.TabIndex = 2;
            this.comboBox1.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // PicBox1
            // 
            this.PicBox1.Location = new System.Drawing.Point(6, 19);
            this.PicBox1.Name = "PicBox1";
            this.PicBox1.Size = new System.Drawing.Size(297, 207);
            this.PicBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PicBox1.TabIndex = 0;
            this.PicBox1.TabStop = false;
            // 
            // ModifiedBox
            // 
            this.ModifiedBox.Controls.Add(this.PicBox2);
            this.ModifiedBox.Location = new System.Drawing.Point(327, 12);
            this.ModifiedBox.Name = "ModifiedBox";
            this.ModifiedBox.Size = new System.Drawing.Size(309, 262);
            this.ModifiedBox.TabIndex = 1;
            this.ModifiedBox.TabStop = false;
            this.ModifiedBox.Text = "Modified Picture";
            // 
            // PicBox2
            // 
            this.PicBox2.Location = new System.Drawing.Point(6, 19);
            this.PicBox2.Name = "PicBox2";
            this.PicBox2.Size = new System.Drawing.Size(297, 207);
            this.PicBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.PicBox2.TabIndex = 1;
            this.PicBox2.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 293);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Filter Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(15, 322);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 3;
            this.label2.Text = "Filter Height";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(92, 290);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(71, 20);
            this.textBox1.TabIndex = 4;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(92, 319);
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(71, 20);
            this.textBox2.TabIndex = 5;
            this.textBox2.TextChanged += new System.EventHandler(this.textBox2_TextChanged);
            // 
            // ApplyBTN
            // 
            this.ApplyBTN.Location = new System.Drawing.Point(450, 293);
            this.ApplyBTN.Name = "ApplyBTN";
            this.ApplyBTN.Size = new System.Drawing.Size(75, 23);
            this.ApplyBTN.TabIndex = 6;
            this.ApplyBTN.Text = "Apply";
            this.ApplyBTN.UseVisualStyleBackColor = true;
            this.ApplyBTN.Click += new System.EventHandler(this.ApplyBTN_Click);
            // 
            // FilterGrid
            // 
            this.FilterGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.FilterGrid.Location = new System.Drawing.Point(169, 290);
            this.FilterGrid.Name = "FilterGrid";
            this.FilterGrid.Size = new System.Drawing.Size(264, 163);
            this.FilterGrid.TabIndex = 7;
            // 
            // CustomFilter
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(652, 465);
            this.Controls.Add(this.FilterGrid);
            this.Controls.Add(this.ApplyBTN);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ModifiedBox);
            this.Controls.Add(this.OriginalBox);
            this.Name = "CustomFilter";
            this.Text = "CustomFilter";
            this.Load += new System.EventHandler(this.CustomFilter_Load);
            this.OriginalBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicBox1)).EndInit();
            this.ModifiedBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.PicBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FilterGrid)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox OriginalBox;
        private System.Windows.Forms.PictureBox PicBox1;
        private System.Windows.Forms.GroupBox ModifiedBox;
        private System.Windows.Forms.PictureBox PicBox2;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button ApplyBTN;
        private System.Windows.Forms.DataGridView FilterGrid;
    }
}