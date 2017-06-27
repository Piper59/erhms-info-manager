﻿using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;

namespace ERHMS.Presentation.Dialogs
{
    public partial class WelcomeDialog : CustomDialog
    {
        public static async Task ShowAsync(MetroWindow window)
        {
            WelcomeDialog dialog = new WelcomeDialog(window);
            await dialog.ShowAsync();
        }

        public WelcomeDialog(MetroWindow window)
            : base(window)
        {
            InitializeComponent();
            Resources.MergedDictionaries.Add(App.Current.Accent);
            Title = "Welcome";
        }

        private async void OK_Click(object sender, RoutedEventArgs e)
        {
            await OwningWindow.HideMetroDialogAsync(this);
        }

        public async Task ShowAsync()
        {
            await OwningWindow.ShowMetroDialogAsync(this, new MetroDialogSettings
            {
                AnimateHide = false
            });
            await WaitUntilUnloadedAsync();
        }

        private void UserManual_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
        }
    }
}
