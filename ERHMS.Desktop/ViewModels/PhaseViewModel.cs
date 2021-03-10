using ERHMS.Domain;
using System.Collections.Generic;

namespace ERHMS.Desktop.ViewModels
{
    public class PhaseViewModel
    {
        public Phase Value { get; }
        public CoreProject CoreProject => Value.ToCoreProject();
        public IEnumerable<CoreView> CoreViews => CoreView.GetInstances(Value);

        public PhaseViewModel(Phase value)
        {
            Value = value;
        }
    }
}
