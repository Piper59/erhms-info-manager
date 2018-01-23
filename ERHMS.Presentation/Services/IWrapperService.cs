using ERHMS.EpiInfo.Wrappers;
using System.Threading.Tasks;

namespace ERHMS.Presentation.Services
{
    public interface IWrapperService
    {
        Task InvokeAsync(Wrapper wrapper);
    }
}
