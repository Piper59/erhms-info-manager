using ERHMS.Common.ComponentModel;
using ERHMS.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace ERHMS.Desktop.ViewModels.Shared
{
    public abstract class ProjectInfoCollectionViewModel : ObservableObject
    {
        public class Workers : ProjectInfoCollectionViewModel
        {
            public override CoreProject CoreProject => CoreProject.Worker;
            public override bool CanHaveRecents => false;

            public Workers()
            {
                Initialize();
            }

            private void Initialize()
            {
                if (Configuration.Instance.HasWorkerProjectPath)
                {
                    Current = new ProjectInfoViewModel.Worker(Configuration.Instance.WorkerProjectPath);
                }
                else
                {
                    Current = Empty;
                }
            }

            protected override void Refresh()
            {
                Initialize();
            }
        }

        public class Incidents : ProjectInfoCollectionViewModel
        {
            public override CoreProject CoreProject => CoreProject.Incident;
            public override bool CanHaveRecents => true;

            public Incidents()
            {
                Initialize();
            }

            private void Initialize()
            {
                if (Configuration.Instance.HasIncidentProjectPaths)
                {
                    Current = new ProjectInfoViewModel.Incident(Configuration.Instance.IncidentProjectPath);
                    Recents = Configuration.Instance.IncidentProjectPaths
                        .Skip(1)
                        .Select(path => (ProjectInfoViewModel)new ProjectInfoViewModel.Incident(path))
                        .DefaultIfEmpty(Empty)
                        .ToList();
                }
                else
                {
                    Current = Empty;
                    Recents = new ProjectInfoViewModel[]
                    {
                        Empty
                    };
                }
            }

            protected override void Refresh()
            {
                Initialize();
            }
        }

        public abstract CoreProject CoreProject { get; }
        protected ProjectInfoViewModel Empty => new ProjectInfoViewModel.Empty(CoreProject);

        private ProjectInfoViewModel current;
        public ProjectInfoViewModel Current
        {
            get { return current; }
            protected set { SetProperty(ref current, value); }
        }

        public abstract bool CanHaveRecents { get; }

        private IEnumerable<ProjectInfoViewModel> recents;
        public IEnumerable<ProjectInfoViewModel> Recents
        {
            get { return recents; }
            protected set { SetProperty(ref recents, value); }
        }

        protected ProjectInfoCollectionViewModel()
        {
            WeakEventManager<Configuration, EventArgs>.AddHandler(
                Configuration.Instance,
                nameof(Configuration.Saved),
                Configuration_Saved);
        }

        private void Configuration_Saved(object sender, EventArgs e)
        {
            Refresh();
        }

        protected abstract void Refresh();
    }
}
