using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Serialization
{
    public class ResultWithResponseMetadata: IWritableResponseMetadata
    {
        private readonly HashSet<Uri> _dependentUris;
        private readonly HashSet<string> _varyHeaders;

        public ResultWithResponseMetadata()
        {
            _varyHeaders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            _dependentUris = new HashSet<Uri>();
        }

        #region IWritableResponseMetadata

        [JsonIgnore]
        Uri IWritableResponseMetadata.Uri { get; set; }
        [JsonIgnore]
        HttpStatusCode IWritableResponseMetadata.StatusCode {  get; set; }

        [JsonIgnore]
        DateTimeOffset IWritableResponseMetadata.Date { get; set; }

        [JsonIgnore]
        EntityTagHeaderValue IWritableResponseMetadata.ETag { get; set; }

        [JsonIgnore]
        DateTimeOffset? IWritableResponseMetadata.LastModified { get; set; }

        [JsonIgnore]
        bool IWritableResponseMetadata.NoStore { get; set; }

        [JsonIgnore]
        bool IWritableResponseMetadata.NoCache { get; set; }

        [JsonIgnore]
        bool IWritableResponseMetadata.ShouldRevalidate { get; set; }

        [JsonIgnore]
        DateTimeOffset? IWritableResponseMetadata.Expiration { get; set; }

        [JsonIgnore]
        ISet<string> IWritableResponseMetadata.VaryHeaders => _varyHeaders;

        [JsonIgnore]
        ISet<Uri> IWritableResponseMetadata.DependentUris => _dependentUris;

        [JsonIgnore]
        bool IWritableResponseMetadata.HasContent { get; set; }

        #endregion

        #region IResponseMetadata
        [JsonIgnore]
        Uri IResponseMetadata.Uri => ((IWritableResponseMetadata)this).Uri;

        [JsonIgnore]

        HttpStatusCode IResponseMetadata.StatusCode => ((IWritableResponseMetadata)this).StatusCode;
        [JsonIgnore]
        DateTimeOffset IResponseMetadata.Date => ((IWritableResponseMetadata)this).Date;

        [JsonIgnore]
        EntityTagHeaderValue IResponseMetadata.ETag => ((IWritableResponseMetadata)this).ETag;

        [JsonIgnore]
        DateTimeOffset? IResponseMetadata.LastModified => ((IWritableResponseMetadata)this).LastModified;

        [JsonIgnore]
        bool IResponseMetadata.NoStore => ((IWritableResponseMetadata)this).NoStore;

        [JsonIgnore]
        bool IResponseMetadata.NoCache => ((IWritableResponseMetadata)this).NoCache;

        [JsonIgnore]
        bool IResponseMetadata.ShouldRevalidate => ((IWritableResponseMetadata)this).ShouldRevalidate;

        [JsonIgnore]
        DateTimeOffset? IResponseMetadata.Expiration => ((IWritableResponseMetadata)this).Expiration;

        [JsonIgnore]
        IReadOnlyCollection<Uri> IResponseMetadata.DependentUris => new ReadOnlyCollection<Uri>(((IWritableResponseMetadata)this).DependentUris.ToList());

        [JsonIgnore]
        IReadOnlyCollection<string> IResponseMetadata.VaryHeaders => new ReadOnlyCollection<string>(((IWritableResponseMetadata)this).VaryHeaders.ToList());

        [JsonIgnore]
        bool IResponseMetadata.HasContent => ((IWritableResponseMetadata)this).HasContent;

        #endregion
    }
}