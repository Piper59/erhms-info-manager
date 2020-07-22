using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public interface IFieldMapping<TField>
        where TField : Field
    {
        void SetProperty(XField xField, TField field);
    }
}
