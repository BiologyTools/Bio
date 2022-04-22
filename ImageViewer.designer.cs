
namespace BioImage
{
    partial class ImageViewer
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImageViewer));
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveOMEToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sizeModeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rGBToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.filteredToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rawToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolboxToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rOIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rOIManagerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportROIsOfFolderOfImagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelsToolToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.autoThresholdToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelsToolToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptRunnerToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.scriptRecorderToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stackToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFilesDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveOMEFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.panel = new System.Windows.Forms.Panel();
            this.folderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
            this.saveCSVFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.openCSVFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.saveTiffFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.sizeModeToolStripMenuItem,
            this.toolboxToolStripMenuItem,
            this.rOIToolStripMenuItem,
            this.channelsToolToolStripMenuItem,
            this.scriptToolStripMenuItem,
            this.stackToolsToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(384, 24);
            this.menuStrip.TabIndex = 0;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveOMEToolStripMenuItem});
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
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.saveToolStripMenuItem.Text = "Save Tiff";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveOMEToolStripMenuItem
            // 
            this.saveOMEToolStripMenuItem.Name = "saveOMEToolStripMenuItem";
            this.saveOMEToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
            this.saveOMEToolStripMenuItem.Text = "Save OME";
            this.saveOMEToolStripMenuItem.Click += new System.EventHandler(this.saveOMEToolStripMenuItem_Click);
            // 
            // sizeModeToolStripMenuItem
            // 
            this.sizeModeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rGBToolStripMenuItem,
            this.filteredToolStripMenuItem,
            this.rawToolStripMenuItem});
            this.sizeModeToolStripMenuItem.Name = "sizeModeToolStripMenuItem";
            this.sizeModeToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.sizeModeToolStripMenuItem.Text = "View";
            // 
            // rGBToolStripMenuItem
            // 
            this.rGBToolStripMenuItem.CheckOnClick = true;
            this.rGBToolStripMenuItem.Name = "rGBToolStripMenuItem";
            this.rGBToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.rGBToolStripMenuItem.Text = "RGB";
            this.rGBToolStripMenuItem.Click += new System.EventHandler(this.rGBToolStripMenuItem_Click);
            // 
            // filteredToolStripMenuItem
            // 
            this.filteredToolStripMenuItem.CheckOnClick = true;
            this.filteredToolStripMenuItem.Name = "filteredToolStripMenuItem";
            this.filteredToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.filteredToolStripMenuItem.Text = "Filtered";
            this.filteredToolStripMenuItem.Click += new System.EventHandler(this.filteredToolStripMenuItem_Click);
            // 
            // rawToolStripMenuItem
            // 
            this.rawToolStripMenuItem.CheckOnClick = true;
            this.rawToolStripMenuItem.Name = "rawToolStripMenuItem";
            this.rawToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
            this.rawToolStripMenuItem.Text = "Raw";
            this.rawToolStripMenuItem.Click += new System.EventHandler(this.rawToolStripMenuItem_Click);
            // 
            // toolboxToolStripMenuItem
            // 
            this.toolboxToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setToolToolStripMenuItem});
            this.toolboxToolStripMenuItem.Name = "toolboxToolStripMenuItem";
            this.toolboxToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.toolboxToolStripMenuItem.Text = "Toolbox";
            this.toolboxToolStripMenuItem.Click += new System.EventHandler(this.toolboxToolStripMenuItem_Click);
            // 
            // setToolToolStripMenuItem
            // 
            this.setToolToolStripMenuItem.Name = "setToolToolStripMenuItem";
            this.setToolToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
            this.setToolToolStripMenuItem.Text = "Set Tool";
            this.setToolToolStripMenuItem.Click += new System.EventHandler(this.setToolToolStripMenuItem_Click);
            // 
            // rOIToolStripMenuItem
            // 
            this.rOIToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.rOIManagerToolStripMenuItem,
            this.exportCSVToolStripMenuItem,
            this.importCSVToolStripMenuItem,
            this.exportROIsOfFolderOfImagesToolStripMenuItem});
            this.rOIToolStripMenuItem.Name = "rOIToolStripMenuItem";
            this.rOIToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.rOIToolStripMenuItem.Text = "ROI";
            // 
            // rOIManagerToolStripMenuItem
            // 
            this.rOIManagerToolStripMenuItem.Name = "rOIManagerToolStripMenuItem";
            this.rOIManagerToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.rOIManagerToolStripMenuItem.Text = "ROI Manager";
            this.rOIManagerToolStripMenuItem.Click += new System.EventHandler(this.rOIManagerToolStripMenuItem_Click);
            // 
            // exportCSVToolStripMenuItem
            // 
            this.exportCSVToolStripMenuItem.Name = "exportCSVToolStripMenuItem";
            this.exportCSVToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.exportCSVToolStripMenuItem.Text = "Export ROI\'s to CSV";
            this.exportCSVToolStripMenuItem.Click += new System.EventHandler(this.exportCSVToolStripMenuItem_Click);
            // 
            // importCSVToolStripMenuItem
            // 
            this.importCSVToolStripMenuItem.Name = "importCSVToolStripMenuItem";
            this.importCSVToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.importCSVToolStripMenuItem.Text = "Import ROI\'s from CSV";
            this.importCSVToolStripMenuItem.Click += new System.EventHandler(this.importCSVToolStripMenuItem_Click);
            // 
            // exportROIsOfFolderOfImagesToolStripMenuItem
            // 
            this.exportROIsOfFolderOfImagesToolStripMenuItem.Name = "exportROIsOfFolderOfImagesToolStripMenuItem";
            this.exportROIsOfFolderOfImagesToolStripMenuItem.Size = new System.Drawing.Size(243, 22);
            this.exportROIsOfFolderOfImagesToolStripMenuItem.Text = "Export ROI\'s of Folder of Images";
            this.exportROIsOfFolderOfImagesToolStripMenuItem.Click += new System.EventHandler(this.exportROIsOfFolderOfImagesToolStripMenuItem_Click);
            // 
            // channelsToolToolStripMenuItem
            // 
            this.channelsToolToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.autoThresholdToolStripMenuItem,
            this.channelsToolToolStripMenuItem1});
            this.channelsToolToolStripMenuItem.Name = "channelsToolToolStripMenuItem";
            this.channelsToolToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.channelsToolToolStripMenuItem.Text = "Channels";
            // 
            // autoThresholdToolStripMenuItem
            // 
            this.autoThresholdToolStripMenuItem.Name = "autoThresholdToolStripMenuItem";
            this.autoThresholdToolStripMenuItem.Size = new System.Drawing.Size(172, 22);
            this.autoThresholdToolStripMenuItem.Text = "Auto Threshold All";
            this.autoThresholdToolStripMenuItem.Click += new System.EventHandler(this.autoThresholdToolStripMenuItem_Click);
            // 
            // channelsToolToolStripMenuItem1
            // 
            this.channelsToolToolStripMenuItem1.Name = "channelsToolToolStripMenuItem1";
            this.channelsToolToolStripMenuItem1.Size = new System.Drawing.Size(172, 22);
            this.channelsToolToolStripMenuItem1.Text = "Channels Tool";
            this.channelsToolToolStripMenuItem1.Click += new System.EventHandler(this.channelsToolToolStripMenuItem_Click);
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
            this.scriptRunnerToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.scriptRunnerToolStripMenuItem.Text = "Script Runner";
            this.scriptRunnerToolStripMenuItem.Click += new System.EventHandler(this.scriptRunnerToolStripMenuItem_Click);
            // 
            // scriptRecorderToolStripMenuItem
            // 
            this.scriptRecorderToolStripMenuItem.Name = "scriptRecorderToolStripMenuItem";
            this.scriptRecorderToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.scriptRecorderToolStripMenuItem.Text = "Script Recorder";
            this.scriptRecorderToolStripMenuItem.Click += new System.EventHandler(this.scriptRecorderToolStripMenuItem_Click);
            // 
            // stackToolsToolStripMenuItem
            // 
            this.stackToolsToolStripMenuItem.Name = "stackToolsToolStripMenuItem";
            this.stackToolsToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.stackToolsToolStripMenuItem.Text = "Stacks";
            this.stackToolsToolStripMenuItem.Click += new System.EventHandler(this.stackToolsToolStripMenuItem_Click);
            // 
            // openFilesDialog
            // 
            this.openFilesDialog.Multiselect = true;
            this.openFilesDialog.Title = "Open Images";
            // 
            // saveOMEFileDialog
            // 
            this.saveOMEFileDialog.DefaultExt = "ome.tif";
            this.saveOMEFileDialog.Filter = "\"OME TIFF Files (*.ome.tif)|*.ome.tif|All files (*.*)|*.*\"";
            this.saveOMEFileDialog.SupportMultiDottedExtensions = true;
            this.saveOMEFileDialog.Title = "Save Image";
            // 
            // panel
            // 
            this.panel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.panel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel.Location = new System.Drawing.Point(0, 24);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(384, 337);
            this.panel.TabIndex = 1;
            this.panel.Click += new System.EventHandler(this.panel_Click);
            // 
            // saveCSVFileDialog
            // 
            this.saveCSVFileDialog.DefaultExt = "csv";
            this.saveCSVFileDialog.Filter = "\"CSV Files (*.csv)|*.csv|All files (*.*)|*.*\"";
            this.saveCSVFileDialog.Title = "Save ROIs to CSV";
            // 
            // openCSVFileDialog
            // 
            this.openCSVFileDialog.DefaultExt = "csv";
            this.openCSVFileDialog.Filter = "\"CSV Files (*.csv)|*.csv|All files (*.*)|*.*\"";
            this.openCSVFileDialog.Title = "Import ROI from CSV";
            // 
            // saveTiffFileDialog
            // 
            this.saveTiffFileDialog.DefaultExt = "ome.tif";
            this.saveTiffFileDialog.Filter = "\"TIFF Files (*.tif)|*.tif\"";
            this.saveTiffFileDialog.SupportMultiDottedExtensions = true;
            this.saveTiffFileDialog.Title = "Save Image";
            // 
            // ImageViewer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(69)))), ((int)(((byte)(77)))), ((int)(((byte)(98)))));
            this.ClientSize = new System.Drawing.Size(384, 361);
            this.Controls.Add(this.panel);
            this.Controls.Add(this.menuStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip;
            this.MinimumSize = new System.Drawing.Size(400, 400);
            this.Name = "ImageViewer";
            this.Text = "BioImage";
            this.Activated += new System.EventHandler(this.ImageViewer_Activated);
            this.Deactivate += new System.EventHandler(this.ImageViewer_Deactivate);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ImageViewer_FormClosing);
            this.Click += new System.EventHandler(this.ImageViewer_Click);
            this.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.ImageViewer_PreviewKeyDown);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog openFilesDialog;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveOMEFileDialog;
        private System.Windows.Forms.Panel panel;
        private System.Windows.Forms.ToolStripMenuItem toolboxToolStripMenuItem;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog;
        private System.Windows.Forms.ToolStripMenuItem rOIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportCSVToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveCSVFileDialog;
        private System.Windows.Forms.OpenFileDialog openCSVFileDialog;
        private System.Windows.Forms.ToolStripMenuItem importCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportROIsOfFolderOfImagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rOIManagerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem channelsToolToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sizeModeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rGBToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem filteredToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rawToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem autoThresholdToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem channelsToolToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem saveOMEToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveTiffFileDialog;
        private System.Windows.Forms.ToolStripMenuItem stackToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptRunnerToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem scriptRecorderToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setToolToolStripMenuItem;
    }
}