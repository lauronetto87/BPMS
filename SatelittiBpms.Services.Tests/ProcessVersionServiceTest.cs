using AutoMapper;
using Microsoft.EntityFrameworkCore.Storage;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.Workflow.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services.Tests
{
    public class ProcessVersionServiceTest
    {
        Mock<IProcessVersionRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<IProcessService> _mockProcessService;
        Mock<IWorkflowValidationService> _mockWorkflowValidationService;
        Mock<IProcessRoleService> _mockProcessRoleService;
        Mock<IActivityService> _mockActivityService;
        Mock<IXmlDiagramParseService> _mockXmlDiagramParseService;
        Mock<IWorkflowHostService> _mockWorkflowHostService;
        Mock<IActivityUserService> _mockActivityUserService;
        Mock<IFlowService> _mockFlowService;
        Mock<ISuiteUserService> _mockSuiteUserService;
        Mock<ITenantService> _mockTenantService;
        Mock<ISignerIntegrationActivityService> _mockSignerIntegrationActivityService;
        Mock<IDbContextTransaction> _mockDbContextTransaction;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IProcessVersionRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockProcessService = new Mock<IProcessService>();
            _mockWorkflowValidationService = new Mock<IWorkflowValidationService>();
            _mockProcessRoleService = new Mock<IProcessRoleService>();
            _mockActivityService = new Mock<IActivityService>();
            _mockXmlDiagramParseService = new Mock<IXmlDiagramParseService>();
            _mockWorkflowHostService = new Mock<IWorkflowHostService>();
            _mockActivityUserService = new Mock<IActivityUserService>();
            _mockSuiteUserService = new Mock<ISuiteUserService>();
            _mockTenantService = new Mock<ITenantService>();
            _mockSignerIntegrationActivityService = new Mock<ISignerIntegrationActivityService>();

            _mockDbContextTransaction = new Mock<IDbContextTransaction>();
            _mockFlowService = new Mock<IFlowService>();
            _mockDbContextTransaction.Setup(x => x.Commit());
            _mockDbContextTransaction.Setup(x => x.Rollback());

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = 34 }, User = new UserInfo { Id = 1 } });
        }

        [Test]
        public async Task ensureThatSaveReturnErrorWhenFailToInserProcess()
        {
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 0,
                NeedPublish = false,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>()
            };

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockProcessService.Setup(x => x.Insert(It.IsAny<ProcessDTO>())).ReturnsAsync(new ResultContent<int>(33, false, null));
            var processVersionService = CreateService();

            var result = await processVersionService.Save(dto);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ExceptionCodes.PROCESS_VERSION_SAVE_INSERT_PROCESS_ERROR, result.ErrorId);

            _mockProcessService.Verify(x => x.Insert(It.IsAny<ProcessDTO>()), Times.Once());
            _mockRepository.Verify(x => x.Insert(It.IsAny<ProcessVersionInfo>()), Times.Never());
            _mockProcessRoleService.Verify(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _mockActivityService.Verify(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatSaveInsertProcessAndVersionProcessAndNotPublishWhenProcessIdIsZeroAndNeedPublishIsFalse()
        {
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 0,
                NeedPublish = false,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>()
            };

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockProcessService.Setup(x => x.Insert(It.IsAny<ProcessDTO>())).ReturnsAsync(new ResultContent<int>(33, true, null));
            _mockMapper.Setup(x => x.Map<ProcessVersionInfo>(It.IsAny<ProcessVersionDTO>())).Returns(new ProcessVersionInfo());
            _mockRepository.Setup(x => x.Insert(It.IsAny<ProcessVersionInfo>())).ReturnsAsync(23);
            _mockProcessRoleService.Setup(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockActivityService.Setup(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()));
            var processVersionService = CreateService();

            var result = await processVersionService.Save(dto);
            Assert.IsTrue(result.Success);

            _mockProcessService.Verify(x => x.Insert(It.IsAny<ProcessDTO>()), Times.Once());
            _mockRepository.Verify(x => x.Insert(It.IsAny<ProcessVersionInfo>()), Times.Once());
            _mockProcessRoleService.Verify(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockActivityService.Verify(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatSaveReturnErrorWhenFailToGetProcess()
        {
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 2,
                NeedPublish = false,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>()
            };

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockProcessService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ProcessInfo>(null, false, null));
            var processVersionService = CreateService();

            var result = await processVersionService.Save(dto);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ExceptionCodes.PROCESS_VERSION_SAVE_GET_PROCESS_ERROR, result.ErrorId);

            _mockProcessService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockProcessService.Verify(x => x.Insert(It.IsAny<ProcessDTO>()), Times.Never());
            _mockRepository.Verify(x => x.Insert(It.IsAny<ProcessVersionInfo>()), Times.Never());
            _mockProcessRoleService.Verify(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _mockActivityService.Verify(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureThatSaveInsertProcessAndVersionProcessAndNotPublishWhenProcessIdIsNotZeroAndProcessNotExistsAndNeedPublishIsFalse()
        {
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 4,
                NeedPublish = false,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>()
            };

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockProcessService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ProcessInfo>(null, true, null));
            _mockProcessService.Setup(x => x.Insert(It.IsAny<ProcessDTO>())).ReturnsAsync(new ResultContent<int>(33, true, null));
            _mockMapper.Setup(x => x.Map<ProcessVersionInfo>(It.IsAny<ProcessVersionDTO>())).Returns(new ProcessVersionInfo());
            _mockRepository.Setup(x => x.Insert(It.IsAny<ProcessVersionInfo>())).ReturnsAsync(23);
            _mockProcessRoleService.Setup(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockActivityService.Setup(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()));
            var processVersionService = CreateService();

            var result = await processVersionService.Save(dto);
            Assert.IsTrue(result.Success);

            _mockProcessService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockProcessService.Verify(x => x.Insert(It.IsAny<ProcessDTO>()), Times.Once());
            _mockRepository.Verify(x => x.Insert(It.IsAny<ProcessVersionInfo>()), Times.Once());
            _mockProcessRoleService.Verify(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockActivityService.Verify(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatSaveGetProcessAndInsertVersionProcessAndNotPublishWhenProcessIdIsNotZeroAndProcessVersionNotExistsAndNeedPublishIsFalse()
        {
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 2,
                NeedPublish = false,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>()
            };
            var processInfo = new ProcessInfo()
            {
                Id = 55,
                TenantId = 4,
                ProcessVersions = new List<ProcessVersionInfo>()
                {
                    new ProcessVersionInfo(){
                        Version = 1
                    },
                    new ProcessVersionInfo(){
                        Version = 3
                    },
                    new ProcessVersionInfo(){
                        Version = 2
                    }
                }
            };

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockProcessService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ProcessInfo>(processInfo, true, null));
            _mockRepository.Setup(x => x.GetByProcessAndStatus(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<int>())).ReturnsAsync(() => null);
            _mockMapper.Setup(x => x.Map<ProcessVersionInfo>(It.IsAny<ProcessVersionDTO>())).Returns(new ProcessVersionInfo());
            _mockRepository.Setup(x => x.Insert(It.IsAny<ProcessVersionInfo>())).ReturnsAsync(23);
            _mockProcessRoleService.Setup(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockActivityService.Setup(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()));
            var processVersionService = CreateService();

            var result = await processVersionService.Save(dto);
            Assert.IsTrue(result.Success);

            _mockProcessService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetByProcessAndStatus(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.Insert(It.Is<ProcessVersionInfo>(x => x.Version == 4)), Times.Once());
            _mockProcessRoleService.Verify(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockActivityService.Verify(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatSaveGetProcessAndVersionProcessAndPublishWhenProcessIdIsNotZeroAndProcessVersionExistsAndNeedPublishIsTrueAndWorkflowValidateIsTrue()
        {
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 2,
                NeedPublish = true,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>(),
                DiagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:process id=\"Process_1\" isExecutable=\"false\"></bpmn2:process></bpmn2:definitions>"
            };
            var processInfo = new ProcessInfo()
            {
                Id = 55,
                TenantId = 4,
                ProcessVersions = new List<ProcessVersionInfo>()
            };
            var processVersion = new ProcessVersionInfo()
            {
                Id = 34,
                Activities = new List<ActivityInfo>(),
                ProcessVersionRoles = new List<ProcessVersionRoleInfo>()
            };

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockProcessService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ProcessInfo>(processInfo, true, null));
            _mockRepository.Setup(x => x.GetByProcessAndStatus(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<int>())).ReturnsAsync(processVersion);
            _mockActivityService.Setup(x => x.Delete(It.IsAny<int>()));
            _mockProcessRoleService.Setup(x => x.Delete(It.IsAny<int>()));
            _mockMapper.Setup(x => x.Map<ProcessVersionInfo>(It.IsAny<ProcessVersionDTO>())).Returns(processVersion);
            _mockMapper.Setup(x => x.Map(It.IsAny<ProcessVersionInfo>(), It.IsAny<ProcessVersionInfo>())).Returns(processVersion);
            _mockRepository.Setup(x => x.Update(It.IsAny<ProcessVersionInfo>()));
            _mockProcessRoleService.Setup(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockActivityService.Setup(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockWorkflowValidationService.Setup(x => x.Validate(It.IsAny<XmlNode>())).Returns(new ResultContent(true, null));
            _mockRepository.Setup(x => x.UpdateStatusAndWorkflowContent(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<string>()));
            _mockProcessService.Setup(x => x.UpdateCurrentVersion(It.IsAny<int>(), It.IsAny<int>()));
            _mockActivityService.Setup(x => x.InsertByDiagram(It.IsAny<XmlNode>(), It.IsAny<int>(), It.IsAny<int>()));
            var processVersionService = CreateService();

            var result = await processVersionService.Save(dto);
            Assert.IsTrue(result.Success);

            _mockProcessService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetByProcessAndStatus(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.Insert(It.Is<ProcessVersionInfo>(x => x.Version == 4)), Times.Never());
            _mockRepository.Verify(x => x.Update(It.IsAny<ProcessVersionInfo>()), Times.Once());
            _mockProcessRoleService.Verify(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockActivityService.Verify(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockWorkflowValidationService.Verify(x => x.Validate(It.IsAny<XmlNode>()), Times.Once());
            _mockRepository.Verify(x => x.UpdateStatusAndWorkflowContent(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<string>()), Times.Once());
            _mockProcessService.Verify(x => x.UpdateCurrentVersion(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockActivityService.Verify(x => x.InsertByDiagram(It.IsAny<XmlNode>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureThatSaveReturnErrorWhenWorkflowValidateIsFalse()
        {
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 2,
                NeedPublish = true,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>(),
                DiagramContent = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><bpmn2:definitions xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:bpmn2=\"http://www.omg.org/spec/BPMN/20100524/MODEL\" xmlns:bpmndi=\"http://www.omg.org/spec/BPMN/20100524/DI\" xmlns:dc=\"http://www.omg.org/spec/DD/20100524/DC\" xmlns:di=\"http://www.omg.org/spec/DD/20100524/DI\" xmlns:satelitti=\"http://selbetti/schema/bpmn/satelitti\" id=\"sample-diagram\" targetNamespace=\"http://bpmn.io/schema/bpmn\" xsi:schemaLocation=\"http://www.omg.org/spec/BPMN/20100524/MODEL BPMN20.xsd\"><bpmn2:process id=\"Process_1\" isExecutable=\"false\"></bpmn2:process></bpmn2:definitions>"
            };
            var processInfo = new ProcessInfo()
            {
                Id = 55,
                TenantId = 4,
                ProcessVersions = new List<ProcessVersionInfo>()
            };
            var processVersion = new ProcessVersionInfo()
            {
                Id = 34,
                Activities = new List<ActivityInfo>(),
                ProcessVersionRoles = new List<ProcessVersionRoleInfo>()
            };

            _mockRepository.Setup(x => x.BeginTransaction()).Returns(_mockDbContextTransaction.Object);
            _mockProcessService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ProcessInfo>(processInfo, true, null));
            _mockRepository.Setup(x => x.GetByProcessAndStatus(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<int>())).ReturnsAsync(processVersion);
            _mockActivityService.Setup(x => x.Delete(It.IsAny<int>()));
            _mockProcessRoleService.Setup(x => x.Delete(It.IsAny<int>()));
            _mockMapper.Setup(x => x.Map<ProcessVersionInfo>(It.IsAny<ProcessVersionDTO>())).Returns(processVersion);
            _mockMapper.Setup(x => x.Map(It.IsAny<ProcessVersionInfo>(), It.IsAny<ProcessVersionInfo>())).Returns(processVersion);
            _mockRepository.Setup(x => x.Update(It.IsAny<ProcessVersionInfo>()));
            _mockProcessRoleService.Setup(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockActivityService.Setup(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()));
            _mockWorkflowValidationService.Setup(x => x.Validate(It.IsAny<XmlNode>())).Returns(new ResultContent(false, ""));
            _mockRepository.Setup(x => x.UpdateStatusAndWorkflowContent(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<string>()));
            _mockProcessService.Setup(x => x.UpdateCurrentVersion(It.IsAny<int>(), It.IsAny<int>()));
            _mockActivityUserService.Setup(x => x.InsertByDiagram(It.IsAny<XmlNode>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()));
            var processVersionService = CreateService();

            var result = await processVersionService.Save(dto);
            Assert.IsFalse(result.Success);

            _mockProcessService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.GetByProcessAndStatus(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.Insert(It.Is<ProcessVersionInfo>(x => x.Version == 4)), Times.Never());
            _mockRepository.Verify(x => x.Update(It.IsAny<ProcessVersionInfo>()), Times.Once());
            _mockProcessRoleService.Verify(x => x.InsertMany(It.IsAny<List<int>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockActivityService.Verify(x => x.InsertMany(It.IsAny<List<ActivityDTO>>(), It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockWorkflowValidationService.Verify(x => x.Validate(It.IsAny<XmlNode>()), Times.Once());
            _mockRepository.Verify(x => x.UpdateStatusAndWorkflowContent(It.IsAny<int>(), It.IsAny<ProcessStatusEnum>(), It.IsAny<string>()), Times.Never());
            _mockProcessService.Verify(x => x.UpdateCurrentVersion(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
            _mockActivityUserService.Verify(x => x.InsertByDiagram(It.IsAny<XmlNode>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public void ensureThatIsNameValidCheckDuplicateToSucess()
        {
            var processName = "Name";
            var editProcessVersionId = 4;
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 2,
                NeedPublish = false,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>()
            };

            var process = new List<ProcessVersionInfo>();

            _mockRepository.Setup(x => x.GetByTenant(It.IsAny<long>())).Returns(process.AsQueryable());

            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ProcessVersionInfo() { Id = editProcessVersionId, ProcessId = 1 });

            _mockProcessService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ProcessInfo>(null, false, null));
            var processVersionService = CreateService();

            var result = processVersionService.IsNameValidCheckDuplicate(processName, editProcessVersionId);
            Assert.IsTrue(result.Success);
        }

        [Test]
        public void ensureThatIsNameValidCheckDuplicateToError()
        {
            var processName = "Name";
            var editProcessVersionId = 4;
            var dto = new ProcessVersionDTO()
            {
                ProcessId = 2,
                NeedPublish = false,
                RolesIds = new List<int>(),
                Activities = new List<ActivityDTO>()
            };

            var process = new List<ProcessVersionInfo>
            {
                new ProcessVersionInfo
                {
                    Name = processName,
                    Id = 5,
                }
            };

            _mockRepository.Setup(x => x.GetByTenant(It.IsAny<long>())).Returns(process.AsQueryable());

            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ProcessVersionInfo() { Id = editProcessVersionId, ProcessId = 1 });

            _mockProcessService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ProcessInfo>(null, false, null));
            var processVersionService = CreateService();

            var result = processVersionService.IsNameValidCheckDuplicate(processName, editProcessVersionId);
            Assert.IsFalse(result.Success);
            Assert.AreEqual(ExceptionCodes.PROCESS_VERSION_SAVE_NAME_DUPLICATE, result.ValidationResult.Errors[0].ErrorMessage);
        }

        private ProcessVersionService CreateService()
        {
            return new ProcessVersionService(_mockRepository.Object,
                                            _mockMapper.Object,
                                            _mockContextDataService.Object,
                                            _mockProcessService.Object,
                                            _mockWorkflowValidationService.Object,
                                            _mockProcessRoleService.Object,
                                            _mockActivityService.Object,
                                            _mockXmlDiagramParseService.Object,
                                            _mockWorkflowHostService.Object,
                                            _mockFlowService.Object,
                                            _mockSuiteUserService.Object,
                                            _mockTenantService.Object,
                                            _mockSignerIntegrationActivityService.Object);
        }
    }
}
