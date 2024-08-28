using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class ActivityUserEntityConfiguration : IEntityTypeConfiguration<ActivityUserInfo>
    {
        public void Configure(EntityTypeBuilder<ActivityUserInfo> builder)
        {
            builder.HasOne(x => x.Role)
                .WithMany(x => x.ActivityUsers)
                .HasForeignKey(x => x.RoleId);

            builder.HasOne(ad => ad.Activity)
                .WithOne(s => s.ActivityUser)
                .HasForeignKey<ActivityUserInfo>(ad => ad.Id);
        }
    }
}
