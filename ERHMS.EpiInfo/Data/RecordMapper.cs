using Epi;
using ERHMS.EpiInfo.Naming;
using System;
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

        private static int GetOrdinal(IDataRecord source, string propertyName)
        {
            for (int index = 0; index < source.FieldCount; index++)
            {
                string sourceFieldName = source.GetName(index);
                string sourcePropertyName = GetPropertyName(sourceFieldName);
                if (NameComparer.Default.Equals(sourcePropertyName, propertyName))
                {
                    return index;
                }
            }
            throw new ArgumentOutOfRangeException(nameof(propertyName));
        }


        public IDataRecord Source { get; }
        public int GlobalRecordIdOrdinal { get; }
        public string GlobalRecordId => Source.GetString(GlobalRecordIdOrdinal);

        public RecordMapper(IDataRecord source)
        {
            Source = source;
            GlobalRecordIdOrdinal = GetOrdinal(source, ColumnNames.GLOBAL_RECORD_ID);
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
