using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using DCGServiceDesk.Services;
using DCGServiceDesk.Session.DataGetter;
using DCGServiceDesk.ViewModels;

namespace DCGServiceDesk.Commands
{
    public class CloseRequestCommand : AsyncCommandBase
    {
        private readonly IRequestQueue _requestQueue;
        private readonly IUserInfo _userInfo;
        private readonly IEmployeeProfile _employeeProfile;
        private NotEscalatedViewModel nEVM;
        private Brush Invalid = new SolidColorBrush(Color.FromRgb(255, 0, 0));
        private Brush Valid = new SolidColorBrush(Color.FromRgb(171, 173, 179));
        private List<bool> isFormValid = new List<bool>();

        public CloseRequestCommand(DbInterfaceContainer dbInterfaces, NotEscalatedViewModel nEVM)
        {
            this.nEVM = nEVM;
            _requestQueue = dbInterfaces.RequestQueue;
            _userInfo = dbInterfaces.UserInfo;
            _employeeProfile = dbInterfaces.EmployeeProfile;
        }

        public async override Task ExecuteAsync(object parameter)
        {
            string contactId = await _userInfo.GetUserId(nEVM.CUsername);
            string recipientId = await _userInfo.GetUserId(nEVM.RUsername);
            CheckStatus();
            CheckContactField(contactId);
            CheckRecipientField(recipientId);
            CheckTopicField();
            CheckDescField();
            CheckSolutionField();
            CheckDueField();
            if (!isFormValid.Contains(false))
            {

            }
            else
                isFormValid.Clear();
        }
        private void CheckStatus()
        {
            if (nEVM.CurrentState != nEVM.States.Where(s => s.StateName == "Closed").FirstOrDefault())
            {
                nEVM.StatusValid = Invalid;
                isFormValid.Add(false);
            }
            else
            {
                isFormValid.Add(true);
                nEVM.StatusValid = Valid;
            }

        }
        private void CheckContactField(string contactId)
        {           
            if (contactId != "" && contactId != null)
            {
                nEVM.ContactValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.ContactValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckRecipientField(string recipientId)
        {
            if (recipientId != "" && recipientId != null)
            {
                nEVM.RecipientValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.RecipientValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckTopicField()
        {
            if (nEVM.Topic != "" && nEVM.Topic != null)
            {
                isFormValid.Add(true);
                nEVM.TitleValid = Valid;
            }
            else
            {
                nEVM.TitleValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckDescField()
        {
            if (nEVM.Description != "" && nEVM.Description != null)
            {
                nEVM.DescriptionValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.DescriptionValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckSolutionField()
        {
            if (nEVM.Solution != "" && nEVM.Solution != null)
            {
                nEVM.SolutionValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.SolutionValid = Invalid;
                isFormValid.Add(false);
            }
        }
        private void CheckDueField()
        {
            if (nEVM.CloserDue != null)
            {
                nEVM.CloserDueValid = Valid;
                isFormValid.Add(true);
            }
            else
            {
                nEVM.CloserDueValid = Invalid;
                isFormValid.Add(false);
            }
        }
    }
}
