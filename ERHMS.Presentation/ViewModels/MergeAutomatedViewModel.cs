using ERHMS.Domain;
using ERHMS.EpiInfo.DataAccess;
using ERHMS.Presentation.Commands;
using ERHMS.Presentation.Properties;
using ERHMS.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Presentation.ViewModels
{
    public class MergeAutomatedViewModel : DocumentViewModel
    {
        public class PairListChildViewModel : ListViewModel<Tuple<Responder, Responder>>
        {
            private ICollection<Tuple<Responder, Responder>> items;

            public PairListChildViewModel(IEnumerable<Tuple<Responder, Responder>> pairs)
            {
                items = pairs.ToList();
                Refresh();
            }

            protected override IEnumerable<Tuple<Responder, Responder>> GetItems()
            {
                return items;
            }

            protected override IEnumerable<string> GetFilteredValues(Tuple<Responder, Responder> item)
            {
                yield return item.Item1.FullName;
                yield return item.Item1.EmailAddress;
                yield return item.Item2.FullName;
                yield return item.Item2.EmailAddress;
            }

            public void Remove(Tuple<Responder, Responder> item)
            {
                items.Remove(item);
            }
        }

        public PairListChildViewModel Pairs { get; private set; }

        public ICommand MergeCommand { get; private set; }
        public ICommand IgnoreCommand { get; private set; }

        public MergeAutomatedViewModel(IEnumerable<Tuple<Responder, Responder>> pairs)
        {
            Title = "Duplicates";
            Pairs = new PairListChildViewModel(pairs);
            MergeCommand = new Command(Merge, Pairs.HasOneSelectedItem);
            IgnoreCommand = new AsyncCommand(IgnoreAsync, Pairs.HasAnySelectedItems);
        }

        public void Merge()
        {
            ServiceLocator.Document.Show(() =>
            {
                Tuple<Responder, Responder> pair = Pairs.SelectedItems.First();
                MergeSelectedViewModel model = new MergeSelectedViewModel(
                    Context.Responders.Refresh(pair.Item1),
                    Context.Responders.Refresh(pair.Item2));
                model.Saved += (sender, e) =>
                {
                    Pairs.Remove(pair);
                    Pairs.Refresh();
                };
                return model;
            });
        }

        public async Task IgnoreAsync()
        {
            if (await ServiceLocator.Dialog.ConfirmAsync(Resources.ResponderPairConfirmIgnore, "Ignore"))
            {
                using (ServiceLocator.Busy.Begin())
                {
                    foreach (Tuple<Responder, Responder> pair in Pairs.SelectedItems)
                    {
                        Context.UniquePairs.Save(new UniquePair(true)
                        {
                            Responder1Id = pair.Item1.ResponderId,
                            Responder2Id = pair.Item2.ResponderId
                        });
                        Pairs.Remove(pair);
                    }
                }
                Pairs.Refresh();
            }
        }
    }
}
