using Moq;
using NUnit.Framework;
using Satelitti.Authentication.Context.Interface;
using Satelitti.Authentication.Context.Model;
using Satelitti.Authentication.Model;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Integration.Signer;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.Storage.Interfaces;
using System;
using System.Collections.Generic;

namespace SatelittiBpms.Services.Tests
{
    public class SignerIntegrationServiceTest
    {
        Mock<IContextDataService<UserInfo>> _mockContextDataService;
        Mock<ITenantService> _mockTenantService;
        Mock<ISignerSegmentService> _mockSignerSegmentService;
        Mock<ISignerReminderService> _mockSignerReminderService;
        Mock<ISignerSubscriberTypeService> _mockSignerSubscriberTypeService;
        Mock<ISignerSignatureTypeService> _mockSignerSignatureTypeService;
        Mock<ITaskRepository> _mockTaskRepository;
        Mock<ISuiteUserService> _mockSuiteUserService;
        Mock<ISignerIntegrationRestService> _mockSignerIntegrationRestService;
        Mock<IWildcardService> _mockWildcardService;
        Mock<IStorageService> _mockStorageService;
        Mock<IFieldValueFileRepository> _fieldValueFileRepository;

        [SetUp]
        public void Setup()
        {
            _mockContextDataService = new Mock<IContextDataService<UserInfo>>();
            _mockTenantService = new Mock<ITenantService>();
            _mockSignerSegmentService = new Mock<ISignerSegmentService>();
            _mockSignerReminderService = new Mock<ISignerReminderService>();
            _mockSignerSubscriberTypeService = new Mock<ISignerSubscriberTypeService>();
            _mockSignerSignatureTypeService = new Mock<ISignerSignatureTypeService>();
            _mockTaskRepository = new Mock<ITaskRepository>();
            _mockSuiteUserService = new Mock<ISuiteUserService>();
            _mockSignerIntegrationRestService = new Mock<ISignerIntegrationRestService>();
            _mockWildcardService = new Mock<IWildcardService>();
            _mockStorageService = new Mock<IStorageService>();
            _fieldValueFileRepository = new Mock<IFieldValueFileRepository>();

            _mockContextDataService.Setup(x => x.GetContextData()).Returns(new ContextData<UserInfo>() { Tenant = new SuiteTenantAuth() { Id = 75 }, User = new UserInfo { Id = 1 } });
        }

        [Test]
        public void EnsureThatReturnsErrorWhenSignerTokenIsMissing()
        {
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo());

            SignerIntegrationService signerIntegrationService = new(_mockContextDataService.Object, _mockTenantService.Object, _mockSignerSegmentService.Object,
                _mockSignerReminderService.Object, _mockSignerSubscriberTypeService.Object, _mockSignerSignatureTypeService.Object, _mockTaskRepository.Object,
               _mockSuiteUserService.Object, _mockSignerIntegrationRestService.Object, _mockWildcardService.Object, _mockStorageService.Object, _fieldValueFileRepository.Object);

            var result = signerIntegrationService.GetSignerInformation();

            Assert.IsFalse(result.Result.Success);
            Assert.AreEqual(ExceptionCodes.MISSING_SSIGN_INTEGRATION_TOKEN, result.Result.ErrorId);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockTenantService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockSignerSegmentService.Verify(x => x.List(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            _mockSignerReminderService.Verify(x => x.List(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            _mockSignerSubscriberTypeService.Verify(x => x.List(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
            _mockSignerSignatureTypeService.Verify(x => x.List(It.IsAny<string>(), It.IsAny<string>()), Times.Never());
        }

        [Test]
        public void EnsureThatReturnsWhenSignerTokenNotMissing()
        {
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { SignerAccessToken = "SomeToken", SubDomain = "SomeSubDomain" });
            _mockSignerSegmentService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<Segment>());
            _mockSignerReminderService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<EnvelopeReminderDescriptionListItem>());
            _mockSignerSubscriberTypeService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<SubscriberTypeDescriptionListItem>());
            _mockSignerSignatureTypeService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<SignatureTypeDescriptionListItem>());

            SignerIntegrationService signerIntegrationService = new(_mockContextDataService.Object, _mockTenantService.Object, _mockSignerSegmentService.Object, 
                _mockSignerReminderService.Object, _mockSignerSubscriberTypeService.Object, _mockSignerSignatureTypeService.Object, _mockTaskRepository.Object,
               _mockSuiteUserService.Object, _mockSignerIntegrationRestService.Object, _mockWildcardService.Object, _mockStorageService.Object, _fieldValueFileRepository.Object);

            var result = signerIntegrationService.GetSignerInformation();
            var typedResult = ResultContent<SignerIntegrationViewModel>.GetValue(result.Result);

            Assert.IsTrue(result.Result.Success);
            Assert.IsNotNull(typedResult);

            _mockContextDataService.Verify(x => x.GetContextData(), Times.Once());
            _mockTenantService.Verify(x => x.Get(It.IsAny<int>()), Times.Once());
            _mockSignerSegmentService.Verify(x => x.List(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _mockSignerReminderService.Verify(x => x.List(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _mockSignerSubscriberTypeService.Verify(x => x.List(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
            _mockSignerSignatureTypeService.Verify(x => x.List(It.IsAny<string>(), It.IsAny<string>()), Times.Once());
        }

        [Test]
        public void EnsureThatReturnsErrorWhenGetFilePrint()
        {
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { SignerAccessToken = "SomeToken", SubDomain = "SomeSubDomain" });
            _mockSignerSegmentService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<Segment>());
            _mockSignerReminderService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<EnvelopeReminderDescriptionListItem>());
            _mockSignerSubscriberTypeService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<SubscriberTypeDescriptionListItem>());
            _mockSignerSignatureTypeService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<SignatureTypeDescriptionListItem>());

            SignerIntegrationService signerIntegrationService = new(_mockContextDataService.Object, _mockTenantService.Object, _mockSignerSegmentService.Object,
                _mockSignerReminderService.Object, _mockSignerSubscriberTypeService.Object, _mockSignerSignatureTypeService.Object, _mockTaskRepository.Object,
               _mockSuiteUserService.Object, _mockSignerIntegrationRestService.Object, _mockWildcardService.Object, _mockStorageService.Object, _fieldValueFileRepository.Object);

            var fileKey = "dev/Task/";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo($"Não foi encontrado o arquivo com a chave de download `{fileKey}`."),
           async () => await signerIntegrationService.GetFilePrint(fileKey));

        }

        [Test]
        public void EnsureThatReturnsErrorWhenGetFileSigned()
        {
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { SignerAccessToken = "SomeToken", SubDomain = "SomeSubDomain" });
            _mockSignerSegmentService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<Segment>());
            _mockSignerReminderService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<EnvelopeReminderDescriptionListItem>());
            _mockSignerSubscriberTypeService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<SubscriberTypeDescriptionListItem>());
            _mockSignerSignatureTypeService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<SignatureTypeDescriptionListItem>());

            SignerIntegrationService signerIntegrationService = new(_mockContextDataService.Object, _mockTenantService.Object, _mockSignerSegmentService.Object,
                _mockSignerReminderService.Object, _mockSignerSubscriberTypeService.Object, _mockSignerSignatureTypeService.Object, _mockTaskRepository.Object,
               _mockSuiteUserService.Object, _mockSignerIntegrationRestService.Object, _mockWildcardService.Object, _mockStorageService.Object, _fieldValueFileRepository.Object);

            var fileKey = "dev/Task/";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo($"Não foi encontrado o arquivo com a chave de download `{fileKey}`."),
           async () => await signerIntegrationService.GetFileSigned(fileKey));

        }

        [Test]
        public void EnsureThatReturnsErrorWhenCreateEnvelopeOnSigner()
        {
            _mockTenantService.Setup(x => x.Get(It.IsAny<int>())).Returns(new TenantInfo() { SignerAccessToken = "SomeToken", SubDomain = "SomeSubDomain" });
            _mockSignerSegmentService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<Segment>());
            _mockSignerReminderService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<EnvelopeReminderDescriptionListItem>());
            _mockSignerSubscriberTypeService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<SubscriberTypeDescriptionListItem>());
            _mockSignerSignatureTypeService.Setup(x => x.List(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new List<SignatureTypeDescriptionListItem>());

            SignerIntegrationService signerIntegrationService = new(_mockContextDataService.Object, _mockTenantService.Object, _mockSignerSegmentService.Object,
                _mockSignerReminderService.Object, _mockSignerSubscriberTypeService.Object, _mockSignerSignatureTypeService.Object, _mockTaskRepository.Object,
               _mockSuiteUserService.Object, _mockSignerIntegrationRestService.Object, _mockWildcardService.Object, _mockStorageService.Object, _fieldValueFileRepository.Object);

            var fileKey = "dev/Task/";

            Assert.ThrowsAsync(Is.TypeOf<ArgumentException>().And.Message.EqualTo($"Não foi encontrado o arquivo com a chave de download `{fileKey}`."),
           async () => await signerIntegrationService.GetFileSigned(fileKey));

        }
    }
}
