using System;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace ERHMS.Utility
{
    public class SingleInstanceExecuter
    {
        private static Mutex GetMutex()
        {
            string name = string.Format(@"Global\{0}", Assembly.GetEntryAssembly().GetName().Name);
            bool created;
            MutexSecurity security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow));
            return new Mutex(false, name, out created, security);
        }

        public int Timeout { get; set; }

        public SingleInstanceExecuter(int timeout = 0)
        {
            Timeout = timeout;
        }

        public event EventHandler Executing;
        private void OnExecuting(EventArgs e)
        {
            EventHandler handler = Executing;
            if (handler == null)
            {
                return;
            }
            handler(this, e);
        }
        private void OnExecuting()
        {
            OnExecuting(EventArgs.Empty);
        }

        public void Execute()
        {
            using (Mutex mutex = GetMutex())
            {
                bool owned = false;
                try
                {
                    try
                    {
                        owned = mutex.WaitOne(Timeout);
                        if (!owned)
                        {
                            throw new TimeoutException("Timed out while waiting to execute.");
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        owned = true;
                    }
                    OnExecuting();
                }
                finally
                {
                    if (owned)
                    {
                        mutex.ReleaseMutex();
                    }
                }
            }
        }
    }
}
