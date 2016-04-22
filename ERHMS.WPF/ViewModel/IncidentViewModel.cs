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
using ERHMS.EpiInfo.MakeView;
using ERHMS.EpiInfo.Enter;
using ERHMS.EpiInfo;
using ERHMS.EpiInfo.ImportExport;
using ERHMS.EpiInfo.Analysis;
using ERHMS.EpiInfo.AnalysisDashboard;
using ERHMS.EpiInfo.Communication;
using Action = System.Action;
using Project = ERHMS.EpiInfo.Project;
using System.Windows.Threading;

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
        private string availableRespondersFilter;
        public string AvailableRespondersFilter
        {
            get { return availableRespondersFilter; }
            set
            {
                Set(() => AvailableRespondersFilter, ref availableRespondersFilter, value);
                AvailableResponders.Filter = AvailableRespondersFilterFunc;
            }
        }
        private bool AvailableRespondersFilterFunc(object item)
        {
            Responder r = item as Responder;

            return (AvailableRespondersFilter == null ||
                AvailableRespondersFilter.Equals("") ||
                (r.Username != null && r.Username.ToLower().Contains(AvailableRespondersFilter.ToLower())) ||
                (r.FirstName != null && r.FirstName.ToLower().Contains(AvailableRespondersFilter.ToLower())) ||
                (r.LastName != null && r.LastName.ToLower().Contains(AvailableRespondersFilter.ToLower())) ||
                (r.City != null && r.City.ToLower().Contains(AvailableRespondersFilter.ToLower())) ||
                (r.State != null && r.State.ToLower().Contains(AvailableRespondersFilter.ToLower())) ||
                (r.EmailAddress != null && r.EmailAddress.ToLower().Contains(AvailableRespondersFilter.ToLower())) ||
                (r.OrganizationName != null && r.OrganizationName.ToLower().Contains(AvailableRespondersFilter.ToLower())) ||
                (r.Occupation != null && r.Occupation.ToLower().Contains(AvailableRespondersFilter.ToLower())));
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
        private string rosteredRespondersFilter;
        public string RosteredRespondersFilter
        {
            get { return rosteredRespondersFilter; }
            set
            {
                Set(() => RosteredRespondersFilter, ref rosteredRespondersFilter, value);
                RosteredResponders.Filter = RosteredRespondersFilterFunc;
            }
        }
        private bool RosteredRespondersFilterFunc(object item)
        {
            Responder r = item as Responder;

            return (RosteredRespondersFilter == null ||
                RosteredRespondersFilter.Equals("") ||
                (r.Username != null && r.Username.ToLower().Contains(RosteredRespondersFilter.ToLower())) ||
                (r.FirstName != null && r.FirstName.ToLower().Contains(RosteredRespondersFilter.ToLower())) ||
                (r.LastName != null && r.LastName.ToLower().Contains(RosteredRespondersFilter.ToLower())) ||
                (r.City != null && r.City.ToLower().Contains(RosteredRespondersFilter.ToLower())) ||
                (r.State != null && r.State.ToLower().Contains(RosteredRespondersFilter.ToLower())) ||
                (r.EmailAddress != null && r.EmailAddress.ToLower().Contains(RosteredRespondersFilter.ToLower())) ||
                (r.OrganizationName != null && r.OrganizationName.ToLower().Contains(RosteredRespondersFilter.ToLower())) ||
                (r.Occupation != null && r.Occupation.ToLower().Contains(RosteredRespondersFilter.ToLower())));
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

        private CollectionViewSource locationList;
        public ICollectionView LocationList
        {
            get { return locationList != null ? locationList.View : null; }
        }
        private string locationListFilter;
        public string LocationListFilter
        {
            get { return locationListFilter; }
            set
            {
                Set(() => LocationListFilter, ref locationListFilter, value);
                LocationList.Filter = LocationListFilterFunc;
            }
        }

        private bool LocationListFilterFunc(object item)
        {
            Location l = item as Location;

            return
                LocationListFilter.Equals("") ||
                (l.Name != null && l.Name.ToLower().Contains(LocationListFilter.ToLower())) ||
                (l.Description != null && l.Description.ToLower().Contains(LocationListFilter.ToLower())) ||
                (l.Address != null && l.Address.ToLower().Contains(LocationListFilter.ToLower()));
        }

        public RelayCommand ViewLocationDetailsCommand { get; private set; }
        public RelayCommand AddLocationCommand { get; private set; }
        public RelayCommand DeleteLocationCommand { get; private set; }

        private bool HasSelectedLocation()
        {
            return SelectedLocation != null;
        }
        #endregion

        #region Forms
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
        public RelayCommand CopyTemplateCommand { get; private set; }

        private bool HasSelectedForm()
        {
            return SelectedForm != null;
        }

        private bool CanDeleteForm()
        {
            return HasSelectedForm();// && !Project.GetSystemViewNames().Contains(SelectedForm.Name);
        }
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
                Messenger.Default.Send(new NotificationMessage<Incident>(CurrentIncident, "ShowNewLocation"));
            });
            ViewLocationDetailsCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<Location>((Location)SelectedLocation.Clone(), "ShowEditLocation"));
            }, HasSelectedLocation);
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

                RefreshRosterData();
                
            }, HasSelectedAvailableResponder);

            RemoveFromRosterCommand = new RelayCommand(() =>
            {
                Messenger.Default.Send(new NotificationMessage<System.Action>(() =>
                {
                    Responder responder = (Responder)SelectedRosteredResponders[0];
                    Registration registration = App.GetDataContext().Registrations.Select().Where(q => q.ResponderId == responder.GlobalRecordId && q.IncidentId == CurrentIncident.IncidentId).FirstOrDefault();

                    App.GetDataContext().Registrations.Delete(registration);

                    RefreshRosterData();
                }, "ConfirmDeleteRegistration"));
            }, HasSelectedRosteredResponder);

            ViewResponderDetailsCommand = new RelayCommand(() =>
            {
                if (SelectedAvailableResponders != null && SelectedAvailableResponders.Count > 0)
                    Messenger.Default.Send(new NotificationMessage<Responder>((Responder)((Responder)SelectedAvailableResponders[0]).Clone(), "ShowEditResponder"));
                else if (SelectedRosteredResponders != null && SelectedRosteredResponders.Count > 0)
                    Messenger.Default.Send(new NotificationMessage<Responder>((Responder)((Responder)SelectedRosteredResponders[0]).Clone(), "ShowEditResponder"));
            }, HasSelectedRoster);

            AddFormCommand = new RelayCommand(() =>
            {
                Epi.View view = MakeView.AddView(App.GetDataContext().Project);
                MakeView.OpenView(view);

                ViewLink vl = App.GetDataContext().ViewLinks.Create();
                vl.IncidentId = CurrentIncident.IncidentId;
                vl.ViewId = view.Id;

                App.GetDataContext().ViewLinks.Save(vl);

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

            SetupListeners();

            RefreshRosterData();
            RefreshLocationData();
            RefreshFormData();
        }

        private void SetupListeners()
        {
            App.Current.Service.RefreshingViews += Service_RefreshingViews;

            Messenger.Default.Register<NotificationMessage>(this, (msg) =>
            {
                if (msg.Notification == "RefreshResponders")
                {
                    RefreshRosterData();
                }
            });

            Messenger.Default.Register<NotificationMessage<string>>(this, (msg) =>
            {
                if (msg.Notification == "RefreshLocations" && msg.Content == CurrentIncident.IncidentId)
                {
                    RefreshLocationData();
                }
            });
        }

        private void Service_RefreshingViews(object sender, ViewEventArgs e)
        {
            if (e.Tag == CurrentIncident.IncidentId)
                RefreshFormData();
        }

        private void RefreshLocationData()
        {
            locationList = new CollectionViewSource();
            locationList.Source = App.GetDataContext().Locations.Select().Where(q => q.IncidentId == CurrentIncident.IncidentId);
            LocationList.Refresh();
            RaisePropertyChanged("LocationList");
            SelectedLocation = null;
        }
        private void RefreshFormData()
        {
            formList = new CollectionViewSource();
            formList.Source = App.GetDataContext().GetLinkedViews(CurrentIncident.IncidentId);
            FormList.Refresh();
            RaisePropertyChanged("FormList");
            SelectedForm = null;
        }
        private void RefreshRosterData()
        {
            List<string> rosterIds = App.GetDataContext().Registrations.Select().Where(q => q.IncidentId == CurrentIncident.IncidentId).Select(q => q.ResponderId).Distinct().ToList();

            availableResponders = new CollectionViewSource();
            availableResponders.Source = App.GetDataContext().Responders.SelectByDeleted(false).Where(q => rosterIds.Contains(q.GlobalRecordId) == false);
            AvailableResponders.Refresh();
            RaisePropertyChanged("AvailableResponders");
            SelectedAvailableResponders = null;

            rosteredResponders = new CollectionViewSource();
            rosteredResponders.Source = App.GetDataContext().Responders.SelectByDeleted(false).Where(q => rosterIds.Contains(q.GlobalRecordId) == true);
            RosteredResponders.Refresh();
            RaisePropertyChanged("RosteredResponders");
            SelectedRosteredResponders = null;
        }
    }
}
