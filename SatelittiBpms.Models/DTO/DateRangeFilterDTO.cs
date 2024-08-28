using System;

namespace SatelittiBpms.Models.DTO
{
    public class DateRangeFilterDTO
    {
        public DateTime? BeginDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TimezoneOffset { get; set; }
    }
}
