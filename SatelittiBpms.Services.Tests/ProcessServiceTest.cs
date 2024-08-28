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
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class ProcessServiceTest
    {
        Mock<IProcessVersionRepository> _mockProcessVersionRepository;
        Mock<IProcessRepository> _mockRepository;
        Mock<IProcessService> _mockProcessService;
        Mock<IMapper> _mockMapper;
        Mock<IContextDataService<UserInfo>> _mockContextDataService;

        [SetUp]
        public void Setup()
        {
            _mockProcessVersionRepository = new Mock<IProcessVersionRepository>();
            _mockRepository = new Mock<IProcessRepository>();
            _mockProcessService = new Mock<IProcessService>();
            _mockMapper = new Mock<IMapper>();
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
        }

        [Test]
        public async Task ensureListProcessListViewModel()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = 2 } });
            _mockRepository.Setup(x => x.GetByTenantIncludingRelationship(It.IsAny<long>())).Returns(new List<ProcessInfo>()
            {
                new ProcessInfo()
                {
                    Id = 22,
                    CurrentVersion = null,
                    ProcessVersions = new List<ProcessVersionInfo>()
                    {
                        new ProcessVersionInfo()
                        {
                            Id = 2,
                            Name = "Version 2",
                            CreatedDate = new DateTime(2021, 02, 11, 5, 16, 00),
                            CreatedByUserName = "CreatedUser Bertucci",
                            LastModifiedDate = new DateTime(2021, 03, 12, 15, 46, 02),
                            Status = ProcessStatusEnum.EDITING
                        }
                    }
                }
            }.AsQueryable());

            ProcessService processService = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = await processService.ListProcessListViewModel(new ProcessFilterDTO());
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Value.Count);
        }

        [Test]
        public async Task ensureThatGetByTenantWhenNotPassTenantId()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = 56 } });
            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ProcessInfo());
            ProcessService processService = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = await processService.GetByTenant(3);
            Assert.IsTrue(result.Success);
            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
        }

        [Test]
        public async Task ensureThatGetByTenantWhenPassTenantId()
        {
            _mockContextDataService.Setup(x => x.GetContextData());
            _mockRepository.Setup(x => x.GetByIdAndTenantId(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ProcessInfo());
            ProcessService processService = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = await processService.GetByTenant(3, 57);
            Assert.IsTrue(result.Success);
            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());
        }


        [Test]
        public async Task ensureThatUpdateCurrentVersionWhenNotPassTenantId()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = 56 } });
            _mockProcessVersionRepository.Setup(x => x.GetLastPublishedProcessVersion(It.IsAny<int>(), It.IsAny<long>())).ReturnsAsync(new ProcessVersionInfo() { Version = 1 });
            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ProcessInfo());
            _mockRepository.Setup(x => x.Update(It.IsAny<ProcessInfo>()));

            ProcessService processService = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            await processService.UpdateCurrentVersion(5);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockProcessVersionRepository.Verify(x => x.GetLastPublishedProcessVersion(It.IsAny<int>(), It.IsAny<long>()), Times.Once());
            _mockRepository.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.Update(It.IsAny<ProcessInfo>()), Times.Once());
        }

        [Test]
        public async Task ensureThatUpdateCurrentVersionWhenPassTenantId()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = 56 } });
            _mockProcessVersionRepository.Setup(x => x.GetLastPublishedProcessVersion(It.IsAny<int>(), It.IsAny<long>())).ReturnsAsync(new ProcessVersionInfo() { Version = 1 });
            _mockRepository.Setup(x => x.Get(It.IsAny<int>())).ReturnsAsync(new ProcessInfo());
            _mockRepository.Setup(x => x.Update(It.IsAny<ProcessInfo>()));

            ProcessService processService = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            await processService.UpdateCurrentVersion(5, 89);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Never());
            _mockProcessVersionRepository.Verify(x => x.GetLastPublishedProcessVersion(It.IsAny<int>(), It.IsAny<long>()), Times.Once());
            _mockRepository.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockRepository.Verify(x => x.Update(It.IsAny<ProcessInfo>()), Times.Once());
        }

        [Test]
        public void ensureList()
        {
            var processVersionList = new List<ProcessVersionInfo>();

            processVersionList.Add(new ProcessVersionInfo()
            {
                Version = 2,
                WorkflowContent = ""
            });

            _mockRepository.Setup(x => x.List()).Returns(new List<ProcessInfo>() { new ProcessInfo() { Id = 3, CurrentVersion = 2, ProcessVersions = processVersionList } });

            ProcessService processService = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = processService.ListWorkFlows();

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public void ensureListPreviousVersion()
        {
            var processVersionList = new List<ProcessVersionInfo>();


            var flowList = new List<FlowInfo>();

            flowList.Add(new FlowInfo()
            {
                ProcessVersionId = 1,
                Status = Models.Enums.FlowStatusEnum.INPROGRESS,
                ProcessVersion = new ProcessVersionInfo() { WorkflowContent = "WorkflowContent" }
            });

            processVersionList.Add(new ProcessVersionInfo()
            {
                Id = 1,
                ProcessId = 3,
                Version = 2,
                WorkflowContent = "WorkflowContent",
                Flows = flowList
            });

            _mockRepository.Setup(x => x.List()).Returns(new List<ProcessInfo>() { new ProcessInfo() { Id = 3, CurrentVersion = 3, ProcessVersions = processVersionList } });
            ProcessService processService = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = processService.ListWorkFlows();

            Assert.AreEqual(1, result.Count);
        }

        [Test]
        public async Task ensureThatGetCounterProcessAsync()
        {
            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = 2 } });
            _mockRepository.Setup(x => x.GetByTenantIncludingRelationship(It.IsAny<long>())).Returns(new List<ProcessInfo>()
            {
                new ProcessInfo()
                {
                    Id = 22,
                    CurrentVersion = null,
                    ProcessVersions = new List<ProcessVersionInfo>()
                    {
                        new ProcessVersionInfo()
                        {
                            Id = 2,
                            Name = "Version 2",
                            CreatedDate = new DateTime(2021, 02, 11, 5, 16, 00),
                            CreatedByUserName = "CreatedUser",
                            LastModifiedDate = new DateTime(2021, 03, 12, 15, 46, 02),
                            Status = ProcessStatusEnum.EDITING
                        }
                    }
                }
            }.AsQueryable());

            ProcessService processService = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = await processService.GetCounterProcess(new ProcessFilterDTO());
            Assert.IsTrue(result.Success);
            Assert.AreEqual(1, result.Value.TotalAll);
        }

        [Test]
        public async Task ensureListToFiltersWhenNoHaveDiffentNames()
        {
            int tenantId = 2;
            List<ProcessInfo> lstProcesses = new List<ProcessInfo>()
            {
                new ProcessInfo(){
                    Id = 3,
                    TenantId = tenantId,
                    ProcessVersions = new List<ProcessVersionInfo>(){
                        new ProcessVersionInfo(){
                            Id= 4,
                            Name = "Process 3"
                        }
                    }
                },
                new ProcessInfo(){
                    Id = 4,
                    TenantId = tenantId,
                    ProcessVersions = new List<ProcessVersionInfo>()
                    {
                        new ProcessVersionInfo(){
                            Id= 7,
                            Name = "Process 4"
                        },
                        new ProcessVersionInfo(){
                            Id= 8,
                            Name = "Process 4"
                        }
                    }
                },
                new ProcessInfo(){
                    Id = 3,
                    TenantId = 23,
                    ProcessVersions = new List<ProcessVersionInfo>()
                }
            };

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = tenantId } });
            _mockRepository.Setup(x => x.ListAsync()).ReturnsAsync(lstProcesses);

            var service = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = await service.ListToFilters();
            var resultValue = ResultContent<List<FlowGroupViewModel>>.GetValue(result);

            Assert.AreEqual(2, resultValue.Count);
            Assert.AreEqual("Process 3", resultValue[0].Name);
            Assert.AreEqual(1, resultValue[0].Ids.Count);
            Assert.AreEqual(4, resultValue[0].Ids[0]);

            Assert.AreEqual("Process 4", resultValue[1].Name);
            Assert.AreEqual(2, resultValue[1].Ids.Count);
            Assert.AreEqual(7, resultValue[1].Ids[0]);
            Assert.AreEqual(8, resultValue[1].Ids[1]);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockRepository.Verify(x => x.ListAsync(), Times.Once());
        }

        [Test]
        public async Task ensureListToFiltersWhenHaveDiffentNamesSameProcess()
        {
            int tenantId = 2;
            List<ProcessInfo> lstProcesses = new List<ProcessInfo>()
            {
                new ProcessInfo(){
                    Id = 3,
                    TenantId = tenantId,
                    ProcessVersions = new List<ProcessVersionInfo>(){
                        new ProcessVersionInfo(){
                            Id= 4,
                            Name = "Process 3"
                        },
                        new ProcessVersionInfo(){
                            Id= 87,
                            Name = "Process 3 Teste"
                        }
                    }
                },
                new ProcessInfo(){
                    Id = 4,
                    TenantId = tenantId,
                    ProcessVersions = new List<ProcessVersionInfo>()
                    {
                        new ProcessVersionInfo(){
                            Id= 7,
                            Name = "Process 4"
                        },
                        new ProcessVersionInfo(){
                            Id= 8,
                            Name = "Process 4 Teste"
                        },
                        new ProcessVersionInfo(){
                            Id= 23,
                            Name = "Process 4 Teste"
                        },
                        new ProcessVersionInfo(){
                            Id= 45,
                            Name = "Process 4"
                        }
                    }
                },
                new ProcessInfo(){
                    Id = 3,
                    TenantId = 23,
                    ProcessVersions = new List<ProcessVersionInfo>()
                }
            };

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = tenantId } });
            _mockRepository.Setup(x => x.ListAsync()).ReturnsAsync(lstProcesses);

            var service = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = await service.ListToFilters();
            var resultValue = ResultContent<List<FlowGroupViewModel>>.GetValue(result);

            Assert.AreEqual(4, resultValue.Count);
            Assert.AreEqual("Process 3", resultValue[0].Name);
            Assert.AreEqual(1, resultValue[0].Ids.Count);
            Assert.AreEqual(4, resultValue[0].Ids[0]);

            Assert.AreEqual("Process 3 Teste", resultValue[1].Name);
            Assert.AreEqual(1, resultValue[1].Ids.Count);
            Assert.AreEqual(87, resultValue[1].Ids[0]);

            Assert.AreEqual("Process 4", resultValue[2].Name);
            Assert.AreEqual(2, resultValue[2].Ids.Count);
            Assert.AreEqual(7, resultValue[2].Ids[0]);
            Assert.AreEqual(45, resultValue[2].Ids[1]);

            Assert.AreEqual("Process 4 Teste", resultValue[3].Name);
            Assert.AreEqual(2, resultValue[3].Ids.Count);
            Assert.AreEqual(8, resultValue[3].Ids[0]);
            Assert.AreEqual(23, resultValue[3].Ids[1]);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockRepository.Verify(x => x.ListAsync(), Times.Once());
        }

        [Test]
        public async Task ensureListToFiltersWhenSameNameDifferentsProcesses()
        {
            int tenantId = 2;
            List<ProcessInfo> lstProcesses = new List<ProcessInfo>()
            {
                new ProcessInfo(){
                    Id = 3,
                    TenantId = tenantId,
                    ProcessVersions = new List<ProcessVersionInfo>(){
                        new ProcessVersionInfo(){
                            Id= 4,
                            Name = "Process 3"
                        },
                        new ProcessVersionInfo(){
                            Id= 34,
                            Name = "Solicitação de férias"
                        }
                    }
                },
                new ProcessInfo(){
                    Id = 4,
                    TenantId = tenantId,
                    ProcessVersions = new List<ProcessVersionInfo>()
                    {
                        new ProcessVersionInfo(){
                            Id= 7,
                            Name = "Process 4"
                        },
                        new ProcessVersionInfo(){
                            Id= 8,
                            Name = "Process 4"
                        },
                        new ProcessVersionInfo(){
                            Id= 53,
                            Name = "Solicitação de férias"
                        }
                    }
                },
                new ProcessInfo(){
                    Id = 3,
                    TenantId = 23,
                    ProcessVersions = new List<ProcessVersionInfo>()
                }
            };

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = tenantId } });
            _mockRepository.Setup(x => x.ListAsync()).ReturnsAsync(lstProcesses);

            var service = new ProcessService(_mockProcessVersionRepository.Object, _mockRepository.Object, _mockMapper.Object, _mockContextDataService.Object);
            var result = await service.ListToFilters();
            var resultValue = ResultContent<List<FlowGroupViewModel>>.GetValue(result);

            Assert.AreEqual(4, resultValue.Count);
            Assert.AreEqual("Process 3", resultValue[0].Name);
            Assert.AreEqual(1, resultValue[0].Ids.Count);
            Assert.AreEqual(4, resultValue[0].Ids[0]);

            Assert.AreEqual("Solicitação de férias", resultValue[1].Name);
            Assert.AreEqual(1, resultValue[1].Ids.Count);
            Assert.AreEqual(34, resultValue[1].Ids[0]);

            Assert.AreEqual("Process 4", resultValue[2].Name);
            Assert.AreEqual(2, resultValue[2].Ids.Count);
            Assert.AreEqual(7, resultValue[2].Ids[0]);
            Assert.AreEqual(8, resultValue[2].Ids[1]);

            Assert.AreEqual("Solicitação de férias", resultValue[3].Name);
            Assert.AreEqual(1, resultValue[3].Ids.Count);
            Assert.AreEqual(53, resultValue[3].Ids[0]);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockRepository.Verify(x => x.ListAsync(), Times.Once());
        }
    }
}