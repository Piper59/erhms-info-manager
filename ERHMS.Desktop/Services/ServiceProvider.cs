namespace ERHMS.Desktop.Services
{
    public static class ServiceProvider
    {
        public delegate IDialogService DialogServiceFactory();
        public delegate IProgressService ProgressServiceFactory(string taskName);

        public static DialogServiceFactory GetDialogService { get; set; }
        public static ProgressServiceFactory GetProgressService { get; set; }
    }
}
