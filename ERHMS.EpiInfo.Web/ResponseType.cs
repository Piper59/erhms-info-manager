using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
        private static readonly IDictionary<ResponseType, int> EpiInfoValues = new Dictionary<ResponseType, int>
        {
            { ResponseType.Unspecified, -1 },
            { ResponseType.Single, 1 },
            { ResponseType.Multiple, 2 }
        };

        public static int ToEpiInfoValue(this ResponseType @this)
        {
            return EpiInfoValues.Single(pair => pair.Key == @this).Value;
        }

        public static ResponseType FromEpiInfoValue(int value)
        {
            return EpiInfoValues.Single(pair => pair.Value == value).Key;
        }
    }
}
