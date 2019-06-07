using ERHMS.Presentation.Services;
using System.Threading;
using System.Windows;

namespace ERHMS.Presentation
{
    public class RefreshingWeakEventManager : WeakEventManager
    {
        private static RefreshingWeakEventManager instance;
        public static RefreshingWeakEventManager Instance
        {
            get
            {
                LazyInitializer.EnsureInitialized(ref instance, () =>
                {
                    RefreshingWeakEventManager manager = new RefreshingWeakEventManager();
                    SetCurrentManager(typeof(RefreshingWeakEventManager), manager);
                    return manager;
                });
                return instance;
            }
        }

        public static void AddListener(object source, IWeakEventListener listener)
        {
            Instance.ProtectedAddListener(source, listener);
        }

        public static void RemoveListener(object source, IWeakEventListener listener)
        {
            Instance.ProtectedRemoveListener(source, listener);
        }

        protected override void StartListening(object source)
        {
            ((IDataService)source).Refreshing += DeliverEvent;
        }

        protected override void StopListening(object source)
        {
            ((IDataService)source).Refreshing -= DeliverEvent;
        }
    }
}
