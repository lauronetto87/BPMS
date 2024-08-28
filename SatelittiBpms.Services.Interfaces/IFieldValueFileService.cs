using SatelittiBpms.Models.DTO;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IFieldValueFileService
    {
        FieldValueFileInfo Get(int taskId, string fileKey);
        FieldValueFileInfo Get(string fileKey);
        int CountByFileKey(string fileKey);
        Task<ResultContent> Insert(FileToFieldValueDTO fileToFieldValue);
        Task<ResultContent> Delete(int taskId, string fileKey);
        Task<ResultContent> Delete(int taskId);
        Task Unassign(TaskInfo previousTaskInfo, TaskInfo taskInfo);
    }
}
