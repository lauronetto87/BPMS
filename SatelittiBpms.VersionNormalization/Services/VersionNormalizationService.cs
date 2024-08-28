using SatelittiBpms.Models.Infos;
using SatelittiBpms.VersionNormalization.Interfaces;
using System.Collections.Generic;

namespace SatelittiBpms.VersionNormalization.Services
{
    public class VersionNormalizationService : IVersionNormalizationService
    {
        private readonly IVersionNormalizationRepository _repository;

        public VersionNormalizationService(
            IVersionNormalizationRepository repository)
        {
            _repository = repository;
        }

        public void AddNormalization(string normalization)
        {
            var tenantInfo = new VersionNormalizationInfo()
            {
                Normalization = normalization
            };

            _repository.Insert(tenantInfo);
        }

        public void Insert(VersionNormalizationInfo tenantInfo)
        {
            _repository.Insert(tenantInfo);
        }

        public List<VersionNormalizationInfo> ListAll()
        {
            return _repository.ListAll();
        }
    }
}
