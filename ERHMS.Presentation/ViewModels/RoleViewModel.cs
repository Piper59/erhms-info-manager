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

        public RelayCommand SaveCommand { get; private set; }

        public RoleViewModel(IServiceManager services, string verb)
            : base(services)
        {
            Title = string.Format("{0} a Role", verb);
            SaveCommand = new RelayCommand(Save, HasName);
        }

        public bool HasName()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public event EventHandler Saved;
        private void OnSaved(EventArgs e)
        {
            Saved?.Invoke(this, e);
        }
        private void OnSaved()
        {
            OnSaved(EventArgs.Empty);
        }

        public void Save()
        {
            OnSaved();
            Close();
        }
    }
}
