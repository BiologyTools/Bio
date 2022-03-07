namespace BioImage
{
    partial class Toolbox
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Toolbox));
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.pencilPanel = new System.Windows.Forms.Panel();
            this.brushPanel = new System.Windows.Forms.Panel();
            this.textPanel = new System.Windows.Forms.Panel();
            this.bucketPanel = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.resizePanel = new System.Windows.Forms.Panel();
            this.rotatePanel = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lassoPanel = new System.Windows.Forms.Panel();
            this.selectPanel = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.cropPanel = new System.Windows.Forms.Panel();
            this.eraserPanel = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.stackApplyBox = new System.Windows.Forms.CheckBox();
            this.flipYPanel = new System.Windows.Forms.Panel();
            this.flipXPanel = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.bucketPanel.SuspendLayout();
            this.rotatePanel.SuspendLayout();
            this.selectPanel.SuspendLayout();
            this.eraserPanel.SuspendLayout();
            this.flipXPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorDialog
            // 
            this.colorDialog.Color = System.Drawing.Color.White;
            this.colorDialog.SolidColorOnly = true;
            // 
            // pencilPanel
            // 
            this.pencilPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pencilPanel.BackgroundImage")));
            this.pencilPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pencilPanel.Location = new System.Drawing.Point(0, 0);
            this.pencilPanel.Name = "pencilPanel";
            this.pencilPanel.Size = new System.Drawing.Size(30, 30);
            this.pencilPanel.TabIndex = 0;
            this.pencilPanel.Click += new System.EventHandler(this.pencilPanel_Click);
            this.pencilPanel.DoubleClick += new System.EventHandler(this.pencilPanel_DoubleClick);
            // 
            // brushPanel
            // 
            this.brushPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("brushPanel.BackgroundImage")));
            this.brushPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.brushPanel.Location = new System.Drawing.Point(31, 0);
            this.brushPanel.Name = "brushPanel";
            this.brushPanel.Size = new System.Drawing.Size(30, 30);
            this.brushPanel.TabIndex = 1;
            // 
            // textPanel
            // 
            this.textPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textPanel.BackgroundImage")));
            this.textPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.textPanel.Location = new System.Drawing.Point(31, 31);
            this.textPanel.Name = "textPanel";
            this.textPanel.Size = new System.Drawing.Size(30, 30);
            this.textPanel.TabIndex = 3;
            this.textPanel.DoubleClick += new System.EventHandler(this.textPanel_DoubleClick);
            // 
            // bucketPanel
            // 
            this.bucketPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bucketPanel.BackgroundImage")));
            this.bucketPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bucketPanel.Controls.Add(this.panel4);
            this.bucketPanel.Location = new System.Drawing.Point(0, 31);
            this.bucketPanel.Name = "bucketPanel";
            this.bucketPanel.Size = new System.Drawing.Size(30, 30);
            this.bucketPanel.TabIndex = 2;
            // 
            // panel4
            // 
            this.panel4.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel4.BackgroundImage")));
            this.panel4.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel4.Location = new System.Drawing.Point(0, 33);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(30, 30);
            this.panel4.TabIndex = 3;
            // 
            // resizePanel
            // 
            this.resizePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("resizePanel.BackgroundImage")));
            this.resizePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.resizePanel.Location = new System.Drawing.Point(31, 93);
            this.resizePanel.Name = "resizePanel";
            this.resizePanel.Size = new System.Drawing.Size(30, 30);
            this.resizePanel.TabIndex = 5;
            // 
            // rotatePanel
            // 
            this.rotatePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rotatePanel.BackgroundImage")));
            this.rotatePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rotatePanel.Controls.Add(this.panel3);
            this.rotatePanel.Location = new System.Drawing.Point(0, 93);
            this.rotatePanel.Name = "rotatePanel";
            this.rotatePanel.Size = new System.Drawing.Size(30, 30);
            this.rotatePanel.TabIndex = 4;
            // 
            // panel3
            // 
            this.panel3.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel3.BackgroundImage")));
            this.panel3.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel3.Location = new System.Drawing.Point(0, 33);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(30, 30);
            this.panel3.TabIndex = 3;
            // 
            // lassoPanel
            // 
            this.lassoPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lassoPanel.BackgroundImage")));
            this.lassoPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lassoPanel.Location = new System.Drawing.Point(31, 155);
            this.lassoPanel.Name = "lassoPanel";
            this.lassoPanel.Size = new System.Drawing.Size(30, 30);
            this.lassoPanel.TabIndex = 7;
            // 
            // selectPanel
            // 
            this.selectPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("selectPanel.BackgroundImage")));
            this.selectPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.selectPanel.Controls.Add(this.panel7);
            this.selectPanel.Location = new System.Drawing.Point(0, 155);
            this.selectPanel.Name = "selectPanel";
            this.selectPanel.Size = new System.Drawing.Size(30, 30);
            this.selectPanel.TabIndex = 6;
            // 
            // panel7
            // 
            this.panel7.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel7.BackgroundImage")));
            this.panel7.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel7.Location = new System.Drawing.Point(0, 33);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(30, 30);
            this.panel7.TabIndex = 3;
            // 
            // cropPanel
            // 
            this.cropPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cropPanel.BackgroundImage")));
            this.cropPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cropPanel.Location = new System.Drawing.Point(31, 62);
            this.cropPanel.Name = "cropPanel";
            this.cropPanel.Size = new System.Drawing.Size(30, 30);
            this.cropPanel.TabIndex = 5;
            // 
            // eraserPanel
            // 
            this.eraserPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("eraserPanel.BackgroundImage")));
            this.eraserPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.eraserPanel.Controls.Add(this.panel10);
            this.eraserPanel.Location = new System.Drawing.Point(0, 62);
            this.eraserPanel.Name = "eraserPanel";
            this.eraserPanel.Size = new System.Drawing.Size(30, 30);
            this.eraserPanel.TabIndex = 4;
            // 
            // panel10
            // 
            this.panel10.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel10.BackgroundImage")));
            this.panel10.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel10.Location = new System.Drawing.Point(0, 33);
            this.panel10.Name = "panel10";
            this.panel10.Size = new System.Drawing.Size(30, 30);
            this.panel10.TabIndex = 3;
            // 
            // stackApplyBox
            // 
            this.stackApplyBox.AutoSize = true;
            this.stackApplyBox.Location = new System.Drawing.Point(5, 189);
            this.stackApplyBox.Name = "stackApplyBox";
            this.stackApplyBox.Size = new System.Drawing.Size(54, 17);
            this.stackApplyBox.TabIndex = 8;
            this.stackApplyBox.Text = "Stack";
            this.stackApplyBox.UseVisualStyleBackColor = true;
            this.stackApplyBox.CheckedChanged += new System.EventHandler(this.stackBox_CheckedChanged);
            // 
            // flipYPanel
            // 
            this.flipYPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("flipYPanel.BackgroundImage")));
            this.flipYPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.flipYPanel.Location = new System.Drawing.Point(31, 124);
            this.flipYPanel.Name = "flipYPanel";
            this.flipYPanel.Size = new System.Drawing.Size(30, 30);
            this.flipYPanel.TabIndex = 7;
            this.flipYPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.flipYPanel_Paint);
            // 
            // flipXPanel
            // 
            this.flipXPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("flipXPanel.BackgroundImage")));
            this.flipXPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.flipXPanel.Controls.Add(this.panel5);
            this.flipXPanel.Location = new System.Drawing.Point(0, 124);
            this.flipXPanel.Name = "flipXPanel";
            this.flipXPanel.Size = new System.Drawing.Size(30, 30);
            this.flipXPanel.TabIndex = 6;
            this.flipXPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.flipXPanel_Paint);
            // 
            // panel5
            // 
            this.panel5.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel5.BackgroundImage")));
            this.panel5.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel5.Location = new System.Drawing.Point(0, 33);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(30, 30);
            this.panel5.TabIndex = 3;
            // 
            // Toolbox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(62, 210);
            this.Controls.Add(this.flipYPanel);
            this.Controls.Add(this.stackApplyBox);
            this.Controls.Add(this.flipXPanel);
            this.Controls.Add(this.cropPanel);
            this.Controls.Add(this.lassoPanel);
            this.Controls.Add(this.eraserPanel);
            this.Controls.Add(this.resizePanel);
            this.Controls.Add(this.selectPanel);
            this.Controls.Add(this.rotatePanel);
            this.Controls.Add(this.textPanel);
            this.Controls.Add(this.bucketPanel);
            this.Controls.Add(this.brushPanel);
            this.Controls.Add(this.pencilPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Toolbox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Toolbox";
            this.bucketPanel.ResumeLayout(false);
            this.rotatePanel.ResumeLayout(false);
            this.selectPanel.ResumeLayout(false);
            this.eraserPanel.ResumeLayout(false);
            this.flipXPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.Panel pencilPanel;
        private System.Windows.Forms.Panel brushPanel;
        private System.Windows.Forms.Panel textPanel;
        private System.Windows.Forms.Panel bucketPanel;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel resizePanel;
        private System.Windows.Forms.Panel rotatePanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel lassoPanel;
        private System.Windows.Forms.Panel selectPanel;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel cropPanel;
        private System.Windows.Forms.Panel eraserPanel;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.CheckBox stackApplyBox;
        private System.Windows.Forms.Panel flipYPanel;
        private System.Windows.Forms.Panel flipXPanel;
        private System.Windows.Forms.Panel panel5;
    }
}