using ERHMS.DataAccess;
using ERHMS.Presentation.Services;
using System.ComponentModel;
using System.Text;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        protected static DataContext Context
        {
            get { return ServiceLocator.Data.Context; }
            set { ServiceLocator.Data.Context = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            protected set { SetProperty(nameof(Title), ref title, value); }
        }

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

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().ToString());
            if (Title != null)
            {
                builder.AppendFormat(" [{0}]", Title);
            }
            return builder.ToString();
        }
    }
}
