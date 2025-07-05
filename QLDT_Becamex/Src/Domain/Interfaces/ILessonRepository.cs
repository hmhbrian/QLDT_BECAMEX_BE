using QLDT_Becamex.Src.Domain.Entities;

namespace QLDT_Becamex.Src.Domain.Interfaces
{
    public interface ILessonRepository : IGenericRepository<Lesson>
    {
        public Task<Lesson?> GetByIdAsync(int id);
        public Task<int> GetMaxPositionAsync(string courseId);

        public Task UpdatePositionAsync(string courseId, int fromPosition, int toPosition, int offset);
        public Task ReorderPositionsAsync(string courseId, CancellationToken cancellationToken);
    }
}
