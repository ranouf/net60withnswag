using System;

namespace SK.Extensions
{
    public static class TypeExtensions
    {
        public static bool IsAssignableToGenericType(this Type givenType, Type genericType)
        {
            foreach (var it in givenType.GetInterfaces())
            {
                if (it.IsGenericType && it.GetGenericTypeDefinition() == genericType)
                    return true;
            }

            if (givenType.IsGenericType && givenType.GetGenericTypeDefinition() == genericType)
                return true;

            Type baseType = givenType.BaseType;
            if (baseType == null) return false;

            return IsAssignableToGenericType(baseType, genericType);
        }

        public static string GetName(this Type type)
        {
            var typeName = type.Name.Contains("`")
                ? type.Name.Split("`")[0]
                : type.Name;
            var genericArgs = type.GetGenericArguments();
            if (genericArgs.Length > 0)
            {
                typeName += "<";
                foreach (var genericArg in genericArgs)
                {
                    typeName += genericArg.GetName() + ", ";
                }
                typeName = typeName.TrimEnd(',', ' ') + ">";
            }
            return typeName;
        }
    }
}
