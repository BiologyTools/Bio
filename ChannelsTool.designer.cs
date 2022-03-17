
namespace BioImage
{
    partial class ChannelsTool
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChannelsTool));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.minBox = new System.Windows.Forms.NumericUpDown();
            this.maxBox = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.channelsBox = new System.Windows.Forms.ComboBox();
            this.maxUintBox = new System.Windows.Forms.ComboBox();
            this.setMaxAllBut = new System.Windows.Forms.Button();
            this.setMinAllBut = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.minBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxBox)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.Black;
            this.label1.Location = new System.Drawing.Point(12, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Value Range";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.Black;
            this.label2.Location = new System.Drawing.Point(12, 79);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(23, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "min";
            // 
            // minBox
            // 
            this.minBox.BackColor = System.Drawing.SystemColors.Control;
            this.minBox.ForeColor = System.Drawing.Color.Black;
            this.minBox.Location = new System.Drawing.Point(41, 77);
            this.minBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.minBox.Name = "minBox";
            this.minBox.Size = new System.Drawing.Size(77, 20);
            this.minBox.TabIndex = 2;
            this.minBox.ValueChanged += new System.EventHandler(this.minBox_ValueChanged);
            // 
            // maxBox
            // 
            this.maxBox.BackColor = System.Drawing.SystemColors.Control;
            this.maxBox.ForeColor = System.Drawing.Color.Black;
            this.maxBox.Increment = new decimal(new int[] {
            100,
            0,
            0,
            0});
            this.maxBox.Location = new System.Drawing.Point(161, 77);
            this.maxBox.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.maxBox.Name = "maxBox";
            this.maxBox.Size = new System.Drawing.Size(77, 20);
            this.maxBox.TabIndex = 4;
            this.maxBox.Value = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.maxBox.ValueChanged += new System.EventHandler(this.maxBox_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.ForeColor = System.Drawing.Color.Black;
            this.label3.Location = new System.Drawing.Point(129, 79);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "max";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.ForeColor = System.Drawing.Color.Black;
            this.label4.Location = new System.Drawing.Point(12, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(46, 13);
            this.label4.TabIndex = 5;
            this.label4.Text = "Channel";
            // 
            // channelsBox
            // 
            this.channelsBox.BackColor = System.Drawing.SystemColors.Control;
            this.channelsBox.ForeColor = System.Drawing.Color.Black;
            this.channelsBox.FormattingEnabled = true;
            this.channelsBox.Location = new System.Drawing.Point(73, 19);
            this.channelsBox.Name = "channelsBox";
            this.channelsBox.Size = new System.Drawing.Size(165, 21);
            this.channelsBox.TabIndex = 6;
            this.channelsBox.SelectedIndexChanged += new System.EventHandler(this.channelsBox_SelectedIndexChanged);
            // 
            // maxUintBox
            // 
            this.maxUintBox.BackColor = System.Drawing.SystemColors.Control;
            this.maxUintBox.ForeColor = System.Drawing.Color.Black;
            this.maxUintBox.FormattingEnabled = true;
            this.maxUintBox.Items.AddRange(new object[] {
            "256",
            "1023",
            "4096",
            "16383",
            "65535"});
            this.maxUintBox.Location = new System.Drawing.Point(161, 52);
            this.maxUintBox.Name = "maxUintBox";
            this.maxUintBox.Size = new System.Drawing.Size(77, 21);
            this.maxUintBox.TabIndex = 7;
            this.maxUintBox.Text = "65535";
            this.maxUintBox.SelectedIndexChanged += new System.EventHandler(this.maxUintBox_SelectedIndexChanged);
            // 
            // setMaxAllBut
            // 
            this.setMaxAllBut.BackColor = System.Drawing.SystemColors.Control;
            this.setMaxAllBut.ForeColor = System.Drawing.Color.Black;
            this.setMaxAllBut.Location = new System.Drawing.Point(161, 103);
            this.setMaxAllBut.Name = "setMaxAllBut";
            this.setMaxAllBut.Size = new System.Drawing.Size(77, 23);
            this.setMaxAllBut.TabIndex = 8;
            this.setMaxAllBut.Text = "Set Max All";
            this.setMaxAllBut.UseVisualStyleBackColor = false;
            this.setMaxAllBut.Click += new System.EventHandler(this.setMaxAllBut_Click);
            // 
            // setMinAllBut
            // 
            this.setMinAllBut.BackColor = System.Drawing.SystemColors.Control;
            this.setMinAllBut.ForeColor = System.Drawing.Color.Black;
            this.setMinAllBut.Location = new System.Drawing.Point(41, 103);
            this.setMinAllBut.Name = "setMinAllBut";
            this.setMinAllBut.Size = new System.Drawing.Size(77, 23);
            this.setMinAllBut.TabIndex = 9;
            this.setMinAllBut.Text = "Set Min All";
            this.setMinAllBut.UseVisualStyleBackColor = false;
            this.setMinAllBut.Click += new System.EventHandler(this.setMinAllBut_Click);
            // 
            // ChannelsTool
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSteelBlue;
            this.ClientSize = new System.Drawing.Size(265, 138);
            this.Controls.Add(this.setMinAllBut);
            this.Controls.Add(this.setMaxAllBut);
            this.Controls.Add(this.maxUintBox);
            this.Controls.Add(this.channelsBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.maxBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.minBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ChannelsTool";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Channels Tool";
            this.TopMost = true;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.ChannelsTool_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.minBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maxBox)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown minBox;
        private System.Windows.Forms.NumericUpDown maxBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox channelsBox;
        private System.Windows.Forms.ComboBox maxUintBox;
        private System.Windows.Forms.Button setMaxAllBut;
        private System.Windows.Forms.Button setMinAllBut;
    }
}