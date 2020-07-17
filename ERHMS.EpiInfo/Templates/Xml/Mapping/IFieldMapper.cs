using Epi.Fields;

namespace ERHMS.EpiInfo.Templates.Xml.Mapping
{
    public interface IFieldMapper
    {
        void SetProperties(XField xField, Field field);
    }

    public interface IFieldMapper<TField> : IFieldMapper
        where TField : Field
    {
        void SetProperties(XField xField, TField field);
    }
}
