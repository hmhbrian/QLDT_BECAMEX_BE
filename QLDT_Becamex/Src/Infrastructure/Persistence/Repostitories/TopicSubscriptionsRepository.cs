using QLDT_Becamex.Src.Domain.Entities;
using QLDT_Becamex.Src.Domain.Interfaces;

namespace QLDT_Becamex.Src.Infrastructure.Persistence.Repostitories
{
    public class TopicSubscriptionsRepository : GenericRepository<TopicSubscriptions>, ITopicSubscriptionsRepository
    {
        public TopicSubscriptionsRepository(ApplicationDbContext dbContext) : base(dbContext)
        {

        }
    }
}