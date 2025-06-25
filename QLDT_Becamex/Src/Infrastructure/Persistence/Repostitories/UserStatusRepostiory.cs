using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;


namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class UserStatusRepostiory : GenericRepository<UserStatus>, IUserStatusRepostiory
    {
        public UserStatusRepostiory(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
