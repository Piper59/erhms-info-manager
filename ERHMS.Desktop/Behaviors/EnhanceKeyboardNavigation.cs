using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ERHMS.Desktop.Behaviors
{
    public class EnhanceKeyboardNavigation : Behavior<DataGrid>
    {
        private readonly Stack<Behavior> behaviors = new Stack<Behavior>();

        public bool MoveFocusExternallyOnTab { get; set; } = true;
        public bool MoveFocusInternallyOnControlArrow { get; set; } = true;
        public bool RestoreCurrentCellOnFocus { get; set; } = true;
        public bool ToggleSelectionOnSpace { get; set; } = true;

        protected override void OnAttached()
        {
            base.OnAttached();
            if (MoveFocusExternallyOnTab)
            {
                Attach(new MoveFocusExternallyOnTab());
            }
            if (MoveFocusInternallyOnControlArrow)
            {
                Attach(new MoveFocusInternallyOnControlArrow());
            }
            if (RestoreCurrentCellOnFocus)
            {
                Attach(new RestoreCurrentCellOnFocus());
            }
            if (ToggleSelectionOnSpace)
            {
                Attach(new ToggleSelectionOnSpace());
            }
        }

        private void Attach(Behavior behavior)
        {
            behavior.Attach(AssociatedObject);
            behaviors.Push(behavior);
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            while (behaviors.Count > 0)
            {
                behaviors.Pop().Detach();
            }
        }
    }
}
