using System;
using System.Windows;

namespace ERHMS.Desktop.Resources
{
    public partial class App : ResourceDictionary
    {
        private ResourceDictionary ThemeDictionary => MergedDictionaries[0];

        public App()
        {
            InitializeComponent();
            if (SystemParameters.HighContrast)
            {
                ThemeDictionary.Source = new Uri("/Themes/HighContrast.xaml", UriKind.Relative);
            }
        }
    }
}
