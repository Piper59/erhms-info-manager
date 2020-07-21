using ERHMS.Desktop.Commands;
using ERHMS.Desktop.ViewModels;
using ERHMS.Desktop.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ResXResources = ERHMS.Desktop.Properties.Resources;

namespace ERHMS.Desktop.Utilities
{
    public abstract class Utility : ViewModel
    {
        public static readonly IDictionary<string, Type> Types = typeof(Utility).Assembly.GetTypes()
            .Where(type => type.IsSubclassOf(typeof(Utility)) && !type.IsAbstract)
            .ToDictionary(type => type.Name, type => type, StringComparer.OrdinalIgnoreCase);

        public static async Task RunAsync(string typeName, IList<string> args)
        {
            if (!Types.TryGetValue(typeName, out Type type))
            {
                throw new ArgumentException($"The utility '{typeName}' could not be found.");
            }
            ConstructorInfo constructor = type.GetConstructors().Single();
            object[] parameters = GetParameters(constructor, args).ToArray();
            Utility utility = (Utility)constructor.Invoke(parameters);
            UtilityView window = null;
            if (utility.LongRunning)
            {
                window = new UtilityView
                {
                    DataContext = utility
                };
                window.Show();
            }
            string result = await utility.RunAsync();
            if (result != null)
            {
                MessageBox.Show(result, ResXResources.AppTitle);
            }
            if (!utility.LongRunning)
            {
                Application.Current.Shutdown();
            }
        }

        private static IEnumerable<object> GetParameters(ConstructorInfo constructor, IList<string> args)
        {
            IList<ParameterInfo> parameters = constructor.GetParameters();
            if (args.Count != parameters.Count)
            {
                StringBuilder message = new StringBuilder();
                message.Append($"The '{constructor.DeclaringType.Name}' utility must be invoked with ");
                if (parameters.Count == 0)
                {
                    message.Append("no arguments");
                }
                else
                {
                    message.Append("the following arguments: ");
                    message.Append(string.Join(", ", parameters.Select(parameter => parameter.Name)));
                }
                message.Append(".");
                throw new ArgumentException(message.ToString());
            }
            for (int index = 0; index < parameters.Count; index++)
            {
                yield return Convert.ChangeType(args[index], parameters[index].ParameterType);
            }
        }

        protected abstract bool LongRunning { get; }

        private bool done;
        public bool Done
        {
            get { return done; }
            private set { SetProperty(ref done, value); }
        }

        public IProgress<string> Progress { get; set; }

        public Command CloseCommand { get; }

        protected Utility()
        {
            CloseCommand = new SyncCommand(Close, CanClose, ErrorBehavior.Throw);
        }

        public event EventHandler CloseRequested;

        protected virtual void OnCloseRequested()
        {
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        protected override void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.PropertyName)
            {
                case nameof(Done):
                    Command.OnCanExecuteChanged();
                    break;
            }
        }

        protected abstract Task<string> RunCoreAsync();

        public async Task<string> RunAsync()
        {
            Log.Default.Debug($"Running: {this}");
            Progress?.Report("Running");
            string result;
            try
            {
                result = await RunCoreAsync();
                Progress?.Report("Completed");
            }
            catch (Exception ex)
            {
                Log.Default.Error(ex);
                result = $"An error occurred while running the '{this}' utility.";
                Progress?.Report(ex.ToString());
                Progress?.Report("Completed with errors");
            }
            Log.Default.Debug($"Completed: {this}");
            Done = true;
            return result;
        }

        private bool CanClose()
        {
            return Done;
        }

        private void Close()
        {
            OnCloseRequested();
        }

        public override string ToString()
        {
            return GetType().Name;
        }
    }
}
