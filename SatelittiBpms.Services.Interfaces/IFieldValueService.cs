using Satelitti.Authentication.Result;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IFieldValueService
    {
        dynamic GetFormatedFormData(IList<FieldValueInfo> fieldValuesList);
        Task UpdateFieldValues(int taskId, dynamic formData);
        Task ReplicateFieldValues(int previousTaskID, int nextTaskId, int tenantId);
        int GetId(int taskId, string componentInternalId);
        void GetJsonFieldValues(IList<FieldValueInfo> fieldValuesList);
    }
}
