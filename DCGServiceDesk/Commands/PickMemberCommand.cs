using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCGServiceDesk.Commands
{
    public class PickMemberCommand : ICommand
    {
        private EscalatedRequestViewModel _eVM;
        public PickMemberCommand(EscalatedRequestViewModel eVM) => _eVM = eVM;
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter)
        {
            try
            {
                _eVM.AUsername = parameter.ToString();
                _eVM.MembersVisibility = false;
            }
            catch(Exception e)
            {

            }
        }
    }
}
