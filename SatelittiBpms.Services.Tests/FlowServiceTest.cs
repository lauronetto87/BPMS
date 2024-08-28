using AutoMapper;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Translate.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class FlowServiceTest
    {
        Mock<IProcessRepository> _mockProcessRepository;
        Mock<IFlowRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<IWorkflowHostService> _workflowHostService;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<ITranslateService> _mockTranslateService;
        Mock<IUserService> _mockUserService;
        Mock<IRoleUserService> _mockRoleUserService;
        Mock<IWildcardService> _mockWildcardService;

        public UserInfo UserMock { get; set; }
        public SuiteTenantAuth TenantMock { get; set; }

        [SetUp]
        public void Init()
        {
            _mockRepository = new Mock<IFlowRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockProcessRepository = new Mock<IProcessRepository>();
            _workflowHostService = new Mock<IWorkflowHostService>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockTranslateService = new Mock<ITranslateService>();
            _mockUserService = new Mock<IUserService>();
            _mockRoleUserService = new Mock<IRoleUserService>();
            _mockWildcardService = new Mock<IWildcardService>();

            UserMock = new UserInfo
            {
                Id = 1,
                Enable = true,
                TenantId = 55,
                Timezone = -3,
                Type = BpmsUserTypeEnum.ADMINISTRATOR
            };

            TenantMock = new SuiteTenantAuth
            {
                Id = 55,
                Language = "pt",
                SubDomain = "tenantSubdomain"
            };

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
        public async Task ensureThatRequest()
        {
            int processId = 10, requesterId = 6, tenant = 55, currentVersion = 1;
            string connectionId = "someGuidId";

            ProcessInfo processInfo = new ProcessInfo();
            processInfo.CurrentVersion = currentVersion;
            processInfo.Id = processId;
            processInfo.TenantId = tenant;

            _mockProcessRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(processInfo);
            _workflowHostService.Setup(x => x.StartFlow(processId, currentVersion, requesterId, connectionId));

            FlowService flowService = new(_mockRepository.Object, _mockMapper.Object, _mockProcessRepository.Object, _mockContextDataService.Object, _workflowHostService.Object, _mockTranslateService.Object, _mockUserService.Object, _mockRoleUserService.Object, _mockWildcardService.Object);
            var result = await flowService.Request(new FlowRequestDTO() { ProcessId = processId, ConnectionId = connectionId });
            Assert.IsTrue(result.Success);

            _workflowHostService.Verify(x => x.StartFlow(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
            _mockProcessRepository.Verify(x => x.Get(It.IsAny<int>()), Times.Once());

        }

        [Test]
        public async Task ensureListAllByTenantWhenNotInformadFiltersReturnsSuccessWhenSkipIsZero()
        {
            var userId = 1;
            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
            };

            var listFlowinfo = new List<FlowInfo>() {
                new FlowInfo()
                {
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "Process name1",
                        Description = "process description1",
                        DescriptionFlow = "process description1",
                        CreatedByUserId = 1,
                        CreatedByUserName = "Created UserName",
                        Status = ProcessStatusEnum.PUBLISHED,
                    },
                    Tasks = new List<TaskInfo>
                    {
                        new TaskInfo(){
                            Id = 4,
                            CreatedDate = DateTime.Now.AddDays(-3),
                            Flow = new FlowInfo{
                                ProcessVersion = new ProcessVersionInfo()
                                {
                                    Name = "Process name1",
                                    Description = "process description1",
                                    DescriptionFlow = "process description1",
                                    CreatedByUserId = 1,
                                    CreatedByUserName = "Created UserName",
                                    Status = ProcessStatusEnum.PUBLISHED,
                                }
                            },
                            Activity = new ActivityInfo
                            {
                                Name = "Tarefa 1",
                                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                                ActivityUser = new ActivityUserInfo()
                                {
                                    ExecutorType = UserTaskExecutorTypeEnum.ROLE
                                }
                            }
                        }
                    }
                }
            };

            var flowViewmodel = new FlowViewModel() { Id = 1 };

            int[] roleIds = new int[] { 1 };

            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };
            var flowviewmodel = listFlowinfo.FirstOrDefault().AsListingViewModel(userViewModel, userId);

            _mockUserService.Setup(x => x.ListUsersSuite()).ReturnsAsync(userViewModel);

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(userId)).ReturnsAsync(new List<int>() { 1 });

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, roleIds, userId)).Returns(listFlowinfo.AsQueryable());

            FlowService flowService = new(_mockRepository.Object, _mockMapper.Object, _mockProcessRepository.Object, _mockContextDataService.Object, _workflowHostService.Object, _mockTranslateService.Object, _mockUserService.Object, _mockRoleUserService.Object, _mockWildcardService.Object);
            var result = await flowService.ListAll(taskFilter);

            Assert.IsTrue(result.Success);

            FlowQueryViewModel tasksQueryViewModel = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.IsNotNull(tasksQueryViewModel);
            Assert.AreEqual(tasksQueryViewModel.List.Count, 1);
            Assert.AreEqual(tasksQueryViewModel.Quantity, 1);
            _mockRepository.Verify(x => x.ListByTenantFilters(taskFilter, roleIds, userId), Times.Once());
        }

        [Test]
        public async Task ensureGetAllTaskGroupByTenantWhenNotInformadFiltersWithTaskGroupTypeIsTaskAndReturnsSuccessWhenSkipIsZero()
        {
            var userId = 1;
            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskGroupType = TaskGroupType.TASK,
            };

            var processVersion1 = new ProcessVersionInfo()
            {
                Id = 1,
                Name = "Process name1",
                Description = "Process description1",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
            };
            var processVersion2 = new ProcessVersionInfo()
            {
                Id = 2,
                Name = "Process name2",
                Description = "Process description2",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
            };

            int[] roleIds = new int[] { 1 };

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, roleIds, userId)).Returns(
                new List<FlowInfo>()
                {
                    new FlowInfo()
                    {
                        Id = 3,
                        Status = FlowStatusEnum.INPROGRESS,
                        ProcessVersion = processVersion1,
                        ProcessVersionId = processVersion1.Id,
                        Tasks = new List<TaskInfo>
                        {
                            new TaskInfo()
                            {
                                Id = 4,
                                CreatedDate = DateTime.Now.AddDays(-3),
                                Flow = new FlowInfo()
                                {
                                    ProcessVersion = processVersion1,
                                    ProcessVersionId = processVersion1.Id,
                                },
                                Activity = new ActivityInfo
                                {
                                    Id = 5,
                                    Name = "Tarefa 1",
                                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                                    ActivityUser = new ActivityUserInfo()
                                    {
                                        ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                                    },
                                },
                            },
                        },
                    },
                    new FlowInfo()
                    {
                        Id = 6,
                        Status = FlowStatusEnum.INPROGRESS,
                        ProcessVersion = processVersion2,
                        ProcessVersionId = processVersion2.Id,
                        Tasks = new List<TaskInfo>
                        {
                            new TaskInfo()
                            {
                                Id = 7,
                                CreatedDate = DateTime.Now.AddDays(-3),
                                Flow = new FlowInfo()
                                {
                                    Id = 8,
                                    ProcessVersion = processVersion2,
                                    ProcessVersionId = processVersion2.Id,
                                },
                                Activity = new ActivityInfo
                                {
                                    Id = 9,
                                    Name = "Tarefa 2",
                                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                                    ActivityUser = new ActivityUserInfo()
                                    {
                                        Id = 10,
                                        ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                                    },
                                },
                            },
                        },
                    },
                    new FlowInfo()
                    {
                        Id = 11,
                        Status = FlowStatusEnum.INPROGRESS,
                        ProcessVersion = processVersion2,
                        ProcessVersionId = processVersion2.Id,
                        Tasks = new List<TaskInfo>
                        {
                            new TaskInfo()
                            {
                                Id = 12,
                                CreatedDate = DateTime.Now.AddDays(-3),
                                Flow = new FlowInfo()
                                {
                                    Id = 13,
                                    ProcessVersion = processVersion2,
                                    ProcessVersionId = processVersion2.Id,
                                },
                                Activity = new ActivityInfo
                                {
                                    Id = 14,
                                    Name = "Tarefa 3",
                                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                                    ActivityUser = new ActivityUserInfo()
                                    {
                                        ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                                    },
                                },
                            },
                        },
                    },
                }.AsQueryable()
            );

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(userId)).ReturnsAsync(new List<int>()
            {
                1
            });

            var wildcardService = new WildcardService(_mockTranslateService.Object);
            FlowService flowService = new(_mockRepository.Object, _mockMapper.Object, _mockProcessRepository.Object, _mockContextDataService.Object, _workflowHostService.Object, _mockTranslateService.Object, _mockUserService.Object, _mockRoleUserService.Object, wildcardService);
            var result = await flowService.GetAllTaskGroup(taskFilter);

            Assert.IsNotNull(result);
            Assert.AreEqual(3, result.Count);

            Assert.AreEqual(5, result[0].Id);
            Assert.AreEqual("Process name1", result[0].Description);
            Assert.AreEqual("Tarefa 1", result[0].Name);

            Assert.AreEqual(9, result[1].Id);
            Assert.AreEqual("Process name2", result[1].Description);
            Assert.AreEqual("Tarefa 2", result[1].Name);

            Assert.AreEqual(14, result[2].Id);
            Assert.AreEqual("Process name2", result[2].Description);
            Assert.AreEqual("Tarefa 3", result[2].Name);

            _mockRepository.Verify(x => x.ListByTenantFilters(taskFilter, roleIds, userId), Times.Once());
        }

        [Test]
        public async Task ensureGetAllTaskGroupByTenantWithFiltersWithTaskGroupTypeIsProcessAndReturnsSuccessWhenSkipIsZero()
        {
            var userId = 1;
            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskGroupType = TaskGroupType.PROCESS,
                IgnoreListId = new List<int> { 50 },
                TotalByQuery = 5,
                OnlyMyRequests = true,
            };

            var processVersion1 = new ProcessVersionInfo()
            {
                Id = 1,
                Name = "Process name1",
                Description = "Process description1",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
            };
            var processVersion2 = new ProcessVersionInfo()
            {
                Id = 2,
                Name = "Process name2",
                Description = "Process description2",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
            };

            int[] roleIds = new int[] { 1 };

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, roleIds, userId)).Returns(
                new List<FlowInfo>()
                {
                    new FlowInfo()
                    {
                        Id = 3,
                        Status = FlowStatusEnum.INPROGRESS,
                        ProcessVersion = processVersion1,
                        ProcessVersionId = processVersion1.Id,
                        RequesterId = 1,
                        Tasks = new List<TaskInfo>
                        {
                            new TaskInfo()
                            {
                                Id = 4,
                                CreatedDate = DateTime.Now.AddDays(-3),
                                Flow = new FlowInfo()
                                {
                                    ProcessVersion = processVersion1,
                                    ProcessVersionId = processVersion1.Id,
                                },
                                Activity = new ActivityInfo
                                {
                                    Id = 5,
                                    Name = "Tarefa 1",
                                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                                    ActivityUser = new ActivityUserInfo()
                                    {
                                        ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                                    },
                                },
                            },
                        },
                    },
                    new FlowInfo()
                    {
                        Id = 6,
                        Status = FlowStatusEnum.INPROGRESS,
                        ProcessVersion = processVersion2,
                        ProcessVersionId = processVersion2.Id,
                        RequesterId = 1,
                        Tasks = new List<TaskInfo>
                        {
                            new TaskInfo()
                            {
                                Id = 7,
                                CreatedDate = DateTime.Now.AddDays(-3),
                                Flow = new FlowInfo()
                                {
                                    Id = 8,
                                    ProcessVersion = processVersion2,
                                    ProcessVersionId = processVersion2.Id,
                                },
                                Activity = new ActivityInfo
                                {
                                    Id = 9,
                                    Name = "Tarefa 2",
                                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                                    ActivityUser = new ActivityUserInfo()
                                    {
                                        Id = 10,
                                        ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                                    },
                                },
                            },
                        },
                    },
                    new FlowInfo()
                    {
                        Id = 11,
                        Status = FlowStatusEnum.INPROGRESS,
                        ProcessVersion = processVersion2,
                        ProcessVersionId = processVersion2.Id,
                        RequesterId = 1,
                        Tasks = new List<TaskInfo>
                        {
                            new TaskInfo()
                            {
                                Id = 12,
                                CreatedDate = DateTime.Now.AddDays(-3),
                                Flow = new FlowInfo()
                                {
                                    Id = 13,
                                    ProcessVersion = processVersion2,
                                    ProcessVersionId = processVersion2.Id,
                                },
                                Activity = new ActivityInfo
                                {
                                    Id = 14,
                                    Name = "Tarefa 3",
                                    Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                                    ActivityUser = new ActivityUserInfo()
                                    {
                                        ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                                    },
                                },
                            },
                        },
                    },
                }.AsQueryable()
            );

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(userId)).ReturnsAsync(new List<int>()
            {
                1
            });

            var wildcardService = new WildcardService(_mockTranslateService.Object);
            FlowService flowService = new(_mockRepository.Object, _mockMapper.Object, _mockProcessRepository.Object, _mockContextDataService.Object, _workflowHostService.Object, _mockTranslateService.Object, _mockUserService.Object, _mockRoleUserService.Object, wildcardService);
            var result = await flowService.GetAllTaskGroup(taskFilter);

            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 2);

            Assert.AreEqual(result[0].Id, 1);
            Assert.IsNull(result[0].Description);
            Assert.AreEqual(result[0].Name, "Process name1");

            Assert.AreEqual(result[1].Id, 2);
            Assert.IsNull(result[1].Description);
            Assert.AreEqual(result[1].Name, "Process name2");

            _mockRepository.Verify(x => x.ListByTenantFilters(taskFilter, roleIds, userId), Times.Once());
        }

        [Test]
        public void ensureThatContainsFlowForProcessVersion()
        {
            int processVersionId = 10;

            _mockRepository.Setup(x => x.GetByTenant(It.IsAny<long>())).Returns(Enumerable.Empty<FlowInfo>().AsQueryable());

            FlowService flowService = new(_mockRepository.Object, _mockMapper.Object, _mockProcessRepository.Object, _mockContextDataService.Object, _workflowHostService.Object, _mockTranslateService.Object, _mockUserService.Object, _mockRoleUserService.Object, _mockWildcardService.Object);
            Assert.IsFalse(flowService.ContainsFlowForProcessVersion(processVersionId));

            _mockRepository.Verify(x => x.GetByTenant(It.Is<long>(p => p != processVersionId)), Times.Once());
        }
    }
}