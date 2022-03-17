
namespace BioImage
{
    partial class PlaySpeed
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PlaySpeed));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.zPlayspeed = new System.Windows.Forms.NumericUpDown();
            this.timePlayspeed = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.timeFpsBox = new System.Windows.Forms.NumericUpDown();
            this.zFpsBox = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.cFpsBox = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.cPlayspeed = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.zPlayspeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timePlayspeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeFpsBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zFpsBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cFpsBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cPlayspeed)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(14, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Slice Playspeed";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(14, 42);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(98, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Timeline Playspeed";
            // 
            // zPlayspeed
            // 
            this.zPlayspeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.zPlayspeed.ForeColor = System.Drawing.Color.White;
            this.zPlayspeed.Location = new System.Drawing.Point(131, 11);
            this.zPlayspeed.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.zPlayspeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.zPlayspeed.Name = "zPlayspeed";
            this.zPlayspeed.Size = new System.Drawing.Size(58, 20);
            this.zPlayspeed.TabIndex = 2;
            this.zPlayspeed.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // timePlayspeed
            // 
            this.timePlayspeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.timePlayspeed.ForeColor = System.Drawing.Color.White;
            this.timePlayspeed.Location = new System.Drawing.Point(131, 37);
            this.timePlayspeed.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.timePlayspeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timePlayspeed.Name = "timePlayspeed";
            this.timePlayspeed.Size = new System.Drawing.Size(58, 20);
            this.timePlayspeed.TabIndex = 3;
            this.timePlayspeed.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(195, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "ms";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(195, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "ms";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(285, 42);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(21, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "fps";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(285, 13);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(21, 13);
            this.label6.TabIndex = 8;
            this.label6.Text = "fps";
            // 
            // timeFpsBox
            // 
            this.timeFpsBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.timeFpsBox.ForeColor = System.Drawing.Color.White;
            this.timeFpsBox.Location = new System.Drawing.Point(221, 37);
            this.timeFpsBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.timeFpsBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.timeFpsBox.Name = "timeFpsBox";
            this.timeFpsBox.Size = new System.Drawing.Size(58, 20);
            this.timeFpsBox.TabIndex = 7;
            this.timeFpsBox.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.timeFpsBox.ValueChanged += new System.EventHandler(this.timeFpsBox_ValueChanged);
            // 
            // zFpsBox
            // 
            this.zFpsBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.zFpsBox.ForeColor = System.Drawing.Color.White;
            this.zFpsBox.Location = new System.Drawing.Point(221, 11);
            this.zFpsBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.zFpsBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.zFpsBox.Name = "zFpsBox";
            this.zFpsBox.Size = new System.Drawing.Size(58, 20);
            this.zFpsBox.TabIndex = 6;
            this.zFpsBox.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.zFpsBox.ValueChanged += new System.EventHandler(this.zFpsBox_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(285, 70);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(21, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "fps";
            // 
            // cFpsBox
            // 
            this.cFpsBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.cFpsBox.ForeColor = System.Drawing.Color.White;
            this.cFpsBox.Location = new System.Drawing.Point(221, 65);
            this.cFpsBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.cFpsBox.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.cFpsBox.Name = "cFpsBox";
            this.cFpsBox.Size = new System.Drawing.Size(58, 20);
            this.cFpsBox.TabIndex = 13;
            this.cFpsBox.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.cFpsBox.ValueChanged += new System.EventHandler(this.cFpsBox_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(195, 70);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(20, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "ms";
            // 
            // cPlayspeed
            // 
            this.cPlayspeed.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.cPlayspeed.ForeColor = System.Drawing.Color.White;
            this.cPlayspeed.Location = new System.Drawing.Point(131, 65);
            this.cPlayspeed.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.cPlayspeed.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.cPlayspeed.Name = "cPlayspeed";
            this.cPlayspeed.Size = new System.Drawing.Size(58, 20);
            this.cPlayspeed.TabIndex = 11;
            this.cPlayspeed.Value = new decimal(new int[] {
            16,
            0,
            0,
            0});
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.ForeColor = System.Drawing.Color.White;
            this.label9.Location = new System.Drawing.Point(14, 67);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 13);
            this.label9.TabIndex = 10;
            this.label9.Text = "C Playspeed";
            // 
            // PlaySpeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.ClientSize = new System.Drawing.Size(332, 97);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cFpsBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.cPlayspeed);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.timeFpsBox);
            this.Controls.Add(this.zFpsBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.timePlayspeed);
            this.Controls.Add(this.zPlayspeed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "PlaySpeed";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Play Speed";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.PlaySpeed_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.zPlayspeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timePlayspeed)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeFpsBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zFpsBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cFpsBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cPlayspeed)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown zPlayspeed;
        private System.Windows.Forms.NumericUpDown timePlayspeed;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown timeFpsBox;
        private System.Windows.Forms.NumericUpDown zFpsBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown cFpsBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.NumericUpDown cPlayspeed;
        private System.Windows.Forms.Label label9;
    }
}