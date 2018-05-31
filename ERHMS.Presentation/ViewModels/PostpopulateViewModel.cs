﻿using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.EpiInfo.Domain;
using ERHMS.Presentation.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using View = Epi.View;

namespace ERHMS.Presentation.ViewModels
{
    public class PostpopulateViewModel : DialogViewModel
    {
        public class ResponderListChildViewModel : ListViewModel<Responder>
        {
            public ResponderListChildViewModel()
            {
                Refresh();
            }

            protected override IEnumerable<Responder> GetItems()
            {
                return Context.Responders.Select().OrderBy(responder => responder.FullName, StringComparer.OrdinalIgnoreCase);
            }

            public void SelectById(string responderId)
            {
                SelectedObject = Items.FirstOrDefault(responder => responder.ResponderId.Equals(responderId, StringComparison.OrdinalIgnoreCase));
            }
        }

        public ViewEntity Entity { get; private set; }
        public ViewEntityRepository<ViewEntity> Entities { get; private set; }
        public ResponderListChildViewModel Responders { get; private set; }

        public ICommand LinkCommand { get; private set; }
        public ICommand UnlinkCommand { get; private set; }

        public PostpopulateViewModel(View view, ViewEntity entity)
        {
            Title = "Link to Responder";
            Entity = entity;
            Entities = new ViewEntityRepository<ViewEntity>(Context.Database, view);
            Responders = new ResponderListChildViewModel();
            Responders.SelectById(entity.GetProperty("ResponderID") as string);
            LinkCommand = new Command(Link, Responders.HasSelectedItem);
            UnlinkCommand = new Command(Unlink);
        }

        public event EventHandler Saved;
        private void OnSaved(EventArgs e)
        {
            Saved?.Invoke(this, e);
        }
        private void OnSaved()
        {
            OnSaved(EventArgs.Empty);
        }

        private void SetResponderId(string responderId)
        {
            ViewEntity entity = Entities.Refresh(Entity);
            entity.SetProperty("ResponderID", responderId);
            Entities.Save(entity);
            OnSaved();
            Close();
        }

        public void Link()
        {
            SetResponderId(Responders.SelectedItem.ResponderId);
        }

        public void Unlink()
        {
            SetResponderId(null);
        }
    }
}
