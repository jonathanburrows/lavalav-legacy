using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace lvl.Web.Configuration
{
    /// <summary>
    /// Parses a variable into a configuration file.
    /// </summary>
    /// <remarks>This was done to be consistent with Microsoft's implementation.</remarks>
    internal class AnonymousConfigurationParser
    {
        private IDictionary<string, string> Data { get; }
        private Stack<string> Context { get; }
        private string currentPath { get; set; }

        public AnonymousConfigurationParser()
        {
            Context = new Stack<string>();
            Data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        }

        public IDictionary<string, string> Parse(object anonymous)
        {
            var jsonConfig = JObject.FromObject(anonymous);
            VisitJObject(jsonConfig);

            return Data;
        }

        private void VisitJObject(JObject jObject)
        {
            foreach (var property in jObject.Properties())
            {
                EnterContext(property.Name);
                VisitProperty(property);
                ExitContext();
            }
        }

        private void VisitProperty(JProperty property)
        {
            VisitToken(property.Value);
        }

        private void VisitToken(JToken token)
        {
            switch (token.Type)
            {
                case JTokenType.Object:
                    VisitJObject(token.Value<JObject>());
                    break;
                case JTokenType.Array:
                    VisitArray(token.Value<JArray>());
                    break;
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.String:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                case JTokenType.Raw:
                case JTokenType.Null:
                    VisitPrimitive(token);
                    break;
                default:
                    throw new FormatException($"Unsupported JSON token: {token.Type}");
            }
        }

        private void VisitArray(JArray array)
        {
            for (int index = 0; index < array.Count; index++)
            {
                EnterContext(index.ToString());
                VisitToken(array[index]);
                ExitContext();
            }
        }

        private void VisitPrimitive(JToken data)
        {
            var key = currentPath;

            if (Data.ContainsKey(key))
            {
                throw new FormatException($"Duplicate key {key} in configuration.");
            }
            Data[key] = data.ToString();
        }

        private void EnterContext(string context)
        {
            Context.Push(context);
            currentPath = ConfigurationPath.Combine(Context.Reverse());
        }

        private void ExitContext()
        {
            Context.Pop();
            currentPath = ConfigurationPath.Combine(Context.Reverse());
        }
    }
}
