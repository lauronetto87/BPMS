using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class FlowPathEntityConfiguration : IEntityTypeConfiguration<FlowPathInfo>
    {
        public void Configure(EntityTypeBuilder<FlowPathInfo> builder)
        {
            builder.HasOne(x => x.Flow)
               .WithMany(x => x.FlowPaths)
               .HasForeignKey(x => x.FlowId);

            builder.HasOne(x => x.SourceTask)
                .WithMany(x => x.TargetTasks)
                .HasForeignKey(x => x.SourceTaskId);

            builder.HasOne(x => x.TargetTask)
                .WithMany(x => x.SourceTasks)
                .HasForeignKey(x => x.TargetTaskId);
        }
    }
}
