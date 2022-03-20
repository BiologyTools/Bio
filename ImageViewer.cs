﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;

namespace BioImage
{
    public partial class ImageViewer : Form
    {
        Tools tools;
        private ImageView viewer = null;
        bool useFolderBrowser = false;

        public ImageViewer(string id)
        {
            InitializeComponent();
            SetFolder(id, false, 0);
        }

        public ImageViewer(string[] arg)
        {
            InitializeComponent();
            tools = new Tools();
            //string s = "E:/TESTIMAGES/ROIS.ome.tif";
            //string s = "E:/TESTIMAGES/text.ome.tif";
            //string s = "E:/TESTIMAGES/points5.ome.tif";
            //string s = "E:/TESTIMAGES/fill.ome.tif";
            //SetFolder(s, false, 0);

            //viewer.image.SaveSeries(s, 0);
            if (arg.Length == 0)
                return;
            else
            if (arg.Length == 1)
            {
                SetFile(arg[0], 0, false);
            }
        }

        public void SetFile(string file, int seri, bool folder)
        {
            if (viewer == null)
            {
                viewer = new ImageView(file, seri, folder);
            }
            viewer.serie = seri;
            viewer.filepath = file;
            
            viewer.Dock = DockStyle.Fill;
            
            panel.Controls.Add(viewer);
            string name = Path.GetFileName(file);
            this.Text = name;
        }
        public void SetFolder(string file, bool folder, int seri)
        {
            viewer = new ImageView(file, seri, folder);
            viewer.Dock = DockStyle.Fill;
            panel.Controls.Add(viewer);
            viewer.serie = seri;
            string name = Path.GetFileName(file);
            this.Text = name;
        }

        public BioImage Image
        {
            get 
            {
                if (viewer == null)
                    return null;
                if (viewer.image == null)
                    return null;
                return viewer.image; 
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            SetFolder(openFileDialog.FileName, false, 0);
        }

        private void openFolderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (useFolderBrowser)
            {
                if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                    return;
                SetFile(openFileDialog.FileName, Image.serie, true);
            }
            else
            {
                if (openFileDialog.ShowDialog() != DialogResult.OK)
                    return;
                SetFile(openFileDialog.FileName, Image.serie, true);
            }
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.viewer != null)
            {
                //viewer.image.Dispose();
                this.viewer.Dispose();
            }
            this.viewer = null;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Image == null)
                return;
            if (saveFileDialog.ShowDialog() != DialogResult.OK)
                return;
            viewer.image.SaveSeries(saveFileDialog.FileName,0);
        }

        private void ImageViewer_SizeChanged(object sender, EventArgs e)
        {
            if (this.Width <= MinimumSize.Width)
            {
                this.Width = MinimumSize.Width;
            }
            if (this.Height <= MinimumSize.Height)
            {
                this.Height = MinimumSize.Height;
            }
        }
        private void ImageViewer_Resize(object sender, EventArgs e)
        {
            if (this.Width <= MinimumSize.Width)
            {
                this.Width = MinimumSize.Width;
            }
            if (this.Height <= MinimumSize.Height)
            {
                this.Height = MinimumSize.Height;
            }
        }

        private void ImageViewer_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if(e.KeyCode == Keys.S && e.Control)
            {
                saveToolStripMenuItem.PerformClick();
            }
            if (e.KeyCode == Keys.O && e.Control)
            {
                openToolStripMenuItem.PerformClick();
            }
        }

        private void toolboxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            tools.Show();
        }

        private void newWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() != DialogResult.OK)
                return;
            Process p = new Process();
            p.StartInfo.FileName = Application.ExecutablePath;
            p.StartInfo.Arguments = openFileDialog.FileName;
            p.Start();
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exportCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (saveCSVFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string con = "";
            string cols = "ROIID,ROINAME,TYPE,ID,SHAPEINDEX,TEXT,S,C,Z,T,X,Y,W,H,POINTS,STROKECOLOR,STROKECOLORW,FILLCOLOR,FONTSIZE" + Environment.NewLine;
            con += cols;
            for (int i = 0; i < viewer.image.Annotations.Count; i++)
            {
                BioImage.Annotation an = viewer.image.Annotations[i];
                BioImage.PointD[] points = an.GetPoints();
                string pts = "";
                for (int j = 0; j < points.Length; j++)
                {
                    if(j == points.Length - 1)
                        pts += points[j].X.ToString() + "," + points[j].Y.ToString();
                    else
                        pts += points[j].X.ToString() + "," + points[j].Y.ToString() + " ";
                }

                char sep = (char)34; 
                string sColor = sep.ToString() + an.strokeColor.A.ToString() + ',' + an.strokeColor.R.ToString() + ',' + an.strokeColor.G.ToString() + ',' + an.strokeColor.B.ToString() + sep.ToString();
                string bColor = sep.ToString() + an.fillColor.A.ToString() + ',' + an.fillColor.R.ToString() + ',' + an.fillColor.G.ToString() + ',' + an.fillColor.B.ToString() + sep.ToString();

                string line = an.roiID + ',' + an.roiName + ',' + an.type.ToString() + ',' + an.id + ',' + an.shapeIndex.ToString() + ',' +
                    an.text + ',' + an.coord.S.ToString() + ',' + an.coord.Z.ToString() + ',' + an.coord.C.ToString() + ',' + an.coord.T.ToString() + ',' + an.X.ToString() + ',' + an.Y.ToString() + ',' +
                    an.W.ToString() + ',' + an.H.ToString() + ',' + sep.ToString() + pts + sep.ToString() + ',' + sColor + ',' + an.strokeWidth.ToString() + ',' + bColor + ',' + an.font.Size.ToString() + ',' + Environment.NewLine;
                con += line;
            }
            File.WriteAllText(saveCSVFileDialog.FileName, con);
        }

        private void importCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openCSVFileDialog.ShowDialog() != DialogResult.OK)
                return;
            string[] sts = File.ReadAllLines(openCSVFileDialog.FileName);
            for (int l = 1; l < sts.Length; l++)
            {
                BioImage.Annotation an = new BioImage.Annotation();
                string val = "";
                bool inSep = false;
                int col = 0;
                double x = 0;
                double y = 0;
                double w = 0;
                double h = 0;
                string line = sts[l];
                
                for (int i = 0; i < line.Length; i++)
                {
                    char c = line[i];
                    if(c == (char)34)
                    {
                        if (!inSep)
                        {
                            inSep = true;
                        }
                        else
                            inSep = false;
                        continue;
                    }

                    if(c == ',' && !inSep)
                    {
                        //ROIID,ROINAME,TYPE,ID,SHAPEINDEX,TEXT,S,C,Z,T,X,Y,W,H,POINTS,STROKECOLOR,STROKECOLORW,FILLCOLOR,FONTSIZE
                        if (col == 0)
                        {
                            //ROIID
                            an.roiID = val;
                        }
                        else
                        if (col == 1)
                        {
                            //ROINAME
                            an.roiName = val;
                        }
                        else
                        if (col == 2)
                        {
                            //TYPE
                            an.type = (BioImage.Annotation.Type)Enum.Parse(typeof(BioImage.Annotation.Type), val);
                        }
                        else
                        if (col == 3)
                        {
                            //ID
                            an.id = val;
                        }
                        else
                        if (col == 4)
                        {
                            //SHAPEINDEX/
                            an.shapeIndex = int.Parse(val);
                        }
                        else
                        if (col == 5)
                        {
                            //TEXT/
                            an.text = val;
                        }
                        else
                        if (col == 6)
                        {
                            an.coord.S = int.Parse(val);
                        }
                        else
                        if (col == 7)
                        {
                            an.coord.Z = int.Parse(val);
                        }
                        else
                        if (col == 8)
                        {
                            an.coord.C = int.Parse(val);
                        }
                        else
                        if (col == 9)
                        {
                            an.coord.T = int.Parse(val);
                        }
                        else
                        if (col == 10)
                        {
                            x = double.Parse(val);
                        }
                        else
                        if (col == 11)
                        {
                            y = double.Parse(val);
                        }
                        else
                        if (col == 12)
                        {
                            w = double.Parse(val);
                        }
                        else
                        if (col == 13)
                        {
                            h = double.Parse(val);
                        }
                        else
                        if (col == 14)
                        {
                            //POINTS
                            an.AddPoints(an.stringToPoints(val));
                            an.Rect = new BioImage.RectangleD(x, y, w, h);
                        }
                        else
                        if (col == 15)
                        {
                            //STROKECOLOR
                            string[] st = val.Split(',');
                            an.strokeColor = System.Drawing.Color.FromArgb(int.Parse(st[0]), int.Parse(st[1]), int.Parse(st[2]), int.Parse(st[3]));
                        }
                        else
                        if (col == 16)
                        {
                            //STROKECOLORW
                            an.strokeWidth = double.Parse(val);
                        }
                        else
                        if (col == 17)
                        {
                            //FILLCOLOR
                            string[] st = val.Split(',');
                            an.fillColor = System.Drawing.Color.FromArgb(int.Parse(st[0]), int.Parse(st[1]), int.Parse(st[2]), int.Parse(st[3]));
                        }
                        else
                        if (col == 18)
                        {
                            //FONTSIZE
                            double s = double.Parse(val);
                            an.font = new System.Drawing.Font(System.Drawing.SystemFonts.DefaultFont.FontFamily, (float)s, System.Drawing.FontStyle.Regular);
                        }
                        col++;
                        val = "";
                    }
                    else
                    val += c;
                }

                viewer.image.Annotations.Add(an);
            }
            
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog.ShowDialog() != DialogResult.OK)
                return;
            //Bio
        }
    }
}
