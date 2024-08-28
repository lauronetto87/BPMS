namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public abstract class BaseData : IData
    {
        public IData Parent { get; set; }

        public T FindFirstParent<T>() where T : IData
        {
            return Parent is T t ? t : Parent.FindFirstParent<T>();
        }
    }
}
