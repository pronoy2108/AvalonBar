﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Runtime.InteropServices;
using Sidebar.Core;
using System.Diagnostics;

namespace Sidebar
{
    /// <summary>
    /// Interaction logic for Notify.xaml
    /// </summary>
    public partial class Notify : Window
    {
        private DispatcherTimer timer;
        private int counter = 0;

        public Notify()
        {
            InitializeComponent();

        }

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            IntPtr handle = new WindowInteropHelper(this).Handle;
            if (DwmManager.IsBlurAvailable && LongBarMain.sett.enableGlass)
                DwmManager.EnableBlurBehindWindow(ref handle);

            timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
        }

        void timer_Tick(object sender, EventArgs e)
        {
            counter++;
            if (counter >= 10)
            {
                HideNotification();
            }
        }

        public void ShowNotification(string header, string message)
        {
            DoubleAnimation loadAnim = (DoubleAnimation)FindResource("LoadAnim");
            loadAnim.From = this.Top + 80;
            loadAnim.To = this.Top;

            Header.Text = header;
            //Body.Text = message;
            Body.Blocks.Clear();
            message = string.Format("<Paragraph xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">{0}</Paragraph>", message);
            Body.Blocks.Add((Block)XamlReader.Parse(message));

            foreach (Inline i in ((Paragraph)Body.Blocks.FirstBlock).Inlines)
            {
                if (i.GetType() == typeof(Hyperlink))
                {
                    ((Hyperlink)i).Click += new RoutedEventHandler(Notify_Click);
                }
            }

            this.Show();

            this.BeginAnimation(TopProperty, loadAnim);
            timer.Start();
        }

        void Notify_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(((Hyperlink)sender).NavigateUri.OriginalString);
        }

        public void HideNotification()
        {
            DoubleAnimation loadAnim = (DoubleAnimation)FindResource("UnloadAnim");
            loadAnim.To = this.Top + 80;

            this.Show();

            this.BeginAnimation(TopProperty, loadAnim);
            timer.Stop();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            counter = 0;
        }

        private void DoubleAnimation_Completed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Image_MouseEnter(object sender, MouseEventArgs e)
        {
            //CloseButton.Source = new BitmapImage(new Uri("/Sidebar;component/Resources/Close_button_g.png", UriKind.Relative));
        }

        private void Image_MouseLeave(object sender, MouseEventArgs e)
        {
            //CloseButton.Source = new BitmapImage(new Uri("/Sidebar;component/Resources/Close_button.png", UriKind.Relative));
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            this.HideNotification();
        }
    }
}
