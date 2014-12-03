using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AonWeb.FluentHttp.Exceptions;

namespace AonWeb.FluentHttp.Handlers
{
    internal class HandlerHelper
    {
        public static async Task<ModifyTracker> InvokeHandlers<TCallHandlerType>(IDictionary<TCallHandlerType, IList<KeyValuePair<HttpCallHandlerPriority, Delegate>>> handlers, TCallHandlerType callType, Type openContextType, Type[] contextGenericTypeArgs, object[] contextCtorArgs, bool suppressTypeExceptions)
        {
            var tasks = new List<Task>();

            IHandlerContext handlerContext = null;

            foreach (var pair in handlers[callType].OrderBy(kp => kp.Key))
            {
                var priority = pair.Key;
                var handler = pair.Value;

                var contextType = CreateHandlerContextType(handler, openContextType, contextGenericTypeArgs, suppressTypeExceptions);

                if (contextType == null)
                    continue;

                var handlerType = CreateHandlerType(contextType);

                if (handlerContext == null) // initially, pass in everything
                    handlerContext = CreateHandlerContext(contextType, contextCtorArgs);
                else if (handlerContext.GetType() != contextType) // subsequent, chain with previous context
                    handlerContext = CreateHandlerContext(contextType, handlerContext);

                var task = InvokeHandler(handlerType, handler, handlerContext);

                if (priority != HttpCallHandlerPriority.Parallel)
                    await task;
                else
                    tasks.Add(task);
            }

            if (tasks.Count > 0)
                await Task.WhenAll(tasks);

            if (handlerContext != null)
                return handlerContext.GetHandlerResult();

            return new ModifyTracker();
        }

        private static Type CreateHandlerContextType(object handler, Type openContextType, Type[] contextGenericTypeArgs, bool suppressTypeExceptions)
        {
            var contextType = CreateHandlerContextType(handler, openContextType, contextGenericTypeArgs);

            if (contextType == null)
            {
                var tempCtxType = CreateHandlerContextType(openContextType, contextGenericTypeArgs);

                if (!suppressTypeExceptions)
                    throw new TypeMismatchException(tempCtxType, handler.GetType());

                return null;
            }

            return contextType;
        }

        private static Type CreateHandlerContextType(object handler, Type contextType, Type[] genericTypes)
        {
            var handlerType = handler.GetType();

            //should be of the form Func<TContext, Task>
            if (!handlerType.IsGenericType)
                return null;

            //TODO: type caching?

            var handlerContextType = handler.GetType().GetGenericArguments().FirstOrDefault();

            if (handlerContextType == null)
                return null;

            //if the context is not generic, we don't need to worry about construction
            if (handlerContextType.IsGenericType)
            {
                var handlerGenericTypes = handlerContextType.GetGenericArguments();
                var handlerTypeDef = handlerContextType.GetGenericTypeDefinition();

                if (!contextType.IsGenericTypeDefinition || !handlerTypeDef.IsAssignableFrom(contextType))
                    return null;

                contextType = handlerTypeDef;

                // if the requested generic types don't match the handler, then we can't construct
                if (handlerGenericTypes.Length != genericTypes.Length)
                    return null;

                for (var i = 0; i < handlerGenericTypes.Length; i++)
                {
                    // if we can't assign to the types in the handler, we can't construct
                    if (!handlerGenericTypes[i].IsAssignableFrom(genericTypes[i]))
                        return null;
                }

                genericTypes = handlerGenericTypes;
            }
            else
            {
                if (!handlerContextType.IsAssignableFrom(contextType))
                    return null;

                contextType = handlerContextType;
            }

            return CreateHandlerContextType(contextType, genericTypes);
        }

        private static Type CreateHandlerContextType(Type contextType, Type[] genericTypes)
        {
            var closedContextType = contextType;

            if (contextType.IsGenericTypeDefinition)
                closedContextType = contextType.MakeGenericType(genericTypes);

            return closedContextType;
        }

        private static IHandlerContext CreateHandlerContext(Type contextType, params object[] ctorArgs)
        {
            var ctors = contextType.GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(c => c.GetParameters().Count() == ctorArgs.Length).ToList();

            ConstructorInfo ctor;

            if (ctors.Count > 1)
            {
                ctor = ctors.FirstOrDefault(c =>
                {
                    var parameters = c.GetParameters().ToList();

                    var isThisTheOne = true;

                    for (var i = 0; i < parameters.Count; i++)
                    {
                        var arg = ctorArgs[i];
                        var type = parameters[i].ParameterType;

                        if ((arg == null && !TypeCanBeNull(type))
                            || (arg != null && !CanAssignOrConvertType(type, arg.GetType())))
                            isThisTheOne = false;
                    }

                    return isThisTheOne;
                });
            }
            else
            {
                ctor = ctors.FirstOrDefault();
            }

            if (ctor == null)
                throw new MissingMethodException(string.Format(SR.ConstructorMissingErrorFormat, contextType.FormattedTypeName(), ctorArgs.Length));

            return (IHandlerContext)ctor.Invoke(ctorArgs);
        }


        public static bool CanAssignOrConvertType(Type targetType, Type currentType)
        {
            //if the context is not generic, we don't need to worry about construction
            if (currentType.IsGenericType && targetType.IsGenericType)
            {
                var currentGenericTypes = currentType.GetGenericArguments();
                var currentTypeDef = currentType.GetGenericTypeDefinition();
                var targetGenericTypes = targetType.GetGenericArguments();
                var targetTypeDef = targetType.GetGenericTypeDefinition();

                return targetTypeDef.IsAssignableFrom(currentTypeDef) &&
                       targetGenericTypes.Length == currentGenericTypes.Length;
            }

            return targetType.IsAssignableFrom(currentType);
        }

        public static bool IsNullableType(Type t)
        {
            if (t.IsGenericType)
                return t.GetGenericTypeDefinition() == typeof(Nullable<>);

            return false;
        }

        public static bool TypeCanBeNull(Type t)
        {
            return !t.IsValueType || IsNullableType(t);
        }

        private static Type CreateHandlerType(Type contextType)
        {
            return typeof(Func<,>).MakeGenericType(contextType, typeof(Task));
        }

        private static Task InvokeHandler(Type handlerType, Delegate handler, IHandlerContext context)
        {
            return (Task)handlerType.InvokeMember("Invoke", BindingFlags.InvokeMethod, null, handler, new object[] { context });
        }
    }
}