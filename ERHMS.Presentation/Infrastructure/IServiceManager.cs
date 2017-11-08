using ERHMS.DataAccess;
using System;

namespace ERHMS.Presentation
{
    public interface IServiceManager
    {
        IDispatcher Dispatcher { get; }
        IDocumentManager Documents { get; }
        IDialogManager Dialogs { get; }
        DataContext Context { get; set; }

        event EventHandler ContextChanged;
    }
}
