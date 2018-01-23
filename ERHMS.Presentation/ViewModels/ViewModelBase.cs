using ERHMS.DataAccess;
using ERHMS.Presentation.Services;
using System;
using System.Text;

namespace ERHMS.Presentation.ViewModels
{
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
        protected IServiceManager Services { get; private set; }

        public DataContext Context
        {
            get { return Services.Data.Context; }
            set { Services.Data.Context = value; }
        }

        private string title;
        public string Title
        {
            get { return title; }
            protected set { SetProperty(nameof(Title), ref title, value); }
        }

        protected ViewModelBase(IServiceManager services)
        {
            Services = services;
            services.Data.ContextChanged += Data_ContextChanged;
        }

        private void Data_ContextChanged(object sender, EventArgs e)
        {
            OnPropertyChanged(nameof(Context));
        }

        public virtual void Dispose()
        {
            Services.Data.ContextChanged -= Data_ContextChanged;
        }

        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(GetType().ToString());
            if (Title != null)
            {
                builder.AppendFormat(" [{0}]", Title);
            }
            return builder.ToString();
        }
    }
}
