using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using AonWeb.FluentHttp.Mocks.WebServer;

namespace AonWeb.FluentHttp.Mocks
{
    public static class LocalWebServerExtensions
    {
        public static LocalWebServer WithResponse(this LocalWebServer server, Predicate<ILocalRequestContext> predicate, ILocalResponse response)
        {

            server.WithResponse(new KeyValuePair<Predicate<ILocalRequestContext>, ILocalResponse>(predicate, response));

            return server;
        }

        public static LocalWebServer WithNextResponse(this LocalWebServer server, ILocalResponse response)
        {
            return server.WithResponse(r => true, response.AsTransient());
        }

        public static LocalWebServer WithAllResponses(this LocalWebServer server, ILocalResponse response)
        {
            return server.WithResponse(r => true, response);
        }
    }
}