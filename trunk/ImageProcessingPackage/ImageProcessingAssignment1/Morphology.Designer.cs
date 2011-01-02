namespace ImageProcessingAssignment1
{
    partial class Morphology
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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.wTBOX = new System.Windows.Forms.TextBox();
            this.hTBOX = new System.Windows.Forms.TextBox();
            this.SetBTN = new System.Windows.Forms.Button();
            this.ResetBTN = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label4 = new System.Windows.Forms.Label();
            this.TriHeiBOX = new System.Windows.Forms.TextBox();
            this.TriWBOX = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.BoundExtrRBTN = new System.Windows.Forms.RadioButton();
            this.ClosingRBTN = new System.Windows.Forms.RadioButton();
            this.OpeningRBTN = new System.Windows.Forms.RadioButton();
            this.ErosionRBTN = new System.Windows.Forms.RadioButton();
            this.DilationRBTN = new System.Windows.Forms.RadioButton();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.RadiusTBOX = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.builtinStructureElementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.circularToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.customToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.triangleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Width";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(38, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Height";
            // 
            // wTBOX
            // 
            this.wTBOX.Location = new System.Drawing.Point(48, 14);
            this.wTBOX.Name = "wTBOX";
            this.wTBOX.Size = new System.Drawing.Size(100, 20);
            this.wTBOX.TabIndex = 2;
            this.wTBOX.TextChanged += new System.EventHandler(this.wTBOX_TextChanged);
            // 
            // hTBOX
            // 
            this.hTBOX.Location = new System.Drawing.Point(48, 39);
            this.hTBOX.Name = "hTBOX";
            this.hTBOX.Size = new System.Drawing.Size(100, 20);
            this.hTBOX.TabIndex = 3;
            this.hTBOX.TextChanged += new System.EventHandler(this.hTBOX_TextChanged);
            // 
            // SetBTN
            // 
            this.SetBTN.Enabled = false;
            this.SetBTN.Location = new System.Drawing.Point(587, 174);
            this.SetBTN.Name = "SetBTN";
            this.SetBTN.Size = new System.Drawing.Size(75, 23);
            this.SetBTN.TabIndex = 4;
            this.SetBTN.Text = "Set";
            this.SetBTN.UseVisualStyleBackColor = true;
            this.SetBTN.Click += new System.EventHandler(this.SetBTN_Click);
            // 
            // ResetBTN
            // 
            this.ResetBTN.Enabled = false;
            this.ResetBTN.Location = new System.Drawing.Point(587, 203);
            this.ResetBTN.Name = "ResetBTN";
            this.ResetBTN.Size = new System.Drawing.Size(75, 23);
            this.ResetBTN.TabIndex = 5;
            this.ResetBTN.Text = "Reset";
            this.ResetBTN.UseVisualStyleBackColor = true;
            this.ResetBTN.Click += new System.EventHandler(this.ResetBTN_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.pictureBox2);
            this.groupBox2.ForeColor = System.Drawing.Color.White;
            this.groupBox2.Location = new System.Drawing.Point(292, 27);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(250, 200);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Modified Picture";
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
            this.groupBox1.Location = new System.Drawing.Point(16, 27);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(250, 200);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Original Picture";
            this.groupBox1.UseWaitCursor = true;
            // 
            // button3
            // 
            this.button3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button3.Location = new System.Drawing.Point(587, 145);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 12;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button2.Location = new System.Drawing.Point(587, 114);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 11;
            this.button2.Text = "Ok";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.button1.Location = new System.Drawing.Point(587, 85);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 10;
            this.button1.Text = "Apply";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Checked = true;
            this.checkBox1.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBox1.ForeColor = System.Drawing.Color.WhiteSmoke;
            this.checkBox1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.checkBox1.Location = new System.Drawing.Point(590, 55);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(64, 17);
            this.checkBox1.TabIndex = 9;
            this.checkBox1.Text = "Preview";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.groupBox5);
            this.groupBox3.Controls.Add(this.groupBox7);
            this.groupBox3.Controls.Add(this.groupBox4);
            this.groupBox3.Controls.Add(this.groupBox6);
            this.groupBox3.ForeColor = System.Drawing.Color.White;
            this.groupBox3.Location = new System.Drawing.Point(16, 233);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(250, 181);
            this.groupBox3.TabIndex = 13;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "User Input";
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.label1);
            this.groupBox5.Controls.Add(this.hTBOX);
            this.groupBox5.Controls.Add(this.wTBOX);
            this.groupBox5.Controls.Add(this.label2);
            this.groupBox5.ForeColor = System.Drawing.Color.White;
            this.groupBox5.Location = new System.Drawing.Point(6, 13);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(238, 68);
            this.groupBox5.TabIndex = 14;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Custom";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label4);
            this.groupBox7.Controls.Add(this.TriHeiBOX);
            this.groupBox7.Controls.Add(this.TriWBOX);
            this.groupBox7.Controls.Add(this.label5);
            this.groupBox7.ForeColor = System.Drawing.Color.White;
            this.groupBox7.Location = new System.Drawing.Point(6, 13);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(238, 68);
            this.groupBox7.TabIndex = 15;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Triangular";
            this.groupBox7.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 17);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(30, 13);
            this.label4.TabIndex = 0;
            this.label4.Text = "Base";
            // 
            // TriHeiBOX
            // 
            this.TriHeiBOX.Location = new System.Drawing.Point(48, 39);
            this.TriHeiBOX.Name = "TriHeiBOX";
            this.TriHeiBOX.Size = new System.Drawing.Size(100, 20);
            this.TriHeiBOX.TabIndex = 3;
            this.TriHeiBOX.TextChanged += new System.EventHandler(this.TriHeiBOX_TextChanged);
            // 
            // TriWBOX
            // 
            this.TriWBOX.Location = new System.Drawing.Point(48, 14);
            this.TriWBOX.Name = "TriWBOX";
            this.TriWBOX.Size = new System.Drawing.Size(100, 20);
            this.TriWBOX.TabIndex = 2;
            this.TriWBOX.TextChanged += new System.EventHandler(this.TriWBOX_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(38, 13);
            this.label5.TabIndex = 1;
            this.label5.Text = "Height";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.BoundExtrRBTN);
            this.groupBox4.Controls.Add(this.ClosingRBTN);
            this.groupBox4.Controls.Add(this.OpeningRBTN);
            this.groupBox4.Controls.Add(this.ErosionRBTN);
            this.groupBox4.Controls.Add(this.DilationRBTN);
            this.groupBox4.ForeColor = System.Drawing.Color.White;
            this.groupBox4.Location = new System.Drawing.Point(4, 83);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(240, 87);
            this.groupBox4.TabIndex = 14;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Operation Type";
            // 
            // BoundExtrRBTN
            // 
            this.BoundExtrRBTN.AutoSize = true;
            this.BoundExtrRBTN.Location = new System.Drawing.Point(9, 62);
            this.BoundExtrRBTN.Name = "BoundExtrRBTN";
            this.BoundExtrRBTN.Size = new System.Drawing.Size(123, 17);
            this.BoundExtrRBTN.TabIndex = 17;
            this.BoundExtrRBTN.TabStop = true;
            this.BoundExtrRBTN.Text = "Boundary Extraction";
            this.BoundExtrRBTN.UseVisualStyleBackColor = true;
            this.BoundExtrRBTN.CheckedChanged += new System.EventHandler(this.BoundExtrRBTN_CheckedChanged);
            // 
            // ClosingRBTN
            // 
            this.ClosingRBTN.AutoSize = true;
            this.ClosingRBTN.Location = new System.Drawing.Point(84, 38);
            this.ClosingRBTN.Name = "ClosingRBTN";
            this.ClosingRBTN.Size = new System.Drawing.Size(59, 17);
            this.ClosingRBTN.TabIndex = 16;
            this.ClosingRBTN.TabStop = true;
            this.ClosingRBTN.Text = "Closing";
            this.ClosingRBTN.UseVisualStyleBackColor = true;
            this.ClosingRBTN.CheckedChanged += new System.EventHandler(this.ClosingRBTN_CheckedChanged);
            // 
            // OpeningRBTN
            // 
            this.OpeningRBTN.AutoSize = true;
            this.OpeningRBTN.Location = new System.Drawing.Point(84, 15);
            this.OpeningRBTN.Name = "OpeningRBTN";
            this.OpeningRBTN.Size = new System.Drawing.Size(65, 17);
            this.OpeningRBTN.TabIndex = 15;
            this.OpeningRBTN.TabStop = true;
            this.OpeningRBTN.Text = "Opening";
            this.OpeningRBTN.UseVisualStyleBackColor = true;
            this.OpeningRBTN.CheckedChanged += new System.EventHandler(this.OpeningRBTN_CheckedChanged);
            // 
            // ErosionRBTN
            // 
            this.ErosionRBTN.AutoSize = true;
            this.ErosionRBTN.Location = new System.Drawing.Point(9, 39);
            this.ErosionRBTN.Name = "ErosionRBTN";
            this.ErosionRBTN.Size = new System.Drawing.Size(60, 17);
            this.ErosionRBTN.TabIndex = 5;
            this.ErosionRBTN.Text = "Erosion";
            this.ErosionRBTN.UseVisualStyleBackColor = true;
            this.ErosionRBTN.CheckedChanged += new System.EventHandler(this.ErosionRBTN_CheckedChanged);
            // 
            // DilationRBTN
            // 
            this.DilationRBTN.AutoSize = true;
            this.DilationRBTN.Checked = true;
            this.DilationRBTN.Location = new System.Drawing.Point(9, 15);
            this.DilationRBTN.Name = "DilationRBTN";
            this.DilationRBTN.Size = new System.Drawing.Size(60, 17);
            this.DilationRBTN.TabIndex = 4;
            this.DilationRBTN.TabStop = true;
            this.DilationRBTN.Text = "Dilation";
            this.DilationRBTN.UseVisualStyleBackColor = true;
            this.DilationRBTN.CheckedChanged += new System.EventHandler(this.DilationRBTN_CheckedChanged);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.RadiusTBOX);
            this.groupBox6.ForeColor = System.Drawing.Color.White;
            this.groupBox6.Location = new System.Drawing.Point(6, 13);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(238, 68);
            this.groupBox6.TabIndex = 15;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Circular";
            this.groupBox6.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 30);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(39, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Radius";
            // 
            // RadiusTBOX
            // 
            this.RadiusTBOX.Location = new System.Drawing.Point(48, 27);
            this.RadiusTBOX.Name = "RadiusTBOX";
            this.RadiusTBOX.Size = new System.Drawing.Size(100, 20);
            this.RadiusTBOX.TabIndex = 2;
            this.RadiusTBOX.TextChanged += new System.EventHandler(this.RadiusTBOX_TextChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(100)))), ((int)(((byte)(100)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.builtinStructureElementsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(715, 24);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // builtinStructureElementsToolStripMenuItem
            // 
            this.builtinStructureElementsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.circularToolStripMenuItem,
            this.customToolStripMenuItem,
            this.triangleToolStripMenuItem});
            this.builtinStructureElementsToolStripMenuItem.Name = "builtinStructureElementsToolStripMenuItem";
            this.builtinStructureElementsToolStripMenuItem.Size = new System.Drawing.Size(160, 20);
            this.builtinStructureElementsToolStripMenuItem.Text = "Built-in Structure Elements";
            // 
            // circularToolStripMenuItem
            // 
            this.circularToolStripMenuItem.Name = "circularToolStripMenuItem";
            this.circularToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.circularToolStripMenuItem.Text = "Circular";
            this.circularToolStripMenuItem.Click += new System.EventHandler(this.circularToolStripMenuItem_Click);
            // 
            // customToolStripMenuItem
            // 
            this.customToolStripMenuItem.Name = "customToolStripMenuItem";
            this.customToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.customToolStripMenuItem.Text = "Custom";
            this.customToolStripMenuItem.Click += new System.EventHandler(this.customToolStripMenuItem_Click);
            // 
            // triangleToolStripMenuItem
            // 
            this.triangleToolStripMenuItem.Name = "triangleToolStripMenuItem";
            this.triangleToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
            this.triangleToolStripMenuItem.Text = "Triangle";
            this.triangleToolStripMenuItem.Click += new System.EventHandler(this.triangleToolStripMenuItem_Click);
            // 
            // Morphology
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(50)))), ((int)(((byte)(50)))));
            this.ClientSize = new System.Drawing.Size(715, 438);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.ResetBTN);
            this.Controls.Add(this.SetBTN);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Morphology";
            this.Text = "StructureElement";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Morphology_FormClosed);
            this.Load += new System.EventHandler(this.StructureElement_Load);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox wTBOX;
        private System.Windows.Forms.TextBox hTBOX;
        private System.Windows.Forms.Button SetBTN;
        private System.Windows.Forms.Button ResetBTN;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.RadioButton ErosionRBTN;
        private System.Windows.Forms.RadioButton DilationRBTN;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem builtinStructureElementsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem circularToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox RadiusTBOX;
        private System.Windows.Forms.ToolStripMenuItem customToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem triangleToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox TriHeiBOX;
        private System.Windows.Forms.TextBox TriWBOX;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton BoundExtrRBTN;
        private System.Windows.Forms.RadioButton ClosingRBTN;
        private System.Windows.Forms.RadioButton OpeningRBTN;
    }
}