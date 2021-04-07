using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.Data.Models;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using System;
using System.Collections.Generic;
using System.Text;

namespace DCGServiceDesk.ViewModels
{
    public class QueueViewModel : Tab
    {
        private List<string> requestTypes = new List<string> { "Incidents", "Changes", "Tasks" };
        private readonly TimeZoneInfo _timeZoneInfo;
        public QueueViewModel(RequestInfo requestInfo, 
            DbInterfaceContainer interfaceContainer, 
            HomeViewModel hVM):base(interfaceContainer, hVM)
        {
            _timeZoneInfo = hVM.loggedUser.ZoneInfo;

            if (!requestTypes.Contains(requestInfo.QueueName))
                MixedTypeQueue(requestInfo);
            else
                SingleTypeQueue(requestInfo);

        }

        private object ConvertType(string requestType, object request)
        {
            switch (requestType)
            {
                case "Incidents":
                    return (Incident)request;
                case "Changes":
                    return (ServiceRequest)request;
                case "Tasks":
                    return (TaskRequest)request;
                default:
                    return null;
            }
        }
        private DateTime ConvertAndGetDate(string requestType, object request, string phase)
        {
            switch (requestType)
            {
                case "Incidents":
                    return GetDateFromIM((Incident)request, phase);
                case "Changes":
                    return GetDateFromC((ServiceRequest)request, phase);
                case "Tasks":
                    return GetDateFromT((TaskRequest)request, phase);
                default:
                    return default;
            }
        }
        private void SingleTypeQueue(RequestInfo requestInfo)
        {
            List<TabContainer> wi = new List<TabContainer>();
            for (int i = 0; i < requestInfo.Requests.Count; i++)
            {
                wi.Add(new TabContainer
                {
                    CommunicationInfo = requestInfo.Infos[i],
                    RequestVisibility = true,
                    ServiceRequests = ConvertType(requestInfo.RequestTypes[0], requestInfo.Requests[i]),
                    StartDate = ConvertAndGetDate(requestInfo.RequestTypes[0], requestInfo.Requests[i], "start"),
                    DeadlineDate = ConvertAndGetDate(requestInfo.RequestTypes[0], requestInfo.Requests[i], "end")
                });
            }

            WorkspaceInfo = wi;
            Label = requestInfo.QueueName;
            QueueType = requestInfo.QueueType;
        }
        private void MixedTypeQueue(RequestInfo requestInfo)
        {
            List<TabContainer> wi = new List<TabContainer>();
            for (int i = 0; i < requestInfo.Requests.Count; i++)
            {
                wi.Add(new TabContainer
                {
                    CommunicationInfo = requestInfo.Infos[i],
                    RequestVisibility = true,
                    ServiceRequests = ConvertType(requestInfo.RequestTypes[i], requestInfo.Requests[i]),
                    RequestType = requestInfo.RequestTypes[i],
                    StartDate = ConvertAndGetDate(requestInfo.RequestTypes[i], requestInfo.Requests[i], "start"),
                    DeadlineDate = ConvertAndGetDate(requestInfo.RequestTypes[i], requestInfo.Requests[i], "end")
                });
            }
            WorkspaceInfo = wi;
            Label = requestInfo.QueueName;
            QueueType = requestInfo.QueueType;
        }
        private DateTime GetDateFromIM(Incident im, string phase)
        {
            if (phase == "start")
                return TimeZoneInfo.ConvertTime(im.History.ActiveStatus.CreateDate, _timeZoneInfo);
            else if (phase == "end")
                return TimeZoneInfo.ConvertTime(im.History.ActiveStatus.DueTime, _timeZoneInfo);
            else
                return default(DateTime);
        }
        private DateTime GetDateFromC(ServiceRequest c, string phase)
        {
            if (phase == "start")
                return TimeZoneInfo.ConvertTime(c.History.ActiveStatus.CreateDate, _timeZoneInfo);
            else if (phase == "end")
                return TimeZoneInfo.ConvertTime(c.History.ActiveStatus.DueTime, _timeZoneInfo);
            else
                return default(DateTime);
        }
        private DateTime GetDateFromT(TaskRequest t, string phase)
        {
            if (phase == "start")
                return TimeZoneInfo.ConvertTime(t.History.ActiveStatus.CreateDate, _timeZoneInfo);
            else if (phase == "end")
                return TimeZoneInfo.ConvertTime(t.History.ActiveStatus.DueTime, _timeZoneInfo);
            else
                return default(DateTime);
        }
    }
}
