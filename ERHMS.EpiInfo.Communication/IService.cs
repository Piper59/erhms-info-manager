using System.ServiceModel;

namespace ERHMS.EpiInfo.Communication
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void Ping();

        [OperationContract]
        void OnViewAdded(string projectPath, string viewName, string tag = null);

        [OperationContract]
        void OnViewDataImported(string projectPath, string viewName, string tag = null);

        [OperationContract]
        void OnRecordSaved(string projectPath, string viewName, string globalRecordId, string tag = null);

        [OperationContract]
        void OnTemplateAdded(string templatePath, string tag = null);

        [OperationContract]
        void OnCanvasSaved(string projectPath, int canvasId, string canvasPath, string tag = null);
    }
}
