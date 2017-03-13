using Epi;
using System;
using System.Security.Principal;

namespace ERHMS.EpiInfo.Domain
{
    public class ViewEntity : EntityBase
    {
        public int? UniqueKey
        {
            get { return GetProperty<int?>(ColumnNames.UNIQUE_KEY); }
            set { SetProperty(ColumnNames.UNIQUE_KEY, value); }
        }

        public string GlobalRecordId
        {
            get { return GetProperty<string>(ColumnNames.GLOBAL_RECORD_ID); }
            set { SetProperty(ColumnNames.GLOBAL_RECORD_ID, value); }
        }

        public string ForeignKey
        {
            get { return GetProperty<string>(ColumnNames.FOREIGN_KEY); }
            set { SetProperty(ColumnNames.FOREIGN_KEY, value); }
        }

        public short? RecordStatus
        {
            get { return GetProperty<short?>(ColumnNames.REC_STATUS); }
            set { SetProperty(ColumnNames.REC_STATUS, value); }
        }

        public string FirstSaveUserName
        {
            get { return GetProperty<string>(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME); }
            set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? FirstSaveStamp
        {
            get { return GetProperty<DateTime?>(ColumnNames.RECORD_FIRST_SAVE_TIME); }
            set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_TIME, value); }
        }

        public string LastSaveUserName
        {
            get { return GetProperty<string>(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME); }
            set { SetProperty(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? LastSaveStamp
        {
            get { return GetProperty<DateTime?>(ColumnNames.RECORD_LAST_SAVE_TIME); }
            set { SetProperty(ColumnNames.RECORD_LAST_SAVE_TIME, value); }
        }

        public bool Deleted
        {
            get { return RecordStatus.HasValue && EpiInfo.RecordStatus.IsDeleted(RecordStatus.Value); }
            set { RecordStatus = value ? EpiInfo.RecordStatus.Deleted : EpiInfo.RecordStatus.Undeleted; }
        }

        public ViewEntity()
        {
            AddSynonym(ColumnNames.UNIQUE_KEY, nameof(UniqueKey));
            AddSynonym(ColumnNames.GLOBAL_RECORD_ID, nameof(GlobalRecordId));
            AddSynonym(ColumnNames.FOREIGN_KEY, nameof(ForeignKey));
            AddSynonym(ColumnNames.REC_STATUS, nameof(RecordStatus));
            AddSynonym(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME, nameof(FirstSaveUserName));
            AddSynonym(ColumnNames.RECORD_FIRST_SAVE_TIME, nameof(FirstSaveStamp));
            AddSynonym(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME, nameof(LastSaveUserName));
            AddSynonym(ColumnNames.RECORD_LAST_SAVE_TIME, nameof(LastSaveStamp));
            AddSynonym(ColumnNames.REC_STATUS, nameof(Deleted));
        }

        public void SetAuditProperties(bool first, bool last, IIdentity user = null)
        {
            if (user == null)
            {
                user = WindowsIdentity.GetCurrent();
            }
            DateTime now = DateTime.Now;
            if (first)
            {
                FirstSaveUserName = user.Name;
                FirstSaveStamp = now;
            }
            if (last)
            {
                LastSaveUserName = user.Name;
                LastSaveStamp = now;
            }
        }
    }
}
