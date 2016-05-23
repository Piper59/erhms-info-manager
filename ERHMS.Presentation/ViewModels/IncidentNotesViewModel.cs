using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
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
            set { Set(() => Notes, ref notes, value); }
        }

        private IncidentNote note;
        public IncidentNote Note
        {
            get { return note; }
            set { Set(() => Note, ref note, value); }
        }

        public RelayCommand SaveCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public IncidentNotesViewModel(Incident incident)
        {
            Incident = incident;
            UpdateTitle();
            Refresh();
            Note = CreateNote();
            SaveCommand = new RelayCommand(Save, HasContent);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Incident>>(this, OnRefreshIncidentMessage);
            Messenger.Default.Register<RefreshListMessage<IncidentNote>>(this, OnRefreshIncidentNoteListMessage);
        }

        private void UpdateTitle()
        {
            Title = GetTitleWithIncidentName("Notes", Incident);
        }

        public void Refresh()
        {
            Notes = DataContext.IncidentNotes.SelectByIncident(Incident.IncidentId)
                .OrderByDescending(note => note.Date)
                .ToList();
        }

        private IncidentNote CreateNote()
        {
            IncidentNote note = DataContext.IncidentNotes.Create();
            note.IncidentId = Incident.IncidentId;
            note.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName != nameof(note.Content))
                {
                    SaveCommand.RaiseCanExecuteChanged();
                }
            };
            return note;
        }

        public bool HasContent()
        {
            return !string.IsNullOrWhiteSpace(Note.Content);
        }

        public void Save()
        {
            Note.Date = DateTime.Now;
            DataContext.IncidentNotes.Save(Note);
            Messenger.Default.Send(new RefreshListMessage<IncidentNote>(Incident.IncidentId));
            Note = CreateNote();
        }

        private void OnRefreshIncidentMessage(RefreshMessage<Incident> msg)
        {
            if (msg.Entity == Incident)
            {
                UpdateTitle();
            }
        }

        private void OnRefreshIncidentNoteListMessage(RefreshListMessage<IncidentNote> msg)
        {
            if (StringExtensions.EqualsIgnoreCase(msg.IncidentId, Incident.IncidentId))
            {
                Refresh();
            }
        }
    }
}
