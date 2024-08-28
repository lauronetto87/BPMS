using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface INotificationService : IServiceBase<NotificationInfo, NotificationInfo>
    {
        public Task<ResultContent> SetToRead(int notificationId);

        public Task<ResultContent> SetToDeleted(int notificationId);

        public Task<List<NotificationViewModel>> List();
    }
}
