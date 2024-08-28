using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class ProcessEntityConfiguration : IEntityTypeConfiguration<ProcessInfo>
    {
        public void Configure(EntityTypeBuilder<ProcessInfo> builder)
        {
            builder.HasMany(g => g.ProcessVersions)
                .WithOne(s => s.Process)
               .HasForeignKey(s => s.ProcessId);
        }
    }
}
