using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Utilities
{
    public interface IUtility
    {
        string Instructions { get; }
        IEnumerable<string> Parameters { get; set; }

        Task<string> ExecuteAsync();
    }
}
