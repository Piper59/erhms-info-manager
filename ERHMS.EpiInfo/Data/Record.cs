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
        private static readonly Regex TableNamePrefixRegex = new Regex(@"^.+\.");

        public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        public string GlobalRecordId => GetProperty<string>(ColumnNames.GLOBAL_RECORD_ID)?.ToLower();
        public int? UniqueKey => GetProperty<int?>(ColumnNames.UNIQUE_KEY);

        public short? Status
        {
            get { return GetProperty<short?>(ColumnNames.REC_STATUS); }
            private set { SetProperty(ColumnNames.REC_STATUS, value); }
        }

        public bool Deleted
        {
            get
            {
                return RecordStatus.ToDeleted(Status);
            }
            set
            {
                Status = RecordStatus.FromDeleted(value);
                OnPropertyChanged(nameof(Deleted));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(PropertyChangedEventArgs e) => PropertyChanged?.Invoke(this, e);
        private void OnPropertyChanged(string propertyName) => OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

        public bool TryGetProperty(string propertyName, out object value)
        {
            return Properties.TryGetValue(propertyName, out value);
        }

        public bool TryGetProperty<T>(string propertyName, out T value)
        {
            if (TryGetProperty(propertyName, out object obj))
            {
                value = (T)obj;
                return true;
            }
            else
            {
                value = default(T);
                return false;
            }
        }

        public object GetProperty(string propertyName)
        {
            TryGetProperty(propertyName, out object value);
            return value;
        }

        public T GetProperty<T>(string propertyName)
        {
            TryGetProperty(propertyName, out T value);
            return value;
        }

        public void SetProperty(string propertyName, object value)
        {
            Properties[propertyName] = value;
            OnPropertyChanged(propertyName);
        }

        public void SetProperties(IDataRecord record)
        {
            for (int index = 0; index < record.FieldCount; index++)
            {
                string propertyName = TableNamePrefixRegex.Replace(record.GetName(index), "");
                object value = record.IsDBNull(index) ? null : record.GetValue(index);
                SetProperty(propertyName, value);
            }
        }

        public sealed override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return TryGetProperty(binder.Name, out result);
        }

        public sealed override bool TrySetMember(SetMemberBinder binder, object value)
        {
            SetProperty(binder.Name, value);
            return true;
        }

        public override int GetHashCode()
        {
            return GlobalRecordId == null ? 0 : GlobalRecordId.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj is Record record && record.GlobalRecordId == GlobalRecordId;
        }
    }
}
