using ERHMS.Domain;

namespace ERHMS.Presentation.ViewModels
{
    public class TeamViewModel : DocumentViewModel
    {
        public Team Team { get; private set; }
        public TeamDetailViewModel Detail { get; private set; }
        public TeamResponderListViewModel Responders { get; private set; }

        public override bool Dirty
        {
            get { return base.Dirty || Detail.Dirty; }
            protected set { base.Dirty = value; }
        }

        public TeamViewModel(Team team)
        {
            Team = team;
            Detail = new TeamDetailViewModel(team);
            Responders = new TeamResponderListViewModel(team);
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
