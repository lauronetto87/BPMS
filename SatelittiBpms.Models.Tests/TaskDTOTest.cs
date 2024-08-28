using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class TaskDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantId()
        {
            TaskDTO dto = new TaskDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }
    }
}
