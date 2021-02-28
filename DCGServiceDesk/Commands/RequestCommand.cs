using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DCGServiceDesk.Commands
{
    public class RequestCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private HomeViewModel _hVM;
        private List<string> ClassTypes = new List<string> { "IncidentProxy", "ServiceRequestProxy", "TaskRequestProxy" };

        public RequestCommand(IRequestQueue requestQueue, IUserInfo userInfo, IEmployeeProfile employeeProfile, HomeViewModel hVM)
        {
            _requestQueue = requestQueue;
            _userInfo = userInfo;
            _employeeProfile = employeeProfile;
            _hVM = hVM;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            try
            {
                string choosen = parameter as string;
                if (parameter.GetType().Name == "TabContainer")
                {

                    choosen = ((TabContainer)parameter).ServiceRequests.GetType().Name;
                }


                switch (choosen)
                {
                    case "RequestQueue":
                        var requests = await _requestQueue.GetRequests();
                        List<int> contactA = (List<int>)requests[1];
                        List<int> requestedA = (List<int>)requests[2];
                        List<string> contactIdA = await _employeeProfile.GetUserId(contactA);
                        List<string> requestedIdA = await _employeeProfile.GetUserId(requestedA);
                        _hVM.SetRequests((List<object>)requests[0],
                            await _userInfo.GetUserName(contactIdA, requestedIdA),
                            (List<string>)requests[3]);
                        break;
                    case "IncidentQueue":
                        var incidents = await _requestQueue.GetIncidents();
                        List<int> contact = incidents.Select(i=>i.ContactPerson).ToList();
                        List<int> requested = incidents.Select(i=>i.RequestedPerson).ToList();
                        List<string> contactId = await _employeeProfile.GetUserId(contact);
                        List<string> requestedId = await _employeeProfile.GetUserId(requested);
                        _hVM.SetIncidents(incidents, await _userInfo.GetUserName(contactId, requestedId));
                        break;
                    case "ChangesQueue":
                        var changes = await _requestQueue.GetChanges();
                        List<int> contactC = changes.Select(i => i.ContactPerson).ToList();
                        List<int> requestedC = changes.Select(i => i.RequestedPerson).ToList();
                        List<string> contactIdC = await _employeeProfile.GetUserId(contactC);
                        List<string> requestedIdC = await _employeeProfile.GetUserId(requestedC);
                        _hVM.SetChanges(changes, await _userInfo.GetUserName(contactIdC, requestedIdC));
                        break;
                    case "TasksQueue":
                        var tasks = await _requestQueue.GetTasks(); 
                        List<int> contactT = tasks.Select(i => i.ContactPerson).ToList();
                        List<int> requestedT = tasks.Select(i => i.RequestedPerson).ToList();
                        List<string> contactIdT = await _employeeProfile.GetUserId(contactT);
                        List<string> requestedIdT = await _employeeProfile.GetUserId(requestedT);
                        _hVM.SetTasks(tasks, await _userInfo.GetUserName(contactIdT, requestedIdT));
                        break;
                    case "IncidentProxy":
                        _hVM.Tabs.Add(new RequestViewModel( ExtractIncident(parameter), "Incident", ExtractAdditionalInfo(parameter)));
                        break;
                    case "ServiceRequestProxy":
                        _hVM.Tabs.Add(new RequestViewModel(ExtractChange(parameter), "Change", ExtractAdditionalInfo(parameter)));
                        break;
                    case "TaskRequestProxy":
                        _hVM.Tabs.Add(new RequestViewModel(ExtractTask(parameter), "Task", ExtractAdditionalInfo(parameter)));
                        break;
                    case "Group":
                        break;

                    default:
                        break;
                }

            }
            catch (Exception e)
            {

            }
        }
        private Incident ExtractIncident(object parameter) =>
            (Incident)((TabContainer)parameter).ServiceRequests;

        private ServiceRequest ExtractChange(object parameter) =>
            (ServiceRequest)((TabContainer)parameter).ServiceRequests;
        private TaskRequest ExtractTask(object parameter) =>
            (TaskRequest)((TabContainer)parameter).ServiceRequests;

        private CommunicationInfo ExtractAdditionalInfo(object parameter) =>
            ((TabContainer)parameter).CommunicationInfo;

        //public List<string> ExtractAllUserID(List<object> requestList)
        //{
        //    foreach (var r in requestList)
        //    {

        //        switch (r.GetType().Name)
        //        {
        //            case "Incident":
        //        }
        //    }
        //}
    }
}
