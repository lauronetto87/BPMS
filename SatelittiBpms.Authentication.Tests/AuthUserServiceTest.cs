using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Authorization;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Model;
using Satelitti.Authentication.Result;
using Satelitti.Authentication.Service.Interface;
using Satelitti.Options;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Authentication.Services;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Authentication.Tests
{
    public class AuthUserServiceTest
    {
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<ITokenService<GenerateTokenParameters>> _mockTokenService;
        Mock<IUserService> _mockUserService;
        Mock<IOptions<SuiteAuthenticationOptions>> _mockAuthenticationOptions;

        [SetUp]
        public void init()
        {
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockTokenService = new Mock<ITokenService<GenerateTokenParameters>>();
            _mockUserService = new Mock<IUserService>();
            _mockAuthenticationOptions = new Mock<IOptions<SuiteAuthenticationOptions>>();

            _mockAuthenticationOptions.SetupGet(x => x.Value).Returns(new SuiteAuthenticationOptions() { TokenLifetimeInMinutes = 30 });
        }

        [Test]
        public async Task ensureThatResolveProductUserReturnsTokenWhenSuiteUserIsAdmin()
        {
            SuiteUserAuth suiteUser = new SuiteUserAuth()
            {
                Admin = true,
                Id = 1,
                Tenant = 55,
                Timezone = -2,
                Name = "anyName",
                Mail = "other_email@teste.com"
            };
            AuthToken authToken = new AuthToken()
            {
                Token = "some_tokem",
                ExpiresIn = 10
            };

            AuthUserService authUserService = new AuthUserService(_mockTokenService.Object, _mockUserService.Object, _mockAuthenticationOptions.Object, _mockContextDataService.Object);
            _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<GenerateTokenParameters>())).Returns(authToken);
            _mockContextDataService.Setup(x => x.SetUser(It.IsAny<UserInfo>()));
            _mockUserService.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(() => null);

            var result = await authUserService.ResolveProductUser(suiteUser);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(suiteUser.Id, result.Value.User.Id);
            Assert.AreEqual(suiteUser.Mail, result.Value.User.Mail);
            Assert.AreEqual(suiteUser.Name, result.Value.User.Name);
            Assert.AreEqual(suiteUser.Timezone, result.Value.User.Timezone);
            Assert.AreEqual(BpmsUserTypeEnum.ADMINISTRATOR, result.Value.User.Type);
            Assert.AreEqual(authToken.ExpiresIn, result.Value.ExpiresIn);
            Assert.AreEqual(authToken.Token, result.Value.Token);

            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<GenerateTokenParameters>()), Times.Once());
            _mockContextDataService.Verify(x => x.SetUser(It.IsAny<UserInfo>()), Times.Once());
            _mockUserService.Verify(x => x.Get(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatResolveProductUserReturnsTokenWhenSuiteUserIsNotAdmin()
        {
            SuiteUserAuth suiteUser = new SuiteUserAuth()
            {
                Admin = false,
                Id = 1,
                Tenant = 55,
                Timezone = -2,
                Name = "anyName",
                Mail = "other_email@teste.com"
            };
            AuthToken authToken = new AuthToken()
            {
                Token = "some_tokem",
                ExpiresIn = 10
            };
            UserInfo bpmsUserInfo = new UserInfo()
            {
                Type = BpmsUserTypeEnum.PUBLISHER,
                Timezone = -5,
                Enable = true
            };

            AuthUserService authUserService = new AuthUserService(_mockTokenService.Object, _mockUserService.Object, _mockAuthenticationOptions.Object, _mockContextDataService.Object);
            _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<GenerateTokenParameters>())).Returns(authToken);
            _mockContextDataService.Setup(x => x.SetUser(It.IsAny<UserInfo>()));
            _mockUserService.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(SatelittiBpms.Models.Result.Result.Success(bpmsUserInfo));

            var result = await authUserService.ResolveProductUser(suiteUser);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(suiteUser.Id, result.Value.User.Id);
            Assert.AreEqual(suiteUser.Mail, result.Value.User.Mail);
            Assert.AreEqual(suiteUser.Name, result.Value.User.Name);
            Assert.AreEqual(suiteUser.Timezone, result.Value.User.Timezone);
            Assert.AreEqual(bpmsUserInfo.Type, result.Value.User.Type);
            Assert.AreEqual(authToken.ExpiresIn, result.Value.ExpiresIn);
            Assert.AreEqual(authToken.Token, result.Value.Token);

            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<GenerateTokenParameters>()), Times.Once());
            _mockContextDataService.Verify(x => x.SetUser(It.IsAny<UserInfo>()), Times.Once());
            _mockUserService.Verify(x => x.Get(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatResolveProductUserReturnsErrorWhenSuiteUserIsNotAdminAndBpmsUserNotEnable()
        {
            SuiteUserAuth suiteUser = new SuiteUserAuth() { Admin = false };
            UserInfo bpmsUserInfo = new UserInfo()
            {
                Type = BpmsUserTypeEnum.PUBLISHER,
                Timezone = -5,
                Enable = false
            };

            AuthUserService authUserService = new AuthUserService(_mockTokenService.Object, _mockUserService.Object, _mockAuthenticationOptions.Object, _mockContextDataService.Object);
            _mockTokenService.Setup(x => x.GenerateToken(It.IsAny<GenerateTokenParameters>())).Returns(() => null);
            _mockContextDataService.Setup(x => x.SetUser(It.IsAny<UserInfo>()));
            _mockUserService.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(SatelittiBpms.Models.Result.Result.Success(bpmsUserInfo));

            var result = await authUserService.ResolveProductUser(suiteUser);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ResultErrors.ERROR_AUTHENTICATION_INVALIDUSERORINACTIVE, result.ErrorId);

            _mockTokenService.Verify(x => x.GenerateToken(It.IsAny<GenerateTokenParameters>()), Times.Never());
            _mockContextDataService.Verify(x => x.SetUser(It.IsAny<UserInfo>()), Times.Never());
            _mockUserService.Verify(x => x.Get(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }
    }
}
