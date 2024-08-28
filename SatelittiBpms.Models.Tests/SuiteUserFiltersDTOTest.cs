using NUnit.Framework;
using SatelittiBpms.Models.DTO;

namespace SatelittiBpms.Models.Tests
{
    class SuiteUserFiltersDTOTest
    {
        [Test]
        public void ensureThatDefaultSelectAllIsFalse()
        {
            SuiteUserFiltersDTO dto = new SuiteUserFiltersDTO();
            Assert.IsFalse(dto.SelectAll);
        }
    }
}
