using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SatelittiBpms.Authentication.Models;
using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.ViewModel;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Storage.Exceptions;
using SatelittiBpms.Storage.Interfaces;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Controllers
{
    public class TaskController : BpmsApiControllerBase
    {
        private readonly ITaskService _taskService;
        private readonly IFieldValueFileService _fieldValueFileService;
        private readonly IStorageService _storageService;
        private readonly ITaskSignerService _taskSignerService;
        private readonly ISignerIntegrationService _signerIntegrationService;

        public TaskController(
            ILogger<TaskController> logger,
            ITaskService taskService,
            IFieldValueFileService fieldValueFileService,
            IStorageService storageService,
            ITaskSignerService taskSignerService,
            ISignerIntegrationService signerIntegrationService) : base(logger)
        {
            _taskService = taskService;
            _fieldValueFileService = fieldValueFileService;
            _storageService = storageService;
            _taskSignerService = taskSignerService;
            _signerIntegrationService = signerIntegrationService;
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = Policies.ADMINISTRATORS)]
        public async Task<ActionResult> Get(int id)
        {
            return await HandleExceptionAsync(async () => await _taskService.Get(id));
        }

        [HttpPost]
        [Route("listTasks")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult<FlowQueryViewModel>> ListTasks([FromBody] TaskFilterDTO filters)
        {
            return await HandleExceptionAsync(async () => await _taskService.ListTasks(filters));
        }

        [HttpPost]
        [Route("listTaskGroup")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult<ProcessListingViewModel>> GetTaskGroup([FromBody] TaskFilterDTO filters)
        {
            return await HandleExceptionAsync(async () => await _taskService.ListTasksGroup(filters));
        }

        [HttpPut]
        [Route("assign")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult> Assign([FromBody] int id)
        {
            return await HandleExceptionAsync(async () => await _taskService.Assign(id));
        }

        [HttpPut]
        [Route("unassign")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult> Unassign([FromBody] int taskId)
        {
            return await HandleExceptionAsync(async () => await _taskService.Unassign(taskId));
        }

        [HttpGet]
        [Route("toexecute/{id}")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult> GetTaskData(int id)
        {
            return await HandleExceptionAsync(async () => await _taskService.GetTaskToExecute(id));
        }

        [HttpPost]
        [Route("nextTask")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult> NextTask([FromBody] NextStepDTO nextStepDTO)
        {
            return await HandleExceptionAsync(async () => await _taskService.NextTask(nextStepDTO));
        }

        [HttpPost]
        [Route("details")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult<TaskDetailsViewModel>> GetDetailsById([FromBody] int taskId)
        {
            return await HandleExceptionAsync(async () => await _taskService.GetDetailsById(taskId));
        }

        [HttpPost]
        [Route("total")]
        [Authorize(Policy = Policies.OBSERVERS)]
        public async Task<ActionResult<TaskCounterViewModel>> Total([FromBody] TaskFilterDTO filters)
        {
            return await HandleExceptionAsync(async () => await _taskService.GetCounterTask(filters));
        }


        [HttpPost]
        [Route("file")]
        [Authorize(Policy = Policies.PUBLISHERS)]
        public async Task<ActionResult<string>> UploadFile([FromForm(Name = "file")] IFormFileCollection files)
        {
            if ((files?.Count ?? 0) == 0)
            {
                return BadRequest("O arquivo não foi enviado pelo método multipart/form-data com a propriedade de nome `file`");
            }
            if (files.Count > 1)
            {
                return BadRequest("Esse método não suporta o upload de vários arquivos.");
            }

            return await HandleExceptionAsync(async () =>
            {
                var file = files[0];
                using var stream = file.OpenReadStream();
                var fileToFieldValueDTO = new FileToFieldValueDTO
                {
                    ComponentInternalId = HttpContext.Request.Form["componentInternalId"].First(),
                    FileName = file.FileName,
                    FileContentType = file.ContentType,
                    Stream = stream,
                    TaskId = int.Parse(HttpContext.Request.Form["taskId"].First()),
                };
                return await _fieldValueFileService.Insert(fileToFieldValueDTO);
            });
        }

        [HttpDelete]
        [Route("file")]
        [Authorize(Policy = Policies.PUBLISHERS)]
        public async Task<ActionResult> DeleteFile([FromQuery] int taskId, [FromQuery] string fileKey)
        {
            return await HandleExceptionAsync(async () =>
            {
                return await _fieldValueFileService.Delete(taskId, fileKey);
            });
        }

        [HttpGet]
        [Route("file/{fileKey}")]
        [AllowAnonymous()]
        public async Task<ActionResult> GetFile(string fileKey)
        {
            fileKey = System.Net.WebUtility.UrlDecode(fileKey);

            var file = _fieldValueFileService.Get(fileKey);
            System.Net.Mime.ContentDisposition cd = new()
            {
                FileName = file.Name,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");
            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    using var storageStream = await _storageService.Download(file.Key);
                    storageStream.CopyTo(memoryStream);
                    return File(memoryStream.ToArray(), file.Type);
                }
            }
            catch (StorageFileNotExistException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet]
        [Route("file/{fileKey}/{version}")]
        [AllowAnonymous()]
        public async Task<ActionResult> GetFileSigned(string fileKey, string version)
        {
            if (version?.ToLower() != "signed" && version?.ToLower() != "print")
            {
                return NotFound();
            }

            FileViewModel file;

            if (version?.ToLower() == "signed")
            {
                file = await _signerIntegrationService.GetFileSigned(fileKey);
            }
            else
            {
                file = await _signerIntegrationService.GetFilePrint(fileKey);
            }

            System.Net.Mime.ContentDisposition cd = new()
            {
                FileName = file.FileName,
                Inline = true,
            };
            Response.Headers.Add("Content-Disposition", cd.ToString());
            Response.Headers.Add("X-Content-Type-Options", "nosniff");

            using var memoryStream = new MemoryStream();
            file.Content.CopyTo(memoryStream);
            return File(memoryStream.ToArray(), file.ContentType);
        }

        [HttpPost]
        [Route("ActionPerformedOnSigner")]
        [AllowAnonymous()]
        public async Task<IActionResult> ActionPerformedOnSigner([FromBody] ActionPerformedOnSignerDTO actionPerformedOnSigner)
        {
            return await HandleExceptionAsync(async () => await _taskSignerService.ActionPerformedOnsigner(actionPerformedOnSigner));
        }
    }
}
