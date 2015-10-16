namespace AonWeb.FluentHttp.Mocks
{
    public class MockFormatterFactory : IMockFormatterFactory
    {
        IFormatter IFormatterFactory.Create()
        {
            return Create();
        }

        public MockFormatter Create()
        {
            return new MockFormatter();
        }
    }
}