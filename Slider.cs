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
    public partial class Slider : UserControl
    {

        public Slider()
        {
            InitializeComponent();
        }

        public Slider(int value, int min, int max)
        {
            InitializeComponent();
            Init(value, min, max);
        }

        public int Maximum
        {
            get {return SliderBar.Maximum;}
            set {SliderBar.Maximum = value;}
        }

        public int Minimum
        {
            get { return SliderBar.Minimum; }
            set {SliderBar.Minimum = value;}
        }

        public int Value
        {
            get { return SliderBar.Value;}
            set {SliderBar.Value = value;}
        }

        public void Init(int val, int min, int max)
        {
            SliderBar.Minimum = min;
            SliderBar.Maximum = max;
            SliderBar.Value = val;
            idLabel.Text = val.ToString();
            statLabel.Text = val.ToString() + ", (" + min.ToString() + "/" + max.ToString() + ")";
        }

        private void SliderBar_ValueChanged(object sender, EventArgs e)
        {
            int val = (int)SliderBar.Value;
            idLabel.Text = val.ToString();
            statLabel.Text = val.ToString() + ", (" + SliderBar.Minimum.ToString() + "/" + SliderBar.Maximum.ToString() + ")";
        }
    }
}
