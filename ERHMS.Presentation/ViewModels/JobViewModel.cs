using ERHMS.Domain;

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

        public JobViewModel(Job job)
        {
            Job = job;
            Detail = new JobDetailViewModel(job);
            Notes = new JobNoteListViewModel(job);
            Teams = new JobTeamListViewModel(job);
            Responders = new JobResponderListViewModel(job);
            Locations = new JobLocationListViewModel(job);
            Title = Detail.Title;
            Detail.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Title))
                {
                    Title = Detail.Title;
                }
            };
        }
    }
}
