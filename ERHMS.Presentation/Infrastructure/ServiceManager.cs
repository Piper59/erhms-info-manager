using ERHMS.Presentation.Services;

namespace ERHMS.Presentation
{
    public class ServiceManager : IServiceManager
    {
        public IAppService App { get; set; }
        public IBusyService Busy { get; set; }
        public IDataService Data { get; set; }
        public IDialogService Dialog { get; set; }
        public IDispatchService Dispatch { get; set; }
        public IDocumentService Document { get; set; }
        public IPrintService Print { get; set; }
        public IProcessService Process { get; set; }
        public IStringService String { get; set; }
        public IWrapperService Wrapper { get; set; }
    }
}
