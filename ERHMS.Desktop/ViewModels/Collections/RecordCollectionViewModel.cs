using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Linq;

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

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public RecordCollectionViewModel()
        {
            items = new List<ItemViewModel>();
            Items = new CustomCollectionView<ItemViewModel>(items);
        }

        public void Initialize(IEnumerable<Record> values)
        {
            items.AddRange(values.Select(value => new ItemViewModel(value)));
        }
    }
}
