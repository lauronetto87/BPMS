using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class FieldDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantIdWithIntParam()
        {
            FieldDTO dto = new FieldDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }

        [Test]
        public void ensureThatGetSetTenantIdWithLongParam()
        {
            FieldDTO dto = new FieldDTO();
            dto.SetTenantId(98L);
            Assert.AreEqual(98, dto.GetTenantId());
        }
    }
}
