namespace BioImage
{
    partial class ScriptEditor
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ScriptEditor));
            this.runButton = new System.Windows.Forms.Button();
            this.outputBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.scriptLoadBut = new System.Windows.Forms.Button();
            this.scriptLabel = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.tabControl = new System.Windows.Forms.TabControl();
            this.outputTab = new System.Windows.Forms.TabPage();
            this.error = new System.Windows.Forms.TabPage();
            this.errorBox = new System.Windows.Forms.TextBox();
            this.saveButton = new System.Windows.Forms.Button();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.textBox = new System.Windows.Forms.RichTextBox();
            this.tabControl.SuspendLayout();
            this.outputTab.SuspendLayout();
            this.error.SuspendLayout();
            this.SuspendLayout();
            // 
            // runButton
            // 
            this.runButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.runButton.Location = new System.Drawing.Point(521, 335);
            this.runButton.Name = "runButton";
            this.runButton.Size = new System.Drawing.Size(75, 23);
            this.runButton.TabIndex = 1;
            this.runButton.TabStop = false;
            this.runButton.Text = "Run";
            this.runButton.UseVisualStyleBackColor = true;
            this.runButton.Click += new System.EventHandler(this.runButton_Click);
            // 
            // outputBox
            // 
            this.outputBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.outputBox.Location = new System.Drawing.Point(3, 3);
            this.outputBox.Multiline = true;
            this.outputBox.Name = "outputBox";
            this.outputBox.Size = new System.Drawing.Size(567, 105);
            this.outputBox.TabIndex = 2;
            this.outputBox.TabStop = false;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(12, 340);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(90, 13);
            this.label1.TabIndex = 3;
            this.label1.Text = "Output and Errors";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(12, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Script";
            // 
            // scriptLoadBut
            // 
            this.scriptLoadBut.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.scriptLoadBut.Location = new System.Drawing.Point(439, 4);
            this.scriptLoadBut.Name = "scriptLoadBut";
            this.scriptLoadBut.Size = new System.Drawing.Size(75, 23);
            this.scriptLoadBut.TabIndex = 5;
            this.scriptLoadBut.TabStop = false;
            this.scriptLoadBut.Text = "Load Script";
            this.scriptLoadBut.UseVisualStyleBackColor = true;
            this.scriptLoadBut.Click += new System.EventHandler(this.scriptBut_Click);
            // 
            // scriptLabel
            // 
            this.scriptLabel.AutoSize = true;
            this.scriptLabel.ForeColor = System.Drawing.Color.White;
            this.scriptLabel.Location = new System.Drawing.Point(52, 9);
            this.scriptLabel.Name = "scriptLabel";
            this.scriptLabel.Size = new System.Drawing.Size(0, 13);
            this.scriptLabel.TabIndex = 6;
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            this.openFileDialog.Filter = "\"C# Files (*.cs)|*.cs|All files (*.*)|*.*\"";
            // 
            // timer
            // 
            this.timer.Interval = 1000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // tabControl
            // 
            this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl.Controls.Add(this.outputTab);
            this.tabControl.Controls.Add(this.error);
            this.tabControl.Location = new System.Drawing.Point(15, 357);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(581, 137);
            this.tabControl.TabIndex = 7;
            this.tabControl.TabStop = false;
            // 
            // outputTab
            // 
            this.outputTab.Controls.Add(this.outputBox);
            this.outputTab.Location = new System.Drawing.Point(4, 22);
            this.outputTab.Name = "outputTab";
            this.outputTab.Padding = new System.Windows.Forms.Padding(3);
            this.outputTab.Size = new System.Drawing.Size(573, 111);
            this.outputTab.TabIndex = 1;
            this.outputTab.Text = "Output";
            this.outputTab.UseVisualStyleBackColor = true;
            // 
            // error
            // 
            this.error.Controls.Add(this.errorBox);
            this.error.Location = new System.Drawing.Point(4, 22);
            this.error.Name = "error";
            this.error.Padding = new System.Windows.Forms.Padding(3);
            this.error.Size = new System.Drawing.Size(573, 111);
            this.error.TabIndex = 0;
            this.error.Text = "Error";
            this.error.UseVisualStyleBackColor = true;
            // 
            // errorBox
            // 
            this.errorBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.errorBox.Location = new System.Drawing.Point(3, 3);
            this.errorBox.Multiline = true;
            this.errorBox.Name = "errorBox";
            this.errorBox.Size = new System.Drawing.Size(567, 105);
            this.errorBox.TabIndex = 3;
            this.errorBox.TabStop = false;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.saveButton.Location = new System.Drawing.Point(521, 4);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(75, 23);
            this.saveButton.TabIndex = 8;
            this.saveButton.TabStop = false;
            this.saveButton.Text = "Save Script";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.FileName = "\"C# Files (*.cs)|*.cs|All files (*.*)|*.*\"";
            // 
            // textBox
            // 
            this.textBox.Location = new System.Drawing.Point(15, 33);
            this.textBox.Name = "textBox";
            this.textBox.Size = new System.Drawing.Size(581, 296);
            this.textBox.TabIndex = 1;
            this.textBox.TabStop = false;
            this.textBox.Text = resources.GetString("textBox.Text");
            // 
            // ScriptEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(95)))), ((int)(((byte)(122)))), ((int)(((byte)(156)))));
            this.ClientSize = new System.Drawing.Size(608, 506);
            this.Controls.Add(this.textBox);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.tabControl);
            this.Controls.Add(this.scriptLabel);
            this.Controls.Add(this.scriptLoadBut);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.runButton);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ScriptEditor";
            this.Text = "Script";
            this.tabControl.ResumeLayout(false);
            this.outputTab.ResumeLayout(false);
            this.outputTab.PerformLayout();
            this.error.ResumeLayout(false);
            this.error.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button runButton;
        private System.Windows.Forms.TextBox outputBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button scriptLoadBut;
        private System.Windows.Forms.Label scriptLabel;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage error;
        private System.Windows.Forms.TextBox errorBox;
        private System.Windows.Forms.TabPage outputTab;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.RichTextBox textBox;
    }
}