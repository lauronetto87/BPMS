using NUnit.Framework;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using System;

namespace SatelittiBpms.Models.Tests
{
    public class TaskInfoTest
    {
        [Test]
        public void ensureAsListingViewModel()
        {
            TaskInfo info = new TaskInfo()
            {
                Id = 3,
                Flow = new FlowInfo()
                {
                    CreatedDate = DateTime.Now,
                    ProcessVersion = new ProcessVersionInfo()
                    {
                        Name = "process Name",
                        Description = "process Description",
                        CreatedByUserName = "created User name",
                        CreatedByUserId = 56,
                        Status = ProcessStatusEnum.EDITING
                    }
                },
                Activity = new ActivityInfo()
                {
                    ActivityUser = new ActivityUserInfo()
                    {
                        ExecutorType = UserTaskExecutorTypeEnum.ROLE
                    }                    
                }                
            };

            var result = info.AsListingViewModel(null);

            Assert.AreEqual(info.Id, result.Id);
            Assert.AreEqual(info.Flow.ProcessVersion.Name, result.Name);
            Assert.AreEqual(info.Flow.ProcessVersion.Description, result.Description);
            Assert.AreEqual(info.CreatedDate, result.CreationDate);
            Assert.AreEqual(info.Flow.ProcessVersion.CreatedByUserName, result.CreatedByUserName);
            Assert.AreEqual(info.Flow.ProcessVersion.CreatedByUserId, result.CreatedByUserId);
            Assert.AreEqual(info.Flow.ProcessVersion.Status, result.ProcessStatus);
            Assert.AreEqual(info.Activity.ActivityUser.ExecutorType, result.ExecutorType);
        }
    }
}
