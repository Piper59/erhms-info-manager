using Epi;
using ERHMS.Utility;
using System;
using System.Security.Principal;

namespace ERHMS.EpiInfo.Domain
{
    public class ViewEntity : Entity
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

        public string CreatedBy
        {
            get { return GetProperty<string>(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME); }
            set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? CreatedOn
        {
            get { return GetProperty<DateTime?>(ColumnNames.RECORD_FIRST_SAVE_TIME); }
            set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_TIME, value?.RemoveMilliseconds()); }
        }

        public string ModifiedBy
        {
            get { return GetProperty<string>(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME); }
            set { SetProperty(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? ModifiedOn
        {
            get { return GetProperty<DateTime?>(ColumnNames.RECORD_LAST_SAVE_TIME); }
            set { SetProperty(ColumnNames.RECORD_LAST_SAVE_TIME, value?.RemoveMilliseconds()); }
        }

        public bool Deleted
        {
            get { return RecordStatus.HasValue && RecordStatus.Value == 0; }
            set { RecordStatus = (short)(value ? 0 : 1); }
        }

        public ViewEntity()
        {
            AddSynonym(ColumnNames.UNIQUE_KEY, nameof(UniqueKey));
            AddSynonym(ColumnNames.GLOBAL_RECORD_ID, nameof(GlobalRecordId));
            AddSynonym(ColumnNames.FOREIGN_KEY, nameof(ForeignKey));
            AddSynonym(ColumnNames.REC_STATUS, nameof(RecordStatus));
            AddSynonym(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME, nameof(CreatedBy));
            AddSynonym(ColumnNames.RECORD_FIRST_SAVE_TIME, nameof(CreatedOn));
            AddSynonym(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME, nameof(ModifiedBy));
            AddSynonym(ColumnNames.RECORD_LAST_SAVE_TIME, nameof(ModifiedOn));
            AddSynonym(ColumnNames.REC_STATUS, nameof(Deleted));
            GlobalRecordId = Guid.NewGuid().ToString();
            Deleted = false;
        }

        public void Touch(IIdentity user = null)
        {
            if (user == null)
            {
                user = WindowsIdentity.GetCurrent();
            }
            DateTime now = DateTime.Now;
            if (New)
            {
                CreatedBy = user.Name;
                CreatedOn = now;
            }
            ModifiedBy = user.Name;
            ModifiedOn = now;
        }
    }
}
