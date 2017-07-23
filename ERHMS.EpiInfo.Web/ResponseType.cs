using ERHMS.Utility;
using System.ComponentModel;

namespace ERHMS.EpiInfo.Web
{
    public enum ResponseType
    {
        [Description("Unspecified")]
        Unspecified,

        [Description("Single response per person")]
        Single,

        [Description("Multiple responses per person")]
        Multiple
    }

    public static class ResponseTypeExtensions
    {
        public static readonly TwoWayDictionary<ResponseType, int> EpiInfoValues = new TwoWayDictionary<ResponseType, int>
        {
            { ResponseType.Unspecified, -1 },
            { ResponseType.Single, 1 },
            { ResponseType.Multiple, 2 }
        };
    }
}
