using Epi.DataSets;
using ERHMS.Common.IO;
using System.Data;

namespace ERHMS.EpiInfo
{
    public class Directories
    {
        public Epi.Configuration Owner { get; }
        public string Configuration => Get(nameof(Config.DirectoriesRow.Configuration));
        public string Projects => Get(nameof(Config.DirectoriesRow.Project));
        public string Templates => Get(nameof(Config.DirectoriesRow.Templates));

        public Directories(Epi.Configuration owner)
        {
            Owner = owner;
        }

        private string Get(string columnName)
        {
            return PathExtensions.TrimEnd(Owner.Directories.Field<string>(columnName));
        }
    }
}
