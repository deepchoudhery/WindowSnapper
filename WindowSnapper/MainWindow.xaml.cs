using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using WindowSnapper.Models;

namespace WindowSnapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        [DllImport("user32.dll")]
        public static extern bool GetWindowRect(IntPtr hwnd, ref RECT rectangle);

        [DllImport("user32.dll", SetLastError = true)]
        static extern bool MoveWindow(IntPtr hWnd, int X, int Y, int Width, int Height, bool Repaint);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, SetWindowPosFlags uFlags);

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }

        public string ProfilesJsonPath 
        {
            get
            {
                if (string.IsNullOrEmpty(_profilesJsonPath))
                {
                    _profilesJsonPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"profiles.json");
                }
                return _profilesJsonPath;
            }

        }

        private string _profilesJsonPath;

        //private NotifyIcon _notifyIcon;
        private WindowState _storedWindowState = WindowState.Normal;

        public MainWindow()
        {
           /* _notifyIcon = new NotifyIcon
            {
                BalloonTipText = "Window Snapper has been minimized. Click to edit preferences.",
                BalloonTipTitle = "Window Snapper",
                Text = "Window Snapper"
            };
            _notifyIcon.Click += new EventHandler(_notifyIcon_Click);*/
            
            //_notifyIcon.Icon = new System.Drawing.Icon("trayIcon.ico");

            /*TODO if default profile is present,
                        minimize to taskbar,
                   else 
                        load the initial setup?
            */
            var profile = LoadProfiles();
            if (profile != null)
            {
                
            }
            //TODO snap apps in monitors
            //TODO save a profile
            InitializeComponent();
        }

        public void ConfigurePreferences_OnClick(object sender, EventArgs e)
        {
            var processes = GetActiveProcesses();
            //var dimensions = monitors.Select(x => x.Bounds.Location);
            IDictionary<string, ProcessInfo> processInfoDict = new Dictionary<string, ProcessInfo>();
            foreach (var process in processes)
            {
                IntPtr ptr = process.MainWindowHandle;
                RECT NotepadRect = new RECT();
                GetWindowRect(ptr, ref NotepadRect);
                try
                {
                    int height = NotepadRect.bottom - NotepadRect.top;
                    int width = NotepadRect.right - NotepadRect.left;
                    int x = NotepadRect.left;
                    int y = NotepadRect.top;
                    string name = process.ProcessName;

                    processInfoDict.Add(name, new ProcessInfo(x, y, width, height));

                    //MoveWindow(ptr, 0, 0, NotepadRect.right - Math.Max(0, NotepadRect.left) - 20, NotepadRect.bottom - Math.Max(0, NotepadRect.top) - 100, true);
                    //SetWindowPos(ptr, new IntPtr(0), x, y, width, height, SetWindowPosFlags.SWP_SHOWWINDOW);  
                }
                catch (Win32Exception ex)
                {
                }
            }

            if (processInfoDict.Any())
            {
                var profile = new Profile("Test", processInfoDict, true);
                SaveProfile(profile);
            }
        }

        private void _notifyIcon_Click(object sender, EventArgs e)
        {
            Show();
            WindowState = _storedWindowState;
        }

        private IList<Process> GetActiveProcesses()
        {
            IList<Process> activeProcesses = new List<Process>();
            IList<Process> allProcesses = Process.GetProcesses().ToList<Process>();
            
            foreach (var process in allProcesses)
            {
                if (process.MainWindowHandle != IntPtr.Zero)
                {
                    activeProcesses.Add(process);
                }
            }
            return activeProcesses;
        }

        private Profile LoadProfiles()
        {
            Profile profile = null;
            string profiles = File.ReadAllText(ProfilesJsonPath, Encoding.UTF8);
            if (!string.IsNullOrEmpty(profiles))
            {
                profile = JsonConvert.DeserializeObject<Profile>(profiles);
            }
            return profile;
        }

        private void SaveProfile(Profile profile)
        {
            string profileJson = JsonConvert.SerializeObject(profile);
            File.WriteAllText(ProfilesJsonPath, profileJson, Encoding.UTF8);
        }

        public void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                /*if (_notifyIcon != null)
                {
                    _notifyIcon.ShowBalloonTip(2000);
                }
                else
                {
                    _storedWindowState = WindowState;
                }*/
            }
        }

        public void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        public void OnClose(object sender, EventArgs args)
        {
/*            _notifyIcon.Dispose();
            _notifyIcon = null;*/
        }

        private void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        private void ShowTrayIcon(bool show)
        {
/*            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = show;
            }*/
        }

        private bool _isProfileSelected;
        public bool IsProfileSelected { get; set; }

        public void ProfileComboBoxSelectionChanged(object sender, EventArgs args)
        {

        }

        public enum SpecialWindowHandles
        {
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2
        }

        [Flags]
        public enum SetWindowPosFlags : uint
        {
            SWP_ASYNCWINDOWPOS = 0x4000,

            SWP_DEFERERASE = 0x2000,

            SWP_DRAWFRAME = 0x0020,

            SWP_FRAMECHANGED = 0x0020,

            SWP_HIDEWINDOW = 0x0080,

            SWP_NOACTIVATE = 0x0010,

            SWP_NOCOPYBITS = 0x0100,

            SWP_NOMOVE = 0x0002,

            SWP_NOOWNERZORDER = 0x0200,

            SWP_NOREDRAW = 0x0008,

            SWP_NOREPOSITION = 0x0200,

            SWP_NOSENDCHANGING = 0x0400,

            SWP_NOSIZE = 0x0001,

            SWP_NOZORDER = 0x0004,

            SWP_SHOWWINDOW = 0x0040,
        }

        private void SnapApps_OnClick(object sender, RoutedEventArgs e)
        {
            Profile profile = LoadProfiles();
            if (profile != null)
            {
                var processes = GetActiveProcesses();
                var relevantProcs = processes.Where(x => profile.Processes.ContainsKey(x.ProcessName));
                
                foreach (var process in relevantProcs)
                {
                    profile.Processes.TryGetValue(process.ProcessName, out var processInfo);
                    if (processInfo != null)
                    {
                        IntPtr ptr = process.MainWindowHandle;
                        SetWindowPos(
                            ptr, 
                            new IntPtr(0),
                            processInfo.X,
                            processInfo.Y,
                            processInfo.Width,
                            processInfo.Height,
                            SetWindowPosFlags.SWP_SHOWWINDOW);
                    }
                }
            }
            else
            {
                //throw dialog
            }
        }
    }
}
