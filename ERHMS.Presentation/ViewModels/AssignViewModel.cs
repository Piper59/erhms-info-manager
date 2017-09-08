using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class AssignViewModel : DialogViewModel
    {
        public Incident Incident { get; private set; }
        public ICollection<Responder> Responders { get; private set; }
        public ICollection<View> Views { get; private set; }

        private View view;
        public View View
        {
            get { return view; }
            set { Set(nameof(View), ref view, value); }
        }

        public RelayCommand CreateCommand { get; private set; }

        public AssignViewModel(IServiceManager services, Incident incident, IEnumerable<Responder> responders)
            : base(services)
        {
            Title = "Create an Assignment";
            Incident = incident;
            Responders = responders.ToList();
            Views = Context.Views.SelectByIncidentId(incident.IncidentId)
                .OrderBy(view => view.Name)
                .ToList();
            CreateCommand = new RelayCommand(Create, HasView);
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(View))
                {
                    CreateCommand.RaiseCanExecuteChanged();
                }
            };
        }

        public bool HasView()
        {
            return View != null;
        }

        public void Create()
        {
            ICollection<string> responderIds = Context.Assignments.Select()
                .Where(assignment => assignment.ViewId == View.ViewId)
                .Select(assignment => assignment.ResponderId)
                .ToList();
            foreach (Responder responder in Responders.Where(responder => !responderIds.ContainsIgnoreCase(responder.ResponderId)))
            {
                Context.Assignments.Save(new Assignment(true)
                {
                    ViewId = View.ViewId,
                    ResponderId = responder.ResponderId
                });
            }
            MessengerInstance.Send(new RefreshMessage(typeof(Assignment)));
            Close();
        }
    }
}
