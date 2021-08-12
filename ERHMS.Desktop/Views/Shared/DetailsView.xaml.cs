using ERHMS.Desktop.ViewModels.Shared;
using System;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views.Shared
{
    public partial class DetailsView : ItemsControl
    {
        public new DetailsViewModel DataContext
        {
            get { return (DetailsViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public string KeyGroup { get; } = $"Group_{Guid.NewGuid():N}";

        public DetailsView()
        {
            InitializeComponent();
        }
    }
}
