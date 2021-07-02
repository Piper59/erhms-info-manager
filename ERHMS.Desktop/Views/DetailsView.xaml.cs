using ERHMS.Desktop.ViewModels;
using System;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class DetailsView : ItemsControl
    {
        public new DetailsViewModel DataContext
        {
            get { return (DetailsViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public string SharedSizeGroup { get; } = $"Group_{Guid.NewGuid():N}";

        public DetailsView()
        {
            InitializeComponent();
        }
    }
}
