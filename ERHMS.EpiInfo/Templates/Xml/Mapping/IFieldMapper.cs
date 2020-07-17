using Epi.Fields;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public interface IFieldMapper
    {
        void SetProperties(XField xField, Field field);
    }
}
