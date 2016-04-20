using Epi;
using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Responder : ViewEntity
    {
        public string ResponderId
        {
            get
            {
                return GetProperty<string>(ColumnNames.GLOBAL_RECORD_ID);
            }
            set
            {
                if (!SetProperty(ColumnNames.GLOBAL_RECORD_ID, value))
                {
                    return;
                }
                OnPropertyChanged("ResponderId");
            }
        }

        public string Username
        {
            get { return GetProperty<string>("Username"); }
            set { SetProperty("Username", value); }
        }

        public string Prefix
        {
            get { return GetProperty<string>("Prefix"); }
            set { SetProperty("Prefix", value); }
        }

        public string FirstName
        {
            get { return GetProperty<string>("FirstName"); }
            set { SetProperty("FirstName", value); }
        }

        public string MiddleInitial
        {
            get { return GetProperty<string>("MiddleInitial"); }
            set { SetProperty("MiddleInitial", value); }
        }

        public string LastName
        {
            get { return GetProperty<string>("LastName"); }
            set { SetProperty("LastName", value); }
        }

        public string Suffix
        {
            get { return GetProperty<string>("Suffix"); }
            set { SetProperty("Suffix", value); }
        }

        public string Alias
        {
            get { return GetProperty<string>("Alias"); }
            set { SetProperty("Alias", value); }
        }

        public DateTime? BirthDate
        {
            get { return GetProperty<DateTime?>("BirthDate"); }
            set { SetProperty("BirthDate", value); }
        }

        public string Gender
        {
            get { return GetProperty<string>("Gender"); }
            set { SetProperty("Gender", value); }
        }

        public int? HeightFeet
        {
            get { return GetProperty<int?>("HeightFeet"); }
            set { SetProperty("HeightFeet", value); }
        }

        public int? HeightInches
        {
            get { return GetProperty<int?>("HeightInches"); }
            set { SetProperty("HeightInches", value); }
        }

        public int? WeightPounds
        {
            get { return GetProperty<int?>("WeightPounds"); }
            set { SetProperty("WeightPounds", value); }
        }

        public string PassportNumber
        {
            get { return GetProperty<string>("PassportNumber"); }
            set { SetProperty("PassportNumber", value); }
        }

        public string AddressLine1
        {
            get { return GetProperty<string>("AddressLine1"); }
            set { SetProperty("AddressLine1", value); }
        }

        public string AddressLine2
        {
            get { return GetProperty<string>("AddressLine2"); }
            set { SetProperty("AddressLine2", value); }
        }

        public string City
        {
            get { return GetProperty<string>("City"); }
            set { SetProperty("City", value); }
        }

        public string State
        {
            get { return GetProperty<string>("State"); }
            set { SetProperty("State", value); }
        }

        public string ZipCode
        {
            get { return GetProperty<string>("ZipCode"); }
            set { SetProperty("ZipCode", value); }
        }

        public string EmailAddress
        {
            get { return GetProperty<string>("EmailAddress"); }
            set { SetProperty("EmailAddress", value); }
        }

        public string PhoneNumber
        {
            get { return GetProperty<string>("PhoneNumber"); }
            set { SetProperty("PhoneNumber", value); }
        }

        public string ContactPrefix
        {
            get { return GetProperty<string>("ContactPrefix"); }
            set { SetProperty("ContactPrefix", value); }
        }

        public string ContactFirstName
        {
            get { return GetProperty<string>("ContactFirstName"); }
            set { SetProperty("ContactFirstName", value); }
        }

        public string ContactMiddleInitial
        {
            get { return GetProperty<string>("ContactMiddleInitial"); }
            set { SetProperty("ContactMiddleInitial", value); }
        }

        public string ContactLastName
        {
            get { return GetProperty<string>("ContactLastName"); }
            set { SetProperty("ContactLastName", value); }
        }

        public string ContactSuffix
        {
            get { return GetProperty<string>("ContactSuffix"); }
            set { SetProperty("ContactSuffix", value); }
        }

        public string ContactAlias
        {
            get { return GetProperty<string>("ContactAlias"); }
            set { SetProperty("ContactAlias", value); }
        }

        public string ContactAddressLine1
        {
            get { return GetProperty<string>("ContactAddressLine1"); }
            set { SetProperty("ContactAddressLine1", value); }
        }

        public string ContactAddressLine2
        {
            get { return GetProperty<string>("ContactAddressLine2"); }
            set { SetProperty("ContactAddressLine2", value); }
        }

        public string ContactCity
        {
            get { return GetProperty<string>("ContactCity"); }
            set { SetProperty("ContactCity", value); }
        }

        public string ContactState
        {
            get { return GetProperty<string>("ContactState"); }
            set { SetProperty("ContactState", value); }
        }

        public string ContactZipCode
        {
            get { return GetProperty<string>("ContactZipCode"); }
            set { SetProperty("ContactZipCode", value); }
        }

        public string ContactEmailAddress
        {
            get { return GetProperty<string>("ContactEmailAddress"); }
            set { SetProperty("ContactEmailAddress", value); }
        }

        public string ContactPhoneNumber
        {
            get { return GetProperty<string>("ContactPhoneNumber"); }
            set { SetProperty("ContactPhoneNumber", value); }
        }

        public string ContactRelationship
        {
            get { return GetProperty<string>("ContactRelationship"); }
            set { SetProperty("ContactRelationship", value); }
        }

        public string OrganizationName
        {
            get { return GetProperty<string>("OrganizationName"); }
            set { SetProperty("OrganizationName", value); }
        }

        public string OrganizationAddressLine1
        {
            get { return GetProperty<string>("OrganizationAddressLine1"); }
            set { SetProperty("OrganizationAddressLine1", value); }
        }

        public string OrganizationAddressLine2
        {
            get { return GetProperty<string>("OrganizationAddressLine2"); }
            set { SetProperty("OrganizationAddressLine2", value); }
        }

        public string OrganizationCity
        {
            get { return GetProperty<string>("OrganizationCity"); }
            set { SetProperty("OrganizationCity", value); }
        }

        public string OrganizationState
        {
            get { return GetProperty<string>("OrganizationState"); }
            set { SetProperty("OrganizationState", value); }
        }

        public string OrganizationZipCode
        {
            get { return GetProperty<string>("OrganizationZipCode"); }
            set { SetProperty("OrganizationZipCode", value); }
        }

        public string OrganizationContactFirstName
        {
            get { return GetProperty<string>("OrganizationContactFirstName"); }
            set { SetProperty("OrganizationContactFirstName", value); }
        }

        public string OrganizationContactLastName
        {
            get { return GetProperty<string>("OrganizationContactLastName"); }
            set { SetProperty("OrganizationContactLastName", value); }
        }

        public string OrganizationEmailAddress
        {
            get { return GetProperty<string>("OrganizationEmailAddress"); }
            set { SetProperty("OrganizationEmailAddress", value); }
        }

        public string OrganizationPhoneNumber
        {
            get { return GetProperty<string>("OrganizationPhoneNumber"); }
            set { SetProperty("OrganizationPhoneNumber", value); }
        }

        public string Occupation
        {
            get { return GetProperty<string>("Occupation"); }
            set { SetProperty("Occupation", value); }
        }

        public bool IsVolunteer
        {
            get { return GetProperty<bool>("IsVolunteer"); }
            set { SetProperty("IsVolunteer", value); }
        }

        public string UnionName
        {
            get { return GetProperty<string>("UnionName"); }
            set { SetProperty("UnionName", value); }
        }

        public string UnionLocalNumber
        {
            get { return GetProperty<string>("UnionLocalNumber"); }
            set { SetProperty("UnionLocalNumber", value); }
        }
    }
}
