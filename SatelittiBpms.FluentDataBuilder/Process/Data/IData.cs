namespace SatelittiBpms.FluentDataBuilder.Process.Data
{
    public interface IData
    {
        public IData Parent { get; set; }
        public T FindFirstParent<T>() where T : IData;
    }
}
