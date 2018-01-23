using ERHMS.DataAccess;
using System;

namespace ERHMS.Presentation.Services
{
    public interface IDataService
    {
        DataContext Context { get; set; }

        event EventHandler ContextChanged;
        event EventHandler<RefreshingEventArgs> Refreshing;

        void Refresh(Type type);
    }
}
