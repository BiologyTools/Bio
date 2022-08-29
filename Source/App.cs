using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Automation;

namespace Bio
{
    public class App
    {
        public static ROIManager manager = null;
        public static ChannelsTool channelsTool = null;
        public static TabsView tabsView = null;
        public static NodeView nodeView = null;
        public static Scripting runner = null;
        public static Recorder recorder = null;
        public static Tools tools = null;
        public static StackTools stackTools = null;
        public static ImageView viewer = null;
        public static Series seriesTool = null;
        public static Elements elements = null;
        public static Automation automation = null;
        public static List<string> recent = new List<string>();

        public static BioImage Image
        {
            get {
                if (ImageView.SelectedImage == null)
                    return tabsView.Image;
                return ImageView.SelectedImage;
            }
        }
        public static List<Channel> Channels
        {
            get { return Image.Channels; }
        }

        public static List<ROI> Annotations
        {
            get { return Image.Annotations; }
        }
        public static void RunScript(string file)
        {
            Scripting.Script sc = new Scripting.Script(file);
            Recorder.AddLine("App.RunScript(" + '"' + file + '"' + ");");
            sc.Run();
        }

        public static void RunFilter(string filter)
        {
            
        }
        public static void Initialize()
        {
            BioImage.Initialize();
            tools = new Tools();
            stackTools = new StackTools();
            manager = new ROIManager();
            runner = new Scripting();
            recorder = new Recorder();
            seriesTool = new Series();
            elements = new Elements();
            automation = new Automation();
            ImageJ.Initialize(Properties.Settings.Default.ImageJPath);
            //channelsTool = new ChannelsTool();
        }

        public static void AddROI(string an)
        {
            Annotations.Add(BioImage.StringToROI(an));
            Recorder.AddLine("App.AddROI(" + '"' + an + "'" + ");");
        }
    }
}
