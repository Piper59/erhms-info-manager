using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobNoteListViewModel : ListViewModel<JobNote>
    {
        public Job Job { get; private set; }

        private string content;
        [DirtyCheck]
        public string Content
        {
            get { return content; }
            set { Set(nameof(Content), ref content, value); }
        }

        public RelayCommand SaveCommand { get; private set; }

        public JobNoteListViewModel(IServiceManager services, Job job)
            : base(services)
        {
            Title = "Notes";
            Job = job;
            Refresh();
            SaveCommand = new RelayCommand(Save, HasContent);
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(Content))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            };
        }

        public bool HasContent()
        {
            return !string.IsNullOrWhiteSpace(Content);
        }

        protected override IEnumerable<JobNote> GetItems()
        {
            return Context.JobNotes.SelectByJobId(Job.JobId).OrderByDescending(note => note.Date);
        }

        public void Save()
        {
            Context.JobNotes.Save(new JobNote(true)
            {
                JobId = Job.JobId,
                Content = Content,
                Date = DateTime.Now
            });
            MessengerInstance.Send(new RefreshMessage(typeof(JobNote)));
            Content = "";
            Dirty = false;
        }
    }
}
