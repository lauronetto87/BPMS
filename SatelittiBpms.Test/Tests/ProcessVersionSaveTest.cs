using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Test.Helpers;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Tests
{
    public class ProcessVersionSaveTest : BaseTest
    {
        MockServices mockServices;
        IQueryable<ProcessVersionInfo> dbSetProcessVersion;
        DbContext dbContext;

        [SetUp]
        public async Task Setup()
        {
            mockServices = new MockServices();
            await mockServices.BuildServiceProvider();
            await mockServices.ActivationTenant();
            dbContext = mockServices.GetService<DbContext>();
            dbSetProcessVersion = dbContext.Set<ProcessVersionInfo>();
        }

        [Test]
        public async Task SaveNewWithOneUserActivityAndExecutorTypeRequesterAndOneField()
        {
            var processVersionId = await mockServices.NewProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField();

            // Limpa o rastreamento porque a thread do motor atualiza os dados e ao efetuar a busca era pego do cache
            // Uma outra opção é colocar o .AsNoTracking(), mas ocorre o seguinte erro em alguns casos:
            // The Include path 'SourceTasks->TargetTask' results in a cycle. Cycles are not allowed in no-tracking queries; either use a tracking query or remove the cycle.
            dbContext.ChangeTracker.Clear();

            var processVersionInfo = dbSetProcessVersion
                .Include(pv => pv.Process)
                .Include(pv => pv.Activities).ThenInclude(ac => ac.ActivityNotification)
                .Include(pv => pv.Activities).ThenInclude(ac => ac.ActivityFields).ThenInclude(af => af.Field)
                .Include(pv => pv.Activities).ThenInclude(ac => ac.ActivityUser).ThenInclude(au => au.ActivityUsersOptions)
                .Include(pv => pv.Fields)
                .First(x => x.TenantId == mockServices.ContextData.Tenant.Id && x.Id == processVersionId);

            Assert.AreEqual(processVersionInfo.Process.CurrentVersion, 1);

            var processDto = ProcessVersionCreator.ProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneField;

            var currentUserSuite = await mockServices.GetUserSuite(mockServices.ContextData.User.Id);

            Assert.AreEqual(processVersionInfo.Version, 1);
            Assert.AreEqual(processVersionInfo.Name, processDto.Name);
            Assert.AreEqual(processVersionInfo.Description, processDto.Description);
            Assert.AreEqual(processVersionInfo.DescriptionFlow, processDto.DescriptionFlow);
            Assert.AreEqual(processVersionInfo.CreatedByUserName, currentUserSuite.Name);
            Assert.AreEqual(processVersionInfo.CreatedByUserId, mockServices.ContextData.User.Id);
            Assert.AreEqual(processVersionInfo.Status, ProcessStatusEnum.PUBLISHED);
            Assert.AreEqual(processVersionInfo.DiagramContent, processDto.DiagramContent);
            Assert.AreEqual(processVersionInfo.FormContent, processDto.FormContent);
            Assert.IsNotNull(processVersionInfo.WorkflowContent);
            Assert.AreNotEqual(processVersionInfo.WorkflowContent, "");

            var activities = processVersionInfo.Activities;
            Assert.AreEqual(activities.Count, 4);

            var activityUser = activities.FirstOrDefault(a => a.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY && a.ActivityUser == null);
            Assert.IsNotNull(activityUser);
            Assert.IsNotNull(activityUser.ActivityFields);
            Assert.AreEqual(activityUser.ActivityFields.Count, 1);
            Assert.AreEqual(activityUser.ActivityFields[0].State, ProcessTaskFieldStateEnum.EDITABLE);
            Assert.AreEqual(activityUser.ActivityFields[0].TenantId, mockServices.ContextData.Tenant.Id);

            var textFieldActivity = activityUser.ActivityFields[0].Field;
            Assert.AreEqual(textFieldActivity.ComponentInternalId, "textField");
            Assert.AreEqual(textFieldActivity.Name, "Campo 1");
            Assert.AreEqual(textFieldActivity.ProcessVersionId, processVersionId);
            Assert.AreEqual(textFieldActivity.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(textFieldActivity.Type, FieldTypeEnum.TEXTFIELD);
            Assert.AreEqual(textFieldActivity.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.IsNull(activityUser.ActivityNotification);

            activityUser = activities.FirstOrDefault(a => a.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY && a.ActivityUser != null);
            Assert.IsNotNull(activityUser.ActivityUser);
            Assert.IsNotNull(activityUser.ActivityUser.ActivityUsersOptions);
            Assert.AreEqual(activityUser.ActivityUser.ActivityUsersOptions.Count, 1);
            var userOptions = activityUser.ActivityUser.ActivityUsersOptions[0];
            Assert.AreEqual(userOptions.Description, "Concluir");
            Assert.AreEqual(userOptions.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(activityUser.ActivityUser.ExecutorType, UserTaskExecutorTypeEnum.REQUESTER);
            Assert.IsNull(activityUser.ActivityUser.PersonId);
            Assert.IsNull(activityUser.ActivityUser.RoleId);
            Assert.AreEqual(activityUser.ActivityUser.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(activityUser.Name, processDto.Activities[0].ActivityName);
            Assert.AreEqual(activityUser.ProcessVersionId, processVersionId);
            Assert.IsNull(activityUser.Tasks);

            var activityStart = activities.FirstOrDefault(a => a.Type == WorkflowActivityTypeEnum.START_EVENT_ACTIVITY);
            Assert.IsNotNull(activityStart);
            Assert.True(activityStart.ActivityFields == null || activityStart.ActivityFields.Count == 0);
            Assert.IsNull(activityStart.ActivityNotification);
            Assert.IsNull(activityStart.ActivityUser);
            Assert.AreEqual(activityStart.ComponentInternalId, "StartEvent_1");
            Assert.Greater(activityStart.Id, 0);
            Assert.IsNull(activityStart.Name);
            Assert.AreEqual(activityStart.ProcessVersionId, processVersionId);
            Assert.IsNull(activityStart.Tasks);
            Assert.AreEqual(activityStart.TenantId, mockServices.ContextData.Tenant.Id);

            var activityEnd = activities.FirstOrDefault(a => a.Type == WorkflowActivityTypeEnum.END_EVENT_ACTIVITY);
            Assert.IsNotNull(activityEnd);
            Assert.True(activityStart.ActivityFields == null || activityStart.ActivityFields.Count == 0);
            Assert.IsNull(activityEnd.ActivityNotification);
            Assert.IsNull(activityEnd.ActivityUser);
            Assert.AreEqual(activityEnd.ComponentInternalId, "Event_0qoynwl");
            Assert.Greater(activityEnd.Id, 0);
            Assert.IsNull(activityEnd.Name);
            Assert.AreEqual(activityEnd.ProcessVersionId, processVersionId);
            Assert.IsNull(activityEnd.Tasks);
            Assert.AreEqual(activityEnd.TenantId, mockServices.ContextData.Tenant.Id);

            Assert.IsNotNull(processVersionInfo.Fields);
            Assert.AreEqual(processVersionInfo.Fields.Count, 1);
            var textFieldProcessVersion = processVersionInfo.Fields[0];
            Assert.AreEqual(textFieldProcessVersion.Id, textFieldActivity.Id);
        }

        [Test]
        public async Task SaveNewWithOneUserActivityAndExecutorTypeRequesterAndOneFieldAndErrorAtDiagram()
        {
            var processVersionId = await mockServices.NewProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneFieldAndErrorAtDiagram();

            // Limpa o rastreamento porque a thread do motor atualiza os dados e ao efetuar a busca era pego do cache
            // Uma outra opção é colocar o .AsNoTracking(), mas ocorre o seguinte erro em alguns casos:
            // The Include path 'SourceTasks->TargetTask' results in a cycle. Cycles are not allowed in no-tracking queries; either use a tracking query or remove the cycle.
            dbContext.ChangeTracker.Clear();
            var processVersionInfo = dbSetProcessVersion
                .Include(pv => pv.Process)
                .Include(pv => pv.Activities).ThenInclude(ac => ac.ActivityNotification)
                .Include(pv => pv.Activities).ThenInclude(ac => ac.ActivityFields).ThenInclude(af => af.Field)
                .Include(pv => pv.Activities).ThenInclude(ac => ac.ActivityUser).ThenInclude(au => au.ActivityUsersOptions)
                .Include(pv => pv.Fields)
                .First(x => x.TenantId == mockServices.ContextData.Tenant.Id && x.Id == processVersionId);

            Assert.Null(processVersionInfo.Process.CurrentVersion);

            var processDto = ProcessVersionCreator.ProcessVersionWithOneUserActivityWithExecutorTypeRequesterAndOneFieldAndErrorAtDiagram;

            var currentUserSuite = await mockServices.GetUserSuite(mockServices.ContextData.User.Id);

            Assert.AreEqual(processVersionInfo.Version, 1);
            Assert.AreEqual(processVersionInfo.Name, processDto.Name);
            Assert.AreEqual(processVersionInfo.Description, processDto.Description);
            Assert.AreEqual(processVersionInfo.DescriptionFlow, processDto.DescriptionFlow);
            AssertDateEqualNowWithDelay(processVersionInfo.CreatedDate);
            Assert.AreEqual(processVersionInfo.CreatedByUserName, currentUserSuite.Name);
            Assert.AreEqual(processVersionInfo.CreatedByUserId, mockServices.ContextData.User.Id);
            Assert.AreEqual(processVersionInfo.Status, ProcessStatusEnum.EDITING);
            AssertDateEqualNowWithDelay(processVersionInfo.LastModifiedDate);
            Assert.IsNull(processVersionInfo.PublishedDate);
            Assert.AreEqual(processVersionInfo.DiagramContent, processDto.DiagramContent);
            Assert.AreEqual(processVersionInfo.FormContent, processDto.FormContent);
            Assert.IsNull(processVersionInfo.WorkflowContent);

            var activities = processVersionInfo.Activities;
            Assert.AreEqual(activities.Count, 1);

            var activityUser = activities.FirstOrDefault(a => a.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY);
            Assert.IsNotNull(activityUser);
            Assert.IsNotNull(activityUser.ActivityFields);
            Assert.AreEqual(activityUser.ActivityFields.Count, 1);
            Assert.AreEqual(activityUser.ActivityFields[0].State, ProcessTaskFieldStateEnum.EDITABLE);
            Assert.AreEqual(activityUser.ActivityFields[0].TenantId, mockServices.ContextData.Tenant.Id);
            var textFieldActivity = activityUser.ActivityFields[0].Field;
            Assert.AreEqual(textFieldActivity.ComponentInternalId, "textField");
            Assert.AreEqual(textFieldActivity.Name, "Campo 1");
            Assert.AreEqual(textFieldActivity.ProcessVersionId, processVersionId);
            Assert.AreEqual(textFieldActivity.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.AreEqual(textFieldActivity.Type, FieldTypeEnum.TEXTFIELD);
            Assert.AreEqual(textFieldActivity.TenantId, mockServices.ContextData.Tenant.Id);
            Assert.IsNull(activityUser.ActivityNotification);
            Assert.IsNull(activityUser.ActivityUser);
            Assert.AreEqual(activityUser.ComponentInternalId, processDto.Activities[0].ActivityId);
            Assert.Greater(activityUser.Id, 0);
            Assert.AreEqual(activityUser.Name, processDto.Activities[0].ActivityName);
            Assert.AreEqual(activityUser.ProcessVersionId, processVersionId);
            Assert.IsNull(activityUser.Tasks);

            var activityStart = activities.FirstOrDefault(a => a.Type == WorkflowActivityTypeEnum.START_EVENT_ACTIVITY);
            Assert.IsNull(activityStart);

            var activityEnd = activities.FirstOrDefault(a => a.Type == WorkflowActivityTypeEnum.END_EVENT_ACTIVITY);
            Assert.IsNull(activityEnd);

            Assert.IsNotNull(processVersionInfo.Fields);
            Assert.AreEqual(processVersionInfo.Fields.Count, 1);
            var textFieldProcessVersion = processVersionInfo.Fields[0];
            Assert.AreEqual(textFieldProcessVersion.Id, textFieldActivity.Id);
        }
    }
}
