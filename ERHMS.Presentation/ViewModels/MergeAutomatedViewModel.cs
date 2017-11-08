using ERHMS.Domain;
using ERHMS.Presentation.Messages;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ERHMS.Presentation.ViewModels
{
    public class MergeAutomatedViewModel : ListViewModel<Tuple<Responder, Responder>>
    {
        private ICollection<Tuple<Responder, Responder>> duplicates;

        public RelayCommand MergeCommand { get; private set; }
        public RelayCommand IgnoreCommand { get; private set; }

        public MergeAutomatedViewModel(IServiceManager services, IEnumerable<Tuple<Responder, Responder>> duplicates)
            : base(services)
        {
            Title = "Duplicates";
            this.duplicates = duplicates.ToList();
            Refresh();
            MergeCommand = new RelayCommand(Merge, HasSingleSelectedItem);
            IgnoreCommand = new RelayCommand(Ignore, HasSelectedItem);
            SelectionChanged += (sender, e) =>
            {
                MergeCommand.RaiseCanExecuteChanged();
                IgnoreCommand.RaiseCanExecuteChanged();
            };
        }

        protected override IEnumerable<Tuple<Responder, Responder>> GetItems()
        {
            return duplicates;
        }

        protected override IEnumerable<string> GetFilteredValues(Tuple<Responder, Responder> item)
        {
            yield return item.Item1.FullName;
            yield return item.Item1.EmailAddress;
            yield return item.Item2.FullName;
            yield return item.Item2.EmailAddress;
        }

        public void Merge()
        {
            MergeSelectedViewModel duplicate = Documents.Show(
                () => new MergeSelectedViewModel(Services, SelectedItem.Item1, SelectedItem.Item2),
                document => false);
            duplicate.Saved += (sender, e) =>
            {
                duplicates.Remove(SelectedItem);
                Refresh();
            };
        }

        public void Ignore()
        {
            ConfirmMessage msg = new ConfirmMessage
            {
                Verb = "Ignore",
                Message = "Ignore the selected pairs of responders?"
            };
            msg.Confirmed += (sender, e) =>
            {
                foreach (Tuple<Responder, Responder> duplicate in SelectedItems)
                {
                    Context.UniquePairs.Save(new UniquePair(true)
                    {
                        Responder1Id = duplicate.Item1.ResponderId,
                        Responder2Id = duplicate.Item2.ResponderId
                    });
                    duplicates.Remove(duplicate);
                }
                Refresh();
            };
            MessengerInstance.Send(msg);
        }
    }
}
