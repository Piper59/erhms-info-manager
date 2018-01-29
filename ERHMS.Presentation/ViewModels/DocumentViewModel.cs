using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using ERHMS.Utility;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class DocumentViewModel : ViewModelBase
    {
        private bool active;
        public bool Active
        {
            get { return active; }
            set { SetProperty(nameof(Active), ref active, value); }
        }

        private bool dirty;
        public virtual bool Dirty
        {
            get { return dirty; }
            protected set { SetProperty(nameof(Dirty), ref dirty, value); }
        }

        public ICommand CloseCommand { get; private set; }

        protected DocumentViewModel(IServiceManager services)
            : base(services)
        {
            CloseCommand = new AsyncCommand(CloseAsync);
            PropertyChanged += (sender, e) =>
            {
                if (GetType().GetProperty(e.PropertyName).HasCustomAttribute<DirtyCheckAttribute>())
                {
                    Dirty = true;
                }
            };
        }

        public event EventHandler Closed;
        protected void OnClosed(EventArgs e)
        {
            Closed?.Invoke(this, e);
        }
        protected void OnClosed()
        {
            OnClosed(EventArgs.Empty);
        }

        protected void AddDirtyCheck(INotifyPropertyChanged model)
        {
            model.PropertyChanged += (sender, e) =>
            {
                Dirty = true;
            };
        }

        public async Task CloseAsync(bool confirm)
        {
            if (Dirty && confirm)
            {
                string message = string.Format("There may be unsaved changes. Are you sure you want to close {0}?", Title);
                if (await Services.Dialog.ConfirmAsync(message, "Close"))
                {
                    OnClosed();
                }
            }
            else
            {
                OnClosed();
            }
        }

        public async Task CloseAsync()
        {
            await CloseAsync(true);
        }
    }
}
