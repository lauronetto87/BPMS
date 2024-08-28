using SatelittiBpms.FluentDataBuilder.Process.Builders;
using SatelittiBpms.FluentDataBuilder.Process.Data;

namespace SatelittiBpms.Test.Extensions
{
    public static class BaseBuilderExtension
    {
        public static ProcessVersionData EndCreateProcess(this IBaseBuilder bilder)
        {
            return bilder.MakeProcess();
        }
    }
}
