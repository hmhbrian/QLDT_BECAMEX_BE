
using Microsoft.AspNetCore.Identity; // Cần thêm namespace này
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Repostitories.Interfaces;
using QLDT_Becamex.Src.Repostitories.GenericRepository;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Config;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore; // Giả định ApplicationUser và UserDto của bạn ở đây

namespace QLDT_Becamex.Src.Repostitories.Implementations
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}