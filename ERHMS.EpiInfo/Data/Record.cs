using Epi;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Dynamic;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Data
{
    public class Record : DynamicObject, INotifyPropertyChanged
    {
        private static readonly Regex tableNamePrefixRegex = new Regex(@"^.+\.");

        private readonly Dictionary<string, object> properties = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public IReadOnlyDictionary<string, object> Properties => properties;

        public int UniqueKey => (int)properties[ColumnNames.UNIQUE_KEY];

        public short RECSTATUS
        {
            get
            {
                return (short)properties[ColumnNames.REC_STATUS];
            }
            private set
            {
                if (SetProperty(ColumnNames.REC_STATUS, value))
                {
                    OnPropertyChanged(nameof(Deleted));
                }
            }
        }

        public bool Deleted
        {
            get { return RECSTATUS == RecordStatuses.Deleted; }
            internal set { RECSTATUS = value ? RecordStatuses.Deleted : RecordStatuses.Undeleted; }
        }

        public string GlobalRecordId => (string)properties[ColumnNames.GLOBAL_RECORD_ID];

        internal Record() { }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        private void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        private bool SetProperty(string propertyName, object value)
        {
            if (properties.TryGetValue(propertyName, out object currentValue) && Equals(value, currentValue))
            {
                return false;
            }
            else
            {
                properties[propertyName] = value;
                OnPropertyChanged(propertyName);
                return true;
            }
        }

        internal void SetProperties(IDataRecord record)
        {
            for (int index = 0; index < record.FieldCount; index++)
            {
                string fieldName = record.GetName(index);
                string propertyName = tableNamePrefixRegex.Replace(fieldName, "");
                object value = record.IsDBNull(index) ? null : record.GetValue(index);
                SetProperty(propertyName, value);
            }
        }

        public sealed override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return properties.TryGetValue(binder.Name, out result);
        }

        public sealed override bool TrySetMember(SetMemberBinder binder, object value)
        {
            return false;
        }

        public override int GetHashCode()
        {
            return GlobalRecordId == null ? 0 : GlobalRecordId.ToLower().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Record record
                && record.GlobalRecordId != null
                && record.GlobalRecordId.Equals(GlobalRecordId, StringComparison.OrdinalIgnoreCase);
        }
    }
}
