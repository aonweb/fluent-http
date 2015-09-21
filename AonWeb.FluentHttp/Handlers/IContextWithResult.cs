using System;

namespace AonWeb.FluentHttp.Handlers
{
    public interface IContextWithResult<in TResult>: IContext
    {
        TResult Result { set; }
    }
}