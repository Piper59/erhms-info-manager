using System.Collections.Generic;

namespace ERHMS.Desktop.Services
{
    public interface IFileDialogService
    {
        string InitialDirectory { get; set; }
        string InitialFileName { get; set; }
        string FileName { get; set; }
        string Filter { get; set; }
        IEnumerable<string> Filters { set; }

        bool? Open();
        bool? Save();
    }
}
