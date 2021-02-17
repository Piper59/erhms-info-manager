using Microsoft.Xaml.Behaviors;
using System.Collections.Generic;
using System.Windows.Controls;

namespace ERHMS.Desktop.Behaviors
{
    public class EnhanceKeyboardNavigation : Behavior<DataGrid>
    {
        private readonly Behavior moveFocusExternallyOnTab = new MoveFocusExternallyOnTab();
        private readonly Behavior moveFocusInternallyOnControlArrow = new MoveFocusInternallyOnControlArrow();
        private readonly Behavior restoreCurrentCellOnFocus = new RestoreCurrentCellOnFocus();
        private readonly Behavior toggleSelectionOnSpace = new ToggleSelectionOnSpace();
        private readonly Stack<Behavior> attachedBehaviors = new Stack<Behavior>();

        public bool MoveFocusExternallyOnTab { get; set; } = true;
        public bool MoveFocusInternallyOnControlArrow { get; set; } = true;
        public bool RestoreCurrentCellOnFocus { get; set; } = true;
        public bool ToggleSelectionOnSpace { get; set; } = true;

        protected override void OnAttached()
        {
            if (MoveFocusExternallyOnTab)
            {
                Attach(moveFocusExternallyOnTab);
            }
            if (MoveFocusInternallyOnControlArrow)
            {
                Attach(moveFocusInternallyOnControlArrow);
            }
            if (RestoreCurrentCellOnFocus)
            {
                Attach(restoreCurrentCellOnFocus);
            }
            if (ToggleSelectionOnSpace)
            {
                Attach(toggleSelectionOnSpace);
            }
        }

        private void Attach(Behavior behavior)
        {
            behavior.Attach(AssociatedObject);
            attachedBehaviors.Push(behavior);
        }

        protected override void OnDetaching()
        {
            while (attachedBehaviors.Count > 0)
            {
                attachedBehaviors.Peek().Detach();
                attachedBehaviors.Pop();
            }
        }
    }
}
