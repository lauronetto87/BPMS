namespace SatelittiBpms.FluentDataBuilder
{
    public class DataId
    {
        public string InternalId { get; internal set; }

        public DataId()
        {
            InternalId = new Bogus.Faker().Database.Random.AlphaNumeric(6);
        }

        public DataId(string internalId)
        {
            InternalId = internalId;
        }
    }
}
