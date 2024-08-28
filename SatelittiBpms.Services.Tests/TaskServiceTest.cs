using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using Newtonsoft.Json.Linq;
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
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Translate.Interfaces;
using SatelittiBpms.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class TaskServiceTest
    {
        Mock<ITaskRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<ITaskHistoryService> _mockTaskHistoryService;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<IDbContextTransaction> _mockDbContextTransaction;
        Mock<IWorkflowHostService> _mockWorkflowHostService;
        Mock<IFieldValueService> _mockFieldValueService;
        Mock<IFieldValueFileService> _mockFieldValueFileService;
        Mock<IFlowService> _mockFlowService;
        Mock<IUserService> _mockUserService;
        Mock<IRoleUserService> _mockRoleUserService;
        Mock<IWildcardService> _mockWildcardService;
        Mock<IFlowPathService> _mockFlowPathService;
        IWildcardService _wildcardService;

        public UserInfo UserMock { get; set; }
        public SuiteTenantAuth TenantMock { get; set; }

        [SetUp]
        public void Init()
        {

            _mockRepository = new Mock<ITaskRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockTaskHistoryService = new Mock<ITaskHistoryService>();
            _mockWorkflowHostService = new Mock<IWorkflowHostService>();
            _mockFieldValueService = new Mock<IFieldValueService>();
            _mockWildcardService = new Mock<IWildcardService>();
            _mockFlowPathService = new Mock<IFlowPathService>();
            _wildcardService = new WildcardService(new Mock<ITranslateService>().Object);

            _mockUserService = new Mock<IUserService>();
            _mockFlowService = new Mock<IFlowService>();
            _mockRoleUserService = new Mock<IRoleUserService>();
            _mockFieldValueFileService = new Mock<IFieldValueFileService>();

            _mockDbContextTransaction = new Mock<IDbContextTransaction>();
            _mockDbContextTransaction.Setup(x => x.Commit());
            _mockDbContextTransaction.Setup(x => x.Rollback());

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
        public async Task ensureListTasksWhenTaskQueryTypeIsMyTaskAndNotInformadFiltersReturnsSuccessWhenSkipIsZero()
        {
            var userId = 0;
            UserMock.Id = userId;
            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = UserMock
                });
            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskQueryType = TaskQueryType.MYTASKS
            };

            var taskList = new List<TaskInfo>() {
                new TaskInfo()
                {
                    Id = 4,
                    ActivityId = 1,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 1,
                        Name = "Atividade 1",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.ROLE
                        }
                    }
                },
                new TaskInfo()
                {
                    Id = 5,
                    ActivityId = 1,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 2,
                        Name = "Atividade 2",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                        }
                    }
                }
            };

            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, userId)).Returns(taskList.AsQueryable());

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.ListTasks(taskFilter);

            FlowQueryViewModel tasksQueryViewModel = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.IsNotNull(tasksQueryViewModel);
            Assert.AreEqual(tasksQueryViewModel.List.Count, 2);
            Assert.AreEqual(tasksQueryViewModel.Quantity, 2);
            _mockRepository.Verify(x => x.ListByTenantFilters(taskFilter, userId), Times.Once());
        }

        [Test]
        public async Task ensureListTasksWhenTaskQueryTypeIsMyTaskAndNotInformadFiltersReturnsSuccessWhenSkipIsOne()
        {
            var userId = 0;
            UserMock.Id = userId;
            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = UserMock
                });
            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskQueryType = TaskQueryType.MYTASKS
            };


            var taskList = new List<TaskInfo>() {
                new TaskInfo()
                {
                    Id = 4,
                    ActivityId = 1,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 1,
                        Name = "Atividade 1",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.ROLE
                        }
                    }
                },
                new TaskInfo()
                {
                    Id = 5,
                    ActivityId = 1,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 2,
                        Name = "Atividade 2",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                        }
                    }
                }
            };

            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, userId)).Returns(taskList.AsQueryable());

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.ListTasks(taskFilter);

            Assert.IsTrue(result.Success);

            FlowQueryViewModel tasksQueryViewModel = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.IsNotNull(tasksQueryViewModel);
            Assert.AreEqual(tasksQueryViewModel.List.Count, 2);
            Assert.AreEqual(tasksQueryViewModel.Quantity, 2);
        }

        [Test]
        public async Task ensureListTasksWhenTaskQueryTypeIsAllAndNotInformadFiltersReturnsSuccessWhenSkipIsZero()
        {
            var userId = 0;
            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskQueryType = TaskQueryType.ALL
            };

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, userId)).Returns(new List<TaskInfo>() {
                new TaskInfo(){
                    Id = 4,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                Activity = new ActivityInfo()
                {
                    ActivityUser = new ActivityUserInfo()
                    {
                        ExecutorType = UserTaskExecutorTypeEnum.ROLE
                    }
                }
                },
                new TaskInfo(){
                Id = 5,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                Activity = new ActivityInfo()
                {
                    ActivityUser = new ActivityUserInfo()
                    {
                        ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                    }
                }
                }
            }.AsQueryable());

            FlowQueryViewModel flowQueryViewModel = new()
            {
                Quantity = 3,
                List = new List<FlowViewModel>()
                {
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 1",
                        Finished = true,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    },
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 2",
                        Finished = false,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    },
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 3",
                        Finished = false,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    }
                }

            };

            _mockFlowService.Setup(x => x.ListAll(taskFilter)).ReturnsAsync(new ResultContent<FlowQueryViewModel>(flowQueryViewModel, true, null));

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.ListTasks(taskFilter);

            Assert.IsTrue(result.Success);

            FlowQueryViewModel tasksQueryViewModel = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.IsNotNull(tasksQueryViewModel);
            Assert.AreEqual(tasksQueryViewModel.List.Count, 3);
            Assert.AreEqual(tasksQueryViewModel.Quantity, 3);
            _mockFlowService.Verify(x => x.ListAll(taskFilter), Times.Once());
        }

        [Test]
        public async Task ensureListTasksWhenTaskQueryTypeIsAllAndNotInformadFiltersReturnsSuccessWhenSkipIsOne()
        {
            var userId = 0;
            UserMock.Id = userId;
            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = UserMock
                });
            UserMock.Id = userId;
            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = UserMock
                });
            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskQueryType = TaskQueryType.MYTASKS
            };

            var taskList = new List<TaskInfo>() {
                new TaskInfo()
                {
                    Id = 4,
                    ActivityId = 1,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 1,
                        Name = "Atividade 1",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.ROLE
                        }
                    }
                },
                new TaskInfo()
                {
                    Id = 5,
                    ActivityId = 1,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 2,
                        Name = "Atividade 2",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                        }
                    }
                }
            };

            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, userId)).Returns(taskList.AsQueryable());

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.ListTasks(taskFilter);

            Assert.IsTrue(result.Success);

            FlowQueryViewModel tasksQueryViewModel = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.IsNotNull(tasksQueryViewModel);
            Assert.AreEqual(tasksQueryViewModel.List.Count, 2);
            Assert.AreEqual(tasksQueryViewModel.Quantity, 2);
        }

        [Test]
        public async Task ensureListTasksWhenTaskQueryTypeIsMyTaskAndInformadGroupIdFiltersReturnsSuccess()
        {
            var userId = 0;
            UserMock.Id = userId;
            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = UserMock
                });
            TaskFilterDTO taskFilter = new()
            {
                TaskQueryType = TaskQueryType.MYTASKS,
                TaskGroupType = TaskGroupType.TASK,
                GroupId = new List<int>() { 1 }
            };

            var taskList = new List<TaskInfo>() {
                new TaskInfo()
                {
                    Id = 4,
                    ActivityId = 1,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 1,
                        Name = "Atividade 1",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.ROLE
                        }
                    }
                },
                new TaskInfo()
                {
                    Id = 5,
                    ActivityId = 1,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo(){
                        ProcessVersion = new ProcessVersionInfo(){
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 2,
                        Name = "Atividade 2",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                        }
                    }
                }
            };

            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, userId)).Returns(taskList.AsQueryable());

            FlowQueryViewModel flowQueryViewModel = new()
            {
                Quantity = 3,
                List = new List<FlowViewModel>()
                {
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 1",
                        Finished = true,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    },
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 2",
                        Finished = false,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    },
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 3",
                        Finished = false,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    }
                }

            };

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.ListTasks(taskFilter);

            Assert.IsTrue(result.Success);

            FlowQueryViewModel flowViewModel = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.IsNotNull(flowViewModel);

            Assert.AreEqual(flowViewModel.Quantity, 2);
            Assert.AreEqual(flowViewModel.List.Count, 2);

            Assert.AreEqual(flowViewModel.List[0].Id, 4);
            Assert.AreEqual(flowViewModel.List[0].Description, "process description1");
            Assert.AreEqual(flowViewModel.List[1].Name, "Process name1");
            Assert.AreEqual(flowViewModel.List[0].Finished, false);

            Assert.AreEqual(flowViewModel.List[1].Id, 5);
            Assert.AreEqual(flowViewModel.List[1].Description, "process description1");
            Assert.AreEqual(flowViewModel.List[1].Name, "Process name1");
            Assert.AreEqual(flowViewModel.List[1].Finished, false);

            _mockRepository.Verify(x => x.ListByTenantFilters(taskFilter, userId), Times.Once());
        }

        [Test]
        public async Task ensureListTasksWhenTaskQueryTypeIsAllAndInformadGroupIdFiltersReturnsSuccess()
        {
            TaskFilterDTO taskFilter = new()
            {
                TaskQueryType = TaskQueryType.ALL,
                TaskGroupType = TaskGroupType.TASK,
                GroupId = new List<int>() { 1 }
            };

            FlowQueryViewModel flowQueryViewModel = new()
            {
                Quantity = 3,
                List = new List<FlowViewModel>()
                {
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 1",
                        Finished = true,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    },
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 2",
                        Finished = false,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    },
                    new FlowViewModel
                    {
                        ActivityId = 1,
                        CreatedByUserId = 1,
                        Description = "Atividade 3",
                        Finished = false,
                        ProcessStatus = ProcessStatusEnum.PUBLISHED
                    }
                }

            };

            _mockFlowService.Setup(x => x.ListAll(taskFilter)).ReturnsAsync(new ResultContent<FlowQueryViewModel>(flowQueryViewModel, true, null));

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.ListTasks(taskFilter);

            Assert.IsTrue(result.Success);

            FlowQueryViewModel flowViewModel = ResultContent<FlowQueryViewModel>.GetValue(result);
            Assert.IsNotNull(flowViewModel);

            Assert.AreEqual(flowViewModel.Quantity, 3);
            Assert.AreEqual(flowViewModel.List.Count, 3);

            Assert.AreEqual(flowViewModel.List[0].ActivityId, 1);
            Assert.AreEqual(flowViewModel.List[0].Description, "Atividade 1");
            Assert.AreEqual(flowViewModel.List[0].Finished, true);

            Assert.AreEqual(flowViewModel.List[1].ActivityId, 1);
            Assert.AreEqual(flowViewModel.List[1].Description, "Atividade 2");
            Assert.AreEqual(flowViewModel.List[1].Finished, false);

            Assert.AreEqual(flowViewModel.List[2].ActivityId, 1);
            Assert.AreEqual(flowViewModel.List[2].Description, "Atividade 3");
            Assert.AreEqual(flowViewModel.List[2].Finished, false);

            _mockFlowService.Verify(x => x.ListAll(taskFilter), Times.Once());
        }

        [Test]
        public async Task ensureListTasksGroupByTaskWhenUsingFiltersTaskGroupTypeIsTaskTaskReturnsSuccessWhenSkipIsZero()
        {
            var userId = 0;
            UserMock.Id = userId;
            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = UserMock
                });

            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskGroupType = TaskGroupType.TASK
            };

            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, userId)).Returns(new List<TaskInfo>()
            {
                new TaskInfo()
                {
                    Id = 4,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    Flow = new FlowInfo()
                    {
                        ProcessVersion = new ProcessVersionInfo()
                        {
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED,
                        },
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 1,
                        Name = "Atividade 1",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                        },
                    },
                },
                new TaskInfo()
                {
                    Id = 5,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo()
                    {
                        ProcessVersion = new ProcessVersionInfo()
                        {
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED,
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 2,
                        Name = "Atividade 2",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.REQUESTER,
                        },
                    },
                },
                new TaskInfo()
                {
                    Id = 6,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo()
                    {
                        ProcessVersion = new ProcessVersionInfo()
                        {
                            Name = "Process name1",
                            Description = "process description1",
                            CreatedByUserId = 1,
                            CreatedByUserName = "Created UserName",
                            Status = ProcessStatusEnum.PUBLISHED,
                        }
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 2,
                        Name = "Atividade 2",
                        ActivityUser = new ActivityUserInfo()
                        {
                            ExecutorType = UserTaskExecutorTypeEnum.REQUESTER,
                        },
                    },
                },
            }.AsQueryable());
            TaskService taskService = InstantiateTaskService(_wildcardService);
            var result = await taskService.ListTasksGroup(taskFilter);

            Assert.IsTrue(result.Success);

            IList<FlowGroupViewModel> lstflowGroupViewModel = ResultContent<List<FlowGroupViewModel>>.GetValue(result);
            Assert.IsNotNull(lstflowGroupViewModel);
            Assert.AreEqual(lstflowGroupViewModel.Count, 2);

            Assert.AreEqual(lstflowGroupViewModel[0].Description, "Process name1");
            Assert.AreEqual(lstflowGroupViewModel[0].Id, 1);
            Assert.AreEqual(lstflowGroupViewModel[0].Name, "Atividade 1");

            Assert.AreEqual(lstflowGroupViewModel[1].Description, "Process name1");
            Assert.AreEqual(lstflowGroupViewModel[1].Id, 2);
            Assert.AreEqual(lstflowGroupViewModel[1].Name, "Atividade 2");

            _mockRepository.Verify(x => x.ListByTenantFilters(taskFilter, userId), Times.Once());
        }

        [Test]
        public async Task ensureThatAssign()
        {
            int taskId = 12;
            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new TaskInfo());
            _mockRepository.Setup(x => x.Update(It.IsAny<TaskInfo>()));
            _mockTaskHistoryService.Setup(x => x.Insert(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(59);

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.Assign(taskId);
            Assert.IsTrue(result.Success);
            Assert.AreEqual(59, ResultContent<int>.GetValue(result));

            _mockRepository.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Once());
            _mockTaskHistoryService.Verify(x => x.Insert(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatAssignThrowsWhenTaskNotExists()
        {
            int taskId = 12;
            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(() => null);
            _mockRepository.Setup(x => x.Update(It.IsAny<TaskInfo>()));
            _mockTaskHistoryService.Setup(x => x.Insert(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(59);

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.Assign(taskId);

            Assert.IsFalse(result.Success);
            Assert.IsTrue(result.ValidationResult.Errors.Any(x => x.PropertyName.Equals("exceptions.task.updateTaskTransactionError")));

            _mockRepository.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Never());
            _mockTaskHistoryService.Verify(x => x.Insert(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatUnassign()
        {
            int taskId = 12;
            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.IsAny<int>(), It.IsAny<long>())).ReturnsAsync(new TaskInfo());
            _mockRepository.Setup(x => x.Update(It.IsAny<TaskInfo>()));
            _mockTaskHistoryService.Setup(x => x.UnassignHistory(It.IsAny<int>(), It.IsAny<int>()));

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.Unassign(taskId);
            Assert.IsTrue(result.Success);

            _mockRepository.Verify(x => x.GetByIdAndTenantId(It.IsAny<int>(), It.IsAny<long>()), Times.Exactly(2));
            _mockRepository.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Once());
            _mockTaskHistoryService.Verify(x => x.UnassignHistory(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatUnassignThrowsWhenTaskNotExists()
        {
            int taskId = 12;
            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.IsAny<int>(), It.IsAny<long>())).ReturnsAsync(() => null);
            _mockRepository.Setup(x => x.Update(It.IsAny<TaskInfo>()));
            _mockTaskHistoryService.Setup(x => x.UnassignHistory(It.IsAny<int>(), It.IsAny<int>()));

            TaskService taskService = InstantiateTaskService();

            var result = await taskService.Unassign(taskId);
            Assert.IsTrue(result.ValidationResult.Errors.Any(x => x.PropertyName.Equals("exceptions.task.updateTaskTransactionError")));
            Assert.IsFalse(result.Success);

            _mockTaskHistoryService.Verify(x => x.Insert(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _mockRepository.Verify(x => x.GetByIdAndTenantId(It.IsAny<int>(), It.IsAny<long>()), Times.Once);
            _mockRepository.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Never());
        }

        [Test]
        public async Task ensureGetTaskToExecuteWhenInformedTaskIdExistingReturnsSuccess()
        {
            int taskId = 1;

            TaskInfo task = new();
            FlowInfo flow = new();
            ProcessVersionInfo processVersion = new();
            ActivityInfo activity = new();
            ActivityUserInfo activityUser = new();
            List<ActivityUserOptionInfo> activityUserOptionInfoList = new();

            task.Id = 1;
            task.FlowId = 1;
            flow.ProcessVersionId = 1;
            processVersion.FormContent = "";
            processVersion.Name = "Solicitação de férias";
            activity.Name = "Validar informações";
            activity.ActivityFields = new List<ActivityFieldInfo>();

            activityUserOptionInfoList.Add(new ActivityUserOptionInfo() { Id = 1, Description = "Aprovar" });
            activityUserOptionInfoList.Add(new ActivityUserOptionInfo() { Id = 1, Description = "Reprovar" });

            activityUser.ActivityUsersOptions = activityUserOptionInfoList;
            activity.ActivityUser = activityUser;
            task.Activity = activity;
            task.Flow = flow;
            flow.ProcessVersion = processVersion;

            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id)).ReturnsAsync(task);

            TaskService taskService = InstantiateTaskService();

            var result = await taskService.GetTaskToExecute(taskId);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<TaskExecuteViewModel>.GetValue(result));
            Assert.AreEqual(ResultContent<TaskExecuteViewModel>.GetValue(result).TaskId, taskId);
            _mockRepository.Verify(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id), Times.Once());
        }

        [Test]
        public async Task ensureGetTaskToExecuteWhenInformedTaskIdNotExistingReturnsSuccess()
        {
            int taskId = 2;

            TaskInfo task = new();
            FlowInfo flow = new();
            ProcessVersionInfo processVersion = new();
            ActivityInfo activity = new();
            ActivityUserInfo activityUser = new();
            List<ActivityUserOptionInfo> activityUserOptionInfoList = new();

            task.Id = 1;
            task.FlowId = 1;
            flow.ProcessVersionId = 1;
            processVersion.FormContent = "";
            processVersion.Name = "Solicitação de férias";
            activity.Name = "Validar informações";
            activity.ActivityFields = new List<ActivityFieldInfo>();

            activityUserOptionInfoList.Add(new ActivityUserOptionInfo() { Id = 1, Description = "Aprovar" });
            activityUserOptionInfoList.Add(new ActivityUserOptionInfo() { Id = 1, Description = "Reprovar" });

            activityUser.ActivityUsersOptions = activityUserOptionInfoList;
            activity.ActivityUser = activityUser;
            task.Activity = activity;
            task.Flow = flow;
            flow.ProcessVersion = processVersion;

            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id)).ReturnsAsync(task);

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.GetTaskToExecute(taskId);

            Assert.IsTrue(result.Success);
            Assert.IsNotNull(ResultContent<TaskExecuteViewModel>.GetValue(result));
            Assert.AreNotEqual(ResultContent<TaskExecuteViewModel>.GetValue(result).TaskId, taskId);
            _mockRepository.Verify(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id), Times.Once());
        }

        [Test]
        public async Task ensureSuitJsonRuleFieldsUpdateJsonAsNecessary()
        {
            string formContentExpected = System.Text.Encoding.UTF8.GetString(Properties.Resources.formContentExpected);
            string formContent = System.Text.Encoding.UTF8.GetString(Properties.Resources.formContent);

            formContentExpected = JObject.Parse(formContentExpected).ToString();

            int taskId = 1;

            FlowInfo flow = new();
            ActivityInfo activity = new();
            ActivityUserInfo activityUser = new();
            List<ActivityUserOptionInfo> activityUserOptionInfoList = new();
            TaskInfo task = new();
            ProcessVersionInfo processVersion = new();
            List<ActivityFieldInfo> activityFieldInfoList = new();

            processVersion.FormContent = formContent;

            activityFieldInfoList.Add(new ActivityFieldInfo() { Field = new FieldInfo() { ComponentInternalId = "textField" }, State = ProcessTaskFieldStateEnum.EDITABLE });
            activityFieldInfoList.Add(new ActivityFieldInfo() { Field = new FieldInfo() { ComponentInternalId = "textField1" }, State = ProcessTaskFieldStateEnum.ONLYREADING });
            activityFieldInfoList.Add(new ActivityFieldInfo() { Field = new FieldInfo() { ComponentInternalId = "textField2" }, State = ProcessTaskFieldStateEnum.MANDATORY });
            activityFieldInfoList.Add(new ActivityFieldInfo() { Field = new FieldInfo() { ComponentInternalId = "textField3" }, State = ProcessTaskFieldStateEnum.INVISIBLE });

            activityUserOptionInfoList.Add(new ActivityUserOptionInfo() { Id = 1, Description = "Aprovar" });
            activityUserOptionInfoList.Add(new ActivityUserOptionInfo() { Id = 1, Description = "Reprovar" });

            task.Id = 1;

            activityUser.ActivityUsersOptions = activityUserOptionInfoList;
            activity.ActivityUser = activityUser;

            activity.ProcessVersion = processVersion;
            activity.ActivityFields = activityFieldInfoList;
            task.Activity = activity;
            flow.ProcessVersion = processVersion;
            task.Flow = flow;

            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id)).ReturnsAsync(task);

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.GetTaskToExecute(taskId);

            Assert.IsTrue(result.Success);
            Assert.AreEqual(formContentExpected, ResultContent<TaskExecuteViewModel>.GetValue(result).FormContent);
        }

        [Test]
        public async Task ensureSuitJsonRuleFieldsDoNotChangeObjectTaskWhenTaskHasNoFields()
        {
            int taskId = 1;

            FlowInfo flow = new();
            ActivityInfo activity = new();
            ActivityUserInfo activityUser = new();
            List<ActivityUserOptionInfo> activityUserOptionInfoList = new();
            TaskInfo task = new();
            ProcessVersionInfo processVersion = new();
            List<ActivityFieldInfo> activityFieldInfoList = new();

            processVersion.FormContent = null;

            activityUserOptionInfoList.Add(new ActivityUserOptionInfo() { Id = 1, Description = "Aprovar" });
            activityUserOptionInfoList.Add(new ActivityUserOptionInfo() { Id = 1, Description = "Reprovar" });

            task.Id = 1;

            activityUser.ActivityUsersOptions = activityUserOptionInfoList;
            activity.ActivityUser = activityUser;
            activity.ActivityFields = new List<ActivityFieldInfo>();

            activity.ProcessVersion = processVersion;
            task.Activity = activity;
            processVersion.ActivityFields = activityFieldInfoList;
            flow.ProcessVersion = processVersion;
            task.Flow = flow;

            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id)).ReturnsAsync(task);

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.GetTaskToExecute(taskId);

            Assert.IsTrue(result.Success);
            Assert.IsNull(ResultContent<TaskExecuteViewModel>.GetValue(result).FormContent);
        }

        [Test]
        public async Task ensureThatNextTaskWhenInformedOptionAndTaskSuccess()
        {
            var nextStepDTO = new NextStepDTO() { TaskId = 1, FormData = "" };

            _mockFieldValueService.Setup(x => x.UpdateFieldValues(It.IsAny<int>(), It.IsAny<object>()));

            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new TaskInfo());
            TaskService taskService = InstantiateTaskService();
            var result = await taskService.NextTask(nextStepDTO);

            Assert.IsTrue(result.Success);
        }

        [Test]
        public async Task ensureThatGetDetailsByIdYouRequestAndYouNeedToRun()
        {
            int taskId = 12, userid = 1, tenantid = 55;
            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };

            var activity1 = new ActivityInfo
            {
                Name = "Atividade 1",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 1,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = 15,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                }
            };
            var activity2 = new ActivityInfo
            {
                Name = "Atividade 2",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 2,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = 1,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                },
            };

            var task = new TaskInfo()
            {
                Id = 5,
                CreatedDate = DateTime.Now.AddDays(-1),
                Activity = activity1,
                FlowId = 1,
                ExecutorId = userid,
                Flow = new FlowInfo()
                {
                    ProcessVersionId = 1,
                    RequesterId = userid,
                    Tasks = new List<TaskInfo>
                    {
                        new TaskInfo
                        {
                            FinishedDate = DateTime.Now,
                            Activity = activity1,
                        },
                        new TaskInfo
                        {
                            FinishedDate = null,
                            Activity = activity2,
                        }
                    },
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "Process name1",
                        Description = "process description1",
                        CreatedByUserId = 1,
                        CreatedByUserName = "Created UserName",
                        Status = ProcessStatusEnum.PUBLISHED,
                        Activities = new List<ActivityInfo>()
                        {
                            activity1,
                            activity2
                        }
                    }
                },
                FieldsValues = new List<FieldValueInfo>()
            };

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(userid)).ReturnsAsync(new List<int>(new List<int>() { 1 }));
            _mockRepository.Setup(x => x.GetDetailsById(taskId)).Returns(task);

            TaskService taskService = InstantiateTaskService();

            var result = await taskService.GetDetailsById(taskId);

            Assert.IsTrue(result.Success);
            var taskDetailViewModel = ResultContent<TaskDetailsViewModel>.GetValue(result);

            Assert.AreEqual(true, taskDetailViewModel.YouRequest);
            Assert.AreEqual(true, taskDetailViewModel.YouNeedToRun);

            _mockRoleUserService.Verify(x => x.GetRulesIdByUser(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetDetailsById(It.IsAny<int>()), Times.Once());
        }


        [Test]
        public async Task ensureThatGetDetailsByIdYouCanAssociate()
        {
            int taskId = 12, userid = 1, tenantid = 55;

            var activity1 = new ActivityInfo
            {
                Name = "Atividade 1",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 1,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = 12,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                }
            };
            var activity2 = new ActivityInfo
            {
                Name = "Atividade 2",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 2,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = userid,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                },
            };

            var task = new TaskInfo()
            {
                Id = 5,
                CreatedDate = DateTime.Now.AddDays(-1),
                Activity = activity1,
                FlowId = 1,
                ExecutorId = userid,
                Flow = new FlowInfo()
                {
                    ProcessVersionId = 1,
                    RequesterId = userid,
                    Tasks = new List<TaskInfo>
                    {
                        new TaskInfo
                        {
                            FinishedDate = DateTime.Now,
                            Activity = activity1,
                        },
                        new TaskInfo
                        {
                            FinishedDate = null,
                            Activity = activity2,
                        }
                    },
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "Process name1",
                        Description = "process description1",
                        CreatedByUserId = 12,
                        CreatedByUserName = "Created UserName",
                        Status = ProcessStatusEnum.PUBLISHED,
                        Activities = new List<ActivityInfo>()
                        {
                            activity1,
                            activity2
                        }
                    },
                },
                FieldsValues = new List<FieldValueInfo>()
            };

            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };
            var currentUserRoleIds = new List<int>() { 1 };

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(userid)).ReturnsAsync(new List<int>(new List<int>() { 1 }));
            _mockRepository.Setup(x => x.GetDetailsById(taskId)).Returns(task);

            TaskService taskService = InstantiateTaskService();

            var result = await taskService.GetDetailsById(taskId);

            Assert.IsTrue(result.Success);
            var taskDetailViewModel = ResultContent<TaskDetailsViewModel>.GetValue(result);

            Assert.AreEqual(true, taskDetailViewModel.YouCanAssociate);

            _mockRoleUserService.Verify(x => x.GetRulesIdByUser(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetDetailsById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatGetDetailsByIdYouWillRun()
        {
            int taskId = 12, tenantid = 55, createdByUserId = 15, requesterId = 22;
            var currentUserRoleIds = new List<int>() { 1 };

            var activityInfo1 = new ActivityInfo
            {
                Name = "Atividade 1",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 1,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.REQUESTER,
                    RoleId = 1
                },
            };
            var activityInfo2 = new ActivityInfo
            {
                Name = "Atividade 2",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 2,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                    Id = 1,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                },
            };
            var activityInfo3 = new ActivityInfo
            {
                Name = "Atividade 3",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 3,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.PERSON,
                    PersonId = requesterId,
                },
            };

            var task = new TaskInfo()
            {
                Id = 5,
                CreatedDate = DateTime.Now.AddDays(-1),
                Activity = new ActivityInfo
                {
                    Name = "Atividade 1",
                    ActivityUser = new ActivityUserInfo
                    {
                        ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                        RoleId = 1
                    }
                },
                FlowId = 1,
                ExecutorId = requesterId,
                Flow = new FlowInfo()
                {
                    Tasks = new List<TaskInfo>
                    {
                        new TaskInfo
                        {
                            FinishedDate = DateTime.Now,
                            Activity = activityInfo1
                        },
                        new TaskInfo
                        {
                            FinishedDate = null,
                            Activity = activityInfo2,
                        },
                        new TaskInfo
                        {
                            FinishedDate = null,
                            Activity = activityInfo3,
                        }
                    },
                    ProcessVersionId = 1,
                    RequesterId = requesterId,
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "Process name1",
                        Description = "process description1",
                        CreatedByUserId = createdByUserId,
                        CreatedByUserName = "Created UserName",
                        Status = ProcessStatusEnum.PUBLISHED,
                        Activities = new List<ActivityInfo>()
                        {
                            activityInfo1,
                            activityInfo2,
                            activityInfo3,
                        }
                    }
                },
                FieldsValues = new List<FieldValueInfo>()
            };

            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = requesterId, Name = "Name" } };

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(It.IsAny<int>())).ReturnsAsync(currentUserRoleIds);
            _mockRepository.Setup(x => x.GetDetailsById(taskId)).Returns(task);
            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = new UserInfo
                    {
                        Id = requesterId,
                        Enable = true,
                        TenantId = 55,
                        Timezone = -3,
                        Type = BpmsUserTypeEnum.ADMINISTRATOR
                    }
                });

            var result = await InstantiateTaskService().GetDetailsById(taskId);

            Assert.IsTrue(result.Success);
            var taskDetailViewModel = ResultContent<TaskDetailsViewModel>.GetValue(result);

            Assert.AreEqual(true, taskDetailViewModel.YouWillRun);

            _mockRoleUserService.Verify(x => x.GetRulesIdByUser(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetDetailsById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatGetDetailsByIdYourRoleWillTask()
        {
            int taskId = 12, userid = 1, tenantid = 55, createdByUserId = 15;
            var currentUserRoleIds = new List<int>() { 1 };
            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };

            var activity1 = new ActivityInfo
            {
                Name = "Atividade 1",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 1,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                },
            };
            var activity2 = new ActivityInfo
            {
                Name = "Atividade 2",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 2,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                },
            };
            var activity3 = new ActivityInfo
            {
                Name = "Atividade 3",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 3,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1

                },
            };

            var task = new TaskInfo()
            {
                Id = 5,
                CreatedDate = DateTime.Now.AddDays(-1),
                Activity = activity1,
                FlowId = 1,
                ExecutorId = userid,
                Flow = new FlowInfo()
                {
                    ProcessVersionId = 1,
                    RequesterId = createdByUserId,
                    Tasks = new List<TaskInfo>
                    {
                        new TaskInfo
                        {
                            FinishedDate = DateTime.Now,
                            Activity = activity1,
                        },
                        new TaskInfo
                        {
                            FinishedDate = null,
                            Activity = activity2,
                        },
                        new TaskInfo
                        {
                            FinishedDate = null,
                            Activity = activity3,
                        }
                    },
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "Process name1",
                        Description = "process description1",
                        CreatedByUserId = createdByUserId,
                        CreatedByUserName = "Created UserName",
                        Status = ProcessStatusEnum.PUBLISHED,
                        Activities = new List<ActivityInfo>()
                        {
                            activity1,
                            activity2,
                            activity3,
                        }
                    }
                },
                FieldsValues = new List<FieldValueInfo>()
            };

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(userid)).ReturnsAsync(new List<int>(new List<int>() { 1 }));
            _mockRepository.Setup(x => x.GetDetailsById(taskId)).Returns(task);

            TaskService taskService = InstantiateTaskService();

            var result = await taskService.GetDetailsById(taskId);

            Assert.IsTrue(result.Success);
            var taskDetailViewModel = ResultContent<TaskDetailsViewModel>.GetValue(result);

            Assert.AreEqual(true, taskDetailViewModel.YourRoleWillTask);

            _mockRoleUserService.Verify(x => x.GetRulesIdByUser(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetDetailsById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatGetDetailsByIdYouHaveRunTask()
        {
            int taskId = 12, userid = 1, tenantid = 55, createdByUserId = 15, flowid = 1;
            var currentUserRoleIds = new List<int>() { 1 };
            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };

            var activity1 = new ActivityInfo
            {
                Name = "Atividade 1",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 1,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                },
            };
            var activity2 = new ActivityInfo
            {
                Name = "Atividade 2",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 2,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.PERSON,
                    PersonId = userid
                },
            };
            var activity3 = new ActivityInfo
            {
                Name = "Atividade 3",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 3,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                },
            };

            var task = new TaskInfo()
            {
                Id = 5,
                CreatedDate = DateTime.Now.AddDays(-1),
                Activity = activity1,
                FlowId = flowid,
                ExecutorId = userid,
                Flow = new FlowInfo()
                {
                    ProcessVersionId = 1,
                    RequesterId = createdByUserId,
                    Tasks = new List<TaskInfo>
                    {
                        new TaskInfo
                        {
                            FinishedDate = DateTime.Now,
                            FlowId = flowid,
                            Activity = activity1,
                        },
                        new TaskInfo
                        {
                            ExecutorId = userid,
                            FinishedDate = DateTime.Now,
                            FlowId = flowid,
                            Activity = activity2,
                        },
                        new TaskInfo
                        {
                            FinishedDate = null,
                            FlowId = flowid,
                            Activity = activity3,
                        },
                    },
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "Process name1",
                        Description = "process description1",
                        CreatedByUserId = createdByUserId,
                        CreatedByUserName = "Created UserName",
                        Status = ProcessStatusEnum.PUBLISHED,
                        Activities = new List<ActivityInfo>()
                        {
                            activity1,
                            activity2,
                            activity3,
                        },
                    },
                },
                FieldsValues = new List<FieldValueInfo>()
            };

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(userid)).ReturnsAsync(new List<int>(new List<int>() { 1 }));
            _mockRepository.Setup(x => x.GetDetailsById(taskId)).Returns(task);

            TaskService taskService = InstantiateTaskService();

            var result = await taskService.GetDetailsById(taskId);

            Assert.IsTrue(result.Success);
            var taskDetailViewModel = ResultContent<TaskDetailsViewModel>.GetValue(result);

            Assert.AreEqual(true, taskDetailViewModel.YouHaveRunTask);

            _mockRoleUserService.Verify(x => x.GetRulesIdByUser(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetDetailsById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatGetDetailsByIdYourRolePerformedTask()
        {
            int taskId = 12, userid = 1, tenantid = 55, createdByUserId = 15, flowid = 1;
            var currentUserRoleIds = new List<int>() { 1 };
            var userViewModel = new List<SuiteUserViewModel>() { new SuiteUserViewModel() { Id = 1, Name = "Name" } };

            var activity1 = new ActivityInfo
            {
                Name = "Atividade 1",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 1,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                }
            };
            var activity2 = new ActivityInfo
            {
                Name = "Atividade 2",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 2,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1
                },
            };
            var activity3 = new ActivityInfo
            {
                Name = "Atividade 3",
                Type = WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                Id = 3,
                TenantId = tenantid,
                ProcessVersion = new ProcessVersionInfo
                {
                    CreatedByUserId = createdByUserId,
                },
                ActivityUser = new ActivityUserInfo
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                    RoleId = 1

                },
            };

            var task = new TaskInfo()
            {
                Id = 5,
                CreatedDate = DateTime.Now.AddDays(-1),
                Activity = activity1,
                FlowId = flowid,
                ExecutorId = userid,
                Flow = new FlowInfo()
                {
                    ProcessVersionId = 1,
                    RequesterId = createdByUserId,
                    Tasks = new List<TaskInfo>
                    {
                        new TaskInfo
                        {
                            FinishedDate = DateTime.Now,
                            FlowId = flowid,
                            Activity = activity1,
                        },
                        new TaskInfo
                        {
                            ExecutorId = userid,
                            FinishedDate = DateTime.Now,
                            FlowId = flowid,
                            Activity = activity2,
                        },
                        new TaskInfo
                        {
                            FinishedDate = null,
                            FlowId = flowid,
                            Activity = activity3,
                        }
                    },
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "Process name1",
                        Description = "process description1",
                        CreatedByUserId = createdByUserId,
                        CreatedByUserName = "Created UserName",
                        Status = ProcessStatusEnum.PUBLISHED,
                        Activities = new List<ActivityInfo>()
                        {
                            activity1,
                            activity2,
                            activity3,
                        }
                    }
                },
                FieldsValues = new List<FieldValueInfo>()
            };

            _mockRoleUserService.Setup(x => x.GetRulesIdByUser(userid)).ReturnsAsync(new List<int>(new List<int>() { 1 }));
            _mockRepository.Setup(x => x.GetDetailsById(taskId)).Returns(task);

            TaskService taskService = InstantiateTaskService();

            var result = await taskService.GetDetailsById(taskId);

            Assert.IsTrue(result.Success);
            var taskDetailViewModel = ResultContent<TaskDetailsViewModel>.GetValue(result);

            Assert.AreEqual(true, taskDetailViewModel.YourRolePerformedTask);

            _mockRoleUserService.Verify(x => x.GetRulesIdByUser(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetDetailsById(It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureListTasksGroupWhenUsingFiltersTaskGroupTypeIsTaskAndTaskQueryTypeIsAllTaskReturnsSuccessWhenSkipIsZero()
        {
            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskGroupType = TaskGroupType.TASK,
                TaskQueryType = TaskQueryType.ALL,
            };

            TaskService taskService = InstantiateTaskService();
            var result = await taskService.ListTasksGroup(taskFilter);

            Assert.IsTrue(result.Success);

            IList<FlowGroupViewModel> lstflowGroupViewModel = ResultContent<List<FlowGroupViewModel>>.GetValue(result);
            Assert.IsNull(lstflowGroupViewModel);
        }


        [Test]
        public async Task ensureListTasksGroupByProcessWhenUsingFiltersTaskGroupTypeIsTaskTaskReturnsSuccessWhenSkipIsZero()
        {
            var userId = 0;
            UserMock.Id = userId;
            _mockContextDataService
                .Setup(x => x.GetContextData())
                .Returns(new ContextData<UserInfo>
                {
                    SubDomain = "tenantSubdomain",
                    Tenant = TenantMock,
                    User = UserMock
                });

            TaskFilterDTO taskFilter = new()
            {
                Skip = 0,
                Take = 25,
                TaskGroupType = TaskGroupType.PROCESS,
            };

            var processVersion1 = new ProcessVersionInfo()
            {
                Id = 1,
                Name = "Process name1",
                Description = "process description1",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
            };
            var processVersion2 = new ProcessVersionInfo()
            {
                Id = 2,
                Name = "Process name2",
                Description = "process description2",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
            };


            _mockRepository.Setup(x => x.ListByTenantFilters(taskFilter, userId)).Returns(new List<TaskInfo>()
            {
                new TaskInfo()
                {
                    Id = 3,
                    CreatedDate = DateTime.Now.AddDays(-3),
                    Flow = new FlowInfo()
                    {
                        Id = 4,
                        ProcessVersion = processVersion1,
                        ProcessVersionId = processVersion1.Id,
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 5,
                        Name = "Atividade 1",
                        ActivityUser = new ActivityUserInfo()
                        {
                            Id = 6,
                            ExecutorType = UserTaskExecutorTypeEnum.ROLE,
                        },
                        ProcessVersion = processVersion1,
                        ProcessVersionId = processVersion1.Id,
                    },
                },
                new TaskInfo()
                {
                    Id = 7,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo()
                    {
                        Id = 8,
                        ProcessVersion = processVersion2,
                        ProcessVersionId = processVersion2.Id,
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 9,
                        Name = "Atividade 2",
                        ActivityUser = new ActivityUserInfo()
                        {
                            Id = 10,
                            ExecutorType = UserTaskExecutorTypeEnum.REQUESTER,
                        },
                        ProcessVersion = processVersion2,
                        ProcessVersionId = processVersion2.Id,
                    },
                },
                new TaskInfo()
                {
                    Id = 11,
                    CreatedDate = DateTime.Now.AddDays(-1),
                    Flow = new FlowInfo()
                    {
                        Id = 12,
                        ProcessVersion = processVersion2,
                        ProcessVersionId = processVersion2.Id,
                    },
                    Activity = new ActivityInfo()
                    {
                        Id = 13,
                        Name = "Atividade 2",
                        ActivityUser = new ActivityUserInfo()
                        {
                            Id = 14,
                            ExecutorType = UserTaskExecutorTypeEnum.REQUESTER,
                        },
                        ProcessVersion = processVersion2,
                        ProcessVersionId = processVersion2.Id,
                    },
                },
            }.AsQueryable());
            TaskService taskService = InstantiateTaskService(_wildcardService);
            var result = await taskService.ListTasksGroup(taskFilter);

            Assert.IsTrue(result.Success);

            IList<FlowGroupViewModel> lstflowGroupViewModel = ResultContent<List<FlowGroupViewModel>>.GetValue(result);
            Assert.IsNotNull(lstflowGroupViewModel);
            Assert.AreEqual(lstflowGroupViewModel.Count, 2);

            Assert.IsNull(lstflowGroupViewModel[0].Description);
            Assert.AreEqual(lstflowGroupViewModel[0].Id, 1);
            Assert.AreEqual(lstflowGroupViewModel[0].Name, "Process name1");

            Assert.IsNull(lstflowGroupViewModel[1].Description);
            Assert.AreEqual(lstflowGroupViewModel[1].Id, 2);
            Assert.AreEqual(lstflowGroupViewModel[1].Name, "Process name2");

            _mockRepository.Verify(x => x.ListByTenantFilters(taskFilter, userId), Times.Once());
        }


        private TaskService InstantiateTaskService(IWildcardService _wildcardService = null)
        {
            return new(_mockRepository.Object, _mockMapper.Object, _mockTaskHistoryService.Object, _mockWorkflowHostService.Object, _mockFlowService.Object, _mockContextDataService.Object, _mockFieldValueService.Object, _mockFieldValueFileService.Object, _mockRoleUserService.Object, _mockUserService.Object, _wildcardService ?? _mockWildcardService.Object, _mockFlowPathService.Object);
        }
    }
}
