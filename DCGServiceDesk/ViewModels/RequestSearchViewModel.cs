using DCGServiceDesk.Commands;
using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
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
        private bool _searchingResultVis;

        public RequestSearchViewModel(DbInterfaceContainer interfaceContainer, HomeViewModel homeViewModel) 
            : base(interfaceContainer, homeViewModel)
        {
            FindRequestCommand = new FindRequestCommand(interfaceContainer, homeViewModel, this);
            Label = "Request Searching";
            RequestInfo requestInfo = new RequestInfo();
            requestInfo.Requests = new List<object>();
            requestInfo.Requests.Add(new Incident());
            FoundedRequests = new QueueViewModel(requestInfo, interfaceContainer, homeViewModel);
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
        public bool SearchingResultVisibility 
        {
            get { return _searchingResultVis; }
            set
            {
                _searchingResultVis = value;
                OnPropertyChanged("SearchingResultVisibility");
            }
        }

    }
}
