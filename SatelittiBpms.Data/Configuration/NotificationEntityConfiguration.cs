using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class NotificationEntityConfiguration : IEntityTypeConfiguration<NotificationInfo>
    {
        public void Configure(EntityTypeBuilder<NotificationInfo> builder)
        {
            builder.HasOne(x => x.User)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.UserId);

            builder.HasOne(x => x.Flow)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.FlowId);

            builder.HasOne(x => x.Task)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.TaskId);

            builder.HasOne(x => x.Role)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.RoleId);
        }
    }
}
