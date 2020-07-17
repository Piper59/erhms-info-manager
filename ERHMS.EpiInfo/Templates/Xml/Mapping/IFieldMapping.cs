using Epi.Fields;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public interface IFieldMapping
    {
        void SetProperty(XField xField, Field field);
    }
}
