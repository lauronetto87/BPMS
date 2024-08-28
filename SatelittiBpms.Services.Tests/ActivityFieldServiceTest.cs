using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Services.Tests
{
    public class ActivityFieldServiceTest
    {
        Mock<IActivityFieldRepository> _mockRepository;
        Mock<IMapper> _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepository = new Mock<IActivityFieldRepository>();
        }

        [Test]
        public void ensureConstructor()
        {
            ActivityFieldService activityFieldService = new ActivityFieldService(_mockRepository.Object, _mockMapper.Object);
            Assert.IsNotNull(activityFieldService);
        }
    }
}
