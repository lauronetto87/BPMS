using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class RoleEntityConfiguration : IEntityTypeConfiguration<RoleInfo>
    {
        public void Configure(EntityTypeBuilder<RoleInfo> builder)
        {
            builder.HasMany(g => g.ActivityUsers)
                .WithOne(s => s.Role)
               .HasForeignKey(s => s.RoleId);

            builder.HasMany(g => g.Notifications)
                .WithOne(s => s.Role)
               .HasForeignKey(s => s.RoleId);
        }
    }
}
