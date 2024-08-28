using Moq;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.VersionNormalization.Interfaces;
using SatelittiBpms.VersionNormalization.Services;
using System.Threading.Tasks;

namespace SatelittiBpms.VersionNormalization.Tests.Normalizations
{
    public class VersionNormalizationServiceTest
    {
        Mock<IVersionNormalizationRepository> _mockRepository;

        [SetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IVersionNormalizationRepository>();
        }

        [Test]
        public void ensureThatAddNormalizationExecuteRepositoryInsertOnce()
        {
            VersionNormalizationService versionNormalizationService = new VersionNormalizationService(_mockRepository.Object);
            versionNormalizationService.AddNormalization("json content");

            _mockRepository.Verify(x => x.Insert(It.IsAny<VersionNormalizationInfo>()), Times.Once());
        }

        [Test]
        public void ensureThatInsertExecuteRepositoryInsertOnce()
        {
            var versionNormalization = new VersionNormalizationInfo() { Normalization = "NormalizationName" };

            VersionNormalizationService versionNormalizationService = new VersionNormalizationService(_mockRepository.Object);
            versionNormalizationService.Insert(versionNormalization);

            _mockRepository.Verify(x => x.Insert(It.IsAny<VersionNormalizationInfo>()), Times.Once());
        }

        [Test]
        public void ensureThatListAllExecuteRepositoryListAllOnce()
        {
            VersionNormalizationService versionNormalizationService = new VersionNormalizationService(_mockRepository.Object);
            versionNormalizationService.ListAll();

            _mockRepository.Verify(x => x.ListAll(), Times.Once());
        }
    }
}
