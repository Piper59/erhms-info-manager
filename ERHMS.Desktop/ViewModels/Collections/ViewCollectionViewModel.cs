using Epi;
using ERHMS.Common;
using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class ViewCollectionViewModel : ViewModel
    {
        public class ItemViewModel : SelectableViewModel
        {
            public View Value { get; }
            public int PageCount { get; private set; }
            public int FieldCount { get; private set; }
            public int RecordCount { get; private set; }

            public ItemViewModel(View value)
            {
                Value = value;
            }

            public async Task InitializeAsync()
            {
                await Task.Run(() =>
                {
                    PageCount = Value.Pages.Count;
                    FieldCount = Value.Fields.InputFields.Count;
                    if (Value.DataTableExists())
                    {
                        RecordRepository repository = new RecordRepository(Value);
                        RecordCount = repository.CountByDeleted(false);
                    }
                    else
                    {
                        RecordCount = 0;
                    }
                });
            }

            public override int GetHashCode()
            {
                return HashCodeCalculator.GetHashCode(Value.Project.Id, Value.Id);
            }

            public override bool Equals(object obj)
            {
                return obj is ItemViewModel item
                    && Value.Project.Id == item.Value.Project.Id
                    && Value.Id == item.Value.Id;
            }
        }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public ViewCollectionViewModel()
        {
            items = new List<ItemViewModel>();
            Items = new CustomCollectionView<ItemViewModel>(items);
        }

        public async Task InitializeAsync(IEnumerable<View> values)
        {
            items.AddRange(values.Select(value => new ItemViewModel(value)));
            foreach (ItemViewModel item in items)
            {
                await item.InitializeAsync();
            }
        }
    }
}
