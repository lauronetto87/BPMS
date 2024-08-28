namespace SatelittiBpms.Services.Helpers
{
    public static class TypeHelper
    {
        public static object GetBalueByProperty(object obj, string propertyName)
        {
            if (obj == null)
            {
                return null;
            }
            var objType = obj.GetType();
            var objProperty = objType.GetProperty(propertyName);
            return objProperty.GetValue(obj);
        }
    }
}
