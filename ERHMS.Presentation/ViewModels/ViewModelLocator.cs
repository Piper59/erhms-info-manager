namespace ERHMS.Presentation.ViewModels
{
    public class ViewModelLocator
    {
        public MainViewModel Main { get; private set; }

        public ViewModelLocator()
        {
            Main = new MainViewModel();
        }
    }
}
