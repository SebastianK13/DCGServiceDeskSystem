﻿using DCGServiceDesk.Data.Models;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
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
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : UserControl
    {
        private AboutWindow about;
        public Home()
        {
            try
            {
                InitializeComponent();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void GridSplitter_MouseEnter(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
                Mouse.OverrideCursor = Cursors.Hand;
        }

        private void GridSplitter_MouseLeave(object sender, MouseEventArgs e)
        {
            if (this.Cursor != Cursors.Wait)
                Mouse.OverrideCursor = Cursors.Arrow;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            
        }

        private void TreeView_SelectedItemChanged(object sender, RoutedEventArgs e)
        {
            if (!(sender is TreeView myTreeView)) return;
            var selectedItem = (TreeViewItem)myTreeView.SelectedItem;
            if (selectedItem == null) return;
            selectedItem.IsSelected = false;
        }

        private void MenuItem_About_Click(object sender, RoutedEventArgs e)
        {
            if(about == null)
            {
                about = new AboutWindow(this);
                about.Show();
                Window aboutWin = Window.GetWindow(about);
                aboutWin.Closing += about_Closing;
            }
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {
            Window window = Window.GetWindow(this);
            window.Close();
        }
        void about_Closing(object sender, global::System.ComponentModel.CancelEventArgs e) =>
            about = null;                       
    }
}
