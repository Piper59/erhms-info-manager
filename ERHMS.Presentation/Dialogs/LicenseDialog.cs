using ERHMS.EpiInfo;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using System;
using System.Windows;

namespace ERHMS.Presentation.Dialogs
{
    public class LicenseDialog
    {
        public MetroWindow Parent { get; private set; }

        public LicenseDialog(MetroWindow parent)
        {
            Parent = parent;
        }

        public event EventHandler Accepted;
        public void OnAccepted(EventArgs e)
        {
            Accepted?.Invoke(this, e);
        }
        public void OnAccepted()
        {
            OnAccepted(EventArgs.Empty);
        }

        public event EventHandler Canceled;
        public void OnCanceled(EventArgs e)
        {
            Canceled?.Invoke(this, e);
        }
        public void OnCanceled()
        {
            OnCanceled(EventArgs.Empty);
        }

        public async void ShowDialogAsync()
        {
            Log.Current.Debug("Showing license");
            MessageDialogResult result = await Parent.ShowMessageAsync(
                string.Format("{0} License", App.Title),
                App.Current.LicenseFullText,
                MessageDialogStyle.AffirmativeAndNegative,
                new MetroDialogSettings
                {
                    AffirmativeButtonText = "Accept",
                    NegativeButtonText = "Cancel",
                    AnimateHide = false,
                    CustomResourceDictionary = (ResourceDictionary)Parent.FindResource("LicenseDialog")
                });
            if (result == MessageDialogResult.Affirmative)
            {
                Log.Current.Debug("License accepted");
                OnAccepted();
            }
            else
            {
                Log.Current.Debug("License not accepted");
                OnCanceled();
            }
        }
    }
}
