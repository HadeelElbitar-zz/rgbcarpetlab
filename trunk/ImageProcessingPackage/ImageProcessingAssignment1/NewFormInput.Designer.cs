namespace ImageProcessingAssignment1
{
    partial class NewFormInput
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
            this.Widthtext = new System.Windows.Forms.TextBox();
            this.HeightText = new System.Windows.Forms.TextBox();
            this.WidthLabel = new System.Windows.Forms.Label();
            this.HeightLabel = new System.Windows.Forms.Label();
            this.Dimensions = new System.Windows.Forms.GroupBox();
            this.GoBtn = new System.Windows.Forms.Button();
            this.Dimensions.SuspendLayout();
            this.SuspendLayout();
            // 
            // Widthtext
            // 
            this.Widthtext.Location = new System.Drawing.Point(47, 30);
            this.Widthtext.Name = "Widthtext";
            this.Widthtext.Size = new System.Drawing.Size(100, 20);
            this.Widthtext.TabIndex = 0;
            // 
            // HeightText
            // 
            this.HeightText.Location = new System.Drawing.Point(47, 56);
            this.HeightText.Name = "HeightText";
            this.HeightText.Size = new System.Drawing.Size(100, 20);
            this.HeightText.TabIndex = 1;
            // 
            // WidthLabel
            // 
            this.WidthLabel.AutoSize = true;
            this.WidthLabel.Location = new System.Drawing.Point(6, 30);
            this.WidthLabel.Name = "WidthLabel";
            this.WidthLabel.Size = new System.Drawing.Size(35, 13);
            this.WidthLabel.TabIndex = 2;
            this.WidthLabel.Text = "Width";
            // 
            // HeightLabel
            // 
            this.HeightLabel.AutoSize = true;
            this.HeightLabel.Location = new System.Drawing.Point(6, 56);
            this.HeightLabel.Name = "HeightLabel";
            this.HeightLabel.Size = new System.Drawing.Size(38, 13);
            this.HeightLabel.TabIndex = 3;
            this.HeightLabel.Text = "Height";
            // 
            // Dimensions
            // 
            this.Dimensions.Controls.Add(this.HeightLabel);
            this.Dimensions.Controls.Add(this.WidthLabel);
            this.Dimensions.Controls.Add(this.HeightText);
            this.Dimensions.Controls.Add(this.Widthtext);
            this.Dimensions.ForeColor = System.Drawing.Color.White;
            this.Dimensions.Location = new System.Drawing.Point(18, 14);
            this.Dimensions.Name = "Dimensions";
            this.Dimensions.Size = new System.Drawing.Size(159, 92);
            this.Dimensions.TabIndex = 4;
            this.Dimensions.TabStop = false;
            this.Dimensions.Text = "Dimensions";
            // 
            // GoBtn
            // 
            this.GoBtn.Location = new System.Drawing.Point(59, 124);
            this.GoBtn.Name = "GoBtn";
            this.GoBtn.Size = new System.Drawing.Size(75, 23);
            this.GoBtn.TabIndex = 5;
            this.GoBtn.Text = "Go";
            this.GoBtn.UseVisualStyleBackColor = true;
            this.GoBtn.Click += new System.EventHandler(this.GoBtn_Click);
            // 
            // NewFormInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(197, 165);
            this.Controls.Add(this.GoBtn);
            this.Controls.Add(this.Dimensions);
            this.Name = "NewFormInput";
            this.Text = "New Image";
            this.Dimensions.ResumeLayout(false);
            this.Dimensions.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox Widthtext;
        private System.Windows.Forms.TextBox HeightText;
        private System.Windows.Forms.Label WidthLabel;
        private System.Windows.Forms.Label HeightLabel;
        private System.Windows.Forms.GroupBox Dimensions;
        private System.Windows.Forms.Button GoBtn;
    }
}