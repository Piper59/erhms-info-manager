using ERHMS.Data;

namespace ERHMS.EpiInfo
{
    public class ProjectCreationInfo : ProjectInfo
    {
        public string Description { get; set; }
        public IDatabase Database { get; set; }
    }
}
