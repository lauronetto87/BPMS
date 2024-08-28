using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Tests
{
    public class RoleInfoTest
    {
        [Test]
        public void ensureThatReturnViewModel()
        {
            RoleInfo info = new RoleInfo()
            {
                Name = "SomeName TEST",
                Id = 22
            };

            var result = info.AsViewModel();
            Assert.AreEqual(info.Name, result.Name);
            Assert.AreEqual(info.Id, result.Id);
        }

        [Test]
        public void ensureThatReturnDetailViewModel()
        {
            RoleInfo info = new RoleInfo()
            {
                Name = "SomeName TEST",
                Id = 22,
                RoleUsers = new List<RoleUserInfo>() {
                    new RoleUserInfo(){UserId = 2 },
                    new RoleUserInfo(){UserId = 5 }
                }
            };

            var result = info.AsDetailViewModel();
            Assert.AreEqual(info.Name, result.Name);
            Assert.AreEqual(info.Id, result.Id);
            Assert.AreEqual(2, result.UsersIds.Count);
            Assert.IsTrue(result.UsersIds.Contains(2));
            Assert.IsTrue(result.UsersIds.Contains(5));

        }
    }
}
