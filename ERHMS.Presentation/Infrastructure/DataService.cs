using ERHMS.DataAccess;
using ERHMS.Presentation.Services;
using System;

namespace ERHMS.Presentation
{
    public class DataService : IDataService
    {
        public DataContext Context { get; set; }

        public event EventHandler<RefreshingEventArgs> Refreshing;
        private void OnRefreshing(RefreshingEventArgs e)
        {
            Refreshing?.Invoke(this, e);
        }
        private void OnRefreshing(Type type)
        {
            OnRefreshing(new RefreshingEventArgs(type));
        }

        public void Refresh(Type type)
        {
            OnRefreshing(type);
        }
    }
}
