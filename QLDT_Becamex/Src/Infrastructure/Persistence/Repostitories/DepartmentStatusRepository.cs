using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class DepartmentStatusRepository : GenericRepository<DepartmentStatus>, IDepartmentStatusRepository
    {
        public DepartmentStatusRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
