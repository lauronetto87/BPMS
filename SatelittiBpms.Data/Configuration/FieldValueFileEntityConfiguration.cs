using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class FieldValueFileEntityConfiguration : IEntityTypeConfiguration<FieldValueFileInfo>
    {
        public void Configure(EntityTypeBuilder<FieldValueFileInfo> builder)
        {
            builder.HasOne(x => x.FieldValue)
                 .WithMany(x => x.FieldValueFiles)
                 .HasForeignKey(x => x.FieldValueId);

            builder.HasOne(x => x.UploadedFieldValue)
                 .WithMany(x => x.UploadedFieldValueFiles)
                 .HasForeignKey(x => x.UploadedFieldValueId);
        }
    }
}
