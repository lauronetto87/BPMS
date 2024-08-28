using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class ActivityUserDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantIdWithIntParam()
        {
            ActivityUserDTO dto = new ActivityUserDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }

        [Test]
        public void ensureThatGetSetTenantIdWithLongParam()
        {
            ActivityUserDTO dto = new ActivityUserDTO();
            dto.SetTenantId(98L);
            Assert.AreEqual(98, dto.GetTenantId());
        }
    }
}
