using System;
using System.Linq;
using System.Net;
using System.Web;

namespace SK.Extensions
{
    public static class UriExtensions
    {
        public static Uri AddQueryStringParameter(this Uri uri, string key, string value)
        {
            var uriBuilder = new UriBuilder(uri);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query[key] = value;
            uriBuilder.Query = query.ToString();

            return uriBuilder.Uri;
        }

        public static Uri Append(this Uri uri, params string[] paths)
        {
            return new Uri(paths.Aggregate(
                uri.AbsoluteUri,
                (current, path) => string.Format("{0}/{1}", current.TrimEnd('/'), WebUtility.UrlEncode(path.TrimStart('/')))
            ));
        }
    }
}
