using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    class ActivityFieldDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantIdWithIntParam()
        {
            ActivityFieldDTO dto = new ActivityFieldDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }

        [Test]
        public void ensureThatGetSetTenantIdWithLongParam()
        {
            ActivityFieldDTO dto = new ActivityFieldDTO();
            dto.SetTenantId(98L);
            Assert.AreEqual(98, dto.GetTenantId());
        }
    }
}
