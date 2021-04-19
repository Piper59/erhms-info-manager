namespace ERHMS.Desktop.Services
{
    public interface IFileDialogService
    {
        string InitialDirectory { get; set; }
        string FileName { get; set; }
        string Filter { get; set; }

        bool? Open();
        bool? Save();
    }
}
