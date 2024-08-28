using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class ProcessRoleEntityConfiguration : IEntityTypeConfiguration<ProcessVersionRoleInfo>
    {
        public void Configure(EntityTypeBuilder<ProcessVersionRoleInfo> builder)
        {
            builder.HasOne(x => x.Role)
             .WithMany(x => x.ProcessUsers)
             .HasForeignKey(x => x.RoleId);

            builder.HasOne(x => x.ProcessVersion)
                .WithMany(x => x.ProcessVersionRoles)
                .HasForeignKey(x => x.ProcessVersionId);
        }
    }
}
