using QLDT_Becamex.Src.Domain.Entities;
using System.Linq.Expressions;


namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface IUserRepository : IGenericRepository<ApplicationUser>
    {
        public IQueryable<ApplicationUser> GetFlexible(
            Expression<Func<ApplicationUser, bool>> predicate,
            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>? orderBy,
            int? page = null,
            int? pageSize = null,
            bool asNoTracking = false,
            Expression<Func<ApplicationUser, object>>[]? includes = null);
    }
}