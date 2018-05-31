using ERHMS.Presentation.Commands;
using System;

namespace ERHMS.Presentation.ViewModels
{
    public class RoleViewModel : DialogViewModel
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(nameof(Name), ref name, value); }
        }

        public ICommand SaveCommand { get; private set; }

        public RoleViewModel(string verb)
        {
            Title = string.Format("{0} a Role", verb);
            SaveCommand = new Command(Save, CanSave);
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

        public bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public void Save()
        {
            OnSaved();
            Close();
        }
    }
}
