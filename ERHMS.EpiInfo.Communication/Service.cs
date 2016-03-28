using ERHMS.Utility;
using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ERHMS.EpiInfo.Communication
{
    public class Service : IService
    {
        public static IService GetService()
        {
            Log.Current.DebugFormat("Connecting to service: {0}", Settings.Default.ServiceAddress);
            NetNamedPipeBinding binding = new NetNamedPipeBinding();
            EndpointAddress address = new EndpointAddress(Settings.Default.ServiceAddress);
            ChannelFactory<IService> factory = new ChannelFactory<IService>(binding, address);
            IService service = factory.CreateChannel();
            try
            {
                service.Ping();
                return service;
            }
            catch
            {
                Log.Current.WarnFormat("Failed to connect to service: {0}", Settings.Default.ServiceAddress);
                return null;
            }
        }

        public ServiceHost OpenHost()
        {
            Log.Current.DebugFormat("Opening service host: {0}", Settings.Default.ServiceAddress);
            ServiceHost host = new ServiceHost(this, new Uri(Settings.Default.ServiceAddress));
            ServiceBehaviorAttribute behavior = host.Description.Behaviors.Find<ServiceBehaviorAttribute>();
            behavior.InstanceContextMode = InstanceContextMode.Single;
#if DEBUG
            host.Description.Behaviors.Add(new ServiceMetadataBehavior());
            host.AddServiceEndpoint(ServiceMetadataBehavior.MexContractName, MetadataExchangeBindings.CreateMexNamedPipeBinding(), "mex");
#endif
            host.AddServiceEndpoint(typeof(IService), new NetNamedPipeBinding(), "");
            try
            {
                host.Open();
                return host;
            }
            catch
            {
                Log.Current.WarnFormat("Failed to open service host: {0}", Settings.Default.ServiceAddress);
                return null;
            }
        }

        public void Ping() { }

        public event EventHandler<ViewEventArgs> RefreshingView;

        private void OnRefreshingView(ViewEventArgs e)
        {
            Log.Current.DebugFormat("Refreshing view: {0}, {1}", e.ProjectPath, e.ViewName);
            EventHandler<ViewEventArgs> handler = RefreshingView;
            if (handler == null)
            {
                return;
            }
            try
            {
                handler(this, e);
            }
            catch (Exception ex)
            {
                Log.Current.Warn(string.Format("Failed to refresh view: {0}, {1}", e.ProjectPath, e.ViewName), ex);
            }
        }

        private void OnRefreshingView(string projectPath, string viewName)
        {
            OnRefreshingView(new ViewEventArgs(projectPath, viewName));
        }

        public void RefreshView(string projectPath, string viewName)
        {
            OnRefreshingView(projectPath, viewName);
        }
    }
}
