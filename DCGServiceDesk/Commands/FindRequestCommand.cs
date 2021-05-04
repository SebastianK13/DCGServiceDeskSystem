using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
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

        }
        private async Task CreateQueue()
        {
            switch(_rSVM.SearchingPhrase.ToUpper())
            {
                case "IM":
                    _rSVM.FoundedRequests = new QueueViewModel(
                        await _viewRequestService.SetAllIncidentsQueue(), _dbInterfaceContainer, _hVM);
                    break;
                case "C":
                    _rSVM.FoundedRequests = new QueueViewModel(
                        await _viewRequestService.SetAllChangesQueue(), _dbInterfaceContainer, _hVM);
                    break;
                case "T":
                    _rSVM.FoundedRequests = new QueueViewModel(
                        await _viewRequestService.SetAllTasksQueue(), _dbInterfaceContainer, _hVM);
                    break;
            }
        }
    }
}
