using System;

namespace ERHMS.EpiInfo.Templating.Mapping
{
    public interface IMappingContext
    {
        void OnError(Exception exception, out bool handled);
        bool MapViewId(int value, out int result);
        bool MapFieldId(int value, out int result);
        bool MapTableName(string value, out string result);
        bool MapFieldName(string value, out string result);
    }
}
