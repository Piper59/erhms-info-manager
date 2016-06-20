using System.Windows.Media;
using System.Windows.Controls;

namespace ERHMS.Presentation.Views
{
    public partial class HelpView : UserControl
    {
        public static readonly Geometry ArrowLineCap = Geometry.Parse("M0,0 L-5,10 L20,0 L-5,-10 Z");

        public HelpView()
        {
            InitializeComponent();
        }
    }
}
