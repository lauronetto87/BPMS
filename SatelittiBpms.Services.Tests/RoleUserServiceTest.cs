using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    class RoleUserServiceTest
    {
        Mock<IRoleUserRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<ITenantService> _mockTenantService;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepository = new Mock<IRoleUserRepository>();
            _mockTenantService = new Mock<ITenantService>();
        }

        [Test]
        public void ensureConstructor()
        {
            RoleUserService activityService = new RoleUserService(_mockRepository.Object, _mockMapper.Object, _mockTenantService.Object);
            Assert.IsNotNull(activityService);
        }

        [Test]
        public async Task ensureThatInsertWhenUserHasNotRegistered()
        {
            int userId = 1;
            int tenantId = 55;

            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { AccessKey = "aaaaa", DefaultRoleId = 1, Id = 55, SubDomain = "bb" });

            RoleUserService roleUserService = new RoleUserService(_mockRepository.Object, _mockMapper.Object, _mockTenantService.Object);

            var result = await roleUserService.InsertUserDefaultRole(tenantId, userId);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(x => x.Insert(It.IsAny<RoleUserInfo>()), Times.Once());
        }

        [Test]
        public async Task ensureThatNotInsertWhenUserAlreadyRegistered()
        {
            int userId = 1;
            int tenantId = 55;

            _mockRepository.Setup(x => x.GetDefaultByUserAndTenant(tenantId, It.IsAny<int>(), userId)).ReturnsAsync(new RoleUserInfo() { Id = 1, TenantId = 55, RoleId = 1, UserId = 1 });
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { AccessKey = "aaaaa", DefaultRoleId = 1, Id = 55, SubDomain = "bb" });

            RoleUserService roleUserService = new RoleUserService(_mockRepository.Object, _mockMapper.Object, _mockTenantService.Object);

            var result = await roleUserService.InsertUserDefaultRole(tenantId, userId);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(x => x.Insert(It.IsAny<RoleUserInfo>()), Times.Never());
        }

        [Test]
        public async Task ensureThatRemoveUser()
        {
            int userId = 1;
            int tenantId = 55;

            _mockRepository.Setup(x => x.GetDefaultByUserAndTenant(tenantId, It.IsAny<int>(), userId)).ReturnsAsync(new RoleUserInfo() { Id = 1, TenantId = 55, RoleId = 1, UserId = 1 });
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { AccessKey = "aaaaa", DefaultRoleId = 1, Id = 55, SubDomain = "bb" });

            RoleUserService roleUserService = new RoleUserService(_mockRepository.Object, _mockMapper.Object, _mockTenantService.Object);

            var result = await roleUserService.RemoveUserDefaultRole(tenantId, userId);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(x => x.Delete(It.IsAny<RoleUserInfo>()), Times.Once());
        }

        [Test]
        public async Task ensureEnsureThatSucessWhenUserHasNotConfigured()
        {
            int userId = 1;
            int tenantId = 55;

            _mockRepository.Setup(x => x.GetDefaultByUserAndTenant(tenantId, It.IsAny<int>(), userId)).ReturnsAsync(() => null);
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { AccessKey = "aaaaa", DefaultRoleId = 1, Id = 55, SubDomain = "bb" });

            RoleUserService roleUserService = new RoleUserService(_mockRepository.Object, _mockMapper.Object, _mockTenantService.Object);

            var result = await roleUserService.RemoveUserDefaultRole(tenantId, userId);

            Assert.IsTrue(result.Success);
            _mockRepository.Verify(x => x.Delete(It.IsAny<RoleUserInfo>()), Times.Never());
        }


        [Test]
        public async Task ensureThatGetRulesIdByUser()
        {
            int userId = 1;
            

            _mockRepository.Setup(x => x.GetQuery(ru => ru.UserId == userId)).Returns(new List<RoleUserInfo>() { 
                new RoleUserInfo()
                {
                    RoleId = 1,
                    UserId = 1,
                    Id =1                    
                },
                new RoleUserInfo()
                {

                    RoleId = 2,
                    UserId = 1,
                    Id = 2
                }           
            }.AsQueryable());

            RoleUserService roleUserService = new(_mockRepository.Object, _mockMapper.Object, _mockTenantService.Object);

            var result = await roleUserService.GetRulesIdByUser(userId);
            
            _mockRepository.Verify(x => x.GetQuery(ru => ru.UserId == userId), Times.Once());
        }
    }
}
