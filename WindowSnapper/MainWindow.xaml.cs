using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using WindowSnapper.Models;

namespace WindowSnapper
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private NotifyIcon _notifyIcon;
        private WindowState _storedWindowState = WindowState.Normal;

        public MainWindow()
        {
            _notifyIcon = new NotifyIcon
            {
                BalloonTipText = "Window Snapper has been minimized. Click to edit preferences.",
                BalloonTipTitle = "Window Snapper",
                Text = "Window Snapper"
            };
            _notifyIcon.Click += new EventHandler(_notifyIcon_Click);
            
            //_notifyIcon.Icon = new System.Drawing.Icon("trayIcon.ico");

            //TODO load all profiles
            /*TODO if default profile is present,
                        minimize to taskbar,
                   else 
                        load the initial setup?
            */
            var profiles = LoadProfiles();
            if (profiles != null && profiles.Any())
            {
                var defaultProfile = profiles.Select(x => x.Default);
                if (defaultProfile != null && defaultProfile.Any())
                {
                    
                }
            }
            else
            {

            }

            var activeProcesses = GetActiveProcesses();
            var monitors = Screen.AllScreens;
            //TODO get a list of monitors
            //TODO snap apps in monitors
            //TODO save a profile

            InitializeComponent();
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

            //SetWindowPos()
        }

        private List<Profile> LoadProfiles()
        {
            return null;
        }

        public void OnStateChanged(object sender, EventArgs args)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
                if (_notifyIcon != null)
                {
                    _notifyIcon.ShowBalloonTip(2000);
                }
                else
                {
                    _storedWindowState = WindowState;
                }
            }
        }

        public void OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            CheckTrayIcon();
        }

        public void OnClose(object sender, EventArgs args)
        {
            _notifyIcon.Dispose();
            _notifyIcon = null;
        }

        private void CheckTrayIcon()
        {
            ShowTrayIcon(!IsVisible);
        }

        private void ShowTrayIcon(bool show)
        {
            if (_notifyIcon != null)
            {
                _notifyIcon.Visible = show;
            }
        }

        private bool _isProfileSelected;
        public bool IsProfileSelected { get; set; }

        public void ProfileComboBoxSelectionChanged(object sender, EventArgs args)
        {

        }


    }
}
