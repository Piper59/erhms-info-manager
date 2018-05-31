using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Threading.Tasks;
using System.Windows;
using Resx = ERHMS.Presentation.Properties.Resources;

namespace ERHMS.Presentation.Dialogs
{
    public partial class LicenseDialog : CustomDialog
    {
        private static MetroDialogSettings GetSettings()
        {
            return new MetroDialogSettings
            {
                CustomResourceDictionary = (ResourceDictionary)Application.Current.Resources["Accent"]
            };
        }

        public static async Task<bool> ShowAsync(MetroWindow window)
        {
            LicenseDialog dialog = new LicenseDialog(window);
            return await dialog.ShowAsync();
        }

        public MessageDialogResult Result { get; private set; }

        public LicenseDialog(MetroWindow window)
            : base(window, GetSettings())
        {
            InitializeComponent();
            Title = Resx.LicenseTitle;
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

        public async Task<bool> ShowAsync()
        {
            await OwningWindow.ShowMetroDialogAsync(this);
            await WaitUntilUnloadedAsync();
            return Result == MessageDialogResult.Affirmative;
        }
    }
}
