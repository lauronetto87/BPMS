using NUnit.Framework;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Models.Enums;
using System.ComponentModel;

namespace SatelittiBpms.Authentication.Tests
{
    public class UserRolesTest
    {
        [Test]
        public void ensureThatFromBpmsUserTypeReturnsAdministratorWhenTypeIsAdministrator()
        {
            string result = UserRoles.FromBpmsUserType(BpmsUserTypeEnum.ADMINISTRATOR);
            Assert.AreEqual(UserRoles.ADMINISTRATOR, result);
        }

        [Test]
        public void ensureThatFromBpmsUserTypeReturnsPublisherWhenTypeIsPublisher()
        {
            string result = UserRoles.FromBpmsUserType(BpmsUserTypeEnum.PUBLISHER);
            Assert.AreEqual(UserRoles.PUBLISHER, result);
        }

        [Test]
        public void ensureThatFromBpmsUserTypeReturnsReaderWhenTypeIsReader()
        {
            string result = UserRoles.FromBpmsUserType(BpmsUserTypeEnum.READER);
            Assert.AreEqual(UserRoles.READER, result);
        }

        [Test]
        public void ensureThatFromBpmsUserTypeThrowsWhenTypeNotMatches()
        {
            Assert.Throws(Is.TypeOf<InvalidEnumArgumentException>().And.Message.EqualTo("bpmsUserType"),
            () => UserRoles.FromBpmsUserType((BpmsUserTypeEnum)99));            
        }
    }
}
