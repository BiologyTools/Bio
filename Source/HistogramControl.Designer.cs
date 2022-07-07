namespace Bio
{
    partial class HistogramControl
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
            this.components = new System.ComponentModel.Container();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyViewToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyViewToClipboardToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(200, 26);
            // 
            // copyViewToClipboardToolStripMenuItem
            // 
            this.copyViewToClipboardToolStripMenuItem.Name = "copyViewToClipboardToolStripMenuItem";
            this.copyViewToClipboardToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.copyViewToClipboardToolStripMenuItem.Text = "Copy View to Clipboard";
            this.copyViewToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyViewToClipboardToolStripMenuItem_Click);
            // 
            // HistogramControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ContextMenuStrip = this.contextMenuStrip;
            this.Cursor = System.Windows.Forms.Cursors.Cross;
            this.DoubleBuffered = true;
            this.Name = "HistogramControl";
            this.SizeChanged += new System.EventHandler(this.HistogramControl_SizeChanged);
            this.Paint += new System.Windows.Forms.PaintEventHandler(this.HistogramControl_Paint);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.HistogramControl_MouseDown);
            this.MouseMove += new System.Windows.Forms.MouseEventHandler(this.HistogramControl_MouseMove);
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem copyViewToClipboardToolStripMenuItem;
    }
}
