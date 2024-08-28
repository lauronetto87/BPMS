using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Options;
using SatelittiBpms.Data;
using SatelittiBpms.Extensions;
using SatelittiBpms.FluentDataBuilder.Process;
using SatelittiBpms.FluentDataBuilder.Process.Builders.Process;
using SatelittiBpms.Mail.Interfaces;
using SatelittiBpms.Models.Constants;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.VersionNormalization.Extensions;
using SatelittiBpms.VersionNormalization.Interfaces;
using SatelittiBpms.Workflow.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test
{
    public class MockServices
    {
        public string ConnectionId = Guid.NewGuid().ToString();

        public readonly ServiceCollection services = new();

        private ServiceProvider _serviceProvider;
        protected ServiceProvider ServiceProvider
        {
            get
            {
                if (_serviceProvider == null)
                {
                    throw new Exception($"Service provider not built, build with {nameof(BuildServiceProvider)} method.");
                }
                return _serviceProvider;
            }
        }

        public ContextData<UserInfo> ContextData
        {
            get
            {
                if (_contextData != null)
                {
                    return _contextData;
                }
                var suiteOptions = GetService<IOptions<SuiteOptions>>().Value;
                _contextData = new ContextData<UserInfo>()
                {
                    SubDomain = suiteOptions.Mock.Tenant.SubDomain,
                    SuiteToken = suiteOptions.Mock.Tenant.SuiteToken,
                    Tenant = new Satelitti.Authentication.Model.SuiteTenantAuth
                    {
                        Id = suiteOptions.Mock.Tenant.Id,
                        Timezone = suiteOptions.Mock.Tenant.Timezone,
                        // TODO Language e Zone não estão definidos no appsettings.<ALL>.json tratar aqui quando for ajustar isso no backend hoje não funciona
                        //Language = XXX,
                        Name = suiteOptions.Mock.Tenant.Name,
                        SubDomain = suiteOptions.Mock.Tenant.SubDomain,
                        AccessKey = suiteOptions.Mock.Tenant.AccessKey,
                        //Zone = XXX,
                    },
                    AccessKey = new Guid(suiteOptions.Mock.Tenant.AccessKey),
                    User = new UserInfo
                    {
                        Enable = true,
                        Id = suiteOptions.Mock.User.Id,
                        TenantId = suiteOptions.Mock.User.Tenant,
                        Timezone = suiteOptions.Mock.Tenant.Timezone,
                        Type = suiteOptions.Mock.User.Admin ? BpmsUserTypeEnum.ADMINISTRATOR : BpmsUserTypeEnum.PUBLISHER,
                    }
                };
                return _contextData;
            }
        }

        private readonly List<Action<IServiceCollection>> _customizeServicesList = new();
        private ContextData<UserInfo> _contextData;

        public T GetService<T>() where T : class
        {
            return ServiceProvider.GetService<T>() ?? new Mock<T>().Object;
        }

        public async Task BuildServiceProvider(ContextBuilder contextBuilder = null)
        {
            if (IsTheServiceProviderAlreadyBuilt)
            {
                throw new Exception("Service provider has already been built.");
            }
            const string environmentTest = "Test";

            var configuration = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile($"appsettings.{environmentTest}.json").Build();
            ConfigureServices(services, configuration, environmentTest, contextBuilder);
            _serviceProvider = services.BuildServiceProvider();

            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetService<BpmsContext>();

                if (configuration["Provider"] == "SQLite")
                {
                    // Não é possível rodar as migrações para o Sqlite, teria de gerar todas as migrações para o provedor do Sqlite mas,
                    // atráves do objeto da interface IRelationalDatabaseCreator as tabelas foram criadas corretas,
                    // se caso termos que realizar uma migração no Sqlite temos que estudar as opções.
                    //dbContext.Database.Migrate();
                    dbContext.GetService<IRelationalDatabaseCreator>().EnsureCreated();
                }
                else
                {
                    dbContext.Database.Migrate();
                }

                var executeNormalizations = scope.ServiceProvider.GetRequiredService<IExecuteNormalizations>();
                await executeNormalizations.Execute();
            }

            #region UseDependencyConfigurationExtension

            Task.WaitAll(Task.Run(async () => await ServiceProvider.UseVersionNormalization()));
            ServiceProvider.UseWorkflowDependencyConfiguration();

            #endregion UseDependencyConfigurationExtension
        }

        private bool IsTheServiceProviderAlreadyBuilt => _serviceProvider != null;


        public async Task ActivationTenant()
        {
            var tenantActivateService = GetService<ITenantActivateService>();
            var result = await tenantActivateService.ActivationTenant(new ActivationTenantDTO
            {
                AccessKey = ContextData.AccessKey.ToString()
            });
            Assert.IsTrue(result.Success || (result.ValidationResult.Errors.Count == 1 && result.ValidationResult.Errors[0].ErrorMessage == "exceptions.task.tenantAlreadyInformed"));
        }

        public async Task ActivationTenant(ContextBuilder builldContext)
        {
            var _roleRepository = GetService<IRoleRepository>();
            var _tenantRepository = GetService<ITenantRepository>();
            var _userService = GetService<IUserRepository>();

            foreach (var tenant in builldContext.Tenants)
            {
                if (_tenantRepository.Get(tenant.Id) == null)
                {
                    _tenantRepository.Insert(tenant);
                }
            }
            foreach (var role in builldContext.Roles)
            {
                await _roleRepository.Insert(role);
            }
            foreach (var user in builldContext.Users)
            {
                await _userService.Insert(user);
            }

            ContextData.User = faker.PickRandom(builldContext.Users);
        }

        private void ConfigureServices(IServiceCollection services, IConfigurationRoot configuration, string environmentTest, ContextBuilder contextBuilder = null)
        {
            var currentEnvironmentMock = new Mock<IHostEnvironment>();
            currentEnvironmentMock.Setup(p => p.EnvironmentName).Returns(environmentTest);

            var currentEnvironment = currentEnvironmentMock.Object;
            services.AddSingleton(currentEnvironment);


            services.ConfigureOptions(configuration);

            services.AddDbContextPool<BpmsContext>(options =>
            {
                var cpmsConnectionString = configuration.GetConnectionString(ProjectVariableConstants.BpmsConnectionString);
                if (configuration["Provider"] == "SQLite")
                {
                    if (cpmsConnectionString == "Filename=:memory:")
                    {
                        // Se for na memória a conexão do Sqlite não pode fechar, se fechar apagar todos os dados
                        var sqliteConnection = new SqliteConnection(cpmsConnectionString);
                        sqliteConnection.Open();
                        options.UseSqlite(sqliteConnection);
                    }
                    else
                    {
                        options.UseSqlite(cpmsConnectionString);
                    }
                }
                else
                {
                    options.UseMySql(cpmsConnectionString, new MySqlServerVersion(new Version(8, 0, 17)));
                }
            });

            services.AddDependencyInjection(currentEnvironment, configuration);

            var contextDataServiceMock = new Mock<IContextDataService<UserInfo>>();
            contextDataServiceMock.Setup(p => p.GetContextData()).Returns(() => ContextData);
            services.AddSingleton(contextDataServiceMock.Object);

            if (contextBuilder != null)
            {
                var suiteUserServiceMock = new Mock<ISuiteUserService>();
                var usersSuit = contextBuilder.Users.Select((u) => new SuiteUserViewModel
                {
                    Admin = u.Type == BpmsUserTypeEnum.ADMINISTRATOR || u.Type == BpmsUserTypeEnum.PUBLISHER,
                    Id = u.Id,
                    Mail = "test@test.com.br",
                    Name = faker.Person.FullName,
                    SuiteToken = null,
                    Tenant = u.TenantId ?? 0,
                    Timezone = u.Timezone,
                }).ToList();
                suiteUserServiceMock.Setup(p => p.ListWithContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(() => usersSuit);
                suiteUserServiceMock.Setup(p => p.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(() => usersSuit);
                services.AddScoped((_) => suiteUserServiceMock.Object);
            }

            services.AddTransient((_) => new Mock<IMailerService>().Object);

            foreach (var item in _customizeServicesList)
            {
                item(services);
            }
        }

        internal void AddCustomizeServices(Action<IServiceCollection> serviceCustomize)
        {
            _customizeServicesList.Add(serviceCustomize);
        }


        public async Task<SuiteUserViewModel> GetUserSuite(int userId)
        {
            var tenant = GetService<ITenantService>().Get(ContextData.Tenant.Id);

            var listSuiteUserResult = await GetService<ISuiteUserService>().ListWithoutContext(new SuiteUserListFilter()
            {
                TenantSubDomain = tenant.SubDomain,
                TenantAccessKey = tenant.AccessKey,
                InUserIds = new List<int>
                {
                    ContextData.User.Id,
                }
            });
            return listSuiteUserResult?.FirstOrDefault(u => u.Id == userId);
        }

        public ProcessBuilder BeginCreateProcess()
        {
            var task = CreateProcessAsync();
            task.Wait();
            return task.Result;
        }
        public async Task<ProcessBuilder> CreateProcessAsync()
        {
            var contextBuilder = GenerateContextBuilder();

            if (!IsTheServiceProviderAlreadyBuilt)
            {
                await BuildServiceProvider(contextBuilder);
            }

            await ActivationTenant(contextBuilder);

            return new ProcessBuilder(contextBuilder);
        }

        protected Bogus.Faker faker = new();

        public ContextBuilder GenerateContextBuilder()
        {
            const int totalTentats = 1;

            /* 
             TODO Hoje o tentant é fixo no MokService, tem de tratar para ser aleatória

            var tenats = new int[totalTentats].Select(_ =>
               new TenantInfo
               {
                   Id = faker.Random.Int(1),
                   AccessKey = Guid.NewGuid().ToString(),
                   SubDomain = faker.Internet.DomainName(),
               }
           ).ToList();
           */
            var tenats = new List<TenantInfo>
            {
                new TenantInfo
                {
                    Id = 55,
                    AccessKey = Guid.NewGuid().ToString(),
                    SubDomain = faker.Internet.DomainName(),
                }
            };

            var roles = new int[totalTentats * 5].Select(_ =>
                new RoleInfo
                {
                    Id = faker.Random.Int(1),
                    Name = faker.Name.FullName(),
                    TenantId = faker.Random.ArrayElement(tenats.ToArray()).Id,
                }
            ).ToList();

            var users = new int[totalTentats * 10].Select(_ =>
                new UserInfo
                {
                    Id = faker.Random.Int(1),
                    Enable = true,
                    Type = BpmsUserTypeEnum.ADMINISTRATOR,
                    TenantId = faker.Random.ArrayElement(tenats.ToArray()).Id,
                }
            ).ToList();

            foreach (var item in tenats)
            {
                item.DefaultRoleId = faker.Random.ArrayElement(roles.ToArray()).Id;
            }

            return new ContextBuilder
            {
                Roles = roles,
                Tenants = tenats,
                Users = users,
                ExtendData = this,
            };
        }
    }
}
