using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class TenantEntityConfiguration : IEntityTypeConfiguration<TenantInfo>
    {
        public void Configure(EntityTypeBuilder<TenantInfo> builder)
        {   
            
        }
    }
}
