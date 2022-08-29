namespace Bio
{
    partial class Elements
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Elements));
            this.view = new System.Windows.Forms.TreeView();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.startBut = new System.Windows.Forms.Button();
            this.stopBut = new System.Windows.Forms.Button();
            this.recordStatusLabel = new System.Windows.Forms.Label();
            this.playBut = new System.Windows.Forms.Button();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.addBut = new System.Windows.Forms.Button();
            this.removeBut = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // view
            // 
            this.view.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.view.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.view.Location = new System.Drawing.Point(2, 4);
            this.view.Name = "view";
            this.view.Size = new System.Drawing.Size(240, 342);
            this.view.TabIndex = 3;
            this.view.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.view_BeforeExpand);
            this.view.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.view_AfterSelect);
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // contextMenuStrip
            // 
            this.contextMenuStrip.Name = "contextMenuStrip";
            this.contextMenuStrip.Size = new System.Drawing.Size(61, 4);
            // 
            // startBut
            // 
            this.startBut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.startBut.ForeColor = System.Drawing.Color.White;
            this.startBut.Location = new System.Drawing.Point(86, 364);
            this.startBut.Name = "startBut";
            this.startBut.Size = new System.Drawing.Size(75, 23);
            this.startBut.TabIndex = 5;
            this.startBut.Text = "Start";
            this.startBut.UseVisualStyleBackColor = false;
            this.startBut.Click += new System.EventHandler(this.startBut_Click);
            // 
            // stopBut
            // 
            this.stopBut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.stopBut.ForeColor = System.Drawing.Color.White;
            this.stopBut.Location = new System.Drawing.Point(5, 364);
            this.stopBut.Name = "stopBut";
            this.stopBut.Size = new System.Drawing.Size(75, 23);
            this.stopBut.TabIndex = 6;
            this.stopBut.Text = "Stop";
            this.stopBut.UseVisualStyleBackColor = false;
            this.stopBut.Click += new System.EventHandler(this.stopBut_Click);
            // 
            // recordStatusLabel
            // 
            this.recordStatusLabel.AutoSize = true;
            this.recordStatusLabel.ForeColor = System.Drawing.Color.White;
            this.recordStatusLabel.Location = new System.Drawing.Point(5, 348);
            this.recordStatusLabel.Name = "recordStatusLabel";
            this.recordStatusLabel.Size = new System.Drawing.Size(102, 13);
            this.recordStatusLabel.TabIndex = 7;
            this.recordStatusLabel.Text = "Recording: Stopped";
            // 
            // playBut
            // 
            this.playBut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.playBut.ForeColor = System.Drawing.Color.White;
            this.playBut.Location = new System.Drawing.Point(167, 364);
            this.playBut.Name = "playBut";
            this.playBut.Size = new System.Drawing.Size(75, 23);
            this.playBut.TabIndex = 8;
            this.playBut.Text = "Play";
            this.playBut.UseVisualStyleBackColor = false;
            this.playBut.Click += new System.EventHandler(this.playBut_Click);
            // 
            // listBox1
            // 
            this.listBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(286, 4);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(197, 342);
            this.listBox1.TabIndex = 9;
            // 
            // addBut
            // 
            this.addBut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.addBut.ForeColor = System.Drawing.Color.White;
            this.addBut.Location = new System.Drawing.Point(249, 21);
            this.addBut.Name = "addBut";
            this.addBut.Size = new System.Drawing.Size(31, 23);
            this.addBut.TabIndex = 10;
            this.addBut.Text = ">";
            this.addBut.UseVisualStyleBackColor = false;
            // 
            // removeBut
            // 
            this.removeBut.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(49)))), ((int)(((byte)(91)))), ((int)(((byte)(138)))));
            this.removeBut.ForeColor = System.Drawing.Color.White;
            this.removeBut.Location = new System.Drawing.Point(249, 50);
            this.removeBut.Name = "removeBut";
            this.removeBut.Size = new System.Drawing.Size(31, 23);
            this.removeBut.TabIndex = 11;
            this.removeBut.Text = "<";
            this.removeBut.UseVisualStyleBackColor = false;
            // 
            // Elements
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.ClientSize = new System.Drawing.Size(491, 395);
            this.Controls.Add(this.removeBut);
            this.Controls.Add(this.addBut);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.playBut);
            this.Controls.Add(this.recordStatusLabel);
            this.Controls.Add(this.stopBut);
            this.Controls.Add(this.startBut);
            this.Controls.Add(this.view);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Elements";
            this.Text = "Recordings";
            this.TopMost = true;
            this.Activated += new System.EventHandler(this.Elements_Activated);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Elements_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView view;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip;
        private System.Windows.Forms.Button startBut;
        private System.Windows.Forms.Button stopBut;
        private System.Windows.Forms.Label recordStatusLabel;
        private System.Windows.Forms.Button playBut;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Button addBut;
        private System.Windows.Forms.Button removeBut;
    }
}