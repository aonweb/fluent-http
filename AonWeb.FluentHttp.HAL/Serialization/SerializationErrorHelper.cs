using System;

using Newtonsoft.Json;

namespace AonWeb.FluentHttp.HAL.Serialization
{
    internal class SerializationErrorHelper
    {
        internal static JsonSerializationException CreateError(JsonReader reader, string message, Exception ex = null)
        {
            message = FormatMessage(reader as IJsonLineInfo, reader.Path, message);

            return new JsonSerializationException(message, ex);
        }

        internal static string FormatMessage(IJsonLineInfo lineInfo, string path, string message)
        {
            if (!message.EndsWith(Environment.NewLine))
            {
                message = message.Trim();
                if (!message.EndsWith("."))
                    message = message + ".";
                message = message + " ";
            }

            message = message + String.Format("Path '{0}'", path);

            if (lineInfo != null && lineInfo.HasLineInfo())
                message = message + String.Format(", line {0}, position {1}", lineInfo.LineNumber, lineInfo.LinePosition);

            message = message + ".";
            return message;
        } 
    }
}