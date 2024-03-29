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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DCGServiceDesk.View
{
    /// <summary>
    /// Interaction logic for EscalatedView.xaml
    /// </summary>
    public partial class EscalatedView : UserControl
    {
        public EscalatedView()
        {
            InitializeComponent();
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e)
        {
            ScrollViewer scrollViewer = sender as ScrollViewer;
            scrollViewer.ScrollToVerticalOffset(0);
            scrollViewer.ScrollToHorizontalOffset(0);
        }
    }
}
