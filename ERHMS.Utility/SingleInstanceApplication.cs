using System;
using System.Reflection;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Threading;

namespace ERHMS.Utility
{
    public class SingleInstanceApplication
    {
        private static Mutex GetMutex()
        {
            string name = string.Format("Global\\{0}", Assembly.GetEntryAssembly().GetName().Name);
            bool created;
            MutexSecurity security = new MutexSecurity();
            security.AddAccessRule(new MutexAccessRule(
                new SecurityIdentifier(WellKnownSidType.WorldSid, null),
                MutexRights.FullControl,
                AccessControlType.Allow));
            return new Mutex(false, name, out created, security);
        }

        public Action Action { get; private set; }
        public int Timeout { get; set; }

        public SingleInstanceApplication(Action action, int timeout)
        {
            Action = action;
            Timeout = timeout;
        }

        public SingleInstanceApplication(Action action)
            : this(action, 0)
        { }

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
                            throw new TimeoutException("Timed out waiting to execute single-instance application.");
                        }
                    }
                    catch (AbandonedMutexException)
                    {
                        owned = true;
                    }
                    Action();
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
