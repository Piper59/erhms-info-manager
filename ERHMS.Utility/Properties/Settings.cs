using System;

namespace ERHMS.Utility
{
    public partial class Settings
    {
        public string ExpandedRootDirectory
        {
            get { return Environment.ExpandEnvironmentVariables(RootDirectory); }
        }
    }
}
