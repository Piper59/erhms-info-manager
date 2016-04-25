using ERHMS.Presentation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ERHMS.Presentation.View
{
    /// <summary>
    /// Interaction logic for ResponseListView.xaml
    /// </summary>
    public partial class FormResponseListView : UserControl
    {
        public FormResponseListView(Epi.View view)
        {
            InitializeComponent();

            DataContext = new FormResponseListViewModel(view);
        }
    }
}
