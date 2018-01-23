using ERHMS.DataAccess;
using ERHMS.Presentation.Services;
using System;

namespace ERHMS.Presentation
{
    public class DataService : IDataService
    {
        private DataContext context;
        public DataContext Context
        {
            get
            {
                return context;
            }
            set
            {
                context = value;
                OnContextChanged();
            }
        }

        public event EventHandler ContextChanged;
        private void OnContextChanged(EventArgs e)
        {
            ContextChanged?.Invoke(this, e);
        }
        private void OnContextChanged()
        {
            OnContextChanged(EventArgs.Empty);
        }

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
