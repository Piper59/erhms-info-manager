using System;
using System.Windows;
using System.Windows.Interactivity;

namespace ERHMS.Presentation.Behaviors
{
    public abstract class BridgeBehavior<T> : Behavior<T>
        where T : DependencyObject
    {
        private bool updating;

        private void Update(Action action)
        {
            if (updating)
            {
                return;
            }
            updating = true;
            action();
            updating = false;
        }

        protected abstract void PushCore();
        protected abstract void PullCore();

        protected void Push()
        {
            Update(PushCore);
        }

        protected void Pull()
        {
            Update(PullCore);
        }
    }
}
