using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class PrepopulateViewModel : ViewModelBase
    {
        private bool active;
        public bool Active
        {
            get { return active; }
            set { Set(nameof(Active), ref active, value); }
        }

        public View View { get; private set; }

        private ICollection<Responder> responders;
        public ICollection<Responder> Responders
        {
            get { return responders; }
            set { Set(nameof(Responders), ref responders, value); }
        }

        private Responder responder;
        public Responder Responder
        {
            get { return responder; }
            set { Set(nameof(Responder), ref responder, value); }
        }

        public RelayCommand ContinueCommand { get; private set; }
        public RelayCommand CancelCommand { get; private set; }

        public PrepopulateViewModel(DeepLink<View> viewDeepLink)
        {
            View = viewDeepLink.Item;
            IEnumerable<Responder> responders;
            if (viewDeepLink.Incident == null)
            {
                responders = DataContext.Responders.SelectUndeleted();
            }
            else
            {
                responders = DataContext.Responders.SelectByIncidentId(viewDeepLink.Incident.IncidentId);
            }
            Responders = responders.OrderBy(responder => responder.FullName).ToList();
            ContinueCommand = new RelayCommand(Continue);
            CancelCommand = new RelayCommand(Cancel);
        }

        public void Continue()
        {
            object record = null;
            if (Responder != null)
            {
                record = new
                {
                    ResponderId = Responder.ResponderId
                };
            }
            DataContext.Project.CollectedData.EnsureDataTablesExist(View);
            Enter.OpenNewRecord.Create(DataContext.Project.FilePath, View.Name, record).Invoke();
            Active = false;
        }

        public void Cancel()
        {
            Active = false;
        }
    }
}
