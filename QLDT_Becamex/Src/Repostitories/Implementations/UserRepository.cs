
using Microsoft.AspNetCore.Identity; // Cần thêm namespace này
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Repostitories.Interfaces;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Config; // Giả định ApplicationUser và UserDto của bạn ở đây

namespace QLDT_Becamex.Src.Repostitories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public UserRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }


    }

}