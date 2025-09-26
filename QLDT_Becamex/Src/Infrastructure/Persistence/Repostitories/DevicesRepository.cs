using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class DevicesRepository : GenericRepository<Device>, IDevicesRepository
    {
        public DevicesRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}