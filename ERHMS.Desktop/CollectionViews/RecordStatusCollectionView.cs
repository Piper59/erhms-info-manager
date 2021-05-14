using ERHMS.Common.ComponentModel;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;

namespace ERHMS.Desktop.CollectionViews
{
    public class RecordStatusCollectionView : ListCollectionView
    {
        public class Item : ObservableObject
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

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set { SetProperty(ref selected, value); }
            }

            private Item(RecordStatus? value)
            {
                Value = value;
            }
        }

        public RecordStatus? CurrentValue => ((Item)CurrentItem)?.Value;

        public RecordStatusCollectionView()
            : base(Item.Instances.ToList()) { }
    }
}
