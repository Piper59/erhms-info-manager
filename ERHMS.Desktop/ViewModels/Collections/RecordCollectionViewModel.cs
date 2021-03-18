using Epi;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using ERHMS.EpiInfo.Metadata;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class RecordCollectionViewModel : ViewModel
    {
        public class ItemViewModel : SelectableViewModel
        {
            public Record Value { get; }

            public ItemViewModel(Record value)
            {
                Value = value;
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ItemViewModel item && Value.Equals(item.Value);
            }
        }

        private static bool IsDisplayable(MetaFieldType fieldType)
        {
            return fieldType == MetaFieldType.GlobalRecordId || fieldType.IsPrintable();
        }

        public View View { get; }
        public IReadOnlyCollection<FieldDataRow> Fields { get; private set; }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public RecordCollectionViewModel(View view, IEnumerable<Record> values)
        {
            View = view;
            items = new List<ItemViewModel>(values.Select(value => new ItemViewModel(value)));
            Items = new CustomCollectionView<ItemViewModel>(items);
        }

        public async Task InitializeAsync()
        {
            Fields = await Task.Run(() =>
            {
                return View.GetFields()
                    .Where(field => IsDisplayable(field.FieldType))
                    .OrderBy(field => field, new FieldDataRowComparer.ByTabIndex())
                    .ToList();
            });
        }
    }
}
