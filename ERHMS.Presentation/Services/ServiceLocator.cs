namespace ERHMS.Presentation.Services
{
    public static class ServiceLocator
    {
        public static IAppService App
        {
            get { return GetInstance<IAppService>(); }
        }

        public static IBusyService Busy
        {
            get { return GetInstance<IBusyService>(); }
        }

        public static IDataService Data
        {
            get { return GetInstance<IDataService>(); }
        }

        public static IDialogService Dialog
        {
            get { return GetInstance<IDialogService>(); }
        }

        public static IDispatcherService Dispatcher
        {
            get { return GetInstance<IDispatcherService>(); }
        }

        public static IDocumentService Document
        {
            get { return GetInstance<IDocumentService>(); }
        }

        public static IPrinterService Printer
        {
            get { return GetInstance<IPrinterService>(); }
        }

        public static IProcessService Process
        {
            get { return GetInstance<IProcessService>(); }
        }

        public static IWrapperService Wrapper
        {
            get { return GetInstance<IWrapperService>(); }
        }

        public static TService GetInstance<TService>()
        {
            return CommonServiceLocator.ServiceLocator.Current.GetInstance<TService>();
        }
    }
}
