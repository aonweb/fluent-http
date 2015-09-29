namespace AonWeb.FluentHttp.Mocks
{
    public class NullMockLogger : IMockLogger
    {
        public void WriteLine(string message) { }
        public void WriteLine(string format, params object[] args) { }
    }
}