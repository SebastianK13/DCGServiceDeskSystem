using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace DCGServiceDesk.ViewModels
{
    public delegate TViewModel CreateViewModel<TViewModel>() where TViewModel : ViewModelBase;
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        //protected void RaisePropertyChanged(string propertyName)
        //{
        //    if (this.PropertyChanged != null)
        //        this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        //}
    }
}
