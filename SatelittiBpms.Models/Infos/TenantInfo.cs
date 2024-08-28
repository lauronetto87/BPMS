namespace SatelittiBpms.Models.Infos
{
    public class TenantInfo
    {
        public int Id { get; set; }
        public string AccessKey { get; set; }
        public string SubDomain { get; set; }
        public int? DefaultRoleId { get; set; }
        public string SignerAccessToken { get; set; }
    }
}
