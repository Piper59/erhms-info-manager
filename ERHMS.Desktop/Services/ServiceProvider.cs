using ERHMS.Desktop.Dialogs;

namespace ERHMS.Desktop.Services
{
    public static class ServiceProvider
    {
        public delegate IDialogService DialogServiceFactory(DialogInfo info);
        public delegate IProgressService ProgressServiceFactory(string taskName);

        public static DialogServiceFactory GetDialogService { get; set; }
        public static ProgressServiceFactory GetProgressService { get; set; }
    }
}
