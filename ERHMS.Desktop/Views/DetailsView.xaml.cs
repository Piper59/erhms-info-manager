using System;
using System.Windows.Controls;

namespace ERHMS.Desktop.Views
{
    public partial class DetailsView : ItemsControl
    {
        public string SharedSizeGroup { get; } = string.Format("Id_{0:N}", Guid.NewGuid());

        public DetailsView()
        {
            InitializeComponent();
        }
    }
}
