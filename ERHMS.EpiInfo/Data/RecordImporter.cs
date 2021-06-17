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

        private readonly ICollection<string> errors = new List<string>();
        public IEnumerable<string> Errors => errors;

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
                    // TODO: Convert empty string to null if field is not required
                    object value = converter.ConvertFromString(texts[index]);
                    record.SetProperty(field.Name, value);
                }
                catch
                {
                    throw GetException($"An error occurred while importing to field '{field.Name}'");
                }
            }
            return record;
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
                        try
                        {
                            repository.Save(record);
                        }
                        catch
                        {
                            throw GetException("An error occurred while saving the record");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add(ex.Message);
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
