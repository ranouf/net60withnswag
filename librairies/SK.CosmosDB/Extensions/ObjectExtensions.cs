using SK.Extensions;

namespace SK.CosmosDB.Extensions
{
    public static class ObjectExtensions
    {
        public static T GetPropertyValue<T>(this object source, string propertyName) where T : class
        {
            return (T)source.GetPropertyValue(propertyName);
        }
    }
}
