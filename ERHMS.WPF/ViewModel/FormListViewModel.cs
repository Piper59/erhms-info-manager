using ERHMS.DataAccess;
using ERHMS.Domain;
using ERHMS.EpiInfo.Communication;
using ERHMS.EpiInfo.Enter;
using ERHMS.EpiInfo.ImportExport;
using ERHMS.EpiInfo.MakeView;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Data;
using System.Windows.Input;

namespace ERHMS.WPF.ViewModel
{
    public class FormListViewModel : ViewModelBase
    {
        private Epi.View selectedForm;
        public Epi.View SelectedForm
        {
            get { return selectedForm; }
            set
            {
                Set(() => SelectedForm, ref selectedForm, value);

                AddFormCommand.RaiseCanExecuteChanged();
                AddFormFromTemplateCommand.RaiseCanExecuteChanged();
                EditFormCommand.RaiseCanExecuteChanged();
                DeleteFormCommand.RaiseCanExecuteChanged();
                EnterFormDataCommand.RaiseCanExecuteChanged();
                OpenFormResponsesCommand.RaiseCanExecuteChanged();
                PublishFormCommand.RaiseCanExecuteChanged();
                PublishFormToNewProjectCommand.RaiseCanExecuteChanged();
                PublishFormToExistingProjectCommand.RaiseCanExecuteChanged();
                PublishFormToTemplateCommand.RaiseCanExecuteChanged();
                PublishFormToWebCommand.RaiseCanExecuteChanged();
                PublishFormToAndroidCommand.RaiseCanExecuteChanged();
                ExportFormCommand.RaiseCanExecuteChanged();
                ExportFormToPackageCommand.RaiseCanExecuteChanged();
                ExportFormToFileCommand.RaiseCanExecuteChanged();
                ImportFormCommand.RaiseCanExecuteChanged();
                ImportFormFromEpiInfoCommand.RaiseCanExecuteChanged();
                ImportFormFromWebCommand.RaiseCanExecuteChanged();
                ImportFormFromAndroidCommand.RaiseCanExecuteChanged();
                ImportFormFromPackageCommand.RaiseCanExecuteChanged();
                ImportFormFromFileCommand.RaiseCanExecuteChanged();
            }
        }

        private CollectionViewSource formList;
        public ICollectionView FormList
        {
            get { return formList != null ? formList.View : null; }
        }

        private string formListFilter;
        public string FormListFilter
        {
            get { return formListFilter; }
            set
            {
                Set(() => FormListFilter, ref formListFilter, value);
                FormList.Filter = FormListFilterFunc;
            }
        }

        private bool FormListFilterFunc(object item)
        {
            ViewLink vl = item as ViewLink;
            Epi.View v = App.GetDataContext().GetViews().Where(q => q.Id == vl.ViewId).First();

            return
                FormListFilter.Equals("") ||
                (v.Name != null && v.Name.ToLower().Contains(FormListFilter.ToLower()));
        }

        public RelayCommand AddFormCommand { get; private set; }
        public RelayCommand AddFormFromTemplateCommand { get; private set; }
        public RelayCommand EditFormCommand { get; private set; }
        public RelayCommand DeleteFormCommand { get; private set; }
        public RelayCommand EnterFormDataCommand { get; private set; }
        public RelayCommand OpenFormResponsesCommand { get; private set; }
        public RelayCommand PublishFormCommand { get; private set; }
        public RelayCommand PublishFormToNewProjectCommand { get; private set; }
        public RelayCommand PublishFormToExistingProjectCommand { get; private set; }
        public RelayCommand PublishFormToTemplateCommand { get; private set; }
        public RelayCommand PublishFormToWebCommand { get; private set; }
        public RelayCommand PublishFormToAndroidCommand { get; private set; }
        public RelayCommand ExportFormCommand { get; private set; }
        public RelayCommand ExportFormToPackageCommand { get; private set; }
        public RelayCommand ExportFormToFileCommand { get; private set; }
        public RelayCommand ImportFormCommand { get; private set; }
        public RelayCommand ImportFormFromEpiInfoCommand { get; private set; }
        public RelayCommand ImportFormFromWebCommand { get; private set; }
        public RelayCommand ImportFormFromAndroidCommand { get; private set; }
        public RelayCommand ImportFormFromPackageCommand { get; private set; }
        public RelayCommand ImportFormFromFileCommand { get; private set; }

        private bool HasSelectedForm()
        {
            return SelectedForm != null;
        }

        private bool CanDeleteForm()
        {
            return HasSelectedForm();// && !Project.GetSystemViewNames().Contains(SelectedForm.Name);
        }

        public FormListViewModel()
        {
            AddFormCommand = new RelayCommand(() =>
            {
                Epi.View view = MakeView.AddView(App.GetDataContext().Project);
                MakeView.OpenView(view);

                RefreshFormData();
            });
            AddFormFromTemplateCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")));
            EditFormCommand = new RelayCommand(() => MakeView.OpenView(SelectedForm), HasSelectedForm);
            EnterFormDataCommand = new RelayCommand(() => Enter.OpenView(SelectedForm), HasSelectedForm);
            DeleteFormCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<Action>(() =>
                {
                    App.GetDataContext().DeleteView(SelectedForm.Id);
                }, "ConfirmDeleteForm"));
            },
                CanDeleteForm);
            OpenFormResponsesCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<Epi.View>(SelectedForm, "ShowResponseList"));
            }, HasSelectedForm);
            PublishFormCommand = new RelayCommand(() => { }, HasSelectedForm);
            PublishFormToNewProjectCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);
            PublishFormToExistingProjectCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);
            PublishFormToTemplateCommand = new RelayCommand(() => MakeView.CreateTemplate(SelectedForm), HasSelectedForm);
            PublishFormToWebCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);
            PublishFormToAndroidCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);
            ImportFormCommand = new RelayCommand(() => { }, HasSelectedForm);
            ImportFormFromEpiInfoCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);
            ImportFormFromWebCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);
            ImportFormFromAndroidCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);
            ImportFormFromPackageCommand = new RelayCommand(() => ImportExport.ImportFromPackage(SelectedForm), HasSelectedForm);
            ImportFormFromFileCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);
            ExportFormCommand = new RelayCommand(() => { }, HasSelectedForm);
            ExportFormToPackageCommand = new RelayCommand(() => ImportExport.ExportToPackage(SelectedForm), HasSelectedForm);
            ExportFormToFileCommand = new RelayCommand(() => Messenger.Default.Send(new NotificationMessage<string>("Not implemented.", "ShowErrorMessage")), HasSelectedForm);

            App.Current.Service.RefreshingViews += Service_RefreshingViews;

            RefreshFormData();
        }

        private void Service_RefreshingViews(object sender, ViewEventArgs e)
        {
            if (e.Tag == null)
                RefreshFormData();
        }

        private void RefreshFormData()
        {
            formList = new CollectionViewSource();
            formList.Source = App.GetDataContext().GetUnlinkedViews();
            FormList.Refresh();
            RaisePropertyChanged("FormList");
            SelectedForm = null;
        }
    }
}
