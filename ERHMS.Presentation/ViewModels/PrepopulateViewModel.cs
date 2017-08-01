using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class PrepopulateViewModel : DialogViewModel
    {
        public View View { get; private set; }
        public ICollection<Responder> Responders { get; private set; }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { Set(nameof(Responder), ref responder, value); }
        }

        private RelayCommand continueCommand;
        public ICommand ContinueCommand
        {
            get { return continueCommand ?? (continueCommand = new RelayCommand(Continue)); }
        }

        public PrepopulateViewModel(IServiceManager services, View view)
            : base(services)
        {
            Title = "Prepopulate Responder ID Field";
            View = view;
            IEnumerable<Responder> responders;
            if (view.Incident == null)
            {
                responders = Context.Responders.SelectUndeleted();
            }
            else
            {
                responders = Context.Rosters.SelectUndeletedByIncidentId(view.Incident.IncidentId).Select(roster => roster.Responder);
            }
            Responders = responders.OrderBy(responder => responder.FullName).ToList();
        }

        public void Continue()
        {
            object record = null;
            if (Responder != null)
            {
                record = new
                {
                    ResponderID = Responder.ResponderId
                };
            }
            Context.Project.CollectedData.EnsureDataTablesExist(View.ViewId);
            Enter.OpenNewRecord.Create(Context.Project.FilePath, View.Name, record).Invoke();
            Close();
        }
    }
}
