using GalaSoft.MvvmLight.Command;
using System.Collections.Generic;
using System.IO;

namespace ERHMS.Presentation.ViewModels
{
    public class AttachmentViewModel : ViewModelBase
    {
        public ICollection<AttachmentViewModel> Container { get; private set; }
        public FileInfo File { get; private set; }

        public RelayCommand RemoveCommand { get; private set; }

        public AttachmentViewModel(ICollection<AttachmentViewModel> container, FileInfo file)
        {
            Container = container;
            File = file;
            RemoveCommand = new RelayCommand(Remove);
        }

        public void Remove()
        {
            Container.Remove(this);
        }
    }
}
