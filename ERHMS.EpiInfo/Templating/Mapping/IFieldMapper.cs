using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public interface IFieldMapper
    {
        IMappingContext MappingContext { get; set; }

        bool IsCompatible(Field field);
        bool IsCompatible(XField xField);
        bool SetProperties(XField xField, Field field);
        bool MapProperties(Field field);
        bool MapProperties(XField xField);
        bool Canonize(XField xField);
    }

    public interface IFieldMapper<TField> : IFieldMapper
        where TField : Field
    {
        bool SetProperties(XField xField, TField field);
        bool MapProperties(TField field);
    }
}
