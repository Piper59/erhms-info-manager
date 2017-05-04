using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentNotesViewModel : ViewModelBase
    {
        public Incident Incident { get; private set; }

        private ICollection<IncidentNote> notes;
        public ICollection<IncidentNote> Notes
        {
            get { return notes; }
            private set { Set(nameof(Notes), ref notes, value); }
        }

        private IncidentNote note;
        public IncidentNote Note
        {
            get { return note; }
            private set { Set(nameof(Note), ref note, value); }
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public IncidentNotesViewModel(Incident incident)
        {
            Incident = incident;
            ResetNote();
            Refresh();
            SaveCommand = new RelayCommand(Save, HasContent);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<IncidentNote>>(this, msg => Refresh());
        }

        public bool HasContent()
        {
            return !string.IsNullOrWhiteSpace(Note.Content);
        }

        private void ResetNote()
        {
            if (Note != null)
            {
                RemoveDirtyCheck(Note);
            }
            Note = DataContext.IncidentNotes.Create();
            Note.IncidentId = Incident.IncidentId;
            Note.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != nameof(IncidentNote.Content))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            };
            AddDirtyCheck(Note);
        }

        public void Refresh()
        {
            Notes = DataContext.IncidentNotes.SelectByIncidentId(Incident.IncidentId)
                .OrderByDescending(note => note.Date)
                .ToList();
        }

        public void Save()
        {
            Note.Date = DateTime.Now;
            DataContext.IncidentNotes.Save(Note);
            Dirty = false;
            Messenger.Default.Send(new RefreshMessage<IncidentNote>());
            ResetNote();
        }
    }
}
