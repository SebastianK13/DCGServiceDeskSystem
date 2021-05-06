using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCGServiceDesk.Commands
{
    public class FindRequestCommand : AsyncCommandBase
    {
        private readonly DbInterfaceContainer _dbInterfaceContainer;
        private readonly HomeViewModel _hVM;
        private RequestSearchViewModel _rSVM;
        private readonly IViewRequestService _viewRequestService;
        private List<string> requestLabels = new List<string> { "im", "c", "t"};
        public FindRequestCommand(DbInterfaceContainer dbInterfaceContainer, HomeViewModel HVM, RequestSearchViewModel RSVM)
        {
            _dbInterfaceContainer = dbInterfaceContainer;
            _hVM = HVM;
            _viewRequestService = new ViewRequestService(dbInterfaceContainer);
            _rSVM = RSVM;
        }
        public async override Task ExecuteAsync(object parameter)
        {
            switch (parameter.ToString())
            {
                case "Search":
                    await SplitPhrase();
                    break;
                case "Abandon":
                    break;
                default:
                    break;
            }
        }
        private async Task SplitPhrase()
        {
            if (requestLabels.Contains(_rSVM.SearchingPhrase.ToLower()) &&
                _rSVM.SearchingPhrase.Length <= 2)
                await CreateQueue();
            else
                await SearchRequest();
        }

        private async Task SearchRequest()
        {
            string label = Regex
                .Replace(_rSVM.SearchingPhrase
                .ToUpper(), @"[^A-Z]+", string.Empty);
            string id = Regex
                .Replace(_rSVM.SearchingPhrase, @"[^0-9]+", string.Empty)
                .TrimStart('0');
            if(id != null)
            {
                switch (label)
                {
                    case "IM":
                        var im = await _viewRequestService
                            .SetAllIncidentsQueue(Convert.ToInt32(id), true);
                        if(!CheckIsNull(im))
                        {
                            _rSVM.FoundedRequests = new QueueViewModel(im,
                                _dbInterfaceContainer, _hVM);
                            _rSVM.SearchingResultVisibility = true;
                        }
                        break;
                    case "C":
                        var c = await _viewRequestService
                            .SetAllChangesQueue(Convert.ToInt32(id), true);
                        if (!CheckIsNull(c))
                        {
                            _rSVM.FoundedRequests = new QueueViewModel(c,
                                _dbInterfaceContainer, _hVM);
                            _rSVM.SearchingResultVisibility = true;
                        }
                        break;
                    case "T":
                        var t = await _viewRequestService
                            .SetAllTasksQueue(Convert.ToInt32(id), true);
                        if (!CheckIsNull(t))
                        {
                            _rSVM.FoundedRequests = new QueueViewModel(t,
                                _dbInterfaceContainer, _hVM);
                            _rSVM.SearchingResultVisibility = true;
                        }
                        break;
                }
            }
        }
        private bool CheckIsNull(object request)
        {
            if (request == null)
                return true;
            else 
                return false;
        }

        private async Task CreateQueue()
        {
            switch(_rSVM.SearchingPhrase.ToUpper())
            {
                case "IM":
                    _rSVM.FoundedRequests = new QueueViewModel(
                        await _viewRequestService.SetAllIncidentsQueue(), _dbInterfaceContainer, _hVM);
                    _rSVM.SearchingResultVisibility = true;
                    break;
                case "C":
                    _rSVM.FoundedRequests = new QueueViewModel(
                        await _viewRequestService.SetAllChangesQueue(), _dbInterfaceContainer, _hVM);
                    _rSVM.SearchingResultVisibility = true;
                    break;
                case "T":
                    _rSVM.FoundedRequests = new QueueViewModel(
                        await _viewRequestService.SetAllTasksQueue(), _dbInterfaceContainer, _hVM);
                    _rSVM.SearchingResultVisibility = true;
                    break;
            }
        }
    }
}
