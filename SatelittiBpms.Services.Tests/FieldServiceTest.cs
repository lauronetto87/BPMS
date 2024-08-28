using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Services.Tests
{
    public class FieldServiceTest
    {
        Mock<IFieldRepository> _mockRepository;
        Mock<IMapper> _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockMapper = new Mock<IMapper>();
            _mockRepository = new Mock<IFieldRepository>();
        }

        [Test]
        public void EnsureConstructor()
        {
            FieldService fieldService = new(_mockRepository.Object, _mockMapper.Object);
            Assert.IsNotNull(fieldService);
        }
    }
}
