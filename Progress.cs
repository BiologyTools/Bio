using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BioImage
{
    public partial class Progress : Form
    {
        public Progress(string file, string status)
        {
            InitializeComponent();
            statusLabel.Text = status;
            fileLabel.Text = file;
        }
        public void UpdateProgress(int p)
        {
            progressBar.Value = p;
        }
    }
}
