using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SatelittiBpms.Services.Tests
{
    public class TaskSignerServiceTest
    {
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<ITaskSignerRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<IWorkflowHostService> _mockWorkflowHostService;        
        Mock<ILogger<TaskSignerService>> _mockLogger;        

        [SetUp]
        public void Setup()
        {
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockRepository = new Mock<ITaskSignerRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockWorkflowHostService = new Mock<IWorkflowHostService>();            
            _mockLogger = new Mock<ILogger<TaskSignerService>>();            
        }

        private TaskSignerService InstantiateTaskSignerService()
        {
            return new(_mockContextDataService.Object, _mockRepository.Object, _mockMapper.Object, _mockWorkflowHostService.Object, _mockLogger.Object);
        }

        [Test]
        public void EnsureThatReturnsErrorWhenEnvelopeIdIsMissing()
        {
            TaskSignerService taskSignerService = InstantiateTaskSignerService();
            var result = taskSignerService.ActionPerformedOnsigner(new ActionPerformedOnSignerDTO { EnvelopeId = 0, Action = "ssss" });

            Assert.IsFalse(result.Result.Success);
            Assert.AreEqual(ExceptionCodes.ENVELOPE_ID_SSIGN_NOT_INFORMED, result.Result.ErrorId);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());            
            _mockWorkflowHostService.Verify(x => x.ExecuteTaskSignerIntegration(It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void EnsureThatReturnsErrorWhenEnvelopeIdDoesNotExists()
        {
            TaskSignerService taskSignerService = InstantiateTaskSignerService();
            var result = taskSignerService.ActionPerformedOnsigner(new ActionPerformedOnSignerDTO { EnvelopeId = 155, Action = "COMPLETED" });
            Assert.IsFalse(result.Result.Success);
            Assert.AreEqual(ExceptionCodes.ENVELOPE_SSIGN_NOT_FOUND, result.Result.ErrorId);
            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());            
            _mockWorkflowHostService.Verify(x => x.ExecuteTaskSignerIntegration(It.IsAny<int>()), Times.Never());            
        }

        [Test]
        public void EnsureThatReturnsErrorWhenActionDoesNotExists()
        {
            TaskSignerService taskSignerService = InstantiateTaskSignerService();
            var result = taskSignerService.ActionPerformedOnsigner(new ActionPerformedOnSignerDTO { EnvelopeId = 155, Action = "OTHERACTION" });
            Assert.IsFalse(result.Result.Success);
            Assert.AreEqual(ExceptionCodes.ACTION_ENVELOPE_SSIGN_NOT_FOUND, result.Result.ErrorId);
            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());        
            _mockWorkflowHostService.Verify(x => x.ExecuteTaskSignerIntegration(It.IsAny<int>()), Times.Never());        

        }

        [Test]
        public void EnsureThatReturnsSuccessWhenEnvelopeIdAndActionExists()
        {
            int EnvelopeId = 5;            
            
            _mockRepository.Setup(x => x.GetQuery(It.IsAny<Expression<Func<TaskSignerInfo, bool>>>())).Returns(new List<TaskSignerInfo>() {
                 new TaskSignerInfo()
                {
                    DateSendEvelope = DateTime.UtcNow,
                    EnvelopeId = EnvelopeId,
                    TenantId = 55,
                    Task = new TaskInfo
                    {
                        Id = 4,
                        ActivityId = 1,
                        CreatedDate = DateTime.UtcNow.AddDays(-3)
                    }
                }
            }.AsQueryable());

            TaskSignerService taskSignerService = InstantiateTaskSignerService();
            var result = taskSignerService.ActionPerformedOnsigner(new ActionPerformedOnSignerDTO { EnvelopeId = EnvelopeId, Action = "COMPLETED" });
            Assert.IsTrue(result.Result.Success);            
            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());            
        }
    }
}
