﻿using Epi;
using Epi.Fields;
using ERHMS.Common.IO;
using ERHMS.Common.Linq;
using ERHMS.Common.Logging;
using ERHMS.Data;
using ERHMS.Data.Logging;
using ERHMS.EpiInfo.Metadata;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;

namespace ERHMS.EpiInfo.Data
{
    public class RecordImporter : CsvReader
    {
        private readonly IDictionary<int, Field> fieldsByIndex = new Dictionary<int, Field>();
        private readonly IDictionary<int, TypeConverter> convertersByIndex = new Dictionary<int, TypeConverter>();

        public View View { get; }
        public IEnumerable<string> Headers { get; }
        public IProgress<int> Progress { get; set; }

        private readonly ICollection<Exception> errors = new List<Exception>();
        public IEnumerable<Exception> Errors => errors;

        public RecordImporter(View view, TextReader reader)
            : base(reader)
        {
            View = view;
            Headers = ReadRow();
            if (Headers == null)
            {
                throw GetException("No headers found", 1);
            }
        }

        public void Reset()
        {
            fieldsByIndex.Clear();
            convertersByIndex.Clear();
        }

        public void Map(int index, Field field)
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
            foreach ((int index, Field field) in fieldsByIndex)
            {
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
            }
            catch (Exception ex)
            {
                throw GetException("An error occurred while saving the record", ex);
            }
        }

        public bool Import(CancellationToken cancellationToken)
        {
            Log.Instance.Debug("Importing records");
            if (fieldsByIndex.Count == 0)
            {
                throw new InvalidOperationException("No fields to import.");
            }
            using (RecordRepository repository = new RecordRepository(View))
            using (ITransactor transactor = repository.Transact())
            {
                if (transactor.Connection is LoggingConnection connection)
                {
                    connection.Level = null;
                }
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    try
                    {
                        Record record = ReadRecord();
                        if (record == null)
                        {
                            break;
                        }
                        Progress?.Report(RowNumber + 1);
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

        public bool Import()
        {
            return Import(CancellationToken.None);
        }
    }
}
