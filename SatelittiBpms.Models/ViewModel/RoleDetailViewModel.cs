using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel
{
    public class RoleDetailViewModel : RoleViewModel
    {
        public IList<int> UsersIds { get; set; }
    }
}
