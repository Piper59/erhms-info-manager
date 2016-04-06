using System.Windows;
using GalaSoft.MvvmLight.Threading;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using Epi;
using System.Windows.Threading;
using System;
using ERHMS.DataAccess;

namespace ERHMS.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }

        public static DataContext GetDataContext()
        {
            throw new NotImplementedException();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
        }

        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (e.Exception != null)
            {
                Logger.Log(e.Exception.ToString());
            }
        }
    }
}
