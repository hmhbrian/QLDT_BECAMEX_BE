using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class TopicsRepository : GenericRepository<Topics>, ITopicsRepository
    {
        public TopicsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}