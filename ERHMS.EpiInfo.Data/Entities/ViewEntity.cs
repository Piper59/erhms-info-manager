using Epi;
using System;
using System.Security.Principal;

namespace ERHMS.EpiInfo.Data
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

        public string FirstSaveLogonName
        {
            get { return GetProperty<string>(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME); }
            set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? FirstSaveTime
        {
            get { return GetProperty<DateTime?>(ColumnNames.RECORD_FIRST_SAVE_TIME); }
            set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_TIME, value); }
        }

        public string LastSaveLogonName
        {
            get { return GetProperty<string>(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME); }
            set { SetProperty(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? LastSaveTime
        {
            get { return GetProperty<DateTime?>(ColumnNames.RECORD_LAST_SAVE_TIME); }
            set { SetProperty(ColumnNames.RECORD_LAST_SAVE_TIME, value); }
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
                    return false;
                }
            }
        }

        public void SetDeleted(bool deleted)
        {
            RecordStatus = deleted ? (short)0 : (short)1;
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
                FirstSaveLogonName = user.Name;
                FirstSaveTime = now;
            }
            if (last)
            {
                LastSaveLogonName = user.Name;
                LastSaveTime = now;
            }
        }
    }
}
