using Epi;
using System;
using System.Data;

namespace ERHMS.EpiInfo.Data.Entities
{
    public class ViewEntity : EpiInfoEntity
    {
        public string FirstSaveLogonName
        {
            get { return GetProperty<string>(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME); }
            internal set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? FirstSaveTime
        {
            get { return GetProperty<DateTime?>(ColumnNames.RECORD_FIRST_SAVE_TIME); }
            internal set { SetProperty(ColumnNames.RECORD_FIRST_SAVE_TIME, value); }
        }

        public string LastSaveLogonName
        {
            get { return GetProperty<string>(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME); }
            internal set { SetProperty(ColumnNames.RECORD_LAST_SAVE_LOGON_NAME, value); }
        }

        public DateTime? LastSaveTime
        {
            get { return GetProperty<DateTime?>(ColumnNames.RECORD_LAST_SAVE_TIME); }
            internal set { SetProperty(ColumnNames.RECORD_LAST_SAVE_TIME, value); }
        }

        public ViewEntity()
        {
            GlobalRecordId = Guid.NewGuid().ToString();
        }

        public ViewEntity(DataRow row) : base(row) { }
    }
}
