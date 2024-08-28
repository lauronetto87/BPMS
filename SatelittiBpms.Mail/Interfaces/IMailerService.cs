using SatelittiBpms.Mail.Models;
using SatelittiBpms.Mail.Models.Config;
using System.Threading.Tasks;

namespace SatelittiBpms.Mail.Interfaces
{
    public interface IMailerService
    {
        Task SendMail(MailMessage message, BaseConfig config = null);
    }
}
