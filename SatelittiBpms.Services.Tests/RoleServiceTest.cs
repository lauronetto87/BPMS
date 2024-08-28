using AutoMapper;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Translate.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class RoleServiceTest
    {
        Mock<IRoleRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<IRoleService> _roleServiceMock;
        Mock<ITranslateService> _translateServiceMock;
        Mock<IRoleUserService> _roleUserServiceMock;
        Mock<ITenantService> _tenantServiceMock;

        public UserInfo UserMock { get; set; }
        public SuiteTenantAuth TenantMock { get; set; }
        public List<RoleInfo> RolesMock { get; set; }


        [SetUp]
        public void Init()
        {

            _mockRepository = new Mock<IRoleRepository>();
            _roleServiceMock = new Mock<IRoleService>();
            _mockMapper = new Mock<IMapper>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _translateServiceMock = new Mock<ITranslateService>();
            _roleUserServiceMock = new Mock<IRoleUserService>();
            _tenantServiceMock = new Mock<ITenantService>();

            UserMock = new UserInfo
            {
                Id = 1,
                Enable = true,
                TenantId = 55,
                Timezone = -3,
                Type = Models.Enums.BpmsUserTypeEnum.ADMINISTRATOR
            };

            TenantMock = new SuiteTenantAuth
            {
                Id = 55,
                Language = "pt",
                SubDomain = "tenantSubdomain"
            };

            RolesMock = new List<RoleInfo>();
            RolesMock.Add(new RoleInfo { Id = 1, Name = "RH", TenantId = TenantMock.Id });
            RolesMock.Add(new RoleInfo { Id = 2, Name = "Financeiro", TenantId = TenantMock.Id });
            RolesMock.Add(new RoleInfo { Id = 3, Name = "TI", TenantId = 5 });

            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = UserMock
                });
        }

        [Test]
        public async Task ensureListByTenantWhenNotInformadFiltersReturnsSuccess()
        {
            _mockRepository.Setup(x => x.ListAsync()).ReturnsAsync(RolesMock);
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.ListByTenant(null);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<List<RoleViewModel>>.GetValue(result));
            Assert.AreEqual(ResultContent<List<RoleViewModel>>.GetValue(result).Count(), 2);
            _mockRepository.Verify(x => x.ListAsync(), Times.Once());
        }

        [Test]
        public async Task ensureListByTenantWhenInformadFiltersReturnsSuccess()
        {
            Dictionary<string, string> pFilters = new Dictionary<string, string>();
            pFilters.Add("Name", "RH");

            _mockRepository.Setup(x => x.ListAsync()).ReturnsAsync(RolesMock);
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.ListByTenant(pFilters);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<List<RoleViewModel>>.GetValue(result));
            Assert.AreEqual(ResultContent<List<RoleViewModel>>.GetValue(result).Count(), 1);
            _mockRepository.Verify(x => x.ListAsync(), Times.Once());
        }

        public async Task ensureListByTenantWhenInformadFiltersTenantReturnsSuccess()
        {
            Dictionary<string, string> pFilters = new Dictionary<string, string>();
            pFilters.Add("TenantId", TenantMock.Id.ToString());

            _mockRepository.Setup(x => x.ListAsync()).ReturnsAsync(RolesMock);
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.ListByTenant(pFilters);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<List<RoleViewModel>>.GetValue(result));
            Assert.AreEqual(ResultContent<List<RoleViewModel>>.GetValue(result).Count(), 2);
            _mockRepository.Verify(x => x.ListAsync(), Times.Once());
        }

        [Test]
        public async Task ensureGetByTenantWhenInformadRoleIdExistingReturnsSuccess()
        {
            int roleId = 1;
            List<RoleUserInfo> lstRoleUserInfo = new List<RoleUserInfo>();
            lstRoleUserInfo.Add(new RoleUserInfo
            {
                Id = 1,
                RoleId = 1,
                TenantId = TenantMock.Id,
                UserId = 1
            });

            RoleInfo roleInfo = new RoleInfo
            {
                Id = 1,
                Name = "RH",
                TenantId = 55,
                RoleUsers = lstRoleUserInfo
            };

            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == roleId), TenantMock.Id)).ReturnsAsync(roleInfo);
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.GetByTenant(roleId);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<RoleDetailViewModel>.GetValue(result));
            Assert.AreEqual(ResultContent<RoleDetailViewModel>.GetValue(result).Id, roleId);
            _mockRepository.Verify(x => x.GetByIdAndTenantId(It.Is<int>(p => p == roleId), TenantMock.Id), Times.Once());
        }

        [Test]
        public async Task ensureGetByTenantWhenInformadRoleIdNotExistingReturnsSuccess()
        {
            int roleId = 3;
            List<RoleUserInfo> lstRoleUserInfo = new List<RoleUserInfo>();
            lstRoleUserInfo.Add(new RoleUserInfo
            {
                Id = 1,
                RoleId = 1,
                TenantId = TenantMock.Id,
                UserId = 1
            });

            RoleInfo roleInfo = new RoleInfo
            {
                Id = 1,
                Name = "RH",
                TenantId = 55,
                RoleUsers = lstRoleUserInfo
            };

            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == roleId), TenantMock.Id)).ReturnsAsync(roleInfo);
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.GetByTenant(roleId);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<RoleDetailViewModel>.GetValue(result));
            Assert.AreNotEqual(ResultContent<RoleDetailViewModel>.GetValue(result).Id, roleId);
            _mockRepository.Verify(x => x.GetByIdAndTenantId(It.Is<int>(p => p == roleId), TenantMock.Id), Times.Once());
        }

        [Test]
        public async Task ensureRoleNameExistsWhenInformadNameIsExistReturnsNameExists()
        {
            _mockRepository.Setup(x => x.Count(It.IsAny<Expression<Func<RoleInfo, bool>>>())).Returns(Task.FromResult(1));
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.VerifyRoleNameExists("RH");
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<bool>.GetValue(result));
            Assert.AreEqual(ResultContent<bool>.GetValue(result), true);
        }

        [Test]
        public async Task ensureRoleNameExistsWhenInformadNameIsNotExistReturnsNameNotExists()
        {
            _mockRepository.Setup(x => x.Count(It.IsAny<Expression<Func<RoleInfo, bool>>>())).Returns(Task.FromResult(0));
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.VerifyRoleNameExists("RH");
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<bool>.GetValue(result));
            Assert.AreEqual(ResultContent<bool>.GetValue(result), false);
        }

        [Test]
        public async Task ensureInsertWithRelationshipWhenInformedAllFieldsReturnsSucess()
        {

            RoleDTO roleDTO = new RoleDTO
            {
                Name = "RH",
                UsersIds = new List<int>() { 1, 2 }
            };

            RoleInfo roleInfo = new RoleInfo
            {
                Id = 10
            };

            _mockRepository.Setup(x => x.BeginTransaction().Commit());
            _mockRepository.Setup(m => m.Insert(It.IsAny<RoleInfo>())).ReturnsAsync(roleInfo.Id);
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.InsertWithRelationship(roleDTO);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<int>.GetValue(result));
            Assert.AreEqual(ResultContent<int>.GetValue(result), roleInfo.Id);

            _mockRepository.Verify(x => x.BeginTransaction().Commit(), Times.Once());
            _roleUserServiceMock.Verify(x => x.Insert(It.IsAny<RoleUserDTO>()), Times.Exactly(2));

        }

        [Test]
        public async Task ensureInsertWithRelationshipWhenNotInformedUserIdsReturnsSucess()
        {
            RoleDTO roleDTO = new RoleDTO
            {
                Name = "RH",
                UsersIds = null
            };

            RoleInfo roleInfo = new RoleInfo
            {
                Id = 10
            };

            _mockRepository.Setup(x => x.BeginTransaction().Commit());
            _mockRepository.Setup(m => m.Insert(It.IsAny<RoleInfo>())).ReturnsAsync(roleInfo.Id);
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.InsertWithRelationship(roleDTO);
            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<int>.GetValue(result));
            Assert.AreEqual(ResultContent<int>.GetValue(result), roleInfo.Id);

            _mockRepository.Verify(x => x.BeginTransaction().Commit(), Times.Once());
            _roleUserServiceMock.Verify(x => x.Insert(It.IsAny<RoleUserDTO>()), Times.Never());

        }


        [Test]
        public async Task ensureThatInsertWithRelationshipThrowWhenNameIsEmpty()
        {
            RoleDTO roleDTO = new RoleDTO
            {
                Name = "",
                UsersIds = null
            };

            RoleInfo roleInfo = new RoleInfo
            {
                Id = 0
            };

            _mockRepository.Setup(x => x.BeginTransaction().Commit());
            _mockRepository.Setup(m => m.Insert(It.IsAny<RoleInfo>())).Throws(new Exception(_translateServiceMock.Object.Localize(ExceptionCodes.INSERT_ROLE_TRANSACTION_ERROR)));
            RoleService roleService = new RoleService(_mockRepository.Object, _mockMapper.Object, _roleUserServiceMock.Object, _translateServiceMock.Object, _tenantServiceMock.Object, _mockContextDataService.Object);
            var result = await roleService.InsertWithRelationship(roleDTO);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.ValidationResult.Errors.Count(), 1);

            _mockRepository.Verify(x => x.BeginTransaction().Rollback(), Times.Once());
            _roleUserServiceMock.Verify(x => x.Insert(It.IsAny<RoleUserDTO>()), Times.Never());
        }
    }
}