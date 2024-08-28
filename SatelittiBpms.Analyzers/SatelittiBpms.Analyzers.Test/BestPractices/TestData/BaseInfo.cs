namespace Satelitti.Model
{
    public abstract class TenantBaseInfo
    {
        protected TenantBaseInfo() { }

        public int? TenantId { get; set; }

        public static string GetPropertyCorrectDescription(string pPropertyDescription)
        {
            return pPropertyDescription;
        }
    }

    public abstract class BaseInfo : TenantBaseInfo
    {
        protected BaseInfo() { }

        public int Id { get; set; }

        public static string GetPropertyCorrectDescription(string pPropertyDescription)
        {
            return pPropertyDescription;
        }
    }
}