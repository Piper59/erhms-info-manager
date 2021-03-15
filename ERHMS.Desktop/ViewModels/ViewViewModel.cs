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
        private static bool IsDisplayable(MetaFieldType fieldType)
        {
            return fieldType == MetaFieldType.GlobalRecordId || fieldType.IsPrintableData();
        }

        public View Value { get; }
        public IReadOnlyCollection<FieldDataRow> Fields { get; private set; }
        public RecordCollectionViewModel Records { get; private set; }

        public ViewViewModel(View value)
        {
            Value = value;
        }

        public async Task InitializeAsync()
        {
            IComparer<FieldDataRow> fieldComparer = new FieldDataRowComparer.ByPageAndTabIndex();
            FieldDataTable fields = await Task.Run(Value.GetFields);
            Fields = fields.Where(field => IsDisplayable(field.FieldType))
                .OrderBy(field => field, fieldComparer)
                .ToList();
            RecordRepository recordRepository = new RecordRepository(Value);
            Records = new RecordCollectionViewModel(await Task.Run(() => recordRepository.Select().ToList()));
        }
    }
}
