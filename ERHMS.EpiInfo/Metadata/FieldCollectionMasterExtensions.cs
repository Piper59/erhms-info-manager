using Epi;
using Epi.Collections;

namespace ERHMS.EpiInfo.Metadata
{
    public static class FieldCollectionMasterExtensions
    {
        public static bool Contains(this FieldCollectionMaster @this, string name, MetaFieldType type)
        {
            return @this.Contains(name) && @this[name].FieldType == type;
        }
    }
}
