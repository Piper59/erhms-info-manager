using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public delegate bool FieldPropertyConverter<TProperty>(XField xField, out TProperty value);
}
