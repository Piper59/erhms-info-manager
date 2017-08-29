using ERHMS.DataAccess;

namespace ERHMS.Presentation
{
    public interface IServiceManager
    {
        IDispatcher Dispatcher { get; }
        IDocumentManager Documents { get; }
        IDialogManager Dialogs { get; }
        DataContext Context { get; set; }
    }
}
