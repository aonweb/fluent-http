using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;
using AonWeb.FluentHttp.Exceptions.Helpers;
using AonWeb.FluentHttp.Handlers;
using AonWeb.FluentHttp.Serialization;

namespace AonWeb.FluentHttp.Helpers
{
    public static class ObjectHelpers
    {
        public static Task ToTask<T>(this Action<T> action, T arg)
        {
            action?.Invoke(arg);

            return Task.FromResult(true);
        }

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

        public static Task<HttpContent> CreateHttpContent(ITypedBuilderContext context, object content)
        {
            // TODO: handle empty request?
            return context.Formatter.CreateContent(content, context);
        }

        public static async Task<object> CreateResult(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response)
        {
            object result;
            if (typeof(IEmptyResult).IsAssignableFrom(context.ResultType))
            {
                result = TypeHelpers.GetDefaultValueForType(context.ResultType);
            }
            else
            {
                result = await context.Formatter.DeserializeResult(response, context);
            }

            var resultWithMeta = result as IResultWithWritableMetadata;

            if (resultWithMeta != null)
            {
                resultWithMeta.Metadata = CachingHelpers.CreateResponseMetadata(result, request, response, context.CacheMetadata);
            }

            return result;
        }

        public static async Task<object> CreateError(ITypedBuilderContext context, HttpRequestMessage request, HttpResponseMessage response, Exception exception)
        {
            var allowNullError = typeof (IEmptyError).IsAssignableFrom(context.ErrorType);

            object error = null;
            if (!allowNullError && response != null)
            {
                try
                {
                    error = await context.Formatter.DeserializeError(response, context);
                }
                catch (Exception ex)
                {
                    exception = exception != null ? new AggregateException(ex, exception) : ex;
                }
            }

            if (error == null)
            {
                error = exception != null 
                    ?  context.DefaultErrorFactory?.Invoke(context.ErrorType, exception) 
                    : TypeHelpers.GetDefaultValueForType(context.ErrorType);
            }

            if(error == null && !allowNullError && exception != null)
                throw exception;

            var errorWithMeta = error as IResultWithWritableMetadata;

            if (errorWithMeta != null)
            {
                errorWithMeta.Metadata = CachingHelpers.CreateResponseMetadata(error, request, response, context.CacheMetadata);
            }

            return error;
        }

        public static Exception CreateTypedException(ExceptionCreationContext context)
        {
            var openExType = typeof(HttpErrorException<>);

            var exType = openExType.MakeGenericType(context.ErrorType);

            var message = context.Error?.ToString() ?? context.InnerException?.Message;

            return (Exception)Activator.CreateInstance(exType, context.Error, context.StatusCode, message, context.InnerException);
        }

        public static Exception CreateHttpException(HttpResponseMessage response, HttpRequestMessage request)
        {
            var message = response.GetExceptionMessage(request);

            return new HttpRequestException(message);
        }

        public static async Task<object> Deserialize(HttpResponseMessage response, Type type, MediaTypeFormatterCollection formatters, CancellationToken token)
        {
            var typeInfo = type.GetTypeInfo();

            if (typeof(HttpResponseMessage).IsAssignableFrom(typeInfo))
                return response;

            var content = response.Content;

            if (content == null)
                return TypeHelpers.GetDefaultValueForType(type);

            if (typeof (Stream).IsAssignableFrom(typeInfo))
            {
                var s = await content.ReadAsStreamAsync();
                var copy = new MemoryStream();

                await s.CopyToAsync(copy);

                if (copy.CanSeek)
                    copy.Position = 0;

                return copy;
            }

            if (typeof(byte[]).IsAssignableFrom(typeInfo))
            {
                token.ThrowIfCancellationRequested();

                return await content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }

            var mediaType = content.Headers.ContentType ?? new MediaTypeHeaderValue("application/octet-stream");

            var formatter = formatters.FindReader(type, mediaType);

            if (formatter == null)
            {
                if (content.Headers.ContentLength == 0)
                    return TypeHelpers.GetDefaultValueForType(type);

                if (typeof(string).IsAssignableFrom(typeInfo))
                    return await content.ReadAsStringAsync();

                throw new UnsupportedMediaTypeException(string.Format(SR.NoReadFormatterForMimeTypeErrorFormat, typeInfo.FormattedTypeName(), mediaType.MediaType, response.GetExceptionMessage(response.RequestMessage)), mediaType);
            }

            token.ThrowIfCancellationRequested();

            var stream = await content.ReadAsStreamAsync();

            return await formatter.ReadFromStreamAsync(type, stream, content, null, token).ConfigureAwait(false);
        }
    }
}