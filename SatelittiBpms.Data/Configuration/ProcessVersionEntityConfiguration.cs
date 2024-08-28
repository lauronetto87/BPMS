using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class ProcessVersionEntityConfiguration : IEntityTypeConfiguration<ProcessVersionInfo>
    {
        public void Configure(EntityTypeBuilder<ProcessVersionInfo> builder)
        {
            builder.Property(x => x.Name).HasColumnType("varchar").HasMaxLength(200);

            builder.HasOne(x => x.Process)
                .WithMany(x => x.ProcessVersions)
                .HasForeignKey(x => x.ProcessId);

            builder.HasMany(g => g.Flows)
               .WithOne(s => s.ProcessVersion)
              .HasForeignKey(s => s.ProcessVersionId);

            builder.HasMany(g => g.Fields)
             .WithOne(s => s.ProcessVersion)
            .HasForeignKey(s => s.ProcessVersionId);

            builder.HasMany(g => g.Activities)
             .WithOne(s => s.ProcessVersion)
            .HasForeignKey(s => s.ProcessVersionId);

            builder.HasMany(g => g.ActivityFields)
             .WithOne(s => s.ProcessVersion)
            .HasForeignKey(s => s.ProcessVersionId);
        }
    }
}
