using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    public class ProcessFilterDTOTest
    {
        [Test]
        public void ensureThatGetSetTenantId()
        {
            ProcessFilterDTO dto = new ProcessFilterDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }

        [Test]
        public void ensureIsOrderAscReturnTrueWhenOrderIsAsc()
        {
            ProcessFilterDTO dto = new ProcessFilterDTO();
            dto.SortOrder = 0;
            Assert.IsTrue(dto.IsOrderAsc());
        }

        [Test]
        public void ensureIsOrderAscReturnFalseWhenOrderIsNotAsc()
        {
            ProcessFilterDTO dto = new ProcessFilterDTO();
            dto.SortOrder = 1;
            Assert.IsFalse(dto.IsOrderAsc());
        }
    }
}
