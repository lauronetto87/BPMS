using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class ProcessRoleServiceTest
    {
        Mock<IProcessRoleRepository> _mockRepository;
        Mock<IMapper> _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepository = new Mock<IProcessRoleRepository>();
        }

        [Test]
        public async Task ensureThatInsertManyDoNothingWhenRoleListIsNull()
        {
            ProcessRoleService processRoleService = new ProcessRoleService(_mockRepository.Object, _mockMapper.Object);
            await processRoleService.InsertMany(null, 1, 2);

            _mockRepository.Verify(x => x.Insert(It.IsAny<ProcessVersionRoleInfo>()), Times.Never());
        }

        [Test]
        public async Task ensureThatInsertManyWhenRoleListIsNotNull()
        {
            _mockRepository.Setup(x => x.Insert(It.IsAny<ProcessVersionRoleInfo>()));

            ProcessRoleService activityService = new ProcessRoleService(_mockRepository.Object, _mockMapper.Object);
            await activityService.InsertMany(new List<int>() { 5, 7, 8, 3 }, 1, 2);

            _mockRepository.Verify(x => x.Insert(It.IsAny<ProcessVersionRoleInfo>()), Times.Exactly(4));
        }
    }
}
