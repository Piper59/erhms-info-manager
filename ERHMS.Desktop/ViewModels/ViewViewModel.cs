using Epi;
using ERHMS.Data;
using ERHMS.Desktop.Infrastructure.ViewModels;
using ERHMS.Desktop.ViewModels.Collections;
using ERHMS.EpiInfo.Data;
using System.Linq;
using System.Threading.Tasks;

namespace ERHMS.Desktop.ViewModels
{
    public class ViewViewModel : ViewModel
    {
        public View Value { get; }
        public RecordCollectionViewModel Records { get; }

        public ViewViewModel(View value)
        {
            Value = value;
            Records = new RecordCollectionViewModel();
        }

        public async Task InitializeAsync()
        {
            Records.Initialize(await Task.Run(() =>
            {
                RecordRepository repository = new RecordRepository(Value);
                return repository.Select().ToList();
            }));
        }
    }
}
