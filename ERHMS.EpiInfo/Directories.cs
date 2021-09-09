using Epi.DataSets;
using ERHMS.Common.IO;
using ERHMS.Common.Logging;
using System;
using System.Data;
using System.IO;

namespace ERHMS.EpiInfo
{
    public class Directories
    {
        public Epi.Configuration Parent { get; }
        public string Configuration => Get(nameof(Config.DirectoriesRow.Configuration));
        public string Projects => Get(nameof(Config.DirectoriesRow.Project));
        public string Templates => Get(nameof(Config.DirectoriesRow.Templates));

        public Directories(Epi.Configuration parent)
        {
            Parent = parent;
        }

        private string Get(string columnName)
        {
            string path = PathExtensions.TrimEnd(Parent.Directories.Field<string>(columnName));
            try
            {
                Directory.CreateDirectory(path);
            }
            catch (Exception ex)
            {
                Log.Instance.Warn(ex);
            }
            return path;
        }
    }
}
