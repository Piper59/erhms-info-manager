using System.ServiceModel;

namespace ERHMS.EpiInfo.Communication
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void Ping();

        [OperationContract]
        void RefreshView(string projectPath, string viewName);
    }
}
