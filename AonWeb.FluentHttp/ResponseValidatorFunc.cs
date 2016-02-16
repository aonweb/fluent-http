using System;
using System.Net.Http;

namespace AonWeb.FluentHttp
{
    public class ResponseValidatorFunc : IResponseValidator
    {
        private readonly Func<HttpResponseMessage, bool> _validator;

        public ResponseValidatorFunc(Func<HttpResponseMessage, bool> validator)
        {
            _validator = validator;
        }
        
        public bool IsValid(HttpResponseMessage response)
        {
            return _validator?.Invoke(response) ?? true;
        }
    }
}