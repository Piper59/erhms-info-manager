using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ERHMS.Desktop.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
        protected void OnPropertyChanged(string name)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(name));
        }

        protected bool Set<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(value, field))
            {
                return false;
            }
            else
            {
                field = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }
    }
}
