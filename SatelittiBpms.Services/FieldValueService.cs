using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class FieldValueService : IFieldValueService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly ITaskRepository _taskRepository;
        private readonly IFieldValueRepository _fieldValueRepository;
        private readonly ITaskSignerRepository _taskSignerRepository;
        
        public FieldValueService(
            IContextDataService<UserInfo> contextDataService,
            IFieldValueRepository fieldValueRepository,
            ITaskRepository taskRepository,
            ITaskSignerRepository taskSignerRepository)
        {
            _contextDataService = contextDataService;
            _taskRepository = taskRepository;
            _fieldValueRepository = fieldValueRepository;
            _taskSignerRepository = taskSignerRepository;
        }

        public dynamic GetFormatedFormData(IList<FieldValueInfo> fieldValuesList)
        {
            dynamic dados = new System.Dynamic.ExpandoObject();

            if (fieldValuesList != null)
            {
                foreach (FieldValueInfo fieldValue in fieldValuesList)
                    ((IDictionary<string, object>)dados).Add(fieldValue.Field.ComponentInternalId, GetFieldValueAsObject(fieldValue.FieldValue));
            }

            return dados;
        }

        private object GetFieldValueAsObject(string fieldValue)
        {
            if (fieldValue == null)
            {
                return null;
            }
            try
            {
                if (string.IsNullOrWhiteSpace(fieldValue))
                {
                    return fieldValue;
                }
                if (bool.TryParse(fieldValue, out bool result))
                    return result;
                return JToken.Parse(fieldValue);
            }
            catch (JsonReaderException)
            {
                return fieldValue;
            }
        }

        public async Task UpdateFieldValues(int taskId, dynamic formData)
        {
            var context = _contextDataService.GetContextData();
            var taskWithDependencies = await _taskRepository.GetByIdAndTenantId(taskId, context.Tenant.Id);

            string json = JsonConvert.SerializeObject(formData, Formatting.Indented);
            dynamic fieldJsonViewModel = JsonConvert.DeserializeObject(json, new JsonSerializerSettings() { DateParseHandling = DateParseHandling.None });

            if (taskWithDependencies.Flow.ProcessVersion.Fields == null)
            {
                return;
            }

            foreach (FieldInfo field in taskWithDependencies.Flow.ProcessVersion.Fields)
            {
                var fielValue = fieldJsonViewModel[field.ComponentInternalId];

                var fieldValueInfo = taskWithDependencies.FieldsValues?.FirstOrDefault(x => x.FieldId == field.Id);

                if (fieldValueInfo != null)
                {
                    fieldValueInfo.FieldValue = fielValue.ToString();
                    await _fieldValueRepository.Update(fieldValueInfo);
                }
                else
                {
                    fieldValueInfo = new FieldValueInfo
                    {
                        FieldValue = fielValue.ToString(),
                        FlowId = taskWithDependencies.FlowId,
                        TaskId = taskId,
                        FieldId = field.Id,
                        TenantId = context.Tenant.Id
                    };
                    await _fieldValueRepository.Insert(fieldValueInfo);
                }
            }
        }

        public async Task ReplicateFieldValues(int previousTaskID, int nextTaskId, int tenantId)
        {
            var taskWithDependencies = await _taskRepository.GetByIdAndTenantId(previousTaskID, tenantId);

            if (taskWithDependencies.FieldsValues != null)
            {
                var signerTasks = taskWithDependencies
                    ?.FieldsValues
                    ?.Where(x => x.FieldValueFiles != null)
                    ?.SelectMany(x => x.FieldValueFiles)
                    ?.Where(x => x.TaskSignerFile?.TaskSigner != null)
                    ?.Select(x => x.TaskSignerFile.TaskSigner).ToList() ?? new List<TaskSignerInfo>();

                var signerTasksCloned = new Dictionary<int, TaskSignerInfo>();

                foreach (var taskGroup in signerTasks.GroupBy(x => x.Id))
                {
                    var taskCloned = taskGroup.First().AsReplicatedNewInfo(nextTaskId);

                    signerTasksCloned.Add(taskGroup.Key, taskCloned);
                    await _taskSignerRepository.Insert(taskCloned);
                }


                foreach (var fieldValue in taskWithDependencies.FieldsValues)
                {
                    var fieldValueInfo = fieldValue.AsReplicatedNewFieldValueInfo(nextTaskId, signerTasksCloned);
                    await _fieldValueRepository.Insert(fieldValueInfo);
                }
            }
        }

        public int GetId(int taskId, string componentInternalId)
        {
            var context = _contextDataService.GetContextData();
            return _fieldValueRepository
                .GetByTenant(context.Tenant.Id)
                .Where(f => f.TaskId == taskId && f.Field.ComponentInternalId == componentInternalId)
                .Select(f => f.Id)
                .FirstOrDefault();
        }

        public void GetJsonFieldValues(IList<FieldValueInfo> fieldValuesList)
        {
            // O FieldValue para o arquivo não é atualizado sempre que um upload é efetuado por problemas de concorrência, é gerado o valor atualizado através da tabela FieldValueFiles
            if (fieldValuesList == null) return;
            foreach (var item in fieldValuesList)
            {
                if (item.Field.Type == Models.Enums.FieldTypeEnum.FILE)
                {
                    var fieldValueItem = new JArray();
                    foreach (var fieldFile in item.FieldValueFiles)
                    {
                        fieldValueItem.Add(new JObject
                        {
                            { "fileKey", new JValue(fieldFile.FileKey) },
                            { "size", new JValue(fieldFile.Size) },
                            { "type", new JValue(fieldFile.Type) },
                            { "originalName", new JValue(fieldFile.Name) },
                        });
                    }
                    item.FieldValue = fieldValueItem.ToString(Formatting.None);
                }
            }
        }
    }
}