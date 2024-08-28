using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class NotificationService : AbstractServiceBase<NotificationInfo, NotificationInfo, INotificationRepository>, INotificationService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private new readonly INotificationRepository _repository;

        public NotificationService(INotificationRepository repository, IMapper mapper, IContextDataService<UserInfo> contextDataService) : base(repository, mapper)
        {
            _repository = repository;
            _contextDataService = contextDataService;
        }

        public async Task<ResultContent> SetToDeleted(int notificationId)
        {
            var context = _contextDataService.GetContextData();
            try
            {
                var taskInfo = await _repository.Get(notificationId);
                if (taskInfo.UserId != context.User.Id)
                {
                    var error = $"Você não tem permissão para alterar a notificação de código {notificationId}.";
                    AddErrors(error, error);
                    return Result.Error(ValidationResult);
                }
                taskInfo.Deleted = true;
                await _repository.Update(taskInfo);

            }
            catch (Exception e)
            {
                AddErrors($"Erro ao atualizar a notificação de código {notificationId}.", e.Message);
                return Result.Error(ValidationResult);
            }
            return Result.Success();
        }

        public async Task<ResultContent> SetToRead(int notificationId)
        {
            var context = _contextDataService.GetContextData();
            try
            {
                var taskInfo = await _repository.Get(notificationId);
                if (taskInfo.UserId != context.User.Id)
                {
                    var error = $"Você não tem permissão para alterar a notificação de código {notificationId}.";
                    AddErrors(error, error);
                    return Result.Error(ValidationResult);
                }
                taskInfo.Read = true;
                await _repository.Update(taskInfo);

            }
            catch (Exception e)
            {
                AddErrors($"Erro ao atualizar a notificação de código {notificationId}.", e.Message);
                return Result.Error(ValidationResult);
            }
            return Result.Success();
        }

        public Task<List<NotificationViewModel>> List()
        {
            var context = _contextDataService.GetContextData();
            var result = _repository.GetToUser(context.User.Id)
                .Select(x => x.AsViewModel())
                .ToList();
            return Task.FromResult(result);
        }
    }
}
