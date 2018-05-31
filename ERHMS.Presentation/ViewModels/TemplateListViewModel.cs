﻿using ERHMS.Domain;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.Wrappers;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class TemplateListViewModel : DocumentViewModel
    {
        public class TemplateListChildViewModel : ListViewModel<TemplateInfo>
        {
            public Incident Incident { get; private set; }

            public TemplateListChildViewModel(Incident incident)
            {
                Incident = incident;
                Refresh();
            }

            protected override IEnumerable<TemplateInfo> GetItems()
            {
                return TemplateInfo.GetByLevel(TemplateLevel.View).OrderBy(templateInfo => templateInfo.Name, StringComparer.OrdinalIgnoreCase);
            }

            protected override IEnumerable<string> GetFilteredValues(TemplateInfo item)
            {
                yield return item.Name;
                yield return item.Description;
            }

            public void SelectByPath(string path)
            {
                SelectedObject = Items.SingleOrDefault(templateInfo => templateInfo.FilePath.Equals(path, StringComparison.OrdinalIgnoreCase));
            }
        }

        public Incident Incident { get; private set; }
        public TemplateListChildViewModel Templates { get; private set; }

        public ICommand CreateCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public TemplateListViewModel(Incident incident)
        {
            Title = "Templates";
            Incident = incident;
            Templates = new TemplateListChildViewModel(incident);
            CreateCommand = new AsyncCommand(CreateAsync, Templates.HasSelectedItem);
            DeleteCommand = new AsyncCommand(DeleteAsync, Templates.HasSelectedItem);
        }

        public async Task CreateAsync()
        {
            string prefix = Incident == null ? "" : Incident.Name + "_";
            Wrapper wrapper = MakeView.InstantiateViewTemplate.Create(Context.Project.FilePath, Templates.SelectedItem.FilePath, prefix);
            wrapper.AddViewCreatedHandler(Incident);
            await ServiceLocator.Wrapper.InvokeAsync(wrapper);
        }

        public async Task DeleteAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.TemplateConfirmDelete, "Delete"))
            {
                Templates.SelectedItem.Delete();
                ServiceLocator.Data.Refresh(typeof(TemplateInfo));
            }
        }
    }
}
