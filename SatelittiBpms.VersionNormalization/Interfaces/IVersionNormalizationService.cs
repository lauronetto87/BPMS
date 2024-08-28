using SatelittiBpms.Models.Infos;
using System.Collections.Generic;

namespace SatelittiBpms.VersionNormalization.Interfaces
{
    public interface IVersionNormalizationService
    {
        void AddNormalization(string normalization);
        void Insert(VersionNormalizationInfo versionNormalization);
        List<VersionNormalizationInfo> ListAll();
    }
}
