using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Repository.Tests.Infos;

namespace SatelittiBpms.Repository.Tests.Context
{
    public class BpmsTestHelperContext : DbContext
    {
        public virtual DbSet<TestHelperInfo> TestHelpers { get; set; }

        public BpmsTestHelperContext() : base()
        {
        }
    }
}
