using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class ActivityUserOptionEntityConfiguration : IEntityTypeConfiguration<ActivityUserOptionInfo>
    {
        public void Configure(EntityTypeBuilder<ActivityUserOptionInfo> builder)
        {
            builder.HasOne(x => x.ActivityUser)
                 .WithMany(x => x.ActivityUsersOptions)
                 .HasForeignKey(x => x.ActivityUserId);
        }
    }
}
