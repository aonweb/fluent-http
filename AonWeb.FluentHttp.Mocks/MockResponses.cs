using System;
using System.Collections.Generic;
using System.Linq;

namespace AonWeb.FluentHttp.Mocks
{
    public class MockResponses<TRequestContext, TResponse>
        where TResponse : class, IMaybeTransient
    {
        private readonly List<KeyValuePair<Predicate<TRequestContext>, Func<TRequestContext, TResponse>>> _responses;
        private readonly Func<TResponse> _defaultResponseFactory;

        public MockResponses()
            :this(() => default(TResponse)) { }

        public MockResponses(Func<TResponse> defaultResponseFactory)
        {
            _defaultResponseFactory = defaultResponseFactory;
            _responses = new List<KeyValuePair<Predicate<TRequestContext>, Func<TRequestContext, TResponse>>>();
        }

        public void Clear()
        {
            _responses.Clear();
        }

        public void Add(Predicate<TRequestContext> predicate, Func<TRequestContext, TResponse> responseFactory)
        {
            _responses.Add(new KeyValuePair<Predicate<TRequestContext>, Func<TRequestContext, TResponse>>(predicate, responseFactory));
        }

        public TResponse GetResponse(TRequestContext context)
        {
            TResponse response;
            lock (_responses)
            {
                var pair = _responses.FirstOrDefault(kp => kp.Key(context));

                response = pair.Value?.Invoke(context);

                if (response?.IsTransient == true)
                    _responses.Remove(pair);
            }

            return response ?? _defaultResponseFactory?.Invoke();
        }
    }
}