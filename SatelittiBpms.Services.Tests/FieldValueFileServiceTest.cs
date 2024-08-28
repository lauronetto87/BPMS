using AutoMapper;
using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class FieldValueFileServiceTest
    {
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<IFieldValueFileRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<IStorageService> _mockStorageService;
        Mock<IFieldValueService> _mockFieldValueService;
        public UserInfo UserMock { get; set; }
        public SuiteTenantAuth TenantMock { get; set; }

        [SetUp]
        public void Setup()
        {
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockRepository = new Mock<IFieldValueFileRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockStorageService = new Mock<IStorageService>();
            _mockFieldValueService = new Mock<IFieldValueService>();

            TenantMock = new SuiteTenantAuth
            {
                Id = 55,
                Language = "pt",
                SubDomain = "tenantSubdomain"
            };

            UserMock = new UserInfo
            {
                Id = 1,
                Enable = true,
                TenantId = 55,
                Timezone = -3,
                Type = Models.Enums.BpmsUserTypeEnum.ADMINISTRATOR
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
        public void EnsureConstructor()
        {
            FieldValueFileService fieldService = new(_mockContextDataService.Object, _mockRepository.Object, _mockStorageService.Object, _mockFieldValueService.Object);
            Assert.IsNotNull(fieldService);
        }

        [Test]
        public async Task EnsureThatDeleteFileFromS3OnlyWhenUploadedOnCurrentTaskAsync()
        {
            var taskId = 2;

            List<FieldValueFileInfo> fieldValueFileList = new()
            {
                new FieldValueFileInfo()
                {
                    FieldValueId = 10,
                    FieldValue = new FieldValueInfo() { TaskId = 2 },
                    Key = "123",
                    UploadedFieldValueId = 5,
                    FileKey = "AAA1"
                },
                new FieldValueFileInfo()
                {
                    FieldValueId = 10,
                    FieldValue = new FieldValueInfo() { TaskId = 2 },
                    Key = "345",
                    UploadedFieldValueId = 6,
                    FileKey = "AAA2"
                },
                new FieldValueFileInfo()
                {
                    FieldValueId = 10,
                    FieldValue = new FieldValueInfo() { TaskId = 2 },
                    Key = "567",
                    UploadedFieldValueId = 10,
                    FileKey = "AAA3"
                },
                new FieldValueFileInfo()
                {
                    FieldValueId = 10,
                    FieldValue = new FieldValueInfo() { TaskId = 2 },
                    Key = "890",
                    UploadedFieldValueId = 10,
                    FileKey = "AAA4"
                }
            };

            _mockRepository.Setup(x => x.GetByTenant(It.IsAny<long>())).Returns(fieldValueFileList.AsQueryable());

            FieldValueFileService fieldValueFileService = new(_mockContextDataService.Object, _mockRepository.Object, _mockStorageService.Object, _mockFieldValueService.Object);

            await fieldValueFileService.Delete(taskId);

            _mockStorageService.Verify(x => x.Delete(It.IsAny<string>()), Times.Exactly(2));
            _mockRepository.Verify(x => x.Delete(It.IsAny<FieldValueFileInfo>()), Times.Exactly(4));
        }

        [Test]
        public async Task EnsureThatRowBackFieldValueFilesWhenUnassignAsync()
        {
            var previousTaskInfo = new TaskInfo()
            {
                Id = 2,
                FieldsValues = new List<FieldValueInfo>() {
                    { new FieldValueInfo() {
                        Id = 1,
                        FieldValue = "[file.doc]",
                        FieldId = 70,
                        Field = new FieldInfo() { Id = 70, Type = FieldTypeEnum.FILE },
                        FieldValueFiles = new List<FieldValueFileInfo>(){
                            new FieldValueFileInfo() {
                                FieldValueId = 1,
                                UploadedFieldValueId = 1,
                                Key = "",
                                Name = "",
                                Size = 123,
                                Type = "",
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = 1,
                                TenantId = TenantMock.Id,
                            }
                        }
                    } },
                    { new FieldValueInfo() {
                        Id = 2,
                        FieldValue = "[file.pdf]",
                        FieldId = 71,
                        Field = new FieldInfo() { Id = 71, Type = FieldTypeEnum.FILE },
                        FieldValueFiles = new List<FieldValueFileInfo>(){
                            new FieldValueFileInfo() {
                                FieldValueId = 2,
                                UploadedFieldValueId = 2,
                                Key = "",
                                Name = "",
                                Size = 123,
                                Type = "",
                                CreatedDate = DateTime.Now,
                                CreatedByUserId = 1,
                                TenantId = TenantMock.Id,
                            }
                        }
                    } },
                    { new FieldValueInfo() {
                        Id = 3,
                        FieldValue = "teste",
                        FieldId = 72,
                        Field = new FieldInfo() { Id = 71, Type = FieldTypeEnum.TEXTFIELD }
                    } },
                }
            };

            var taskInfo = new TaskInfo()
            {
                Id = 3,
                FieldsValues = new List<FieldValueInfo>() {
                    { new FieldValueInfo() {
                        Id = 4,
                        FieldValue = "[file.doc]",
                        FieldId = 70,
                        Field = new FieldInfo() { Id = 70, Type = FieldTypeEnum.FILE },
                    } },
                    { new FieldValueInfo() {
                        Id = 5,
                        FieldValue = "[file.pdf]",
                        FieldId = 71,
                        Field = new FieldInfo() { Id = 70, Type = FieldTypeEnum.FILE },
                    } },
                    { new FieldValueInfo() { Id = 4, FieldValue = "teste", FieldId = 72, Field = new FieldInfo() { Id = 71, Type = FieldTypeEnum.TEXTFIELD } } },
                }
            };


            FieldValueFileService fieldValueFileService = new FieldValueFileService(_mockContextDataService.Object, _mockRepository.Object, _mockStorageService.Object, _mockFieldValueService.Object);

            await fieldValueFileService.Unassign(previousTaskInfo, taskInfo);

            _mockRepository.Verify(x => x.Insert(It.IsAny<FieldValueFileInfo>()), Times.Exactly(2));
        }
    }
}
