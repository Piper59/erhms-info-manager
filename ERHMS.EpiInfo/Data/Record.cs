using Epi;
using ERHMS.EpiInfo.Naming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace ERHMS.EpiInfo.Data
{
    public class Record : DynamicObject, INotifyPropertyChanged
    {
        public static StringComparer GlobalRecordIdComparer => StringComparer.OrdinalIgnoreCase;

        private readonly IDictionary<string, object> propertiesByName =
            new Dictionary<string, object>(NameComparer.Default);

        public int? UniqueKey
        {
            get { return GetProperty<int?>(ColumnNames.UNIQUE_KEY); }
            set { SetProperty(ColumnNames.UNIQUE_KEY, value); }
        }

        public RecordStatus RECSTATUS
        {
            get
            {
                return GetProperty<RecordStatus>(ColumnNames.REC_STATUS);
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
            get { return RECSTATUS == RecordStatus.Deleted; }
            set { RECSTATUS = value ? RecordStatus.Deleted : RecordStatus.Undeleted; }
        }

        public string GlobalRecordId
        {
            get { return GetProperty<string>(ColumnNames.GLOBAL_RECORD_ID); }
            set { SetProperty(ColumnNames.GLOBAL_RECORD_ID, value); }
        }

        public Record()
        {
            UniqueKey = null;
            RECSTATUS = RecordStatus.Undeleted;
            GlobalRecordId = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        private void OnPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        public IEnumerable<string> GetPropertyNames()
        {
            return propertiesByName.Keys;
        }

        public TProperty GetProperty<TProperty>(string propertyName)
        {
            return (TProperty)propertiesByName[propertyName];
        }

        public bool SetProperty(string propertyName, object value)
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

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            return GetPropertyNames();
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return propertiesByName.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetProperty(binder.Name, value);
            return true;
        }

        public override int GetHashCode()
        {
            return GlobalRecordId == null ? 0 : GlobalRecordIdComparer.GetHashCode(GlobalRecordId);
        }

        public override bool Equals(object obj)
        {
            return GlobalRecordId != null
                && obj is Record record
                && GlobalRecordIdComparer.Equals(GlobalRecordId, record.GlobalRecordId);
        }
    }
}
