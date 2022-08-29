using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Automation;
using Gma.System.MouseKeyHook;
using System.Threading;
using System.Diagnostics;
using WindowsInput;
using WindowsInput.Native;

namespace Bio
{
    public class Automation
    {
        public static Elements elements;
        public static InputSimulator input;
        public static List<Recording> Recordings = new List<Recording>();

        public class Action
        {
            public KeyEventArgs key;
            public KeyPressEventArgs keyPress;
            public MouseEventArgs mouse;
            public AutomationElement automationElement;
            public long ticks;
            public string script;
            public string args;
            public bool move = false;
            DateTime time;
            bool handled = false;
            public enum Type
            {
                mousedown,
                mouseup,
                mousemove,
                keyup,
                keydown,
                keypress,
                script,
                macro
            }
            private Type type;
            public Type ActionType
            {
                get { return type; }
                set { type = value; }
            }

            public MouseEventArgs Mouse
            {
                get { return mouse; }
                set { mouse = value; }
            }
            public Action(Type t, KeyEventArgs e)
            {
                type = t;
                key = e;
                automationElement = Element;
            }
            public Action(Type t, KeyPressEventArgs e)
            {
                type = t;
                keyPress = e;
                automationElement = Element;
            }
            public Action(Type t, MouseEventArgs e)
            {
                type = t;
                mouse = e;
                automationElement = Element;
            }
            public Action(Type t, string s)
            {
                type = t;
                script = s;
                automationElement = Element;
            }
            public void Perform()
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                if (type == Type.mousedown)
                {
                    System.Windows.Point p = automationElement.GetClickablePoint();
                    input.Mouse.MoveMouseTo(p.X, p.Y);
                    if (mouse.Button == MouseButtons.Left)
                        input.Mouse.LeftButtonDown();
                    if (mouse.Button == MouseButtons.Right)
                        input.Mouse.RightButtonDown();
                }
                else if (type == Type.mouseup)
                {
                    System.Windows.Point p = automationElement.GetClickablePoint();
                    input.Mouse.MoveMouseTo(p.X, p.Y);
                    if (mouse.Button == MouseButtons.Left)
                        input.Mouse.LeftButtonUp();
                    if (mouse.Button == MouseButtons.Right)
                        input.Mouse.RightButtonUp();
                }
                else if (type == Type.keydown)
                {
                    input.Keyboard.KeyDown((VirtualKeyCode)key.KeyData);
                }
                else if (type == Type.keyup)
                {
                    input.Keyboard.KeyUp((VirtualKeyCode)key.KeyData);
                }
                else if (type == Type.keypress)
                {
                    input.Keyboard.KeyPress((VirtualKeyCode)key.KeyData);
                }
                else if (type == Type.script)
                    Bio.App.RunScript(script);
                else if (type == Type.macro)
                    ImageJ.RunMacro(script,args);
                stopwatch.Stop();
                ticks = stopwatch.ElapsedMilliseconds;
                stopwatch = null;
                handled = true;
            }
            
        }
        public class Recording
        {
            private string name;
            private List<Action> list = new List<Action>();
            public List<Action> List
            {
                get { return list; }
                set { list = value; }
            }
            public List<AutomationElement> Elements
            {
                get
                {
                    List<AutomationElement> l = new List<AutomationElement>();
                    foreach (var item in list)
                    {
                        l.Add(item.automationElement);
                    }
                    return l;
                }
            }
            public Recording()
            {
            }
            public Recording(List<Action> l)
            {
                list = l;
            }
            public void Run()
            {
                foreach (Action a in list)
                {
                    a.Perform();
                }
            }
        }

        private IKeyboardMouseEvents m_GlobalHook;
        private static bool recording = false;
        private static bool move = false;
        public static Recording actions = new Recording();
        public Automation()
        {
            m_GlobalHook = Hook.GlobalEvents();
            m_GlobalHook.MouseDownExt += GlobalHookMouseDownExt;
            m_GlobalHook.MouseUpExt += GlobalHookMouseUpExt;
            m_GlobalHook.MouseMoveExt += GlobalHookMouseMoveExt;
            m_GlobalHook.KeyPress += GlobalHookKeyPress;
            m_GlobalHook.KeyUp += GlobalHookKeyUp;
            m_GlobalHook.KeyDown += GlobalHookKeyDown;
        }

        //This represent the current element under the mouse.
        public static AutomationElement Element
        {
            get
            {
                return AutomationElement.FromPoint(new System.Windows.Point(Cursor.Position.X, Cursor.Position.Y));
            }
        }
        public static void StartRecording()
        {
            recording = true;
        }
        public static void StopRecording()
        {
            recording=false;
            Recordings.Add(actions);
        }
        private void GlobalHookMouseDownExt(object sender, MouseEventExtArgs e)
        {
            if (!recording)
                return;
            actions.List.Add(new Action(Action.Type.mousedown,e));

        }
        private void GlobalHookMouseUpExt(object sender, MouseEventExtArgs e)
        {
            if (!recording)
                return;
            actions.List.Add(new Action(Action.Type.mouseup, e));
        }
        private void GlobalHookMouseMoveExt(object sender, MouseEventExtArgs e)
        {
            if (!move)
                return;
            if (!recording)
                return;
            actions.List.Add(new Action(Action.Type.mousemove, e));

        }
        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            if (!recording)
                return;
            actions.List.Add(new Action(Action.Type.keypress, e));
        }
        private void GlobalHookKeyUp(object sender, KeyEventArgs e)
        {
            if (!recording)
                return;
            actions.List.Add(new Action(Action.Type.keyup, e));
        }
        private void GlobalHookKeyDown(object sender, KeyEventArgs e)
        {
            if (!recording)
                return;
            actions.List.Add(new Action(Action.Type.keydown, e));
        }

        public static class AutomationHelpers
        {
            public static List<AutomationElement> GetChildren(AutomationElement parent)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }

                AutomationElementCollection collection = parent.FindAll(TreeScope.Children, Condition.TrueCondition);

                if (collection != null)
                {
                    List<AutomationElement> result = new List<AutomationElement>(collection.Cast<AutomationElement>());
                    return result;
                }
                else
                {
                    // some error occured
                    return null;
                }
            }
            public static List<AutomationElement> GetAllChildren(AutomationElement parent)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }
                try
                {
                    AutomationElementCollection collection = parent.FindAll(TreeScope.Subtree, Condition.TrueCondition);
                    if (collection != null)
                    {
                        List<AutomationElement> result = new List<AutomationElement>(collection.Cast<AutomationElement>());
                        return result;
                    }
                    else
                    {
                        // some error occured
                        return null;
                    }
                }
                catch (Exception )
                {
                    return null;
                }
            }
            public static List<AutomationElement> GetName(AutomationElement parent, string name)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }

                AutomationElementCollection collection = parent.FindAll(TreeScope.Subtree, Condition.TrueCondition);

                if (collection != null)
                {
                    List<AutomationElement> result = new List<AutomationElement>();
                    foreach (AutomationElement item in collection)
                    {
                        if (item.Current.Name == name)
                        {
                            result.Add(item);
                        }
                    }
                    return result;
                }
                else
                {
                    // some error occured
                    return null;
                }
            }
            public static List<AutomationElement> GetAutomationID(AutomationElement parent, string id)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }

                AutomationElementCollection collection = parent.FindAll(TreeScope.Subtree, Condition.TrueCondition);

                if (collection != null)
                {
                    List<AutomationElement> result = new List<AutomationElement>();
                    foreach (AutomationElement item in collection)
                    {
                        if (item.Current.AutomationId == id)
                        {
                            result.Add(item);
                        }
                    }
                    return result;
                }
                else
                {
                    // some error occured
                    return null;
                }
            }
            public static List<AutomationElement> GetClassName(AutomationElement parent, string name)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }

                AutomationElementCollection collection = parent.FindAll(TreeScope.Subtree, Condition.TrueCondition);

                if (collection != null)
                {
                    List<AutomationElement> result = new List<AutomationElement>();
                    foreach (AutomationElement item in collection)
                    {
                        if (item.Current.ClassName == name)
                        {
                            result.Add(item);
                        }
                    }
                    return result;
                }
                else
                {
                    // some error occured
                    return null;
                }
            }
            public static List<AutomationElement> GetLocalizedControlType(AutomationElement parent, TreeScope scope, string type)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }
                AutomationElementCollection collection = parent.FindAll(scope, Condition.TrueCondition);
                if (collection != null)
                {
                    List<AutomationElement> result = new List<AutomationElement>();
                    foreach (AutomationElement item in collection)
                    {
                        if (item.Current.LocalizedControlType == type)
                        {
                            result.Add(item);
                        }
                    }
                    return result;
                }
                else
                {
                    // some error occured
                    return null;
                }
            }
            public static List<AutomationElement> GetRawChildren(AutomationElement parent)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }

                List<AutomationElement> result = new List<AutomationElement>();

                // the predefined tree walker wich iterates through controls
                TreeWalker tw = TreeWalker.RawViewWalker;
                AutomationElement child = tw.GetFirstChild(parent);

                while (child != null)
                {
                    result.Add(child);
                    child = tw.GetNextSibling(child);
                }

                return result;
            }
            public static List<AutomationElement> GetRawAutomationID(AutomationElement parent, string auto)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }

                List<AutomationElement> all = new List<AutomationElement>();
                List<AutomationElement> result = new List<AutomationElement>();

                // the predefined tree walker wich iterates through controls
                TreeWalker tw = TreeWalker.RawViewWalker;
                AutomationElement child = tw.GetFirstChild(parent);

                while (child != null)
                {
                    all.Add(child);
                    child = tw.GetNextSibling(child);
                }
                foreach (AutomationElement item in all)
                {
                    if (item.Current.AutomationId == auto)
                    {
                        result.Add(item);
                    }
                }

                return result;
            }
            public static List<AutomationElement> GetRawLocalizedControlType(AutomationElement parent, string auto)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }

                List<AutomationElement> all = new List<AutomationElement>();
                List<AutomationElement> result = new List<AutomationElement>();

                // the predefined tree walker wich iterates through controls
                TreeWalker tw = TreeWalker.RawViewWalker;
                AutomationElement child = tw.GetFirstChild(parent);

                while (child != null)
                {
                    all.Add(child);
                    child = tw.GetNextSibling(child);
                }
                foreach (AutomationElement item in all)
                {
                    if (item.Current.LocalizedControlType == auto)
                    {
                        result.Add(item);
                    }
                }

                return result;
            }
            public static List<AutomationElement> GetRaw(AutomationElement parent)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }

                List<AutomationElement> result = new List<AutomationElement>();

                // the predefined tree walker wich iterates through controls
                TreeWalker tw = TreeWalker.RawViewWalker;
                AutomationElement child = tw.GetFirstChild(parent);

                while (child != null)
                {
                    result.Add(child);
                    child = tw.GetNextSibling(child);
                }

                return result;
            }
            public static int GetCount(AutomationElement parent)
            {
                if (parent == null)
                {
                    // null parameter
                    throw new ArgumentException();
                }
                int i = 0;
                // the predefined tree walker wich iterates through controls
                TreeWalker tw = TreeWalker.RawViewWalker;
                AutomationElement child = tw.GetFirstChild(parent);
                while (child != null)
                {
                    child = tw.GetNextSibling(child);
                    i++;
                }
                return i;
            }
            public static bool IsElementToggledOn(AutomationElement element)
            {
                if (element == null)
                {
                    return false;
                }

                Object objPattern;
                TogglePattern togPattern;
                if (true == element.TryGetCurrentPattern(TogglePattern.Pattern, out objPattern))
                {
                    togPattern = objPattern as TogglePattern;
                    return togPattern.Current.ToggleState == ToggleState.On;
                }
                return false;
            }

        }
        
    }

}
