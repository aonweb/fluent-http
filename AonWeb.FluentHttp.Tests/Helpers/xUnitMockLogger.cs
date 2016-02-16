using AonWeb.FluentHttp.Mocks;
using Xunit.Abstractions;

namespace AonWeb.FluentHttp.Tests.Helpers
{
    public class XUnitMockLogger: IMockLogger
    {
        private readonly ITestOutputHelper _logger;

        public XUnitMockLogger(ITestOutputHelper logger)
        {
            _logger = logger;
        }
        public void WriteLine(string message)
        {
            _logger.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
            _logger.WriteLine(format, args);
        }
    }
}