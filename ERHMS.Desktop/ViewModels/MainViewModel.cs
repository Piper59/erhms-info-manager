﻿using Epi;
using ERHMS.Common.ComponentModel;
using ERHMS.Common.Compression;
using ERHMS.Common.Logging;
using ERHMS.Desktop.Commands;
using ERHMS.Desktop.Properties;
using ERHMS.Desktop.Services;
using ERHMS.Desktop.ViewModels.Wizards;
using ERHMS.Desktop.Wizards;
using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Data;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Module = ERHMS.EpiInfo.Module;
using Settings = ERHMS.Desktop.Properties.Settings;

namespace ERHMS.Desktop.ViewModels
{
    public class MainViewModel : ObservableObject, IAppCommands
    {
        private object content = new HomeViewModel();
        public object Content
        {
            get
            {
                return content;
            }
            set
            {
                Log.Instance.Debug($"Navigating: {value}");
                SetProperty(ref content, value);
            }
        }

        public HelpViewModel Help { get; } = new HelpViewModel();
        public StartViewModel Start { get; } = new StartViewModel();

        public ICommand GoToHomeCommand { get; }
        public ICommand GoToHelpCommand { get; }
        public ICommand GoToStartCommand { get; }
        public ICommand GoToProjectCommand { get; }
        public ICommand GoToCoreProjectCommand { get; }
        public ICommand GoToViewCommand { get; }
        public ICommand GoToCoreViewCommand { get; }
        public ICommand CreateCoreProjectCommand { get; }
        public ICommand OpenCoreProjectCommand { get; }
        public ICommand ImportCoreViewRecordsCommand { get; }
        public ICommand EnterCoreViewRecordsCommand { get; }
        public ICommand OpenPathCommand { get; }
        public ICommand ExportLogsCommand { get; }
        public ICommand StartEpiInfoCommand { get; }
        public ICommand StartCommandPromptCommand { get; }

        public MainViewModel()
        {
            GoToHomeCommand = new SyncCommand(GoToHome);
            GoToHelpCommand = new SyncCommand(GoToHelp);
            GoToStartCommand = new SyncCommand(GoToStart);
            GoToProjectCommand = new AsyncCommand<Project>(GoToProjectAsync, CanGoToProject);
            GoToCoreProjectCommand = new AsyncCommand<CoreProject>(GoToCoreProjectAsync);
            GoToViewCommand = new AsyncCommand<View>(GoToViewAsync, CanGoToView);
            GoToCoreViewCommand = new AsyncCommand<CoreView>(GoToCoreViewAsync);
            CreateCoreProjectCommand = new SyncCommand<CoreProject>(CreateCoreProject);
            OpenCoreProjectCommand = new SyncCommand<CoreProject>(OpenCoreProject);
            ImportCoreViewRecordsCommand = new AsyncCommand<CoreView>(ImportCoreViewRecordsAsync);
            EnterCoreViewRecordsCommand = new AsyncCommand<CoreView>(EnterCoreViewRecordsAsync);
            OpenPathCommand = new SyncCommand<string>(OpenPath);
            ExportLogsCommand = new AsyncCommand(ExportLogsAsync);
            StartEpiInfoCommand = new SyncCommand(StartEpiInfo);
            StartCommandPromptCommand = new SyncCommand(StartCommandPrompt);
        }

        private async Task GoToProjectAsync(Task<Project> task)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_LoadingProject;
            Content = await progress.Run(async () =>
            {
                return await ProjectViewModel.CreateAsync(await task);
            });
        }

        private async Task GoToViewAsync(Task<View> task)
        {
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_LoadingView;
            Content = await progress.Run(async () =>
            {
                return await ViewViewModel.CreateAsync(await task);
            });
        }

        private async Task<View> GetViewAsync(CoreView coreView)
        {
            return await Task.Run(() =>
            {
                string projectPath = Settings.Default.GetProjectPath(coreView.CoreProject);
                Project project = ProjectExtensions.Open(projectPath);
                return project.Views[coreView.Name];
            });
        }

        public void GoToHome()
        {
            if (Content is HomeViewModel)
            {
                return;
            }
            Content = new HomeViewModel();
        }

        public void GoToHelp()
        {
            IWindowService window = ServiceLocator.Resolve<IWindowService>();
            window.Show(Help);
        }

        public void GoToStart()
        {
            Start.Minimized = false;
            Start.Closed = false;
        }

        public bool CanGoToProject(Project project)
        {
            return project != null;
        }

        public async Task GoToProjectAsync(Project project)
        {
            await GoToProjectAsync(Task.FromResult(project));
        }

        public async Task GoToCoreProjectAsync(CoreProject coreProject)
        {
            // TODO: Handle errors
            await GoToProjectAsync(Task.Run(() =>
            {
                string projectPath = Settings.Default.GetProjectPath(coreProject);
                return ProjectExtensions.Open(projectPath);
            }));
        }

        public bool CanGoToView(View view)
        {
            return view != null;
        }

        public async Task GoToViewAsync(View view)
        {
            await GoToViewAsync(Task.FromResult(view));
        }

        public async Task GoToCoreViewAsync(CoreView coreView)
        {
            // TODO: Handle errors
            await GoToViewAsync(GetViewAsync(coreView));
        }

        public void CreateCoreProject(CoreProject coreProject)
        {
            // TODO: Handle errors
            if (coreProject == CoreProject.Worker && Settings.Default.HasWorkerProjectPath)
            {
                // TODO: Confirm
            }
            CreateProjectViewModel wizard = new CreateProjectViewModel(coreProject);
            if (wizard.Run() != true)
            {
                return;
            }
            GoToHome();
        }

        public void OpenCoreProject(CoreProject coreProject)
        {
            // TODO: Handle errors
            if (coreProject == CoreProject.Worker && Settings.Default.HasWorkerProjectPath)
            {
                // TODO: Confirm
            }
            IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
            fileDialog.InitialDirectory = Configuration.Instance.GetProjectsDirectory();
            fileDialog.Filter = Strings.FileDialog_Filter_Projects;
            if (fileDialog.Open() != true)
            {
                return;
            }
            // TODO: Check for core views
            Settings.Default.SetProjectPath(coreProject, fileDialog.FileName);
            Settings.Default.Save();
            GoToHome();
        }

        public async Task ImportCoreViewRecordsAsync(CoreView coreView)
        {
            // TODO: Handle errors
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_LoadingView;
            await progress.Run(async () =>
            {
                View view = await GetViewAsync(coreView);
                Content = await ViewViewModel.CreateAsync(view);
                progress.Lead = Strings.Lead_SynchronizingView;
                await Task.Run(() =>
                {
                    view.Project.CollectedData.SynchronizeViewTree(view);
                });
            });
            await ((ViewViewModel)Content).Records.ImportAsync(false);
        }

        public async Task EnterCoreViewRecordsAsync(CoreView coreView)
        {
            // TODO: Handle errors
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_LoadingView;
            await progress.Run(async () =>
            {
                View view = await GetViewAsync(coreView);
                Content = await ViewViewModel.CreateAsync(view);
                progress.Lead = Strings.Lead_StartingEpiInfo;
                await Integration.StartCoreAsync(
                    Module.Enter,
                    $"/project:{view.Project.FilePath}",
                    $"/view:{view.Name}",
                    "/record:*");
            });
        }

        public void OpenPath(string path)
        {
            Process.Start(path)?.Dispose();
        }

        public async Task ExportLogsAsync()
        {
            IFileDialogService fileDialog = ServiceLocator.Resolve<IFileDialogService>();
            fileDialog.InitialFileName = $"Logs_{DateTime.Now:yyyyMMdd_HHmmss}.zip";
            fileDialog.Filter = Strings.FileDialog_Filter_ZipFiles;
            if (fileDialog.Save() != true)
            {
                return;
            }
            IProgressService progress = ServiceLocator.Resolve<IProgressService>();
            progress.Lead = Strings.Lead_ExportingLogs;
            await progress.Run(() =>
            {
                ZipFileExtensions.CreateFromDirectory(
                    FileAppender.Directory,
                    fileDialog.FileName,
                    $"*{FileAppender.Extension}",
                    FileMode.Create,
                    FileShare.ReadWrite);
            });
        }

        public void StartEpiInfo()
        {
            Module.Menu.Start()?.Dispose();
        }

        public void StartCommandPrompt()
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                WorkingDirectory = AppDomain.CurrentDomain.BaseDirectory,
                FileName = Environment.GetEnvironmentVariable("ComSpec")
            };
            Process.Start(startInfo)?.Dispose();
        }
    }
}
