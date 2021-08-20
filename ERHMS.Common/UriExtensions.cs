using System;

namespace ERHMS.Common
{
    public static class UriExtensions
    {
        public static bool IsWebUri(this Uri @this)
        {
            return @this.IsAbsoluteUri && (@this.Scheme == Uri.UriSchemeHttp || @this.Scheme == Uri.UriSchemeHttps);
        }
    }
}
