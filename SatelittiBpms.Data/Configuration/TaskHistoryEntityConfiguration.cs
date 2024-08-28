using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class TaskHistoryEntityConfiguration : IEntityTypeConfiguration<TaskHistoryInfo>
    {
        public void Configure(EntityTypeBuilder<TaskHistoryInfo> builder)
        {
            builder.HasOne(x => x.Task)
                 .WithMany(x => x.TasksHistories)
                 .HasForeignKey(x => x.TaskId);

            builder.HasOne(x => x.Executor)
                .WithMany(x => x.ExecutorTaskHistories)
                .HasForeignKey(x => x.TaskId);
        }
    }
}
