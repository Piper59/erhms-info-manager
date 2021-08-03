namespace ERHMS.Desktop.Services
{
    public interface IDirectoryDialogService
    {
        string Directory { get; set; }

        bool? Open();
    }
}
