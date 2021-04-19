using ERHMS.Common;
using ERHMS.Desktop.Data;
using ERHMS.EpiInfo.Data;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Desktop.CollectionViews
{
    public class RecordStatusCollectionView : CustomCollectionView<RecordStatusCollectionView.Item>
    {
        public class Item : ObservableObject, ISelectable
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

        public RecordStatusCollectionView()
            : base(Item.Instances.ToList()) { }
    }
}
