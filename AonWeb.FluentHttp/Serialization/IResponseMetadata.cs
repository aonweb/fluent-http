using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http.Headers;

namespace AonWeb.FluentHttp.Serialization
{
    public interface IResponseMetadata
    {
        Uri Uri { get; }
        HttpStatusCode StatusCode { get; }
        DateTimeOffset Date { get; }
        EntityTagHeaderValue ETag { get; }
        DateTimeOffset? LastModified { get; }
        bool NoStore { get; }
        bool NoCache { get; }
        bool ShouldRevalidate { get; }
        DateTimeOffset? Expiration { get; }
        IReadOnlyCollection<string> VaryHeaders { get; }
        IReadOnlyCollection<Uri> DependentUris { get; }
        bool HasContent { get; }
    }
}