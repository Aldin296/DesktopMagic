﻿using Microsoft.Win32;

using System;
using System.Timers;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace DesktopMagic
{
    /// <summary>
    /// Interaktionslogik für TimeWindow.xaml
    /// </summary>
    public partial class DateWindow : Window
    {
        private RegistryKey key;

        public DateWindow()
        {
            InitializeComponent();

            Window w = new Window();
            w.Top = -100;
            w.Left = -100;
            w.Width = 0;
            w.Height = 0;

            w.WindowStyle = WindowStyle.ToolWindow;
            w.ShowInTaskbar = false;
            w.Show();
            Owner = w;
            w.Hide();

            Timer t = new Timer();
            t.Interval = 100;
            t.Elapsed += T_Elapsed;
            t.Start();

            key = Registry.CurrentUser.CreateSubKey(@"Software\" + App.AppName);
            Top = double.Parse(key.GetValue("DateWindowTop", 100).ToString());
            Left = double.Parse(key.GetValue("DateWindowLeft", 100).ToString());
            Height = double.Parse(key.GetValue("DateWindowHeight", 200).ToString());
            Width = double.Parse(key.GetValue("DateWindowWidth", 500).ToString());
            IsEnabled = false;
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);

            //Set the window style to noactivate.
            WindowInteropHelper helper = new WindowInteropHelper(this);
            WindowPos.SetWindowLong(helper.Handle, WindowPos.GWL_EXSTYLE,
            WindowPos.GetWindowLong(helper.Handle, WindowPos.GWL_EXSTYLE) | WindowPos.WS_EX_NOACTIVATE);
        }

        private void T_Elapsed(object sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                if (MainWindow.EditMode)
                {
                    panel.Visibility = Visibility.Visible;
                    WindowPos.SetIsLocked(this, false);
                }
                else
                {
                    panel.Visibility = Visibility.Collapsed;
                    WindowPos.SetIsLocked(this, true);
                }
                textBlock.FontFamily = new FontFamily(MainWindow.GlobalFont);
                textBlock.Foreground = MainWindow.GlobalColor;
                textBlock.Text = DateTime.Now.ToString("D");
            });
        }

        private void Window_LocationChanged(object sender, EventArgs e)
        {
            key.SetValue("DateWindowTop", Top);
            key.SetValue("DateWindowLeft", Left);
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            key.SetValue("DateWindowHeight", Height);
            key.SetValue("DateWindowWidth", Width);
            tileBar.CaptionHeight = ActualHeight - 10;
        }
    }
}