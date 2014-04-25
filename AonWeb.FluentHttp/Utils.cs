using System;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace AonWeb.Fluent.Http
{
    public static class Utils
    {
        public static T As<T>(this object @this)
        {
            return (T) @this;
        }

        public static Action<T> MergeAction<T>(Action<T> action1, Action<T> action2)
        {
            if (action1 == null && action2 == null)
                return x => { };

            var result = action1 ?? action2;

            if (action1 != null && action2 != null)
            {
                result = x =>
                {
                    action1(x);
                    action2(x);
                };
            }

            return result;
        }

        public static Uri AppendToQueryString(Uri uri, string key, string value)
        {
            return AppendToQueryString(uri, new NameValueCollection { { key, value } });
        }

        public static Uri AppendToQueryString(Uri uri, NameValueCollection newValues)
        {
            if (uri == null)
                throw new ArgumentNullException("uri");

            if (newValues == null || newValues.Count == 0)
                return uri;

            var values = HttpUtility.ParseQueryString(uri.Query);

            foreach (var key in newValues.Keys.OfType<string>())
            {
                values[key] = newValues[key];
            }

            var builder = new UriBuilder(uri)
            {
                Query = values.ToString()
            };

            return builder.Uri;
        }
    }
}