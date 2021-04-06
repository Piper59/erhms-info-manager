using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Desktop.Collections
{
    public class RecordStatusCollection : CustomCollectionView<RecordStatusCollection.Item>
    {
        public class Item : SelectableViewModel
        {
            public static Item Undeleted { get; } = new Item(RecordStatus.Undeleted);
            public static Item Deleted { get; } = new Item(RecordStatus.Deleted);
            public static Item All { get; } = new Item(null);

            public static IReadOnlyCollection<Item> Instances { get; } = new Item[]
            {
                Undeleted,
                Deleted,
                All
            };

            public RecordStatus? Value { get; }

            private Item(RecordStatus? value)
            {
                Value = value;
            }
        }

        public RecordStatusCollection()
            : base(Item.Instances.ToList()) { }
    }
}
