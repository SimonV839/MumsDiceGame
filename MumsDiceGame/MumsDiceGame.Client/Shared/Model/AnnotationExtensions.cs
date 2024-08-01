namespace MumsDiceGame.Client.Shared.Model
{
    public static class AnnotationExtensions
    {
        /// <summary>
        /// Gets the value of an attribute
        /// </summary>
        /// <typeparam name="T">The type of the attribute required (eg DisplayAttribute)</typeparam>
        /// <param name="instance">the object which is to be checked</param>
        /// <param name="propertyName">the name of the attribute property</param>
        /// <returns>the value if present else null</returns>
        public static T? GetAttributeFrom<T>(this object instance, string propertyName) where T : Attribute
        {
            var attrType = typeof(T);
            var property = instance.GetType().GetProperty(propertyName);
            if (property == null)
            {
                return null;
            }
            return (T)property.GetCustomAttributes(attrType, false).First();
        }
    }
}
