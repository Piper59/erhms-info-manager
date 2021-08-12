using ERHMS.Common.Collections;
using ERHMS.Desktop.Services;
using System;
using System.Linq;
using System.Windows;

namespace ERHMS.Desktop.Infrastructure.Services
{
    public class WindowService : IWindowService
    {
        private TypeMap WindowTypeMap { get; } = (TypeMap)Application.Current.FindResource(nameof(WindowTypeMap));

        private bool TryGet(object dataContext, out Window window)
        {
            window = Application.Current.Windows.Cast<Window>()
                .SingleOrDefault(_window => Equals(_window.DataContext, dataContext));
            return window != null;
        }

        private Window Create(object dataContext)
        {
            Window owner = Application.Current.GetActiveWindow();
            Type windowType = WindowTypeMap.Map(dataContext.GetType());
            Window window = (Window)Activator.CreateInstance(windowType);
            window.DataContext = dataContext;
            window.SetOwner(owner);
            return window;
        }

        public void Show(object dataContext)
        {
            if (TryGet(dataContext, out Window window))
            {
                window.Activate();
            }
            else
            {
                window = Create(dataContext);
                window.Show();
                window.Owner = null;
            }
        }

        public void ShowDialog(object dataContext)
        {
            Create(dataContext).ShowDialog();
        }
    }
}
