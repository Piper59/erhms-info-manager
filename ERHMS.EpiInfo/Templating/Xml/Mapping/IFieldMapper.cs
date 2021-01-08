using Epi.Fields;

namespace ERHMS.EpiInfo.Templating.Xml.Mapping
{
    public interface IFieldMapper
    {
        bool TrySetProperties(XField xField, Field field);
    }

    public interface IFieldMapper<TField> : IFieldMapper
        where TField : Field
    {
        void SetProperties(XField xField, TField field);
    }
}
