using Epi;
using ERHMS.Utility;

namespace ERHMS.EpiInfo.Xml
{
    public partial class XTemplate
    {
        public void Instantiate(Project project)
        {
            Log.Default.Debug("Instantiating template");
            Metadata = project.Metadata;
            // TODO
        }
    }
}
