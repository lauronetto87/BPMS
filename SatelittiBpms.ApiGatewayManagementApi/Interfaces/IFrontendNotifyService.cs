using System.Threading.Tasks;

namespace SatelittiBpms.ApiGatewayManagementApi.Interfaces
{
    public interface IFrontendNotifyService
    {
        Task Notify(string connectionId, object message);
    }
}
