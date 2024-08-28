using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;
using SatelittiBpms.Models.Result;

namespace SatelittiBpms.Services.Tests
{
    public class TenantServiceTest
    {
        Mock<ITenantRepository> _mockRepository;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<ITenantAuthService> _mockTenantAuthService;
        Mock<ISuiteUserService> _mockSuiteUserService;
        Mock<IUserService> _mockUserService;
        Mock<ITenantService> _mockTenantService;
        Mock<IRoleService> _mockRoleService;

        Mock<IDbContextTransaction> _mockDbContextTransaction;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITenantRepository>();
            _mockTenantAuthService = new Mock<ITenantAuthService>();
            _mockSuiteUserService = new Mock<ISuiteUserService>();
            _mockUserService = new Mock<IUserService>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockTenantService = new Mock<ITenantService>();
            _mockRoleService = new Mock<IRoleService>();

            _mockDbContextTransaction = new Mock<IDbContextTransaction>();
            _mockDbContextTransaction.Setup(x => x.Commit());
            _mockDbContextTransaction.Setup(x => x.Rollback());
        }

        [Test]
        public async Task ensureThatActivationTenantInsertWhenTenantIsAlreadyEntered()
        {
            var subDomain = "subTenantTest";
            var tenantId = 4;

            ActivationTenantDTO activationTenantDTO = new()
            {
                AccessKey = "djia-ewd32-sqw"
            };

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>()
            {
                SubDomain = subDomain,
                Tenant = new SuiteTenantAuth()
                {
                    Id = tenantId,
                    AccessKey = activationTenantDTO.AccessKey
                }
            });

            _mockTenantAuthService.Setup(x => x.GetTenantAuth(It.IsAny<TenantAuthFilter>()))
                .ReturnsAsync(new TenantAuthViewModel
                {
                    AccessKey = activationTenantDTO.AccessKey,
                    Id = tenantId,
                    SubDomain = subDomain,
                    Name = "Selbetti",
                    AuthenticatedByAccessKey = false,
                    Customizable = false,
                    Print_id = false,
                    SystemName = "satelliti",
                    Timezone = -3,
                    Zone = "America/Sao_Paulo"
                });

            _mockTenantService.Setup(x => x.Get(tenantId)).Returns(new TenantInfo { AccessKey = activationTenantDTO.AccessKey, Id = tenantId });

            List<SuiteUserViewModel> listUserViewModel = new()
            {
                new SuiteUserViewModel { Admin = true, Id = 1, Tenant = 4 },
                new SuiteUserViewModel { Admin = false, Id = 2, Tenant = 4 },
                new SuiteUserViewModel { Admin = true, Id = 3, Tenant = 4 }
            };

            _mockSuiteUserService.Setup(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(listUserViewModel);

            _mockTenantService.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            TenantActivateService tenantActivateService = new TenantActivateService(_mockTenantAuthService.Object, _mockSuiteUserService.Object, _mockUserService.Object, _mockTenantService.Object, _mockRoleService.Object, _mockContextDataService.Object);
            var result = await tenantActivateService.ActivationTenant(activationTenantDTO);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(ExceptionCodes.TENANT_ALREADY_INFORMED, result.ValidationResult.Errors[0].ErrorMessage);

            _mockTenantAuthService.Verify(x => x.GetTenantAuth(It.IsAny<TenantAuthFilter>()), Times.Once());
            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());

            _mockSuiteUserService.Verify(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>()), Times.Never());
            _mockRepository.Verify(x => x.Insert(It.IsAny<TenantInfo>()), Times.Never());
            _mockUserService.Verify(x => x.Insert(It.IsAny<UserDTO>()), Times.Never());
        }

        [Test]
        public async Task ensureThatActivationTenantInsertWhenTenantNotInInserted()
        {
            var subDomain = "subTenantTest";
            ActivationTenantDTO activationTenantDTO = new()
            {
                AccessKey = "djia-ewd32-sqw"
            };

            _mockRoleService.Setup(x => x.Insert(It.IsAny<RoleDTO>())).ReturnsAsync(Result.Success(1));

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>()
            {
                SubDomain = subDomain,
                Tenant = new SuiteTenantAuth()
                {
                    Id = 4,
                    AccessKey = activationTenantDTO.AccessKey
                }
            });

            _mockTenantAuthService.Setup(x => x.GetTenantAuth(It.IsAny<TenantAuthFilter>()))
                .ReturnsAsync(new TenantAuthViewModel
                {
                    AccessKey = activationTenantDTO.AccessKey,
                    Id = 4,
                    SubDomain = subDomain,
                    Name = "Selbetti",
                    AuthenticatedByAccessKey = false,
                    Customizable = false,
                    Print_id = false,
                    SystemName = "satelliti",
                    Timezone = -3,
                    Zone = "America/Sao_Paulo"
                });

            List<SuiteUserViewModel> listUserViewModel = new()
            {
                new SuiteUserViewModel { Admin = true, Id = 1, Tenant = 4 },
                new SuiteUserViewModel { Admin = false, Id = 2, Tenant = 4 },
                new SuiteUserViewModel { Admin = true, Id = 3, Tenant = 4 }
            };

            _mockSuiteUserService.Setup(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(listUserViewModel);

            _mockTenantService.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            TenantActivateService tenantActivateService = new TenantActivateService(_mockTenantAuthService.Object, _mockSuiteUserService.Object, _mockUserService.Object, _mockTenantService.Object, _mockRoleService.Object, _mockContextDataService.Object);
            var result = await tenantActivateService.ActivationTenant(activationTenantDTO);
            Assert.IsTrue(result.Success);

            _mockTenantAuthService.Verify(x => x.GetTenantAuth(It.IsAny<TenantAuthFilter>()), Times.Once());
            _mockSuiteUserService.Verify(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>()), Times.Once());
            _mockTenantService.Verify(x => x.Insert(It.IsAny<TenantInfo>()), Times.Once());
            _mockUserService.Verify(x => x.Insert(It.IsAny<UserDTO>()), Times.Exactly(2));
            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
        }

        [Test]
        public async Task ensureThatActivationTenantNotInsertWhenAcessKeyIsDifferentFromTheOneRegistred()
        {
            var subDomain = "subTenantTest";
            ActivationTenantDTO activationTenantDTO = new()
            {
                AccessKey = "djia-ewd32-sqw"
            };

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>()
            {
                SubDomain = subDomain,
                Tenant = new SuiteTenantAuth()
                {
                    Id = 4,
                    AccessKey = activationTenantDTO.AccessKey
                }
            });

            _mockTenantAuthService.Setup(x => x.GetTenantAuth(It.IsAny<TenantAuthFilter>()))
                .ReturnsAsync(new TenantAuthViewModel
                {
                    AccessKey = "sdjfksd-4fdsjfsdkla-12123",
                    Id = 6,
                    SubDomain = subDomain,
                    Name = "Selbetti",
                    AuthenticatedByAccessKey = false,
                    Customizable = false,
                    Print_id = false,
                    SystemName = "satelliti",
                    Timezone = -3,
                    Zone = "America/Sao_Paulo"
                });

            List<SuiteUserViewModel> listUserViewModel = new()
            {
                new SuiteUserViewModel { Admin = true, Id = 1, Tenant = 4 },
                new SuiteUserViewModel { Admin = false, Id = 2, Tenant = 4 },
                new SuiteUserViewModel { Admin = true, Id = 3, Tenant = 4 }
            };

            _mockSuiteUserService.Setup(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(listUserViewModel);

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            TenantActivateService tenantActivateService = new TenantActivateService(_mockTenantAuthService.Object, _mockSuiteUserService.Object, _mockUserService.Object, _mockTenantService.Object, _mockRoleService.Object, _mockContextDataService.Object);
            var result = await tenantActivateService.ActivationTenant(activationTenantDTO);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(ExceptionCodes.ACCESSKEY_DIFFERENT_FROM_INFORMED, result.ValidationResult.Errors[0].ErrorMessage);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockTenantAuthService.Verify(x => x.GetTenantAuth(It.IsAny<TenantAuthFilter>()), Times.Once());
            _mockSuiteUserService.Verify(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>()), Times.Never());
            _mockRepository.Verify(x => x.Insert(It.IsAny<TenantInfo>()), Times.Never());
            _mockUserService.Verify(x => x.Insert(It.IsAny<UserDTO>()), Times.Never());

        }


        [Test]
        public async Task ensureThatActivationTenantNotInsertWhenSubDomainIsDifferentFromTheOneRegistred()
        {
            var subDomain = "subTenantTest";
            ActivationTenantDTO activationTenantDTO = new()
            {
                AccessKey = "djia-ewd32-sqw"
            };

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>()
            {
                SubDomain = subDomain,
                Tenant = new SuiteTenantAuth()
                {
                    Id = 4,
                    AccessKey = activationTenantDTO.AccessKey
                }
            });

            _mockTenantAuthService.Setup(x => x.GetTenantAuth(It.IsAny<TenantAuthFilter>()))
                .ReturnsAsync(new TenantAuthViewModel
                {
                    AccessKey = activationTenantDTO.AccessKey,
                    Id = 6,
                    SubDomain = "OutroSubDomain",
                    Name = "Selbetti",
                    AuthenticatedByAccessKey = false,
                    Customizable = false,
                    Print_id = false,
                    SystemName = "satelliti",
                    Timezone = -3,
                    Zone = "America/Sao_Paulo"
                });

            List<SuiteUserViewModel> listUserViewModel = new()
            {
                new SuiteUserViewModel { Admin = true, Id = 1, Tenant = 4 },
                new SuiteUserViewModel { Admin = false, Id = 2, Tenant = 4 },
                new SuiteUserViewModel { Admin = true, Id = 3, Tenant = 4 }
            };

            _mockSuiteUserService.Setup(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(listUserViewModel);

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            TenantActivateService tenantActivateService = new TenantActivateService(_mockTenantAuthService.Object, _mockSuiteUserService.Object, _mockUserService.Object, _mockTenantService.Object, _mockRoleService.Object, _mockContextDataService.Object);
            var result = await tenantActivateService.ActivationTenant(activationTenantDTO);

            Assert.IsFalse(result.Success);
            Assert.AreEqual(ExceptionCodes.SUBDOMAIN_DIFFERENT_FROM_INFORMED, result.ValidationResult.Errors[0].ErrorMessage);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockTenantAuthService.Verify(x => x.GetTenantAuth(It.IsAny<TenantAuthFilter>()), Times.Once());
            _mockSuiteUserService.Verify(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>()), Times.Never());
            _mockRepository.Verify(x => x.Insert(It.IsAny<TenantInfo>()), Times.Never());
            _mockUserService.Verify(x => x.Insert(It.IsAny<UserDTO>()), Times.Never());

        }
    }
}
