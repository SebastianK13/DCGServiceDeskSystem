using System;
using System.Collections.Generic;
using System.Text;
using DCGServiceDesk.ViewModels;

namespace DCGServiceDesk.Session.Navigation
{
    class ViewForwarding : IViewForwarding
    {
        private ViewModelBase _activeViewModel;
        public ViewModelBase ActiveViewModel 
        {
            get
            {
                return _activeViewModel;
            }
            set
            {
                _activeViewModel = value;
                StateChanged?.Invoke();
            }
        }

        public event Action StateChanged;
    }
}
