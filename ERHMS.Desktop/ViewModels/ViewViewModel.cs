using ERHMS.Common;
using ERHMS.Data.Databases;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Infrastructure;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ObservableObject
    {
        private readonly RecordRepository repository;

        public ProjectViewModel Parent { get; }
        public Epi.View View { get; }

        private ICollection<string> fieldNames;
        public ICollection<string> FieldNames
        {
            get { return fieldNames; }
            set { SetProperty(ref fieldNames, value); }
        }

        private ICollection<Record> records;
        public ICollection<Record> Records
        {
            get { return records; }
            set { SetProperty(ref records, value); }
        }

        public Command RefreshCommand { get; }
        public Command GoBackCommand { get; }

        public ViewViewModel(ProjectViewModel parent, Epi.View view)
        {
            repository = new RecordRepository(DatabaseFactory.GetDatabase(view.Project), view);
            Parent = parent;
            View = view;
            RefreshInternal();
            RefreshCommand = new SimpleAsyncCommand(RefreshAsync);
            GoBackCommand = new AsyncCommand(GoBackAsync, CanGoBack, ErrorBehavior.Raise);
        }

        public ViewViewModel(Epi.View view)
            : this(null, view) { }

        private void RefreshInternal()
        {
            // TODO: Handle errors
            fieldNames = View.GetMetadata().GetSortedFieldNames(View.Id, MetaFieldTypeExtensions.IsTextualData).ToList();
            records = repository.Select().ToList();
        }

        public async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingViewTaskName);
            await progress.RunAsync(() =>
            {
                View.LoadFields();
                RefreshInternal();
            });
            OnPropertyChanged(nameof(FieldNames));
            OnPropertyChanged(nameof(Records));
        }

        public bool CanGoBack()
        {
            return Parent != null;
        }

        public async Task GoBackAsync()
        {
            await Parent.RefreshAsync();
            Parent.SelectedViewItem = null;
            MainViewModel.Current.Content = Parent;
        }
    }
}
