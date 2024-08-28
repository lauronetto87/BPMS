using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Storage.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class FieldValueFileService : IFieldValueFileService
    {
        private readonly IContextDataService<UserInfo> _contextDataService;
        private readonly IFieldValueFileRepository _repository;
        private readonly IStorageService _storageService;
        private readonly IFieldValueService _fieldValueService;

        public FieldValueFileService(
            IContextDataService<UserInfo> contextDataService,
            IFieldValueFileRepository repository,
            IStorageService storageService,
            IFieldValueService fieldValueService)
        {
            _contextDataService = contextDataService;
            _repository = repository;
            _storageService = storageService;
            _fieldValueService = fieldValueService;
        }

        public int CountByFileKey(string fileKey)
        {
            var context = _contextDataService.GetContextData();
            return _repository.GetByTenant(context.Tenant.Id).Count(f => f.FileKey == fileKey);
        }

        public FieldValueFileInfo Get(int taskId, string fileKey)
        {
            var context = _contextDataService.GetContextData();
            return _repository.GetByTenant(context.Tenant.Id).FirstOrDefault(f => f.FileKey == fileKey && f.FieldValue.TaskId == taskId);
        }

        public FieldValueFileInfo Get(string fileKey)
        {
            // Não pode filtrar pelo tenant esse método é chamado por usuário anônimo para download de arquivos
            return _repository.GetQuery(f => f.FileKey == fileKey).FirstOrDefault();
        }

        public async Task<ResultContent> Insert(FileToFieldValueDTO fileToFieldValue)
        {
            var context = _contextDataService.GetContextData();
            var fieldValueId = _fieldValueService.GetId(fileToFieldValue.TaskId, fileToFieldValue.ComponentInternalId);

            if (fieldValueId == 0)
            {
                throw new Exception("Não foi inserido o FieldValue antes da inserir o arquivo.");
            }

            const string folderNameTasks = "task";

            var key = await _storageService.Upload(fileToFieldValue.Stream, folderNameTasks, fileToFieldValue.FileName);

            var fieldValueFileInfo = new FieldValueFileInfo
            {
                FieldValueId = fieldValueId,
                UploadedFieldValueId = fieldValueId,
                Key = key,
                Name = fileToFieldValue.FileName,
                Size = fileToFieldValue.Stream.Length,
                Type = fileToFieldValue.FileContentType,
                CreatedDate = DateTime.UtcNow,
                CreatedByUserId = context.User.Id,
                TenantId = context.Tenant.Id,
                FileKey = Guid.NewGuid().ToString().Replace("-", ""),
            };
            await _repository.Insert(fieldValueFileInfo);

            return new ResultContent<string>(fieldValueFileInfo.FileKey, true, "");
        }

        public async Task<ResultContent> Delete(int taskId, string fileKey)
        {
            var fieldValueFile = Get(taskId, fileKey);
            if (fieldValueFile == null)
            {
                throw new KeyNotFoundException($"Registro do tipo {nameof(FieldValueFileInfo)} não encontrado para tarefa de código {taskId} arquivo de chave `{fileKey}`.");
            }

            await Delete(fieldValueFile);

            return new ResultContent(true, null);
        }

        private async Task Delete(FieldValueFileInfo fieldValueFile)
        {
            int fieldValueFileCount = CountByFileKey(fieldValueFile.FileKey);
            if (fieldValueFileCount == 1)
            {
                if (fieldValueFile.UploadedFieldValueId == fieldValueFile.FieldValueId)
                    await _storageService.Delete(fieldValueFile.Key);
            }
            await _repository.Delete(fieldValueFile);
        }

        public async Task<ResultContent> Delete(int taskId)
        {
            var context = _contextDataService.GetContextData();
            var files = _repository.GetByTenant(context.Tenant.Id).Where(f => f.FieldValue.TaskId == taskId).ToList();
            foreach (var file in files)
            {
                await Delete(file);
            }
            return new ResultContent(true, null);
        }

        public async Task Unassign(TaskInfo previousTaskInfo, TaskInfo taskInfo)
        {
            foreach (var fieldValueInfos in previousTaskInfo.FieldsValues.Where(x => x.Field.Type == Models.Enums.FieldTypeEnum.FILE))
            {
                foreach (var fieldValueFileInfo in fieldValueInfos.FieldValueFiles)
                {
                    var newFieldValueFileInfo = new FieldValueFileInfo
                    {
                        FieldValueId = taskInfo.FieldsValues.FirstOrDefault(x => x.FieldId == fieldValueInfos.FieldId).Id,
                        UploadedFieldValueId = fieldValueFileInfo.UploadedFieldValueId,
                        Key = fieldValueFileInfo.Key,
                        Name = fieldValueFileInfo.Name,
                        Size = fieldValueFileInfo.Size,
                        Type = fieldValueFileInfo.Type,
                        CreatedDate = fieldValueFileInfo.CreatedDate,
                        CreatedByUserId = fieldValueFileInfo.CreatedByUserId,
                        TenantId = fieldValueFileInfo.TenantId,
                    };

                    await _repository.Insert(newFieldValueFileInfo);
                }
            }
        }
    }
}