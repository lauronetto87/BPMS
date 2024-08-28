using AutoMapper;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class SignerIntegrationActivityService : AbstractServiceBase<SignerIntegrationActivityDTO, SignerIntegrationActivityInfo, ISignerIntegrationActivityRepository>, ISignerIntegrationActivityService
    {
        readonly IContextDataService<UserInfo> _contextDataService;
        public SignerIntegrationActivityService(
            ISignerIntegrationActivityRepository repository,
            IMapper mapper,
            IContextDataService<UserInfo> contextDataService
            ) : base(repository, mapper)
        {
            _contextDataService = contextDataService;
        }

        public async Task InsertMany(IList<SignerIntegrationActivityDTO> signerActivities, ProcessVersionInfo processVersion, Models.BpmnIo.Definitions bpmnDefinitions)
        {
            var tenantId = _contextDataService.GetContextData().Tenant.Id;
            foreach (var activityDto in signerActivities)
            {
                var signerInfo = _mapper.Map<SignerIntegrationActivityInfo>(activityDto);

                signerInfo.ActivityId = GetActivityFromKeyRequired(activityDto.ActivityKey, processVersion, nameof(activityDto.ActivityKey));
                signerInfo.TenantId = tenantId;
                signerInfo.ExpirationDateFieldId = GetFieldFromKey(activityDto.ExpirationDateFieldKey, processVersion, nameof(activityDto.ExpirationDateFieldKey));
                signerInfo.Files = activityDto.FileFieldKeys.Select(fieldComponentInternalId => new SignerIntegrationActivityFileInfo
                {
                    FileFieldId = GetFieldFromKey(fieldComponentInternalId, processVersion, nameof(activityDto.FileFieldKeys)) ?? 0,
                    TenantId = tenantId,
                }).ToList();

                signerInfo.Signatories = activityDto.Signatories.Select(dto => new SignerIntegrationActivitySignatoryInfo
                {
                    CpfFieldId = GetFieldFromKey(dto.CpfFieldKey, processVersion, nameof(dto.CpfFieldKey)),
                    EmailFieldId = GetFieldFromKey(dto.EmailFieldKey, processVersion, nameof(dto.EmailFieldKey)),
                    NameFieldId = GetFieldFromKey(dto.NameFieldKey, processVersion, nameof(dto.NameFieldKey)),
                    RegistrationLocation = dto.RegistrationLocation,
                    SignatureTypeId = dto.SignatureTypeId,
                    SubscriberTypeId = dto.SubscriberTypeId,
                    TenantId = tenantId,
                    OriginActivityId = GetActivityFromKey(dto.OriginActivityId, processVersion, nameof(activityDto.ActivityKey)),
                }).ToList();

                signerInfo.Authorizers = activityDto.Authorizers.Select(dto => new SignerIntegrationActivityAuthorizerInfo
                {
                    CpfFieldId = GetFieldFromKey(dto.CpfFieldKey, processVersion, nameof(dto.CpfFieldKey)),
                    EmailFieldId = GetFieldFromKey(dto.EmailFieldKey, processVersion, nameof(dto.EmailFieldKey)),
                    NameFieldId = GetFieldFromKey(dto.NameFieldKey, processVersion, nameof(dto.NameFieldKey)),
                    RegistrationLocation = dto.RegistrationLocation,
                    TenantId = tenantId,
                    OriginActivityId = GetActivityFromKey(dto.OriginActivityId, processVersion, nameof(activityDto.ActivityKey)),
                }).ToList();

                await _repository.Insert(signerInfo);
            }

        }

        private int? GetFieldFromKey(string fieldComponentInternalId, ProcessVersionInfo processVersion, string nameOfProperty)
        {
            if (string.IsNullOrWhiteSpace(fieldComponentInternalId))
            {
                return null;
            }
            var field = processVersion.Fields.FirstOrDefault(f => f.ComponentInternalId == fieldComponentInternalId);
            if (field == null)
            {
                throw new System.ArgumentException($"Não econtrado campo no formulário de chave {fieldComponentInternalId} para a propriedade {nameOfProperty}.");
            }
            return field.Id;
        }

        private int? GetActivityFromKey(string activityId, ProcessVersionInfo processVersion, string nameOfProperty)
        {
            if (string.IsNullOrWhiteSpace(activityId))
            {
                return null;
            }
            var actvity = processVersion.Activities.FirstOrDefault(a => a.ComponentInternalId == activityId);
            if (actvity == null)
            {
                throw new System.ArgumentException($"Não econtrado atividade no fluxo de chave {activityId} para a propriedade {nameOfProperty}");
            }
            return actvity.Id;
        }

        private int GetActivityFromKeyRequired(string activityId, ProcessVersionInfo processVersion, string nameOfProperty)
        {
            var id = GetActivityFromKey(activityId, processVersion, nameOfProperty);
            if (id == null)
            {
                throw new System.ArgumentException($"Não foi informado a atividade no fluxo para a propriedade {nameOfProperty}");
            }
            return id.Value;
        }
    }
}
