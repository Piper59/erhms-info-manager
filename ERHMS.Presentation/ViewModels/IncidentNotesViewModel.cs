using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
            Refresh();
            Note = CreateNote();
            SaveCommand = new RelayCommand(Save, HasContent);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<IncidentNote>>(this, OnRefreshMessage);
        }

        public bool HasContent()
        {
            return !string.IsNullOrWhiteSpace(Note.Content);
        }

        private IncidentNote CreateNote()
        {
            IncidentNote note = DataContext.IncidentNotes.Create();
            note.IncidentId = Incident.IncidentId;
            note.PropertyChanged += Note_PropertyChanged;
            return note;
        }

        private void Note_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            IncidentNote note = (IncidentNote)sender;
            switch (e.PropertyName)
            {
                case "Content":
                    SaveCommand.RaiseCanExecuteChanged();
                    break;
            }
        }

        public void Save()
        {
            Note.Date = DateTime.Now;
            DataContext.IncidentNotes.Save(Note);
            Messenger.Default.Send(new RefreshMessage<IncidentNote>());
            Note.PropertyChanged -= Note_PropertyChanged;
            Note = CreateNote();
        }

        public void Refresh()
        {
            Notes = DataContext.IncidentNotes.Select(DataContext.GetIncidentPredicate(Incident.IncidentId))
                .OrderByDescending(note => note.Date)
                .ToList();
        }

        private void OnRefreshMessage(RefreshMessage<IncidentNote> msg)
        {
            Refresh();
        }
    }
}
