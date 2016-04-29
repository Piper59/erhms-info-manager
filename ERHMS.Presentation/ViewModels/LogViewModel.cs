using ERHMS.EpiInfo;
using System.IO;

namespace ERHMS.Presentation.ViewModels
{
    public class LogViewModel : DocumentViewModel
    {
        public string Content { get; private set; }

        public LogViewModel()
        {
            Title = "Log";
            using (StreamReader reader = new StreamReader(Log.Read()))
            {
                Content = reader.ReadToEnd();
            }
        }
    }
}
