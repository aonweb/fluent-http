using System;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace AonWeb.FluentHttp
{
    public static class Helper
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

        public static string CombineVirtualPaths(string basePath, string relativePath)
        {
            return string.Concat(basePath.TrimEnd('/'), "/", relativePath.TrimStart('/'));
        }

        /// <summary>
        /// Converts the specified NameValueCollection to a QueryString formatted string i.e. "key1=val1&amp;key2=val2" suitable for use in a Url query string.
        /// </summary>
        /// <param name="list">The list.</param>
        /// <returns>A QueryString formatted string i.e. "key1=val1&amp;key2=val2"</returns>
        public static string ToEncoded(this NameValueCollection list)
        {

            var sb = new StringBuilder();
            foreach (var key in list.AllKeys)
            {
                foreach (var value in list.GetValues(key))
                {
                    if (sb.Length != 0)
                    sb.Append("&");

                sb.Append(HttpUtility.UrlEncode(value));
                }
                
            }

            return sb.ToString();
        }
    }
}