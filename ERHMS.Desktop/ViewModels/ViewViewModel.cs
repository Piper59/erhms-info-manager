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
            return fieldType == MetaFieldType.GlobalRecordId || fieldType.IsPrintable();
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
            Fields = await Task.Run(() =>
            {
                return Value.GetFields()
                    .Where(field => IsDisplayable(field.FieldType))
                    .OrderBy(field => field, new FieldDataRowComparer.ByTabIndex())
                    .ToList();
            });
            Records = new RecordCollectionViewModel(await Task.Run(() =>
            {
                RecordRepository recordRepository = new RecordRepository(Value);
                return recordRepository.Select().ToList();
            }));
        }
    }
}
