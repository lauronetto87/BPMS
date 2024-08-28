using SatelittiBpms.Models.Enums;

namespace SatelittiBpms.Models.DTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public BpmsUserTypeEnum Type { get; set; }
        public bool Enable { get; set; }
        public int? Timezone { get; set; }
        public int? TenantId { get; set; }
    }
}
