using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class ActivityNotificationEntityConfiguration : IEntityTypeConfiguration<ActivityNotificationInfo>
    {
        public void Configure(EntityTypeBuilder<ActivityNotificationInfo> builder)
        {
            builder.HasOne(x => x.Role)
                .WithMany(x => x.ActivityNotifications)
                .HasForeignKey(x => x.RoleId);

            builder.HasOne(ad => ad.Activity)
                .WithOne(s => s.ActivityNotification)
                .HasForeignKey<ActivityNotificationInfo>(ad => ad.Id);
        }
    }
}
