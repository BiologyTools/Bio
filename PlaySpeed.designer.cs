
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
            ((System.ComponentModel.ISupportInitialize)(this.zPlayspeed)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.timePlayspeed)).BeginInit();
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
            this.zPlayspeed.Location = new System.Drawing.Point(131, 11);
            this.zPlayspeed.Maximum = new decimal(new int[] {
            1000,
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
            35,
            0,
            0,
            0});
            // 
            // timePlayspeed
            // 
            this.timePlayspeed.Location = new System.Drawing.Point(131, 37);
            this.timePlayspeed.Maximum = new decimal(new int[] {
            1000,
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
            35,
            0,
            0,
            0});
            this.timePlayspeed.ValueChanged += new System.EventHandler(this.timePlayspeed_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(195, 13);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(20, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "ms";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.White;
            this.label4.Location = new System.Drawing.Point(195, 42);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(20, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "ms";
            // 
            // PlaySpeed
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(98)))));
            this.ClientSize = new System.Drawing.Size(231, 76);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.timePlayspeed);
            this.Controls.Add(this.zPlayspeed);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
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
    }
}