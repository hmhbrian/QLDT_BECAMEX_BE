using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        public Task<Lesson?> GetByIdAsync(int id);
    }
}
