using ERHMS.Domain;
using ERHMS.Presentation.Services;

namespace ERHMS.Presentation.ViewModels
{
    public class JobViewModel : DocumentViewModel
    {
        public Job Job { get; private set; }
        public JobDetailViewModel Detail { get; private set; }
        public JobNoteListViewModel Notes { get; private set; }
        public JobTeamListViewModel Teams { get; private set; }
        public JobResponderListViewModel Responders { get; private set; }
        public JobLocationListViewModel Locations { get; private set; }

        public override bool Dirty
        {
            get { return base.Dirty || Detail.Dirty || Notes.Dirty; }
            protected set { base.Dirty = value; }
        }

        public JobViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Job = job;
            Detail = new JobDetailViewModel(services, job);
            Notes = new JobNoteListViewModel(services, job);
            Teams = new JobTeamListViewModel(services, job);
            Responders = new JobResponderListViewModel(services, job);
            Locations = new JobLocationListViewModel(services, job);
            Title = Detail.Title;
            Detail.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Title))
                {
                    Title = Detail.Title;
                }
            };
        }

        public override void Dispose()
        {
            Detail.Dispose();
            Notes.Dispose();
            Teams.Dispose();
            Responders.Dispose();
            Locations.Dispose();
            base.Dispose();
        }
    }
}
