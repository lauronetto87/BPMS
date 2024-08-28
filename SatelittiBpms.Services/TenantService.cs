using Microsoft.EntityFrameworkCore.Storage;
using Satelitti.Authentication.Context.Interface;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Repository.Interfaces;
using SatelittiBpms.Services.Interfaces;
using System.Threading.Tasks;

namespace SatelittiBpms.Services
{
    public class TenantService : FluentValidationServiceBase, ITenantService
    {
        private readonly ITenantRepository _repository;
        private readonly IContextDataService<UserInfo> _contextDataService;

        public TenantService(
            ITenantRepository repository,
            IContextDataService<UserInfo> contextDataService)
        {
            _repository = repository;
            _contextDataService = contextDataService;
        }

        public TenantInfo Get(int id)
        {
            return _repository.Get(id);
        }

        public IDbContextTransaction BeginTransaction()
        {
            return _repository.BeginTransaction();
        }

        public void Insert(TenantInfo tenantInfo)
        {
            _repository.Insert(tenantInfo);
        }

        public void Update(TenantInfo tenantInfo)
        {
            _repository.Update(tenantInfo);
        }

        public ResultContent<bool> SignerIntegrationIsEnable()
        {
            var tenant = Get(_contextDataService.GetContextData().Tenant.Id);
            return new ResultContent<bool>(!string.IsNullOrWhiteSpace(tenant.SignerAccessToken), true, null);
            
        }
    }
}
