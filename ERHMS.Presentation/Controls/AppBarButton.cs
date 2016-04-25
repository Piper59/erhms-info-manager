using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace ERHMS.Presentation.Controls
{
    public class AppBarButton : Button
    {
        public Visual Visual 
        {
            get { return (Visual)GetValue(VisualProperty); }
            set { SetValue(VisualProperty, value); }
        }
        public static readonly DependencyProperty VisualProperty = 
            DependencyProperty.Register("Visual", typeof(Visual), typeof(AppBarButton), new UIPropertyMetadata(null));
    }
}
