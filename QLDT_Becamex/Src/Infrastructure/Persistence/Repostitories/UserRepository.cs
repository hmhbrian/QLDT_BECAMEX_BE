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
            int page,
            bool asNoTracking,
            Expression<Func<ApplicationUser, object>>[]? includes)
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

            // Phân trang
            int limit = 10; // Giả sử limit mặc định
            query = query.Skip((page - 1) * limit).Take(limit);

            return query;
        }
    }
}