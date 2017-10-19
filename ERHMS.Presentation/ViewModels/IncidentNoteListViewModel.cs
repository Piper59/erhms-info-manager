﻿using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class IncidentNoteListViewModel : ListViewModel<IncidentNote>
    {
        public Incident Incident { get; private set; }

        private string content;
        [DirtyCheck]
        public string Content
        {
            get { return content; }
            set { Set(nameof(Content), ref content, value); }
        }

        public RelayCommand SaveCommand { get; private set; }

        public IncidentNoteListViewModel(IServiceManager services, Incident incident)
            : base(services)
        {
            Title = "Notes";
            Incident = incident;
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

        protected override IEnumerable<IncidentNote> GetItems()
        {
            return Context.IncidentNotes.SelectByIncidentId(Incident.IncidentId).OrderByDescending(note => note.Date);
        }

        public void Save()
        {
            Context.IncidentNotes.Save(new IncidentNote(true)
            {
                IncidentId = Incident.IncidentId,
                Content = Content,
                Date = DateTime.Now
            });
            MessengerInstance.Send(new RefreshMessage(typeof(IncidentNote)));
            Content = "";
            Dirty = false;
        }
    }
}