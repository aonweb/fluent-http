using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;

namespace AonWeb.FluentHttp.Serialization
{
    public class ResponseMetadata: IWritableResponseMetadata
    {
        public ResponseMetadata()
        {
            VaryHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            DependentUris = new HashSet<Uri>();
        }
        public Uri Uri { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        
        public bool HasContent { get; set; }
        public DateTimeOffset Date { get; set; }
        public string ETag { get; set; }
        public DateTimeOffset? LastModified { get; set; }
        public bool NoStore { get; set; }
        public bool NoCache { get; set; }
        public bool ShouldRevalidate { get; set; }
        public DateTimeOffset? Expiration { get; set; }
        
        public ISet<string> VaryHeaders { get; }
        public ISet<Uri> DependentUris { get; }

        IReadOnlyCollection<Uri> IResponseMetadata.DependentUris => new ReadOnlyCollection<Uri>(DependentUris.ToList());

        IReadOnlyCollection<string> IResponseMetadata.VaryHeaders => new ReadOnlyCollection<string>(VaryHeaders.ToList());
    }
}