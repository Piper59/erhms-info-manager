using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System;
using System.Linq;
using ERHMS.EpiInfo.Domain;
using ERHMS.DataAccess;
using ERHMS.Domain;

namespace ERHMS.WPF.ViewModel
{
    public class ResponderViewModel : ViewModelBase
    {
        public List<string> HeightOptions
        {
            get;
            private set;
        }
        private ObservableCollection<string> prefixes;
        public ObservableCollection<string> Prefixes
        {
            get { return prefixes; }
            set { Set(() => prefixes, ref prefixes, value); }
        }
        private ObservableCollection<string> suffixes;
        public ObservableCollection<string> Suffixes
        {
            get { return suffixes; }
            set { Set(() => suffixes, ref suffixes, value); }
        }
        private ObservableCollection<string> genders;
        public ObservableCollection<string> Genders
        {
            get { return genders; }
            set { Set(() => genders, ref genders, value); }
        }
        private ObservableCollection<string> states;
        public ObservableCollection<string> States
        {
            get { return states; }
            set { Set(() => states, ref states, value); }
        }
        //private IEnumerable<ViewEntity> roles;
        //public IEnumerable<ViewEntity> Roles
        //{
        //    get { return roles; }
        //    set { Set(() => roles, ref roles, value); }
        //}

        public string SelectedHeightFeet
        {
            get { return SelectedResponder != null && SelectedResponder.GetProperty("HeightFeet") != null ? HeightOptions.FirstOrDefault(q => q == (string)SelectedResponder.GetProperty("HeightFeet").ToString()) : null; }
            set { SelectedResponder.SetProperty("HeightFeet", value); }
        }
        public string SelectedHeightInches
        {
            get { return SelectedResponder != null && SelectedResponder.GetProperty("HeightInches") != null ? HeightOptions.FirstOrDefault(q => q == (string)SelectedResponder.GetProperty("HeightInches").ToString()) : null; }
            set { SelectedResponder.SetProperty("HeightInches", value); }
        }

        public string SelectedPrefix
        {
            get { return SelectedResponder != null && SelectedResponder.GetProperty("Prefix") != null ? Prefixes.FirstOrDefault(q => q == (string)SelectedResponder.GetProperty("Prefix")) : null; }
            set { SelectedResponder.SetProperty("Prefix", value); }
        }
        public string SelectedSuffix
        {
            get { return SelectedResponder != null && SelectedResponder.GetProperty("Suffix") != null ? Suffixes.FirstOrDefault(q => q == (string)SelectedResponder.GetProperty("Suffix")) : null; }
            set { SelectedResponder.SetProperty("Suffix", value); }
        }
        public string SelectedGender
        {
            get { return SelectedResponder != null && SelectedResponder.GetProperty("Gender") != null ? Genders.FirstOrDefault(q => q == (string)SelectedResponder.GetProperty("Gender")) : null; }
            set { SelectedResponder.SetProperty("Gender", value); }
        }
        public string SelectedResponderPersonState
        {
            get { return SelectedResponder != null && SelectedResponder.GetProperty("State") != null ? States.FirstOrDefault(q => q == (string)SelectedResponder.GetProperty("State")) : null; }
            set { SelectedResponder.SetProperty("State", value); }
        }
        public string SelectedResponderOrganizationState
        {
            get { return SelectedResponder != null && SelectedResponder.GetProperty("OrganizationState") != null ? States.FirstOrDefault(q => q == (string)SelectedResponder.GetProperty("OrganizationState")) : null; }
            set { SelectedResponder.SetProperty("OrganizationState", value); }
        }
        public string SelectedResponderContactState
        {
            get { return SelectedResponder != null && SelectedResponder.GetProperty("ContactState") != null ? States.FirstOrDefault(q => q == (string)SelectedResponder.GetProperty("ContactState")).ToString() : null; }
            set { SelectedResponder.SetProperty("ContactState", value); }
        }        
  
        private Responder selectedResponder;
        public Responder SelectedResponder
        {
            get { return selectedResponder; }
            set
            {
                Set(() => selectedResponder, ref selectedResponder, value);

                RaisePropertyChanged("SelectedPrefix");
                RaisePropertyChanged("SelectedSuffix");
                RaisePropertyChanged("SelectedGender");
                RaisePropertyChanged("SelectedResponderPersonState");
                RaisePropertyChanged("SelectedResponderOrganizationState");
                RaisePropertyChanged("SelectedResponderContactState");
                RaisePropertyChanged("SelectedRole");
            }
        }

        #region Commands

        private RelayCommand saveResponderCommand;
        public RelayCommand SaveResponderCommand
        {
            get
            {
                return saveResponderCommand ?? (saveResponderCommand = new RelayCommand(
                    () =>
                    {
                        try
                        {
                            //check for required fields
                            List<string> missingData = new List<string>();
                            dynamic responder = SelectedResponder;
                            if (string.IsNullOrEmpty(responder.ResponderId))
                            {
                                missingData.Add("ResponderId");
                            }
                            if (string.IsNullOrEmpty(responder.Username))
                            {
                                missingData.Add("Username");
                            }
                            if (string.IsNullOrEmpty(responder.FirstName))
                            {
                                missingData.Add("First Name");
                            }
                            if (string.IsNullOrEmpty(responder.LastName))
                            {
                                missingData.Add("Last Name");
                            }
                            if (string.IsNullOrEmpty(responder.EmailAddress))
                            {
                                missingData.Add("Email Address");
                            }
                            if (responder.BirthDate == null)
                            {
                                missingData.Add("Birth Date");
                            }
                            if (responder.ResponderId == null)
                            {
                                missingData.Add("Responder Id");
                            }

                            if (missingData.Count() > 0)
                            {
                                Messenger.Default.Send(new NotificationMessage<string>("The following fields are required:\n\n" + string.Join("\n", missingData), "ShowErrorMessage"));
                            }
                            else
                            {
                                App.GetDataContext().Responders.Save(SelectedResponder);

                                Messenger.Default.Send(new NotificationMessage("RefreshResponders"));

                                Messenger.Default.Send(new NotificationMessage<string>("Responder has been saved.", "ShowSuccessMessage"));
                            }
                        }
                        catch (Exception e)
                        {
                            Messenger.Default.Send(new NotificationMessage<string>("Error while saving the responder information.  Details: " + e.Message + ".", "ShowErrorMessage"));
                        }   
                    }
                    ));
            }
        }

        #endregion

        /// <summary>
        /// Initializes a new instance of the ResponderWindowViewModel class.
        /// </summary>
        public ResponderViewModel(Responder responder)
        {
            SelectedResponder = responder;

            Initialize();
        }

        public ResponderViewModel()
        {
            SelectedResponder = App.GetDataContext().Responders.Create();

            Initialize();
        }

        private void Initialize()
        {
            HeightOptions = new List<string>() { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "10", "11" };

            Prefixes = new ObservableCollection<string>(App.GetDataContext().Prefixes.Select());
            Suffixes = new ObservableCollection<string>(App.GetDataContext().Suffixes.Select());
            Genders = new ObservableCollection<string>(App.GetDataContext().Genders.Select());
            States = new ObservableCollection<string>(App.GetDataContext().States.Select());
        }
    }
}