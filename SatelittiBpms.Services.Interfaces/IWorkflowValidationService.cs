using SatelittiBpms.Models.Result;
using System.Xml;

namespace SatelittiBpms.Services.Interfaces
{
    public interface IWorkflowValidationService
    {
        ResultContent Validate(XmlNode xmlProcessNode);
    }
}
