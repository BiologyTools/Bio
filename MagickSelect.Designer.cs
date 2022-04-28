namespace BioImage
{
    partial class MagickSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MagickSelect));
            this.label1 = new System.Windows.Forms.Label();
            this.thBox = new System.Windows.Forms.ComboBox();
            this.numBox = new System.Windows.Forms.NumericUpDown();
            this.numericBox = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.numBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Threshold";
            // 
            // thBox
            // 
            this.thBox.FormattingEnabled = true;
            this.thBox.Items.AddRange(new object[] {
            "Min",
            "Median",
            "Median - Min"});
            this.thBox.Location = new System.Drawing.Point(95, 6);
            this.thBox.Name = "thBox";
            this.thBox.Size = new System.Drawing.Size(121, 21);
            this.thBox.TabIndex = 1;
            // 
            // numBox
            // 
            this.numBox.Location = new System.Drawing.Point(95, 33);
            this.numBox.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.numBox.Name = "numBox";
            this.numBox.Size = new System.Drawing.Size(120, 20);
            this.numBox.TabIndex = 2;
            // 
            // numericBox
            // 
            this.numericBox.AutoSize = true;
            this.numericBox.ForeColor = System.Drawing.Color.White;
            this.numericBox.Location = new System.Drawing.Point(12, 34);
            this.numericBox.Name = "numericBox";
            this.numericBox.Size = new System.Drawing.Size(65, 17);
            this.numericBox.TabIndex = 3;
            this.numericBox.Text = "Numeric";
            this.numericBox.UseVisualStyleBackColor = true;
            this.numericBox.CheckedChanged += new System.EventHandler(this.numericBox_CheckedChanged);
            // 
            // MagickSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.ClientSize = new System.Drawing.Size(224, 65);
            this.Controls.Add(this.numericBox);
            this.Controls.Add(this.numBox);
            this.Controls.Add(this.thBox);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MagickSelect";
            this.ShowInTaskbar = false;
            this.Text = "Magick Select";
            ((System.ComponentModel.ISupportInitialize)(this.numBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox thBox;
        private System.Windows.Forms.NumericUpDown numBox;
        private System.Windows.Forms.CheckBox numericBox;
    }
}