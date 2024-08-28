using NUnit.Framework;
using SatelittiBpms.Models.Tests.DTO;

namespace SatelittiBpms.Models.Tests
{
    class DTOValidatorBaseTest
    {
        [Test]
        public void ensureBeferoValidate()
        {
            DTOValidatorBaseHelper dto = new DTOValidatorBaseHelper();
            dto.BeforeValidate();
            Assert.Pass();
        }
    }
}
