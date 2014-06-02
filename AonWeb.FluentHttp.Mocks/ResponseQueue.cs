using System;
using System.Collections.Generic;

namespace AonWeb.FluentHttp.Mocks
{
    public class ResponseQueue<TRequest, TResponse>
        where TResponse : new()
    {
        private readonly Queue<Func<TRequest, TResponse>> _responses;
        private Func<TRequest, TResponse> _last;

        public ResponseQueue()
        {
            _responses = new Queue<Func<TRequest, TResponse>>();
            _last = request => new TResponse();
        }

        public ResponseQueue(IEnumerable<Func<TRequest, TResponse>> responses)
            :this()
        {
            AddRange(responses);
        }

        public Func<TRequest, TResponse> GetNext()
        {
            if (_responses.Count > 0)
                _last = _responses.Dequeue();

            return _last;
        }

        public Func<TRequest, TResponse> Replay()
        {
            return _last;
        }

        public ResponseQueue<TRequest, TResponse> RemoveNext()
        {
            if (_responses.Count > 0)
                _responses.Dequeue();

            return this;
        }

        public ResponseQueue<TRequest, TResponse> Add(Func<TRequest, TResponse> response)
        {
            _responses.Enqueue(response);

            return this;
        }

        public ResponseQueue<TRequest, TResponse> AddRange(IEnumerable<Func<TRequest, TResponse>> responses)
        {
            foreach (var response in responses)
                Add(response);

            return this;
        }
    }
}