using System;
using System.Collections.Generic;
using System.Net;

namespace AonWeb.FluentHttp.Serialization
{
    public interface IResponseMetadata
    {
        Uri Uri { get; set; }
        HttpStatusCode StatusCode { get; set; }
        DateTimeOffset Date { get; set; }
        string ETag { get; set; }
        DateTimeOffset? LastModified { get; set; }
        bool NoStore { get; set; }
        bool NoCache { get; set; }
        bool ShouldRevalidate { get; set; }
        DateTimeOffset? Expiration { get; set; }
        ISet<string> VaryHeaders { get; }
        ISet<Uri> DependentUris { get; }
        bool HasContent { get; set; }
    }
}