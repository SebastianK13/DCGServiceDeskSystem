using System;
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
    /// Interaction logic for SearchingResultView.xaml
    /// </summary>
    public partial class SearchingResultView : UserControl
    {
        public SearchingResultView()
        {
            InitializeComponent();
        }

        private void serviceRequests_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            ListView listView = sender as ListView;
            GridView gView = listView.View as GridView;

            var workingWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth;
            double col1 = 0.13;
            double col2 = 0.2;
            double col3 = 0.12;
            double col4 = 0.12;
            double col5 = 0.14;
            double col6 = 0.15;
            double col7 = 0.14;

            gView.Columns[0].Width = workingWidth * col1;
            gView.Columns[1].Width = workingWidth * col2;
            gView.Columns[2].Width = workingWidth * col3;
            gView.Columns[3].Width = workingWidth * col4;
            gView.Columns[4].Width = workingWidth * col5;
            gView.Columns[5].Width = workingWidth * col6;
            gView.Columns[6].Width = workingWidth * col7;
        }
    }
}
