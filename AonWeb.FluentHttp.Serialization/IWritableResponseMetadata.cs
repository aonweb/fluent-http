namespace AonWeb.FluentHttp.Serialization
{
    public interface IResultWithMetadata
    {
       IResponseMetadata Metadata { get; }
    }

    public interface IResultWithWritableMetadata: IResultWithMetadata
    {
        new IResponseMetadata Metadata { get; set; }
    }
}