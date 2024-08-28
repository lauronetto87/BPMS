using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using SatelittiBpms.FluentDataBuilder.FlowExecute.Data;
using SatelittiBpms.FluentDataBuilder.FlowExecute.Helpers;
using SatelittiBpms.FluentDataBuilder.Process;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Enums;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test;
using SatelittiBpms.Test.Data;
using SatelittiBpms.Test.Extensions;
using SatelittiBpms.Test.Helpers;
using SatelittiBpms.Workflow.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WorkflowCore.Interface;
using WorkflowCore.Models;

namespace SatelittiBpms.Tests.Executors
{
    public class FlowCollectionExecutor
    {
        private readonly MockServices _mockServices;
        private readonly ContextBuilder _contextBuilder;
        private readonly FlowCollectionData _flowCollectionData;
        private readonly IWorkflowHost _workflowHost;
        private readonly Bogus.Faker faker = new();
        readonly DbContext dbContext;
        private ProcessVersionInfo _processVersionInfo;
        private Exception _workflowHostLastStepException;

        public FlowCollectionExecutor(ContextBuilder contextBuilder, FlowCollectionData flowCollectionData)
        {
            _mockServices = (MockServices)contextBuilder.ExtendData;
            _contextBuilder = contextBuilder;
            _flowCollectionData = flowCollectionData;
            _workflowHost = _mockServices.GetService<IWorkflowHost>();
            dbContext = _mockServices.GetService<DbContext>();

            _workflowHost.OnStepError += (WorkflowInstance workflow, WorkflowStep step, Exception exception) =>
            {
                _workflowHostLastStepException = exception;
            };
        }


        public async Task<List<FlowExecutedData>> Execute()
        {
            var flowsExecuted = new List<FlowExecutedData>();
            _processVersionInfo = await ProcessVersionExecutor.Save(_mockServices, _flowCollectionData.ProcessVersionData.AsDto());
            foreach (var flowData in _flowCollectionData.FlowsData)
            {
                flowsExecuted.Add(await ExecuteFlowData(flowData));
            }
            return flowsExecuted;
        }

        private async Task<FlowExecutedData> ExecuteFlowData(FlowData flowData)
        {
            var flowExecutedData = await RequestNewFlow(flowData);
            await RunTaskData(flowExecutedData);
            SetFlowInfo(flowExecutedData.FlowId, flowExecutedData);
            return flowExecutedData;
        }

        private async Task<FlowExecutedData> RequestNewFlow(FlowData flowData)
        {
            var flowService = _mockServices.GetService<IFlowService>();

            var flowRequestDTO = new FlowRequestDTO()
            {
                ProcessId = _processVersionInfo.ProcessId,
                ConnectionId = _mockServices.ConnectionId,
            };
            var workFlowRequestResult = await flowService.Request(flowRequestDTO);
            Assert.IsTrue(workFlowRequestResult.Success);

            var workflowInstanceId = workFlowRequestResult.Value;

            FlowDataInfo flowDataInfo = null;
            await ThreadHelper.WaitUntil(async () =>
            {
                CheckErrorOnStep();
                var requestFinalized = false;
                var workflowInstance = await _workflowHost.PersistenceStore.GetWorkflowInstance(workflowInstanceId);

                var completed = workflowInstance.Status == WorkflowStatus.Complete;
                if (completed)
                {
                    flowDataInfo = (FlowDataInfo)workflowInstance.Data;
                    requestFinalized = true;
                }
                else
                {
                    var waitForEvent = workflowInstance.ExecutionPointers.All(p => p.Status == PointerStatus.Complete || p.Status == PointerStatus.WaitingForEvent || p.Status == PointerStatus.Cancelled);
                    if (waitForEvent)
                    {
                        flowDataInfo = (FlowDataInfo)workflowInstance.Data;
                        requestFinalized = true;
                    }
                }
                CheckErrorOnStep();
                return requestFinalized;
            });
            return new FlowExecutedData(flowData)
            {
                WorkflowInstanceId = workflowInstanceId,
                FlowDataInfoRequestFlow = flowDataInfo,
                FlowId = flowDataInfo.FlowId,
            };
        }


        private void CheckErrorOnStep()
        {
            var error = _workflowHostLastStepException;
            if (error != null)
            {
                throw error;
            }
        }

        private async Task RunTaskData(FlowExecutedData flowExecutedData)
        {
            foreach (var taskData in flowExecutedData.FlowData.Tasks)
            {
                var taskIdToExecute = await GetFirstPendingTaskIdOrNull(flowExecutedData.FlowId);
                if (taskIdToExecute == null)
                {
                    return;
                }
                await RunTask(taskData, taskIdToExecute.Value, flowExecutedData);
            }
        }

        private async Task<int?> GetFirstPendingTaskIdOrNull(int flowId)
        {
            var flow = await dbContext.Set<TaskInfo>()
                .Where(f => f.FlowId == flowId && f.FinishedDate == null)
                .FirstOrDefaultAsync();
            return flow?.Id;
        }

        private async Task RunTask(FlowTaskData flowTaskData, int taskId, FlowExecutedData flowExecutedData)
        {
            var taskExecuted = new TaskExecutedData()
            {
                TaskId = taskId,
                TaskInput = flowTaskData,
            };

            var task = await dbContext.Set<TaskInfo>()
                .Where(t => t.Id == taskId)
                .Include(t => t.FieldsValues).ThenInclude(f => f.FieldValueFiles)
                .Include(t => t.Activity.ActivityFields).ThenInclude(af => af.Field)
                .Include(t => t.Activity.ActivityUser.ActivityUsersOptions)
                .FirstAsync();

            Models.Result.ResultContent resultTaskResult;
            if (task.Activity.Type == WorkflowActivityTypeEnum.SIGNER_TASK)
            {
                resultTaskResult = await _mockServices.GetService<ITaskService>().ExecuteTaskSignerIntegration(task.Id);
            }
            else
            {
                if (task.Activity.Type == WorkflowActivityTypeEnum.USER_TASK_ACTIVITY && task.Activity.ActivityUser.ExecutorType == UserTaskExecutorTypeEnum.ROLE)
                {
                    var roleId = task.Activity.ActivityUser.RoleId;
                    if (roleId == null)
                    {
                        throw new Exception("A atividade está configurada para usuário de um papel, mas não foi informado o papel.");
                    }
                    // TODO Colocar para pegar um usuário dinâmico da lista de papel
                    await _mockServices.GetService<ITaskService>().Assign(taskId);
                }

                var nextStepDTO = await GenerateNextStepDto(task, taskExecuted);
                resultTaskResult = await _mockServices.GetService<ITaskService>().NextTask(nextStepDTO);
            }


            Assert.IsTrue(resultTaskResult.Success);

            FlowDataInfo flowDataInfo = null;

            var workflowHost = _mockServices.GetService<IWorkflowHost>();
            await ThreadHelper.WaitUntil(async () =>
            {
                CheckErrorOnStep();
                var workflowInstance = await workflowHost.PersistenceStore.GetWorkflowInstance(flowExecutedData.WorkflowInstanceId);

                var completed = workflowInstance.Status == WorkflowStatus.Complete;
                if (completed)
                {
                    flowDataInfo = (FlowDataInfo)workflowInstance.Data;
                    return true;
                }
                var waitForEvent = workflowInstance.ExecutionPointers.All(p => p.Status == PointerStatus.Complete || p.Status == PointerStatus.WaitingForEvent || p.Status == PointerStatus.Cancelled);
                if (waitForEvent)
                {
                    var enventUser = new EventUserInfo(taskId);
                    if (workflowInstance.ExecutionPointers.Any(p => p.Active || (!p.EventPublished && p.EventKey == enventUser.eventKey && p.EventName == enventUser.eventName)))
                    {
                        return false;
                    }
                    flowDataInfo = (FlowDataInfo)workflowInstance.Data;
                    return true;
                }
                CheckErrorOnStep();
                return false;
            });

            taskExecuted.FlowDataInfo = flowDataInfo;
            flowExecutedData.Tasks.Add(taskExecuted);
        }

        private async Task<NextStepDTO> GenerateNextStepDto(TaskInfo task, TaskExecutedData taskExecuted)
        {
            var option = faker.PickRandom(task.Activity.ActivityUser.ActivityUsersOptions);
            var fields = task.Activity.ActivityFields.Select(f => f.Field).ToList();

            taskExecuted.OptionButton = option;
            taskExecuted.TaskInfo = task;

            var nextStepDTO = new NextStepDTO()
            {
                OptionId = option.Id,
                TaskId = task.Id,
                FormData = await GenerateFormData(fields, taskExecuted),
            };

            return nextStepDTO;
        }

        private async Task<JObject> GenerateFormData(List<FieldInfo> fields, TaskExecutedData taskExecuted)
        {
            JObject formValue = new();
            var activityFields = taskExecuted.TaskInfo.Activity.ActivityFields;

            foreach (var field in fields)
            {
                var taskFieldValue = taskExecuted.TaskInput.FieldValues.FirstOrDefault(taskFieldValue => taskFieldValue.FieldId.InternalId.Contains(field.ComponentInternalId));
                if (taskFieldValue != null)
                {
                    if (taskFieldValue.Value == null)
                    {
                        formValue.Add(field.ComponentInternalId, JValue.CreateNull());
                    }
                    else
                    {
                        if (field.Type == FieldTypeEnum.FILE)
                        {
                            if (taskFieldValue.Value is IEnumerable<FileToFieldValueDTO> files)
                            {
                                var _fieldValueFileService = _mockServices.GetService<IFieldValueFileService>();
                                foreach (var fileToFieldValueDTO in files)
                                {
                                    await _fieldValueFileService.Insert(fileToFieldValueDTO);
                                }
                                formValue.Add(field.ComponentInternalId, GenerateFormValueForFiles(files));
                            }
                            else
                            {
                                throw new ArgumentException($"O valor do Field para um campo de arquivo tem de ser um enumerado de {nameof(FileToFieldValueDTO)}.");
                            }
                        }
                        else
                        {
                            formValue.Add(field.ComponentInternalId, new JValue(taskFieldValue.Value.ToString()));
                        }
                    }
                }
                else
                {
                    var valueForField = await GenerateNewValueOrGetValueIfNotEditable(activityFields, field, taskExecuted.TaskInfo);

                    taskExecuted.TaskInput.FieldValues.Add(new FlowFieldValue(new FluentDataBuilder.DataId(field.ComponentInternalId), valueForField));

                    JToken valueForFieldJson;
                    if (field.Type == FieldTypeEnum.FILE)
                    {
                        valueForFieldJson = GenerateFormValueForFiles((IEnumerable<FileToFieldValueDTO>)valueForField);
                    }
                    else if (valueForField == null)
                    {
                        valueForFieldJson = JValue.CreateNull();
                    }
                    else
                    {
                        valueForFieldJson = new JValue(valueForField);
                    }
                    formValue.Add(field.ComponentInternalId, valueForFieldJson);
                }
            }

            return formValue;
        }

        private static JArray GenerateFormValueForFiles(IEnumerable<FileToFieldValueDTO> valueForField)
        {
            var newValue = new JArray();
            foreach (var fileDto in valueForField)
            {
                var fieldValueItem = new JObject
                {
                    { "key", new JValue(fileDto.FileName) },
                    { "size", new JValue(fileDto.Stream?.Length ?? 0) },
                    { "type", new JValue(fileDto.FileContentType) },
                    { "originalName", new JValue(fileDto.FileName) },
                };
                newValue.Add(fieldValueItem);
            }

            return newValue;
        }

        private async Task<object> GenerateNewValueOrGetValueIfNotEditable(IList<ActivityFieldInfo> activityFields, FieldInfo field, TaskInfo taskInfo)
        {
            var activityField = activityFields.FirstOrDefault(a => a.Field.ComponentInternalId == field.ComponentInternalId);
            object valueForField;

            if (activityField != null
                && (activityField.State == ProcessTaskFieldStateEnum.ONLYREADING || activityField.State == ProcessTaskFieldStateEnum.INVISIBLE))
            {
                var fieldValue = taskInfo.FieldsValues.FirstOrDefault(v => v.FieldId == activityField.FieldId);
                if (fieldValue == null)
                {
                    valueForField = null;
                }
                else
                {
                    if (activityField.Field.Type == FieldTypeEnum.FILE)
                    {
                        valueForField = fieldValue.FieldValueFiles?.Select(x => FileInfoToDto(x, activityField.Field.ComponentInternalId))?.ToList();
                    }
                    else
                    {
                        valueForField = fieldValue.FieldValue;
                    }
                }
            }
            else
            {
                if (field.Type == FieldTypeEnum.FILE)
                {
                    var fieldValue = taskInfo.FieldsValues.FirstOrDefault(v => v.FieldId == activityField.FieldId);

                    var files = fieldValue?.FieldValueFiles?.Select(x => FileInfoToDto(x, activityField.Field.ComponentInternalId))?.ToList() ?? new List<FileToFieldValueDTO>();

                    var minFileCount = 0;
                    if (activityField?.State == ProcessTaskFieldStateEnum.MANDATORY)
                    {
                        minFileCount = 1;
                    }
                    var totalFile = faker.Random.Int(minFileCount, 4);
                    for (int i = 0; i < totalFile; i++)
                    {
                        files.Add(await InsertFileToTask(field, taskInfo.Id));
                    }
                    valueForField = files;
                }
                else
                {
                    valueForField = FieldHelper.GenerateValueByType(field.Type);
                }
            }

            return valueForField;
        }

        private static FileToFieldValueDTO FileInfoToDto(FieldValueFileInfo fieldValueFileInfo, string componentInternalId)
        {
            return new FileToFieldValueDTO
            {
                ComponentInternalId = componentInternalId,
                FileContentType = fieldValueFileInfo.Type,
                FileName = fieldValueFileInfo.Name,
                TaskId = fieldValueFileInfo?.FieldValue?.TaskId ?? 0,
            };
        }

        private async Task<FileToFieldValueDTO> InsertFileToTask(FieldInfo field, int taskId)
        {
            var _fieldValueFileService = _mockServices.GetService<IFieldValueFileService>();

            var mimeType = faker.System.FileType();
            var extension = faker.System.FileExt(mimeType);

            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write(faker.Lorem.Lines());
            writer.Flush();
            stream.Position = 0;

            var fileToFieldValueDto = new FileToFieldValueDTO
            {
                ComponentInternalId = field.ComponentInternalId,
                FileContentType = mimeType,
                FileName = faker.System.FileName(extension),
                Stream = stream,
                TaskId = taskId,
            };
            await _fieldValueFileService.Insert(fileToFieldValueDto);
            return fileToFieldValueDto;
        }

        private void SetFlowInfo(int flowId, FlowExecutedData flowExecutedData)
        {
            // Limpa o rastreamento porque a thread do motor atualiza os dados e ao efetuar a busca era pego do cache
            // Uma outra opção é colocar o .AsNoTracking(), mas ocorre o seguinte erro em alguns casos:
            // The Include path 'SourceTasks->TargetTask' results in a cycle. Cycles are not allowed in no-tracking queries; either use a tracking query or remove the cycle.
            dbContext.ChangeTracker.Clear();
            var flowInfo = dbContext.Set<FlowInfo>()
                .Include(f => f.Tasks)
                    .ThenInclude(f => f.Activity.ActivityUser)
                .Include(f => f.Tasks).ThenInclude(f => f.FieldsValues).ThenInclude(f => f.FieldValueFiles).ThenInclude(f => f.TaskSignerFile.TaskSigner)
                .Include(f => f.Tasks).ThenInclude(f => f.SourceTasks)
                .Include(f => f.Tasks).ThenInclude(f => f.TargetTasks)
                .Include(f => f.FieldValues)
                    .ThenInclude(f => f.Field)
                .Include(f => f.FlowPaths)
                .Include(f => f.ProcessVersion.Process)
                .Include(f => f.Requester)
                .Where(f => f.Id == flowId)
                .First();

            flowExecutedData.FlowInfo = flowInfo;
            AdjustReferenceTasksFromFlow(flowExecutedData, flowInfo);
        }

        private static void AdjustReferenceTasksFromFlow(FlowExecutedData flowExecutedData, FlowInfo flowInfo)
        {
            foreach (var task in flowExecutedData.Tasks)
            {
                task.TaskInfo = flowInfo.Tasks.First(t => t.Id == task.TaskId);
            }
        }
    }
}
