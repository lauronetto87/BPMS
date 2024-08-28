using SatelittiBpms.Mail.Models;
using System.Threading.Tasks;

namespace SatelittiBpms.Mail.Interfaces
{
    public interface IMessageService
    {
        Task<MailMessage> CreateMessage(int tenantId, int activityId, int requesterId, int taskId);
    }
}
