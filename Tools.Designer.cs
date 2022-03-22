namespace BioImage
{
    partial class Tools
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Tools));
            this.colorDialog = new System.Windows.Forms.ColorDialog();
            this.fontDialog = new System.Windows.Forms.FontDialog();
            this.stackApplyBox = new System.Windows.Forms.CheckBox();
            this.pencilPanel = new System.Windows.Forms.Panel();
            this.panel4 = new System.Windows.Forms.Panel();
            this.bucketPanel = new System.Windows.Forms.Panel();
            this.textPanel = new System.Windows.Forms.Panel();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rotatePanel = new System.Windows.Forms.Panel();
            this.panel7 = new System.Windows.Forms.Panel();
            this.movePanel = new System.Windows.Forms.Panel();
            this.resizePanel = new System.Windows.Forms.Panel();
            this.panel10 = new System.Windows.Forms.Panel();
            this.eraserPanel = new System.Windows.Forms.Panel();
            this.lassoPanel = new System.Windows.Forms.Panel();
            this.cropPanel = new System.Windows.Forms.Panel();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addNewToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.brushPanel = new System.Windows.Forms.Panel();
            this.rectPanel = new System.Windows.Forms.Panel();
            this.ellipsePanel = new System.Windows.Forms.Panel();
            this.linePanel = new System.Windows.Forms.Panel();
            this.polyPanel = new System.Windows.Forms.Panel();
            this.panel11 = new System.Windows.Forms.Panel();
            this.pointPanel = new System.Windows.Forms.Panel();
            this.panel13 = new System.Windows.Forms.Panel();
            this.bucketPanel.SuspendLayout();
            this.rotatePanel.SuspendLayout();
            this.movePanel.SuspendLayout();
            this.eraserPanel.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.polyPanel.SuspendLayout();
            this.pointPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // colorDialog
            // 
            this.colorDialog.Color = System.Drawing.Color.White;
            this.colorDialog.SolidColorOnly = true;
            // 
            // stackApplyBox
            // 
            this.stackApplyBox.AutoSize = true;
            this.stackApplyBox.ForeColor = System.Drawing.Color.Black;
            this.stackApplyBox.Location = new System.Drawing.Point(115, 92);
            this.stackApplyBox.Name = "stackApplyBox";
            this.stackApplyBox.Size = new System.Drawing.Size(54, 17);
            this.stackApplyBox.TabIndex = 8;
            this.stackApplyBox.Text = "Stack";
            this.stackApplyBox.UseVisualStyleBackColor = true;
            this.stackApplyBox.CheckedChanged += new System.EventHandler(this.stackBox_CheckedChanged);
            // 
            // pencilPanel
            // 
            this.pencilPanel.BackColor = System.Drawing.Color.White;
            this.pencilPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pencilPanel.BackgroundImage")));
            this.pencilPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pencilPanel.Location = new System.Drawing.Point(0, 0);
            this.pencilPanel.Name = "pencilPanel";
            this.pencilPanel.Size = new System.Drawing.Size(30, 30);
            this.pencilPanel.TabIndex = 0;
            this.pencilPanel.Click += new System.EventHandler(this.pencilPanel_Click);
            this.pencilPanel.DoubleClick += new System.EventHandler(this.pencilPanel_DoubleClick);
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
            // bucketPanel
            // 
            this.bucketPanel.BackColor = System.Drawing.Color.White;
            this.bucketPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("bucketPanel.BackgroundImage")));
            this.bucketPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.bucketPanel.Controls.Add(this.panel4);
            this.bucketPanel.Location = new System.Drawing.Point(115, 115);
            this.bucketPanel.Name = "bucketPanel";
            this.bucketPanel.Size = new System.Drawing.Size(30, 30);
            this.bucketPanel.TabIndex = 2;
            // 
            // textPanel
            // 
            this.textPanel.BackColor = System.Drawing.Color.White;
            this.textPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("textPanel.BackgroundImage")));
            this.textPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.textPanel.Location = new System.Drawing.Point(115, 60);
            this.textPanel.Name = "textPanel";
            this.textPanel.Size = new System.Drawing.Size(30, 30);
            this.textPanel.TabIndex = 3;
            this.textPanel.Click += new System.EventHandler(this.textPanel_Click);
            this.textPanel.DoubleClick += new System.EventHandler(this.textPanel_DoubleClick);
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
            // rotatePanel
            // 
            this.rotatePanel.BackColor = System.Drawing.Color.White;
            this.rotatePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rotatePanel.BackgroundImage")));
            this.rotatePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rotatePanel.Controls.Add(this.panel3);
            this.rotatePanel.Location = new System.Drawing.Point(115, 177);
            this.rotatePanel.Name = "rotatePanel";
            this.rotatePanel.Size = new System.Drawing.Size(30, 30);
            this.rotatePanel.TabIndex = 4;
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
            // movePanel
            // 
            this.movePanel.BackColor = System.Drawing.Color.LightGray;
            this.movePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("movePanel.BackgroundImage")));
            this.movePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.movePanel.Controls.Add(this.panel7);
            this.movePanel.Location = new System.Drawing.Point(29, 0);
            this.movePanel.Name = "movePanel";
            this.movePanel.Size = new System.Drawing.Size(30, 30);
            this.movePanel.TabIndex = 6;
            this.movePanel.Click += new System.EventHandler(this.movePanel_Click);
            // 
            // resizePanel
            // 
            this.resizePanel.BackColor = System.Drawing.Color.White;
            this.resizePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("resizePanel.BackgroundImage")));
            this.resizePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.resizePanel.Location = new System.Drawing.Point(146, 177);
            this.resizePanel.Name = "resizePanel";
            this.resizePanel.Size = new System.Drawing.Size(30, 30);
            this.resizePanel.TabIndex = 5;
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
            // eraserPanel
            // 
            this.eraserPanel.BackColor = System.Drawing.Color.White;
            this.eraserPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("eraserPanel.BackgroundImage")));
            this.eraserPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.eraserPanel.Controls.Add(this.panel10);
            this.eraserPanel.Location = new System.Drawing.Point(115, 146);
            this.eraserPanel.Name = "eraserPanel";
            this.eraserPanel.Size = new System.Drawing.Size(30, 30);
            this.eraserPanel.TabIndex = 4;
            // 
            // lassoPanel
            // 
            this.lassoPanel.BackColor = System.Drawing.Color.White;
            this.lassoPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("lassoPanel.BackgroundImage")));
            this.lassoPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.lassoPanel.Location = new System.Drawing.Point(146, 215);
            this.lassoPanel.Name = "lassoPanel";
            this.lassoPanel.Size = new System.Drawing.Size(30, 30);
            this.lassoPanel.TabIndex = 7;
            // 
            // cropPanel
            // 
            this.cropPanel.BackColor = System.Drawing.Color.White;
            this.cropPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("cropPanel.BackgroundImage")));
            this.cropPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.cropPanel.Location = new System.Drawing.Point(146, 146);
            this.cropPanel.Name = "cropPanel";
            this.cropPanel.Size = new System.Drawing.Size(30, 30);
            this.cropPanel.TabIndex = 5;
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addNewToolToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(149, 26);
            // 
            // addNewToolToolStripMenuItem
            // 
            this.addNewToolToolStripMenuItem.Name = "addNewToolToolStripMenuItem";
            this.addNewToolToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
            this.addNewToolToolStripMenuItem.Text = "Add New Tool";
            // 
            // brushPanel
            // 
            this.brushPanel.BackColor = System.Drawing.Color.White;
            this.brushPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("brushPanel.BackgroundImage")));
            this.brushPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.brushPanel.Location = new System.Drawing.Point(115, 215);
            this.brushPanel.Name = "brushPanel";
            this.brushPanel.Size = new System.Drawing.Size(30, 30);
            this.brushPanel.TabIndex = 1;
            // 
            // rectPanel
            // 
            this.rectPanel.BackColor = System.Drawing.Color.White;
            this.rectPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("rectPanel.BackgroundImage")));
            this.rectPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.rectPanel.Location = new System.Drawing.Point(29, 30);
            this.rectPanel.Name = "rectPanel";
            this.rectPanel.Size = new System.Drawing.Size(30, 30);
            this.rectPanel.TabIndex = 2;
            this.rectPanel.Click += new System.EventHandler(this.rectPanel_Click);
            // 
            // ellipsePanel
            // 
            this.ellipsePanel.BackColor = System.Drawing.Color.White;
            this.ellipsePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("ellipsePanel.BackgroundImage")));
            this.ellipsePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ellipsePanel.Location = new System.Drawing.Point(0, 30);
            this.ellipsePanel.Name = "ellipsePanel";
            this.ellipsePanel.Size = new System.Drawing.Size(30, 30);
            this.ellipsePanel.TabIndex = 8;
            this.ellipsePanel.Click += new System.EventHandler(this.ellipsePanel_Click);
            // 
            // linePanel
            // 
            this.linePanel.BackColor = System.Drawing.Color.White;
            this.linePanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("linePanel.BackgroundImage")));
            this.linePanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.linePanel.Location = new System.Drawing.Point(29, 60);
            this.linePanel.Name = "linePanel";
            this.linePanel.Size = new System.Drawing.Size(30, 30);
            this.linePanel.TabIndex = 3;
            this.linePanel.Click += new System.EventHandler(this.linePanel_Click);
            // 
            // polyPanel
            // 
            this.polyPanel.BackColor = System.Drawing.Color.White;
            this.polyPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("polyPanel.BackgroundImage")));
            this.polyPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.polyPanel.Controls.Add(this.panel11);
            this.polyPanel.Location = new System.Drawing.Point(0, 90);
            this.polyPanel.Name = "polyPanel";
            this.polyPanel.Size = new System.Drawing.Size(30, 30);
            this.polyPanel.TabIndex = 4;
            this.polyPanel.Click += new System.EventHandler(this.polyPanel_Click);
            // 
            // panel11
            // 
            this.panel11.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel11.BackgroundImage")));
            this.panel11.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel11.Location = new System.Drawing.Point(0, 33);
            this.panel11.Name = "panel11";
            this.panel11.Size = new System.Drawing.Size(30, 30);
            this.panel11.TabIndex = 3;
            // 
            // pointPanel
            // 
            this.pointPanel.BackColor = System.Drawing.Color.White;
            this.pointPanel.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("pointPanel.BackgroundImage")));
            this.pointPanel.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.pointPanel.Controls.Add(this.panel13);
            this.pointPanel.Location = new System.Drawing.Point(0, 60);
            this.pointPanel.Name = "pointPanel";
            this.pointPanel.Size = new System.Drawing.Size(30, 30);
            this.pointPanel.TabIndex = 5;
            this.pointPanel.Click += new System.EventHandler(this.pointPanel_Click);
            // 
            // panel13
            // 
            this.panel13.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("panel13.BackgroundImage")));
            this.panel13.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.panel13.Location = new System.Drawing.Point(0, 33);
            this.panel13.Name = "panel13";
            this.panel13.Size = new System.Drawing.Size(30, 30);
            this.panel13.TabIndex = 3;
            // 
            // Tools
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.ClientSize = new System.Drawing.Size(59, 121);
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Controls.Add(this.textPanel);
            this.Controls.Add(this.brushPanel);
            this.Controls.Add(this.pointPanel);
            this.Controls.Add(this.polyPanel);
            this.Controls.Add(this.linePanel);
            this.Controls.Add(this.ellipsePanel);
            this.Controls.Add(this.rectPanel);
            this.Controls.Add(this.stackApplyBox);
            this.Controls.Add(this.cropPanel);
            this.Controls.Add(this.lassoPanel);
            this.Controls.Add(this.eraserPanel);
            this.Controls.Add(this.resizePanel);
            this.Controls.Add(this.movePanel);
            this.Controls.Add(this.rotatePanel);
            this.Controls.Add(this.bucketPanel);
            this.Controls.Add(this.pencilPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Tools";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Tools";
            this.TopMost = true;
            this.bucketPanel.ResumeLayout(false);
            this.rotatePanel.ResumeLayout(false);
            this.movePanel.ResumeLayout(false);
            this.eraserPanel.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.polyPanel.ResumeLayout(false);
            this.pointPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ColorDialog colorDialog;
        private System.Windows.Forms.FontDialog fontDialog;
        private System.Windows.Forms.CheckBox stackApplyBox;
        private System.Windows.Forms.Panel pencilPanel;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel bucketPanel;
        private System.Windows.Forms.Panel textPanel;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel rotatePanel;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.Panel movePanel;
        private System.Windows.Forms.Panel resizePanel;
        private System.Windows.Forms.Panel panel10;
        private System.Windows.Forms.Panel eraserPanel;
        private System.Windows.Forms.Panel lassoPanel;
        private System.Windows.Forms.Panel cropPanel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem addNewToolToolStripMenuItem;
        private System.Windows.Forms.Panel brushPanel;
        private System.Windows.Forms.Panel rectPanel;
        private System.Windows.Forms.Panel ellipsePanel;
        private System.Windows.Forms.Panel linePanel;
        private System.Windows.Forms.Panel polyPanel;
        private System.Windows.Forms.Panel panel11;
        private System.Windows.Forms.Panel pointPanel;
        private System.Windows.Forms.Panel panel13;
    }
}