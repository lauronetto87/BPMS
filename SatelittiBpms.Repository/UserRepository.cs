using Microsoft.EntityFrameworkCore;
using SatelittiBpms.Models.Infos;
using SatelittiBpms.Repository.Interfaces;

namespace SatelittiBpms.Repository
{
    public class UserRepository : AbstractRepositoryBase<UserInfo>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context) { }

    }
}
