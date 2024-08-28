using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class ActivityFieldEntityConfiguration : IEntityTypeConfiguration<ActivityFieldInfo>
    {
        public void Configure(EntityTypeBuilder<ActivityFieldInfo> builder)
        {
            builder.HasOne(x => x.ProcessVersion)
                 .WithMany(x => x.ActivityFields)
                 .HasForeignKey(x => x.ProcessVersionId);

            builder.HasOne(x => x.Activity)
                .WithMany(x => x.ActivityFields)
                .HasForeignKey(x => x.ActivityId);

            builder.HasOne(x => x.Field)
                .WithMany(x => x.ActivityFields)
                .HasForeignKey(x => x.FieldId);
        }
    }
}
