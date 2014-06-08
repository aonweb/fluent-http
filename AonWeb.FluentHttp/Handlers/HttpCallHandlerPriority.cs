namespace AonWeb.FluentHttp.Handlers
{
    public enum HttpCallHandlerPriority
    {
        Parallel = 0,
        First = 1,
        High = 2,
        Default = 3,
        Low = 4,
        Last = 5,
        None = int.MaxValue
    }
}