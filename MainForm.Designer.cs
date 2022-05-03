﻿namespace BioImage
{
    partial class MainForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.treeView = new System.Windows.Forms.TreeView();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setTextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setIDToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptRunnerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptRecorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFilesDialog = new System.Windows.Forms.OpenFileDialog();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip.SuspendLayout();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeView
            // 
            this.treeView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.treeView.ContextMenuStrip = this.contextMenuStrip;
            this.treeView.Location = new System.Drawing.Point(-1, 27);
            this.treeView.Name = "treeView";
            this.treeView.Size = new System.Drawing.Size(311, 219);
            this.treeView.TabIndex = 2;
            this.treeView.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView_AfterSelect);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.setTextToolStripMenuItem,
            this.setIDToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(137, 70);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.deleteToolStripMenuItem.Text = "Delete";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // setTextToolStripMenuItem
            // 
            this.setTextToolStripMenuItem.Name = "setTextToolStripMenuItem";
            this.setTextToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.setTextToolStripMenuItem.Text = "Set ROI Text";
            this.setTextToolStripMenuItem.Click += new System.EventHandler(this.setTextToolStripMenuItem_Click);
            // 
            // setIDToolStripMenuItem
            // 
            this.setIDToolStripMenuItem.Name = "setIDToolStripMenuItem";
            this.setIDToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
            this.setIDToolStripMenuItem.Text = "Set ROI ID";
            this.setIDToolStripMenuItem.Click += new System.EventHandler(this.setIDToolStripMenuItem_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.refreshToolStripMenuItem,
            this.scriptToolStripMenuItem,
            this.aboutToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(310, 24);
            this.menuStrip.TabIndex = 3;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.newToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.openToolStripMenuItem.Text = "Open Files";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // newToolStripMenuItem
            // 
            this.newToolStripMenuItem.Name = "newToolStripMenuItem";
            this.newToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.newToolStripMenuItem.Text = "New";
            this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
            // 
            // refreshToolStripMenuItem
            // 
            this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
            this.refreshToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.refreshToolStripMenuItem.Text = "Refresh";
            this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click_1);
            // 
            // scriptToolStripMenuItem
            // 
            this.scriptToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.scriptRunnerToolStripMenuItem,
            this.scriptRecorderToolStripMenuItem});
            this.scriptToolStripMenuItem.Name = "scriptToolStripMenuItem";
            this.scriptToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.scriptToolStripMenuItem.Text = "Script";
            // 
            // scriptRunnerToolStripMenuItem
            // 
            this.scriptRunnerToolStripMenuItem.Name = "scriptRunnerToolStripMenuItem";
            this.scriptRunnerToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.scriptRunnerToolStripMenuItem.Text = "Script Runner";
            this.scriptRunnerToolStripMenuItem.Click += new System.EventHandler(this.scriptRunnerToolStripMenuItem_Click_1);
            // 
            // scriptRecorderToolStripMenuItem
            // 
            this.scriptRecorderToolStripMenuItem.Name = "scriptRecorderToolStripMenuItem";
            this.scriptRecorderToolStripMenuItem.Size = new System.Drawing.Size(154, 22);
            this.scriptRecorderToolStripMenuItem.Text = "Script Recorder";
            this.scriptRecorderToolStripMenuItem.Click += new System.EventHandler(this.scriptRecorderToolStripMenuItem_Click);
            // 
            // openFilesDialog
            // 
            this.openFilesDialog.Multiselect = true;
            this.openFilesDialog.Title = "Open Images";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(310, 246);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.treeView);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "BioImage";
            this.Activated += new System.EventHandler(this.MainForm_Activated);
            this.contextMenuStrip.ResumeLayout(false);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeView;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFilesDialog;
        private System.Windows.Forms.ToolStripMenuItem refreshToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptRunnerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptRecorderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setTextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setIDToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
    }
}