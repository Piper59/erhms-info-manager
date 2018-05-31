using ERHMS.EpiInfo;

namespace ERHMS.Presentation.Dialogs
{
    public static class FileDialogExtensions
    {
        public static class Filters
        {
            public static readonly string DataSources = GetFilter("Data Sources", Project.FileExtension);
            public static readonly string ZipFiles = GetFilter("ZIP Files", ".zip");
        }

        public static string GetFilter(string type, string extension)
        {
            return string.Format("{0} (*{1})|*{1}", type, extension);
        }
    }
}
