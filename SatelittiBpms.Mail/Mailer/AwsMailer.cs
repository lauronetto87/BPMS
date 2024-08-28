using Amazon.SimpleEmail;
using Amazon.SimpleEmail.Model;
using SatelittiBpms.Mail.Exceptions;
using SatelittiBpms.Mail.Interfaces;
using SatelittiBpms.Mail.Models;
using SatelittiBpms.Mail.Models.Config;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace SatelittiBpms.Mail.Mailer
{
    public class AwsMailer : IMailerService
    {
        public async Task SendMail(MailMessage message, BaseConfig config)
        {
            if (message == null)
                throw MessageMissingException.Create();

            using var client = CreateClientInstance();
            SendEmailRequest request = CreateRequest(message, (SESMailMessageConfig)config);
            try
            {
                await client.SendEmailAsync(request);
            }
            catch (AccountSendingPausedException ex)
            {
                throw EmailSendingDisabledException.Create(ex);
            }
            catch (ConfigurationSetDoesNotExistException ex)
            {
                throw ConfigurationNotExistException.Create(ex);
            }
            catch (ConfigurationSetSendingPausedException ex)
            {
                throw ConfigurationSendingDisabledException.Create(ex);
            }
            catch (MailFromDomainNotVerifiedException ex)
            {
                throw SenderNotVerifiedException.Create(ex);
            }
            catch (Amazon.SimpleEmail.Model.MessageRejectedException ex)
            {
                throw SatelittiBpms.Mail.Exceptions.MessageRejectedException.Create(ex);
            }
            catch (Exception ex)
            {
                throw MailGenericException.Create(ex);
            }
        }

        [ExcludeFromCodeCoverage]
        protected virtual IAmazonSimpleEmailService CreateClientInstance()
        {
            return new AmazonSimpleEmailServiceClient(Amazon.RegionEndpoint.USEast1);
        }

        protected virtual SendEmailRequest CreateRequest(MailMessage message, SESMailMessageConfig config)
        {
            if (message == null)
                throw MessageMissingException.Create();

            var request = new SendEmailRequest
            {
                Source = message.Sender,
                Destination = new Destination
                {
                    ToAddresses = message.To,
                    CcAddresses = message.Cc,
                    BccAddresses = message.Bcc
                },
                Message = new Message
                {
                    Subject = new Content(message.Subject),
                    Body = new Body
                    {
                        Html = new Content
                        {
                            Charset = "UTF-8",
                            Data = message.Body.Html
                        },
                        Text = new Content
                        {
                            Charset = "UTF-8",
                            Data = message.Body.Text
                        }
                    }
                }
            };

            if (config != null && config.ConfigSet != null)
                request.ConfigurationSetName = config.ConfigSet;

            return request;
        }
    }
}
