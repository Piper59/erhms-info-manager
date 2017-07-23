using ERHMS.Utility;
using System.ComponentModel;

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
        public static readonly TwoWayDictionary<BindingType, string> EpiInfoNames = new TwoWayDictionary<BindingType, string>
        {
            { BindingType.BasicHttp, "BASIC" },
            { BindingType.WsHttp, "WSHTTP" }
        };
    }
}
