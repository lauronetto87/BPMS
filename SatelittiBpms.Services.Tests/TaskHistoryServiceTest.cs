using AutoMapper;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class TaskHistoryServiceTest
    {
        Mock<ITaskHistoryRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<ITaskRepository> _mockTaskRepository;
        public SuiteTenantAuth TenantMock { get; set; }

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<ITaskHistoryRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockTaskRepository = new Mock<ITaskRepository>();

            TenantMock = new SuiteTenantAuth
            {
                Id = 55,
                Language = "pt",
                SubDomain = "tenantSubdomain"
            };

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo> { Tenant = new SuiteTenantAuth() { Id = 32 } });
        }

        [Test]
        public async Task ensureThatInsertTaskHistory()
        {
            int executorId = 6, taskId = 4;
            TaskHistoryService taskHistoryService = new TaskHistoryService(_mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            _mockRepository.Setup(x => x.Insert(It.IsAny<TaskHistoryInfo>()));

            var result = await taskHistoryService.Insert(taskId, executorId);

            _mockRepository.Verify(x => x.Insert(It.Is<TaskHistoryInfo>(x => x.TenantId.Equals(32) && x.ExecutorId.Equals(executorId) && x.TaskId.Equals(taskId) && !x.EndDate.HasValue)), Times.Once());
        }

        [Test]
        public async Task ensureThatUpdateEndDateWhenHasTaskHistory()
        {
            int taskId = 1;

            _mockRepository.Setup(x => x.GetLastByTask(It.IsAny<int>(), It.IsAny<int>(), null)).ReturnsAsync(new TaskHistoryInfo());

            TaskHistoryService taskHistoryService = new TaskHistoryService(_mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);

            await taskHistoryService.UpdateEndDateFromTaskHistories(taskId, TenantMock.Id);

            _mockRepository.Verify(x => x.Update(It.IsAny<TaskHistoryInfo>()), Times.Once());
        }
        [Test]
        public async Task ensureThatNotUpdateEndDateWhenHasNotTaskHistory()
        {
            int taskId = 1;

            _mockRepository.Setup(x => x.GetLastByTask(It.IsAny<int>(), It.IsAny<int>(), null)).Returns(Task.FromResult<TaskHistoryInfo>(null));

            TaskHistoryService taskHistoryService = new TaskHistoryService(_mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);

            await taskHistoryService.UpdateEndDateFromTaskHistories(taskId, TenantMock.Id);

            _mockRepository.Verify(x => x.Update(It.IsAny<TaskHistoryInfo>()), Times.Never());
        }
    }
}
