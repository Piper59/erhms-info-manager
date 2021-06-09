using System;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class DetailsView : ItemsControl
    {
        public string SharedSizeGroup { get; } = $"Group_{Guid.NewGuid():N}";

        public DetailsView()
        {
            InitializeComponent();
        }
    }
}
