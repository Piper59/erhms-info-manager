using ERHMS.Common;
using ERHMS.Desktop.Data;
using ERHMS.Domain;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Desktop.CollectionViews
{
    public class PhaseCollectionView : CustomCollectionView<PhaseCollectionView.Item>
    {
        public class Item : ObservableObject, ISelectable
        {
            public static Item PreDeployment { get; } = new Item(Phase.PreDeployment);
            public static Item Deployment { get; } = new Item(Phase.Deployment);
            public static Item PostDeployment { get; } = new Item(Phase.PostDeployment);

            public static IReadOnlyCollection<Item> Instances { get; } = new Item[]
            {
                PreDeployment,
                Deployment,
                PostDeployment
            };

            public Phase Value { get; }
            public CoreProject CoreProject { get; }
            public IReadOnlyCollection<CoreView> CoreViews { get; }

            private bool selected;
            public bool Selected
            {
                get { return selected; }
                set { SetProperty(ref selected, value); }
            }

            private Item(Phase value)
            {
                Value = value;
                CoreProject = value.ToCoreProject();
                CoreViews = CoreView.Instances.Where(coreView => coreView.Phase == value).ToList();
            }
        }

        public PhaseCollectionView()
            : base(Item.Instances.ToList()) { }
    }
}
