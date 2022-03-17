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
    public partial class Nodes : UserControl
    {
        public class Node
        {
            string id;
            public Node(string id)
            {

            }
            public Node(BioImage image)
            {
                
            }
        }

        public Nodes()
        {
            InitializeComponent();
        }
    }
}
