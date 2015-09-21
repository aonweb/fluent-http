using System.Collections.Generic;

namespace AonWeb.FluentHttp.Mocks
{
    public class ResponseQueue<T>
    {
        private readonly Queue<T> _responses;
        private T _last;

        public ResponseQueue(T defaultFactory)
        {
            _responses = new Queue<T>();
            _last = defaultFactory;
        }

        public ResponseQueue(T defaultFactory, IEnumerable<T> responses)
            : this(defaultFactory)
        {
            AddRange(responses);
        }

        public T GetNext()
        {
            if (_responses.Count > 0)
                _last = _responses.Dequeue();

            return _last;
        }

        public T Replay()
        {
            return _last;
        }

        public ResponseQueue<T> RemoveNext()
        {
            if (_responses.Count > 0)
                _responses.Dequeue();

            return this;
        }

        public ResponseQueue<T> Add(T response)
        {
            _responses.Enqueue(response);

            return this;
        }

        public ResponseQueue<T> AddRange(IEnumerable<T> responses)
        {
            foreach (var response in responses)
                Add(response);

            return this;
        }
    }
}