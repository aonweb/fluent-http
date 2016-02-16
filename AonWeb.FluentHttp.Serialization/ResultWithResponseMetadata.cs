using Newtonsoft.Json;

namespace AonWeb.FluentHttp.Serialization
{
    public class ResultWithResponseMetadata: IResultWithWritableMetadata
    {
        [JsonIgnore]
         IResponseMetadata IResultWithWritableMetadata.Metadata { get; set; } = new ResponseMetadata();

        IResponseMetadata IResultWithMetadata.Metadata => ((IResultWithWritableMetadata) this).Metadata;
    }
}