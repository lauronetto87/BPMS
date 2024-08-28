using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using System.Collections.Generic;
using System.Linq;

namespace SatelittiBpms.Models.Tests
{
    public class ProcessVersionInfoTest
    {
        [Test]
        public void ensureThatProcessVersionEditViewModel()
        {
            ProcessVersionInfo info = new ProcessVersionInfo()
            {
                Id = 22,
                ProcessId = 3,
                Process = new ProcessInfo()
                {
                    Id = 3,
                    TaskSequance = 5
                },
                Name = "Some Name",
                Description = "Some process description",
                FormContent = "Any JSON form content",
                DiagramContent = "Some XML Diagram Content",
                ProcessVersionRoles = new List<ProcessVersionRoleInfo>() {
                    new ProcessVersionRoleInfo() {
                        RoleId = 3,
                        Role = new RoleInfo() {
                            Name = "role 1"
                        }
                    },
                    new ProcessVersionRoleInfo() {
                        RoleId = 5,
                        Role = new RoleInfo(){
                            Name= "role 2"
                        }
                    }
                },
                Activities = new List<ActivityInfo>()
                {
                    new ActivityInfo(){
                        ComponentInternalId = "idiaid12",
                        Name = "task 1",
                        Type = Enums.WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                        ActivityFields = new    List<ActivityFieldInfo>(){
                            new ActivityFieldInfo() {
                                FieldId = 1,
                                State = Enums.ProcessTaskFieldStateEnum.ONLYREADING,
                                Field = new FieldInfo() {
                                    ComponentInternalId = "peutb55",
                                    Name = "name 1"
                                }
                            }
                        },
                        ActivityUser = new ActivityUserInfo()
                    },
                    new ActivityInfo(){
                        ComponentInternalId = "cpdjsi45",
                        Name = "task 2",
                        Type = Enums.WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                        ActivityFields = new    List<ActivityFieldInfo>(){
                            new ActivityFieldInfo() {
                                FieldId = 2,
                                State = Enums.ProcessTaskFieldStateEnum.EDITABLE,
                                Field = new FieldInfo() {
                                    ComponentInternalId = "jgihnbc89",
                                    Name = "name 2"
                                }
                            }
                        },
                        ActivityUser = new ActivityUserInfo()
                    },
                    new ActivityInfo(){
                        ComponentInternalId = "oepdj87",
                        Name = "task 3",
                        Type = Enums.WorkflowActivityTypeEnum.USER_TASK_ACTIVITY,
                        ActivityFields = new    List<ActivityFieldInfo>(){
                            new ActivityFieldInfo() {
                                FieldId = 3,
                                State = Enums.ProcessTaskFieldStateEnum.MANDATORY,
                                Field = new FieldInfo() {
                                    ComponentInternalId = "ifijgr567",
                                    Name = "name 3"
                                }
                            }
                        },
                        ActivityUser = new ActivityUserInfo()
                    }
                }
            };

            var result = info.AsEditViewModel();
            Assert.AreEqual(info.Id, result.ProcessVersionId);
            Assert.AreEqual(info.ProcessId, result.ProcessId);
            Assert.AreEqual(info.Name, result.Name);
            Assert.AreEqual(info.Description, result.Description);
            Assert.AreEqual(info.DiagramContent, result.DiagramContent);
            Assert.AreEqual(info.FormContent, result.FormContent);
            Assert.AreEqual(2, result.RolesIds.Count);
            Assert.AreEqual(5, result.TaskSequance);
            Assert.IsTrue(result.RolesIds.Any(x => x.Id == 3));
            Assert.IsTrue(result.RolesIds.Any(x => x.Id == 5));
            Assert.AreEqual(3, result.ProcessTaskSettingViewModelList.Count);

            Assert.IsTrue(result.ProcessTaskSettingViewModelList.Any(x => x.ActivityName == "task 2"));
            Assert.IsTrue(result.ProcessTaskSettingViewModelList.Any(x => x.ActivityId == "idiaid12"));
            Assert.IsTrue(result.ProcessTaskSettingViewModelList[2].Fields.Any(x => x.FieldId == "ifijgr567"));
            Assert.IsTrue(result.ProcessTaskSettingViewModelList[2].Fields.Any(x => x.FieldLabel == "name 3"));
            Assert.IsTrue(result.ProcessTaskSettingViewModelList[2].Fields.Any(x => x.State == Enums.ProcessTaskFieldStateEnum.MANDATORY));
        }
    }
}

