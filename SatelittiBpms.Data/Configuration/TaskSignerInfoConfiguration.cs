using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class TaskSignerInfoConfiguration : IEntityTypeConfiguration<TaskSignerInfo>
    {
        public void Configure(EntityTypeBuilder<TaskSignerInfo> builder)
        {
            builder.HasOne(x => x.Task)
                 .WithMany(x => x.SignerTasks)
                 .HasForeignKey(x => x.TaskId);

            builder.HasMany(g => g.Files)
             .WithOne(s => s.TaskSigner)
            .HasForeignKey(s => s.TaskSignerId);
        }
    }
}
