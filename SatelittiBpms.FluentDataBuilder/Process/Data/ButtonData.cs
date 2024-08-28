namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public class ButtonData
    {
        public string Description { get; set; }
        public ExclusiveGatewayBranchData Branch { get; internal set; }
    }
}
