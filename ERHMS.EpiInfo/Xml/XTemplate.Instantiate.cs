using Epi;

namespace ERHMS.EpiInfo.Xml
{
    public partial class XTemplate
    {
        public void Instantiate(Project project)
        {
            Log.Debug("Instantiating template");
            Metadata = project.Metadata;
            // TODO
        }
    }
}
