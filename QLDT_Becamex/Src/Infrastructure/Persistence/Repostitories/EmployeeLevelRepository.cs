using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class EmployeeLevelRepository : GenericRepository<EmployeeLevel>, IEmployeeLevelRepository
    {

        public EmployeeLevelRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }

    }
}
