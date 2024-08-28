using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class FieldEntityConfiguration : IEntityTypeConfiguration<FieldInfo>
    {
        public void Configure(EntityTypeBuilder<FieldInfo> builder)
        {
            builder.HasOne(x => x.ProcessVersion)
                .WithMany(x => x.Fields)
                .HasForeignKey(x => x.ProcessVersionId);

            builder.HasMany(g => g.FieldValues)
             .WithOne(s => s.Field)
            .HasForeignKey(s => s.FieldId);
        }
    }
}
