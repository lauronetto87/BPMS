using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class TaskFilterDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantId()
        {
            TaskFilterDTO dto = new TaskFilterDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }

        [Test]
        public void ensureIsOrderAscReturnTrueWhenOrderIsAsc()
        {
            TaskFilterDTO dto = new TaskFilterDTO();
            dto.SortOrder = 0;
            Assert.IsTrue(dto.IsOrderAsc());
        }

        [Test]
        public void ensureIsOrderAscReturnFalseWhenOrderIsNotAsc()
        {
            TaskFilterDTO dto = new TaskFilterDTO();
            dto.SortOrder = 1;
            Assert.IsFalse(dto.IsOrderAsc());
        }
    }
}
