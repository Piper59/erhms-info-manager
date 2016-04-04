using System.ServiceModel;

namespace ERHMS.EpiInfo.Communication
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void Ping();

        [OperationContract]
        void RefreshViews(string projectPath);

        [OperationContract]
        void RefreshTemplates();

        [OperationContract]
        void RefreshViewData(string projectPath, string viewName);
    }
}
