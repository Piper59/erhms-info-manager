using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class AnalysisViewModel : DialogViewModel
    {
        public View View { get; private set; }

        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(nameof(Name), ref name, value); }
        }

        public ICommand CreateCommand { get; private set; }

        protected AnalysisViewModel(IServiceManager services, View view)
            : base(services)
        {
            View = view;
            CreateCommand = new AsyncCommand(CreateAsync, CanCreate);
        }

        public bool CanCreate()
        {
            return !string.IsNullOrWhiteSpace(Name);
        }

        public abstract Task CreateAsync();
    }
}
