using Epi;
using Epi.Fields;
using ERHMS.Common.IO;
using ERHMS.Data;
using ERHMS.EpiInfo.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

namespace ERHMS.EpiInfo.Data
{
    public class RecordImporter : CsvReader
    {
        private readonly IDictionary<int, Field> fieldsByIndex = new Dictionary<int, Field>();
        private readonly IDictionary<int, TypeConverter> convertersByIndex = new Dictionary<int, TypeConverter>();

        public View View { get; }
        public IEnumerable<string> Headers { get; }
        public int RecordCount { get; private set; }

        private readonly ICollection<Exception> errors = new List<Exception>();
        public IEnumerable<Exception> Errors => errors;

        public RecordImporter(View view, TextReader reader)
            : base(reader)
        {
            View = view;
            Headers = ReadRow();
        }

        public void MapField(int index, Field field)
        {
            Type type = field.FieldType.ToClrType();
            TypeConverter converter = TypeDescriptor.GetConverter(type);
            if (!converter.CanConvertFrom(typeof(string)))
            {
                throw new ArgumentException($"Cannot import to field '{field.Name}'.", nameof(field));
            }
            fieldsByIndex[index] = field;
            convertersByIndex[index] = converter;
        }

        private Record ReadRecord()
        {
            IList<string> texts = ReadRow();
            if (texts == null)
            {
                return null;
            }
            Record record = new Record();
            foreach (KeyValuePair<int, Field> fieldByIndex in fieldsByIndex)
            {
                int index = fieldByIndex.Key;
                Field field = fieldByIndex.Value;
                TypeConverter converter = convertersByIndex[index];
                string text = texts[index];
                try
                {
                    object value = converter.ConvertFromString(texts[index]);
                    if (value is string str && str == "" && !field.IsRequired)
                    {
                        value = null;
                    }
                    record.SetProperty(field.Name, value);
                }
                catch (Exception ex)
                {
                    throw GetException($"An error occurred while importing to field '{field.Name}'", ex);
                }
            }
            return record;
        }

        private void SaveRecord(RecordRepository repository, Record record)
        {
            try
            {
                repository.Save(record);
                RecordCount++;
            }
            catch (Exception ex)
            {
                throw GetException("An error occurred while saving the record", ex);
            }
        }

        public bool Import()
        {
            if (fieldsByIndex.Count == 0)
            {
                throw new InvalidOperationException("No fields to import.");
            }
            using (RecordRepository repository = new RecordRepository(View))
            using (ITransactor transactor = repository.Transact())
            {
                while (true)
                {
                    try
                    {
                        Record record = ReadRecord();
                        if (record == null)
                        {
                            break;
                        }
                        SaveRecord(repository, record);
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex);
                    }
                }
                if (errors.Count == 0)
                {
                    transactor.Commit();
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
