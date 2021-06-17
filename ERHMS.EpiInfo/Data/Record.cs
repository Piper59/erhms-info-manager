using Epi;
using ERHMS.EpiInfo.Naming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Security.Principal;

namespace ERHMS.EpiInfo.Data
{
    public class Record : DynamicObject, INotifyPropertyChanged
    {
        public static StringComparer GlobalRecordIdComparer => StringComparer.OrdinalIgnoreCase;

        private readonly IDictionary<string, object> propertiesByName =
            new Dictionary<string, object>(NameComparer.Default);

        public int? UniqueKey
        {
            get { return (int?)GetProperty(ColumnNames.UNIQUE_KEY); }
            set { SetProperty(ColumnNames.UNIQUE_KEY, value); }
        }

        public RecordStatus RECSTATUS
        {
            get
            {
                return (RecordStatus)GetProperty(ColumnNames.REC_STATUS, RecordStatus.Undeleted);
            }
            set
            {
                if (SetProperty(ColumnNames.REC_STATUS, value))
                {
                    OnPropertyChanged(nameof(Deleted));
                }
            }
        }

        public bool Deleted
        {
            get { return RECSTATUS.ToDeleted(); }
            set { RECSTATUS = RecordStatusExtensions.FromDeleted(value); }
        }

        public string GlobalRecordId
        {
            get { return (string)GetProperty(ColumnNames.GLOBAL_RECORD_ID); }
            set { SetProperty(ColumnNames.GLOBAL_RECORD_ID, value); }
        }

        public string FirstSaveLogonName
        {
            get { return (string)GetProperty(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME); }
            set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? FirstSaveTime
        {
            get { return (DateTime?)GetProperty(ColumnNames.RECORD_FIRST_SAVE_TIME); }
            set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_TIME, value); }
        }

        public string LastSaveLogonName
        {
            get { return (string)GetProperty(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME); }
            set { SetProperty(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? LastSaveTime
        {
            get { return (DateTime?)GetProperty(ColumnNames.RECORD_LAST_SAVE_TIME); }
            set { SetProperty(ColumnNames.RECORD_LAST_SAVE_TIME, value); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        private void OnPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        public bool TryGetProperty(string propertyName, out object value)
        {
            return propertiesByName.TryGetValue(propertyName, out value);
        }

        public object GetProperty(string propertyName, object defaultValue = null)
        {
            return propertiesByName.TryGetValue(propertyName, out object value) ? value : defaultValue;
        }

        protected internal bool SetProperty(string propertyName, object value)
        {
            if (propertiesByName.TryGetValue(propertyName, out object currentValue) && Equals(value, currentValue))
            {
                return false;
            }
            else
            {
                propertiesByName[propertyName] = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        public void Touch(bool creating)
        {
            string userName = null;
            try
            {
                using (WindowsIdentity identity = WindowsIdentity.GetCurrent())
                {
                    userName = identity.Name;
                }
            }
            catch { }
            DateTime now = DateTime.Now;
            if (creating)
            {
                FirstSaveLogonName = userName;
                FirstSaveTime = now;
            }
            LastSaveLogonName = userName;
            LastSaveTime = now;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return propertiesByName.Keys;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return propertiesByName.TryGetValue(binder.Name, out result);
        }

        public override int GetHashCode()
        {
            return GlobalRecordId == null ? 0 : GlobalRecordIdComparer.GetHashCode(GlobalRecordId);
        }

        public override bool Equals(object obj)
        {
            return obj is Record record
                && GlobalRecordId != null
                && GlobalRecordIdComparer.Equals(GlobalRecordId, record.GlobalRecordId);
        }
    }
}
