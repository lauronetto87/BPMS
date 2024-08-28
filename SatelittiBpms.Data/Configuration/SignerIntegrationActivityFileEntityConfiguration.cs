using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class SignerIntegrationActivityFileEntityConfiguration : IEntityTypeConfiguration<SignerIntegrationActivityFileInfo>
    {
        public void Configure(EntityTypeBuilder<SignerIntegrationActivityFileInfo> builder)
        {
            builder.HasOne(x => x.SignerIntegrationActivity)
                 .WithMany(x => x.Files)
                 .HasForeignKey(x => x.SignerIntegrationActivityId);

            builder.HasOne(x => x.FileField)
                .WithMany(x => x.SignerIntegrationActivityFile)
                .HasForeignKey(x => x.FileFieldId);
        }
    }
}
