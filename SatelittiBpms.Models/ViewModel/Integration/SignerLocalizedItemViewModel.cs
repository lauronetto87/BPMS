using System.Collections.Generic;

namespace SatelittiBpms.Models.ViewModel.Integration
{
    public class SignerLocalizedItemViewModel
    {
        public string Language { get; set; }
        public List<SignerItemViewModel> Items { get; set; }
    }
}
