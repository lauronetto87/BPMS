using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class SignerIntegrationActivitySignatoryEntityConfiguration : IEntityTypeConfiguration<SignerIntegrationActivitySignatoryInfo>
    {
        public void Configure(EntityTypeBuilder<SignerIntegrationActivitySignatoryInfo> builder)
        {
            builder.HasOne(x => x.SignerIntegrationActivity)
                 .WithMany(x => x.Signatories)
                 .HasForeignKey(x => x.SignerIntegrationActivityId);

            builder.HasOne(x => x.NameField)
                .WithMany(x => x.SignerIntegrationActivitySignatoryName)
                .HasForeignKey(x => x.NameFieldId);

            builder.HasOne(x => x.CpfField)
                .WithMany(x => x.SignerIntegrationActivitySignatoryCpf)
                .HasForeignKey(x => x.CpfFieldId);

            builder.HasOne(x => x.EmailField)
                .WithMany(x => x.SignerIntegrationActivitySignatoryEmail)
                .HasForeignKey(x => x.EmailFieldId);

            builder.HasOne(x => x.OriginActivity)
                .WithMany(x => x.SignerIntegrationActivitySignatoryOrigin)
                .HasForeignKey(x => x.OriginActivityId);
        }
    }
}
