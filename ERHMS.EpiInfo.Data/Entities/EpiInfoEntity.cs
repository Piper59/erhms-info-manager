using Epi;
using System.Data;

namespace ERHMS.EpiInfo.Data.Entities
{
    public abstract class EpiInfoEntity : EntityBase
    {
        public int? UniqueKey
        {
            get { return GetProperty<int?>(ColumnNames.UNIQUE_KEY); }
            internal set { SetProperty(ColumnNames.UNIQUE_KEY, value); }
        }

        public string GlobalRecordId
        {
            get { return GetProperty<string>(ColumnNames.GLOBAL_RECORD_ID); }
            internal set { SetProperty(ColumnNames.GLOBAL_RECORD_ID, value); }
        }

        public string ForeignKey
        {
            get { return GetProperty<string>(ColumnNames.FOREIGN_KEY); }
            internal set { SetProperty(ColumnNames.FOREIGN_KEY, value); }
        }

        public short? RecordStatus
        {
            get { return GetProperty<short?>(ColumnNames.REC_STATUS); }
            internal set { SetProperty(ColumnNames.REC_STATUS, value); }
        }

        public bool IsNew
        {
            get
            {
                int? uniqueKey;
                if (TryGetProperty(ColumnNames.UNIQUE_KEY, out uniqueKey))
                {
                    return uniqueKey == null;
                }
                else
                {
                    return true;
                }
            }
        }

        public bool IsDeleted
        {
            get
            {
                short? recordStatus;
                if (TryGetProperty(ColumnNames.REC_STATUS, out recordStatus))
                {
                    return recordStatus.HasValue && recordStatus.Value == 0;
                }
                else
                {
                    return true;
                }
            }
        }

        protected EpiInfoEntity()
        {
            SetDeleted(false);
        }

        protected EpiInfoEntity(DataRow row) : base(row) { }

        internal void SetDeleted(bool deleted)
        {
            RecordStatus = deleted ? (short)0 : (short)1;
        }
    }
}
