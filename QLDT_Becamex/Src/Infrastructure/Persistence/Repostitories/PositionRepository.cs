using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Models;
using QLDT_Becamex.Src.Domain.Models;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class PositionRepository : GenericRepository<Position>, IPositionRepostiory
    {

        public PositionRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

    }
}
