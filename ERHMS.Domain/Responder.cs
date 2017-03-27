using Epi;
using ERHMS.EpiInfo.Domain;
using System;

namespace ERHMS.Domain
{
    public class Responder : ViewEntity
    {
        public string ResponderId
        {
            get { return GlobalRecordId; }
            set { GlobalRecordId = value; }
        }

        public string Prefix
        {
            get { return GetProperty<string>(nameof(Prefix)); }
            set { SetProperty(nameof(Prefix), value); }
        }

        public string FirstName
        {
            get { return GetProperty<string>(nameof(FirstName)); }
            set { SetProperty(nameof(FirstName), value); }
        }

        public string MiddleInitial
        {
            get { return GetProperty<string>(nameof(MiddleInitial)); }
            set { SetProperty(nameof(MiddleInitial), value); }
        }

        public string LastName
        {
            get { return GetProperty<string>(nameof(LastName)); }
            set { SetProperty(nameof(LastName), value); }
        }

        public string Suffix
        {
            get { return GetProperty<string>(nameof(Suffix)); }
            set { SetProperty(nameof(Suffix), value); }
        }

        public string Alias
        {
            get { return GetProperty<string>(nameof(Alias)); }
            set { SetProperty(nameof(Alias), value); }
        }

        public DateTime? BirthDate
        {
            get { return GetProperty<DateTime?>(nameof(BirthDate)); }
            set { SetProperty(nameof(BirthDate), value); }
        }

        public string Gender
        {
            get { return GetProperty<string>(nameof(Gender)); }
            set { SetProperty(nameof(Gender), value); }
        }

        public double? HeightFeet
        {
            get { return GetProperty<double?>(nameof(HeightFeet)); }
            set { SetProperty(nameof(HeightFeet), value); }
        }

        public double? HeightInches
        {
            get { return GetProperty<double?>(nameof(HeightInches)); }
            set { SetProperty(nameof(HeightInches), value); }
        }

        public double? WeightPounds
        {
            get { return GetProperty<double?>(nameof(WeightPounds)); }
            set { SetProperty(nameof(WeightPounds), value); }
        }

        public string PassportNumber
        {
            get { return GetProperty<string>(nameof(PassportNumber)); }
            set { SetProperty(nameof(PassportNumber), value); }
        }

        public string AddressLine1
        {
            get { return GetProperty<string>(nameof(AddressLine1)); }
            set { SetProperty(nameof(AddressLine1), value); }
        }

        public string AddressLine2
        {
            get { return GetProperty<string>(nameof(AddressLine2)); }
            set { SetProperty(nameof(AddressLine2), value); }
        }

        public string City
        {
            get { return GetProperty<string>(nameof(City)); }
            set { SetProperty(nameof(City), value); }
        }

        public string State
        {
            get { return GetProperty<string>(nameof(State)); }
            set { SetProperty(nameof(State), value); }
        }

        public string ZipCode
        {
            get { return GetProperty<string>(nameof(ZipCode)); }
            set { SetProperty(nameof(ZipCode), value); }
        }

        public string EmailAddress
        {
            get { return GetProperty<string>(nameof(EmailAddress)); }
            set { SetProperty(nameof(EmailAddress), value); }
        }

        public string PhoneNumber
        {
            get { return GetProperty<string>(nameof(PhoneNumber)); }
            set { SetProperty(nameof(PhoneNumber), value); }
        }

        public string ContactRelationship
        {
            get { return GetProperty<string>(nameof(ContactRelationship)); }
            set { SetProperty(nameof(ContactRelationship), value); }
        }

        public string ContactPrefix
        {
            get { return GetProperty<string>(nameof(ContactPrefix)); }
            set { SetProperty(nameof(ContactPrefix), value); }
        }

        public string ContactFirstName
        {
            get { return GetProperty<string>(nameof(ContactFirstName)); }
            set { SetProperty(nameof(ContactFirstName), value); }
        }

        public string ContactMiddleInitial
        {
            get { return GetProperty<string>(nameof(ContactMiddleInitial)); }
            set { SetProperty(nameof(ContactMiddleInitial), value); }
        }

        public string ContactLastName
        {
            get { return GetProperty<string>(nameof(ContactLastName)); }
            set { SetProperty(nameof(ContactLastName), value); }
        }

        public string ContactSuffix
        {
            get { return GetProperty<string>(nameof(ContactSuffix)); }
            set { SetProperty(nameof(ContactSuffix), value); }
        }

        public string ContactAlias
        {
            get { return GetProperty<string>(nameof(ContactAlias)); }
            set { SetProperty(nameof(ContactAlias), value); }
        }

        public string ContactAddressLine1
        {
            get { return GetProperty<string>(nameof(ContactAddressLine1)); }
            set { SetProperty(nameof(ContactAddressLine1), value); }
        }

        public string ContactAddressLine2
        {
            get { return GetProperty<string>(nameof(ContactAddressLine2)); }
            set { SetProperty(nameof(ContactAddressLine2), value); }
        }

        public string ContactCity
        {
            get { return GetProperty<string>(nameof(ContactCity)); }
            set { SetProperty(nameof(ContactCity), value); }
        }

        public string ContactState
        {
            get { return GetProperty<string>(nameof(ContactState)); }
            set { SetProperty(nameof(ContactState), value); }
        }

        public string ContactZipCode
        {
            get { return GetProperty<string>(nameof(ContactZipCode)); }
            set { SetProperty(nameof(ContactZipCode), value); }
        }

        public string ContactEmailAddress
        {
            get { return GetProperty<string>(nameof(ContactEmailAddress)); }
            set { SetProperty(nameof(ContactEmailAddress), value); }
        }

        public string ContactPhoneNumber
        {
            get { return GetProperty<string>(nameof(ContactPhoneNumber)); }
            set { SetProperty(nameof(ContactPhoneNumber), value); }
        }

        public string Occupation
        {
            get { return GetProperty<string>(nameof(Occupation)); }
            set { SetProperty(nameof(Occupation), value); }
        }

        public bool IsVolunteer
        {
            get { return GetProperty<bool>(nameof(IsVolunteer)); }
            set { SetProperty(nameof(IsVolunteer), value); }
        }

        public string OrganizationName
        {
            get { return GetProperty<string>(nameof(OrganizationName)); }
            set { SetProperty(nameof(OrganizationName), value); }
        }

        public string OrganizationContactName
        {
            get { return GetProperty<string>(nameof(OrganizationContactName)); }
            set { SetProperty(nameof(OrganizationContactName), value); }
        }

        public string OrganizationAddressLine1
        {
            get { return GetProperty<string>(nameof(OrganizationAddressLine1)); }
            set { SetProperty(nameof(OrganizationAddressLine1), value); }
        }

        public string OrganizationAddressLine2
        {
            get { return GetProperty<string>(nameof(OrganizationAddressLine2)); }
            set { SetProperty(nameof(OrganizationAddressLine2), value); }
        }

        public string OrganizationCity
        {
            get { return GetProperty<string>(nameof(OrganizationCity)); }
            set { SetProperty(nameof(OrganizationCity), value); }
        }

        public string OrganizationState
        {
            get { return GetProperty<string>(nameof(OrganizationState)); }
            set { SetProperty(nameof(OrganizationState), value); }
        }

        public string OrganizationZipCode
        {
            get { return GetProperty<string>(nameof(OrganizationZipCode)); }
            set { SetProperty(nameof(OrganizationZipCode), value); }
        }

        public string OrganizationEmailAddress
        {
            get { return GetProperty<string>(nameof(OrganizationEmailAddress)); }
            set { SetProperty(nameof(OrganizationEmailAddress), value); }
        }

        public string OrganizationPhoneNumber
        {
            get { return GetProperty<string>(nameof(OrganizationPhoneNumber)); }
            set { SetProperty(nameof(OrganizationPhoneNumber), value); }
        }

        public string UnionName
        {
            get { return GetProperty<string>(nameof(UnionName)); }
            set { SetProperty(nameof(UnionName), value); }
        }

        public string UnionLocalNumber
        {
            get { return GetProperty<string>(nameof(UnionLocalNumber)); }
            set { SetProperty(nameof(UnionLocalNumber), value); }
        }

        public Responder()
        {
            AddSynonym(ColumnNames.GLOBAL_RECORD_ID, nameof(ResponderId));
        }
    }
}
