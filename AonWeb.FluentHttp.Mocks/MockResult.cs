namespace AonWeb.FluentHttp.Mocks
{
    public class MockResult<TResult> : IMockResult<TResult>
    {
        private MockHttpResponseMessage _response;

        public MockResult()
            :this(default(TResult)) { }

        public MockResult(TResult result)
            : this(result, new MockHttpResponseMessage()) { }

        public MockResult(TResult result, MockHttpResponseMessage response)
        {
            Result = result;
            Response = response;
        }

        public bool IsTransient
        {
            get
            {
                return (Response?.IsTransient).GetValueOrDefault();
            }
            set
            {
                if (Response == null)
                    Response = new MockHttpResponseMessage();

                if (value)
                    Response.AsTransient();
                else
                    Response.AsIntransient();
            }
        }

        public MockHttpResponseMessage Response {
            get { return _response; }
            set
            {
                if (IsTransient)
                {
                    value.AsTransient();
                }

                _response = value;
            }
        }

        public TResult Result { get; set; }

        object IMockResult.Result
        {
            get { return Result; }
            set { Result = (TResult)value; }
        }
    }
}