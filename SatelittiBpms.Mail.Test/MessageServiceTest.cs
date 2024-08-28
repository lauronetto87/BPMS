using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Mail.Services;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Options.Models;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Mail.Tests
{
    public class MessageServiceTest
    {
        Mock<IActivityService> _mockActivityService;
        Mock<ISuiteUserService> _mockSuiteUserService;
        Mock<IRoleService> _mockRoleService;
        Mock<ITenantService> _mockTenantService;
        Mock<IOptions<AwsOptions>> _mockAwsOptions;
        Mock<IWildcardService> _mockWildcardService;
        readonly AwsOptions awsOptions = new()
        {
            SES = new SesOptions
            {
                SenderAddress = "noreply@satelitti.com.br"
            }
        };

        [SetUp]
        public void Setup()
        {
            _mockActivityService = new Mock<IActivityService>();
            _mockSuiteUserService = new Mock<ISuiteUserService>();
            _mockRoleService = new Mock<IRoleService>();
            _mockTenantService = new Mock<ITenantService>();
            _mockAwsOptions = new Mock<IOptions<AwsOptions>>();
            _mockWildcardService = new Mock<IWildcardService>();

            _mockAwsOptions.SetupGet(x => x.Value).Returns(awsOptions);
        }

        [Test]
        public async Task ensureCreateMessageWhenExecutorTypeRequester()
        {
            int tenantId = 483,
                activityId = 4745,
                requesterId = 887,
                taskid = 35;

            var activityInfo = new ActivityInfo()
            {
                ActivityNotification = new ActivityNotificationInfo()
                {
                    TitleMessage = "someTitle",
                    Message = "Some message to send",
                    DestinataryType = SendTaskDestinataryTypeEnum.REQUESTER
                },
                Tasks = new List<TaskInfo>()
                {
                    new TaskInfo
                    {
                        Id = taskid,
                        FieldsValues = new List<FieldValueInfo>()
                        {
                            new FieldValueInfo
                            {
                                FieldValue = "email@teste.com",
                                Field = new FieldInfo
                                {
                                    Name = "Email",
                                    ComponentInternalId = "email"
                                }
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
                }
            };

            var userList = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel() { Id = 887, Mail = "teste@teste.com.br" }
            };

            _mockActivityService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ActivityInfo>(activityInfo, true, null));
            _mockSuiteUserService.Setup(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(userList);
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { SubDomain = "subdomain", AccessKey = "accesskey" });
            _mockWildcardService.Setup(x => x.FormatDescriptionWildcard(activityInfo.ActivityNotification.TitleMessage, It.IsAny<FlowInfo>(), userList)).Returns(activityInfo.ActivityNotification.TitleMessage);
            _mockWildcardService.Setup(x => x.FormatDescriptionWildcard(activityInfo.ActivityNotification.Message, It.IsAny<FlowInfo>(), userList)).Returns(activityInfo.ActivityNotification.Message);

            MessageService messageService = new(_mockActivityService.Object, _mockSuiteUserService.Object, _mockRoleService.Object, _mockTenantService.Object, _mockWildcardService.Object, _mockAwsOptions.Object);
            var result = await messageService.CreateMessage(tenantId, activityId, requesterId, taskid);

            Assert.AreEqual($"iBPMS Satelitti <noreply@satelitti.com.br>", result.Sender);
            Assert.AreEqual(activityInfo.ActivityNotification.TitleMessage, result.Subject);
            Assert.AreEqual(activityInfo.ActivityNotification.Message, result.Body.Html);
            Assert.AreEqual(activityInfo.ActivityNotification.Message, result.Body.Text);
            Assert.AreEqual(1, result.To.Count);
            Assert.AreEqual(userList[0].Mail, result.To[0]);

            _mockActivityService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockSuiteUserService.Verify(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>()), Times.Exactly(1));            
            _mockTenantService.Verify(x => x.Get(It.IsAny<int>()), Times.Exactly(1));
            _mockRoleService.Verify(x => x.Get(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureCreateMessageWhenExecutorTypeRole()
        {
            int tenantId = 483,
              activityId = 4745,
              requesterId = 887,
              taskid = 35;

            var activityInfo = new ActivityInfo()
            {
                ActivityNotification = new ActivityNotificationInfo()
                {
                    TitleMessage = "someTitle",
                    Message = "Some message to send",
                    DestinataryType = SendTaskDestinataryTypeEnum.ROLE,
                    RoleId = 49
                },
                Tasks = new List<TaskInfo>
                {
                    new TaskInfo()
                    {
                        Id = 35
                    }
                }
            };

            var userList = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel() { Id = 43, Mail = "teste@teste.com.br" },
                new SuiteUserViewModel() { Id = 85, Mail = "teste1@teste.com.br" },
                new SuiteUserViewModel() { Id = 7878, Mail = "teste2@teste.com.br" },
            };

            var roleUsers = new List<RoleUserInfo>()
            {
                new RoleUserInfo() { UserId = 43 },
                new RoleUserInfo() { UserId = 85 },
                new RoleUserInfo() { UserId = 7878 }
            };

            _mockActivityService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ActivityInfo>(activityInfo, true, null));
            _mockSuiteUserService.Setup(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(userList);
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { SubDomain = "subdomain", AccessKey = "accesskey" });
            _mockRoleService.Setup(x => x.Get(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<RoleInfo>(new RoleInfo() { RoleUsers = roleUsers }, true, null));
            _mockWildcardService.Setup(x => x.FormatDescriptionWildcard("someTitle", It.IsAny<FlowInfo>(), It.IsAny<IList<SuiteUserViewModel>>())).Returns("someTitle");
            _mockWildcardService.Setup(x => x.FormatDescriptionWildcard("Some message to send", It.IsAny<FlowInfo>(), It.IsAny<IList<SuiteUserViewModel>>())).Returns("Some message to send");

            MessageService messageService = new(_mockActivityService.Object, _mockSuiteUserService.Object, _mockRoleService.Object, _mockTenantService.Object, _mockWildcardService.Object, _mockAwsOptions.Object);
            var result = await messageService.CreateMessage(tenantId, activityId, requesterId, taskid);

            Assert.AreEqual($"iBPMS Satelitti <noreply@satelitti.com.br>", result.Sender);
            Assert.AreEqual(activityInfo.ActivityNotification.TitleMessage, result.Subject);
            Assert.AreEqual(activityInfo.ActivityNotification.Message, result.Body.Html);
            Assert.AreEqual(activityInfo.ActivityNotification.Message, result.Body.Text);
            Assert.AreEqual(roleUsers.Count, result.To.Count);

            userList.ForEach(x => Assert.Contains(x.Mail, result.To));

            _mockActivityService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockSuiteUserService.Verify(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>()), Times.Once());
            _mockTenantService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockRoleService.Verify(x => x.Get(It.Is<int>(y => y == activityInfo.ActivityNotification.RoleId), It.IsAny<int>()), Times.Once());
        }

        [Test]
        public async Task ensureCreateMessageWhenExecutorTypePerson()
        {
            int tenantId = 483;
            int activityId = 4745;
            int requesterId = 887;
            int personId = 887;
            int taskid = 35;

            var activityInfo = new ActivityInfo()
            {
                ActivityNotification = new ActivityNotificationInfo()
                {
                    TitleMessage = "someTitle",
                    Message = "Some message to send",
                    DestinataryType = SendTaskDestinataryTypeEnum.PERSON,
                    PersonId = personId,
                },
                Tasks = new List<TaskInfo>
                {
                    new TaskInfo()
                    {
                        Id = 35
                    }
                }
            };

            var userList = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel()
                {
                    Id = personId,
                    Mail = "teste@teste.com.br",                    
                }
            };

            _mockActivityService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ActivityInfo>(activityInfo, true, null));
            _mockSuiteUserService.Setup(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(userList);
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { SubDomain = "subdomain", AccessKey = "accesskey" });
            _mockWildcardService.Setup(x => x.FormatDescriptionWildcard("someTitle", It.IsAny<FlowInfo>(), It.IsAny<IList<SuiteUserViewModel>>())).Returns("someTitle");
            _mockWildcardService.Setup(x => x.FormatDescriptionWildcard("Some message to send", It.IsAny<FlowInfo>(), It.IsAny<IList<SuiteUserViewModel>>())).Returns("Some message to send");

            MessageService messageService = new(_mockActivityService.Object, _mockSuiteUserService.Object, _mockRoleService.Object, _mockTenantService.Object, _mockWildcardService.Object, _mockAwsOptions.Object);
            var result = await messageService.CreateMessage(tenantId, activityId, requesterId, taskid);

            Assert.AreEqual($"iBPMS Satelitti <noreply@satelitti.com.br>", result.Sender);
            Assert.AreEqual(activityInfo.ActivityNotification.TitleMessage, result.Subject);
            Assert.AreEqual(activityInfo.ActivityNotification.Message, result.Body.Html);
            Assert.AreEqual(activityInfo.ActivityNotification.Message, result.Body.Text);
            Assert.AreEqual(1, result.To.Count);
            Assert.AreEqual(userList[0].Mail, result.To[0]);

            _mockActivityService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockSuiteUserService.Verify(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>()), Times.Once());
            _mockTenantService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockRoleService.Verify(x => x.Get(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }

        [Test]
        public async Task ensureCreateMessageWhenExecutorTypeCustom()
        {
            int tenantId = 483;
            int activityId = 4745;
            int requesterId = 887;
            int personId = 887;
            int taskid = 35;

            var activityInfo = new ActivityInfo()
            {
                ActivityNotification = new ActivityNotificationInfo()
                {
                    TitleMessage = "someTitle",
                    Message = "Some message to send",
                    DestinataryType = SendTaskDestinataryTypeEnum.CUSTOM,
                    PersonId = personId,
                    CustomEmail = "custom@teste.com.br"
                },
                Tasks = new List<TaskInfo>
                {
                    new TaskInfo()
                    {
                        Id = 35
                    }
                }
            };

            var userList = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel()
                {
                    Id = personId,
                    Mail = "teste@teste.com.br",                    
                }
            };

            _mockActivityService.Setup(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>())).ReturnsAsync(new ResultContent<ActivityInfo>(activityInfo, true, null));
            _mockSuiteUserService.Setup(x => x.ListWithoutContext(It.IsAny<SuiteUserListFilter>())).ReturnsAsync(userList);
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { SubDomain = "subdomain", AccessKey = "accesskey" });
            _mockWildcardService.Setup(x => x.FormatDescriptionWildcard("someTitle", It.IsAny<FlowInfo>(), It.IsAny<IList<SuiteUserViewModel>>())).Returns("someTitle");
            _mockWildcardService.Setup(x => x.FormatDescriptionWildcard("Some message to send", It.IsAny<FlowInfo>(), It.IsAny<IList<SuiteUserViewModel>>())).Returns("Some message to send");

            MessageService messageService = new(_mockActivityService.Object, _mockSuiteUserService.Object, _mockRoleService.Object, _mockTenantService.Object, _mockWildcardService.Object, _mockAwsOptions.Object);
            var result = await messageService.CreateMessage(tenantId, activityId, requesterId, taskid);

            Assert.AreEqual($"iBPMS Satelitti <noreply@satelitti.com.br>", result.Sender);
            Assert.AreEqual(activityInfo.ActivityNotification.TitleMessage, result.Subject);
            Assert.AreEqual(activityInfo.ActivityNotification.Message, result.Body.Html);
            Assert.AreEqual(activityInfo.ActivityNotification.Message, result.Body.Text);
            Assert.AreEqual(1, result.To.Count);
            Assert.AreEqual(activityInfo.ActivityNotification.CustomEmail, result.To[0]);

            _mockActivityService.Verify(x => x.GetByTenant(It.IsAny<int>(), It.IsAny<int>()), Times.Once());
            _mockTenantService.Verify(x => x.Get(It.IsAny<int>()), Times.Exactly(1));
            _mockRoleService.Verify(x => x.Get(It.IsAny<int>(), It.IsAny<int>()), Times.Never());
        }
    }
}
