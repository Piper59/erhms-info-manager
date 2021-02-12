using Epi.Fields;
using ERHMS.EpiInfo.Templating.Xml;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public interface IFieldPropertySetter<TField>
        where TField : Field
    {
        void SetProperty(XField xField, TField field);
    }
}
