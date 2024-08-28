using NUnit.Framework;
using Satelitti.Authentication.Model;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Authentication.Tests
{
    public class GenerateTokenParametersTest
    {
        [Test]
        public void EnsureThatAsSignerUserParameterReturnsSuccess()
        {
            var tokenLifetimeInMinutes = 60;

            var bpmsUser = new UserInfo
            {
                Type = BpmsUserTypeEnum.ADMINISTRATOR,
                Timezone = -4
            };

            var suiteUser = new SuiteUserAuth
            {
                Id = 10,
                Name = "Teste",
                Mail = "teste@teste.com",
                SuiteToken = "aaaa"
            };

            var parameteres = new GenerateTokenParameters
            {
                Role = UserRoles.FromBpmsUserType(bpmsUser.Type),
                Sid = suiteUser.Id.ToString(),
                Name = suiteUser.Name,
                Email = suiteUser.Mail,
                SuiteToken = suiteUser.SuiteToken,
                TokenLifetimeInMinutes = tokenLifetimeInMinutes,
                Timezone = bpmsUser.Timezone
            };

            var parametersAsBpmsUser = GenerateTokenParameters
                                .AsBpmsUserParameter(suiteUser, bpmsUser, tokenLifetimeInMinutes);

            Assert.AreEqual(parametersAsBpmsUser.Name, parameteres.Name);
            Assert.AreEqual(parametersAsBpmsUser.Sid, parameteres.Sid);
            Assert.AreEqual(parametersAsBpmsUser.Email, parameteres.Email);
            Assert.AreEqual(parametersAsBpmsUser.SuiteToken, parameteres.SuiteToken);
            Assert.AreEqual(parametersAsBpmsUser.Role, parameteres.Role);            
            Assert.AreEqual(parametersAsBpmsUser.Timezone, parameteres.Timezone);
        }

        [Test]
        public void EnsureThatGetTimezoneReturnsEmptyWhenIsNull()
        {
            GenerateTokenParameters generateTokenParameters = new GenerateTokenParameters();
            generateTokenParameters.Timezone = null;
            string result = generateTokenParameters.GetTimezone();
            Assert.IsEmpty(result);
        }

        [Test]
        public void EnsureThatGetTimezoneReturnsNotEmptyWhenIsNotNull()
        {
            GenerateTokenParameters generateTokenParameters = new GenerateTokenParameters();
            generateTokenParameters.Timezone = 8;
            string result = generateTokenParameters.GetTimezone();
            Assert.IsNotEmpty(result);
            Assert.AreEqual("8", result);
        }

        [Test]
        public void EnsureThatGetSuiteTokenReturnsEmptyWhenIsNull()
        {
            GenerateTokenParameters generateTokenParameters = new GenerateTokenParameters();
            generateTokenParameters.SuiteToken = null;
            string result = generateTokenParameters.GetSuiteToken();
            Assert.IsEmpty(result);
        }

        [Test]
        public void EnsureThatGetSuiteTokenReturnsNotEmptyWhenIsNotNull()
        {
            GenerateTokenParameters generateTokenParameters = new GenerateTokenParameters();
            generateTokenParameters.SuiteToken = "asdsadsa-dsa-dsa-dsad-asd";
            string result = generateTokenParameters.GetSuiteToken();
            Assert.IsNotEmpty(result);
            Assert.AreEqual("asdsadsa-dsa-dsa-dsad-asd", result);
        }
    }
}
