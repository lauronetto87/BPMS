using Moq;
using Moq.Protected;
using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.VersionNormalization.Interfaces;
using SatelittiBpms.VersionNormalization.Normalizations;
using SatelittiBpms.VersionNormalization.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.VersionNormalization.Tests.Normalizations
{
    public class ExecuteNormalizationsTest
    {
        Mock<IVersionNormalizationService> _mockVersionNormalizationService;
        Mock<IServiceProvider> _mockServiceProvider;
        Mock<IVersionNormalization> _mockVersionNormalization;

        [SetUp]
        public void Setup()
        {
            _mockVersionNormalization = new Mock<IVersionNormalization>();
            _mockVersionNormalizationService = new Mock<IVersionNormalizationService>();
            _mockServiceProvider = new Mock<IServiceProvider>();
        }

        [Test]
        public async Task EnsureThatNotExecuteNormalizationWhenHasAlreadyExecutedAsync()
        {
            var normalizationList = new List<VersionNormalizationInfo>();
            var types = new List<Type>() { typeof(_20211206_WebSocketWorkFlowContentNormalization) };

            normalizationList.Add(new VersionNormalizationInfo() { Id = 1, Normalization = "_20211206_WebSocketWorkFlowContentNormalization" });
            _mockVersionNormalizationService.Setup(x => x.ListAll()).Returns(normalizationList);

            Mock<ExecuteNormalizations> executeNormalizations = new Mock<ExecuteNormalizations>(_mockVersionNormalizationService.Object, _mockServiceProvider.Object) { CallBase = true };
            executeNormalizations.Protected().Setup<List<Type>>("GetClassessInNamespace").Returns(types);
            executeNormalizations.Protected().Setup<IVersionNormalization>("VersionNormalizationInstance", types[0]).Returns(_mockVersionNormalization.Object);
            await executeNormalizations.Object.Execute();

            _mockVersionNormalizationService.Verify(x => x.AddNormalization(It.IsAny<string>()), Times.Never());
        }

        [Test]
        public async Task EnsureThatExecuteNormalizationWhenHasNotExecuted()
        {
            var normalizationList = new List<VersionNormalizationInfo>();
            var types = new List<Type>() { typeof(_20211206_WebSocketWorkFlowContentNormalization) };

            normalizationList.Add(new VersionNormalizationInfo() { Id = 1, Normalization = "NormalizationNotExecuted" });
            _mockVersionNormalizationService.Setup(x => x.ListAll()).Returns(normalizationList);

            Mock<ExecuteNormalizations> executeNormalizations = new Mock<ExecuteNormalizations>(_mockVersionNormalizationService.Object, _mockServiceProvider.Object) { CallBase = true };
            executeNormalizations.Protected().Setup<List<Type>>("GetClassessInNamespace").Returns(types);
            executeNormalizations.Protected().Setup<IVersionNormalization>("VersionNormalizationInstance", types[0]).Returns(_mockVersionNormalization.Object);
            await executeNormalizations.Object.Execute();

            _mockVersionNormalizationService.Verify(x => x.AddNormalization(It.IsAny<string>()), Times.Once());
        }
    }
}
