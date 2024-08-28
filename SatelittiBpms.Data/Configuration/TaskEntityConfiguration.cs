using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class TaskEntityConfiguration : IEntityTypeConfiguration<TaskInfo>
    {
        public void Configure(EntityTypeBuilder<TaskInfo> builder)
        {
            builder.HasOne(x => x.Activity)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.ActivityId);

            builder.HasOne(x => x.Flow)
                .WithMany(x => x.Tasks)
                .HasForeignKey(x => x.FlowId);

            builder.HasOne(x => x.Executor)
                .WithMany(x => x.ExecutorTasks)
                .HasForeignKey(x => x.ExecutorId);

            builder.HasMany(g => g.FieldsValues)
                .WithOne(s => s.Task)
               .HasForeignKey(s => s.TaskId);

            builder.HasMany(g => g.SourceTasks)
                .WithOne(s => s.TargetTask)
               .HasForeignKey(s => s.TargetTaskId);

            builder.HasMany(g => g.TargetTasks)
                .WithOne(s => s.SourceTask)
               .HasForeignKey(s => s.SourceTaskId);

            builder.HasMany(g => g.TasksHistories)
                .WithOne(s => s.Task)
               .HasForeignKey(s => s.TaskId);

            builder.HasOne(g => g.Option)
                .WithMany(s => s.OptionTasks)
               .HasForeignKey(s => s.OptionId);

            builder.HasMany(g => g.SignerTasks)
                .WithOne(s => s.Task)
               .HasForeignKey(s => s.TaskId);

            builder.HasMany(g => g.Notifications)
              .WithOne(s => s.Task)
             .HasForeignKey(s => s.TaskId);
        }
    }
}
