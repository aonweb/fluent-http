using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Serialization
{
    public interface IWritableResponseMetadata : IResponseMetadata
    {
        new Uri Uri { get; set; }
        new HttpStatusCode StatusCode { get; set; }
        new DateTimeOffset Date { get; set; }
        new EntityTagHeaderValue ETag { get; set; }
        new DateTimeOffset? LastModified { get; set; }
        new bool NoStore { get; set; }
        new bool NoCache { get; set; }
        new bool ShouldRevalidate { get; set; }
        new DateTimeOffset? Expiration { get; set; }
        new ISet<string> VaryHeaders { get; }
        new ISet<Uri> DependentUris { get; }
        new bool HasContent { get; set; }
    }
}