using System.ServiceModel;

namespace ERHMS.Sandbox
{
    [ServiceContract]
    public interface IService
    {
        [OperationContract]
        void SayHello(string name);
    }
}
