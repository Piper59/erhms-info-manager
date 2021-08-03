using Microsoft.Xaml.Behaviors;
using System;
using System.Windows;

namespace ERHMS.Desktop.Behaviors
{
    public abstract class BindProperty<T> : Behavior<T>
        where T : DependencyObject
    {
        private bool updating;

        protected static void OnPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ((BindProperty<T>)sender).Push();
        }

        protected virtual void PushCore()
        {
            throw new NotSupportedException("Cannot update binding target.");
        }

        protected virtual void PullCore()
        {
            throw new NotSupportedException("Cannot update binding source.");
        }

        protected void Push()
        {
            if (AssociatedObject == null)
            {
                return;
            }
            Update(PushCore);
        }

        protected void Pull()
        {
            Update(PullCore);
        }

        private void Update(Action action)
        {
            if (updating)
            {
                return;
            }
            updating = true;
            try
            {
                action();
            }
            finally
            {
                updating = false;
            }
        }
    }
}
