namespace ERHMS.Presentation.Services
{
    public interface IServiceManager
    {
        IAppService App { get; }
        IBusyService Busy { get; }
        IDataService Data { get; }
        IDialogService Dialog { get; }
        IDispatchService Dispatch { get; }
        IDocumentService Document { get; }
        IPrintService Print { get; }
        IProcessService Process { get; }
        IStringService String { get; }
        IWrapperService Wrapper { get; }
    }
}
