namespace BioImage
{
    partial class Slider
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.SliderBar = new System.Windows.Forms.TrackBar();
            this.idLabel = new System.Windows.Forms.Label();
            this.statLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.SliderBar)).BeginInit();
            this.SuspendLayout();
            // 
            // SliderBar
            // 
            this.SliderBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SliderBar.AutoSize = false;
            this.SliderBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.SliderBar.LargeChange = 1;
            this.SliderBar.Location = new System.Drawing.Point(32, 0);
            this.SliderBar.Margin = new System.Windows.Forms.Padding(0);
            this.SliderBar.Name = "SliderBar";
            this.SliderBar.Size = new System.Drawing.Size(453, 25);
            this.SliderBar.TabIndex = 13;
            this.SliderBar.ValueChanged += new System.EventHandler(this.SliderBar_ValueChanged);
            // 
            // idLabel
            // 
            this.idLabel.AutoSize = true;
            this.idLabel.ForeColor = System.Drawing.Color.White;
            this.idLabel.Location = new System.Drawing.Point(4, 6);
            this.idLabel.Name = "idLabel";
            this.idLabel.Size = new System.Drawing.Size(15, 13);
            this.idLabel.TabIndex = 14;
            this.idLabel.Text = "R";
            // 
            // statLabel
            // 
            this.statLabel.AutoSize = true;
            this.statLabel.ForeColor = System.Drawing.Color.White;
            this.statLabel.Location = new System.Drawing.Point(488, 6);
            this.statLabel.Name = "statLabel";
            this.statLabel.Size = new System.Drawing.Size(15, 13);
            this.statLabel.TabIndex = 15;
            this.statLabel.Text = "R";
            // 
            // Slider
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.Controls.Add(this.statLabel);
            this.Controls.Add(this.idLabel);
            this.Controls.Add(this.SliderBar);
            this.Name = "Slider";
            this.Size = new System.Drawing.Size(537, 25);
            ((System.ComponentModel.ISupportInitialize)(this.SliderBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TrackBar SliderBar;
        private System.Windows.Forms.Label idLabel;
        private System.Windows.Forms.Label statLabel;
    }
}
