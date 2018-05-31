using ERHMS.Domain;
using ERHMS.Presentation.Commands;
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

        protected AnalysisViewModel(View view)
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
