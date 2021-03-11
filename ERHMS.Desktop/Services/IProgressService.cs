﻿using System;
using System.Threading;
using System.Threading.Tasks;

namespace ERHMS.Desktop.Services
{
    public interface IProgressService : IProgress<string>
    {
        Task RunAsync(string title, Action action);
        Task RunAsync(string title, Action<CancellationToken> action);
    }
}