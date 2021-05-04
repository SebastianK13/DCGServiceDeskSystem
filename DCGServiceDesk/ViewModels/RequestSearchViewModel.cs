using DCGServiceDesk.Commands;
using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using System.Windows.Media;

namespace DCGServiceDesk.ViewModels
{
    public class RequestSearchViewModel : Tab
    {
        private string _searchingPhrase;
        private Brush _searchTextBoxColor;
        private QueueViewModel _foundedRequests;

        public RequestSearchViewModel(DbInterfaceContainer interfaceContainer, HomeViewModel homeViewModel) 
            : base(interfaceContainer, homeViewModel)
        {
            FindRequestCommand = new FindRequestCommand(interfaceContainer, homeViewModel, this);
        }
        public ICommand FindRequestCommand { get; }
        public QueueViewModel FoundedRequests 
        {
            get { return _foundedRequests; }
            set
            {
                _foundedRequests = value;
                OnPropertyChanged("FoundedRequests");
            }
        }

        public string SearchingPhrase 
        {
            get { return _searchingPhrase; }
            set
            {
                if(value == "" || value == null)
                   SearchTextBoxColor = new SolidColorBrush(Color.FromArgb(0, 0, 0, 0));
                else
                    SearchTextBoxColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));

                _searchingPhrase = value;
                OnPropertyChanged("SearchingPhrase");
            }
        }
        public Brush SearchTextBoxColor
        {
            get { return _searchTextBoxColor; }
            set
            {
                if (value != _searchTextBoxColor)
                {
                    _searchTextBoxColor = value;
                    OnPropertyChanged("SearchTextBoxColor");
                }
            }
        }
    }
}
