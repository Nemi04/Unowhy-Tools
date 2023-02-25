﻿using System.Windows.Controls;
using Unowhy_Tools_WPF.ViewModels;

using Unowhy_Tools;
using System.Windows.Forms;
using System.Windows;
using CommunityToolkit.Mvvm.Input;
using System.Windows.Input;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Unowhy_Tools_WPF.Views.Windows
{
    /// <summary>
    /// Interaction logic for DialogQ.xaml
    /// </summary>
    public partial class DialogI : System.Windows.Controls.UserControl
    {
        public DialogI()
        {
            InitializeComponent();
            Visibility = Visibility.Hidden;
        }

        private bool _hideRequest = false;
        private bool _result = false;


        public bool ShowDialog(string message, BitmapImage image)
        {
            icon.Source = image;
            text.Text = message;
            Visibility = Visibility.Visible;
            _hideRequest = false;
            while (!_hideRequest)
            {
                if (this.Dispatcher.HasShutdownStarted ||
                    this.Dispatcher.HasShutdownFinished)
                {
                    break;
                }

                this.Dispatcher.Invoke(
                    DispatcherPriority.Background,
                    new ThreadStart(delegate { }));
                Thread.Sleep(20);
            }

            return _result;
        }

        private void HideDialog()
        {
            _hideRequest = true;
            Visibility = Visibility.Hidden;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            _result = true;
            HideDialog();
        }
    }
}