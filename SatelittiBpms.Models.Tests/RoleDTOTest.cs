using NUnit.Framework;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.HandleException;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Tests
{
    class RoleDTOTest
    {
        [Test]
        public void ensureThatNameCanNotBeNull()
        {
            RoleDTO dto = new RoleDTO() { Name = null };
            var result = dto.Validate();
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Equals("Name")));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.ROLE_NAME_REQUIRED)));
        }

        [Test]
        public void ensureThatNameCanNotBeEmpty()
        {
            RoleDTO dto = new RoleDTO() { Name = String.Empty };
            var result = dto.Validate();
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Equals("Name")));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.ROLE_NAME_REQUIRED)));
        }

        [Test]
        public void ensureThatUsersIdsCanNotBeEmpty()
        {
            RoleDTO dto = new RoleDTO() { UsersIds = new List<int>() };
            var result = dto.Validate();
            Assert.IsFalse(result.IsValid);
            Assert.IsTrue(result.Errors.Any(x => x.PropertyName.Equals("UsersIds")));
            Assert.IsTrue(result.Errors.Any(x => x.ErrorMessage.Equals(ExceptionCodes.ROLE_USERS_IDS_REQUIRED)));
        }

        [Test]
        public void ensureThatCanBeValid()
        {
            RoleDTO dto = new RoleDTO() { Name = "someName", UsersIds = new List<int>() { 1 } };
            var result = dto.Validate();
            Assert.IsTrue(result.IsValid);
            Assert.AreEqual(0, result.Errors.Count);
        }

        [Test]
        public void ensureThatGetSetTenantId()
        {
            RoleDTO dto = new RoleDTO();
            dto.SetTenantId(99);
            Assert.AreEqual(99, dto.GetTenantId());
        }
    }
}
