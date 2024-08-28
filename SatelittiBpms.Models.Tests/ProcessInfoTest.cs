using NUnit.Framework;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using System;
using System.Collections.Generic;

namespace SatelittiBpms.Models.Tests
{
    public class ProcessInfoTest
    {
        [Test]
        public void ensureThatReturnProcessListiningViewModelWhenHaveEditingVersionAndCurrentVersionIsNull()
        {
            DateTime createdDate = new DateTime(2021, 02, 11, 5, 16, 00);
            DateTime lastModifiedDate = new DateTime(2021, 03, 12, 15, 46, 02);
            string createdUserName = "CreatedUser Bertucci",
                processVersionName = "Version 2";
            int processVersionId = 2;

            ProcessInfo info = new ProcessInfo()
            {
                Id = 22,
                CurrentVersion = null,
                ProcessVersions = new List<ProcessVersionInfo>()
                {
                    new ProcessVersionInfo(){
                        Id = processVersionId,
                        Name = processVersionName,
                        CreatedDate = createdDate,
                        CreatedByUserName = createdUserName,
                        LastModifiedDate = lastModifiedDate,
                        Status = ProcessStatusEnum.EDITING
                    }
                }
            };

            var result = info.AsListingViewModel();
            Assert.AreEqual(info.Id, result.ProcessId);
            Assert.AreEqual(processVersionId, result.ProcessVersionId);
            Assert.AreEqual(processVersionName, result.Name);
            Assert.AreEqual(createdDate, result.CreatedDate);
            Assert.AreEqual(createdUserName, result.CreatedByUserName);
            Assert.AreEqual(lastModifiedDate, result.LastModifiedDate);
            Assert.AreEqual(ProcessStatusEnum.EDITING, result.Status);
        }


        [Test]
        public void ensureThatReturnProcessListiningViewModelWhenHaveEditingVersionAndCurrentVersionIsNotNull()
        {
            DateTime createdDate = new DateTime(2021, 02, 11, 5, 16, 00);
            DateTime lastModifiedDate = new DateTime(2021, 03, 12, 15, 46, 02);
            string createdUserName = "CreatedUser Bertucci",
                processVersionName = "Version 2";
            int processVersionId = 2;

            ProcessInfo info = new ProcessInfo()
            {
                Id = 22,
                CurrentVersion = 1,
                ProcessVersions = new List<ProcessVersionInfo>()
                {
                       new ProcessVersionInfo(){
                        Id = 1,
                        Version = 1,
                        Name = "Version 1",
                        CreatedDate = createdDate.AddDays(-1),
                        CreatedByUserName = "other user",
                        LastModifiedDate = lastModifiedDate.AddHours(-20),
                        Status = ProcessStatusEnum.PUBLISHED
                    },
                    new ProcessVersionInfo(){
                        Id = processVersionId,
                        Version = 2,
                        Name = processVersionName,
                        CreatedDate = createdDate,
                        CreatedByUserName = createdUserName,
                        LastModifiedDate = lastModifiedDate,
                        Status = ProcessStatusEnum.EDITING
                    }
                }
            };

            var result = info.AsListingViewModel();
            Assert.AreEqual(info.Id, result.ProcessId);
            Assert.AreEqual(processVersionId, result.ProcessVersionId);
            Assert.AreEqual(processVersionName, result.Name);
            Assert.AreEqual(createdDate, result.CreatedDate);
            Assert.AreEqual(createdUserName, result.CreatedByUserName);
            Assert.AreEqual(lastModifiedDate, result.LastModifiedDate);
            Assert.AreEqual(ProcessStatusEnum.EDITING, result.Status);
        }

        [Test]
        public void ensureThatReturnProcessListiningViewModelWhenNotHaveEditingVersionAndCurrentVersionIsNotNull()
        {
            DateTime createdDate = new DateTime(2021, 02, 11, 5, 16, 00);
            DateTime lastModifiedDate = new DateTime(2021, 03, 12, 15, 46, 02);
            string createdUserName = "CreatedUser Bertucci",
                processVersionName = "Version 2";
            int processVersionId = 2;

            ProcessInfo info = new ProcessInfo()
            {
                Id = 22,
                CurrentVersion = 2,
                ProcessVersions = new List<ProcessVersionInfo>()
                {
                       new ProcessVersionInfo(){
                        Id = 1,
                        Version = 1,
                        Name = "Version 1",
                        CreatedDate = createdDate.AddDays(-1),
                        CreatedByUserName = "other user",
                        LastModifiedDate = lastModifiedDate.AddHours(-20),
                        Status = ProcessStatusEnum.PUBLISHED
                    },
                    new ProcessVersionInfo(){
                        Id = processVersionId,
                        Version = 2,
                        Name = processVersionName,
                        CreatedDate = createdDate,
                        CreatedByUserName = createdUserName,
                        LastModifiedDate = lastModifiedDate,
                        Status = ProcessStatusEnum.PUBLISHED
                    }
                }
            };

            var result = info.AsListingViewModel();
            Assert.AreEqual(info.Id, result.ProcessId);
            Assert.AreEqual(processVersionId, result.ProcessVersionId);
            Assert.AreEqual(processVersionName, result.Name);
            Assert.AreEqual(createdDate, result.CreatedDate);
            Assert.AreEqual(createdUserName, result.CreatedByUserName);
            Assert.AreEqual(lastModifiedDate, result.LastModifiedDate);
            Assert.AreEqual(ProcessStatusEnum.PUBLISHED, result.Status);
        }
    }
}
