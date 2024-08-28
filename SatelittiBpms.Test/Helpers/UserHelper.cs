using SatelittiBpms.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace SatelittiBpms.Test.Helpers
{
    public static class UserHelper
    {
        public async static Task<string> GetNameOfUser(int? userId, MockServices mockServices)
        {
            if (userId == null || userId == 0)
            {
                return "";
            }
            var users = await mockServices.GetService<IUserService>().ListUsersSuite();
            return users.First(u => u.Id == userId).Name;
        }
    }
}
