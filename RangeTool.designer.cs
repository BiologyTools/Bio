
namespace BioImage
{
    partial class RangeTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RangeTool));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.timeMinBox = new System.Windows.Forms.NumericUpDown();
            this.zMinBox = new System.Windows.Forms.NumericUpDown();
            this.zMaxBox = new System.Windows.Forms.NumericUpDown();
            this.timeMaxBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.timeMinBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zMinBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.zMaxBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeMaxBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Time Range";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(65, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Slice Range";
            // 
            // timeMinBox
            // 
            this.timeMinBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(76)))));
            this.timeMinBox.ForeColor = System.Drawing.Color.White;
            this.timeMinBox.Location = new System.Drawing.Point(99, 26);
            this.timeMinBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.timeMinBox.Name = "timeMinBox";
            this.timeMinBox.Size = new System.Drawing.Size(86, 20);
            this.timeMinBox.TabIndex = 2;
            this.timeMinBox.ValueChanged += new System.EventHandler(this.timeMinBox_ValueChanged);
            // 
            // zMinBox
            // 
            this.zMinBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(76)))));
            this.zMinBox.ForeColor = System.Drawing.Color.White;
            this.zMinBox.Location = new System.Drawing.Point(99, 54);
            this.zMinBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.zMinBox.Name = "zMinBox";
            this.zMinBox.Size = new System.Drawing.Size(86, 20);
            this.zMinBox.TabIndex = 3;
            // 
            // zMaxBox
            // 
            this.zMaxBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(76)))));
            this.zMaxBox.ForeColor = System.Drawing.Color.White;
            this.zMaxBox.Location = new System.Drawing.Point(204, 54);
            this.zMaxBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.zMaxBox.Name = "zMaxBox";
            this.zMaxBox.Size = new System.Drawing.Size(86, 20);
            this.zMaxBox.TabIndex = 5;
            // 
            // timeMaxBox
            // 
            this.timeMaxBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(48)))), ((int)(((byte)(56)))), ((int)(((byte)(76)))));
            this.timeMaxBox.ForeColor = System.Drawing.Color.White;
            this.timeMaxBox.Location = new System.Drawing.Point(204, 26);
            this.timeMaxBox.Maximum = new decimal(new int[] {
            10000,
            0,
            0,
            0});
            this.timeMaxBox.Name = "timeMaxBox";
            this.timeMaxBox.Size = new System.Drawing.Size(86, 20);
            this.timeMaxBox.TabIndex = 4;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(99, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(23, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "min";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(203, 9);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(26, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "max";
            // 
            // RangeTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(98)))));
            this.ClientSize = new System.Drawing.Size(306, 87);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.zMaxBox);
            this.Controls.Add(this.timeMaxBox);
            this.Controls.Add(this.zMinBox);
            this.Controls.Add(this.timeMinBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "RangeTool";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Range Tool";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.RangeTool_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.timeMinBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zMinBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.zMaxBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.timeMaxBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown timeMinBox;
        private System.Windows.Forms.NumericUpDown zMinBox;
        private System.Windows.Forms.NumericUpDown zMaxBox;
        private System.Windows.Forms.NumericUpDown timeMaxBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
    }
}