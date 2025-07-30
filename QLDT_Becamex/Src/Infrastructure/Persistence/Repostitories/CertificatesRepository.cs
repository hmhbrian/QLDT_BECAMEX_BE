using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class CertificatesRepository : GenericRepository<Certificates>, ICertificatesRepository
    {
        public CertificatesRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
