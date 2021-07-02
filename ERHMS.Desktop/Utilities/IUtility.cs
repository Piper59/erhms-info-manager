using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Utilities
{
    public interface IUtility
    {
        IEnumerable<string> Parameters { get; set; }

        string Invoke();
        Task<string> ExecuteAsync();
    }
}
