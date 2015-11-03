using System.Collections.Generic;

namespace AonWeb.FluentHttp
{
    public interface IBuilderFactory<TBuilder>
    {
        TBuilder Create();
        IList<IBuilderConfiguration<TBuilder>> Configurations { get; } 
    }
}