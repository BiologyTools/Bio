namespace BioImage
{
    partial class ScriptRunner
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptRunner));
            this.scriptView = new System.Windows.Forms.ListView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.runToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openScriptFolderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // scriptView
            // 
            this.scriptView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.scriptView.ContextMenuStrip = this.contextMenuStrip;
            this.scriptView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scriptView.ForeColor = System.Drawing.Color.White;
            this.scriptView.HideSelection = false;
            this.scriptView.Location = new System.Drawing.Point(0, 0);
            this.scriptView.MultiSelect = false;
            this.scriptView.Name = "scriptView";
            this.scriptView.Size = new System.Drawing.Size(412, 300);
            this.scriptView.Sorting = System.Windows.Forms.SortOrder.Ascending;
            this.scriptView.TabIndex = 1;
            this.scriptView.UseCompatibleStateImageBehavior = false;
            this.scriptView.View = System.Windows.Forms.View.List;
            this.scriptView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.scriptView_MouseDoubleClick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.runToolStripMenuItem,
            this.openScriptFolderToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(181, 70);
            // 
            // runToolStripMenuItem
            // 
            this.runToolStripMenuItem.Name = "runToolStripMenuItem";
            this.runToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.runToolStripMenuItem.Text = "Run";
            this.runToolStripMenuItem.Click += new System.EventHandler(this.runToolStripMenuItem_Click);
            // 
            // openScriptFolderToolStripMenuItem
            // 
            this.openScriptFolderToolStripMenuItem.Name = "openScriptFolderToolStripMenuItem";
            this.openScriptFolderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.openScriptFolderToolStripMenuItem.Text = "Open Script Folder";
            this.openScriptFolderToolStripMenuItem.Click += new System.EventHandler(this.openScriptFolderToolStripMenuItem_Click);
            // 
            // ScriptRunner
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.ClientSize = new System.Drawing.Size(412, 300);
            this.Controls.Add(this.scriptView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScriptRunner";
            this.Text = "Script Runner";
            this.contextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ListView scriptView;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem runToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openScriptFolderToolStripMenuItem;
    }
}