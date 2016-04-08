using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Windows.Data;
using System.ComponentModel;
using ERHMS.DataAccess;
using ERHMS.Domain;
using System.Linq;
using ERHMS.EpiInfo.Domain;

namespace ERHMS.WPF.ViewModel
{
    public class IncidentViewModel : ViewModelBase
    {
        public Incident CurrentIncident
        {
            get;
            set;
        }

        public RelayCommand SaveIncidentDetailsCommand { get; private set; }


        #region Roster
        //Commands
        public RelayCommand AddToRosterCommand { get; private set; }
        public RelayCommand RemoveFromRosterCommand { get; private set; }
        public RelayCommand ViewResponderDetailsCommand { get; private set; }

        //available responders for the incident
        private CollectionViewSource availableResponders;
        public ICollectionView AvailableResponders
        {
            get { return availableResponders != null ? availableResponders.View : null; }
        }
        private IList selectedAvailableResponders = new ArrayList();
        public IList SelectedAvailableResponders
        {
            get { return selectedAvailableResponders; }
            set { Set(() => selectedAvailableResponders, ref selectedAvailableResponders, value); }
        }
        private string availableResponderFilter;
        public string AvailableResponderFilter
        {
            get { return availableResponderFilter; }
            set
            {
                Set(() => availableResponderFilter, ref availableResponderFilter, value);
                AvailableResponders.Filter = AvailableRespondersFilterFunc;
            }
        }
        private bool AvailableRespondersFilterFunc(object item)
        {
            dynamic r = item as ViewEntity;

            return r.IsDeleted() == false &&
                //HasRegistration(r) == false &&
                (AvailableResponderFilter == null ||
                AvailableResponderFilter.Equals("") ||
                //(r.Username != null && r.Username.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.FirstName != null && r.FirstName.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.LastName != null && r.LastName.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.City != null && r.City.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.State != null && r.State.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                //(r.EmailAddress != null && r.EmailAddress.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.OrganizationName != null && r.OrganizationName.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.Occupation != null && r.Occupation.ToLower().Contains(AvailableResponderFilter.ToLower())));
        }

        //responders currently on the roster
        private CollectionViewSource rosteredResponders;
        public ICollectionView RosteredResponders
        {
            get { return rosteredResponders != null ? rosteredResponders.View : null; }
        }

        private IList selectedRosteredResponders = new ArrayList();
        public IList SelectedRosteredResponders
        {
            get { return selectedRosteredResponders; }
            set { Set(() => selectedRosteredResponders, ref selectedRosteredResponders, value); }
        }
        private string rosteredResponderFilter;
        public string RosteredResponderFilter
        {
            get { return rosteredResponderFilter; }
            set
            {
                Set(() => rosteredResponderFilter, ref rosteredResponderFilter, value);
                RosteredResponders.Filter = RosteredRespondersFilterFunc;
            }
        }
        private bool RosteredRespondersFilterFunc(object item)
        {
            dynamic r = item as ViewEntity;

            return r.IsDeleted() == false &&
                //HasRegistration(r) == false &&
                (AvailableResponderFilter == null ||
                AvailableResponderFilter.Equals("") ||
                //(r.Username != null && r.Username.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.FirstName != null && r.FirstName.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.LastName != null && r.LastName.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.City != null && r.City.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.State != null && r.State.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                //(r.EmailAddress != null && r.EmailAddress.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.OrganizationName != null && r.OrganizationName.ToLower().Contains(AvailableResponderFilter.ToLower())) ||
                (r.Occupation != null && r.Occupation.ToLower().Contains(AvailableResponderFilter.ToLower())));
        }
        #endregion

        #region Locations
        public Location SelectedLocation;

        private ICollectionView locationList;
        public ICollectionView LocationList
        {
            get { return locationList; }
            private set { Set(() => locationList, ref locationList, value); }
        }
        private string locationListFilter;
        public string LocationListFilter
        {
            get { return locationListFilter; }
            set
            {
                Set(() => locationListFilter, ref locationListFilter, value);
                LocationList.Filter = LocationListFilterFunc;
            }
        }

        private bool LocationListFilterFunc(object item)
        {
            dynamic l = item as ViewEntity;

            return
                LocationListFilter.Equals("") ||
                (l.Name != null && l.Name.ToLower().Contains(LocationListFilter.ToLower())) ||
                (l.Description != null && l.Description.ToLower().Contains(LocationListFilter.ToLower())) ||
                (l.Address != null && l.Address.ToLower().Contains(LocationListFilter.ToLower()));
        }

        public RelayCommand ViewLocationDetailsCommand;
        public RelayCommand AddLocationCommand;
        public RelayCommand DeleteLocationCommand;


        private bool HasSelectedLocation()
        {
            return SelectedLocation != null;
        }
        #endregion

        #region Forms
        public ViewEntity SelectedForm;

        private ICollectionView formList;
        public ICollectionView FormList
        {
            get { return formList; }
            private set { Set(() => formList, ref formList, value); }
        }
        private string formListFilter;
        public string FormListFilter
        {
            get { return formListFilter; }
            set
            {
                Set(() => formListFilter, ref formListFilter, value);
                FormList.Filter = FormListFilterFunc;
            }
        }

        private bool FormListFilterFunc(object item)
        {
            dynamic f = item as ViewEntity;

            return
                FormListFilter.Equals("") ||
                (f.Name != null && f.Name.ToLower().Contains(FormListFilter.ToLower())) ||
                (f.Description != null && f.Description.ToLower().Contains(FormListFilter.ToLower()));
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
        #endregion

        public IncidentViewModel()
        {
            CurrentIncident = App.GetDataContext().Incidents.Create();
            
            Initialize();
        }

        public IncidentViewModel(Incident incident)
        {
            CurrentIncident = incident;

            Initialize();
        }

        private void Initialize()
        { 
            LocationList = CollectionViewSource.GetDefaultView(App.GetDataContext().Locations.Select().Where(q => q.IncidentId == CurrentIncident.IncidentId));
            FormList = CollectionViewSource.GetDefaultView(App.GetDataContext().ViewLinks.Select().Where(q => q.IncidentId == CurrentIncident.IncidentId));

            availableResponders = new CollectionViewSource();
            availableResponders.Source = App.GetDataContext().Responders.Select();

            rosteredResponders = new CollectionViewSource();
            //rosteredResponders.Source = 

            SaveIncidentDetailsCommand = new RelayCommand(() =>
            {
                App.GetDataContext().Incidents.Save(CurrentIncident);
            });

            AddLocationCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<Location>(SelectedLocation, "ShowEditLocation"));
            });
            ViewLocationDetailsCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage("ShowNewLocation"));
            });
            DeleteLocationCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<Action>(() =>
                {
                    App.GetDataContext().Locations.Delete(SelectedLocation);
                }, "ConfirmDeleteLocation"));
            }, HasSelectedLocation);
        }
    }
}
