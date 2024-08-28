using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class TaskSignerFileInfoConfiguration : IEntityTypeConfiguration<TaskSignerFileInfo>
    {
        public void Configure(EntityTypeBuilder<TaskSignerFileInfo> builder)
        {
            builder.HasOne(x => x.FieldValueFile)
                .WithOne(x => x.TaskSignerFile)
                .HasForeignKey<TaskSignerFileInfo>(x => x.FieldValueFileId);

            builder.HasOne(g => g.TaskSigner)
             .WithMany(s => s.Files)
            .HasForeignKey(s => s.TaskSignerId);
        }
    }
}
