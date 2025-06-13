using Microsoft.AspNetCore.Identity;
using QLDT_Becamex.Src.Dtos;
using QLDT_Becamex.Src.Models;
using QLDT_Becamex.Src.Repostitories.GenericRepository;
using System.Linq.Expressions;

namespace QLDT_Becamex.Src.Repostitories.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {

    }
}