using System;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class TemporaryProject : ProjectBase
    {
        public TemporaryProject(string name, string location, string driver, string connectionString)
            : base(driver, connectionString)
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
