using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace SK.Extensions
{
    public static class ObjectExtensions
    {
        public static object GetPropertyValue(this object source, string propertyName)
        {
            return source.GetType()
                .GetRuntimeProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                ?.GetValue(source);
        }

        public static bool TryGetPropertyValue<T>(this object source, string propertyName, out T result)
        {
            var property = source.GetType()
                .GetRuntimeProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase));

            if (property != null)
            {
                result = (T)property.GetValue(source);
                return true;
            }
            result = default;
            return false;
        }

        public static void SetPropertyValue<T>(this object source, string propertyName, T value)
        {
            source.GetType()
                .GetRuntimeProperties()
                .FirstOrDefault(p => string.Equals(p.Name, propertyName, StringComparison.OrdinalIgnoreCase))
                ?.SetValue(source, value);
        }

        public static bool IsAssignableToGenericType(this object source, Type genericType)
        {
            return source.GetType().IsAssignableToGenericType(genericType);
        }

        public static string ToJson(this object obj)
        {
            var token = JToken.FromObject(obj, new JsonSerializer() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            return token.ToString();
        }

        public static FormUrlEncodedContent ToFormUrlEncodedContent(this object o)
        {
            var json = JsonConvert.SerializeObject(o);
            var dictionary = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return new FormUrlEncodedContent(dictionary);
        }
    }
}
