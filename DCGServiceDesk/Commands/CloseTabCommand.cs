using DCGServiceDesk.Controls.Tab.Model;
using DCGServiceDesk.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DCGServiceDesk.Commands
{
    public class CloseTabCommand : ICommand
    {
        private readonly HomeViewModel _hVM;

        public CloseTabCommand(HomeViewModel hVM)
        {
            _hVM = hVM;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            var tab = (ITab)parameter;
            try
            {
                _hVM.CloseTab(tab);
            }
            catch (Exception e)
            {

            }
    }
    }
}
