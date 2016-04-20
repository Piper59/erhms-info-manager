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

        private void OnEvent(EventHandler handler, string message)
        {
            Log.Current.Debug(message);
            if (handler == null)
            {
                return;
            }
            try
            {
                handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                Log.Current.Warn(message, ex);
            }
        }

        private void OnEvent<TEventArgs>(EventHandler<TEventArgs> handler, TEventArgs e, string message) where TEventArgs : EventArgs
        {
            Log.Current.DebugFormat("{0}: {1}", message, e);
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
                Log.Current.Warn(string.Format("{0} failed: {1}", message, e), ex);
            }
        }

        public event EventHandler<ProjectEventArgs> RefreshingViews;

        public void RefreshViews(string projectPath, string viewName, string tag)
        {
            OnEvent(RefreshingViews, new ViewEventArgs(projectPath, viewName, tag), "Refreshing views");
        }

        public event EventHandler RefreshingTemplates;

        public void RefreshTemplates()
        {
            OnEvent(RefreshingTemplates, "Refreshing templates");
        }

        public event EventHandler<ViewEventArgs> RefreshingViewData;

        public void RefreshViewData(string projectPath, string viewName)
        {
            OnEvent(RefreshingViewData, new ViewEventArgs(projectPath, viewName), "Refreshing view data");
        }

        public event EventHandler<RecordEventArgs> RefreshingRecordData;

        public void RefreshRecordData(string projectPath, string viewName, string globalRecordId)
        {
            OnEvent(RefreshingRecordData, new RecordEventArgs(projectPath, viewName, globalRecordId), "Refreshing record data");
        }
    }
}
