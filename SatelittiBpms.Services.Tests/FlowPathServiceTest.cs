using AutoMapper;
using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Tests
{
    public class FlowPathServiceTest
    {
        Mock<IFlowPathRepository> _mockRepository;
        Mock<IMapper> _mockMapper;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IFlowPathRepository>();
            _mockMapper = new Mock<IMapper>();
        }

        [Test]
        public async Task ensureInsertWithInfoParam()
        {
            _mockRepository.Setup(x => x.Insert(It.IsAny<FlowPathInfo>())).ReturnsAsync(4);

            FlowPathService flowPathService = new FlowPathService(_mockRepository.Object, _mockMapper.Object);
            var result = await flowPathService.Insert(new FlowPathInfo());
            Assert.AreEqual(4, result.Value);
            _mockRepository.Verify(x => x.Insert(It.IsAny<FlowPathInfo>()), Times.Once());
        }
    }
}
