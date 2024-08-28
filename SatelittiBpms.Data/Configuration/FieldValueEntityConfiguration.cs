using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class FieldValueEntityConfiguration : IEntityTypeConfiguration<FieldValueInfo>
    {
        public void Configure(EntityTypeBuilder<FieldValueInfo> builder)
        {
            builder.HasOne(x => x.Field)
                 .WithMany(x => x.FieldValues)
                 .HasForeignKey(x => x.FieldId);

            builder.HasOne(x => x.Flow)
                .WithMany(x => x.FieldValues)
                .HasForeignKey(x => x.FieldId);

            builder.HasOne(x => x.Task)
                .WithMany(x => x.FieldsValues)
                .HasForeignKey(x => x.FieldId);

            builder.HasMany(x => x.FieldValueFiles)
                .WithOne(s => s.FieldValue)
                .HasForeignKey(s => s.FieldValueId);
        }
    }
}
