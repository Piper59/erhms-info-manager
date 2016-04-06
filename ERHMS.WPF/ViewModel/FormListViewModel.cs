using ERHMS.DataAccess;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.ComponentModel;
using System.IO;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.WPF.ViewModel
{
    public class FormListViewModel : ViewModelBase
    {
        private FileInfo selectedForm;
        public FileInfo SelectedForm
        {
            get { return selectedForm; }
            set { Set(() => selectedForm, ref selectedForm, value); }
        }

        private ICollectionView formList;
        public ICollectionView FormList
        {
            get { return formList; }
            private set { Set(() => formList, ref formList, value); }
        }

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                Set(() => filter, ref filter, value);
                FormList.Filter = ListFilterFunc;
            }
        }

        private bool ListFilterFunc(object item)
        {
            dynamic t = item as FileInfo;

            return Filter == null ||
                Filter.Equals("") ||
                (t.Name != null && t.Name.ToLower().Contains(Filter.ToLower()));
        }

        public RelayCommand AddFormCommand { get; private set; }
        public RelayCommand AddFormFromTemplateCommand { get; private set; }
        public RelayCommand EditFormCommand { get; private set; }
        public RelayCommand DeleteFormCommand { get; private set; }
        public RelayCommand EnterFormDataCommand { get; private set; }
        public RelayCommand OpenFormResponsesCommand { get; private set; }
        public RelayCommand PublishFormToNewProjectCommand { get; private set; }
        public RelayCommand PublishFormToExistingProjectCommand { get; private set; }
        public RelayCommand PublishFormToTemplateCommand { get; private set; }
        public RelayCommand PublishFormToWebCommand { get; private set; }
        public RelayCommand PublishFormToAndroidCommand { get; private set; }
        public RelayCommand ExportFormToPackageCommand { get; private set; }
        public RelayCommand ExportFormToFileCommand { get; private set; }
        public RelayCommand ImportFormFromEpiInfoCommand { get; private set; }
        public RelayCommand ImportFormFromWebCommand { get; private set; }
        public RelayCommand ImportFormFromAndroidCommand { get; private set; }
        public RelayCommand ImportFormFromPackageCommand { get; private set; }
        public RelayCommand ImportFormFromFileCommand { get; private set; }
        public RelayCommand AnalyzeFormClassicCommand { get; private set; }
        public RelayCommand AnalyzeFormVisualCommand { get; private set; }

        public FormListViewModel()
        {       
            FormList = CollectionViewSource.GetDefaultView(App.GetDataContext().Forms.Select());
        }

        private bool HasSelectedTemplate()
        {
            return SelectedForm != null;
        }
    }
}
