namespace ERHMS.Presentation.ViewModels
{
    public class DialogViewModel : ViewModelBase
    {
        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        protected DialogViewModel(IServiceManager services)
            : base(services) { }

        public override void Close()
        {
            Active = false;
        }
    }
}
