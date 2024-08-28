using SatelittiBpms.Models.Infos;
using System.Collections.Generic;

namespace SatelittiBpms.VersionNormalization.Interfaces
{
    public interface IVersionNormalizationRepository
    {
        void Insert(VersionNormalizationInfo versionNormalization);
        List<VersionNormalizationInfo> ListAll();
    }
}
