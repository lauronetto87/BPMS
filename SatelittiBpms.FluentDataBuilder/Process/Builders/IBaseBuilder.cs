using SatelittiBpms.FluentDataBuilder.Process.Data;

namespace SatelittiBpms.FluentDataBuilder.Process.Builders
{
    public interface IBaseBuilder
    {
        public ProcessVersionData MakeProcess();
        internal IData LastBuild { get; set; }
    }
}
