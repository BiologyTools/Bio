namespace Bio
{
    partial class Series
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Series));
            this.imagesBox = new System.Windows.Forms.ListBox();
            this.seriesBox = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.addBut = new System.Windows.Forms.Button();
            this.removeBut = new System.Windows.Forms.Button();
            this.saveBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // imagesBox
            // 
            this.imagesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.imagesBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.imagesBox.ForeColor = System.Drawing.Color.White;
            this.imagesBox.FormattingEnabled = true;
            this.imagesBox.Location = new System.Drawing.Point(9, 22);
            this.imagesBox.Name = "imagesBox";
            this.imagesBox.Size = new System.Drawing.Size(162, 173);
            this.imagesBox.TabIndex = 0;
            // 
            // seriesBox
            // 
            this.seriesBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.seriesBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.seriesBox.ForeColor = System.Drawing.Color.White;
            this.seriesBox.FormattingEnabled = true;
            this.seriesBox.Location = new System.Drawing.Point(219, 22);
            this.seriesBox.Name = "seriesBox";
            this.seriesBox.Size = new System.Drawing.Size(162, 173);
            this.seriesBox.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(9, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Images";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(216, 6);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(36, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Series";
            // 
            // addBut
            // 
            this.addBut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.addBut.ForeColor = System.Drawing.Color.White;
            this.addBut.Location = new System.Drawing.Point(175, 22);
            this.addBut.Name = "addBut";
            this.addBut.Size = new System.Drawing.Size(38, 23);
            this.addBut.TabIndex = 23;
            this.addBut.Text = ">";
            this.addBut.UseVisualStyleBackColor = false;
            this.addBut.Click += new System.EventHandler(this.addBut_Click);
            // 
            // removeBut
            // 
            this.removeBut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.removeBut.ForeColor = System.Drawing.Color.White;
            this.removeBut.Location = new System.Drawing.Point(175, 51);
            this.removeBut.Name = "removeBut";
            this.removeBut.Size = new System.Drawing.Size(38, 23);
            this.removeBut.TabIndex = 24;
            this.removeBut.Text = "<";
            this.removeBut.UseVisualStyleBackColor = false;
            this.removeBut.Click += new System.EventHandler(this.removeBut_Click);
            // 
            // saveBut
            // 
            this.saveBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.saveBut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.saveBut.ForeColor = System.Drawing.Color.White;
            this.saveBut.Location = new System.Drawing.Point(310, 204);
            this.saveBut.Name = "saveBut";
            this.saveBut.Size = new System.Drawing.Size(68, 23);
            this.saveBut.TabIndex = 25;
            this.saveBut.Text = "Save";
            this.saveBut.UseVisualStyleBackColor = false;
            this.saveBut.Click += new System.EventHandler(this.saveBut_Click);
            // 
            // Series
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.ClientSize = new System.Drawing.Size(387, 236);
            this.Controls.Add(this.saveBut);
            this.Controls.Add(this.removeBut);
            this.Controls.Add(this.addBut);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.seriesBox);
            this.Controls.Add(this.imagesBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Series";
            this.Text = "Save Series";
            this.Activated += new System.EventHandler(this.Series_Activated);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ListBox imagesBox;
        private System.Windows.Forms.ListBox seriesBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button addBut;
        private System.Windows.Forms.Button removeBut;
        private System.Windows.Forms.Button saveBut;
    }
}