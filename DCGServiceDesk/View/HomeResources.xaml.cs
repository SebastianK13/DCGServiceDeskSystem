using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Linq;
using System.Collections;

namespace DCGServiceDesk.View
{
    public partial class HomeResources : ResourceDictionary
    {
        private TabControl tabControl;
        private ScrollViewer scrollViewer;
        private double lastWidth = 0;
        TabItem lastTabItem = null;
        public HomeResources()
        {
            InitializeComponent();

        }
        private void HeaderPanel_Loaded(object sender, RoutedEventArgs e) =>
            scrollViewer = sender as ScrollViewer;

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                tabControl = sender as TabControl;
                if (tabControl != null)
                {
                    var rootVisual = Application.Current.MainWindow;
                    object item = tabControl.SelectedItem;
                    var tabItems = tabControl.ItemsSource;
                    int index = tabControl.SelectedIndex;

                    if(index == -1 && item == null && lastTabItem.Content.ToString() != "{DisconnectedItem}")
                    {
                        if (lastTabItem != null)
                        {
                            scrollViewer.ScrollToHorizontalOffset(CountScrollOffset(tabItems, lastTabItem.Content));
                            lastTabItem?.Focus();
                        }
                    }
                    if(item != null)
                    {
                        TabItem tabItem = (TabItem)tabControl.ItemContainerGenerator.ContainerFromItem(item);
                        if (tabItem != lastTabItem && tabItem != null)
                        {
                            scrollViewer.ScrollToHorizontalOffset(CountScrollOffset(tabItems, item));
                            lastTabItem = tabItem;
                            lastTabItem?.Focus();
                        }
                    }
                }
            }
            catch(Exception ex)
            {

            }


        }

        private double CountScrollOffset(IEnumerable tabItems, object choosenItem)
        {
            double scrollWidth = scrollViewer.ActualWidth;
            List<object> tabItemsList = (tabItems as IEnumerable<object>).Cast<object>().ToList();
            TabItem choosenTabItem = (TabItem)tabControl.ItemContainerGenerator
                .ContainerFromItem(choosenItem);
            double actualWidthTabItem = choosenTabItem.ActualWidth;
            double offset = 0;
            for (int i = 0; i < tabItemsList.Count(); i++)
            {
                TabItem tabItem = (TabItem)tabControl.ItemContainerGenerator
                    .ContainerFromItem(tabItemsList[i]);

                offset += tabItem.ActualWidth;

                if (tabItemsList[i] == choosenItem)
                    i = tabItemsList.Count();
            }
            if (lastWidth > offset)
                offset -= actualWidthTabItem;

            lastWidth = offset;

            return offset;
        }

        private void TabControl_Unselected(object sender, RoutedEventArgs e)
        {

        }

        private void FrameworkElement_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
