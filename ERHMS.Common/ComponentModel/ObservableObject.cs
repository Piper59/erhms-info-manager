using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ERHMS.Common.ComponentModel
{
    public abstract class ObservableObject : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        protected void OnPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        protected bool SetProperty<TProperty>(
            ref TProperty field,
            TProperty value,
            [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<TProperty>.Default.Equals(value, field))
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
