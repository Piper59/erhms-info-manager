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

        private ICollectionView templateListStoreView;
        public ICollectionView TemplateListStoreView
        {
            get { return templateListStoreView; }
            private set { Set(ref templateListStoreView, value); }
        }

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                Set(ref filter, value);
                TemplateListStoreView.Filter = ListFilter;
            }
        }

        private bool ListFilter(object item)
        {
            dynamic t = item as FileInfo;

            return Filter == null ||
                Filter.Equals("") ||
                (t.Name != null && t.Name.ToLower().Contains(Filter.ToLower()));
        }

        public ICommand InstantiateCommand { get; private set; }
        public ICommand DeleteCommand { get; private set; }

        public TemplateListViewModel()
        {
            //TemplateListStoreView = CollectionViewSource.GetDefaultView(Project.Templates);

            
            InstantiateCommand = new RelayCommand(() =>
                {
                    //Project.InstantiateViewTemplate(SelectedTemplate.FullName);
                   // Project.Views.Refresh();
                    Messenger.Default.Send(new NotificationMessage<string>("The template has been copied. You may view these under the Forms tab.", "ShowSuccessMessage"));

                },
                HasSelectedTemplate);
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
