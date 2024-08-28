namespace SatelittiBpms.Models.ViewModel
{
    public class TenantAuthViewModel
    {
        public int Id { get; set; }
        public string SubDomain { get; set; }
        public string Name { get; set; }
        public string Zone { get; set; }
        public bool Customizable { get; set; }
        public string SystemName { get; set; }        
        public bool Print_id { get; set; }
        public string AccessKey { get; set; }
        public bool AuthenticatedByAccessKey { get; set; }
        public float Timezone { get; set; }
    }
}
