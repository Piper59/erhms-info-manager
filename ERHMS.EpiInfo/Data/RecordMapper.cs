using Epi;
using ERHMS.EpiInfo.Naming;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace ERHMS.EpiInfo.Data
{
    public class RecordMapper
    {
        private static readonly Regex FieldNameQualifierRegex = new Regex(@"^.+\.");

        private static string GetPropertyName(string fieldName)
        {
            return FieldNameQualifierRegex.Replace(fieldName, "");
        }

        private static int GetOrdinal(IDataRecord source, string propertyName)
        {
            for (int index = 0; index < source.FieldCount; index++)
            {
                string fieldName = source.GetName(index);
                string currentPropertyName = GetPropertyName(fieldName);
                if (NameComparer.Default.Equals(currentPropertyName, propertyName))
                {
                    return index;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(propertyName));
        }

        private readonly int globalRecordIdOrdinal;

        public IDataRecord Source { get; }
        public string GlobalRecordId => Source.GetString(globalRecordIdOrdinal);

        public RecordMapper(IDataRecord source)
        {
            Source = source;
            globalRecordIdOrdinal = GetOrdinal(source, ColumnNames.GLOBAL_RECORD_ID);
        }

        public void Update(Record target)
        {
            for (int index = 0; index < Source.FieldCount; index++)
            {
                string fieldName = Source.GetName(index);
                string propertyName = GetPropertyName(fieldName);
                object value = Source.IsDBNull(index) ? null : Source.GetValue(index);
                target.SetProperty(propertyName, value);
            }
        }

        public Record Create()
        {
            Record target = new Record();
            Update(target);
            return target;
        }
    }
}
