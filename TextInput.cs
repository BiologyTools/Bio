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
    public partial class TextInput : Form
    {
        BioImage.Annotation anno = new BioImage.Annotation();
        public TextInput(BioImage.Annotation an)
        {
            InitializeComponent();
            anno = an;
            anno.font = DefaultFont;
            DialogResult = DialogResult.Cancel;
        }

        private void okBut_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelBut_Click(object sender, EventArgs e)
        {
            DialogResult=DialogResult.Cancel;
        }

        private void fontBut_Click(object sender, EventArgs e)
        {
            if (fontDialog.ShowDialog() != DialogResult.OK)
                return;
            anno.font = fontDialog.Font;
        }
    }
}
