using QLDT_Becamex.Src.Domain.Models;
using QLDT_Becamex.Src.Infrastructure.Persistence.GenericRepository;
using QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories.Interfaces;


namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories.Implementations
{
    public class UserStatusRepostiory : GenericRepository<UserStatus>, IUserStatusRepostiory
    {
        public UserStatusRepostiory(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
