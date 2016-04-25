using ERHMS.Presentation.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Windows;
using System.Windows.Controls;

namespace ERHMS.Presentation.View
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
