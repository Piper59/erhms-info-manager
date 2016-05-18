using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ERHMS.EpiInfo.WebSurvey
{
    public enum Binding
    {
        [Description("Basic HTTP")]
        BasicHttp,

        [Description("WS HTTP")]
        WsHttp
    }

    public static class BindingExtensions
    {
        private static readonly IDictionary<Binding, string> EpiInfoNames = new Dictionary<Binding, string>
        {
            { Binding.BasicHttp, "BASIC" },
            { Binding.WsHttp, "WSHTTP" }
        };

        public static string ToEpiInfoName(this Binding @this)
        {
            return EpiInfoNames.Single(pair => pair.Key == @this).Value;
        }

        public static Binding FromEpiInfoName(string epiInfoName)
        {
            return EpiInfoNames.Single(pair => pair.Value == epiInfoName).Key;
        }
    }
}
