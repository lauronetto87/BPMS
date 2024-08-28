using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Data.Configuration
{
    public class TemplateEntityConfiguration : IEntityTypeConfiguration<TemplateInfo>
    {
        public void Configure(EntityTypeBuilder<TemplateInfo> builder)
        {
        }
    }
}
