using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class UserEntityConfiguration : IEntityTypeConfiguration<UserInfo>
    {
        public void Configure(EntityTypeBuilder<UserInfo> builder)
        {
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.HasIndex(x => new { x.TenantId });

            builder.HasMany(g => g.RequesterFlows)
                .WithOne(s => s.Requester)
               .HasForeignKey(s => s.RequesterId);

            builder.HasMany(g => g.ExecutorTasks)
                .WithOne(s => s.Executor)
               .HasForeignKey(s => s.ExecutorId);

            builder.HasMany(g => g.ExecutorTaskHistories)
                .WithOne(s => s.Executor)
               .HasForeignKey(s => s.ExecutorId);

            builder.HasMany(g => g.RoleUsers)
               .WithOne(s => s.User)
              .HasForeignKey(s => s.UserId);

            builder.HasMany(g => g.Notifications)
              .WithOne(s => s.User)
             .HasForeignKey(s => s.UserId);
        }
    }
}
