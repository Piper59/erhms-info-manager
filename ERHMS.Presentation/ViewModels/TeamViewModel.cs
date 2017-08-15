using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamViewModel : ViewModelBase
    {
        public Team Team { get; private set; }
        public TeamDetailViewModel Detail { get; private set; }
        public TeamResponderListViewModel Responders { get; private set; }

        public override bool Dirty
        {
            get { return base.Dirty || Detail.Dirty; }
            protected set { base.Dirty = value; }
        }

        public TeamViewModel(IServiceManager services, Team team)
            : base(services)
        {
            Team = team;
            Detail = new TeamDetailViewModel(services, team);
            Responders = new TeamResponderListViewModel(services, team);
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
