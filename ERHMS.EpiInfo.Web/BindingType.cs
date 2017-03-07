using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ERHMS.EpiInfo.Web
{
    public enum BindingType
    {
        [Description("Basic HTTP")]
        BasicHttp,

        [Description("WS-HTTP")]
        WsHttp
    }

    public static class BindingTypeExtensions
    {
        private static readonly IDictionary<BindingType, string> EpiInfoNames = new Dictionary<BindingType, string>
        {
            { BindingType.BasicHttp, "BASIC" },
            { BindingType.WsHttp, "WSHTTP" }
        };

        public static string ToEpiInfoName(this BindingType @this)
        {
            return EpiInfoNames.Single(pair => pair.Key == @this).Value;
        }

        public static BindingType FromEpiInfoName(string value)
        {
            return EpiInfoNames.Single(pair => pair.Value == value).Key;
        }
    }
}
