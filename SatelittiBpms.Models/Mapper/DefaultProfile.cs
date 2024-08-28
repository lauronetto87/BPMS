using AutoMapper;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Extensions;
using SatelittiBpms.Models.Infos;

namespace SatelittiBpms.Models.Mapper
{
    public class DefaultProfile : Profile
    {
        public DefaultProfile()
        {
            #region USER
            CreateMap<UserDTO, UserInfo>();
            #endregion

            #region ROLE
            CreateMap<RoleDTO, RoleInfo>().Ignore(x => x.Id);
            CreateMap<RoleInfo, RoleInfo>().Ignore(x => x.Id);
            #endregion

            #region ROLE_USER
            CreateMap<RoleUserDTO, RoleUserInfo>().Ignore(x => x.Id);
            #endregion

            #region PROCESS
            CreateMap<ProcessDTO, ProcessInfo>().Ignore(x => x.Id);
            CreateMap<ProcessVersionDTO, ProcessDTO>(MemberList.None)
                .ForMember(dest => dest.TenantId, opt => opt.MapFrom(src => src.TenantId))
                .ForMember(dest => dest.TaskSequance, opt => opt.MapFrom(src => src.TaskSequance));
            #endregion

            #region PROCESS VERSION
            CreateMap<ProcessVersionInfo, ProcessVersionInfo>()
                .Ignore(x => x.Id)
                .Ignore(x => x.Version)
                .Ignore(x => x.CreatedDate);
            CreateMap<ProcessVersionDTO, ProcessVersionInfo>()
                .Ignore(x => x.Id)
                .Ignore(x => x.Version)
                .Ignore(x => x.CreatedDate)
                .Ignore(x => x.ProcessVersionRoles)
                .Ignore(x => x.Activities);
            #endregion

            #region PROCESS_ROLE
            CreateMap<ProcessRoleDTO, ProcessVersionRoleInfo>().Ignore(x => x.Id);
            #endregion

            #region ACTIVITY
            CreateMap<ActivityDTO, ActivityDTO>();
            CreateMap<ActivityInfo, ActivityInfo>();
            CreateMap<ActivityDTO, ActivityInfo>().Ignore(x => x.Id)
                .ForMember(dest => dest.ComponentInternalId, act => act.MapFrom(src => src.ActivityId))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.ActivityName))
                .ForMember(dest => dest.Type, act => act.MapFrom(src => src.ActivityType));
            CreateMap<ActivityInfo, ActivityDTO>()
                .ForMember(dest => dest.ActivityId, act => act.MapFrom(src => src.ComponentInternalId))
                .ForMember(dest => dest.ActivityName, act => act.MapFrom(src => src.Name))
                .ForMember(dest => dest.ActivityType, act => act.MapFrom(src => src.Type));
            #endregion

            #region FIELD
            CreateMap<ActivityFieldDTO, FieldDTO>();
            CreateMap<ActivityFieldDTO, ActivityFieldInfo>().Ignore(x => x.Id)
                .ForMember(dest => dest.ProcessVersionId, act => act.MapFrom(src => src.ProcessVersionId))
                .ForMember(dest => dest.ActivityId, act => act.MapFrom(src => src.TaskId))
                .ForMember(dest => dest.FieldId, act => act.MapFrom(src => src.SystemFieldId))
                .ForMember(dest => dest.State, act => act.MapFrom(src => src.State));

            CreateMap<FieldDTO, FieldInfo>().Ignore(x => x.Id)
                .ForMember(dest => dest.ComponentInternalId, act => act.MapFrom(src => src.FieldId))
                .ForMember(dest => dest.Name, act => act.MapFrom(src => src.FieldLabel))
                .ForMember(dest => dest.Type, act => act.MapFrom(src => src.FieldType));
            #endregion

            #region TASK 
            CreateMap<TaskDTO, TaskInfo>().Ignore(x => x.Id);
            CreateMap<TaskInfo, TaskInfo>().Ignore(x => x.Id);
            #endregion

            #region TASK_HISTORY 
            CreateMap<TaskHistoryDTO, TaskHistoryInfo>();
            #endregion

            #region TASK_FIELD

            #endregion

            #region ACTIVE_USER

            #endregion

            #region ACTIVE_USER_OPTION
            CreateMap<ActivityUserOptionDTO, ActivityUserOptionDTO>();
            CreateMap<ActivityUserOptionDTO, ActivityUserOptionInfo>().Ignore(x => x.Id);
            #endregion

            #region TEMPLATE
            CreateMap<TemplateDTO, TemplateInfo>().Ignore(x => x.Id);
            CreateMap<TemplateInfo, TemplateDTO>();
            #endregion

            #region SIGNER
            CreateMap<SignerIntegrationActivityDTO, SignerIntegrationActivityInfo>().Ignore(x => x.Id).Ignore(x => x.Signatories).Ignore(x => x.Authorizers);
            CreateMap<SignerIntegrationActivityInfo, SignerIntegrationActivityDTO>();
            #endregion            
        }
    }
}
