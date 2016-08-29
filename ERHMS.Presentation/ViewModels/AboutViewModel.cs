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

        public RelayCommand PrintCommand { get; private set; }

        public AboutViewModel()
        {
            Title = "About";
            AppTitle = App.Title;
            Assembly assembly = Assembly.GetExecutingAssembly();
            Version = assembly.GetVersion();
            InformationalVersion = assembly.GetInformationalVersion();
            PrintCommand = new RelayCommand(Print);
        }

        public void Print()
        {
            PrintDialog dialog = new PrintDialog();
            if (dialog.ShowDialog().GetValueOrDefault())
            {
                FlowDocument document = new FlowDocument
                {
                    ColumnWidth = dialog.PrintableAreaWidth,
                    PagePadding = new Thickness(48.0),
                    FontFamily = new FontFamily("Segoe UI"),
                    FontSize = 14.0
                };
                foreach (string line in App.Current.TermsOfUse.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
                {
                    document.Blocks.Add(new Paragraph(new Run(line)));
                }
                dialog.PrintDocument(((IDocumentPaginatorSource)document).DocumentPaginator, string.Format("{0} - Terms of Use", App.Title));
            }
        }
    }
}
