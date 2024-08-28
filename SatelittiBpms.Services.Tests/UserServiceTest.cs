using AutoMapper;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class UserServiceTest
    {
        Mock<IUserRepository> _mockRepository;
        Mock<IRoleUserService> _mockRoleUserService;
        Mock<IMapper> _mockMapper;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<ISuiteUserService> _mockSuiteUserService;

        [SetUp]
        public void Init()
        {
            _mockRepository = new Mock<IUserRepository>();
            _mockRoleUserService = new Mock<IRoleUserService>();
            _mockMapper = new Mock<IMapper>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockSuiteUserService = new Mock<ISuiteUserService>();

        }

        [Test]
        public async Task ensureThatGetReturnSuccess()
        {
            int userId = 1, tenantId = 55;
            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(x => x == userId), It.Is<long>(x => x == tenantId))).ReturnsAsync(new UserInfo() { Id = userId, TenantId = tenantId });
            UserService userService = new UserService(_mockRepository.Object, _mockSuiteUserService.Object, _mockMapper.Object, _mockRoleUserService.Object, _mockContextDataService.Object);
            var result = await userService.Get(userId, tenantId);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(result.Value);
            _mockRepository.Verify(x => x.GetByIdAndTenantId(It.Is<int>(x => x == userId), It.Is<long>(x => x == tenantId)), Times.Once());
        }

        [Test]
        public async Task ensureThatDeleteDisableUserWhenIDIsValid()
        {
            int userId = 1;
            var userInfo = new UserInfo() { Id = 1, TenantId = 55, Enable = true, Timezone = -3 };

            _mockRepository.Setup(x => x.Get(userId)).ReturnsAsync(userInfo);
            UserService userService = new UserService(_mockRepository.Object, _mockSuiteUserService.Object, _mockMapper.Object, _mockRoleUserService.Object, _mockContextDataService.Object);

            var result = await userService.Disable(userId);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(x => x.Update(It.IsAny<UserInfo>()), Times.Once());
        }

        [Test]
        public async Task ensureThatDeleteThrowsWhenIDIsNotValid()
        {
            int userId = 2; ;
            var userInfo = new UserInfo() { Id = 1, TenantId = 55, Enable = true, Timezone = -3 };

            _mockRepository.Setup(x => x.Get(1)).ReturnsAsync(userInfo);
            UserService userService = new UserService(_mockRepository.Object, _mockSuiteUserService.Object, _mockMapper.Object, _mockRoleUserService.Object, _mockContextDataService.Object);

            try
            {
                var result = await userService.Disable(userId);
                Assert.Fail();
            }
            catch (System.Exception e)
            {
                Assert.AreEqual(ExceptionCodes.ENTITY_TO_UPDATE_NOT_FOUND, e.Message);
            }

            _mockRepository.Verify(x => x.Update(It.IsAny<UserInfo>()), Times.Never());
        }

        [Test]
        public async Task ensureThatDeleteRangeUpdateUsersWithValidAndInvalidIdsList()
        {
            var userList = new List<int>() { 1, 500, 2 };

            _mockRepository.Setup(x => x.Get(1)).ReturnsAsync(new UserInfo() { Id = 1, TenantId = 55, Enable = true, Timezone = -3 });
            _mockRepository.Setup(x => x.Get(2)).ReturnsAsync(new UserInfo() { Id = 2, TenantId = 55, Enable = true, Timezone = -3 });

            UserService userService = new UserService(_mockRepository.Object, _mockSuiteUserService.Object, _mockMapper.Object, _mockRoleUserService.Object, _mockContextDataService.Object);

            var result = await userService.Disable(userList);

            _mockRepository.Verify(x => x.Update(It.IsAny<UserInfo>()), Times.Exactly(2));
        }

        [Test]
        public async Task ensureThatUpdateUserWhenValidData()
        {
            int userId = 1;
            var userInfo = new UserInfo() { Id = 1, TenantId = 55, Enable = true, Timezone = -3, Type = Models.Enums.BpmsUserTypeEnum.PUBLISHER };
            var userDTO = new UserDTO() { Id = 1, TenantId = 55, Enable = false, Timezone = -2, Type = Models.Enums.BpmsUserTypeEnum.PUBLISHER };

            _mockRepository.Setup(x => x.Get(userId)).ReturnsAsync(userInfo);
            UserService userService = new UserService(_mockRepository.Object, _mockSuiteUserService.Object, _mockMapper.Object, _mockRoleUserService.Object, _mockContextDataService.Object);

            var result = await userService.Update(userId, userDTO);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(x => x.Update(It.IsAny<UserInfo>()), Times.Once());
            _mockRepository.Verify(x => x.Insert(It.IsAny<UserInfo>()), Times.Never());
        }

        [Test]
        public async Task ensureThatInsertWhenUpdateUserThatNotExists()
        {
            int userId = 2;
            var userInfo = new UserInfo() { Id = 1, TenantId = 55, Enable = true, Timezone = -3, Type = Models.Enums.BpmsUserTypeEnum.PUBLISHER };
            var userDTO = new UserDTO() { Id = 1, TenantId = 55, Enable = false, Timezone = -2, Type = Models.Enums.BpmsUserTypeEnum.PUBLISHER };

            _mockRepository.Setup(x => x.Get(1)).ReturnsAsync(userInfo);
            UserService userService = new UserService(_mockRepository.Object, _mockSuiteUserService.Object, _mockMapper.Object, _mockRoleUserService.Object, _mockContextDataService.Object);

            var result = await userService.Update(userId, userDTO);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(x => x.Insert(It.IsAny<UserInfo>()), Times.Once());
            _mockRepository.Verify(x => x.Update(It.IsAny<UserInfo>()), Times.Never());
        }
    }
}
