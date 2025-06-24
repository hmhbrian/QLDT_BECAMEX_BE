using QLDT_Becamex.Src.Domain.Interfaces;
using QLDT_Becamex.Src.Domain.Models;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class DepartmentRepository : GenericRepository<Department>, IDepartmentRepository
    {

        public DepartmentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}
