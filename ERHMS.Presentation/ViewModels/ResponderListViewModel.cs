using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class ResponderListViewModel : ListViewModel<Responder>
    {
        private ViewListViewModel views;
        private ViewListViewModel Views
        {
            get
            {
                views.SelectedItem = views.TypedItems.SingleOrDefault(view => view.Name == Context.Responders.View.Name);
                return views;
            }
        }

        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand MergeAutomatedCommand { get; private set; }
        public RelayCommand MergeSelectedCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }
        public RelayCommand ImportFromProjectCommand { get; private set; }
        public RelayCommand ImportFromPackageCommand { get; private set; }
        public RelayCommand ImportFromFileCommand { get; private set; }
        public RelayCommand ImportFromWebCommand { get; private set; }
        public RelayCommand ImportFromMobileCommand { get; private set; }
        public RelayCommand ExportToPackageCommand { get; private set; }
        public RelayCommand ExportToFileCommand { get; private set; }
        public RelayCommand AnalyzeClassicCommand { get; private set; }
        public RelayCommand AnalyzeVisualCommand { get; private set; }

        public ResponderListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Responders";
            views = new ViewListViewModel(services, null);
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSingleSelectedItem);
            MergeAutomatedCommand = new RelayCommand(MergeAutomated);
            MergeSelectedCommand = new RelayCommand(MergeSelected);
            DeleteCommand = new RelayCommand(Delete, HasSingleSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
            ImportFromProjectCommand = new RelayCommand(ImportFromProject);
            ImportFromPackageCommand = new RelayCommand(ImportFromPackage);
            ImportFromFileCommand = new RelayCommand(ImportFromFile);
            ImportFromWebCommand = new RelayCommand(ImportFromWeb);
            ImportFromMobileCommand = new RelayCommand(ImportFromMobile);
            ExportToPackageCommand = new RelayCommand(ExportToPackage);
            ExportToFileCommand = new RelayCommand(ExportToFile);
            AnalyzeClassicCommand = new RelayCommand(AnalyzeClassic);
            AnalyzeVisualCommand = new RelayCommand(AnalyzeVisual);
            SelectionChanged += (sender, e) =>
            {
                EditCommand.RaiseCanExecuteChanged();
                MergeSelectedCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
                EmailCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<Responder> GetItems()
        {
            return Context.Responders.SelectUndeleted()
                .OrderBy(responder => responder.FullName)
                .ThenBy(responder => responder.EmailAddress);
        }

        protected override IEnumerable<string> GetFilteredValues(Responder item)
        {
            yield return item.LastName;
            yield return item.FirstName;
            yield return item.EmailAddress;
            yield return item.City;
            yield return item.State;
            yield return item.OrganizationName;
            yield return item.Occupation;
        }

        public void Create()
        {
            Documents.ShowNewResponder();
        }

        public void Edit()
        {
            Documents.ShowResponder((Responder)SelectedItem.Clone());
        }

        public void MergeAutomated()
        {
            ICollection<Tuple<Responder, Responder>> duplicates = new List<Tuple<Responder, Responder>>();
            BlockMessage msg = new BlockMessage
            {
                Message = "Searching for duplicate responders \u2026"
            };
            msg.Executing += (sender, e) =>
            {
                IList<Responder> responders = Context.Responders.SelectUndeleted()
                    .OrderBy(responder => responder.FullName)
                    .ToList();
                ILookup<string, string> uniquePairs = Context.UniquePairs.SelectLookup();
                for (int index1 = 0; index1 < responders.Count; index1++)
                {
                    Responder responder1 = responders[index1];
                    for (int index2 = index1 + 1; index2 < responders.Count; index2++)
                    {
                        Responder responder2 = responders[index2];
                        if (uniquePairs[responder1.ResponderId].Contains(responder2.ResponderId, StringComparer.OrdinalIgnoreCase))
                        {
                            continue;
                        }
                        if (responder1.IsSimilar(responder2))
                        {
                            duplicates.Add(Tuple.Create(responder1, responder2));
                        }
                    }
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (duplicates.Count == 0)
                {
                    ICollection<string> message = new List<string>();
                    message.Add("No duplicate responders found.");
                    message.Add("You may still perform a merge by selecting two responders from the list and clicking Merge > Selected.");
                    MessengerInstance.Send(new AlertMessage
                    {
                        Title = "Help",
                        Message = string.Join(" ", message)
                    });
                }
                else
                {
                    Documents.Show(
                        () => new MergeAutomatedViewModel(Services, duplicates),
                        document => false);
                }
            };
            MessengerInstance.Send(msg);
        }

        public void MergeSelected()
        {
            if (SelectedItems.Count != 2)
            {
                MessengerInstance.Send(new AlertMessage
                {
                    Message = "Please select two responders to merge."
                });
                return;
            }
            Documents.Show(
                () => new MergeSelectedViewModel(Services, (Responder)SelectedItems[0], (Responder)SelectedItems[1]),
                document => false);
        }

        public void Delete()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Delete",
                Message = "Delete the selected responder?"
            };
            msg.Confirmed += (sender, e) =>
            {
                SelectedItem.Deleted = true;
                Context.Responders.Save(SelectedItem);
                MessengerInstance.Send(new RefreshMessage(typeof(Responder)));
            };
            MessengerInstance.Send(msg);
        }

        public void Email()
        {
            Documents.Show(
                () => new EmailViewModel(Services, TypedSelectedItems),
                document => false);
        }

        public void ImportFromProject()
        {
            Views.ImportFromProject();
        }

        public void ImportFromPackage()
        {
            Views.ImportFromPackage();
        }

        public void ImportFromFile()
        {
            Views.ImportFromFile();
        }

        public void ImportFromWeb()
        {
            Views.ImportFromWeb();
        }

        public void ImportFromMobile()
        {
            Views.ImportFromMobile();
        }

        public void ExportToPackage()
        {
            Views.ExportToPackage();
        }

        public void ExportToFile()
        {
            Views.ExportToFile();
        }

        public void AnalyzeClassic()
        {
            Views.AnalyzeClassic();
        }

        public void AnalyzeVisual()
        {
            Views.AnalyzeVisual();
        }

    }
}
