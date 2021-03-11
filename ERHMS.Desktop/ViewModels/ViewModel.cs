using ERHMS.Common;
using System;
using System.ComponentModel;
using System.Threading;

namespace ERHMS.Desktop.ViewModels
{
    public abstract class ViewModel : ObservableObject
    {
        protected SynchronizationContext SynchronizationContext { get; }

        protected ViewModel()
        {
            if (SynchronizationContext.Current == null)
            {
                throw new InvalidOperationException("Current synchronization context cannot be null.");
            }
            SynchronizationContext = SynchronizationContext.Current;
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            SynchronizationContext.Send(_ => base.OnPropertyChanged(e), null);
        }
    }
}
