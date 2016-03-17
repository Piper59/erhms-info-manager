using ERHMS.EpiInfo;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ERHMS.Sandbox
{
    public class SayingHelloEventArgs : EventArgs
    {
        public string Name { get; private set; }

        public SayingHelloEventArgs(string name)
        {
            Name = name;
        }
    }

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Service : IService
    {
        public event EventHandler<SayingHelloEventArgs> SayingHello;

        private void OnSayingHello(SayingHelloEventArgs e)
        {
            EventHandler<SayingHelloEventArgs> handler = SayingHello;
            if (handler == null)
            {
                return;
            }
            handler(this, e);
        }

        private void OnSayingHello(string name)
        {
            OnSayingHello(new SayingHelloEventArgs(name));
        }

        public ServiceHost OpenHost()
        {
            ServiceHost host = new ServiceHost(this, new Uri(Configuration.ServiceAddress));
#if DEBUG
            host.Description.Behaviors.Add(new ServiceMetadataBehavior());
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");
#endif
            host.AddServiceEndpoint(typeof(IService), new NetNamedPipeBinding(), "");
            host.Open();
            return host;
        }

        public void SayHello(string name)
        {
            Log.Current.DebugFormat("Saying hello: {0}", name);
            OnSayingHello(name);
        }
    }
}
