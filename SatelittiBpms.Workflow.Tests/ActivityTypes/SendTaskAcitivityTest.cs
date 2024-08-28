using Moq;
using NUnit.Framework;
using SatelittiBpms.ApiGatewayManagementApi.Interfaces;
using SatelittiBpms.Mail.Interfaces;
using SatelittiBpms.Mail.Models;
using SatelittiBpms.Mail.Models.Config;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Workflow.ActivityTypes;
using System.Collections.Generic;
using System.Threading.Tasks;
using WorkflowCore.Interface;

namespace SatelittiBpms.Workflow.Tests.ActivityTypes
{
    public class SendTaskAcitivityTest
    {
        int tenantId = 23,
            activityId = 3,            
            requesterId = 65;

        Mock<IFieldValueService> _mockFieldValueService;
        Mock<IFlowPathService> _mockFlowPathService;
        Mock<ITaskService> _mockTaskService;
        Mock<IStepExecutionContext> _mockStepExecutionContext;
        Mock<IMailerService> _mockMailerService;
        Mock<IMessageService> _mockMessageService;
        Mock<IFrontendNotifyService> _mockFrontendNotifyService;

        [SetUp]
        public void Setup()
        {
            _mockFieldValueService = new Mock<IFieldValueService>();
            _mockFlowPathService = new Mock<IFlowPathService>();
            _mockTaskService = new Mock<ITaskService>();
            _mockStepExecutionContext = new Mock<IStepExecutionContext>();
            _mockMailerService = new Mock<IMailerService>();
            _mockFrontendNotifyService = new Mock<IFrontendNotifyService>();
            _mockMessageService = new Mock<IMessageService>();
        }

        [Test]
        public async Task ensureThatSendEventActivityInsertFlowPathAndTask()
        {
            int flowPathId = 84, taskId = 6;

            _mockStepExecutionContext.SetupGet(x => x.PersistenceData).Returns(() => null);
            _mockFrontendNotifyService.Setup(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()));
            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync(new ResultContent<int>(taskId, true, null));
            _mockFlowPathService.Setup(x => x.Insert(It.IsAny<FlowPathInfo>())).ReturnsAsync(new ResultContent<int>(flowPathId, true, null));
            _mockFieldValueService.Setup(x => x.UpdateFieldValues(It.IsAny<int>(), It.IsAny<object>()));

            SendTaskActivity sendEventActivity = new SendTaskActivity(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockMailerService.Object, _mockMessageService.Object, _mockFrontendNotifyService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId
            };
            var result = await sendEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.IsFalse(result.Proceed);
            Assert.NotNull(result.PersistenceData);
            Assert.AreEqual(taskId, result.PersistenceData);

            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()), Times.Once());
            _mockTaskService.Verify(x => x.Insert(It.IsAny<TaskInfo>()), Times.Once());
            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Once());
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockMailerService.Verify(x => x.SendMail(It.IsAny<MailMessage>(), It.IsAny<BaseConfig>()), Times.Never());
            _mockTaskService.Verify(x => x.Get(It.IsAny<int>()), Times.Never());
            _mockTaskService.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Never());
            _mockMessageService.Verify(x => x.CreateMessage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatSendEventActivitySendMail()
        {
            int taskId = 6;

            _mockStepExecutionContext.SetupGet(x => x.PersistenceData).Returns(taskId);
            _mockMailerService.Setup(x => x.SendMail(It.IsAny<MailMessage>(), It.IsAny<BaseConfig>()));
            _mockTaskService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(Result.Success(new TaskInfo()));
            _mockTaskService.Setup(x => x.Update(It.IsAny<TaskInfo>()));
            _mockMessageService.Setup(x => x.CreateMessage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new MailMessage());

            SendTaskActivity sendEventActivity = new SendTaskActivity(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockMailerService.Object, _mockMessageService.Object, _mockFrontendNotifyService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId
            };

            var result = await sendEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.AreEqual(taskId, sendEventActivity.TaskId);
            Assert.IsTrue(result.Proceed);

            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.IsAny<object>()), Times.Never());
            _mockTaskService.Verify(x => x.Insert(It.IsAny<TaskInfo>()), Times.Never());
            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Never());
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _mockMailerService.Verify(x => x.SendMail(It.IsAny<MailMessage>(), It.IsAny<BaseConfig>()), Times.Once());
            _mockTaskService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockTaskService.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Once());
            _mockMessageService.Verify(x => x.CreateMessage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatOpenFormToExecuteTaskInFrontEnd()
        {
            int? taskId = null;
            int taskIdNew = 2;

            _mockStepExecutionContext.SetupGet(x => x.PersistenceData).Returns(taskId);
            _mockMailerService.Setup(x => x.SendMail(It.IsAny<MailMessage>(), It.IsAny<BaseConfig>()));
            _mockTaskService.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(Result.Success(new TaskInfo()));
            _mockTaskService.Setup(x => x.Update(It.IsAny<TaskInfo>()));
            _mockTaskService.Setup(x => x.Insert(It.IsAny<TaskInfo>())).ReturnsAsync((Result.Success(taskIdNew)));
            _mockMessageService.Setup(x => x.CreateMessage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new MailMessage());

            SendTaskActivity sendEventActivity = new SendTaskActivity(_mockFieldValueService.Object, _mockFlowPathService.Object, _mockTaskService.Object, _mockMailerService.Object, _mockMessageService.Object, _mockFrontendNotifyService.Object)
            {
                TenantId = tenantId,
                RequesterId = requesterId,
                ActivityId = activityId
            };

            var result = await sendEventActivity.RunAsync(_mockStepExecutionContext.Object);
            Assert.AreEqual(taskIdNew, result.PersistenceData);
            Assert.IsFalse(result.Proceed);

            _mockFrontendNotifyService.Verify(x => x.Notify(It.IsAny<string>(), It.Is<IDictionary<string, object?>>((d) => d.ContainsKey("canExecute") && ((bool)d["canExecute"]) == false)));
            _mockTaskService.Verify(x => x.Insert(It.IsAny<TaskInfo>()), Times.Once());
            _mockFlowPathService.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Once());
            _mockFieldValueService.Verify(x => x.ReplicateFieldValues(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockMailerService.Verify(x => x.SendMail(It.IsAny<MailMessage>(), It.IsAny<BaseConfig>()), Times.Never());
            _mockTaskService.Verify(x => x.Get(It.IsAny<int>()), Times.Never());
            _mockTaskService.Verify(x => x.Update(It.IsAny<TaskInfo>()), Times.Never());
            _mockMessageService.Verify(x => x.CreateMessage(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }
    }
}
