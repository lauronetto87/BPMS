using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class FlowEntityConfiguration : IEntityTypeConfiguration<FlowInfo>
    {
        public void Configure(EntityTypeBuilder<FlowInfo> builder)
        {
            builder.HasOne(x => x.ProcessVersion)
                 .WithMany(x => x.Flows)
                 .HasForeignKey(x => x.ProcessVersionId);

            builder.HasOne(x => x.Requester)
                 .WithMany(x => x.RequesterFlows)
                 .HasForeignKey(x => x.RequesterId);

            builder.HasMany(g => g.FieldValues)
               .WithOne(s => s.Flow)
              .HasForeignKey(s => s.FlowId);

            builder.HasMany(g => g.FlowPaths)
               .WithOne(s => s.Flow)
              .HasForeignKey(s => s.FlowId);

            builder.HasMany(g => g.Tasks)
               .WithOne(s => s.Flow)
              .HasForeignKey(s => s.FlowId);

            builder.HasMany(g => g.Notifications)
              .WithOne(s => s.Flow)
             .HasForeignKey(s => s.FlowId);
        }
    }
}
