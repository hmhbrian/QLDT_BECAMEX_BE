using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class TypeDocumentRepository : GenericRepository<TypeDocument>, ITypeDocumentRepository
    {
        public TypeDocumentRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }
    }
}
