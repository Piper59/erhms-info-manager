namespace ERHMS.Presentation.Dialogs
{
    public static class FileDialogExtensions
    {
        public static string GetFilter(string type, string extension)
        {
            return string.Format("{0} (*{1})|*{1}", type, extension);
        }
    }
}
