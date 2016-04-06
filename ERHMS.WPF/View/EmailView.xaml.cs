using ERHMS.WPF.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;

namespace ERHMS.WPF.View
{
    public partial class EmailView
    {
        public EmailView(IList responders, string subject, string body)
        {
            DataContext = new EmailViewModel(responders, subject, body);
            InitializeComponent();
        }
    }
}
