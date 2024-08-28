using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SatelittiBpms.Mail.Exceptions;
using SatelittiBpms.Mail.Mailer;
using SatelittiBpms.Mail.Models;
using SatelittiBpms.Mail.Models.Config;
using SatelittiBpms.Mail.Tests.ProtectedWrapper;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SatelittiBpms.Mail.Tests
{
    public class Tests
    {
        Mock<AwsMailer> _mailerMock;        
        Mock<IAmazonSimpleEmailService> _AwsSESMock;

        [SetUp]
        public void Setup()
        {            
            _mailerMock = new Mock<AwsMailer>() { CallBase = true };
            _AwsSESMock = new Mock<IAmazonSimpleEmailService>();
        }

        [Test]
        public void EnsureThatThowsExceptionWhenMessageIsNullOnSendMail()
        {
            var mailer = new AwsMailer();

            Assert.Throws(
                 Is.TypeOf<AggregateException>()
                .And.InnerException.TypeOf(typeof(MessageMissingException))
                .And.InnerException.Message.EqualTo("The mail message argument is missing."),
                    () => mailer.SendMail(null, null).Wait());
        }

        [Test]
        public void EnsureThatEmailSendingDisabledExceptionIsCreateWhenSendThrowsAccountSendingPausedException()
        {
            _AwsSESMock.Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>())).Throws(new AccountSendingPausedException("error"));
            _mailerMock.Protected().Setup<IAmazonSimpleEmailService>("CreateClientInstance").Returns(_AwsSESMock.Object);
            _mailerMock.Protected().Setup<SendEmailRequest>("CreateRequest", ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>()).Returns(new SendEmailRequest());

            var message = new MailMessage();
            var SESconfig = new SESMailMessageConfig();

            Assert.Throws(
                 Is.TypeOf<AggregateException>()
                .And.InnerException.TypeOf(typeof(EmailSendingDisabledException))
                .And.InnerException.Message.EqualTo("Email sending is disabled for your entire Amazon SES account.")
                .And.InnerException.InnerException.Not.Null,
                    () => _mailerMock.Object.SendMail(message, SESconfig).Wait());

            _mailerMock.Protected().Verify("CreateClientInstance", Times.Once());
            _mailerMock.Protected().Verify("CreateRequest", Times.Once(), ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>());
            _AwsSESMock.Verify(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void EnsureThatConfigurationNotExistExceptionIsCreateWhenSendThrowsConfigurationSetDoesNotExistException()
        {
            _AwsSESMock.Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>())).Throws(new ConfigurationSetDoesNotExistException("error"));
            _mailerMock.Protected().Setup<IAmazonSimpleEmailService>("CreateClientInstance").Returns(_AwsSESMock.Object);
            _mailerMock.Protected().Setup<SendEmailRequest>("CreateRequest", ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>()).Returns(new SendEmailRequest());

            var message = new MailMessage();
            var SESconfig = new SESMailMessageConfig();

            Assert.Throws(
                 Is.TypeOf<AggregateException>()
                .And.InnerException.TypeOf(typeof(ConfigurationNotExistException))
                .And.InnerException.Message.EqualTo("The configuration set does not exist.")
                .And.InnerException.InnerException.Not.Null,
                    () => _mailerMock.Object.SendMail(message, SESconfig).Wait());

            _mailerMock.Protected().Verify("CreateClientInstance", Times.Once());
            _mailerMock.Protected().Verify("CreateRequest", Times.Once(), ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>());
            _AwsSESMock.Verify(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void EnsureThatConfigurationSendingDisabledExceptionIsCreateWhenSendThrowsConfigurationSetSendingPausedException()
        {
            _AwsSESMock.Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>())).Throws(new ConfigurationSetSendingPausedException("error"));
            _mailerMock.Protected().Setup<IAmazonSimpleEmailService>("CreateClientInstance").Returns(_AwsSESMock.Object);
            _mailerMock.Protected().Setup<SendEmailRequest>("CreateRequest", ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>()).Returns(new SendEmailRequest());

            var message = new MailMessage();
            var SESconfig = new SESMailMessageConfig();

            Assert.Throws(
                Is.TypeOf<AggregateException>()
                .And.InnerException.TypeOf(typeof(ConfigurationSendingDisabledException))
                .And.InnerException.Message.EqualTo("Email sending is disabled for the configuration set.")
                .And.InnerException.InnerException.Not.Null,
                    () => _mailerMock.Object.SendMail(message, SESconfig).Wait());

            _mailerMock.Protected().Verify("CreateClientInstance", Times.Once());
            _mailerMock.Protected().Verify("CreateRequest", Times.Once(), ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>());
            _AwsSESMock.Verify(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void EnsureThatSenderNotVerifiedExceptionIsCreateWhenSendThrowsMailFromDomainNotVerifiedException()
        {
            _AwsSESMock.Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>())).Throws(new MailFromDomainNotVerifiedException("error"));
            _mailerMock.Protected().Setup<IAmazonSimpleEmailService>("CreateClientInstance").Returns(_AwsSESMock.Object);
            _mailerMock.Protected().Setup<SendEmailRequest>("CreateRequest", ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>()).Returns(new SendEmailRequest());

            var message = new MailMessage();
            var SESconfig = new SESMailMessageConfig();

            Assert.Throws(
                 Is.TypeOf<AggregateException>()
                .And.InnerException.TypeOf(typeof(SenderNotVerifiedException))
                .And.InnerException.Message.EqualTo("The message could not be sent because Amazon SES could not read the MX record required to use the specified MAIL FROM domain.")
                .And.InnerException.InnerException.Not.Null,
                    () => _mailerMock.Object.SendMail(message, SESconfig).Wait());

            _mailerMock.Protected().Verify("CreateClientInstance", Times.Once());
            _mailerMock.Protected().Verify("CreateRequest", Times.Once(), ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>());
            _AwsSESMock.Verify(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void EnsureThatMessageRejectedExceptionIsCreateWhenSendThrowsMessageRejectedException()
        {
            _AwsSESMock.Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>())).Throws(new Amazon.SimpleEmail.Model.MessageRejectedException("error"));
            _mailerMock.Protected().Setup<IAmazonSimpleEmailService>("CreateClientInstance").Returns(_AwsSESMock.Object);
            _mailerMock.Protected().Setup<SendEmailRequest>("CreateRequest", ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>()).Returns(new SendEmailRequest());

            var message = new MailMessage();
            var SESconfig = new SESMailMessageConfig();

            Assert.Throws(
                 Is.TypeOf<AggregateException>()
                .And.InnerException.TypeOf(typeof(SatelittiBpms.Mail.Exceptions.MessageRejectedException))
                .And.InnerException.Message.EqualTo("The action failed, and the message could not be sent.Check the error stack for more information about what caused the error.")
                .And.InnerException.InnerException.Not.Null,
                    () => _mailerMock.Object.SendMail(message, SESconfig).Wait());

            _mailerMock.Protected().Verify("CreateClientInstance", Times.Once());
            _mailerMock.Protected().Verify("CreateRequest", Times.Once(), ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>());
            _AwsSESMock.Verify(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void EnsureThatMailGenericExceptionIsCreateWhenSendThrowsAnyException()
        {
            _AwsSESMock.Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>())).Throws(new Exception("error"));
            _mailerMock.Protected().Setup<IAmazonSimpleEmailService>("CreateClientInstance").Returns(_AwsSESMock.Object);
            _mailerMock.Protected().Setup<SendEmailRequest>("CreateRequest", ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>()).Returns(new SendEmailRequest());

            var message = new MailMessage();
            var SESconfig = new SESMailMessageConfig();

            Assert.Throws(
                 Is.TypeOf<AggregateException>()
                .And.InnerException.TypeOf(typeof(MailGenericException))
                .And.InnerException.Message.EqualTo("A generic error occurred while sending email. Check the Stack for more information.")
                .And.InnerException.InnerException.Not.Null,
                    () => _mailerMock.Object.SendMail(message, SESconfig).Wait());

            _mailerMock.Protected().Verify("CreateClientInstance", Times.Once());
            _mailerMock.Protected().Verify("CreateRequest", Times.Once(), ItExpr.IsAny<MailMessage>(), ItExpr.IsAny<SESMailMessageConfig>());
            _AwsSESMock.Verify(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public void EnsureThatThowsExceptionWhenMessageIsNullOnCreateRequest()
        {
            var mailer = new AwsSimpleEmailService_MailerTestable();

            Assert.Throws(Is.TypeOf<MessageMissingException>()
                .And.Message.EqualTo("The mail message argument is missing.")
                .And.InnerException.Null,
                    () => mailer.CreateRequestTestMethod(null, null));
        }

        [Test]
        public void EnsureThatAllPropertiesOfSendEmailRequestAreFilledAndConfigSetNotWhenConfigIsNull()
        {
            var mailer = new AwsSimpleEmailService_MailerTestable();
            var message = new MailMessage()
            {
                Sender = "TenantName via BPMS <exemplo@exemplo.com.br>",
                To = new List<string>() { "to1@exemple.com.br", "to2@exemple.com.br" },
                Cc = new List<string>() { "cc1@exemple.com.br" },
                Bcc = new List<string>() { "bcc1@exemple.com.br", "bcc2@exemple.com.br", "bcc3@exemple.com.br" },
                Subject = "Titulo e-mail",
                Body = new MailBody()
                {
                    Html = "<body>Exemplo de body </br> para teste</body>",
                    Text = "Exemplo de body \r\n para teste"
                }
            };

            var result = mailer.CreateRequestTestMethod(message, null);

            Assert.AreEqual(message.Sender, result.Source);
            Assert.AreEqual(message.Subject, result.Message.Subject.Data);
            Assert.AreEqual("UTF-8", result.Message.Body.Html.Charset);
            Assert.AreEqual(message.Body.Html, result.Message.Body.Html.Data);
            Assert.AreEqual("UTF-8", result.Message.Body.Text.Charset);
            Assert.AreEqual(message.Body.Text, result.Message.Body.Text.Data);
            Assert.AreEqual(2, result.Destination.ToAddresses.Count);
            Assert.AreEqual(1, result.Destination.CcAddresses.Count);
            Assert.AreEqual(3, result.Destination.BccAddresses.Count);
            Assert.AreEqual("to1@exemple.com.br", result.Destination.ToAddresses[0]);
            Assert.AreEqual("to2@exemple.com.br", result.Destination.ToAddresses[1]);
            Assert.AreEqual("cc1@exemple.com.br", result.Destination.CcAddresses[0]);
            Assert.AreEqual("bcc1@exemple.com.br", result.Destination.BccAddresses[0]);
            Assert.AreEqual("bcc2@exemple.com.br", result.Destination.BccAddresses[1]);
            Assert.AreEqual("bcc3@exemple.com.br", result.Destination.BccAddresses[2]);
            Assert.IsNull(result.ConfigurationSetName);
        }

        [Test]
        public void EnsureThatAllPropertiesOfSendEmailRequestAreFilled()
        {
            var mailer = new AwsSimpleEmailService_MailerTestable();
            var message = new MailMessage()
            {
                Sender = "TenantName <exemplo@exemplo.com.br>",
                To = new List<string>() { "to1@exemple.com.br", "to2@exemple.com.br" },
                Cc = new List<string>() { "cc1@exemple.com.br" },
                Bcc = new List<string>() { "bcc1@exemple.com.br", "bcc2@exemple.com.br", "bcc3@exemple.com.br" },
                Subject = "Titulo e-mail",
                Body = new MailBody()
                {
                    Html = "<body>Exemplo de body </br> para teste</body>",
                    Text = "Exemplo de body \r\n para teste"
                }
            };
            var SESConfig = new SESMailMessageConfig()
            {
                ConfigSet = "ConfigTest"
            };

            var result = mailer.CreateRequestTestMethod(message, SESConfig);

            Assert.AreEqual(message.Sender, result.Source);
            Assert.AreEqual(message.Subject, result.Message.Subject.Data);
            Assert.AreEqual("UTF-8", result.Message.Body.Html.Charset);
            Assert.AreEqual(message.Body.Html, result.Message.Body.Html.Data);
            Assert.AreEqual("UTF-8", result.Message.Body.Text.Charset);
            Assert.AreEqual(message.Body.Text, result.Message.Body.Text.Data);
            Assert.AreEqual(2, result.Destination.ToAddresses.Count);
            Assert.AreEqual(1, result.Destination.CcAddresses.Count);
            Assert.AreEqual(3, result.Destination.BccAddresses.Count);
            Assert.AreEqual("to1@exemple.com.br", result.Destination.ToAddresses[0]);
            Assert.AreEqual("to2@exemple.com.br", result.Destination.ToAddresses[1]);
            Assert.AreEqual("cc1@exemple.com.br", result.Destination.CcAddresses[0]);
            Assert.AreEqual("bcc1@exemple.com.br", result.Destination.BccAddresses[0]);
            Assert.AreEqual("bcc2@exemple.com.br", result.Destination.BccAddresses[1]);
            Assert.AreEqual("bcc3@exemple.com.br", result.Destination.BccAddresses[2]);
            Assert.AreEqual(SESConfig.ConfigSet, result.ConfigurationSetName);
        }

        [Test]
        public async Task EnsureThatNotThrowsExcepitonWhenConfigIsNull()
        {
            _AwsSESMock.Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()));
            _mailerMock.Protected().Setup<IAmazonSimpleEmailService>("CreateClientInstance").Returns(_AwsSESMock.Object);

            var message = new MailMessage()
            {
                Sender = "TenantName via BPMS <exemplo@exemplo.com.br>",
                To = new List<string>() { "to1@exemple.com.br", "to2@exemple.com.br" },
                Cc = new List<string>() { "cc1@exemple.com.br" },
                Bcc = new List<string>() { "bcc1@exemple.com.br", "bcc2@exemple.com.br", "bcc3@exemple.com.br" },
                Subject = "Titulo e-mail",
                Body = new MailBody()
                {
                    Html = "<body>Exemplo de body </br> para teste</body>",
                    Text = "Exemplo de body \r\n para teste"
                }
            };

            await _mailerMock.Object.SendMail(message, null);

            _mailerMock.Protected().Verify("CreateClientInstance", Times.Once());
            _AwsSESMock.Verify(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Test]
        public async Task EnsureThatNotThrowsExcepiton()
        {
            _AwsSESMock.Setup(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()));
            _mailerMock.Protected().Setup<IAmazonSimpleEmailService>("CreateClientInstance").Returns(_AwsSESMock.Object);

            var message = new MailMessage()
            {
                Sender = "TenantName via BPMS <exemplo@exemplo.com.br>",
                To = new List<string>() { "to1@exemple.com.br", "to2@exemple.com.br" },
                Cc = new List<string>() { "cc1@exemple.com.br" },
                Bcc = new List<string>() { "bcc1@exemple.com.br", "bcc2@exemple.com.br", "bcc3@exemple.com.br" },
                Subject = "Titulo e-mail",
                Body = new MailBody()
                {
                    Html = "<body>Exemplo de body </br> para teste</body>",
                    Text = "Exemplo de body \r\n para teste"
                }
            };
            var SESConfig = new SESMailMessageConfig()
            {
                ConfigSet = "ConfigTest"
            };

            await _mailerMock.Object.SendMail(message, SESConfig);

            _mailerMock.Protected().Verify("CreateClientInstance", Times.Once());
            _AwsSESMock.Verify(m => m.SendEmailAsync(It.IsAny<SendEmailRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}