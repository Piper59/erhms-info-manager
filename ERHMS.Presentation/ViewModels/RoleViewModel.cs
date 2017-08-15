using GalaSoft.MvvmLight.Command;
using System;

namespace ERHMS.Presentation.ViewModels
{
    public class RoleViewModel : DialogViewModel
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { Set(nameof(Name), ref name, value); }
        }

        public RelayCommand AddCommand { get; private set; }

        public RoleViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Add a Role";
            AddCommand = new RelayCommand(Add, HasName);
        }

        public bool HasName()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public event EventHandler Added;
        private void OnAdded(EventArgs e)
        {
            Added?.Invoke(this, e);
        }
        private void OnAdded()
        {
            OnAdded(EventArgs.Empty);
        }

        public void Add()
        {
            OnAdded();
            Close();
        }
    }
}
