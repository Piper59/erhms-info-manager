using ERHMS.WPF.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.WPF.View
{
    public partial class ResponderSearchView
    {
        public ResponderSearchView()
        {
            InitializeComponent();

            DataContext = new ResponderSearchViewModel();
        }
    }
}
