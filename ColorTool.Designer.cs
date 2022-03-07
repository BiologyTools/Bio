namespace BioImage
{
    partial class ColorTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ColorTool));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.redBox = new System.Windows.Forms.NumericUpDown();
            this.greenBox = new System.Windows.Forms.NumericUpDown();
            this.blueBox = new System.Windows.Forms.NumericUpDown();
            this.colorPanel = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.redBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(18, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "R:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 35);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "G:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "B:";
            // 
            // redBox
            // 
            this.redBox.Location = new System.Drawing.Point(36, 7);
            this.redBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.redBox.Name = "redBox";
            this.redBox.Size = new System.Drawing.Size(80, 20);
            this.redBox.TabIndex = 3;
            this.redBox.ValueChanged += new System.EventHandler(this.redBox_ValueChanged);
            // 
            // greenBox
            // 
            this.greenBox.Location = new System.Drawing.Point(36, 33);
            this.greenBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.greenBox.Name = "greenBox";
            this.greenBox.Size = new System.Drawing.Size(80, 20);
            this.greenBox.TabIndex = 4;
            this.greenBox.ValueChanged += new System.EventHandler(this.greenBox_ValueChanged);
            // 
            // blueBox
            // 
            this.blueBox.Location = new System.Drawing.Point(35, 60);
            this.blueBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.blueBox.Name = "blueBox";
            this.blueBox.Size = new System.Drawing.Size(80, 20);
            this.blueBox.TabIndex = 5;
            this.blueBox.ValueChanged += new System.EventHandler(this.blueBox_ValueChanged);
            // 
            // colorPanel
            // 
            this.colorPanel.Location = new System.Drawing.Point(137, 12);
            this.colorPanel.Name = "colorPanel";
            this.colorPanel.Size = new System.Drawing.Size(64, 64);
            this.colorPanel.TabIndex = 6;
            // 
            // ColorTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(223, 88);
            this.Controls.Add(this.colorPanel);
            this.Controls.Add(this.blueBox);
            this.Controls.Add(this.greenBox);
            this.Controls.Add(this.redBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ColorTool";
            this.ShowInTaskbar = false;
            this.Text = "Color Tool";
            ((System.ComponentModel.ISupportInitialize)(this.redBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.greenBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.blueBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown redBox;
        private System.Windows.Forms.NumericUpDown greenBox;
        private System.Windows.Forms.NumericUpDown blueBox;
        private System.Windows.Forms.Panel colorPanel;
    }
}