using ERHMS.Desktop.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Domain;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Desktop.ViewModels.Collections
{
    public class PhaseCollectionViewModel : ViewModel
    {
        public class ItemViewModel : SelectableViewModel
        {
            public static ItemViewModel PreDeployment { get; } = new ItemViewModel(Phase.PreDeployment);
            public static ItemViewModel Deployment { get; } = new ItemViewModel(Phase.Deployment);
            public static ItemViewModel PostDeployment { get; } = new ItemViewModel(Phase.PostDeployment);

            public static IReadOnlyCollection<ItemViewModel> Instances { get; } = new ItemViewModel[]
            {
                PreDeployment,
                Deployment,
                PostDeployment
            };

            public Phase Value { get; }
            public CoreProject CoreProject { get; }
            public IReadOnlyCollection<CoreView> CoreViews { get; }

            private ItemViewModel(Phase value)
            {
                Value = value;
                CoreProject = value.ToCoreProject();
                CoreViews = CoreView.Instances.Where(coreView => coreView.Phase == value).ToList();
            }

            public override int GetHashCode()
            {
                return Value.GetHashCode();
            }

            public override bool Equals(object obj)
            {
                return obj is ItemViewModel item && Value == item.Value;
            }
        }

        private readonly List<ItemViewModel> items;
        public CustomCollectionView<ItemViewModel> Items { get; }

        public PhaseCollectionViewModel()
        {
            items = ItemViewModel.Instances.ToList();
            Items = new CustomCollectionView<ItemViewModel>(items);
        }
    }
}
