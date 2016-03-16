using System;
using System.Data.Common;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class TemporaryProject : ProjectBase
    {
        public TemporaryProject(string name, string location, string driver, DbConnectionStringBuilder builder)
            : base(driver, builder)
        {
            Name = name;
            Location = location;
            Save();
        }

        ~TemporaryProject()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                base.Dispose();
            }
            try
            {
                File.Delete(FilePath);
            }
            catch { }
        }

        public override void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
