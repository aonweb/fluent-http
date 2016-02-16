using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace AonWeb.FluentHttp
{
    public class ResponseValidatorCollection : List<IResponseValidator>, IResponseValidator
    {
        public ResponseValidatorCollection()
            : base(new [] { new DefaultResponseValidator() }) { }

        public ResponseValidatorCollection(IEnumerable<IResponseValidator> collection) 
            : base(collection ?? Enumerable.Empty<IResponseValidator>()) { }

        public bool IsValid(HttpResponseMessage response)
        {
            return !this.Any() || this.All(v => v.IsValid(response));
        }
    }
}