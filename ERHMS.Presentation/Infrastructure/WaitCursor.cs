using System;
using System.Windows.Input;

namespace ERHMS.Presentation
{
    public class WaitCursor : IDisposable
    {
        private Cursor cursor;

        public WaitCursor()
        {
            cursor = Mouse.OverrideCursor;
            Mouse.OverrideCursor = Cursors.Wait;
        }

        public void Dispose()
        {
            Mouse.OverrideCursor = cursor;
        }
    }
}
