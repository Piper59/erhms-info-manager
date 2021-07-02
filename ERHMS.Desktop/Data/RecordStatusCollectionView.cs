using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Desktop.Data
{
    public class RecordStatusCollectionView : ListCollectionView<RecordStatusCollectionView.Item>
    {
        public class Item
        {
            public static Item Undeleted { get; } = new Item(RecordStatus.Undeleted);
            public static Item Deleted { get; } = new Item(RecordStatus.Deleted);
            public static Item All { get; } = new Item(null);

            public static IEnumerable<Item> Instances { get; } = new Item[]
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

        public RecordStatusCollectionView()
            : base(Item.Instances.ToList()) { }
    }
}
