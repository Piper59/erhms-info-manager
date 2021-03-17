using ERHMS.Common;
using System;
using System.ComponentModel;
using System.Threading;

namespace ERHMS.Desktop.Infrastructure.ViewModels
{
    public abstract class ViewModel : ObservableObject
    {
        protected SynchronizationContext SynchronizationContext { get; }

        protected ViewModel(SynchronizationContext synchronizationContext)
        {
            if (synchronizationContext == null)
            {
                throw new ArgumentNullException(nameof(synchronizationContext));
            }
            SynchronizationContext = synchronizationContext;
        }

        protected ViewModel()
            : this(SynchronizationContext.Current) { }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            SynchronizationContext.Send(_ => base.OnPropertyChanged(e), null);
        }
    }
}
