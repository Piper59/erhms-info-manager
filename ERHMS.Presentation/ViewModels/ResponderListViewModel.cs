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
        public RelayCommand CreateCommand { get; private set; }
        public RelayCommand EditCommand { get; private set; }
        public RelayCommand MergeAutomatedCommand { get; private set; }
        public RelayCommand MergeSelectedCommand { get; private set; }
        public RelayCommand DeleteCommand { get; private set; }
        public RelayCommand EmailCommand { get; private set; }

        public ResponderListViewModel(IServiceManager services)
            : base(services)
        {
            Title = "Responders";
            Refresh();
            CreateCommand = new RelayCommand(Create);
            EditCommand = new RelayCommand(Edit, HasSingleSelectedItem);
            MergeAutomatedCommand = new RelayCommand(MergeAutomated);
            MergeSelectedCommand = new RelayCommand(MergeSelected);
            DeleteCommand = new RelayCommand(Delete, HasSingleSelectedItem);
            EmailCommand = new RelayCommand(Email, HasSelectedItem);
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
                Message = "Searching for potentially duplicate responders \u2026"
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
                            duplicates.Add(new Tuple<Responder, Responder>(responder1, responder2));
                        }
                    }
                }
            };
            msg.Executed += (sender, e) =>
            {
                if (duplicates.Count == 0)
                {
                    MessengerInstance.Send(new AlertMessage
                    {
                        Message = "No potentially duplicate responders found."
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
    }
}
