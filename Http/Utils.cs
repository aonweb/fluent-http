using System;

namespace AonWeb.Fluent.Http
{
    public static class Utils
    {
        public static Action<T> MergeAction<T>(Action<T> action1, Action<T> action2)
        {
            if (action1 == null && action2 == null)
                return x => { };

            var result = action1 ?? action2;

            if (action1 != null && action2 != null)
            {
                    result = x =>
                    {
                        action1(x);
                        action2(x);
                    };
            }

            return result;
        }
    }
}