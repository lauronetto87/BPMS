using AutoMapper;
using FluentValidation.Results;
using Microsoft.EntityFrameworkCore;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Exceptions;
using SatelittiBpms.Models.Filters;
using SatelittiBpms.Models.HandleException;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Helpers;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Services.Interfaces.Integration;
using SatelittiBpms.Services.ProcessVersionValidation;
using SatelittiBpms.Workflow.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Services
{
    public class ProcessVersionService : AbstractServiceBase<ProcessVersionDTO, ProcessVersionInfo, IProcessVersionRepository>, IProcessVersionService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly IProcessService _processService;
        private readonly IWorkflowValidationService _workflowValidationService;
        private readonly IProcessRoleService _processRoleService;
        private readonly IActivityService _activityService;
        private readonly IXmlDiagramParseService _xmlDiagramParseService;
        private readonly IWorkflowHostService _workflowHostService;
        private readonly IFlowService _flowService;
        private readonly ISuiteUserService _suiteUserService;
        private readonly ITenantService _tenantService;
        private readonly ISignerIntegrationActivityService _signerIntegrationActivityService;

        public ProcessVersionService(
            IProcessVersionRepository repository,
            IMapper mapper,
            IContextDataService<UserInfo> contextDataService,
            IProcessService processService,
            IWorkflowValidationService workflowValidationService,
            IProcessRoleService processRoleService,
            IActivityService activityService,
            IXmlDiagramParseService xmlDiagramParseService,
            IWorkflowHostService workflowHostService,
            IFlowService flowService,
            ISuiteUserService suiteUserService,
            ITenantService tenantService,
            ISignerIntegrationActivityService signerIntegrationActivityService
            ) : base(repository, mapper)
        {
            _contextDataService = contextDataService;
            _processService = processService;
            _workflowValidationService = workflowValidationService;
            _processRoleService = processRoleService;
            _activityService = activityService;
            _xmlDiagramParseService = xmlDiagramParseService;
            _workflowHostService = workflowHostService;
            _flowService = flowService;
            _suiteUserService = suiteUserService;
            _tenantService = tenantService;
            _signerIntegrationActivityService = signerIntegrationActivityService;
        }

        public async Task<ResultContent> GetByTenant(int processVersionId)
        {
            var context = _contextDataService.GetContextData();
            var process = await _repository.GetByIdAndTenantId(processVersionId, context.Tenant.Id);
            return Result.Success(process.AsEditViewModel());
        }

        public async Task<ResultContent> Save(ProcessVersionDTO dto)
        {
            var contextData = _contextDataService.GetContextData();
            dto.SetTenantId(contextData.Tenant.Id);

            Models.BpmnIo.Definitions bpmnDefinitions = null;
            if (!string.IsNullOrWhiteSpace(dto.DiagramContent))
            {
                dto.DiagramContent = dto.DiagramContent
                    .Replace("satelitti:executorId=\"\"", string.Empty)
                    .Replace("satelitti:personId=\"\"", string.Empty)
                    .Replace("satelitti:destinataryId=\"\"", string.Empty)
                    .Replace("satelitti:executorId=\"NaN\"", string.Empty)
                    .Replace("satelitti:personId=\"NaN\"", string.Empty)
                    .Replace("satelitti:signerIntegrationChangesNeeded=\"undefined\"", "satelitti:signerIntegrationChangesNeeded=\"false\"");

                bpmnDefinitions = BpmnIoSerializerHelper.Deserialize(dto.DiagramContent);
            }

            try
            {
                ProcessVersionInfo processVersionNewInfo = null;
                using (var transaction = _repository.BeginTransaction())
                {
                    try
                    {
                        ProcessInfo processInfo = null;
                        ProcessVersionInfo processVersionInEditInfo = null;
                        bool processIsSaved = false;

                        if (dto.ProcessId != 0)
                        {
                            var processGetResult = await _processService.GetByTenant(dto.ProcessId);
                            if (!processGetResult.Success)
                            {
                                return Result.Error(ExceptionCodes.PROCESS_VERSION_SAVE_GET_PROCESS_ERROR);
                            }
                            processInfo = processGetResult.Value;
                            processIsSaved = processInfo != null;
                            if (processIsSaved)
                            {
                                processVersionInEditInfo = await _repository.GetByProcessAndStatus(processInfo.Id, ProcessStatusEnum.EDITING, processInfo.TenantId.Value);
                            }
                        }

                        var validators = new List<ProcessValidationBase>
                        {
                            new ProcessNameDuplicateValidation(_repository, _contextDataService, _flowService, dto.Name, processVersionInEditInfo?.Id ?? 0, processInfo?.Id ?? 0),
                        };
                        if (dto.NeedPublish)
                        {
                            if (bpmnDefinitions != null)
                            {
                                validators.Add(new FieldVisibilityValidationToSignIntegration(bpmnDefinitions, dto));
                                validators.Add(new ValidateIfUserTaskWillAlwaysRunForSsignIntegrationTaskValidator(bpmnDefinitions, dto));
                            }
                            validators.Add(new SignerIntegrationActivityValidator(dto));
                        }


                        var validationResult = new ValidationResult();
                        foreach (var validator in validators)
                        {
                            var errors = validator.Validate();
                            validationResult.Errors.AddRange(errors);
                        }
                        if (!validationResult.IsValid)
                        {
                            return Result.Error(validationResult, true);
                        }


                        processVersionNewInfo = _mapper.Map<ProcessVersionInfo>(dto);

                        if (processIsSaved)
                        {
                            if (processVersionInEditInfo != null)
                            {
                                foreach (var activity in processVersionInEditInfo.Activities.ToList())
                                    await _activityService.Delete(activity.Id);

                                foreach (var roleUser in processVersionInEditInfo.ProcessVersionRoles.ToList())
                                    await _processRoleService.Delete(roleUser.Id);

                                processVersionNewInfo = _mapper.Map(processVersionNewInfo, processVersionInEditInfo);
                                await VersionAndUpdate(processVersionNewInfo);
                            }
                            else
                            {
                                int maxVersion = processInfo.ProcessVersions.Max(x => x.Version);
                                processVersionNewInfo.Version = maxVersion + 1;
                                var processVersionInsertResult = await VersionAndInsert(processVersionNewInfo);
                            }

                            await _processService.UpdateTaskSequance(dto.ProcessId, dto.TaskSequance);
                        }
                        else
                        {
                            var processInsertResult = await _processService.Insert(_mapper.Map<ProcessDTO>(dto));
                            if (!processInsertResult.Success)
                            {
                                return Result.Error(ExceptionCodes.PROCESS_VERSION_SAVE_INSERT_PROCESS_ERROR);
                            }

                            dto.ProcessId = processInsertResult.Value;
                            processVersionNewInfo.ProcessId = processInsertResult.Value;
                            var processVersionInsertResult = await VersionAndInsert(processVersionNewInfo);
                        }

                        await _processRoleService.InsertMany(dto.RolesIds.ToList(), processVersionNewInfo.Id, contextData.Tenant.Id);
                        await _activityService.InsertMany(dto.Activities.ToList(), processVersionNewInfo.Id, contextData.Tenant.Id);


                        await SaveIntegrationActivitiesWithSigner(bpmnDefinitions, processVersionNewInfo.Id);

                        if (dto.SignerTasks != null && dto.SignerTasks.Count > 0)
                        {
                            var processVersionSaved = _repository.GetQuery(processVersionNewInfo.Id)
                                                    .Include(p => p.Activities).ThenInclude(a => a.SignerIntegrationActivity.Files)
                                                    .Include(p => p.Fields)
                                                    .First();
                            await _signerIntegrationActivityService.InsertMany(dto.SignerTasks, processVersionSaved, bpmnDefinitions);
                        }

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }

                if (dto.NeedPublish)
                {
                    XmlNode processNode = GetProcessXmlNode(dto.DiagramContent);
                    ResultContent validationResult = _workflowValidationService.Validate(processNode);
                    if (!validationResult.Success)
                        return Result.Error(dto.ProcessId, validationResult.ValidationResult, true);

                    await _activityService.InsertByDiagram(processNode, processVersionNewInfo.Id, contextData.Tenant.Id);
                    string workflowJsonAsString = await _xmlDiagramParseService.Parse(processNode, processVersionNewInfo.ProcessId, processVersionNewInfo.Version, processVersionNewInfo.Id, contextData.Tenant.Id);
                    _workflowHostService.LoadWorkflowFromJson(workflowJsonAsString);
                    await _repository.UpdateStatusAndWorkflowContent(processVersionNewInfo.Id, ProcessStatusEnum.PUBLISHED, workflowJsonAsString);
                    await _processService.UpdateCurrentVersion(dto.ProcessId);
                }
                return Result.Success(processVersionNewInfo.ProcessId);
            }
            catch (ActivityTypeNotExpectedException ex)
            {
                AddErrors(ExceptionCodes.ACTIVITY_TYPE_NOT_EXPECTED, ExceptionCodes.ACTIVITY_TYPE_NOT_EXPECTED, new { name = ex.ActivityTypeName });
                return Result.Error(dto.ProcessId, ValidationResult);
            }
            catch (Exception ex)
            {
                AddErrors(ExceptionCodes.PROCESS_VERSION_SAVE_TRANSACTION_ERROR, ex.Message);
                return Result.Error(dto.ProcessId, ValidationResult);
            }
        }

        private async Task SaveIntegrationActivitiesWithSigner(Models.BpmnIo.Definitions bpmnDefinitions, int processVersionId)
        {
            if (bpmnDefinitions == null)
            {
                return;
            }
            var contextData = _contextDataService.GetContextData();

            foreach (var signer in bpmnDefinitions.Process.SatelittiSigner)
            {
                var activityId = _activityService.GetId(signer.Name, processVersionId, contextData.Tenant.Id);

                var activityDTO = new ActivityDTO
                {
                    ActivityId = signer.Id,
                    ActivityName = signer.Name,
                    ActivityType = WorkflowActivityTypeEnum.SIGNER_TASK,
                    ProcessVersionId = processVersionId,
                };
                activityDTO.SetTenantId(contextData.Tenant.Id);
                if (activityId != null && activityId > 0)
                {
                    await _activityService.Update(activityId.Value, activityDTO);
                }
                else
                {
                    await _activityService.Insert(activityDTO);
                }
            }
        }

        public ResultContent IsNameValidCheckDuplicate(string processName, int editProcessVersionId)
        {
            List<ValidationFailure> errors;
            if (editProcessVersionId <= 0)
            {
                errors = new ProcessNameDuplicateValidation(_repository, _contextDataService, _flowService, processName, 0, 0).Validate();
            }
            else
            {
                var processId = _repository.Get(editProcessVersionId).Result.ProcessId;
                errors = new ProcessNameDuplicateValidation(_repository, _contextDataService, _flowService, processName, editProcessVersionId, processId).Validate();
            }
            if (errors != null && errors.Count > 0)
            {
                var validationResult = new ValidationResult();
                validationResult.Errors.AddRange(errors);
                return Result.Error(validationResult);
            }
            return Result.Success();
        }

        private static XmlNode GetProcessXmlNode(string diagramContent)
        {
            var bpmnXml = new XmlDocument();
            bpmnXml.LoadXml(diagramContent);
            return bpmnXml.GetElementsByTagName("bpmn2:process")[0];
        }

        public async Task<ResultContent> UpdateWorkFlowContent(int Id, string workflowContent)
        {
            var process = await _repository.Get(Id);
            process.WorkflowContent = workflowContent;
            await _repository.Update(process);
            return Result.Success();
        }

        private async Task VersionAndUpdate(ProcessVersionInfo info)
        {
            if (info.Status == ProcessStatusEnum.PUBLISHED)
                info.Version = +1;
            info.Status = ProcessStatusEnum.EDITING;
            info.LastModifiedDate = DateTime.UtcNow;
            info.CreatedByUserId = _contextDataService.GetContextData().User.Id;
            info.CreatedByUserName = await GetCurrentUserName();
            await _repository.Update(info);
        }

        private async Task<int> VersionAndInsert(ProcessVersionInfo info)
        {
            if (info.Status == ProcessStatusEnum.PUBLISHED)
                info.Version = +1;
            info.Status = ProcessStatusEnum.EDITING;
            info.LastModifiedDate = DateTime.UtcNow;
            info.CreatedDate = DateTime.UtcNow;
            info.CreatedByUserId = _contextDataService.GetContextData().User.Id;
            info.CreatedByUserName = await GetCurrentUserName();
            return await _repository.Insert(info);
        }


        private async Task<string> GetCurrentUserName()
        {
            var context = _contextDataService.GetContextData();
            var userId = context.User.Id;
            var tenant = _tenantService.Get(context.Tenant.Id);

            var listSuiteUserResult = await _suiteUserService.ListWithoutContext(new SuiteUserListFilter()
            {
                TenantSubDomain = tenant?.SubDomain,
                TenantAccessKey = tenant?.AccessKey,
                SuiteToken = context.SuiteToken,
                InUserIds = new System.Collections.Generic.List<int>
                {
                    userId,
                }
            });
            return listSuiteUserResult?.FirstOrDefault(u => u.Id == userId)?.Name;
        }
    }
}
