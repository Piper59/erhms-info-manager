﻿using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentNotesViewModel : ListViewModel<IncidentNote>
    {
        public Incident Incident { get; private set; }

        private IncidentNote note;
        public IncidentNote Note
        {
            get { return note; }
            private set { Set(nameof(Note), ref note, value); }
        }

        private RelayCommand saveCommand;
        public ICommand SaveCommand
        {
            get { return saveCommand ?? (saveCommand = new RelayCommand(Save, HasContent)); }
        }

        public IncidentNotesViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Notes";
            Incident = incident;
            Reset();
            Refresh();
        }

        public bool HasContent()
        {
            return !string.IsNullOrWhiteSpace(Note.Content);
        }

        protected override IEnumerable<IncidentNote> GetItems()
        {
            return Context.IncidentNotes.SelectByIncidentId(Incident.IncidentId).OrderByDescending(note => note.Date);
        }

        private void Reset()
        {
            if (Note != null)
            {
                RemoveDirtyCheck(Note);
                Note.PropertyChanged -= Note_PropertyChanged;
            }
            Note = new IncidentNote
            {
                IncidentId = Incident.IncidentId
            };
            AddDirtyCheck(Note);
            Note.PropertyChanged += Note_PropertyChanged;
        }

        private void Note_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IncidentNote.Content))
            {
                saveCommand.RaiseCanExecuteChanged();
            }
        }

        public void Save()
        {
            Note.Date = DateTime.Now;
            Context.IncidentNotes.Save(Note);
            MessengerInstance.Send(new RefreshMessage(typeof(IncidentNote)));
            Reset();
            Dirty = false;
        }
    }
}
