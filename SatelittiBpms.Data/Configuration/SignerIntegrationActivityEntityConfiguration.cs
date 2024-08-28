using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class SignerIntegrationActivityEntityConfiguration : IEntityTypeConfiguration<SignerIntegrationActivityInfo>
    {
        public void Configure(EntityTypeBuilder<SignerIntegrationActivityInfo> builder)
        {
            builder.HasOne(x => x.Activity)
                 .WithOne(x => x.SignerIntegrationActivity)
                 .HasForeignKey<SignerIntegrationActivityInfo>(x => x.ActivityId);

            builder.HasOne(x => x.ExpirationDateField)
                .WithMany(x => x.SignerIntegrationActivityExpirationDate)
                .HasForeignKey(x => x.ExpirationDateFieldId);

            builder.HasMany(g => g.Authorizers)
                .WithOne(s => s.SignerIntegrationActivity)
                .HasForeignKey(s => s.SignerIntegrationActivityId);

            builder.HasMany(g => g.Signatories)
                .WithOne(s => s.SignerIntegrationActivity)
                .HasForeignKey(s => s.SignerIntegrationActivityId);

            builder.HasMany(g => g.Files)
                .WithOne(s => s.SignerIntegrationActivity)
                .HasForeignKey(s => s.SignerIntegrationActivityId);
        }
    }
}
