using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class RoleUserEntityConfiguration : IEntityTypeConfiguration<RoleUserInfo>
    {
        public void Configure(EntityTypeBuilder<RoleUserInfo> builder)
        {
            builder.HasOne(x => x.Role)
              .WithMany(x => x.RoleUsers)
              .HasForeignKey(x => x.RoleId);

            builder.HasOne(x => x.User)
                .WithMany(x => x.RoleUsers)
                .HasForeignKey(x => x.UserId);
        }
    }
}
