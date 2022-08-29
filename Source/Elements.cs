using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Automation;

namespace Bio
{
    public partial class Elements : Form
    {
        
        List<AutomationElement> elements = new List<AutomationElement>();
        public Elements(AutomationElement el)
        {
            InitializeComponent();
            elements = Automation.AutomationHelpers.GetAllChildren(el);
        }
        public Elements()
        {
            InitializeComponent();
        }
        public void UpdateElements()
        {
            view.Nodes.Clear();
            foreach (Automation.Recording rec in Automation.Recordings)
            {

                foreach (var item in rec.Elements)
                {
                    try
                    {
                        TreeNode tn = new TreeNode();
                        tn.Text = item.Current.Name + item.Current.AutomationId + "," + item.Current.LocalizedControlType;
                        Node n = new Node(item, tn);
                        tn.Tag = n;
                        view.Nodes.Add(tn);
                    }
                    catch (Exception)
                    {

                    }
                    
                }
            }
            
        }
        public class Node
        {
            public AutomationElement element;
            public List<AutomationElement> items = new List<AutomationElement>();

            public TreeNode node;
            public Node(AutomationElement el,TreeNode n)
            {
                element = el;
                node = n;
                items = Automation.AutomationHelpers.GetAllChildren(el);
            }
            public override string ToString()
            {
                return element.Current.Name + ", " + element.Current.LocalizedControlType.ToString(); 
            }
        }

        private void view_AfterSelect(object sender, TreeViewEventArgs e)
        {
            
        }

        private void timer_Tick(object sender, EventArgs e)
        {

        }

        private void startBut_Click(object sender, EventArgs e)
        {
            recordStatusLabel.Text = "Recording: Started";
            Automation.StartRecording();
        }

        private void stopBut_Click(object sender, EventArgs e)
        {
            recordStatusLabel.Text = "Recording: Stopped";
            Automation.StopRecording();

            if (Automation.Recordings.Count > 0)
            {
                elements = Automation.Recordings[0].Elements;
                UpdateElements();
            }
        }

        private void playBut_Click(object sender, EventArgs e)
        {
            
        }

        private void view_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            
        }

        private void Elements_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.Hide();
            e.Cancel = true;
        }

        private void Elements_Activated(object sender, EventArgs e)
        {
            UpdateElements();
        }
    }
}
