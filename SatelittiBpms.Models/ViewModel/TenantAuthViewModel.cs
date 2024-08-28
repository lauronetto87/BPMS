namespace SatelittiBpms.Models.ViewModel
{
    public class SuiteUserViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Tenant { get; set; }
        public float? Timezone { get; set; }
        public string Mail { get; set; }
        public bool Admin { get; set; }
        public string SuiteToken { get; set; }
    }
}
