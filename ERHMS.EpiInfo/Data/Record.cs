using Epi;
using ERHMS.EpiInfo.Naming;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Runtime.CompilerServices;

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
                return (RecordStatus)GetProperty(ColumnNames.REC_STATUS);
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

        public Record()
        {
            UniqueKey = null;
            Deleted = false;
            GlobalRecordId = null;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);

        private void OnPropertyChanged(string propertyName) =>
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        public IEnumerable<string> GetPropertyNames()
        {
            return propertiesByName.Keys;
        }

        protected object GetPropertyCore([CallerMemberName] string propertyName = null)
        {
            return propertiesByName[propertyName];
        }

        public object GetProperty(string propertyName)
        {
            return GetPropertyCore(propertyName);
        }

        protected bool SetPropertyCore(object value, [CallerMemberName] string propertyName = null)
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

        public bool SetProperty(string propertyName, object value)
        {
            return SetPropertyCore(value, propertyName);
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
            return obj is Record record
                && GlobalRecordId != null
                && GlobalRecordIdComparer.Equals(GlobalRecordId, record.GlobalRecordId);
        }
    }
}
