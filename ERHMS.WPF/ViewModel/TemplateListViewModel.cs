using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.WPF.ViewModel
{
    public class TemplateListViewModel : ViewModelBase
    {
        private FileInfo selectedTemplate;
        public FileInfo SelectedTemplate
        {
            get { return selectedTemplate; }
            set { Set(ref selectedTemplate, value); }
        }

        private ICollectionView templateList;
        public ICollectionView TemplateList
        {
            get { return templateList; }
            private set { Set(ref templateList, value); }
        }

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                Set(ref filter, value);
                TemplateList.Filter = ListFilterFunc;
            }
        }

        private bool ListFilterFunc(object item)
        {
            dynamic t = item as FileInfo;

            return Filter == null ||
                Filter.Equals("") ||
                (t.Name != null && t.Name.ToLower().Contains(Filter.ToLower()));
        }
        
        public ICommand DeleteCommand { get; private set; }

        public TemplateListViewModel()
        {
            //TemplateListStoreView = CollectionViewSource.GetDefaultView(Project.Templates);

            
            DeleteCommand = new RelayCommand(() =>
                {
                    //Messenger.Default.Send(new NotificationMessage<System.Action>(() => Project.Templates.Delete(SelectedTemplate), "ConfirmDeleteTemplate"));
                },
                HasSelectedTemplate);
        }

        private bool HasSelectedTemplate()
        {
            return SelectedTemplate != null;
        }
    }
}
