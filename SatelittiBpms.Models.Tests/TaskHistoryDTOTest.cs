using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class TaskHistoryDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantId()
        {
            TaskHistoryDTO dto = new TaskHistoryDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }
    }
}
