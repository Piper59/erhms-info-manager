using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Utilities
{
    public interface IUtility
    {
        string Instructions { get; }

        IEnumerable<string> GetParameters();
        void ParseParameters(IReadOnlyList<string> parameters);
        Task<string> ExecuteAsync();
    }
}
