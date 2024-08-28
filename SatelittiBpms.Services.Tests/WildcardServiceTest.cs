using Moq;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Translate.Interfaces;
using System;
using System.Collections.Generic;

namespace SatelittiBpms.Services.Tests
{
    public class WildcardServiceTest
    {
        Mock<ITranslateService> _mockTranslateService;

        [SetUp]
        public void Init()
        {
            _mockTranslateService = new Mock<ITranslateService>();
        }

        [Test]
        public void EnsureThatSwapWildcardTaskReturnFilledSystemFields()
        {
            var taskinfo = new TaskInfo()
            {
                Id = 4,
                ActivityId = 1,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                Flow = new FlowInfo()
                {
                    Id = 3,
                    CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "Process name1",
                        DescriptionFlow = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n",
                        CreatedByUserId = 1,
                        CreatedByUserName = "Created UserName",
                        Status = ProcessStatusEnum.PUBLISHED
                    },
                    Tasks = new List<TaskInfo>()
                    {
                        new TaskInfo
                        {
                            FieldsValues = new List<FieldValueInfo>()
                        }
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
            };

            var users = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel()
                {
                    Name = "Nome 1",
                }
            };

            var descriptionFlow = InstantiateWildcardService().FormatDescriptionWildcard(taskinfo.Flow.ProcessVersion.DescriptionFlow, taskinfo.Flow, users);

            Assert.IsNotNull(descriptionFlow);
            Assert.AreEqual(descriptionFlow, "Numero do fluxo: 3 e solicitante: Nome 1 Process: Process name1 Data Solicitacao: " + new DateTime(2021, 12, 15, 16, 30, 00) + " Email: \n");

        }

        [Test]
        public void EnsureThatSwapWildcardTaskReturnAllFieldsFilledIn()
        {
            var formContentObj = new JObject()
            {
                {
                    "components", new JArray()
                    {
                        new JObject
                        {
                            { "key", "email" },
                            { "type", "textbox" },
                            { "input", true },
                        }
                    }
                },
            };

            var processVersion = new ProcessVersionInfo()
            {
                Name = "Process name1",
                DescriptionFlow = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
                FormContent = formContentObj.ToString(),
            };


            var activity = new ActivityInfo()
            {
                Id = 1,
                Name = "Atividade 1",
                ProcessVersion = processVersion,
                ActivityUser = new ActivityUserInfo()
                {
                    ExecutorType = UserTaskExecutorTypeEnum.ROLE
                },
            };

            var taskinfo = new TaskInfo
            {
                Id = 4,
                ActivityId = 1,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                Activity = activity,
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
                }
            };

            var flow = new FlowInfo
            {
                Id = 3,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                ProcessVersion = processVersion,
                Tasks = new List<TaskInfo>
                {
                    taskinfo
                }
            };
            taskinfo.Flow = flow;
            taskinfo.FlowId = flow.Id;

            var users = new List<SuiteUserViewModel>
            {
                new SuiteUserViewModel
                {
                    Name = "Nome 1",
                }
            };


            var descriptionFlow = InstantiateWildcardService().FormatDescriptionWildcard(taskinfo.Flow.ProcessVersion.DescriptionFlow, taskinfo.Flow, users);

            Assert.IsNotNull(descriptionFlow);
            Assert.AreEqual(descriptionFlow, "Numero do fluxo: 3 e solicitante: Nome 1 Process: Process name1 Data Solicitacao: " + new DateTime(2021, 12, 15, 16, 30, 00) + " Email: email@teste.com\n");

        }

        [Test]
        public void EnsureThatSwapWildcardFlowReturnFilledSystemFields()
        {
            var userLoggedId = 1;
            var flowInfo = new FlowInfo()
            {
                Id = 3,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                RequesterId = 1,
                ProcessVersion = new ProcessVersionInfo()
                {
                    Name = "Process name1",
                    DescriptionFlow = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n",
                    CreatedByUserId = 1,
                    CreatedByUserName = "Created UserName",
                    Status = ProcessStatusEnum.PUBLISHED
                },
                Tasks = new List<TaskInfo>()
                {
                    new TaskInfo
                    {
                        FieldsValues = new List<FieldValueInfo>(),
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

            var users = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel()
                {
                    Id = userLoggedId,
                    Name = "Nome 1",
                },
                new SuiteUserViewModel()
                {
                    Id = 2,
                    Name = "Nome 2",
                }
            };


            var descriptionFlow = InstantiateWildcardService().FormatDescriptionWildcard(flowInfo.ProcessVersion.DescriptionFlow, flowInfo, users);

            Assert.IsNotNull(descriptionFlow);
            Assert.AreEqual(descriptionFlow, "Numero do fluxo: 3 e solicitante: Nome 1 Process: Process name1 Data Solicitacao: " + new DateTime(2021, 12, 15, 16, 30, 00) + " Email: \n");

        }

        [Test]
        public void EnsureThatSwapWildcardFlowReturnAllFieldsFilledIn()
        {
            var formContentObj = new JObject()
            {
                {
                    "components", new JArray()
                    {
                        new JObject
                        {
                            { "key", "email" },
                            { "type", "textbox" },
                            { "input", true },
                        }
                    }
                },
            };

            var processVersion = new ProcessVersionInfo()
            {
                Name = "Process name1",
                DescriptionFlow = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
                FormContent = formContentObj.ToString(),
            };

            var taskInfo = new TaskInfo
            {
                FieldsValues = new List<FieldValueInfo>
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
                    ProcessVersion = processVersion,
                    ActivityUser = new ActivityUserInfo()
                    {
                        ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                    }
                }
            };

            var userLoggedId = 1;
            var flowInfo = new FlowInfo()
            {
                Id = 3,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                RequesterId = 1,
                ProcessVersion = processVersion,
                Tasks = new List<TaskInfo>()
                {
                    taskInfo
                }
            };
            taskInfo.Flow = flowInfo;
            taskInfo.FlowId = flowInfo.Id;

            var users = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel()
                {
                    Id = userLoggedId,
                    Name = "Nome 1",
                },
                new SuiteUserViewModel()
                {
                    Id = 2,
                    Name = "Nome 2",
                }
            };


            var descriptionFlow = InstantiateWildcardService().FormatDescriptionWildcard(flowInfo.ProcessVersion.DescriptionFlow, flowInfo, users);

            Assert.IsNotNull(descriptionFlow);
            Assert.AreEqual(descriptionFlow, "Numero do fluxo: 3 e solicitante: Nome 1 Process: Process name1 Data Solicitacao: " + new DateTime(2021, 12, 15, 16, 30, 00) + " Email: email@teste.com\n");
        }

        [Test]
        public void EnsureThatSwapWildcardTaskDetailReturnFilledSystemFields()
        {
            var flowInfo = new FlowInfo()
            {
                Id = 3,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                ProcessVersion = new ProcessVersionInfo()
                {
                    Name = "Process name1",
                    DescriptionFlow = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n",
                    CreatedByUserId = 1,
                    CreatedByUserName = "Created UserName",
                    Status = ProcessStatusEnum.PUBLISHED,
                    Activities = new List<ActivityInfo>()
                    {
                        new ActivityInfo()
                        {
                            Id = 1,
                            Name = "Atividade 1",
                            ActivityUser = new ActivityUserInfo()
                            {
                                ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                            }
                        }
                    }
                },
                Tasks = new List<TaskInfo>()
                {
                    new TaskInfo
                    {
                        FieldsValues = new List<FieldValueInfo>()
                    }
                }
            };

            var users = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel()
                {
                    Name = "Nome 1",
                }
            };


            var descriptionFlow = InstantiateWildcardService().FormatDescriptionWildcard(flowInfo.ProcessVersion.DescriptionFlow, flowInfo, users);

            Assert.IsNotNull(descriptionFlow);
            Assert.AreEqual(descriptionFlow, "Numero do fluxo: 3 e solicitante: Nome 1 Process: Process name1 Data Solicitacao: " + new DateTime(2021, 12, 15, 16, 30, 00) + " Email: \n");

        }

        [Test]
        public void EnsureThatSwapWildcardTaskDetailReturnAllFieldsFilledIn()
        {
            var formContentObj = new JObject()
            {
                {
                    "components", new JArray()
                    {
                        new JObject
                        {
                            { "key", "email" },
                            { "type", "textbox" },
                            { "input", true },
                        }
                    }
                },
            };

            var activity = new ActivityInfo()
            {
                Id = 1,
                Name = "Atividade 1",
                ActivityUser = new ActivityUserInfo()
                {
                    ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                }
            };

            var processVersion = new ProcessVersionInfo()
            {
                Name = "Process name1",
                DescriptionFlow = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] Teste Fluxo Repetido: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n",
                CreatedByUserId = 1,
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
                FormContent = formContentObj.ToString(),
                Activities = new List<ActivityInfo>()
                {
                    activity
                }
            };

            activity.ProcessVersion = processVersion;

            var taskinfo = new TaskInfo
            {
                Id = 4,
                ActivityId = 1,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                Activity = activity,
                FieldsValues = new List<FieldValueInfo>
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
            };

            var flow = new FlowInfo
            {
                Id = 3,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                ProcessVersion = processVersion,
                Tasks = new List<TaskInfo>
                {
                    taskinfo
                }
            };
            taskinfo.Flow = flow;
            taskinfo.FlowId = flow.Id;

            var users = new List<SuiteUserViewModel>
            {
                new SuiteUserViewModel
                {
                    Name = "Nome 1",
                }
            };

            var descriptionFlow = InstantiateWildcardService().FormatDescriptionWildcard(taskinfo.Flow.ProcessVersion.DescriptionFlow, taskinfo.Flow, users);

            Assert.IsNotNull(descriptionFlow);
            Assert.AreEqual(descriptionFlow, "Numero do fluxo: 3 Teste Fluxo Repetido: 3 e solicitante: Nome 1 Process: Process name1 Data Solicitacao: " + new DateTime(2021, 12, 15, 16, 30, 00) + " Email: email@teste.com\n");

        }


        [Test]
        public void EnsureThatFormatNotificationReturnFilledSystemFields()
        {
            var description = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n";

            var flowInfo = new FlowInfo()
            {
                Id = 3,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                ProcessVersion = new ProcessVersionInfo()
                {
                    Name = "Process name1",
                    DescriptionFlow = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n",
                    CreatedByUserId = 1,
                    CreatedByUserName = "Created UserName",
                    Status = ProcessStatusEnum.PUBLISHED,
                    Activities = new List<ActivityInfo>()
                    {
                        new ActivityInfo()
                        {
                            Id = 1,
                            Name = "Atividade 1",
                            ActivityUser = new ActivityUserInfo()
                            {
                                ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                            }
                        }
                    }
                },
                Tasks = new List<TaskInfo>()
                {
                    new TaskInfo
                    {
                        FieldsValues = new List<FieldValueInfo>()
                    }
                }
            };

            var users = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel()
                {
                    Name = "Nome 1",
                }
            };

            WildcardService wildcardService = InstantiateWildcardService();
            var descriptionFormat = wildcardService.FormatDescriptionWildcard(description, flowInfo, users);

            Assert.IsNotNull(descriptionFormat);
            Assert.AreEqual(descriptionFormat, "Numero do fluxo: 3 e solicitante: Nome 1 Process: Process name1 Data Solicitacao: " + new DateTime(2021, 12, 15, 16, 30, 00) + " Email: \n");

        }

        [Test]
        public void EnsureThatFormatNotificationReturnAllFieldsFilledIn()
        {
            var description = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n";

            var activity = new ActivityInfo()
            {
                Id = 1,
                Name = "Atividade 1",
                ActivityUser = new ActivityUserInfo()
                {
                    ExecutorType = UserTaskExecutorTypeEnum.REQUESTER
                }
            };

            var formContentObj = new JObject()
            {
                {
                    "components", new JArray()
                    {
                        new JObject
                        {
                            { "key", "email" },
                            { "type", "textbox" },
                            { "input", true },
                        }
                    }
                },
            };

            var processVersion = new ProcessVersionInfo()
            {
                Name = "Process name1",
                DescriptionFlow = "Numero do fluxo: [[{\"text\":\"Numero Fluxo\",\"value\":\"wildcards.flowNumber\",\"prefix\":\"#\"}]] e solicitante: [[{\"text\":\"Solicitante\",\"value\":\"wildcards.requester\",\"prefix\":\"#\"}]] Process: [[{\"text\":\"Processo\",\"value\":\"wildcards.process\",\"prefix\":\"#\"}]] Data Solicitacao: [[{\"text\":\"Data Solicitacao\",\"value\":\"wildcards.requestDate\",\"prefix\":\"#\"}]] Email: [[{\"text\":\"Email\",\"value\":\"email\",\"prefix\":\"@\"}]]\n",
                CreatedByUserId = 1,
                FormContent = formContentObj.ToString(),
                CreatedByUserName = "Created UserName",
                Status = ProcessStatusEnum.PUBLISHED,
                Activities = new List<ActivityInfo>()
                {
                    activity
                }
            };
            activity.ProcessVersion = processVersion;


            var taskInfo = new TaskInfo
            {
                Id = 4,
                ActivityId = 1,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                Activity = activity,
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
            };

            var flow = new FlowInfo()
            {
                Id = 3,
                CreatedDate = new DateTime(2021, 12, 15, 16, 30, 00),
                ProcessVersion = processVersion,
                Tasks = new List<TaskInfo>
                {
                    taskInfo
                }
            };
            taskInfo.Flow = flow;
            taskInfo.FlowId = flow.Id;

            var users = new List<SuiteUserViewModel>()
            {
                new SuiteUserViewModel()
                {
                    Name = "Nome 1",
                }
            };

            WildcardService wildcardService = InstantiateWildcardService();
            var descriptionFormat = wildcardService.FormatDescriptionWildcard(description, taskInfo.Flow, users);

            Assert.IsNotNull(descriptionFormat);
            Assert.AreEqual(descriptionFormat, "Numero do fluxo: 3 e solicitante: Nome 1 Process: Process name1 Data Solicitacao: " + new DateTime(2021, 12, 15, 16, 30, 00) + " Email: email@teste.com\n");

        }


        private WildcardService InstantiateWildcardService()
        {
            return new(_mockTranslateService.Object);
        }
    }
}
