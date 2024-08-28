using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class SignerIntegrationActivityAuthorizerEntityConfiguration : IEntityTypeConfiguration<SignerIntegrationActivityAuthorizerInfo>
    {
        public void Configure(EntityTypeBuilder<SignerIntegrationActivityAuthorizerInfo> builder)
        {
            builder.HasOne(x => x.SignerIntegrationActivity)
                 .WithMany(x => x.Authorizers)
                 .HasForeignKey(x => x.SignerIntegrationActivityId);

            builder.HasOne(x => x.NameField)
                .WithMany(x => x.SignerIntegrationActivityAuthorizerName)
                .HasForeignKey(x => x.NameFieldId);

            builder.HasOne(x => x.CpfField)
                .WithMany(x => x.SignerIntegrationActivityAuthorizerCpf)
                .HasForeignKey(x => x.CpfFieldId);

            builder.HasOne(x => x.EmailField)
                .WithMany(x => x.SignerIntegrationActivityAuthorizerEmail)
                .HasForeignKey(x => x.EmailFieldId);

            builder.HasOne(x => x.OriginActivity)
                .WithMany(x => x.SignerIntegrationActivityAuthorizerOrigin)
                .HasForeignKey(x => x.OriginActivityId);
        }
    }
}
