using Epi;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace ERHMS.Presentation.ViewModels
{
    public class PgmViewModel : AnalysisViewModel
    {
        public PgmViewModel(DeepLink<View> viewDeepLink)
            : base(viewDeepLink)
        {
            Title = "Create an Analysis";
        }

        public override void Create()
        {
            Pgm pgm = new Pgm
            {
                Name = Name,
                Content = Pgm.GetContentForView(ViewDeepLink.Item)
            };
            DataContext.Project.InsertPgm(pgm);
            if (ViewDeepLink.Incident != null)
            {
                PgmLink pgmLink = DataContext.PgmLinks.Create();
                pgmLink.PgmId = pgm.PgmId;
                pgmLink.IncidentId = ViewDeepLink.Incident.IncidentId;
                DataContext.PgmLinks.Save(pgmLink);
            }
            Messenger.Default.Send(new RefreshMessage<Pgm>());
            DataContext.Project.CollectedData.EnsureDataTablesExist(ViewDeepLink.Item);
            Wrapper wrapper = Analysis.OpenPgm.Create(DataContext.Project.FilePath, pgm.Name, pgm.Content, true);
            wrapper.Event += (sender, e) =>
            {
                if (e.Type == WrapperEventType.PgmSaved)
                {
                    Messenger.Default.Send(new RefreshMessage<Pgm>());
                }
            };
            wrapper.Invoke();
            Active = false;
        }
    }
}
