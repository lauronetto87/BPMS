using NUnit.Framework;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using System.Linq;

namespace SatelittiBpms.Models.Tests
{
    public class ResultTest
    {
        [Test]
        public void ensureSuccessWithType()
        {
            UserInfo user = new UserInfo()
            {
                Id = 1,
                Enable = true,
                TenantId = 55,
                Type = Enums.BpmsUserTypeEnum.PUBLISHER,
                Timezone = -4
            };

            var result = Result.Result.Success<UserInfo>(user);
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorId);
            Assert.IsNotNull(result.Value);
            Assert.AreEqual(user.Id, result.Value.Id);
            Assert.AreEqual(user.Enable, result.Value.Enable);
            Assert.AreEqual(user.TenantId, result.Value.TenantId);
            Assert.AreEqual(user.Type, result.Value.Type);
            Assert.AreEqual(user.Timezone, result.Value.Timezone);
        }

        [Test]
        public void ensureSuccessWithoutType()
        {
            var result = Result.Result.Success();
            Assert.IsTrue(result.Success);
            Assert.IsNull(result.ErrorId);
        }

        [Test]
        public void ensureErrorWithMessageId()
        {
            var result = Result.Result.Error("Erro ID");
            Assert.IsFalse(result.Success);
            Assert.AreEqual("Erro ID", result.ErrorId);
        }

        [Test]
        public void ensureErrorWithValidationResultWithoutValue()
        {
            RoleDTO dto = new RoleDTO() { Name = null };
            var roleValidationResult = dto.Validate();
            var result = Result.Result.Error(roleValidationResult);
            Assert.IsFalse(result.Success);
            Assert.IsFalse(result.MergeErrorsList);
            Assert.IsTrue(result.ValidationResult.Errors.Any(x => x.PropertyName.Equals("Name")));
            Assert.IsTrue(result.ValidationResult.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.ROLE_NAME_REQUIRED)));
        }

        [Test]
        public void ensureErrorWithValidationResultWithValue()
        {
            RoleDTO dto = new RoleDTO() { Name = null };
            var roleValidationResult = dto.Validate();
            var result = Result.Result.Error<int>(22, roleValidationResult, true);
            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.MergeErrorsList);
            Assert.AreEqual(22, result.Value);
            Assert.IsTrue(result.ValidationResult.Errors.Any(x => x.PropertyName.Equals("Name")));
            Assert.IsTrue(result.ValidationResult.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.ROLE_NAME_REQUIRED)));
        }
    }
}
