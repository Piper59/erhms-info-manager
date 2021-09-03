using ERHMS.Desktop.ViewModels.Utilities;
using ERHMS.Domain.Data;
using ERHMS.EpiInfo.Data;
using System.Windows;

namespace ERHMS.Desktop.Views.Utilities
{
    public partial class GetWorkerIdView : Window
    {
        public new GetWorkerIdViewModel DataContext
        {
            get { return (GetWorkerIdViewModel)base.DataContext; }
            set { base.DataContext = value; }
        }

        public GetWorkerIdView()
        {
            InitializeComponent();
            DataContextChanged += GetWorkerIdView_DataContextChanged;
        }

        private void GetWorkerIdView_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                ((GetWorkerIdViewModel)e.OldValue).Workers.Committed -= Workers_Committed;
            }
            if (e.NewValue != null)
            {
                ((GetWorkerIdViewModel)e.NewValue).Workers.Committed += Workers_Committed;
            }
        }

        private void Workers_Committed(object sender, RecordEventArgs<Worker> e)
        {
            Close();
        }
    }
}
