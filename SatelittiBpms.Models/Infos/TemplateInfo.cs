using Satelitti.Model;

namespace SatelittiBpms.Models.Infos
{
    public class TemplateInfo : BaseInfo
    {
        #region Properties        
        public string Name { get; set; }
        public string Description { get; set; }
        public string DescriptionFlow { get; set; }
        public string DiagramContent { get; set; }
        public string FormContent { get; set; }
        #endregion       
    }
}
