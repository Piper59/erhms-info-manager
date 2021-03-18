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
        public RecordCollectionViewModel Records { get; private set; }

        public ViewViewModel(View value)
        {
            Value = value;
        }

        public async Task InitializeAsync()
        {
            Records = new RecordCollectionViewModel(Value, await Task.Run(() =>
            {
                RecordRepository recordRepository = new RecordRepository(Value);
                return recordRepository.Select().ToList();
            }));
            await Records.InitializeAsync();
        }
    }
}
