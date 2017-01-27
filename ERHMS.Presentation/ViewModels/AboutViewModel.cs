using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace ERHMS.Presentation.ViewModels
{
    public class AboutViewModel : ViewModelBase
    {
        public string AppTitle { get; private set; }
        public string Version { get; private set; }
        public string InformationalVersion { get; private set; }

        public RelayCommand PrintLicenseCommand { get; private set; }

        public AboutViewModel()
        {
            Title = "About";
            AppTitle = App.Title;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetVersion().ToString();
            InformationalVersion = assembly.GetInformationalVersion();
            PrintLicenseCommand = new RelayCommand(PrintLicense);
        }

        private void Print(string title, string text)
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                FlowDocument document = new FlowDocument
                {
                    ColumnWidth = dialog.PrintableAreaWidth,
                    PagePadding = new Thickness(48.0),
                    FontFamily = new FontFamily("Consolas"),
                    FontSize = 12.0
                };
                foreach (string line in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    document.Blocks.Add(new Paragraph(new Run(line)));
                }
                dialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, string.Format("{0} - {1}", App.Title, title));
            }
        }

        public void PrintLicense()
        {
            Print("License", App.Current.LicenseFullText);
        }
    }
}
