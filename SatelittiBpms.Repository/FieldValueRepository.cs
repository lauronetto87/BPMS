using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Repository
{
    public class FieldValueRepository : AbstractRepositoryBase<FieldValueInfo>, IFieldValueRepository
    {
        private readonly IServiceProvider _serviceProvider;

        public FieldValueRepository(DbContext context, IServiceProvider serviceProvider) : base(context)
        {
            this._serviceProvider = serviceProvider;
        }

        public async Task UpdateFieldValues(List<FieldValueInfo> fieldValueInfoList)
        {
            foreach (FieldValueInfo item in fieldValueInfoList)
            {
                var fieldValueInfo = this.GetByTaskFieldTenant(item.TaskId, item.FieldId, item.TenantId);

                if (fieldValueInfo != null)
                {
                    fieldValueInfo.FieldValue = item.FieldValue;
                    await Update(fieldValueInfo);
                }
                else
                {
                    await Insert(item);
                }
            }
        }

        public FieldValueInfo GetByTaskFieldTenant(int taskId, int fieldId, int? tenantId)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<DbContext>();
                return _context.Set<FieldValueInfo>().Where(x => x.TaskId == taskId).FirstOrDefault(x => x.TenantId == tenantId && x.FieldId == fieldId);
            }
        }
    }
}
