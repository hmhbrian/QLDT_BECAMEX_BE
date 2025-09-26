using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class MessageLogsRepository : GenericRepository<MessageLogs>, IMessageLogsRepository
    {
        public MessageLogsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}