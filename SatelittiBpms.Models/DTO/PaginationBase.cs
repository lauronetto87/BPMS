using System.Collections.Generic;

namespace SatelittiBpms.Models.DTO
{
    public class PaginationBase
    {
        public int Take { get; set; }

        public int Skip { get; set; }

        public int TotalByQuery { get; set; }

        public List<int> IgnoreListId { get; set; }
    }
}
