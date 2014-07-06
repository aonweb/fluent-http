using System;

namespace AonWeb.FluentHttp.Mocks
{
    public class AssertAction<T> : IAssertAction {
        private bool _called;

        private readonly Action<T> _innerAction;
        private readonly Func<Action> _failurefactory;

        public AssertAction(Action<T> action, Func<Action> failurefactory)
        {
            _innerAction = action;
            _failurefactory = failurefactory;
        }

        public static implicit operator Action<T>(AssertAction<T> a)
        {
            if (!a._called)
                return obj =>
                    {
                        a._called = true;
                        a._innerAction(obj);
                    };

            return obj => { };
        }

        public void DoAssert()
        {
            if (!_called)
            {
                var failureAction = _failurefactory();

                failureAction();
            }
                
        }
    }
}