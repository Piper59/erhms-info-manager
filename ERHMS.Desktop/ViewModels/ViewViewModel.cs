using Epi;
using ERHMS.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ViewModel
    {
        public View Value { get; }

        private FieldDataTable fields;
        public IEnumerable<FieldDataRow> Fields => fields;

        public RecordCollectionViewModel Records { get; }

        public ViewViewModel(View value)
        {
            Value = value;
            Records = new RecordCollectionViewModel();
        }

        public async Task InitializeAsync()
        {
            fields = await Task.Run(Value.GetFields);
            RecordRepository repository = new RecordRepository(Value);
            Records.Initialize(await Task.Run(() => repository.Select().ToList()));
        }
    }
}
