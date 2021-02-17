﻿using ERHMS.Desktop.Infrastructure;
using Microsoft.Xaml.Behaviors;
using System.Windows.Controls;
using System.Windows.Input;

namespace ERHMS.Desktop.Behaviors
{
    public class MoveFocusExternallyOnTab : Behavior<DataGrid>
    {
        private static bool TryGetDirection(ModifierKeys modifiers, out bool forward)
        {
            switch (modifiers)
            {
                case ModifierKeys.None:
                    forward = true;
                    return true;
                case ModifierKeys.Shift:
                    forward = false;
                    return true;
                default:
                    forward = default;
                    return false;
            }
        }

        private static FocusNavigationDirection GetExternalDirection(bool forward)
        {
            return forward ? FocusNavigationDirection.Next : FocusNavigationDirection.Previous;
        }

        private static FocusNavigationDirection GetTerminalDirection(bool forward)
        {
            return forward ? FocusNavigationDirection.Last : FocusNavigationDirection.First;
        }

        protected override void OnAttached()
        {
            AssociatedObject.PreviewKeyDown += AssociatedObject_PreviewKeyDown;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.PreviewKeyDown -= AssociatedObject_PreviewKeyDown;
        }

        private void AssociatedObject_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Tab
                && TryGetDirection(e.KeyboardDevice.Modifiers, out bool forward)
                && MoveFocus(forward))
            {
                e.Handled = true;
            }
        }

        private bool MoveFocus(bool forward)
        {
            bool intercepted = false;
            bool result = false;

            void AssociatedObject_PreviewGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
            {
                intercepted = true;
                result = MoveFocusExternally(forward);
            }

            AssociatedObject.PreviewGotKeyboardFocus += AssociatedObject_PreviewGotKeyboardFocus;
            try
            {
                MoveFocusTerminally(forward);
                return intercepted ? result : MoveFocusExternally(forward);
            }
            finally
            {
                AssociatedObject.PreviewGotKeyboardFocus -= AssociatedObject_PreviewGotKeyboardFocus;
            }
        }

        private bool MoveFocusTerminally(bool forward)
        {
            return AssociatedObject.MoveFocus(GetTerminalDirection(forward));
        }

        private bool MoveFocusExternally(bool forward)
        {
            return Keyboard.FocusedElement.MoveFocus(GetExternalDirection(forward));
        }
    }
}
