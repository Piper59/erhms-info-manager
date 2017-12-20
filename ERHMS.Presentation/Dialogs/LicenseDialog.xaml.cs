using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;

namespace ERHMS.Presentation.Dialogs
{
    public partial class LicenseDialog : CustomDialog
    {
        public static async Task<MessageDialogResult> ShowAsync(MetroWindow window)
        {
            LicenseDialog dialog = new LicenseDialog(window);
            return await dialog.ShowAsync();
        }

        public MessageDialogResult Result { get; private set; }

        public LicenseDialog(MetroWindow window)
            : base(window)
        {
            InitializeComponent();
            Resources.MergedDictionaries.Add(App.Current.Accent);
            Title = string.Format("{0} License", App.Title);
        }

        private async void Accept_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageDialogResult.Affirmative;
            await OwningWindow.HideMetroDialogAsync(this);
        }

        private async void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Result = MessageDialogResult.Negative;
            await OwningWindow.HideMetroDialogAsync(this);
        }

        public async Task<MessageDialogResult> ShowAsync()
        {
            await OwningWindow.ShowMetroDialogAsync(this);
            await WaitUntilUnloadedAsync();
            return Result;
        }
    }
}
