using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;


namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class UserRepository : GenericRepository<ApplicationUser>, IUserRepository
    {
        protected readonly ApplicationDbContext _dbContext;
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<ApplicationUser> GetFlexible(
            Expression<Func<ApplicationUser, bool>> predicate,
            Func<IQueryable<ApplicationUser>, IOrderedQueryable<ApplicationUser>>? orderBy,
            int? page = null,
            int? pageSize = null,
            bool asNoTracking = false,
            Expression<Func<ApplicationUser, object>>[]? includes = null)
        {
            var query = _dbContext.Users.AsQueryable();

            if (asNoTracking)
                query = query.AsNoTracking();

            if (predicate != null)
                query = query.Where(predicate);

            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = query.Include(include);
                }
            }

            if (orderBy != null)
                query = orderBy(query);

            if (page.HasValue && pageSize.HasValue)
            {
                int skip = (page.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return query;
        }
    }
}