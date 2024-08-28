using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class ActivityUserOptionDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantIdWithIntParam()
        {
            ActivityUserOptionDTO dto = new ActivityUserOptionDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }

        [Test]
        public void ensureThatGetSetTenantIdWithLongParam()
        {
            ActivityUserOptionDTO dto = new ActivityUserOptionDTO();
            dto.SetTenantId(98L);
            Assert.AreEqual(98, dto.GetTenantId());
        }
    }
}
