using Epi;
using ERHMS.EpiInfo.Naming;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Data
{
    public class RecordMapper<TRecord>
        where TRecord : Record, new()
    {
        private static readonly Regex fieldNameQualifierRegex = new Regex(@"^.+\.");

        private static string GetPropertyName(string fieldName)
        {
            return fieldNameQualifierRegex.Replace(fieldName, "");
        }

        private readonly IReadOnlyDictionary<string, int> ordinalsByPropertyName;

        public IDataRecord Source { get; }
        public object this[int ordinal] => Source.GetValue(ordinal);
        public object this[string propertyName] => this[ordinalsByPropertyName[propertyName]];
        public string GlobalRecordId => (string)this[ColumnNames.GLOBAL_RECORD_ID];

        public RecordMapper(IDataRecord source)
        {
            Source = source;
            Dictionary<string, int> ordinalsByPropertyName = new Dictionary<string, int>(NameComparer.Default);
            for (int index = 0; index < source.FieldCount; index++)
            {
                string fieldName = source.GetName(index);
                string propertyName = GetPropertyName(fieldName);
                if (!ordinalsByPropertyName.ContainsKey(propertyName))
                {
                    ordinalsByPropertyName[propertyName] = index;
                }
            }
            this.ordinalsByPropertyName = ordinalsByPropertyName;
        }

        public void Update(TRecord target)
        {
            for (int index = 0; index < Source.FieldCount; index++)
            {
                string fieldName = Source.GetName(index);
                string propertyName = GetPropertyName(fieldName);
                object value = Source.IsDBNull(index) ? null : Source.GetValue(index);
                target.SetProperty(propertyName, value);
            }
        }

        public TRecord Create()
        {
            TRecord target = new TRecord();
            Update(target);
            return target;
        }
    }
}
