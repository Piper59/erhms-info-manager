using System.ComponentModel;

namespace ERHMS.Presentation
{
    public abstract class ObservableObject : INotifyPropertyChanged
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

        protected bool SetProperty<T>(string name, ref T field, T value)
        {
            if (Equals(value, field))
            {
                return false;
            }
            else
            {
                field = value;
                OnPropertyChanged(name);
                return true;
            }
        }
    }
}
