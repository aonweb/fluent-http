using System;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Caching
{
    public class CacheContext<TResult>
    {
        public CacheContext(CacheSettings<TResult> settings, Uri uri, HttpHeaders headers)
        {
            Key = Helper.BuildKey(typeof(TResult), uri, headers, settings.GetVaryByHeaders(uri));
            Uri = uri;

            ValidationResult = ResponseValidationResult.NotExist;
        }

        public string Key { get; private set; }
        public Uri Uri { get; private set; }
        public ResponseValidationResult ValidationResult { get; set; }
        public CacheResult<TResult> Result { get; set; }
    } 
}