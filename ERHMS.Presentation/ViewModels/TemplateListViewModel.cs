using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.MakeView;
using ERHMS.Presentation.Messages;
using ERHMS.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;
using Template = ERHMS.EpiInfo.Template;

namespace ERHMS.Presentation.ViewModels
{
    public class TemplateListViewModel : DocumentViewModel
    {
        private static readonly ICollection<Func<Template, string>> FilterPropertyAccessors = new Func<Template, string>[]
        {
            template => template.Name,
            template => template.Description
        };

        public Incident Incident { get; private set; }

        public string IncidentId
        {
            get { return Incident == null ? null : Incident.IncidentId; }
        }

        private string filter;
        public string Filter
        {
            get
            {
                return filter;
            }
            set
            {
                if (!Set(() => Filter, ref filter, value))
                {
                    return;
                }
                Templates.Refresh();
            }
        }

        private ICollectionView templates;
        public ICollectionView Templates
        {
            get { return templates; }
            set { Set(() => Templates, ref templates, value); }
        }

        private Template selectedTemplate;
        public Template SelectedTemplate
        {
            get
            {
                return selectedTemplate;
            }
            set
            {
                if (!Set(() => SelectedTemplate, ref selectedTemplate, value))
                {
                    return;
                }
                CreateCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand RefreshCommand { get; private set; }

        public TemplateListViewModel(Incident incident)
        {
            if (incident == null)
            {
                Title = "Templates";
            }
            else
            {
                Title = string.Format("{0} Templates", incident.Name);
            }
            Incident = incident;
            Refresh();
            CreateCommand = new RelayCommand(Create, HasSelectedTemplate);
            DeleteCommand = new RelayCommand(Delete, HasSelectedTemplate);
            RefreshCommand = new RelayCommand(Refresh);
            Messenger.Default.Register<RefreshMessage<Template>>(this, OnRefreshMessage);
        }

        public bool HasSelectedTemplate()
        {
            return SelectedTemplate != null;
        }

        public void Create()
        {
            MakeView.AddFromTemplate(DataContext.Project, SelectedTemplate, Incident == null ? null : Incident.Name, IncidentId);
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage(
                "Delete?",
                "Are you sure you want to delete this template?",
                "Delete",
                "Don't Delete");
            msg.Confirmed += (sender, e) =>
            {
                SelectedTemplate.Delete();
                Messenger.Default.Send(new RefreshMessage<Template>());
            };
            Messenger.Default.Send(msg);
        }
        public void Refresh()
        {
            Templates = CollectionViewSource.GetDefaultView(DataContext.GetTemplates(TemplateLevel.View).OrderBy(template => template.Name));
            Templates.Filter = MatchesFilter;
        }

        private bool MatchesFilter(object item)
        {
            if (string.IsNullOrWhiteSpace(Filter))
            {
                return true;
            }
            Template template = (Template)item;
            foreach (Func<Template, string> accessor in FilterPropertyAccessors)
            {
                string property = accessor(template);
                if (property != null && property.ContainsIgnoreCase(Filter))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnRefreshMessage(RefreshMessage<Template> msg)
        {
            Refresh();
        }
    }
}
