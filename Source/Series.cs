using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bio
{
    public partial class Series : Form
    {
        public Series()
        {
            InitializeComponent();
            UpdateItems();
        }

        public void UpdateItems()
        {
           imagesBox.Items.Clear();
            foreach (BioImage item in Table.images)
            {
                imagesBox.Items.Add(item);
            }
        }

        private void addBut_Click(object sender, EventArgs e)
        {
            if (imagesBox.SelectedIndices.Count == 0)
                return;
            foreach (BioImage item in imagesBox.SelectedItems)
            {
                seriesBox.Items.Add(item);
            }
        }

        private void removeBut_Click(object sender, EventArgs e)
        {
            if (seriesBox.SelectedIndices.Count == 0)
                return;
            foreach (BioImage item in imagesBox.SelectedItems)
            {
                seriesBox.Items.Remove(item);
            }
        }

        private void Series_Activated(object sender, EventArgs e)
        {
            UpdateItems();
        }

        private void saveBut_Click(object sender, EventArgs e)
        {
            int i = 0;
            foreach (BioImage item in seriesBox.Items)
            {
                item.series = i;
                BioImage.SaveAsync(item.ID, item.ID);
                
                i++;
            }
        }
    }
}
