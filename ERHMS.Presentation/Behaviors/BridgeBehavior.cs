using System;
using System.Windows;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    public class BridgeBehavior<T> : Behavior<T>
        where T : DependencyObject
    {
        private bool updating;

        protected BridgeBehavior() { }

        protected void Update(Action action)
        {
            if (updating)
            {
                return;
            }
            updating = true;
            action();
            updating = false;
        }
    }
}
