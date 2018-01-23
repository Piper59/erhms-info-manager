using ERHMS.EpiInfo;
using ERHMS.Presentation.ViewModels;
using System;
using System.Threading.Tasks;

namespace ERHMS.Presentation.Services
{
    public interface IDocumentService
    {
        Task SetContextAsync(ProjectInfo projectInfo);
        TModel Show<TModel>(Func<TModel> constructor)
            where TModel : DocumentViewModel;
        TModel Show<TModel>(Func<TModel, bool> predicate, Func<TModel> constructor)
            where TModel : DocumentViewModel;
        TModel ShowByType<TModel>(Func<TModel> constructor)
            where TModel : DocumentViewModel;
    }
}
