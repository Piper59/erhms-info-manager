namespace ERHMS.Desktop.Services
{
    public interface IFileDialogService
    {
        bool? Open(string initialDirectory, string filter, out string fileName);
        bool? Save(string initialDirectory, string initialFileName, string filter, out string fileName);
    }
}
