using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace Bio
{
    public class ImageJ
    {
        public static string ImageJPath;
        public static List<Process> processes = new List<Process>();
        private static Random rng = new Random();
        public static void RunMacro(string file, string param)
        {
            file.Replace("/", "\\");
            Process pr = new Process();
            pr.StartInfo.FileName = ImageJPath;
            pr.StartInfo.Arguments = "-macro " + file + " " + param;
            pr.Start();
            processes.Add(pr);
            Recorder.AddLine("ImageJ.RunMacro(" + file + "," + '"' + param + '"' + ");");
        }
        public static void RunString(string con, string param, bool headless)
        {
            Process pr = new Process();
            pr.StartInfo.FileName = ImageJPath;
            string te = rng.Next(0, 9999999).ToString();
            string p = Environment.CurrentDirectory + "\\" + te + ".txt";
            p.Replace("/", "\\");
            File.WriteAllText(p,con);
            if(headless)
                pr.StartInfo.Arguments = "--headless -macro " + p + " " + param;
            else
                pr.StartInfo.Arguments = "-macro " + p + " " + param;
            pr.Start();
            File.Delete(Path.GetDirectoryName(ImageJPath) + "/done.txt");
            processes.Add(pr);
            do
            {
                if (File.Exists(Path.GetDirectoryName(ImageJPath) + "/done.txt"))
                {
                    do
                    {
                        try
                        {
                            File.Delete(Path.GetDirectoryName(ImageJPath) + "/done.txt");
                        }
                        catch (Exception)
                        {
                        
                        }
                    } while (File.Exists(Path.GetDirectoryName(ImageJPath) + "/done.txt"));
                    pr.Kill();
                    break;
                }
            } while (!pr.HasExited);

        }
        public static void RunOnImage(string con, string param, bool headless)
        {
            string st =
            "run(\"Bio-Formats Importer\", \"open=\" + getArgument + \" autoscale color_mode=Default open_all_series display_metadata display_rois rois_import=[ROI manager] view=Hyperstack stack_order=XYCZT\");" +
            con + 
            "run(\"Bio-Formats Exporter\", \"save=F:/TESTIMAGES/CZI/16Bit-ZStack.ome.tif export compression=Uncompressed\");" +
            "dir = getDir(\"startup\");" +
            "File.saveString(\"done\", dir + \"/done.txt\");";
            RunString(st, param, headless);
        }
        public static void Initialize(string path)
        {
            ImageJPath = path;
        }
    }
}
