using Epi;
using ERHMS.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using Project = ERHMS.EpiInfo.Projects.Project;

namespace ERHMS.Desktop.ViewModels
{
    public class ProjectViewModel : ObservableObject
    {
        public class ViewChildViewModel : ObservableObject
        {
            public View View { get; }
            public string Name { get; }
            public int RecordCount { get; }

            public ViewChildViewModel(View view, ISet<string> tableNames)
            {
                View = view;
                Name = view.Name;
                RecordCount = tableNames.Contains(view.TableName) ? view.GetRecordCount() : 0;
            }
        }

        public Project Project { get; }
        public ICollection<ViewChildViewModel> Views { get; }

        public ProjectViewModel(Project project)
        {
            Project = project;
            ISet<string> tableNames = project.GetTableNameSet();
            Views = project.Views.Cast<View>()
                .OrderBy(view => view.Name, StringComparer.OrdinalIgnoreCase)
                .Select(view => new ViewChildViewModel(view, tableNames))
                .ToList();
        }
    }
}
