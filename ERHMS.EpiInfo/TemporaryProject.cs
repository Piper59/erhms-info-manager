using System;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class TemporaryProject : ProjectBase
    {
        public FileInfo File { get; private set; }

        public TemporaryProject(FileInfo file, string driver, string connectionString)
            : base(driver, connectionString)
        {
            File = file;
            Name = Path.GetFileNameWithoutExtension(file.Name);
            Location = file.DirectoryName;
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
                File.Delete();
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
