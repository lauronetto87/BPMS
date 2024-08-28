using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class ActivityNotificationDTOTest
    {
        [Test]
        public void EnsureThatGetSetTenantIdWithIntParam()
        {
            ActivityNotificationDTO dto = new();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }

        [Test]
        public void EnsureThatGetSetTenantIdWithLongParam()
        {
            ActivityNotificationDTO dto = new();
            dto.SetTenantId(98L);
            Assert.AreEqual(98, dto.GetTenantId());
        }
    }
}
