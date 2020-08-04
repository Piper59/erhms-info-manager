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
using View = Epi.View;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ObservableObject
    {
        private readonly RecordRepository repository;

        public View View { get; }

        private ICollection<string> propertyNames;
        public ICollection<string> PropertyNames
        {
            get { return propertyNames; }
            set { SetProperty(ref propertyNames, value); }
        }

        private ICollection<Record> records;
        public ICollection<Record> Records
        {
            get { return records; }
            set { SetProperty(ref records, value); }
        }

        public Command RefreshCommand { get; }

        public ViewViewModel(View view)
        {
            View = view;
            IDatabase database = DatabaseFactory.GetDatabase(view.Project);
            repository = new RecordRepository(database, view);
            RefreshInternal();
            RefreshCommand = new SimpleAsyncCommand(RefreshAsync);
        }

        private void RefreshInternal()
        {
            propertyNames = View.GetMetadata().GetSortedFieldNames(View.Id, MetaFieldTypeExtensions.IsTextualData).ToList();
            // TODO: Filter out deleted records
            records = repository.Select().ToList();
        }

        private async Task RefreshAsync()
        {
            IProgressService progress = ServiceProvider.GetProgressService(Resources.RefreshingViewTaskName);
            await progress.RunAsync(RefreshInternal);
            OnPropertyChanged(nameof(PropertyNames));
            OnPropertyChanged(nameof(Records));
        }
    }
}
