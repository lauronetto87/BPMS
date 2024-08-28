using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class ActivityDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantIdWithIntParam()
        {
            ActivityDTO dto = new ActivityDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }

        [Test]
        public void ensureThatGetSetTenantIdWithLongParam()
        {
            ActivityDTO dto = new ActivityDTO();
            dto.SetTenantId(98L);
            Assert.AreEqual(98, dto.GetTenantId());
        }
    }
}
