using NUnit.Framework;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Models.Result;
using SatelittiBpms.Services.Interfaces;
using SatelittiBpms.Test;
using System.Threading.Tasks;

namespace SatelittiBpms.Tests.Executors
{
    public static class ProcessVersionExecutor
    {
        public static async Task<ProcessVersionInfo> Save(MockServices _mockServices, Models.DTO.ProcessVersionDTO processVersionDTO)
        {
            var result = await _mockServices.GetService<IProcessVersionService>().Save(processVersionDTO);
            Assert.IsTrue(result.Success);
            var processVersionId = ResultContent<int>.GetValue(result);
            return _mockServices.GetService<IProcessVersionService>().Get(processVersionId).Result.Value;
        }

        public static async Task<ResultContent> SaveAndReturnResult(MockServices _mockServices, Models.DTO.ProcessVersionDTO processVersionDTO)
        {
            return await _mockServices.GetService<IProcessVersionService>().Save(processVersionDTO);
        }
    }
}
