
namespace BioImage
{
    partial class ImageView
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
            this.timePlayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playTimeToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.stopTimeToolStripMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.playSpeedToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.setValueRangeToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.loopTimeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteROIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.renameSelectionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyROIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteROIToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyViewToClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showControlToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showStatusToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zPlayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playSpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setValueRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loopZToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.channelBoxB = new System.Windows.Forms.ComboBox();
            this.channelBoxG = new System.Windows.Forms.ComboBox();
            this.labelRGB = new System.Windows.Forms.Label();
            this.channelBoxR = new System.Windows.Forms.ComboBox();
            this.rgbBoxsPanel = new System.Windows.Forms.Panel();
            this.statusLabel = new System.Windows.Forms.Label();
            this.tLabel = new System.Windows.Forms.Label();
            this.timeBar = new System.Windows.Forms.TrackBar();
            this.controlsMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.hideControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cPlayMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.playCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stopCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CPlaySpeedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setCValueRangeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loopCToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.zBar = new System.Windows.Forms.TrackBar();
            this.zLabel = new System.Windows.Forms.Label();
            this.cBar = new System.Windows.Forms.TrackBar();
            this.cLabel = new System.Windows.Forms.Label();
            this.timelineTimer = new System.Windows.Forms.Timer(this.components);
            this.zTimer = new System.Windows.Forms.Timer(this.components);
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cTimer = new System.Windows.Forms.Timer(this.components);
            this.trackBarPanel = new System.Windows.Forms.Panel();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.ticksLabel = new System.Windows.Forms.Label();
            this.showControlsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.HideStatusMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.overlayPictureBox = new System.Windows.Forms.PictureBox();
            this.panel = new System.Windows.Forms.Panel();
            this.timePlayMenuStrip.SuspendLayout();
            this.contextMenuStrip.SuspendLayout();
            this.zPlayMenuStrip.SuspendLayout();
            this.rgbBoxsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).BeginInit();
            this.controlsMenuStrip.SuspendLayout();
            this.cPlayMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.zBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cBar)).BeginInit();
            this.trackBarPanel.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.statusMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlayPictureBox)).BeginInit();
            this.panel.SuspendLayout();
            this.SuspendLayout();
            // 
            // timePlayMenuStrip
            // 
            this.timePlayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playTimeToolStripMenu,
            this.stopTimeToolStripMenu,
            this.playSpeedToolStripMenuItem1,
            this.setValueRangeToolStripMenuItem1,
            this.loopTimeToolStripMenuItem});
            this.timePlayMenuStrip.Name = "zPlayMenuStrip";
            this.timePlayMenuStrip.Size = new System.Drawing.Size(158, 114);
            // 
            // playTimeToolStripMenu
            // 
            this.playTimeToolStripMenu.Name = "playTimeToolStripMenu";
            this.playTimeToolStripMenu.Size = new System.Drawing.Size(157, 22);
            this.playTimeToolStripMenu.Text = "Play";
            this.playTimeToolStripMenu.Click += new System.EventHandler(this.playTimeToolStripMenu_Click);
            // 
            // stopTimeToolStripMenu
            // 
            this.stopTimeToolStripMenu.Checked = true;
            this.stopTimeToolStripMenu.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopTimeToolStripMenu.Name = "stopTimeToolStripMenu";
            this.stopTimeToolStripMenu.Size = new System.Drawing.Size(157, 22);
            this.stopTimeToolStripMenu.Text = "Stop";
            this.stopTimeToolStripMenu.Click += new System.EventHandler(this.stopTimeToolStripMenu_Click);
            // 
            // playSpeedToolStripMenuItem1
            // 
            this.playSpeedToolStripMenuItem1.Name = "playSpeedToolStripMenuItem1";
            this.playSpeedToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.playSpeedToolStripMenuItem1.Text = "Play Speed";
            this.playSpeedToolStripMenuItem1.Click += new System.EventHandler(this.playSpeedToolStripMenuItem1_Click);
            // 
            // setValueRangeToolStripMenuItem1
            // 
            this.setValueRangeToolStripMenuItem1.Name = "setValueRangeToolStripMenuItem1";
            this.setValueRangeToolStripMenuItem1.Size = new System.Drawing.Size(157, 22);
            this.setValueRangeToolStripMenuItem1.Text = "Set Value Range";
            this.setValueRangeToolStripMenuItem1.Click += new System.EventHandler(this.setValueRangeToolStripMenuItem1_Click);
            // 
            // loopTimeToolStripMenuItem
            // 
            this.loopTimeToolStripMenuItem.Checked = true;
            this.loopTimeToolStripMenuItem.CheckOnClick = true;
            this.loopTimeToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopTimeToolStripMenuItem.Name = "loopTimeToolStripMenuItem";
            this.loopTimeToolStripMenuItem.ShowShortcutKeys = false;
            this.loopTimeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loopTimeToolStripMenuItem.Text = "Loop";
            this.loopTimeToolStripMenuItem.Click += new System.EventHandler(this.loopTimeToolStripMenuItem_Click);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteROIToolStripMenuItem,
            this.renameSelectionToolStripMenuItem,
            this.copyROIToolStripMenuItem,
            this.pasteROIToolStripMenuItem,
            this.copyViewToClipboardToolStripMenuItem,
            this.showControlToolStripMenuItem,
            this.showStatusToolStripMenuItem});
            this.contextMenuStrip.Name = "contextMenuStrip1";
            this.contextMenuStrip.Size = new System.Drawing.Size(200, 180);
            // 
            // deleteROIToolStripMenuItem
            // 
            this.deleteROIToolStripMenuItem.Name = "deleteROIToolStripMenuItem";
            this.deleteROIToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.deleteROIToolStripMenuItem.Text = "Delete ROI Selection";
            this.deleteROIToolStripMenuItem.Click += new System.EventHandler(this.deleteROIToolStripMenuItem_Click);
            // 
            // renameSelectionToolStripMenuItem
            // 
            this.renameSelectionToolStripMenuItem.Name = "renameSelectionToolStripMenuItem";
            this.renameSelectionToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.renameSelectionToolStripMenuItem.Text = "Set ROI Text";
            this.renameSelectionToolStripMenuItem.Click += new System.EventHandler(this.setTextSelectionToolStripMenuItem_Click);
            // 
            // copyROIToolStripMenuItem
            // 
            this.copyROIToolStripMenuItem.Name = "copyROIToolStripMenuItem";
            this.copyROIToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.copyROIToolStripMenuItem.Text = "Copy ROI";
            this.copyROIToolStripMenuItem.Click += new System.EventHandler(this.copyROIToolStripMenuItem_Click);
            // 
            // pasteROIToolStripMenuItem
            // 
            this.pasteROIToolStripMenuItem.Name = "pasteROIToolStripMenuItem";
            this.pasteROIToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.pasteROIToolStripMenuItem.Text = "Paste ROI";
            this.pasteROIToolStripMenuItem.Click += new System.EventHandler(this.pasteROIToolStripMenuItem_Click);
            // 
            // copyViewToClipboardToolStripMenuItem
            // 
            this.copyViewToClipboardToolStripMenuItem.Name = "copyViewToClipboardToolStripMenuItem";
            this.copyViewToClipboardToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.copyViewToClipboardToolStripMenuItem.Text = "Copy View to Clipboard";
            this.copyViewToClipboardToolStripMenuItem.Click += new System.EventHandler(this.copyViewToClipboardToolStripMenuItem_Click);
            // 
            // showControlToolStripMenuItem
            // 
            this.showControlToolStripMenuItem.Name = "showControlToolStripMenuItem";
            this.showControlToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.showControlToolStripMenuItem.Text = "Hide Controls";
            this.showControlToolStripMenuItem.Click += new System.EventHandler(this.showControlToolStripMenuItem_Click);
            // 
            // showStatusToolStripMenuItem
            // 
            this.showStatusToolStripMenuItem.Name = "showStatusToolStripMenuItem";
            this.showStatusToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.showStatusToolStripMenuItem.Text = "Hide Status";
            this.showStatusToolStripMenuItem.Click += new System.EventHandler(this.showStatusToolStripMenuItem_Click);
            // 
            // zPlayMenuStrip
            // 
            this.zPlayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playZToolStripMenuItem,
            this.stopZToolStripMenuItem,
            this.playSpeedToolStripMenuItem,
            this.setValueRangeToolStripMenuItem,
            this.loopZToolStripMenuItem});
            this.zPlayMenuStrip.Name = "zPlayMenuStrip";
            this.zPlayMenuStrip.Size = new System.Drawing.Size(158, 114);
            // 
            // playZToolStripMenuItem
            // 
            this.playZToolStripMenuItem.Name = "playZToolStripMenuItem";
            this.playZToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.playZToolStripMenuItem.Text = "Play";
            this.playZToolStripMenuItem.Click += new System.EventHandler(this.playZToolStripMenuItem_Click);
            // 
            // stopZToolStripMenuItem
            // 
            this.stopZToolStripMenuItem.Checked = true;
            this.stopZToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopZToolStripMenuItem.Name = "stopZToolStripMenuItem";
            this.stopZToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.stopZToolStripMenuItem.Text = "Stop";
            this.stopZToolStripMenuItem.Click += new System.EventHandler(this.stopZToolStripMenuItem_Click);
            // 
            // playSpeedToolStripMenuItem
            // 
            this.playSpeedToolStripMenuItem.Name = "playSpeedToolStripMenuItem";
            this.playSpeedToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.playSpeedToolStripMenuItem.Text = "Play Speed";
            this.playSpeedToolStripMenuItem.Click += new System.EventHandler(this.playSpeedToolStripMenuItem_Click);
            // 
            // setValueRangeToolStripMenuItem
            // 
            this.setValueRangeToolStripMenuItem.Name = "setValueRangeToolStripMenuItem";
            this.setValueRangeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.setValueRangeToolStripMenuItem.Text = "Set Value Range";
            this.setValueRangeToolStripMenuItem.Click += new System.EventHandler(this.setValueRangeToolStripMenuItem_Click);
            // 
            // loopZToolStripMenuItem
            // 
            this.loopZToolStripMenuItem.Checked = true;
            this.loopZToolStripMenuItem.CheckOnClick = true;
            this.loopZToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopZToolStripMenuItem.Name = "loopZToolStripMenuItem";
            this.loopZToolStripMenuItem.ShowShortcutKeys = false;
            this.loopZToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loopZToolStripMenuItem.Text = "Loop";
            this.loopZToolStripMenuItem.Click += new System.EventHandler(this.loopZToolStripMenuItem_Click);
            // 
            // channelBoxB
            // 
            this.channelBoxB.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(73)))), ((int)(((byte)(112)))));
            this.channelBoxB.DropDownWidth = 150;
            this.channelBoxB.ForeColor = System.Drawing.Color.White;
            this.channelBoxB.FormattingEnabled = true;
            this.channelBoxB.Location = new System.Drawing.Point(292, 2);
            this.channelBoxB.Name = "channelBoxB";
            this.channelBoxB.Size = new System.Drawing.Size(120, 21);
            this.channelBoxB.TabIndex = 8;
            this.channelBoxB.SelectedIndexChanged += new System.EventHandler(this.channelBoxB_SelectedIndexChanged);
            // 
            // channelBoxG
            // 
            this.channelBoxG.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(73)))), ((int)(((byte)(112)))));
            this.channelBoxG.DropDownWidth = 150;
            this.channelBoxG.ForeColor = System.Drawing.Color.White;
            this.channelBoxG.FormattingEnabled = true;
            this.channelBoxG.Location = new System.Drawing.Point(166, 2);
            this.channelBoxG.Name = "channelBoxG";
            this.channelBoxG.Size = new System.Drawing.Size(120, 21);
            this.channelBoxG.TabIndex = 6;
            this.channelBoxG.SelectedIndexChanged += new System.EventHandler(this.channelBoxG_SelectedIndexChanged);
            // 
            // labelRGB
            // 
            this.labelRGB.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.labelRGB.AutoSize = true;
            this.labelRGB.ForeColor = System.Drawing.Color.White;
            this.labelRGB.Location = new System.Drawing.Point(4, 6);
            this.labelRGB.Name = "labelRGB";
            this.labelRGB.Size = new System.Drawing.Size(30, 13);
            this.labelRGB.TabIndex = 5;
            this.labelRGB.Text = "RGB";
            // 
            // channelBoxR
            // 
            this.channelBoxR.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(73)))), ((int)(((byte)(112)))));
            this.channelBoxR.DropDownWidth = 150;
            this.channelBoxR.ForeColor = System.Drawing.Color.White;
            this.channelBoxR.FormattingEnabled = true;
            this.channelBoxR.Location = new System.Drawing.Point(40, 2);
            this.channelBoxR.Name = "channelBoxR";
            this.channelBoxR.Size = new System.Drawing.Size(120, 21);
            this.channelBoxR.TabIndex = 4;
            this.channelBoxR.SelectedIndexChanged += new System.EventHandler(this.channelBoxR_SelectedIndexChanged);
            // 
            // rgbBoxsPanel
            // 
            this.rgbBoxsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rgbBoxsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.rgbBoxsPanel.Controls.Add(this.channelBoxR);
            this.rgbBoxsPanel.Controls.Add(this.channelBoxB);
            this.rgbBoxsPanel.Controls.Add(this.labelRGB);
            this.rgbBoxsPanel.Controls.Add(this.channelBoxG);
            this.rgbBoxsPanel.ForeColor = System.Drawing.Color.White;
            this.rgbBoxsPanel.Location = new System.Drawing.Point(0, 50);
            this.rgbBoxsPanel.Name = "rgbBoxsPanel";
            this.rgbBoxsPanel.Size = new System.Drawing.Size(428, 25);
            this.rgbBoxsPanel.TabIndex = 13;
            // 
            // statusLabel
            // 
            this.statusLabel.AutoSize = true;
            this.statusLabel.ForeColor = System.Drawing.Color.White;
            this.statusLabel.Location = new System.Drawing.Point(8, 6);
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 13);
            this.statusLabel.TabIndex = 3;
            // 
            // tLabel
            // 
            this.tLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tLabel.AutoSize = true;
            this.tLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.tLabel.ForeColor = System.Drawing.Color.White;
            this.tLabel.Location = new System.Drawing.Point(4, 31);
            this.tLabel.Name = "tLabel";
            this.tLabel.Size = new System.Drawing.Size(14, 13);
            this.tLabel.TabIndex = 13;
            this.tLabel.Text = "T";
            // 
            // timeBar
            // 
            this.timeBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.timeBar.AutoSize = false;
            this.timeBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.timeBar.ContextMenuStrip = this.timePlayMenuStrip;
            this.timeBar.LargeChange = 1;
            this.timeBar.Location = new System.Drawing.Point(15, 25);
            this.timeBar.Margin = new System.Windows.Forms.Padding(0);
            this.timeBar.Name = "timeBar";
            this.timeBar.Size = new System.Drawing.Size(413, 25);
            this.timeBar.TabIndex = 16;
            this.timeBar.ValueChanged += new System.EventHandler(this.timeBar_ValueChanged);
            // 
            // controlsMenuStrip
            // 
            this.controlsMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.hideControlsToolStripMenuItem});
            this.controlsMenuStrip.Name = "controlsMenuStrip";
            this.controlsMenuStrip.Size = new System.Drawing.Size(148, 26);
            // 
            // hideControlsToolStripMenuItem
            // 
            this.hideControlsToolStripMenuItem.Name = "hideControlsToolStripMenuItem";
            this.hideControlsToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.hideControlsToolStripMenuItem.Text = "Hide Controls";
            this.hideControlsToolStripMenuItem.Click += new System.EventHandler(this.hideControlsToolStripMenuItem_Click);
            // 
            // cPlayMenuStrip
            // 
            this.cPlayMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.playCToolStripMenuItem,
            this.stopCToolStripMenuItem,
            this.CPlaySpeedToolStripMenuItem,
            this.setCValueRangeToolStripMenuItem,
            this.loopCToolStripMenuItem});
            this.cPlayMenuStrip.Name = "zPlayMenuStrip";
            this.cPlayMenuStrip.Size = new System.Drawing.Size(158, 114);
            // 
            // playCToolStripMenuItem
            // 
            this.playCToolStripMenuItem.Name = "playCToolStripMenuItem";
            this.playCToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.playCToolStripMenuItem.Text = "Play";
            this.playCToolStripMenuItem.Click += new System.EventHandler(this.playCToolStripMenuItem_Click);
            // 
            // stopCToolStripMenuItem
            // 
            this.stopCToolStripMenuItem.Checked = true;
            this.stopCToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.stopCToolStripMenuItem.Name = "stopCToolStripMenuItem";
            this.stopCToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.stopCToolStripMenuItem.Text = "Stop";
            this.stopCToolStripMenuItem.Click += new System.EventHandler(this.stopCToolStripMenuItem_Click);
            // 
            // CPlaySpeedToolStripMenuItem
            // 
            this.CPlaySpeedToolStripMenuItem.Name = "CPlaySpeedToolStripMenuItem";
            this.CPlaySpeedToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.CPlaySpeedToolStripMenuItem.Text = "Play Speed";
            this.CPlaySpeedToolStripMenuItem.Click += new System.EventHandler(this.CPlaySpeedToolStripMenuItem_Click);
            // 
            // setCValueRangeToolStripMenuItem
            // 
            this.setCValueRangeToolStripMenuItem.Name = "setCValueRangeToolStripMenuItem";
            this.setCValueRangeToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.setCValueRangeToolStripMenuItem.Text = "Set Value Range";
            this.setCValueRangeToolStripMenuItem.Click += new System.EventHandler(this.setCValueRangeToolStripMenuItem_Click);
            // 
            // loopCToolStripMenuItem
            // 
            this.loopCToolStripMenuItem.Checked = true;
            this.loopCToolStripMenuItem.CheckOnClick = true;
            this.loopCToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.loopCToolStripMenuItem.Name = "loopCToolStripMenuItem";
            this.loopCToolStripMenuItem.ShowShortcutKeys = false;
            this.loopCToolStripMenuItem.Size = new System.Drawing.Size(157, 22);
            this.loopCToolStripMenuItem.Text = "Loop";
            this.loopCToolStripMenuItem.Click += new System.EventHandler(this.loopCToolStripMenuItem_Click);
            // 
            // zBar
            // 
            this.zBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zBar.AutoSize = false;
            this.zBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.zBar.ContextMenuStrip = this.zPlayMenuStrip;
            this.zBar.LargeChange = 1;
            this.zBar.Location = new System.Drawing.Point(15, 0);
            this.zBar.Margin = new System.Windows.Forms.Padding(0);
            this.zBar.Name = "zBar";
            this.zBar.Size = new System.Drawing.Size(413, 25);
            this.zBar.TabIndex = 12;
            this.zBar.ValueChanged += new System.EventHandler(this.zBar_ValueChanged);
            // 
            // zLabel
            // 
            this.zLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.zLabel.AutoSize = true;
            this.zLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.zLabel.ForeColor = System.Drawing.Color.White;
            this.zLabel.Location = new System.Drawing.Point(4, 4);
            this.zLabel.Name = "zLabel";
            this.zLabel.Size = new System.Drawing.Size(14, 13);
            this.zLabel.TabIndex = 9;
            this.zLabel.Text = "Z";
            // 
            // cBar
            // 
            this.cBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cBar.AutoSize = false;
            this.cBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.cBar.ContextMenuStrip = this.cPlayMenuStrip;
            this.cBar.LargeChange = 1;
            this.cBar.Location = new System.Drawing.Point(15, 50);
            this.cBar.Margin = new System.Windows.Forms.Padding(0);
            this.cBar.Name = "cBar";
            this.cBar.Size = new System.Drawing.Size(413, 25);
            this.cBar.TabIndex = 15;
            this.cBar.ValueChanged += new System.EventHandler(this.cBar_ValueChanged);
            // 
            // cLabel
            // 
            this.cLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cLabel.AutoSize = true;
            this.cLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.cLabel.ForeColor = System.Drawing.Color.White;
            this.cLabel.Location = new System.Drawing.Point(4, 55);
            this.cLabel.Name = "cLabel";
            this.cLabel.Size = new System.Drawing.Size(14, 13);
            this.cLabel.TabIndex = 15;
            this.cLabel.Text = "C";
            // 
            // timelineTimer
            // 
            this.timelineTimer.Interval = 32;
            this.timelineTimer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // zTimer
            // 
            this.zTimer.Interval = 32;
            this.zTimer.Tick += new System.EventHandler(this.zTimer_Tick);
            // 
            // cTimer
            // 
            this.cTimer.Interval = 32;
            this.cTimer.Tick += new System.EventHandler(this.cTimer_Tick);
            // 
            // trackBarPanel
            // 
            this.trackBarPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.trackBarPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.trackBarPanel.ContextMenuStrip = this.controlsMenuStrip;
            this.trackBarPanel.Controls.Add(this.cBar);
            this.trackBarPanel.Controls.Add(this.zBar);
            this.trackBarPanel.Controls.Add(this.timeBar);
            this.trackBarPanel.Controls.Add(this.tLabel);
            this.trackBarPanel.Controls.Add(this.zLabel);
            this.trackBarPanel.Controls.Add(this.cLabel);
            this.trackBarPanel.Controls.Add(this.rgbBoxsPanel);
            this.trackBarPanel.Location = new System.Drawing.Point(0, 293);
            this.trackBarPanel.Name = "trackBarPanel";
            this.trackBarPanel.Size = new System.Drawing.Size(428, 75);
            this.trackBarPanel.TabIndex = 17;
            // 
            // statusPanel
            // 
            this.statusPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.statusPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.statusPanel.ContextMenuStrip = this.contextMenuStrip;
            this.statusPanel.Controls.Add(this.ticksLabel);
            this.statusPanel.Controls.Add(this.statusLabel);
            this.statusPanel.Location = new System.Drawing.Point(0, 0);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(428, 25);
            this.statusPanel.TabIndex = 18;
            // 
            // ticksLabel
            // 
            this.ticksLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ticksLabel.AutoSize = true;
            this.ticksLabel.ForeColor = System.Drawing.Color.White;
            this.ticksLabel.Location = new System.Drawing.Point(335, 6);
            this.ticksLabel.Name = "ticksLabel";
            this.ticksLabel.Size = new System.Drawing.Size(0, 13);
            this.ticksLabel.TabIndex = 4;
            // 
            // showControlsToolStripMenuItem
            // 
            this.showControlsToolStripMenuItem.Name = "showControlsToolStripMenuItem";
            this.showControlsToolStripMenuItem.Size = new System.Drawing.Size(199, 22);
            this.showControlsToolStripMenuItem.Text = "Hide Controls";
            this.showControlsToolStripMenuItem.Click += new System.EventHandler(this.showControlsToolStripMenuItem_Click);
            // 
            // statusMenuStrip
            // 
            this.statusMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.HideStatusMenuItem});
            this.statusMenuStrip.Name = "controlsMenuStrip";
            this.statusMenuStrip.Size = new System.Drawing.Size(135, 26);
            // 
            // HideStatusMenuItem
            // 
            this.HideStatusMenuItem.Name = "HideStatusMenuItem";
            this.HideStatusMenuItem.Size = new System.Drawing.Size(134, 22);
            this.HideStatusMenuItem.Text = "Hide Status";
            this.HideStatusMenuItem.Click += new System.EventHandler(this.HideStatusMenuItem_Click);
            // 
            // pictureBox
            // 
            this.pictureBox.BackColor = System.Drawing.Color.Transparent;
            this.pictureBox.ContextMenuStrip = this.contextMenuStrip;
            this.pictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pictureBox.Location = new System.Drawing.Point(0, 0);
            this.pictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(425, 269);
            this.pictureBox.TabIndex = 20;
            this.pictureBox.TabStop = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            // 
            // overlayPictureBox
            // 
            this.overlayPictureBox.BackColor = System.Drawing.Color.Transparent;
            this.overlayPictureBox.ContextMenuStrip = this.contextMenuStrip;
            this.overlayPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.overlayPictureBox.Location = new System.Drawing.Point(0, 0);
            this.overlayPictureBox.Margin = new System.Windows.Forms.Padding(0);
            this.overlayPictureBox.Name = "overlayPictureBox";
            this.overlayPictureBox.Size = new System.Drawing.Size(425, 269);
            this.overlayPictureBox.TabIndex = 19;
            this.overlayPictureBox.TabStop = false;
            this.overlayPictureBox.Click += new System.EventHandler(this.pictureBox_Click);
            this.overlayPictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.overlayPictureBox_Paint);
            this.overlayPictureBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDoubleClick);
            this.overlayPictureBox.MouseDown += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseDown);
            this.overlayPictureBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.rgbPictureBox_MouseMove);
            this.overlayPictureBox.MouseUp += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseUp);
            // 
            // panel
            // 
            this.panel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.panel.Controls.Add(this.overlayPictureBox);
            this.panel.Controls.Add(this.pictureBox);
            this.panel.Location = new System.Drawing.Point(0, 25);
            this.panel.Name = "panel";
            this.panel.Size = new System.Drawing.Size(425, 269);
            this.panel.TabIndex = 21;
            // 
            // ImageView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.Controls.Add(this.trackBarPanel);
            this.Controls.Add(this.statusPanel);
            this.Controls.Add(this.panel);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(100, 100);
            this.Name = "ImageView";
            this.Size = new System.Drawing.Size(428, 368);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ImageView_KeyDown);
            this.timePlayMenuStrip.ResumeLayout(false);
            this.contextMenuStrip.ResumeLayout(false);
            this.zPlayMenuStrip.ResumeLayout(false);
            this.rgbBoxsPanel.ResumeLayout(false);
            this.rgbBoxsPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeBar)).EndInit();
            this.controlsMenuStrip.ResumeLayout(false);
            this.cPlayMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.zBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cBar)).EndInit();
            this.trackBarPanel.ResumeLayout(false);
            this.trackBarPanel.PerformLayout();
            this.statusPanel.ResumeLayout(false);
            this.statusPanel.PerformLayout();
            this.statusMenuStrip.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.overlayPictureBox)).EndInit();
            this.panel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.ComboBox channelBoxR;
        private System.Windows.Forms.ComboBox channelBoxB;
        private System.Windows.Forms.ComboBox channelBoxG;
        private System.Windows.Forms.Label labelRGB;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.Label statusLabel;
        private System.Windows.Forms.Panel rgbBoxsPanel;
        private System.Windows.Forms.Timer timelineTimer;
        private System.Windows.Forms.ContextMenuStrip zPlayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem playZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopZToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip timePlayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem playTimeToolStripMenu;
        private System.Windows.Forms.ToolStripMenuItem stopTimeToolStripMenu;
        private System.Windows.Forms.Timer zTimer;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.ToolStripMenuItem playSpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playSpeedToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem setValueRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setValueRangeToolStripMenuItem1;
        private System.Windows.Forms.Timer cTimer;
        private System.Windows.Forms.ContextMenuStrip cPlayMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem playCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem stopCToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem CPlaySpeedToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setCValueRangeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loopZToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loopTimeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loopCToolStripMenuItem;
        private System.Windows.Forms.TrackBar cBar;
        private System.Windows.Forms.TrackBar zBar;
        private System.Windows.Forms.Label cLabel;
        private System.Windows.Forms.Label zLabel;
        private System.Windows.Forms.Label tLabel;
        private System.Windows.Forms.TrackBar timeBar;
        private System.Windows.Forms.Panel trackBarPanel;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.Label ticksLabel;
        private System.Windows.Forms.ToolStripMenuItem deleteROIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem renameSelectionToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyViewToClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyROIToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteROIToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip controlsMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem hideControlsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showControlToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip statusMenuStrip;
        private System.Windows.Forms.ToolStripMenuItem HideStatusMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showStatusToolStripMenuItem;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.PictureBox overlayPictureBox;
        private System.Windows.Forms.Panel panel;
    }
}
