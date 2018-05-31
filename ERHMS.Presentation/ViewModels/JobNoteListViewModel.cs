using ERHMS.Domain;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class JobNoteListViewModel : DocumentViewModel
    {
        public class JobNoteListChildViewModel : ListViewModel<JobNote>
        {
            public Job Job { get; private set; }

            public JobNoteListChildViewModel(Job job)
            {
                Job = job;
                Refresh();
            }

            protected override IEnumerable<JobNote> GetItems()
            {
                return Context.JobNotes.SelectByJobId(Job.JobId).OrderByDescending(jobNote => jobNote.Date);
            }
        }

        public Job Job { get; private set; }
        public JobNoteListChildViewModel JobNotes { get; private set; }

        private string content;
        [DirtyCheck]
        public string Content
        {
            get { return content; }
            set { SetProperty(nameof(Content), ref content, value); }
        }

        public ICommand SaveCommand { get; private set; }

        public JobNoteListViewModel(Job job)
        {
            Title = "Notes";
            Job = job;
            JobNotes = new JobNoteListChildViewModel(job);
            SaveCommand = new Command(Save, CanSave);
        }

        public bool CanSave()
        {
            return !string.IsNullOrWhiteSpace(Content);
        }

        public void Save()
        {
            Context.JobNotes.Save(new JobNote(true)
            {
                JobId = Job.JobId,
                Content = Content,
                Date = DateTime.Now
            });
            ServiceLocator.Data.Refresh(typeof(JobNote));
            Content = "";
            Dirty = false;
        }
    }
}
