namespace AonWeb.FluentHttp.Mocks
{
    public interface IMockLogger
    {
        void WriteLine(string message);

        void WriteLine(string format, params object[] args);
    }
}