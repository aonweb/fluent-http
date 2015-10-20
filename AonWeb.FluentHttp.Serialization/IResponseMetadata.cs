using System;
using System.Collections.Generic;
using System.Net;

namespace AonWeb.FluentHttp.Serialization
{
    public interface IResponseMetadata
    {
        Uri Uri { get; }
        HttpStatusCode StatusCode { get; }
        DateTimeOffset Date { get; }
        string ETag { get; }
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