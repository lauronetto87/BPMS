using Microsoft.Extensions.Options;
using SatelittiBpms.Mail.Interfaces;
using SatelittiBpms.Mail.Models;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Options.Models;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SatelittiBpms.Mail.Services
{
    public class MessageService : IMessageService
    {
        private readonly IActivityService _activityService;
        private readonly ISuiteUserService _suiteUserService;
        private readonly IRoleService _roleService;
        private readonly ITenantService _tenantService;
        private readonly IWildcardService _wildcardService;
        private readonly AwsOptions _awsOptions;

        public MessageService(
            IActivityService activityService,
            ISuiteUserService suiteUserService,
            IRoleService roleService,
            ITenantService tenantService,
            IWildcardService wildcardService,
            IOptions<AwsOptions> awsOptions)
        {
            _activityService = activityService;
            _suiteUserService = suiteUserService;
            _roleService = roleService;
            _tenantService = tenantService;
            _wildcardService = wildcardService;
            _awsOptions = awsOptions.Value;
        }

        public async Task<MailMessage> CreateMessage(int tenantId, int activityId, int requesterId, int taskId)
        {
            var listUsers = await ListSuiteUser(tenantId);
            var activity = await _activityService.GetByTenant(activityId, tenantId);
            ActivityInfo info = activity.Value;
            TaskInfo taskInfo = info.Tasks.FirstOrDefault(x => x.Id == taskId);
            var titleMessageNotifiction = _wildcardService.FormatDescriptionWildcard(info.ActivityNotification.TitleMessage, taskInfo.Flow, listUsers);
            var messageNotifiction = _wildcardService.FormatDescriptionWildcard(info.ActivityNotification.Message, taskInfo.Flow, listUsers);
            
            titleMessageNotifiction = Regex.Replace(titleMessageNotifiction, @"[\n]+", "");
            messageNotifiction = Regex.Replace(messageNotifiction, @"[\n]", "<br />");

            var message = new MailMessage()
            {
                Sender = GetSenderAddress(),
                Subject = titleMessageNotifiction,
                To = await GetAddressTo(info, tenantId, requesterId, listUsers),
                Body = new MailBody()
                {
                    Html = messageNotifiction,
                    Text = messageNotifiction
                }
            };

            return message;
        }

        private string GetSenderAddress()
        {
            return $"iBPMS Satelitti <{ _awsOptions.SES.SenderAddress }>";
        }

        private async Task<List<string>> GetAddressTo(ActivityInfo activity, int tenantId, int requesterId, IList<SuiteUserViewModel> listUsers)
        {
            List<int> users = new();

            switch (activity.ActivityNotification.DestinataryType)
            {
                case SendTaskDestinataryTypeEnum.REQUESTER:
                    users.Add(requesterId);
                    break;
                case SendTaskDestinataryTypeEnum.PERSON:
                    users.Add(activity.ActivityNotification.PersonId ?? 0);
                    break;
                case SendTaskDestinataryTypeEnum.CUSTOM:
                    return new List<string>
                    {
                        activity.ActivityNotification.CustomEmail
                    };
                case SendTaskDestinataryTypeEnum.ROLE:
                    var role = await _roleService.Get(activity.ActivityNotification.RoleId.Value, tenantId);
                    users = role.Value.RoleUsers.Select(u => u.UserId).ToList();
                    break;
            }

            return listUsers.Where(x => users.Contains(x.Id)).Select(x => x.Mail).ToList();

        }

        private async Task<IList<SuiteUserViewModel>> ListSuiteUser(int tenantId)
        {
            var tenant = _tenantService.Get(tenantId);

            var listSuiteUserResult = await _suiteUserService.ListWithoutContext(new SuiteUserListFilter()
            {
                TenantSubDomain = tenant.SubDomain,
                TenantAccessKey = tenant.AccessKey
            });

            return listSuiteUserResult;
        }
    }
}
