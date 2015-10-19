using System;
using System.Collections.Generic;
using System.Linq;

namespace AonWeb.FluentHttp
{
    public partial class Defaults
    {
        public class HandlerFactoryDefaults<THandler, TSettings>
			where THandler : class
        {
            private readonly Dictionary<object, Func<TSettings, THandler>> _additionalHandlers = new Dictionary<object, Func<TSettings, THandler>>();

            public Func<TSettings, IEnumerable<THandler>> Factory { get; set; }

            public HandlerFactoryDefaults<THandler, TSettings> AdditionalHandlers(params THandler[] handlers)
            {
                foreach(var handler in handlers){
                    AdditionalHandler(handler, s => handler);
				}

                return this;
            }

            public HandlerFactoryDefaults<THandler, TSettings> AdditionalHandlers(IEnumerable<THandler> handlers)
            {
                AdditionalHandlers((handlers ?? Enumerable.Empty<THandler>()).ToArray());

                return this;
            }

            public HandlerFactoryDefaults<THandler, TSettings> AdditionalHandler(object key, Func<TSettings, THandler> factory)
            {
                if (_additionalHandlers.ContainsKey(key))
                    _additionalHandlers.Add(key, factory);

                return this;
            }

            public HandlerFactoryDefaults<THandler, TSettings> AdditionalHandlers(IEnumerable<KeyValuePair<object, Func<TSettings, THandler>>> handlers)
            {
                foreach (var pair in handlers)
                {
                    AdditionalHandler(pair.Key, pair.Value);
                }

                return this;
            }

            public IEnumerable<THandler> GetHandlers(TSettings settings)
            {
                return (Factory?.Invoke(settings) ?? Enumerable.Empty<THandler>()).Concat(_additionalHandlers.Values.Select(f => f?.Invoke(settings)).Where(h => h != null));
            }
        }
    }
}