using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class ActivityEntityConfiguration : IEntityTypeConfiguration<ActivityInfo>
    {
        public void Configure(EntityTypeBuilder<ActivityInfo> builder)
        {
            builder.HasOne(x => x.ProcessVersion)
                 .WithMany(x => x.Activities)
                 .HasForeignKey(x => x.ProcessVersionId);

            builder.HasMany(g => g.Tasks)
                .WithOne(s => s.Activity)
                .HasForeignKey(s => s.ActivityId);

            builder.HasOne(s => s.ActivityUser)
                .WithOne(ad => ad.Activity)
                .HasForeignKey<ActivityUserInfo>(ad => ad.Id);

            builder.HasOne(x => x.SignerIntegrationActivity)
                .WithOne(x => x.Activity)
                .HasForeignKey<SignerIntegrationActivityInfo>(x => x.ActivityId);

            builder.HasMany(x => x.SignerIntegrationActivitySignatoryOrigin)
                .WithOne(x => x.OriginActivity)
                .HasForeignKey(x => x.OriginActivityId);

            builder.HasMany(x => x.SignerIntegrationActivityAuthorizerOrigin)
                .WithOne(x => x.OriginActivity)
                .HasForeignKey(x => x.OriginActivityId);
        }
    }
}
