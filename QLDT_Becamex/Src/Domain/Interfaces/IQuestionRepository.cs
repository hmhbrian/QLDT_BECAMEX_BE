using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface IQuestionRepository : IGenericRepository<Question>
    {
        public Task<int> GetMaxPositionAsync(int testId);
    }
}
