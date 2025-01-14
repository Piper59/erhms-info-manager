﻿using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.Utilities;
using ERHMS.EpiInfo;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ERHMS.Desktop
{
    public static class Integration
    {
        public static async Task StartCoreAsync(Module module, params string[] args)
        {
            await Task.Run(() =>
            {
                using (Process process = module.Start(args))
                {
                    process.WaitForExit(3000);
                }
            });
        }

        public static async Task StartAsync(Module module, params string[] args)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_StartingEpiInfo;
            progress.Delay = TimeSpan.Zero;
            await progress.Run(() =>
            {
                return StartCoreAsync(module, args);
            });
        }

        public static string GetWorkerId(string firstName, string lastName, string emailAddress, string workerId)
        {
            GetWorkerId utility = new GetWorkerId
            {
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                WorkerId = workerId
            };
            return utility.Invoke();
        }
    }
}
