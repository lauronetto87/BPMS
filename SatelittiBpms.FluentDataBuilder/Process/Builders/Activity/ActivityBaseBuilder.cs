namespace SatelittiBpms.FluentDataBuilder.Process.Builders.Activity
{
    public abstract class ActivityBaseBuilder : BaseBuilder
    {
        public DataId Id { get; set; }

        public ActivityBaseBuilder(ContextBuilder context, IBaseBuilder parent, DataId id) : base(context, parent)
        {
            Id = id;
        }
    }
}