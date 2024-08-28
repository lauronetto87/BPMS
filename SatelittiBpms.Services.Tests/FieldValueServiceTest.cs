using AutoMapper;
using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class FieldValueServiceTest
    {
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<IFieldValueRepository> _mockRepository;
        Mock<IMapper> _mockMapper;
        Mock<ITaskRepository> _mockTaskRepository;
        Mock<ITaskSignerRepository> _mockTaskSignerRepository;
         
        public UserInfo UserMock { get; set; }
        public SuiteTenantAuth TenantMock { get; set; }

        [SetUp]
        public void Setup()
        {
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockRepository = new Mock<IFieldValueRepository>();
            _mockMapper = new Mock<IMapper>();
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockTaskSignerRepository = new Mock<ITaskSignerRepository>();

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
        public void ensureConstructor()
        {

            FieldValueService fieldService = new FieldValueService(_mockContextDataService.Object, _mockRepository.Object, _mockTaskRepository.Object, _mockTaskSignerRepository.Object);
            Assert.IsNotNull(fieldService);
        }

        [Test]
        public async Task ensureThatReplicateFieldValuesToNextTask()
        {
            var taskId = 1;
            var nextTaskId = 1;
            var flowId = 1;

            var taskInfo = new TaskInfo();
            var fieldValues = new List<FieldValueInfo>();

            fieldValues.Add(new FieldValueInfo()
            {
                Id = 1,
                FieldId = 1,
                FieldValue = "Valor do campo 1 sem alteração",
                FlowId = flowId,
                TaskId = taskId,
                TenantId = 55,
            });
            fieldValues.Add(new FieldValueInfo()
            {
                Id = 2,
                FieldId = 2,
                FieldValue = "Valor do campo 2 sem alteração",
                FlowId = flowId,
                TaskId = taskId,
                TenantId = 55,
            });

            taskInfo.FieldsValues = fieldValues;

            _mockTaskRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id)).ReturnsAsync(taskInfo);

            FieldValueService fieldService = new FieldValueService(_mockContextDataService.Object, _mockRepository.Object, _mockTaskRepository.Object, _mockTaskSignerRepository.Object);

            await fieldService.ReplicateFieldValues(taskId, nextTaskId, TenantMock.Id);

            _mockRepository.Verify(x => x.Insert(It.IsAny<FieldValueInfo>()), Times.Exactly(2));
        }

        [Test]
        public void ensureThatGetTaskFormFormatedDataWhenHasNotFieldValues()
        {
            var taskWithDependencies = new List<FieldValueInfo>();
            dynamic expectedFormatedFormData = new System.Dynamic.ExpandoObject();

            FieldValueService fieldValueService = new FieldValueService(_mockContextDataService.Object, _mockRepository.Object, _mockTaskRepository.Object, _mockTaskSignerRepository.Object);
            dynamic formatedFormData = fieldValueService.GetFormatedFormData(taskWithDependencies);

            Assert.AreEqual(expectedFormatedFormData, formatedFormData);
        }

        [Test]
        public void ensureThatGetTaskFormFormatedDataWhenHasFieldValues()
        {
            var taskWithDependencies = new List<FieldValueInfo>();
            dynamic expectedFormatedFormData = new System.Dynamic.ExpandoObject();

            ((IDictionary<String, Object>)expectedFormatedFormData).Add("form", "Valor do campo 1");
            ((IDictionary<String, Object>)expectedFormatedFormData).Add("form1", "Valor do campo 2");
            ((IDictionary<String, Object>)expectedFormatedFormData).Add("form2", "Valor do campo 3");
            ((IDictionary<String, Object>)expectedFormatedFormData).Add("form3", "2021-09-16T03:42:00-03:00");
            ((IDictionary<String, Object>)expectedFormatedFormData).Add("form4", false);
            ((IDictionary<String, Object>)expectedFormatedFormData).Add("form5", new JArray("a", "c"));

            taskWithDependencies.Add(new FieldValueInfo() { FieldValue = "Valor do campo 1", Field = new FieldInfo() { ComponentInternalId = "form" } });
            taskWithDependencies.Add(new FieldValueInfo() { FieldValue = "Valor do campo 2", Field = new FieldInfo() { ComponentInternalId = "form1" } });
            taskWithDependencies.Add(new FieldValueInfo() { FieldValue = "Valor do campo 3", Field = new FieldInfo() { ComponentInternalId = "form2" } });
            taskWithDependencies.Add(new FieldValueInfo() { FieldValue = "2021-09-16T03:42:00-03:00", Field = new FieldInfo() { ComponentInternalId = "form3" } });
            taskWithDependencies.Add(new FieldValueInfo() { FieldValue = "False", Field = new FieldInfo() { ComponentInternalId = "form4" } });
            taskWithDependencies.Add(new FieldValueInfo() { FieldValue = "[\"a\", \"c\"]", Field = new FieldInfo() { ComponentInternalId = "form5" } });

            FieldValueService fieldValueService = new FieldValueService(_mockContextDataService.Object, _mockRepository.Object, _mockTaskRepository.Object, _mockTaskSignerRepository.Object);
            dynamic formatedFormData = fieldValueService.GetFormatedFormData(taskWithDependencies);

            Assert.AreEqual(expectedFormatedFormData, formatedFormData);
        }

        [Test]
        public async Task ensureThatAddOrUpdateFieldValuesAsNecessary()
        {
            var taskId = 1;
            var flowId = 1;

            var taskInfo = new TaskInfo();
            var fieldValues = new List<FieldValueInfo>();
            var flow = new FlowInfo();
            var processVersion = new ProcessVersionInfo();
            var fields = new List<FieldInfo>();
            var activityFields = new List<ActivityFieldInfo>();

            fieldValues.Add(new FieldValueInfo()
            {
                Id = 1,
                FieldId = 1,
                FieldValue = "Valor do campo 1 sem alteração",
                FlowId = flowId,
                TaskId = taskId,
                TenantId = 55,
            });
            fieldValues.Add(new FieldValueInfo()
            {
                Id = 2,
                FieldId = 2,
                FieldValue = "Valor do campo 2 sem alteração",
                FlowId = flowId,
                TaskId = taskId,
                TenantId = 55,
            });

            activityFields.Add(new ActivityFieldInfo() { State = ProcessTaskFieldStateEnum.EDITABLE, Field = new FieldInfo() { ComponentInternalId = "form" } });
            activityFields.Add(new ActivityFieldInfo() { State = ProcessTaskFieldStateEnum.EDITABLE, Field = new FieldInfo() { ComponentInternalId = "form1" } });
            activityFields.Add(new ActivityFieldInfo() { State = ProcessTaskFieldStateEnum.EDITABLE, Field = new FieldInfo() { ComponentInternalId = "form2" } });

            fields.Add(new FieldInfo()
            {
                Id = 1,
                ComponentInternalId = "form",
                ActivityFields = activityFields
            });
            fields.Add(new FieldInfo()
            {
                Id = 2,
                ComponentInternalId = "form1",
                ActivityFields = activityFields
            });
            fields.Add(new FieldInfo()
            {
                Id = 3,
                ComponentInternalId = "form2",
                ActivityFields = activityFields
            });

            taskInfo.Id = taskId;
            taskInfo.FlowId = flowId;
            taskInfo.FieldsValues = fieldValues;
            processVersion.Fields = fields;
            flow.ProcessVersion = processVersion;
            taskInfo.Flow = flow;

            _mockTaskRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id)).ReturnsAsync(taskInfo);

            dynamic dynamicFieldValues = new System.Dynamic.ExpandoObject();
            ((IDictionary<String, Object>)dynamicFieldValues).Add("form", "Valor do campo 1");
            ((IDictionary<String, Object>)dynamicFieldValues).Add("form1", "Valor do campo 2");
            ((IDictionary<String, Object>)dynamicFieldValues).Add("form2", "Valor do campo 3");

            FieldValueService fieldValueService = new FieldValueService(_mockContextDataService.Object, _mockRepository.Object, _mockTaskRepository.Object, _mockTaskSignerRepository.Object);
            await fieldValueService.UpdateFieldValues(taskId, dynamicFieldValues);

            _mockRepository.Verify(x => x.Update(It.IsAny<FieldValueInfo>()), Times.Exactly(2));
            _mockRepository.Verify(x => x.Insert(It.IsAny<FieldValueInfo>()), Times.Once());
        }

        [Test]
        public async Task ensureThatAddOrUpdateNotFieldValuesAsNecessary()
        {
            var taskId = 1;
            var flowId = 1;

            var taskInfo = new TaskInfo();
            var fieldValues = new List<FieldValueInfo>();
            var flow = new FlowInfo();
            var processVersion = new ProcessVersionInfo();
            var fields = new List<FieldInfo>();
            var activityFields = new List<ActivityFieldInfo>();

            taskInfo.Id = taskId;
            taskInfo.FlowId = flowId;
            taskInfo.FieldsValues = fieldValues;
            processVersion.Fields = fields;
            flow.ProcessVersion = processVersion;
            taskInfo.Flow = flow;

            _mockTaskRepository.Setup(x => x.GetByIdAndTenantId(It.Is<int>(p => p == taskId), TenantMock.Id)).ReturnsAsync(taskInfo);

            dynamic dynamicFieldValues = new System.Dynamic.ExpandoObject();

            FieldValueService fieldValueService = new FieldValueService(_mockContextDataService.Object, _mockRepository.Object, _mockTaskRepository.Object, _mockTaskSignerRepository.Object);
            await fieldValueService.UpdateFieldValues(taskId, dynamicFieldValues);

            _mockRepository.Verify(x => x.Update(It.IsAny<FieldValueInfo>()), Times.Exactly(0));
            _mockRepository.Verify(x => x.Insert(It.IsAny<FieldValueInfo>()), Times.Exactly(0));
        }


    }
}
