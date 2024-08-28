using System.Threading.Tasks;
using System.Xml;

namespace SatelittiBpms.Workflow.Interfaces
{
    public interface IXmlDiagramParseService
    {
        Task<string> Parse(XmlNode xmlNode, int processId, int version, int processVersionId, int tenantId);
    }
}
