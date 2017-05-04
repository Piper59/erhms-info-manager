using System;
using System.Threading.Tasks;

namespace ERHMS.Presentation.Messages
{
    public class BlockMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }

        public BlockMessage()
        {
            Title = "Working \u2026";
        }

        public event EventHandler Executing;
        public event EventHandler Executed;

        public async Task OnExecuting()
        {
            await Task.Factory.StartNew(() =>
            {
                Executing?.Invoke(this, EventArgs.Empty);
            });
        }

        public void OnExecuted()
        {
            Executed?.Invoke(this, EventArgs.Empty);
        }
    }
}
