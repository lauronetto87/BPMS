using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class VersionNormalizationEntityConfiguration : IEntityTypeConfiguration<VersionNormalizationInfo>
    {
        public void Configure(EntityTypeBuilder<VersionNormalizationInfo> builder)
        {

        }
    }
}
