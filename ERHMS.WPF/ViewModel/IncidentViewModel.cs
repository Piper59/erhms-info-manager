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
using System.Collections.Generic;

namespace ERHMS.WPF.ViewModel
{
    public class IncidentViewModel : ViewModelBase
    {
        private Incident currentIncident;
        public Incident CurrentIncident
        {
            get { return currentIncident; }
            set
            {
                Set(() => CurrentIncident, ref currentIncident, value);
            }
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
            set
            {
                Set(() => SelectedAvailableResponders, ref selectedAvailableResponders, value);

                AddToRosterCommand.RaiseCanExecuteChanged();
                RemoveFromRosterCommand.RaiseCanExecuteChanged();
                ViewResponderDetailsCommand.RaiseCanExecuteChanged();
            }
        }
        private string availableResponderFilter;
        public string AvailableResponderFilter
        {
            get { return availableResponderFilter; }
            set
            {
                Set(() => AvailableResponderFilter, ref availableResponderFilter, value);
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
            set
            {
                Set(() => SelectedRosteredResponders, ref selectedRosteredResponders, value);

                AddToRosterCommand.RaiseCanExecuteChanged();
                RemoveFromRosterCommand.RaiseCanExecuteChanged();
                ViewResponderDetailsCommand.RaiseCanExecuteChanged();
            }
        }
        private string rosteredResponderFilter;
        public string RosteredResponderFilter
        {
            get { return rosteredResponderFilter; }
            set
            {
                Set(() => RosteredResponderFilter, ref rosteredResponderFilter, value);
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

        private bool HasSelectedAvailableResponder()
        {
            if (SelectedAvailableResponders == null)
                return false;
            return SelectedAvailableResponders.Count > 0;
        }

        private bool HasSelectedRosteredResponder()
        {
            if (SelectedRosteredResponders == null)
                return false;
            return SelectedRosteredResponders.Count > 0;
        }
        private bool HasSelectedRoster()
        {
            if (SelectedAvailableResponders != null && SelectedAvailableResponders.Count > 0)
                return true;
            if (SelectedRosteredResponders != null && SelectedRosteredResponders.Count > 0)
                return true;

            return false;
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
            
            SaveIncidentDetailsCommand = new RelayCommand(() =>
            {
                try
                {
                    //check for required fields
                    List<string> missingData = new List<string>();

                    if (string.IsNullOrEmpty(CurrentIncident.Name))
                    {
                        missingData.Add("Incident Name");
                    }
                    if (string.IsNullOrEmpty(CurrentIncident.Phase.ToString()))
                    {
                        missingData.Add("Incident Phase");
                    }

                    if (missingData.Count() > 0)
                    {
                        Messenger.Default.Send(new NotificationMessage<string>("The following fields are required:\n\n" + string.Join("\n", missingData), "ShowErrorMessage"));
                    }
                    else
                    {
                        App.GetDataContext().Incidents.Save(CurrentIncident);

                        Messenger.Default.Send(new NotificationMessage<string>("Incident has been saved.", "ShowSuccessMessage"));
                    }
                }
                catch (Exception e)
                {
                    Messenger.Default.Send(new NotificationMessage<string>("Error while saving the incident information.  Details: " + e.Message + ".", "ShowErrorMessage"));
                }
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

            AddToRosterCommand = new RelayCommand(() =>
            {
                for (int i = SelectedAvailableResponders.Count - 1; i >= 0; i--)
                {
                    Responder responder = (Responder)SelectedAvailableResponders[i];

                    Registration registration = App.GetDataContext().Registrations.Create();
                    registration.ResponderId = responder.GlobalRecordId;
                    registration.IncidentId = CurrentIncident.IncidentId;

                    App.GetDataContext().Registrations.Save(registration);
                }
            },
                HasSelectedAvailableResponder);

            RemoveFromRosterCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<System.Action>(() =>
                {
                    Responder responder = (Responder)SelectedRosteredResponders[0];
                    Registration registration = App.GetDataContext().Registrations.Select().Where(q => q.ResponderId == responder.GlobalRecordId && q.IncidentId == CurrentIncident.IncidentId).FirstOrDefault();

                    App.GetDataContext().Registrations.Delete(registration);

                }, "ConfirmDeleteRegistration"));
            },
                HasSelectedRosteredResponder);

            ViewResponderDetailsCommand = new RelayCommand(() =>
            {
                Responder selectedResponder;

                if (SelectedAvailableResponders.Count > 0)
                    selectedResponder = (Responder)SelectedAvailableResponders[0];
                else
                    selectedResponder = (Responder)SelectedRosteredResponders[0];

                Messenger.Default.Send(new NotificationMessage<Responder>((Responder)selectedResponder.Clone(), "ShowEditResponder"));
            },
                HasSelectedRoster);

            List<string> rosterIds = App.GetDataContext().Registrations.Select().Where(q => q.IncidentId == CurrentIncident.IncidentId).Select(q => q.ResponderId).ToList();

            availableResponders = new CollectionViewSource();
            availableResponders.Source = App.GetDataContext().Responders.SelectByDeleted(false).Where(q => rosterIds.Contains(q.GlobalRecordId) == false);
            SelectedAvailableResponders = null;

            rosteredResponders = new CollectionViewSource();
            rosteredResponders.Source = App.GetDataContext().Responders.SelectByDeleted(false).Where(q => rosterIds.Contains(q.GlobalRecordId) == true);
            SelectedRosteredResponders = null;
        }
    }
}
