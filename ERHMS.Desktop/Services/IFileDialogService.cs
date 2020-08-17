namespace ERHMS.Desktop.Services
{
    public interface IFileDialogService
    {
        string Title { get; set; }
        string InitialDirectory { get; set; }
        string Filter { get; set; }

        bool? Open(out string path);
    }
}
