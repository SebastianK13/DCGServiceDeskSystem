﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DCGServiceDesk.View
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        public AboutWindow(Home home)
        {
            Window window = Window.GetWindow(home);           
            InitializeComponent();
            window.Closing += home_Closing;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;
            Owner = Application.Current.MainWindow;
        }       
        void home_Closing(object sender, global::System.ComponentModel.CancelEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Close();
        }
    }
}
