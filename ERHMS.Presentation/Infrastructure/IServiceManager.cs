using ERHMS.DataAccess;

namespace ERHMS.Presentation
{
    public interface IServiceManager
    {
        IDocumentManager Documents { get; }
        IDialogManager Dialogs { get; }
        DataContext Context { get; set; }
    }
}
