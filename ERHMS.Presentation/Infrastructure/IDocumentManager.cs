using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.Presentation.ViewModels;
using System;

namespace ERHMS.Presentation
{
    public interface IDocumentManager
    {
        ViewModelBase ActiveDocument { get; }

        void OpenDataSource(ProjectInfo dataSource);
        void Show<TViewModel>(Func<TViewModel> constructor, Func<TViewModel, bool> predicate = null)
            where TViewModel : ViewModelBase;
        void ShowDataSources();
        void ShowResponders();
        void ShowResponder(Responder responder);
        void ShowNewResponder();
        void ShowIncidents();
        void ShowIncident(Incident incident);
        void ShowNewIncident();
        void ShowTeam(Team team);
        void ShowLocation(Location location);
        void ShowViews();
        void ShowRecords(Epi.View view);
        void ShowTemplates();
        void ShowAssignments();
        void ShowPgms();
        void ShowCanvases();
        void ShowSettings();
        void ShowSettings(string message, Exception exception = null);
        void ShowLogs();
        void ShowHelp();
        void ShowAbout();
        void Close();
        void Exit();
    }
}
