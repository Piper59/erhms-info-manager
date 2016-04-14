using System.Windows;
using ERHMS.WPF.ViewModel;
using Xceed.Wpf.AvalonDock.Layout;
using ERHMS.WPF.View;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using ERHMS.Domain;

namespace ERHMS.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            DataContext = new MainViewModel();

            this.Closing += (s, e) => ViewModelLocator.Cleanup();
            this.Closing += MetroNavigationWindow_Closing;

            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "ShowResponderSearch":
                        {
                            OpenWindow("Responder Search", new ResponderSearchView());
                            break;
                        }
                    case "ShowNewResponder":
                        {
                            OpenWindow("New Responder", new ResponderView());
                            break;
                        }
                    case "ShowNewIncident":
                        {
                            OpenWindow("New Incident", new IncidentView());
                            break;
                        }
                    case "ShowTemplateList":
                        {
                            OpenWindow("Templates", new TemplateListView());
                            break;
                        }
                    case "ShowHelp":
                        {
                            OpenWindow("Help", new HelpView());
                            break;
                        }
                    case "ShowAbout":
                        {
                            OpenWindow("About", new AboutView());
                            break;
                        }
                    case "SelectDataSource":
                        {
                            //winSelectProject.IsOpen = true;
                            break;
                        }
                    case "ExitApplication":
                        {
                            this.Close();
                            break;
                        }
                }
            });

            //used for adding a location
            Messenger.Default.Register<NotificationMessage<Incident>>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "ShowNewLocation":
                        {
                            OpenWindow("New Location", new LocationView(msg.Content));
                            break;
                        }
                }
            });

            //used for editing a responder
            Messenger.Default.Register<NotificationMessage<Responder>>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "ShowEditResponder":
                        {
                            OpenWindow("Edit Responder", new ResponderView(msg.Content));
                            break;
                        }
                }
            });

            //used for editing a location
            Messenger.Default.Register<NotificationMessage<Location>>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "ShowEditLocation":
                        {
                            OpenWindow("Edit Location", new LocationView(msg.Content));
                            break;
                        }
                }
            });

            //used for editing an incident
            Messenger.Default.Register<NotificationMessage<Incident>>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "ShowEditIncident":
                        {
                            OpenWindow("Edit Incident", new IncidentView(msg.Content));
                            break;
                        }
                }
            });

            //used for sending emails to responders
            Messenger.Default.Register<NotificationMessage<Tuple<IList, string, string>>>(this, (msg) =>
            {
                if (msg.Notification == "ComposeEmail")
                {
                    OpenWindow("Edit Responder", new EmailView(msg.Content.Item1, msg.Content.Item2, msg.Content.Item3));
                }
            });

            //used for showing a message
            Messenger.Default.Register<NotificationMessage<string>>(this, (msg) =>
            {
                if (msg.Notification == "ShowSuccessMessage")
                {
                    SuccessToaster.Toast(msg.Content, netoaster.ToasterPosition.ApplicationBottomRight, netoaster.ToasterAnimation.FadeIn);
                }
                else if (msg.Notification == "ShowErrorMessage")
                {
                    ErrorToaster.Toast(msg.Content, netoaster.ToasterPosition.ApplicationBottomRight, netoaster.ToasterAnimation.FadeIn);
                }
            });

            //used to confirm deletion of a record
            Messenger.Default.Register<NotificationMessage<System.Action>>(this, (msg) =>
            {
                switch (msg.Notification)
                {
                    case "ConfirmDeleteResponder":
                        ShowConfirmDialog("Are you sure you want to delete the selected responder(s)?", msg.Content);
                        break;
                    case "ConfirmDeleteAssignment":
                        ShowConfirmDialog("Are you sure you want to delete this form assignment(s)?", msg.Content);
                        break;
                    case "ConfirmDeleteCanvas":
                        ShowConfirmDialog("Are you sure you want to delete this canvas file?", msg.Content);
                        break;
                    case "ConfirmDeleteIncident":
                        ShowConfirmDialog("Are you sure you want to delete this incident?", msg.Content);
                        break;
                    case "ConfirmDeletePgm":
                        ShowConfirmDialog("Are you sure you want to delete this PGM file?", msg.Content);
                        break;
                    case "ConfirmDeleteTemplate":
                        ShowConfirmDialog("Are you sure you want to delete this template form?", msg.Content);
                        break;
                    case "ConfirmDeleteForm":
                        ShowConfirmDialog("Are you sure you want to delete this form?", msg.Content);
                        break;
                    case "ConfirmDeleteLocation":
                        ShowConfirmDialog("Are you sure you want to delete this location?", msg.Content);
                        break;
                    case "ConfirmDeleteRegistration":
                        ShowConfirmDialog("Are you sure you want to delete this registration from the incident?", msg.Content);
                        break;
                }
            });
        }

        private void OpenWindow(string title, object content)
        {
            LayoutDocument newDoc = new LayoutDocument();
            newDoc.Title = title;
            newDoc.Content = content;
            layoutDocumentPane.Children.Add(newDoc);
            layoutDocumentPane.SelectedContentIndex = layoutDocumentPane.ChildrenCount - 1;
        }

        private async void ShowConfirmDialog(string message, Action onConfirm = null)
        {
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Confirm",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };
            MessageDialogResult result;

            result = await this.ShowMessageAsync("Confirm", message,
                MessageDialogStyle.AffirmativeAndNegative, dialogSettings);

            if (result.Equals(MessageDialogResult.Affirmative))
            {
                if (onConfirm != null)
                    onConfirm();
            }
        }

        async void MetroNavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            var dialogSettings = new MetroDialogSettings()
            {
                AffirmativeButtonText = "Quit",
                NegativeButtonText = "Cancel",
                AnimateShow = true,
                AnimateHide = false
            };

            var result = await this.ShowMessageAsync("Confirm?",
                "Are you sure you want to quit ERHMS?",
                MessageDialogStyle.AffirmativeAndNegative, dialogSettings);

            if (result == MessageDialogResult.Affirmative)
            {
                this.Closing -= MetroNavigationWindow_Closing;

                Properties.Settings.Default.WindowState = this.WindowState == System.Windows.WindowState.Maximized ? "Maximized" : "Normal";
                Properties.Settings.Default.Save();

                Application.Current.Shutdown();
            }
            else
            {
                return;
            }
        }
    }
}