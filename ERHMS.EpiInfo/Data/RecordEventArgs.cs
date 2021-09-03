using System;

namespace ERHMS.EpiInfo.Data
{
    public class RecordEventArgs<TRecord> : EventArgs
    {
        public TRecord Record { get; }

        public RecordEventArgs(TRecord record)
        {
            Record = record;
        }
    }

    public class RecordEventArgs : RecordEventArgs<Record>
    {
        public RecordEventArgs(Record record)
            : base(record) { }
    }
}
