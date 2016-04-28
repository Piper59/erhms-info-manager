using ERHMS.DataAccess;

namespace ERHMS.Presentation.ViewModels
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        public ViewModelLocator Locator
        {
            get { return (ViewModelLocator)App.Current.FindResource("ViewModelLocator"); }
        }

        protected DataContext DataContext
        {
            get { return App.Current.DataContext; }
        }
    }
}
