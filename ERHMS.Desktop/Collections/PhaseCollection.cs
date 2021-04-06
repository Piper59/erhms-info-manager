using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Domain;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Desktop.Collections
{
    public class PhaseCollection : CustomCollectionView<PhaseCollection.Item>
    {
        public class Item : SelectableViewModel
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

            private Item(Phase value)
            {
                Value = value;
                CoreProject = value.ToCoreProject();
                CoreViews = CoreView.Instances.Where(coreView => coreView.Phase == value).ToList();
            }
        }

        public PhaseCollection()
            : base(Item.Instances.ToList()) { }
    }
}
