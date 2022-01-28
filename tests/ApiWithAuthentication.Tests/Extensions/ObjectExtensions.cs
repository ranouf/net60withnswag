using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using SK.Extensions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace ApiWithAuthentication.Tests.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToQueryString(this object obj)
        {
            if (!obj.GetType().IsComplex())
            {
                return obj.ToString();
            }

            var values = obj
                .GetType()
                .GetProperties()
                .Where(o => o.GetValue(obj, null) != null);

            var result = new QueryString();

            foreach (var value in values)
            {
                if (!typeof(string).IsAssignableFrom(value.PropertyType)
                    && typeof(IEnumerable).IsAssignableFrom(value.PropertyType))
                {
                    var items = value.GetValue(obj) as IList;
                    if (items.Count > 0)
                    {
                        for (int i = 0; i < items.Count; i++)
                        {
                            result = result.Add(value.Name, ToQueryString(items[i]));
                        }
                    }
                }
                else if (value.PropertyType.IsComplex())
                {
                    result = result.Add(value.Name, ToQueryString(value));
                }
                else
                {
                    result = result.Add(value.Name, value.GetValue(obj).ToString());
                }
            }

            return result.Value;
        }

        public static StringContent ToStringContent(this object o)
        {
            var result = new StringContent(o.ToJson(), Encoding.UTF8, "application/json");
            return result;
        }
        public static MultipartFormDataContent ToFormData(this object obj, MultipartFormDataContent parent = null, string parentPropertyName = null)
        {
            try
            {

                if (obj == null)
                {
                    return null;
                }

                var result = parent ?? new MultipartFormDataContent();
                foreach (var property in obj.GetType().GetProperties())
                {
                    var value = property.GetValue(obj);
                    var propertyName = parentPropertyName + property.Name;
                    if (!property.IsNullOrDefaultValue(obj))
                    {
                        if (typeof(string).IsAssignableFrom(property.PropertyType))
                        {
                            result.Add(new StringContent(value.ToString()), propertyName);
                        }
                        else if (typeof(IFormFile).IsAssignableFrom(property.PropertyType))
                        {
                            var formFile = (FormFile)value;
                            result.Add(new ByteArrayContent(formFile.OpenReadStream().ReadAllBytes()), propertyName, formFile.FileName);
                        }
                        else if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                        {
                            if (property.PropertyType == typeof(IEnumerable<IFormFile>))
                            {
                                foreach (var item in (IEnumerable)value)
                                {
                                    result = item.ToFormData(result, $"{propertyName}.");
                                }
                            }
                            else
                            {
                                var i = 0;
                                foreach (var item in (IEnumerable)value)
                                {
                                    result = item.ToFormData(result, $"{propertyName}[{i}].");
                                    i++;
                                }
                            }
                        }
                        else if (property.PropertyType.IsClass)
                        {
                            result.Add(ToFormData(value));
                        }
                        else
                        {
                            result.Add(new StringContent(value.ToString()), propertyName);
                        }
                    }
                }
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #region Private
        private static bool IsNullOrDefaultValue(this PropertyInfo propertyInfo, object obj)
        {
            var value = propertyInfo.GetValue(obj);
            var type = propertyInfo.PropertyType;
            if (type.IsValueType)
            {
                var defaultObj = Activator.CreateInstance(type);
                return defaultObj == null
                    ? value == null
                    : defaultObj.Equals(value);
            }
            else
            {
                return value == null;
            }
        }

        private static byte[] ReadAllBytes(this Stream instream)
        {
            if (instream is MemoryStream stream)
            {
                return stream.ToArray();
            }

            using var memoryStream = new MemoryStream();
            instream.CopyTo(memoryStream);
            return memoryStream.ToArray();
        }

        private static bool IsComplex(this Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                // nullable type, check if the nested type is simple.
                return IsComplex(typeInfo.GetGenericArguments()[0]);
            }
            return !(typeInfo.IsPrimitive
              || typeInfo.IsEnum
              || type.Equals(typeof(Guid))
              || type.Equals(typeof(string))
              || type.Equals(typeof(decimal)));
        }
        #endregion
    }
}
