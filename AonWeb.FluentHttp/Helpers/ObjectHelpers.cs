using System;
using System.Net.Http;
using System.Threading;
using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Helpers
{
    internal static class ObjectHelpers
    {
        public static void Dispose(IDisposable disposable)
        {
            try
            {
                disposable?.Dispose();

            }
            catch (ObjectDisposedException)
            {
            }
            
        }

        public static CancellationTokenSource GetCancellationTokenSource(CancellationToken token)
        {
            return token == CancellationToken.None ? new CancellationTokenSource() : CancellationTokenSource.CreateLinkedTokenSource(token);
        }

        public static T CheckType<T>(object value, bool suppressTypeMismatchException = false)
        {
            if (value == null)
                return default(T);

            var requestedType = typeof(T);
            var actualType = value.GetType();
            var canCast = requestedType.IsAssignableFrom(actualType);

            if (canCast)
                return (T) value;

            if (!suppressTypeMismatchException)
                throw new TypeMismatchException(requestedType, actualType);

            return default(T);
        }
    }
}